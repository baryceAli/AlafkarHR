namespace Procurement.Procurement.Features;

public record CreateProcurementDocumentCommand(ProcurementDocumentKind Kind, ProcurementDocumentDto Document) : ICommand<CreateProcurementDocumentResult>;
public record CreateProcurementDocumentResult(Guid Id);
public record UpdateProcurementDocumentCommand(Guid Id, ProcurementDocumentKind Kind, ProcurementDocumentDto Document) : ICommand;
public record RemoveProcurementDocumentCommand(Guid Id, ProcurementDocumentKind Kind) : ICommand;
public record GetProcurementDocumentByIdQuery(Guid Id, ProcurementDocumentKind Kind) : IQuery<GetProcurementDocumentByIdResult>;
public record GetProcurementDocumentByIdResult(ProcurementDocumentDto Document);
public record GetProcurementDocumentsQuery(ProcurementDocumentKind Kind, Guid? CompanyId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetProcurementDocumentsResult>;
public record GetProcurementDocumentsResult(PaginatedResult<ProcurementDocumentDto> Documents);
public record GetProcurementDashboardQuery(Guid? CompanyId) : IQuery<GetProcurementDashboardResult>;
public record GetProcurementDashboardResult(ProcurementDashboardDto Dashboard);
public record ChangeProcurementDocumentStatusCommand(Guid Id, ProcurementDocumentKind Kind, string Action) : ICommand;

public class CreateProcurementDocumentCommandValidator : AbstractValidator<CreateProcurementDocumentCommand>
{
    public CreateProcurementDocumentCommandValidator()
    {
        RuleFor(x => x.Document.Number)
            .NotEmpty()
            .When(x => x.Kind is not ProcurementDocumentKind.PurchaseRequest
                and not ProcurementDocumentKind.RequestForQuotation
                and not ProcurementDocumentKind.SupplierQuotation
                and not ProcurementDocumentKind.PurchaseOrder
                and not ProcurementDocumentKind.GoodsReceipt
                and not ProcurementDocumentKind.SupplierInvoice);
        RuleFor(x => x.Document.Number).MaximumLength(50);
        RuleFor(x => x.Document.CompanyId).NotEmpty();
        RuleForEach(x => x.Document.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateProcurementDocumentCommand, CreateProcurementDocumentResult>
{
    public async Task<CreateProcurementDocumentResult> Handle(CreateProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId(httpContextAccessor);
        await EnsureCanMutateBranchAsync(sender, command.Document.CompanyId, command.Document.BranchId, cancellationToken);
        await EnsureWarehousesMatchBranchAsync(sender, command.Document, cancellationToken);
        var sourceDocument = await EnsureValidSourceDocumentAsync(dbContext, sender, command.Kind, command.Document, cancellationToken);
        command.Document.Kind = command.Kind;
        if (command.Kind == ProcurementDocumentKind.PurchaseRequest)
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "PR", 4, "purchase request", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.RequestForQuotation)
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "RFQ", 5, "request for quotation", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.SupplierQuotation)
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "SQ", 5, "supplier quotation", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.PurchaseOrder)
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "PO", 5, "purchase order", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.GoodsReceipt)
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "GR", 5, "goods receipt", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.SupplierInvoice)
        {
            await ProcurementReceiptRules.EnsureSupplierInvoiceDoesNotExceedReceivedQuantitiesAsync(dbContext, command.Document, cancellationToken);
            return await CreateNumberedDocumentAsync(command, userId, sourceDocument, "INV", 5, "supplier invoice", cancellationToken);
        }

        var document = ProcurementDocumentFactory.Create(command.Kind, command.Document, userId);
        await dbContext.ProcurementDocuments.AddAsync(document, cancellationToken);
        MarkSourcePurchaseRequestConverted(sourceDocument, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementDocumentResult(document.Id);
    }

    private async Task<CreateProcurementDocumentResult> CreateNumberedDocumentAsync(
        CreateProcurementDocumentCommand command,
        string userId,
        ProcurementDocument? sourceDocument,
        string prefixCode,
        int sequenceDigits,
        string documentName,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            command.Document.Number = await GenerateDocumentNumberAsync(
                command.Kind,
                command.Document.CompanyId,
                command.Document.DocumentDate,
                prefixCode,
                sequenceDigits,
                cancellationToken);
            var document = ProcurementDocumentFactory.Create(command.Kind, command.Document, userId);
            await dbContext.ProcurementDocuments.AddAsync(document, cancellationToken);
            MarkSourcePurchaseRequestConverted(sourceDocument, userId);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                return new CreateProcurementDocumentResult(document.Id);
            }
            catch (DbUpdateException ex) when (IsProcurementNumberUniqueConflict(ex))
            {
                dbContext.Entry(document).State = EntityState.Detached;

                if (attempt == maxAttempts)
                    throw new Exception($"Could not generate a unique {documentName} number. Please try again.");
            }
        }

        throw new Exception($"Could not generate a unique {documentName} number. Please try again.");
    }

    private async Task<string> GenerateDocumentNumberAsync(
        ProcurementDocumentKind kind,
        Guid companyId,
        DateTime documentDate,
        string prefixCode,
        int sequenceDigits,
        CancellationToken cancellationToken)
    {
        var date = documentDate == default ? DateTime.UtcNow : documentDate;
        var prefix = $"{prefixCode}-{date:yyMM}-";
        var numbers = await dbContext.ProcurementDocuments.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Kind == kind
                && x.CompanyId == companyId
                && x.Number.StartsWith(prefix))
            .Select(x => x.Number)
            .ToListAsync(cancellationToken);

        var nextSequence = numbers
            .Select(number => ParseSequence(number, prefix))
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{nextSequence.ToString($"D{sequenceDigits}")}";
    }

    private static int ParseSequence(string number, string prefix)
    {
        if (string.IsNullOrWhiteSpace(number) || !number.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return 0;

        return int.TryParse(number[prefix.Length..], out var sequence) ? sequence : 0;
    }

    private static bool IsProcurementNumberUniqueConflict(DbUpdateException exception)
    {
        var message = exception.ToString();
        return message.Contains("IX_ProcurementDocuments_CompanyId_Kind_Number", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Cannot insert duplicate key", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    internal static async Task<ProcurementDocument?> EnsureValidSourceDocumentAsync(
        ProcurementDbContext dbContext,
        ISender sender,
        ProcurementDocumentKind kind,
        ProcurementDocumentDto document,
        CancellationToken cancellationToken)
    {
        if (!document.SourceDocumentId.HasValue)
            return null;

        var source = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == document.SourceDocumentId.Value, cancellationToken)
            ?? throw new NotFoundException("Source procurement document", document.SourceDocumentId.Value);

        if (source.CompanyId != document.CompanyId)
            throw new BadRequestException("Source procurement document must belong to the same company.");

        await EnsureCanReadBranchAsync(sender, source.CompanyId, source.BranchId, cancellationToken);
        await EnsureCanMutateBranchAsync(sender, source.CompanyId, source.BranchId, cancellationToken);

        if (!IsValidSourceDocument(kind, source))
            throw new BadRequestException($"Invalid source document '{source.Number}' for {kind}.");

        return source;
    }

    internal static void MarkSourcePurchaseRequestConverted(ProcurementDocument? sourceDocument, string userId)
    {
        if (sourceDocument?.Kind == ProcurementDocumentKind.PurchaseRequest
            && IsStatus(sourceDocument.Status, PurchaseRequestStatus.Approved.ToString()))
            sourceDocument.ChangeStatus(PurchaseRequestStatus.Converted.ToString(), userId);
    }

    private static bool IsValidSourceDocument(ProcurementDocumentKind kind, ProcurementDocument source) =>
        kind switch
        {
            ProcurementDocumentKind.RequestForQuotation =>
                source.Kind == ProcurementDocumentKind.PurchaseRequest
                && (IsStatus(source.Status, PurchaseRequestStatus.Approved.ToString())
                    || IsStatus(source.Status, PurchaseRequestStatus.Converted.ToString())),
            ProcurementDocumentKind.SupplierQuotation =>
                source.Kind == ProcurementDocumentKind.RequestForQuotation
                && IsStatus(source.Status, RequestForQuotationStatus.Sent.ToString()),
            ProcurementDocumentKind.PurchaseOrder =>
                (source.Kind == ProcurementDocumentKind.SupplierQuotation
                    && IsStatus(source.Status, SupplierQuotationStatus.Accepted.ToString()))
                || (source.Kind == ProcurementDocumentKind.PurchaseRequest
                    && (IsStatus(source.Status, PurchaseRequestStatus.Approved.ToString())
                        || IsStatus(source.Status, PurchaseRequestStatus.Converted.ToString()))),
            ProcurementDocumentKind.GoodsReceipt =>
                source.Kind == ProcurementDocumentKind.PurchaseOrder
                && (IsStatus(source.Status, PurchaseOrderStatus.Sent.ToString())
                    || IsStatus(source.Status, PurchaseOrderStatus.PartiallyReceived.ToString())),
            ProcurementDocumentKind.SupplierInvoice =>
                (source.Kind == ProcurementDocumentKind.PurchaseOrder
                    && (IsStatus(source.Status, PurchaseOrderStatus.PartiallyReceived.ToString())
                        || IsStatus(source.Status, PurchaseOrderStatus.Received.ToString())
                        || IsStatus(source.Status, PurchaseOrderStatus.Closed.ToString())))
                || (source.Kind == ProcurementDocumentKind.GoodsReceipt
                    && IsStatus(source.Status, PostedDocumentStatus.Posted.ToString())),
            _ => true
        };

    private static bool IsStatus(string? status, string expected) =>
        string.Equals(status, expected, StringComparison.OrdinalIgnoreCase);

    internal static async Task EnsureCanMutateBranchAsync(ISender sender, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(access, branchId))
            throw new ForbiddenException("You do not have permission to change procurement data in this branch scope.");
    }

    internal static async Task EnsureCanReadBranchAsync(ISender sender, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(access, branchId))
            throw new ForbiddenException("You do not have permission to view procurement data in this branch scope.");
    }

    internal static async Task<IQueryable<ProcurementDocument>> ApplyBranchAccessAsync(ISender sender, IQueryable<ProcurementDocument> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, branchId))
            throw new ForbiddenException("You do not have permission to filter procurement data by this branch.");

        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => x.BranchId == branchId.Value) : query;

        return branchId.HasValue
            ? query.Where(x => x.BranchId == null || x.BranchId == branchId.Value)
            : query.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    internal static async Task EnsureWarehousesMatchBranchAsync(ISender sender, ProcurementDocumentDto document, CancellationToken cancellationToken)
    {
        if (!document.BranchId.HasValue)
            return;

        if (document.WarehouseId.HasValue)
            await sender.Send(new EnsureWarehouseBranchScopeQuery(document.CompanyId, document.WarehouseId.Value, document.BranchId.Value), cancellationToken);

        foreach (var warehouseId in document.Lines.Select(x => x.WarehouseId).Where(x => x.HasValue).Select(x => x!.Value).Distinct())
            await sender.Send(new EnsureWarehouseBranchScopeQuery(document.CompanyId, warehouseId, document.BranchId.Value), cancellationToken);
    }
}

