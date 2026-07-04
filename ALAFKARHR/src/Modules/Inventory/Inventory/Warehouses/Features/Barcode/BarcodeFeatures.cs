using Catalog.Contracts.Products.Features.ResolveCatalogBarcode;
using Inventory.Warehouses.Features.InventoryControls;

namespace Inventory.Warehouses.Features.Barcode;

public record ResolveBarcodeQuery(BarcodeScanRequestDto Request) : IQuery<BarcodeScanResultDto>;
public record CreateBarcodeSessionCommand(BarcodeOperationSessionDto Session) : ICommand<CreateInventoryControlResult>;
public record ScanBarcodeSessionCommand(Guid SessionId, BarcodeScanRequestDto Request) : ICommand<BarcodeOperationSessionDto>;
public record ApplyBarcodeSessionCommand(Guid SessionId, bool ConfirmWarnings) : ICommand<BarcodeApplyResultDto>;
public record GetBarcodeSessionsQuery(Guid CompanyId) : IQuery<IReadOnlyCollection<BarcodeOperationSessionDto>>;

public class ResolveBarcodeHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<ResolveBarcodeQuery, BarcodeScanResultDto>
{
    public async Task<BarcodeScanResultDto> Handle(ResolveBarcodeQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;
        var barcode = Normalize(request.Barcode);
        if (string.IsNullOrWhiteSpace(barcode))
            return Rejected(request.Barcode, "Barcode is required.");

        var gs1 = TryParseGs1(barcode);
        if (gs1 is not null)
        {
            var nested = await Handle(new ResolveBarcodeQuery(new BarcodeScanRequestDto
            {
                CompanyId = request.CompanyId,
                Barcode = gs1.ProductCode ?? barcode,
                OperationType = request.OperationType,
                WarehouseId = request.WarehouseId,
                WarehouseLocationId = request.WarehouseLocationId,
                ProductSkuId = request.ProductSkuId,
                IsOutbound = request.IsOutbound
            }), cancellationToken);

            nested.RawBarcode = request.Barcode;
            nested.NormalizedBarcode = barcode;
            nested.EntityType = nested.EntityType == BarcodeScanEntityType.Unknown ? BarcodeScanEntityType.Gs1 : nested.EntityType;
            nested.BatchNumber ??= gs1.BatchNumber;
            nested.ExpiryDate ??= gs1.ExpiryDate;
            nested.Quantity ??= gs1.Quantity;
            nested.Warnings.Add("GS1 barcode parsed with limited product, lot, expiry, and quantity support.");
            return nested;
        }

        var catalogResult = await ResolveCatalogAsync(request, barcode, cancellationToken);
        if (catalogResult is not null)
            return catalogResult;

        var locationResult = await ResolveLocationAsync(request, barcode, cancellationToken);
        if (locationResult is not null)
            return locationResult;

        var batchResult = await ResolveBatchAsync(request, barcode, cancellationToken);
        if (batchResult is not null)
            return batchResult;

        var transferResult = await ResolveTransferAsync(request, barcode, cancellationToken);
        if (transferResult is not null)
            return transferResult;

        var cycleCountResult = await ResolveCycleCountAsync(request, barcode, cancellationToken);
        if (cycleCountResult is not null)
            return cycleCountResult;

        var sourceDocumentResult = await ResolveSourceDocumentAsync(request, barcode, cancellationToken);
        if (sourceDocumentResult is not null)
            return sourceDocumentResult;

        return Rejected(request.Barcode, "No active SKU, package, location, batch, transfer, cycle count, delivery note, or goods receipt matches this barcode.");
    }

