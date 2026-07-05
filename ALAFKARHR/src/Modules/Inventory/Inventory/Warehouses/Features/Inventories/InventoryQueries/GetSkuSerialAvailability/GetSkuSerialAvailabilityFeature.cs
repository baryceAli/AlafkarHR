namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetSkuSerialAvailability;

public class GetSkuSerialAvailabilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/availability/serials/company/{companyId:guid}/sku/{productSkuId:guid}", async (
                Guid companyId,
                Guid productSkuId,
                Guid? warehouseId,
                Guid? warehouseLocationId,
                Guid? batchId,
                Guid? branchId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetSkuSerialAvailabilityQuery(companyId, productSkuId, warehouseId, warehouseLocationId, batchId, branchId));
                return Results.Ok(result);
            })
            .WithName("GetSkuSerialAvailability")
            .Produces<SkuSerialAvailabilityResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapGet("/api/v1/inventory/serial-trace/company/{companyId:guid}", async (
                Guid companyId,
                Guid? productSkuId,
                string? serialNumber,
                ISender sender) =>
            {
                var result = await sender.Send(new GetSerialNumberTraceQuery(companyId, productSkuId, serialNumber));
                return Results.Ok(result);
            })
            .WithName("GetSerialNumberTrace")
            .Produces<SerialNumberTraceResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }
}

public class GetSkuSerialAvailabilityHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetSkuSerialAvailabilityQuery, SkuSerialAvailabilityResult>
{
    public async Task<SkuSerialAvailabilityResult> Handle(GetSkuSerialAvailabilityQuery request, CancellationToken cancellationToken)
    {
        if (request.WarehouseId.HasValue)
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext,
                sender,
                request.CompanyId,
                request.WarehouseId,
                cancellationToken);
        }

        var query = from serial in dbContext.InventorySerialNumbers.AsNoTracking()
                    join batch in dbContext.Batches.AsNoTracking() on serial.BatchId equals batch.Id into batchJoin
                    from batch in batchJoin.DefaultIfEmpty()
                    join warehouse in dbContext.Warehouses.AsNoTracking() on serial.WarehouseId equals warehouse.Id into warehouseJoin
                    from warehouse in warehouseJoin.DefaultIfEmpty()
                    join location in dbContext.WarehouseLocations.AsNoTracking() on serial.WarehouseLocationId equals location.Id into locationJoin
                    from location in locationJoin.DefaultIfEmpty()
                    where serial.CompanyId == request.CompanyId
                        && serial.ProductSkuId == request.ProductSkuId
                    select new { serial, batch, warehouse, location };

        if (request.WarehouseId.HasValue)
            query = query.Where(x => x.serial.WarehouseId == request.WarehouseId.Value);
        if (request.WarehouseLocationId.HasValue)
            query = query.Where(x => x.serial.WarehouseLocationId == request.WarehouseLocationId.Value);
        if (request.BatchId.HasValue)
            query = query.Where(x => x.serial.BatchId == request.BatchId.Value);

        var rows = await query
            .OrderBy(x => x.batch == null ? DateTime.MaxValue : x.batch.ExpiryDate)
            .ThenBy(x => x.serial.SerialNumber)
            .Select(x => new InventorySerialAvailabilityRow(
                x.serial.Id,
                x.serial.CompanyId,
                x.serial.ProductId,
                x.serial.ProductSkuId,
                x.serial.SerialNumber,
                x.serial.BatchId,
                x.batch == null ? null : x.batch.BatchNumber,
                x.batch == null ? null : x.batch.ExpiryDate,
                x.serial.WarehouseId,
                x.warehouse == null ? null : x.warehouse.Name,
                x.warehouse == null ? null : x.warehouse.NameEng,
                x.serial.WarehouseLocationId,
                x.location == null ? null : x.location.Code,
                x.location == null ? null : x.location.Name,
                x.location == null ? null : x.location.NameEng,
                (int)x.serial.Status,
                x.serial.SourceDocumentId,
                x.serial.SourceDocumentLineId,
                x.serial.LastStockMovementId,
                x.serial.LastMovementAt))
            .ToListAsync(cancellationToken);

        return new SkuSerialAvailabilityResult(
            request.CompanyId,
            request.ProductSkuId,
            request.WarehouseId,
            request.WarehouseLocationId,
            request.BatchId,
            rows.Count,
            rows.Count(x => x.Status == (int)InventorySerialStatus.Reserved),
            rows.Count(x => x.Status == (int)InventorySerialStatus.Available || x.Status == (int)InventorySerialStatus.Returned),
            rows);
    }
}

public class GetSerialNumberTraceHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetSerialNumberTraceQuery, SerialNumberTraceResult>
{
    public async Task<SerialNumberTraceResult> Handle(GetSerialNumberTraceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SerialNumber))
            throw new BadRequestException("Serial number is required.");

        var normalized = InventorySerialNumber.Normalize(request.SerialNumber);
        var serial = await dbContext.InventorySerialNumbers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId
                && x.SerialNumber == normalized
                && (!request.ProductSkuId.HasValue || x.ProductSkuId == request.ProductSkuId.Value),
                cancellationToken)
            ?? throw new NotFoundException($"Serial number not found: {normalized}");

        var movementIds = await dbContext.StockMovementSerials.AsNoTracking()
            .Where(x => x.InventorySerialNumberId == serial.Id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.StockMovementId)
            .ToListAsync(cancellationToken);

        var movements = await dbContext.StockMovements.AsNoTracking()
            .Where(x => movementIds.Contains(x.Id))
            .OrderBy(x => x.CreatedAt)
            .Select(x => new SerialTraceMovementRow(
                x.Id,
                x.ProductId,
                x.ProductSkuId,
                x.WarehouseId,
                x.BatchId,
                x.ReferenceNumber,
                x.SourceDocumentType,
                x.NormalizedQuantity,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new SerialNumberTraceResult(
            serial.Id,
            serial.SerialNumber,
            serial.ProductSkuId,
            (int)serial.Status,
            movements);
    }
}