public class UpdateProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateProcurementDocumentCommand>
{
    public async Task<Unit> Handle(UpdateProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.Kind == command.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", command.Id);

        if (!ProcurementDocumentLockRules.CanEditDocument(command.Kind, document.Status))
            throw new BadRequestException(ProcurementDocumentLockRules.LockedEditMessage(command.Kind, document.Status));
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, document.CompanyId, document.BranchId, cancellationToken);
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, command.Document.CompanyId, command.Document.BranchId, cancellationToken);
        await CreateProcurementDocumentHandler.EnsureWarehousesMatchBranchAsync(sender, command.Document, cancellationToken);
        var sourceDocument = await CreateProcurementDocumentHandler.EnsureValidSourceDocumentAsync(dbContext, sender, command.Kind, command.Document, cancellationToken);

        command.Document.Kind = command.Kind;
        if (command.Kind is ProcurementDocumentKind.PurchaseRequest
            or ProcurementDocumentKind.RequestForQuotation
            or ProcurementDocumentKind.SupplierQuotation
            or ProcurementDocumentKind.PurchaseOrder
            or ProcurementDocumentKind.GoodsReceipt
            or ProcurementDocumentKind.SupplierInvoice)
            command.Document.Number = document.Number;

        if (command.Kind == ProcurementDocumentKind.SupplierInvoice)
            await ProcurementReceiptRules.EnsureSupplierInvoiceDoesNotExceedReceivedQuantitiesAsync(dbContext, command.Document, cancellationToken);

        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        document.Update(command.Document, userId);
        CreateProcurementDocumentHandler.MarkSourcePurchaseRequestConverted(sourceDocument, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class RemoveProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<RemoveProcurementDocumentCommand>
{
    public async Task<Unit> Handle(RemoveProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.Kind == command.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", command.Id);

        if (!ProcurementDocumentLockRules.CanDeleteDocument(command.Kind, document.Status))
            throw new BadRequestException(ProcurementDocumentLockRules.LockedDeleteMessage(command.Kind, document.Status));
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, document.CompanyId, document.BranchId, cancellationToken);

        document.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetProcurementDocumentByIdHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetProcurementDocumentByIdQuery, GetProcurementDocumentByIdResult>
{
    public async Task<GetProcurementDocumentByIdResult> Handle(GetProcurementDocumentByIdQuery query, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.AsNoTracking().Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == query.Id && x.Kind == query.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", query.Id);
        await CreateProcurementDocumentHandler.EnsureCanReadBranchAsync(sender, document.CompanyId, document.BranchId, cancellationToken);

        return new GetProcurementDocumentByIdResult(document.ToDto());
    }
}

public class GetProcurementDocumentsHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetProcurementDocumentsQuery, GetProcurementDocumentsResult>
{
    public async Task<GetProcurementDocumentsResult> Handle(GetProcurementDocumentsQuery query, CancellationToken cancellationToken)
    {
        var documents = dbContext.ProcurementDocuments.AsNoTracking()
            .Where(x => x.Kind == query.Kind);

        if (query.CompanyId.HasValue)
        {
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);
            documents = await CreateProcurementDocumentHandler.ApplyBranchAccessAsync(sender, documents, query.CompanyId.Value, null, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
            documents = documents.Where(x => x.Number.Contains(query.SearchText) || (x.SupplierName != null && x.SupplierName.Contains(query.SearchText)));

        var count = await documents.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await documents.Include(x => x.Lines)
            .OrderByDescending(x => x.DocumentDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetProcurementDocumentsResult(new PaginatedResult<ProcurementDocumentDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }
}

public class GetProcurementDashboardHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetProcurementDashboardQuery, GetProcurementDashboardResult>
{
    public async Task<GetProcurementDashboardResult> Handle(GetProcurementDashboardQuery query, CancellationToken cancellationToken)
    {
        var documents = dbContext.ProcurementDocuments.AsNoTracking();
        if (query.CompanyId.HasValue)
        {
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);
            documents = await CreateProcurementDocumentHandler.ApplyBranchAccessAsync(sender, documents, query.CompanyId.Value, null, cancellationToken);
        }

        return new GetProcurementDashboardResult(new ProcurementDashboardDto
        {
            PurchaseRequests = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.PurchaseRequest, cancellationToken),
            RequestsForQuotation = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.RequestForQuotation, cancellationToken),
            SupplierQuotations = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.SupplierQuotation, cancellationToken),
            PurchaseOrders = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.PurchaseOrder, cancellationToken),
            GoodsReceipts = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.GoodsReceipt, cancellationToken),
            PurchaseReturns = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.PurchaseReturn, cancellationToken),
            SupplierInvoices = await documents.CountAsync(x => x.Kind == ProcurementDocumentKind.SupplierInvoice, cancellationToken)
        });
    }
}

public class ChangeProcurementDocumentStatusHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ChangeProcurementDocumentStatusCommand>
{
    public async Task<Unit> Handle(ChangeProcurementDocumentStatusCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.Kind == command.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", command.Id);
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, document.CompanyId, document.BranchId, cancellationToken);

        var status = ProcurementWorkflow.ResolveNextStatus(command.Kind, document.Status, command.Action);
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        if (command.Kind == ProcurementDocumentKind.GoodsReceipt && status == PostedDocumentStatus.Posted.ToString())
            await PostGoodsReceiptAsync(document, cancellationToken);

        if (command.Kind == ProcurementDocumentKind.PurchaseReturn && status == PostedDocumentStatus.Posted.ToString())
            await PostPurchaseReturnAsync(document, cancellationToken);

        if (command.Kind == ProcurementDocumentKind.SupplierInvoice && status == SupplierInvoiceStatus.Posted.ToString())
            await PostSupplierInvoiceAccountingAsync(document, cancellationToken);

        document.ChangeStatus(status, userId);

        if (command.Kind == ProcurementDocumentKind.GoodsReceipt && status == PostedDocumentStatus.Posted.ToString())
            await UpdateSourcePurchaseOrderReceiptStatusAsync(document, userId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task PostGoodsReceiptAsync(ProcurementDocument document, CancellationToken cancellationToken)
    {
        foreach (var line in document.Lines)
        {
            var warehouseId = line.WarehouseId ?? document.WarehouseId ?? throw new Exception("Warehouse is required for goods receipt lines.");
            if (document.BranchId.HasValue)
                await sender.Send(new EnsureWarehouseBranchScopeQuery(document.CompanyId, warehouseId, document.BranchId.Value), cancellationToken);
            var batchId = line.BatchId ?? throw new Exception("Batch is required for goods receipt lines.");
            var currencyId = document.CurrencyId ?? throw new Exception("Currency is required for goods receipt.");
            await EnsureInventoryPostableAsync(document.CompanyId, line.ProductSkuId, cancellationToken);
            var inventoryQuantity = await ResolveInventoryPackageEnteredQuantityAsync(line, cancellationToken);
            await sender.Send(new PostInventoryStockInCommand(
                ProductId: line.ProductId,
                ProductSkuId: line.ProductSkuId,
                ProductPackageId: line.ProductPackageId,
                WarehouseId: warehouseId,
                BatchId: batchId,
                Quantity: inventoryQuantity,
                UnitCost: line.UnitCost,
                TotalCost: line.TotalAmount,
                CurrencyId: currencyId,
                CompanyId: document.CompanyId,
                Notes: $"Goods receipt {document.Number}",
                ReferenceNumber: document.Number,
                SourceDocumentType: "PurchaseReceipt",
                UnitId: line.UnitOfMeasureId,
                SourceDocumentId: document.Id,
                SourceDocumentLineId: line.Id), cancellationToken);
        }
    }

    private async Task PostPurchaseReturnAsync(ProcurementDocument document, CancellationToken cancellationToken)
    {
        foreach (var line in document.Lines)
        {
            var warehouseId = line.WarehouseId ?? document.WarehouseId ?? throw new Exception("Warehouse is required for purchase return lines.");
            if (document.BranchId.HasValue)
                await sender.Send(new EnsureWarehouseBranchScopeQuery(document.CompanyId, warehouseId, document.BranchId.Value), cancellationToken);
            var batchId = line.BatchId ?? throw new Exception("Batch is required for purchase return lines.");
            var currencyId = document.CurrencyId ?? throw new Exception("Currency is required for purchase return.");
            await EnsureInventoryPostableAsync(document.CompanyId, line.ProductSkuId, cancellationToken);
            var inventoryQuantity = await ResolveInventoryPackageEnteredQuantityAsync(line, cancellationToken);
            await sender.Send(new PostInventoryStockOutCommand(
                ProductId: line.ProductId,
                ProductSkuId: line.ProductSkuId,
                ProductPackageId: line.ProductPackageId,
                WarehouseId: warehouseId,
                BatchId: batchId,
                Quantity: inventoryQuantity,
                UnitCost: line.UnitCost,
                TotalCost: line.TotalAmount,
                CurrencyId: currencyId,
                CompanyId: document.CompanyId,
                Notes: $"Purchase return {document.Number}",
                ReferenceNumber: document.Number,
                SourceDocumentType: "SupplierReturn",
                UnitId: line.UnitOfMeasureId,
                SourceDocumentId: document.Id,
                SourceDocumentLineId: line.Id), cancellationToken);
        }

        await PostPurchaseReturnAccountingAsync(document, cancellationToken);
    }

    private async Task EnsureInventoryPostableAsync(Guid companyId, Guid productSkuId, CancellationToken cancellationToken)
    {
        var context = await sender.Send(new GetProductSkuInventoryContextQuery(companyId, productSkuId), cancellationToken);
        if (!context.ProductIsActive || !context.SkuIsActive || !context.CategoryIsActive || !context.BrandIsActive || !context.UnitIsActive)
            throw new BadRequestException("Catalog product, SKU, category, brand, or unit is inactive and cannot be posted to Inventory.");

        if (context.ProductType == SharedWithUI.Catalog.Enums.CatalogProductType.Service)
            throw new BadRequestException("Service products cannot be posted to Inventory.");

        if (context.ProductType == SharedWithUI.Catalog.Enums.CatalogProductType.Combo
            || context.ProductionType == SharedWithUI.Catalog.Enums.SkuProductionType.CompositeBundle)
            throw new BadRequestException("Combo parent SKUs cannot be directly received or returned. Post their component SKUs instead.");

        if (!context.IsInventoryTracked)
            throw new BadRequestException("Only inventory-tracked SKUs can be posted to Inventory.");
    }

    private async Task PostSupplierInvoiceAccountingAsync(ProcurementDocument document, CancellationToken cancellationToken)
    {
        var accountingDocument = new AccountingDocumentDto
        {
            CompanyId = document.CompanyId,
            BranchId = document.BranchId,
            Type = AccountingDocumentType.SupplierInvoice,
            DocumentDate = document.DocumentDate,
            PartyId = document.SupplierId,
            PartyName = document.SupplierName,
            CurrencyId = document.CurrencyId,
            SourceModule = "Procurement",
            SourceDocumentId = document.Id,
            SourceDocumentNumber = document.Number,
            Lines = document.Lines.Select(line => new AccountingDocumentLineDto
            {
                Description = string.IsNullOrWhiteSpace(line.ProductNameEng) ? line.ProductName : line.ProductNameEng,
                ProductId = line.ProductId,
                ProductSkuId = line.ProductSkuId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitCost,
                DiscountAmount = (line.Quantity * line.UnitCost) * line.DiscountRate / 100m,
                TaxRate = line.TaxRate,
                NetAmount = line.NetAmount,
                TaxAmount = line.TaxAmount,
                TotalAmount = line.TotalAmount
            }).ToList()
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(accountingDocument), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
    }

    private async Task PostPurchaseReturnAccountingAsync(ProcurementDocument document, CancellationToken cancellationToken)
    {
        var accountingDocument = new AccountingDocumentDto
        {
            CompanyId = document.CompanyId,
            BranchId = document.BranchId,
            Type = AccountingDocumentType.SupplierCreditNote,
            DocumentDate = document.DocumentDate,
            PartyId = document.SupplierId,
            PartyName = document.SupplierName,
            CurrencyId = document.CurrencyId,
            SourceModule = "Procurement",
            SourceDocumentId = document.Id,
            SourceDocumentNumber = document.Number,
            Lines = document.Lines.Select(line => new AccountingDocumentLineDto
            {
                Description = string.IsNullOrWhiteSpace(line.ProductNameEng) ? line.ProductName : line.ProductNameEng,
                ProductId = line.ProductId,
                ProductSkuId = line.ProductSkuId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitCost,
                DiscountAmount = (line.Quantity * line.UnitCost) * line.DiscountRate / 100m,
                TaxRate = line.TaxRate,
                NetAmount = line.NetAmount,
                TaxAmount = line.TaxAmount,
                TotalAmount = line.TotalAmount
            }).ToList()
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(accountingDocument), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
    }

    private async Task<decimal> ResolveInventoryPackageEnteredQuantityAsync(ProcurementDocumentLine line, CancellationToken cancellationToken)
    {
        if (!line.ProductPackageId.HasValue || line.ProductPackageId.Value == Guid.Empty)
            return line.Quantity;

        var productResult = await sender.Send(new GetProductByIdQuery(line.ProductId), cancellationToken);
        var sku = productResult.Product.Skus.FirstOrDefault(x => x.Id == line.ProductSkuId)
            ?? throw new NotFoundException($"SKU not found: {line.ProductSkuId}");
        var package = sku.Packages.FirstOrDefault(x => x.Id == line.ProductPackageId.Value)
            ?? throw new NotFoundException($"Package ({line.ProductPackageId}) is not linked to SKU ({line.ProductSkuId})");

        if (package.Quantity <= 0)
            throw new BadRequestException("Selected package quantity must be greater than zero.");

        return line.Quantity / package.Quantity;
    }

    private async Task UpdateSourcePurchaseOrderReceiptStatusAsync(ProcurementDocument goodsReceipt, string userId, CancellationToken cancellationToken)
    {
        if (!goodsReceipt.SourceDocumentId.HasValue)
            return;

        var purchaseOrder = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == goodsReceipt.SourceDocumentId.Value
                && x.Kind == ProcurementDocumentKind.PurchaseOrder, cancellationToken);

        if (purchaseOrder is null || purchaseOrder.Status is "Closed" or "Cancelled")
            return;

        var postedReceipts = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .Where(x => x.Kind == ProcurementDocumentKind.GoodsReceipt
                && x.SourceDocumentId == purchaseOrder.Id
                && x.Id != goodsReceipt.Id
                && x.Status == PostedDocumentStatus.Posted.ToString())
            .ToListAsync(cancellationToken);

        var orderedBySku = purchaseOrder.Lines
            .GroupBy(x => x.ProductSkuId)
            .ToDictionary(x => x.Key, x => x.Sum(line => line.Quantity));

        var receivedBySku = postedReceipts
            .SelectMany(x => x.Lines)
            .Concat(goodsReceipt.Lines)
            .GroupBy(x => x.ProductSkuId)
            .ToDictionary(x => x.Key, x => x.Sum(line => line.Quantity));

        var totalOrdered = orderedBySku.Values.Sum();
        var totalReceived = receivedBySku.Values.Sum();

        if (totalOrdered <= 0 || totalReceived <= 0)
            return;

        var allLinesReceived = orderedBySku.All(line =>
            receivedBySku.TryGetValue(line.Key, out var receivedQuantity)
            && receivedQuantity >= line.Value);

        purchaseOrder.ChangeStatus(
            allLinesReceived ? PurchaseOrderStatus.Received.ToString() : PurchaseOrderStatus.PartiallyReceived.ToString(),
            userId);
    }
}

internal static class ProcurementDocumentFactory
{
    public static ProcurementDocument Create(ProcurementDocumentKind kind, ProcurementDocumentDto dto, string userId) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => PurchaseRequest.Create(dto, userId),
            ProcurementDocumentKind.RequestForQuotation => RequestForQuotation.Create(dto, userId),
            ProcurementDocumentKind.SupplierQuotation => SupplierQuotation.Create(dto, userId),
            ProcurementDocumentKind.PurchaseOrder => PurchaseOrder.Create(dto, userId),
            ProcurementDocumentKind.GoodsReceipt => GoodsReceipt.Create(dto, userId),
            ProcurementDocumentKind.PurchaseReturn => PurchaseReturn.Create(dto, userId),
            ProcurementDocumentKind.SupplierInvoice => SupplierInvoice.Create(dto, userId),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported procurement document kind.")
        };
}

internal static class ProcurementDocumentLockRules
{
    public static bool CanEditDocument(ProcurementDocumentKind kind, string? status) =>
        kind switch
        {
            ProcurementDocumentKind.SupplierQuotation => Is(status, SupplierQuotationStatus.Received.ToString()),
            ProcurementDocumentKind.PurchaseRequest => Is(status, PurchaseRequestStatus.Draft.ToString()),
            ProcurementDocumentKind.RequestForQuotation => Is(status, RequestForQuotationStatus.Draft.ToString()),
            ProcurementDocumentKind.PurchaseOrder => Is(status, PurchaseOrderStatus.Draft.ToString()),
            ProcurementDocumentKind.GoodsReceipt => Is(status, PostedDocumentStatus.Draft.ToString()),
            ProcurementDocumentKind.PurchaseReturn => Is(status, PostedDocumentStatus.Draft.ToString()),
            ProcurementDocumentKind.SupplierInvoice => Is(status, SupplierInvoiceStatus.Draft.ToString()),
            _ => false
        };