    private async Task<BarcodeScanResultDto?> ResolveCatalogAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ResolveCatalogBarcodeQuery(request.CompanyId, barcode), cancellationToken);
        var match = result.Items.FirstOrDefault();
        if (match is null)
            return null;

        var warnings = new List<string>();
        var isRejected = false;
        if (!match.ProductIsActive || !match.SkuIsActive || !match.PackageIsActive || !match.CategoryIsActive || !match.BrandIsActive || !match.UnitIsActive)
        {
            warnings.Add("Catalog record is inactive or archived.");
            isRejected = true;
        }

        if (request.OperationType is not BarcodeOperationType.Receipt and not BarcodeOperationType.StockIn and not BarcodeOperationType.Adjustment
            && (!match.IsInventoryTracked || match.ProductType == CatalogProductType.Service || match.ProductType == CatalogProductType.Combo || match.ProductionType == SkuProductionType.CompositeBundle))
        {
            warnings.Add("Outbound scans require inventory-tracked goods SKUs. Combo parents are not scanned as stock movement SKUs.");
            isRejected = true;
        }

        var packageMultiplier = Math.Max(match.PackageQuantity, 1m);
        var unitMultiplier = Math.Max(match.PackageUnitConversionFactor ?? 1m, 1m);
        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = match.ProductPackageId.HasValue ? BarcodeScanEntityType.ProductPackage : BarcodeScanEntityType.ProductSku,
            EntityId = match.ProductPackageId ?? match.ProductSkuId,
            CompanyId = match.CompanyId,
            ProductId = match.ProductId,
            ProductSkuId = match.ProductSkuId,
            ProductPackageId = match.ProductPackageId,
            Code = match.Code,
            Label = match.Name,
            LabelEng = match.NameEng,
            Quantity = packageMultiplier * unitMultiplier,
            IsActive = !isRejected,
            IsRejected = isRejected,
            Warning = warnings.FirstOrDefault(),
            Warnings = warnings
        };
    }

    private async Task<BarcodeScanResultDto?> ResolveLocationAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var location = await dbContext.WarehouseLocations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.Code == barcode && !x.IsDeleted, cancellationToken);
        if (location is null)
            return null;

        try
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext, sender, request.CompanyId, location.WarehouseId, cancellationToken);
        }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException or BadRequestException)
        {
            return Rejected(request.Barcode, ex.Message, BarcodeScanEntityType.WarehouseLocation, location.Id);
        }

        var warnings = new List<string>();
        if (!location.IsActive)
            warnings.Add("Warehouse location is inactive.");

        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = BarcodeScanEntityType.WarehouseLocation,
            EntityId = location.Id,
            CompanyId = location.CompanyId,
            WarehouseId = location.WarehouseId,
            WarehouseLocationId = location.Id,
            Code = location.Code,
            Label = location.Name,
            LabelEng = location.NameEng,
            IsActive = location.IsActive,
            IsRejected = !location.IsActive,
            Warning = warnings.FirstOrDefault(),
            Warnings = warnings
        };
    }

    private async Task<BarcodeScanResultDto?> ResolveBatchAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var batchQuery = dbContext.Batches.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.BatchNumber == barcode && !x.IsDeleted);
        if (request.ProductSkuId.HasValue)
            batchQuery = batchQuery.Where(x => x.ProductSkuId == request.ProductSkuId.Value);

        var batch = await batchQuery.OrderBy(x => x.ExpiryDate).FirstOrDefaultAsync(cancellationToken);
        if (batch is null)
            return null;

        var expired = batch.ExpiryDate.Date < DateTime.UtcNow.Date;
        var reject = request.IsOutbound && expired && request.OperationType is not BarcodeOperationType.Adjustment;
        var warnings = new List<string>();
        if (expired)
            warnings.Add("Batch is expired.");
        if (reject)
            warnings.Add("Expired batches cannot be used for normal outbound scans.");

        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = BarcodeScanEntityType.Batch,
            EntityId = batch.Id,
            CompanyId = batch.CompanyId,
            ProductId = batch.ProductId,
            ProductSkuId = batch.ProductSkuId,
            BatchId = batch.Id,
            Code = batch.BatchNumber,
            Label = batch.BatchNumber,
            LabelEng = batch.BatchNumber,
            BatchNumber = batch.BatchNumber,
            ExpiryDate = batch.ExpiryDate,
            IsExpired = expired,
            IsActive = true,
            IsRejected = reject,
            Warning = warnings.FirstOrDefault(),
            Warnings = warnings
        };
    }

    private async Task<BarcodeScanResultDto?> ResolveTransferAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var transfer = await dbContext.WarehouseTransfers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId
                && (x.TransferNumber == barcode || x.ReferenceNumber == barcode)
                && !x.IsDeleted,
                cancellationToken);
        if (transfer is null)
            return null;

        try
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext, sender, request.CompanyId, transfer.SourceWarehouseId, cancellationToken);
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext, sender, request.CompanyId, transfer.DestinationWarehouseId, cancellationToken);
        }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException or BadRequestException)
        {
            return Rejected(request.Barcode, ex.Message, BarcodeScanEntityType.WarehouseTransfer, transfer.Id);
        }

        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = BarcodeScanEntityType.WarehouseTransfer,
            EntityId = transfer.Id,
            CompanyId = transfer.CompanyId,
            WarehouseId = request.OperationType == BarcodeOperationType.TransferReceive ? transfer.DestinationWarehouseId : transfer.SourceWarehouseId,
            Code = transfer.TransferNumber,
            Label = transfer.TransferNumber,
            LabelEng = transfer.TransferNumber,
            IsActive = transfer.Status is TransferStatus.Pending or TransferStatus.Shipped or TransferStatus.PartiallyReceived,
            IsRejected = transfer.Status is TransferStatus.Cancelled or TransferStatus.Completed,
            Warning = transfer.Status is TransferStatus.Cancelled or TransferStatus.Completed ? "Transfer is no longer open." : null
        };
    }

    private async Task<BarcodeScanResultDto?> ResolveCycleCountAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var count = await dbContext.CycleCounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.CountNumber == barcode && !x.IsDeleted, cancellationToken);
        if (count is null)
            return null;

        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = BarcodeScanEntityType.CycleCount,
            EntityId = count.Id,
            CompanyId = count.CompanyId,
            WarehouseId = count.WarehouseId,
            WarehouseLocationId = count.WarehouseLocationId,
            Code = count.CountNumber,
            Label = count.CountNumber,
            LabelEng = count.CountNumber,
            IsActive = !count.IsPosted,
            IsRejected = count.IsPosted,
            Warning = count.IsPosted ? "Cycle count is already posted." : null
        };
    }

    private async Task<BarcodeScanResultDto?> ResolveSourceDocumentAsync(BarcodeScanRequestDto request, string barcode, CancellationToken cancellationToken)
    {
        var movement = await dbContext.StockMovements.AsNoTracking()
            .Where(x => x.ReferenceNumber == barcode
                && x.SourceDocumentId.HasValue
                && (x.SourceDocumentType == "PurchaseReceipt"
                    || x.SourceDocumentType == "SalesDeliveryNote"
                    || x.SourceDocumentType == "SupplierReturn"
                    || x.SourceDocumentType == "SalesReturn"))
            .Join(dbContext.Warehouses.AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
                movement => movement.WarehouseId,
                warehouse => warehouse.Id,
                (movement, warehouse) => new { movement, warehouse })
            .FirstOrDefaultAsync(cancellationToken);

        if (movement is null)
            return null;

        try
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext, sender, request.CompanyId, movement.movement.WarehouseId, cancellationToken);
        }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException or BadRequestException)
        {
            return Rejected(request.Barcode, ex.Message, SourceEntityType(movement.movement.SourceDocumentType), movement.movement.SourceDocumentId);
        }

        return new BarcodeScanResultDto
        {
            RawBarcode = request.Barcode,
            NormalizedBarcode = barcode,
            EntityType = SourceEntityType(movement.movement.SourceDocumentType),
            EntityId = movement.movement.SourceDocumentId,
            CompanyId = request.CompanyId,
            ProductId = movement.movement.ProductId,
            ProductSkuId = movement.movement.ProductSkuId,
            WarehouseId = movement.movement.WarehouseId,
            WarehouseLocationId = movement.movement.SourceLocationId ?? movement.movement.DestinationLocationId,
            BatchId = movement.movement.BatchId,
            Code = movement.movement.ReferenceNumber,
            Label = movement.movement.ReferenceNumber,
            LabelEng = movement.movement.ReferenceNumber,
            IsActive = true
        };

        static BarcodeScanEntityType SourceEntityType(string sourceDocumentType) => sourceDocumentType switch
        {
            "PurchaseReceipt" => BarcodeScanEntityType.GoodsReceipt,
            "SalesDeliveryNote" => BarcodeScanEntityType.DeliveryNote,
            _ => BarcodeScanEntityType.Unknown
        };
    }

    private static BarcodeScanResultDto Rejected(string raw, string warning, BarcodeScanEntityType entityType = BarcodeScanEntityType.Unknown, Guid? entityId = null) => new()
    {
        RawBarcode = raw,
        NormalizedBarcode = Normalize(raw),
        EntityType = entityType,
        EntityId = entityId,
        IsActive = false,
        IsRejected = true,
        Warning = warning,
        Warnings = [warning]
    };

    private static string Normalize(string? barcode) => barcode?.Trim() ?? string.Empty;

    private static ParsedGs1Barcode? TryParseGs1(string barcode)
    {
        if (!barcode.StartsWith("01", StringComparison.Ordinal) || barcode.Length < 16)
            return null;

        var parsed = new ParsedGs1Barcode { ProductCode = barcode.Substring(2, 14) };
        var remainder = barcode[16..];
        var cursor = 0;
        while (cursor + 2 <= remainder.Length)
        {
            var ai = remainder.Substring(cursor, 2);
            cursor += 2;
            if (ai == "17" && cursor + 6 <= remainder.Length)
            {
                var value = remainder.Substring(cursor, 6);
                cursor += 6;
                if (DateTime.TryParseExact(value, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out var expiry))
                    parsed.ExpiryDate = expiry;
            }
            else if (ai == "10")
            {
                parsed.BatchNumber = remainder[cursor..];
                break;
            }
            else if (ai == "30")
            {
                var value = remainder[cursor..];
                if (decimal.TryParse(value, out var quantity))
                    parsed.Quantity = quantity;
                break;
            }
            else
            {
                break;
            }
        }

        return parsed;
    }

    private sealed class ParsedGs1Barcode
    {
        public string? ProductCode { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? Quantity { get; set; }
    }
}

