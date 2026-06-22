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

public class CreateProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateProcurementDocumentCommand, CreateProcurementDocumentResult>
{
    public async Task<CreateProcurementDocumentResult> Handle(CreateProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId(httpContextAccessor);
        command.Document.Kind = command.Kind;
        if (command.Kind == ProcurementDocumentKind.PurchaseRequest)
            return await CreateNumberedDocumentAsync(command, userId, "PR", 4, "purchase request", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.RequestForQuotation)
            return await CreateNumberedDocumentAsync(command, userId, "RFQ", 5, "request for quotation", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.SupplierQuotation)
            return await CreateNumberedDocumentAsync(command, userId, "SQ", 5, "supplier quotation", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.PurchaseOrder)
            return await CreateNumberedDocumentAsync(command, userId, "PO", 5, "purchase order", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.GoodsReceipt)
            return await CreateNumberedDocumentAsync(command, userId, "GR", 5, "goods receipt", cancellationToken);

        if (command.Kind == ProcurementDocumentKind.SupplierInvoice)
        {
            await ProcurementReceiptRules.EnsureSupplierInvoiceDoesNotExceedReceivedQuantitiesAsync(dbContext, command.Document, cancellationToken);
            return await CreateNumberedDocumentAsync(command, userId, "INV", 5, "supplier invoice", cancellationToken);
        }

        var document = ProcurementDocumentFactory.Create(command.Kind, command.Document, userId);
        await dbContext.ProcurementDocuments.AddAsync(document, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementDocumentResult(document.Id);
    }

    private async Task<CreateProcurementDocumentResult> CreateNumberedDocumentAsync(
        CreateProcurementDocumentCommand command,
        string userId,
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
}

public class UpdateProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProcurementDocumentCommand>
{
    public async Task<Unit> Handle(UpdateProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.Kind == command.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", command.Id);

        if (!ProcurementDocumentLockRules.CanEditDocument(command.Kind, document.Status))
            throw new BadRequestException(ProcurementDocumentLockRules.LockedEditMessage(command.Kind, document.Status));

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

        document.Update(command.Document, CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class RemoveProcurementDocumentHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveProcurementDocumentCommand>
{
    public async Task<Unit> Handle(RemoveProcurementDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.Kind == command.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", command.Id);

        if (!ProcurementDocumentLockRules.CanDeleteDocument(command.Kind, document.Status))
            throw new BadRequestException(ProcurementDocumentLockRules.LockedDeleteMessage(command.Kind, document.Status));

        document.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetProcurementDocumentByIdHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetProcurementDocumentByIdQuery, GetProcurementDocumentByIdResult>
{
    public async Task<GetProcurementDocumentByIdResult> Handle(GetProcurementDocumentByIdQuery query, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProcurementDocuments.AsNoTracking().Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == query.Id && x.Kind == query.Kind, cancellationToken)
            ?? throw new NotFoundException("Procurement document", query.Id);

        return new GetProcurementDocumentByIdResult(document.ToDto());
    }
}

public class GetProcurementDocumentsHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetProcurementDocumentsQuery, GetProcurementDocumentsResult>
{
    public async Task<GetProcurementDocumentsResult> Handle(GetProcurementDocumentsQuery query, CancellationToken cancellationToken)
    {
        var documents = dbContext.ProcurementDocuments.AsNoTracking()
            .Where(x => x.Kind == query.Kind);

        if (query.CompanyId.HasValue)
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);

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

public class GetProcurementDashboardHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetProcurementDashboardQuery, GetProcurementDashboardResult>
{
    public async Task<GetProcurementDashboardResult> Handle(GetProcurementDashboardQuery query, CancellationToken cancellationToken)
    {
        var documents = dbContext.ProcurementDocuments.AsNoTracking();
        if (query.CompanyId.HasValue)
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);

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
            var batchId = line.BatchId ?? throw new Exception("Batch is required for goods receipt lines.");
            var currencyId = document.CurrencyId ?? throw new Exception("Currency is required for goods receipt.");
            var inventoryQuantity = await ResolveInventoryPackageEnteredQuantityAsync(line, cancellationToken);
            await sender.Send(new PostInventoryStockInCommand(
                line.ProductId,
                line.ProductSkuId,
                line.ProductPackageId,
                warehouseId,
                batchId,
                inventoryQuantity,
                line.UnitCost,
                line.TotalAmount,
                currencyId,
                document.CompanyId,
                $"Goods receipt {document.Number}"), cancellationToken);
        }
    }

    private async Task PostPurchaseReturnAsync(ProcurementDocument document, CancellationToken cancellationToken)
    {
        foreach (var line in document.Lines)
        {
            var warehouseId = line.WarehouseId ?? document.WarehouseId ?? throw new Exception("Warehouse is required for purchase return lines.");
            var batchId = line.BatchId ?? throw new Exception("Batch is required for purchase return lines.");
            var currencyId = document.CurrencyId ?? throw new Exception("Currency is required for purchase return.");
            var inventoryQuantity = await ResolveInventoryPackageEnteredQuantityAsync(line, cancellationToken);
            await sender.Send(new PostInventoryStockOutCommand(
                line.ProductId,
                line.ProductSkuId,
                line.ProductPackageId,
                warehouseId,
                batchId,
                inventoryQuantity,
                line.UnitCost,
                line.TotalAmount,
                currencyId,
                document.CompanyId,
                $"Purchase return {document.Number}"), cancellationToken);
        }

        await PostPurchaseReturnAccountingAsync(document, cancellationToken);
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
            ("Approved", "convert") => PurchaseRequestStatus.Converted.ToString(),
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