    public static bool CanDeleteDocument(ProcurementDocumentKind kind, string? status) =>
        CanEditDocument(kind, status);

    public static string LockedEditMessage(ProcurementDocumentKind kind, string? status) =>
        $"This {DisplayName(kind)} cannot be edited in status '{status ?? "-"}'.";

    public static string LockedDeleteMessage(ProcurementDocumentKind kind, string? status) =>
        $"This {DisplayName(kind)} cannot be deleted in status '{status ?? "-"}'.";

    private static bool Is(string? status, string expected) =>
        string.Equals(status, expected, StringComparison.OrdinalIgnoreCase);

    private static string DisplayName(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => "purchase request",
            ProcurementDocumentKind.RequestForQuotation => "request for quotation",
            ProcurementDocumentKind.SupplierQuotation => "supplier quotation",
            ProcurementDocumentKind.PurchaseOrder => "purchase order",
            ProcurementDocumentKind.GoodsReceipt => "goods receipt",
            ProcurementDocumentKind.PurchaseReturn => "purchase return",
            ProcurementDocumentKind.SupplierInvoice => "supplier invoice",
            _ => "procurement document"
        };
}

internal static class ProcurementReceiptRules
{
    public static async Task EnsureSupplierInvoiceDoesNotExceedReceivedQuantitiesAsync(
        ProcurementDbContext dbContext,
        ProcurementDocumentDto supplierInvoice,
        CancellationToken cancellationToken)
    {
        if (!supplierInvoice.SourceDocumentId.HasValue)
            return;

        var source = await dbContext.ProcurementDocuments.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == supplierInvoice.SourceDocumentId.Value
                && (x.Kind == ProcurementDocumentKind.PurchaseOrder || x.Kind == ProcurementDocumentKind.GoodsReceipt), cancellationToken);