public class CreateBarcodeSessionHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateBarcodeSessionCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(CreateBarcodeSessionCommand request, CancellationToken cancellationToken)
    {
        if (request.Session.WarehouseId.HasValue)
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
                dbContext, sender, request.Session.CompanyId, request.Session.WarehouseId, cancellationToken);
        }

        var userId = BarcodeHelpers.GetUserId(httpContextAccessor);
        var session = BarcodeOperationSession.Create(request.Session, userId);
        await dbContext.BarcodeOperationSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(session.Id);
    }
}

public class ScanBarcodeSessionHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ScanBarcodeSessionCommand, BarcodeOperationSessionDto>
{
    public async Task<BarcodeOperationSessionDto> Handle(ScanBarcodeSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.BarcodeOperationSessions.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("Barcode session", request.SessionId);

        await EnsureSessionScopeAsync(session, cancellationToken);

        var scanRequest = request.Request;
        scanRequest.CompanyId = session.CompanyId;
        scanRequest.OperationType = session.OperationType;
        scanRequest.WarehouseId ??= session.WarehouseId;
        scanRequest.WarehouseLocationId ??= session.SourceLocationId ?? session.DestinationLocationId;
        scanRequest.ProductSkuId ??= session.Lines.LastOrDefault(x => x.ProductSkuId.HasValue)?.ProductSkuId;
        scanRequest.IsOutbound = session.OperationType is BarcodeOperationType.StockOut or BarcodeOperationType.Delivery or BarcodeOperationType.TransferShip;

        var scan = await sender.Send(new ResolveBarcodeQuery(scanRequest), cancellationToken);
        var userId = BarcodeHelpers.GetUserId(httpContextAccessor);
        session.UpdateContextFromScan(scan, userId);

        var quantity = scan.Quantity ?? 1m;
        var status = scan.IsRejected
            ? BarcodeLineStatus.Rejected
            : scan.Warnings.Count > 0 ? BarcodeLineStatus.Warning : BarcodeLineStatus.Accepted;
        var sourceLocationId = session.OperationType is BarcodeOperationType.StockOut or BarcodeOperationType.Delivery or BarcodeOperationType.TransferShip
            ? scan.WarehouseLocationId ?? session.SourceLocationId
            : null;
        var destinationLocationId = session.OperationType is BarcodeOperationType.StockIn or BarcodeOperationType.Receipt or BarcodeOperationType.TransferReceive or BarcodeOperationType.CycleCount or BarcodeOperationType.Adjustment
            ? scan.WarehouseLocationId ?? session.DestinationLocationId
            : null;

        var line = BarcodeOperationLine.Create(session.Id, new BarcodeOperationLineDto
        {
            RawBarcode = scan.RawBarcode,
            EntityType = scan.EntityType,
            Status = status,
            ProductId = scan.ProductId,
            ProductSkuId = scan.ProductSkuId,
            ProductPackageId = scan.ProductPackageId,
            BatchId = scan.BatchId,
            WarehouseId = scan.WarehouseId ?? session.WarehouseId,
            SourceLocationId = sourceLocationId,
            DestinationLocationId = destinationLocationId,
            EnteredQuantity = quantity,
            PackageMultiplier = scan.ProductPackageId.HasValue ? quantity : 1m,
            UnitMultiplier = 1m,
            NormalizedQuantity = quantity,
            DisplayLabel = scan.Label,
            DisplayLabelEng = scan.LabelEng,
            BatchNumber = scan.BatchNumber,
            ExpiryDate = scan.ExpiryDate,
            Warning = scan.Warning
        }, userId);
        session.AddLine(line, userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return session.ToDto();

        async Task EnsureSessionScopeAsync(BarcodeOperationSession sessionToCheck, CancellationToken token)
        {
            if (sessionToCheck.WarehouseId.HasValue)
                await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
                    dbContext, sender, sessionToCheck.CompanyId, sessionToCheck.WarehouseId, token);
        }
    }
}

public class ApplyBarcodeSessionHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ApplyBarcodeSessionCommand, BarcodeApplyResultDto>
{
    public async Task<BarcodeApplyResultDto> Handle(ApplyBarcodeSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.BarcodeOperationSessions.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("Barcode session", request.SessionId);

        if (session.Lines.Any(x => x.Status == BarcodeLineStatus.Rejected))
            throw new BadRequestException("Barcode session contains rejected scan lines.");
        if (!request.ConfirmWarnings && session.Lines.Any(x => x.Status == BarcodeLineStatus.Warning))
            throw new BadRequestException("Barcode session has warnings. Confirm warnings to apply.");

        var userId = BarcodeHelpers.GetUserId(httpContextAccessor);
        Guid? createdDocumentId = null;
        string? createdDocumentType = null;
        string message = "Barcode session validated and marked as applied. Use existing operation posting screens for financial stock posting.";

        if (session.OperationType == BarcodeOperationType.CycleCount)
        {
            var locationId = session.DestinationLocationId ?? session.SourceLocationId;
            if (!session.WarehouseId.HasValue || !locationId.HasValue)
                throw new BadRequestException("Cycle count barcode session requires warehouse and location scans.");

            var countLines = session.Lines
                .Where(x => x.ProductId.HasValue && x.ProductSkuId.HasValue && x.BatchId.HasValue)
                .GroupBy(x => new { ProductId = x.ProductId!.Value, ProductSkuId = x.ProductSkuId!.Value, BatchId = x.BatchId!.Value })
                .Select(group => new CycleCountLineDto
                {
                    ProductId = group.Key.ProductId,
                    ProductSkuId = group.Key.ProductSkuId,
                    BatchId = group.Key.BatchId,
                    CountedQuantity = group.Sum(x => x.NormalizedQuantity),
                    BatchNumber = group.FirstOrDefault()?.BatchNumber,
                    Notes = "Created from barcode scan session."
                })
                .ToList();

            if (countLines.Count == 0)
                throw new BadRequestException("Cycle count barcode session requires SKU/package and batch scans.");

            var result = await sender.Send(new UpsertCycleCountCommand(new CycleCountDto
            {
                CompanyId = session.CompanyId,
                WarehouseId = session.WarehouseId.Value,
                WarehouseLocationId = locationId.Value,
                CountNumber = session.ReferenceNumber ?? $"CC-BAR-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                Reason = session.Notes ?? "Barcode cycle count",
                CountDate = DateTime.UtcNow,
                Lines = countLines
            }), cancellationToken);

            createdDocumentId = result.Id;
            createdDocumentType = "CycleCount";
            message = "Cycle count draft created from barcode scan session.";
        }

        session.MarkApplied(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BarcodeApplyResultDto
        {
            SessionId = session.Id,
            Status = session.Status,
            CreatedDocumentId = createdDocumentId,
            CreatedDocumentType = createdDocumentType,
            Message = message
        };
    }
}

public class GetBarcodeSessionsHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetBarcodeSessionsQuery, IReadOnlyCollection<BarcodeOperationSessionDto>>
{
    public async Task<IReadOnlyCollection<BarcodeOperationSessionDto>> Handle(GetBarcodeSessionsQuery request, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        var query = dbContext.BarcodeOperationSessions.Include(x => x.Lines).AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (!branchAccess.CanViewAllBranches)
        {
            var readableWarehouseIds = dbContext.Warehouses.AsNoTracking()
                .Where(x => x.CompanyId == request.CompanyId
                    && (x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value))))
                .Select(x => x.Id);
            query = query.Where(x => !x.WarehouseId.HasValue || readableWarehouseIds.Contains(x.WarehouseId.Value));
        }