        if (source is null)
            return;

        var invoiceableSourceIds = new HashSet<Guid> { source.Id };
        Dictionary<Guid, decimal> receivedBySku;

        if (source.Kind == ProcurementDocumentKind.PurchaseOrder)
        {
            var postedReceipts = await dbContext.ProcurementDocuments.AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.Kind == ProcurementDocumentKind.GoodsReceipt
                    && x.SourceDocumentId == source.Id
                    && x.Status == PostedDocumentStatus.Posted.ToString())
                .ToListAsync(cancellationToken);

            foreach (var receipt in postedReceipts)
                invoiceableSourceIds.Add(receipt.Id);

            receivedBySku = postedReceipts
                .SelectMany(x => x.Lines)
                .GroupBy(x => x.ProductSkuId)
                .ToDictionary(x => x.Key, x => x.Sum(line => line.Quantity));
        }
        else
        {
            if (!string.Equals(source.Status, PostedDocumentStatus.Posted.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("Supplier invoice can only be matched to a posted goods receipt.");

            if (source.SourceDocumentId.HasValue)
                invoiceableSourceIds.Add(source.SourceDocumentId.Value);

            receivedBySku = source.Lines
                .GroupBy(x => x.ProductSkuId)
                .ToDictionary(x => x.Key, x => x.Sum(line => line.Quantity));
        }

        var alreadyInvoicedBySku = await dbContext.ProcurementDocuments.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.Kind == ProcurementDocumentKind.SupplierInvoice
                && x.Id != supplierInvoice.Id
                && x.SourceDocumentId.HasValue
                && invoiceableSourceIds.Contains(x.SourceDocumentId.Value)
                && x.Status != SupplierInvoiceStatus.Cancelled.ToString())
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.ProductSkuId)
            .Select(x => new { ProductSkuId = x.Key, Quantity = x.Sum(line => line.Quantity) })
            .ToDictionaryAsync(x => x.ProductSkuId, x => x.Quantity, cancellationToken);

        foreach (var invoiceLine in supplierInvoice.Lines
            .GroupBy(x => x.ProductSkuId)
            .Select(x => new { ProductSkuId = x.Key, Quantity = x.Sum(line => line.Quantity) }))
        {
            receivedBySku.TryGetValue(invoiceLine.ProductSkuId, out var receivedQuantity);
            alreadyInvoicedBySku.TryGetValue(invoiceLine.ProductSkuId, out var alreadyInvoicedQuantity);
            var invoiceableQuantity = receivedQuantity - alreadyInvoicedQuantity;

            if (invoiceLine.Quantity > invoiceableQuantity)
                throw new Exception("Supplier invoice quantity cannot exceed uninvoiced posted goods receipt quantity.");
        }
    }
}

internal static class ProcurementWorkflow
{
    public static string ResolveNextStatus(ProcurementDocumentKind kind, string currentStatus, string action)
    {
        var normalizedAction = action.Trim().ToLowerInvariant();
        return kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => ResolvePurchaseRequest(currentStatus, normalizedAction),
            ProcurementDocumentKind.RequestForQuotation => ResolveRfq(currentStatus, normalizedAction),
            ProcurementDocumentKind.SupplierQuotation => ResolveSupplierQuotation(currentStatus, normalizedAction),
            ProcurementDocumentKind.PurchaseOrder => ResolvePurchaseOrder(currentStatus, normalizedAction),
            ProcurementDocumentKind.GoodsReceipt => ResolvePostedDocument(currentStatus, normalizedAction),
            ProcurementDocumentKind.PurchaseReturn => ResolvePostedDocument(currentStatus, normalizedAction),
            ProcurementDocumentKind.SupplierInvoice => ResolveSupplierInvoice(currentStatus, normalizedAction),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported procurement document kind.")
        };
    }

    private static string ResolvePurchaseRequest(string status, string action) =>
        (status, action) switch
        {
            ("Draft", "submit") => PurchaseRequestStatus.Submitted.ToString(),
            ("Submitted", "approve") => PurchaseRequestStatus.Approved.ToString(),
            ("Submitted", "reject") => PurchaseRequestStatus.Rejected.ToString(),
            (_, "cancel") => PurchaseRequestStatus.Cancelled.ToString(),
            _ => throw new Exception($"Invalid purchase request workflow action '{action}' from status '{status}'.")
        };

    private static string ResolveRfq(string status, string action) =>
        (status, action) switch
        {
            ("Draft", "send") => RequestForQuotationStatus.Sent.ToString(),
            (_, "close") => RequestForQuotationStatus.Closed.ToString(),
            (_, "cancel") => RequestForQuotationStatus.Cancelled.ToString(),
            _ => throw new Exception($"Invalid RFQ workflow action '{action}' from status '{status}'.")
        };

    private static string ResolveSupplierQuotation(string status, string action) =>
        (status, action) switch
        {
            ("Received", "accept") => SupplierQuotationStatus.Accepted.ToString(),
            ("Received", "reject") => SupplierQuotationStatus.Rejected.ToString(),
            _ => throw new Exception($"Invalid supplier quotation workflow action '{action}' from status '{status}'.")
        };

    private static string ResolvePurchaseOrder(string status, string action) =>
        (status, action) switch
        {
            ("Draft", "approve") => PurchaseOrderStatus.Approved.ToString(),
            ("Approved", "send") => PurchaseOrderStatus.Sent.ToString(),
            ("Sent", "close") => PurchaseOrderStatus.Closed.ToString(),
            ("PartiallyReceived", "close") => PurchaseOrderStatus.Closed.ToString(),
            (_, "cancel") => PurchaseOrderStatus.Cancelled.ToString(),
            _ => throw new Exception($"Invalid purchase order workflow action '{action}' from status '{status}'.")
        };

    private static string ResolvePostedDocument(string status, string action) =>
        (status, action) switch
        {
            ("Draft", "post") => PostedDocumentStatus.Posted.ToString(),
            (_, "cancel") => PostedDocumentStatus.Cancelled.ToString(),
            _ => throw new Exception($"Invalid posted document workflow action '{action}' from status '{status}'.")
        };

    private static string ResolveSupplierInvoice(string status, string action) =>
        (status, action) switch
        {
            ("Draft", "match") => SupplierInvoiceStatus.Matched.ToString(),
            ("Matched", "post") => SupplierInvoiceStatus.Posted.ToString(),
            (_, "cancel") => SupplierInvoiceStatus.Cancelled.ToString(),
            _ => throw new Exception($"Invalid supplier invoice workflow action '{action}' from status '{status}'.")
        };
}