        var sessions = await query
            .OrderByDescending(x => x.StartedAt)
            .Take(100)
            .ToListAsync(cancellationToken);
        return sessions.Select(x => x.ToDto()).ToList();
    }
}

public class BarcodeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/barcode/resolve", async (BarcodeScanRequestDto request, ISender sender) =>
            Results.Ok(new { result = await sender.Send(new ResolveBarcodeQuery(request)) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost("/api/v1/inventory/barcode/sessions", async (BarcodeOperationSessionDto session, ISender sender) =>
            Results.Ok(await sender.Send(new CreateBarcodeSessionCommand(session))))
            .RequireAuthorization(PermissionList.InventoryPermissions.Create);

        app.MapPost("/api/v1/inventory/barcode/sessions/{id:guid}/scan", async (Guid id, BarcodeScanRequestDto request, ISender sender) =>
            Results.Ok(new { session = await sender.Send(new ScanBarcodeSessionCommand(id, request)) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit);

        app.MapPost("/api/v1/inventory/barcode/sessions/{id:guid}/apply", async (Guid id, ApplyBarcodeSessionDto request, ISender sender) =>
            Results.Ok(new { result = await sender.Send(new ApplyBarcodeSessionCommand(id, request.ConfirmWarnings)) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit);

        app.MapGet("/api/v1/inventory/barcode/sessions/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { sessions = await sender.Send(new GetBarcodeSessionsQuery(companyId)) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }
}

file static class BarcodeHelpers
{
    public static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");
}
