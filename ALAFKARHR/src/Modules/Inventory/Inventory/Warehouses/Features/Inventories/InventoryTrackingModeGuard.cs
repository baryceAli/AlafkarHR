namespace Inventory.Warehouses.Features.Inventories;

public sealed record InventoryTrackingResolution(
    GetProductSkuInventoryContextResult Context,
    InventoryPackageQuantityResult Quantity,
    IReadOnlyList<InventorySerialSelectionDto> Serials);

public static class InventoryTrackingModeGuard
{
    public static async Task<InventoryTrackingResolution> ResolveAndValidateAsync(
        InventoryDbContext dbContext,
        ISender sender,
        CreateInventoryAggregateDto inventoryAggregate,
        InventorySerialOperation operation,
        string userId,
        CancellationToken cancellationToken)
    {
        var packageQuantity = await InventoryPackageQuantityResolver.ResolveAsync(sender, inventoryAggregate, cancellationToken);
        var context = await sender.Send(
            new GetProductSkuInventoryContextQuery(inventoryAggregate.CompanyId, packageQuantity.ProductSkuId),
            cancellationToken);

        if (context.TrackingMode == CatalogTrackingMode.Quantity && inventoryAggregate.InitialBatchId == Guid.Empty)
        {
            inventoryAggregate.InitialBatchId = await EnsureQuantityCompatibilityBatchAsync(
                dbContext,
                inventoryAggregate.CompanyId,
                packageQuantity.ProductId,
                packageQuantity.ProductSkuId,
                userId,
                cancellationToken);
        }

        ValidateBatchRequirement(context.TrackingMode, inventoryAggregate.InitialBatchId, operation);
        if (context.TrackingMode == CatalogTrackingMode.Serial && NormalizeSerials(inventoryAggregate.SerialNumbers).Count == 0)
        {
            inventoryAggregate.SerialNumbers = await AutoSelectSerialsAsync(
                dbContext,
                inventoryAggregate,
                packageQuantity,
                operation,
                cancellationToken);
        }
        ValidateSerials(context.TrackingMode, packageQuantity.NormalizedQuantity, inventoryAggregate.SerialNumbers);

        if (context.TrackingMode == CatalogTrackingMode.Serial)
        {
            await ValidateSerialOwnershipAsync(dbContext, inventoryAggregate, packageQuantity, operation, cancellationToken);
        }

        return new InventoryTrackingResolution(context, packageQuantity, NormalizeSerials(inventoryAggregate.SerialNumbers));
    }

    public static async Task ApplySerialMovementAsync(
        InventoryDbContext dbContext,
        StockMovement movement,
        Guid companyId,
        Guid? locationId,
        InventorySerialOperation operation,
        IReadOnlyList<InventorySerialSelectionDto> serials,
        string userId,
        CancellationToken cancellationToken)
    {
        if (serials.Count == 0)
            return;

        var normalized = NormalizeSerials(serials);
        var numbers = normalized.Select(x => InventorySerialNumber.Normalize(x.SerialNumber)).ToList();
        var existing = await dbContext.InventorySerialNumbers
            .Where(x => x.CompanyId == companyId
                && x.ProductSkuId == movement.ProductSkuId
                && numbers.Contains(x.SerialNumber))
            .ToListAsync(cancellationToken);

        foreach (var serial in normalized)
        {
            var serialNumber = InventorySerialNumber.Normalize(serial.SerialNumber);
            var entity = existing.FirstOrDefault(x => x.SerialNumber == serialNumber);

            if (operation is InventorySerialOperation.StockIn or InventorySerialOperation.Return)
            {
                if (entity is null)
                {
                    entity = InventorySerialNumber.Create(
                        companyId,
                        movement.ProductId,
                        movement.ProductSkuId,
                        serialNumber,
                        serial.BatchId ?? movement.BatchId,
                        movement.WarehouseId,
                        locationId,
                        movement.SourceDocumentId,
                        movement.SourceDocumentLineId,
                        movement.Id,
                        userId);
                    await dbContext.InventorySerialNumbers.AddAsync(entity, cancellationToken);
                }
                else if (entity.Status is InventorySerialStatus.Available or InventorySerialStatus.Reserved)
                {
                    throw new BadRequestException($"Serial number '{serialNumber}' is already active for this SKU.");
                }
                else if (operation == InventorySerialOperation.Return)
                {
                    entity.ReturnToStock(serial.BatchId ?? movement.BatchId, movement.WarehouseId, locationId, movement.Id, userId);
                }
                else
                {
                    entity.Receive(serial.BatchId ?? movement.BatchId, movement.WarehouseId, locationId, movement.Id, userId);
                }
            }
            else
            {
                if (entity is null)
                    throw new BadRequestException($"Serial number '{serialNumber}' was not found for this SKU.");

                if (operation != InventorySerialOperation.TransferReceive)
                    EnsureSerialCanMove(entity, movement, locationId, operation);

                switch (operation)
                {
                    case InventorySerialOperation.Reserve:
                        entity.Reserve(movement.Id, userId);
                        break;
                    case InventorySerialOperation.Release:
                        entity.Release(movement.Id, userId);
                        break;
                    case InventorySerialOperation.StockOut:
                        entity.Consume(movement.Id, userId);
                        break;
                    case InventorySerialOperation.Scrap:
                        entity.Scrap(movement.Id, userId);
                        break;
                    case InventorySerialOperation.TransferShip:
                        entity.Move(entity.BatchId, null, null, InventorySerialStatus.Consumed, movement.Id, userId);
                        break;
                    case InventorySerialOperation.TransferReceive:
                        entity.Move(serial.BatchId ?? movement.BatchId, movement.WarehouseId, locationId, InventorySerialStatus.Available, movement.Id, userId);
                        break;
                }
            }

            await dbContext.StockMovementSerials.AddAsync(
                StockMovementSerial.Create(movement.Id, entity.Id, entity.SerialNumber, entity.Status, userId),
                cancellationToken);
        }
    }

    private static void ValidateBatchRequirement(CatalogTrackingMode trackingMode, Guid batchId, InventorySerialOperation operation)
    {
        if (trackingMode == CatalogTrackingMode.Batch && batchId == Guid.Empty)
            throw new BadRequestException("Batch is required for batch-tracked SKUs.");

        if (trackingMode == CatalogTrackingMode.Serial && batchId == Guid.Empty)
            throw new BadRequestException("Batch is required for serial-tracked SKUs so expiry and traceability remain auditable.");
    }

    public static async Task<Guid> EnsureQuantityCompatibilityBatchAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        string userId,
        CancellationToken cancellationToken)
    {
        var batchNumber = $"QTY-{productSkuId:N}";
        var existing = await dbContext.Batches
            .FirstOrDefaultAsync(x => x.CompanyId == companyId
                && x.ProductSkuId == productSkuId
                && x.BatchNumber == batchNumber
                && !x.IsDeleted, cancellationToken);

        if (existing is not null)
            return existing.Id;

        var batch = Batch.Create(
            Guid.NewGuid(),
            productId,
            productSkuId,
            batchNumber,
            DateTime.UtcNow.Date,
            new DateTime(9999, 12, 31),
            companyId,
            userId);

        await dbContext.Batches.AddAsync(batch, cancellationToken);
        return batch.Id;
    }

    private static void ValidateSerials(
        CatalogTrackingMode trackingMode,
        decimal normalizedQuantity,
        IReadOnlyCollection<InventorySerialSelectionDto> serials)
    {
        if (trackingMode != CatalogTrackingMode.Serial)
            return;

        if (normalizedQuantity != Math.Truncate(normalizedQuantity))
            throw new BadRequestException("Serial-tracked SKU quantity must resolve to a whole number after package and unit conversion.");

        var cleaned = NormalizeSerials(serials);
        if (cleaned.Count != normalizedQuantity)
            throw new BadRequestException("Serial number count must match the normalized quantity.");
    }

    private static async Task ValidateSerialOwnershipAsync(
        InventoryDbContext dbContext,
        CreateInventoryAggregateDto inventoryAggregate,
        InventoryPackageQuantityResult quantity,
        InventorySerialOperation operation,
        CancellationToken cancellationToken)
    {
        if (operation is InventorySerialOperation.StockIn or InventorySerialOperation.Return)
            return;

        var numbers = NormalizeSerials(inventoryAggregate.SerialNumbers)
            .Select(x => InventorySerialNumber.Normalize(x.SerialNumber))
            .ToList();

        var locationId = inventoryAggregate.SourceLocationId ?? inventoryAggregate.DestinationLocationId;
        var rows = await dbContext.InventorySerialNumbers.AsNoTracking()
            .Where(x => x.CompanyId == inventoryAggregate.CompanyId
                && x.ProductSkuId == quantity.ProductSkuId
                && numbers.Contains(x.SerialNumber))
            .ToListAsync(cancellationToken);

        foreach (var number in numbers)
        {
            var row = rows.FirstOrDefault(x => x.SerialNumber == number)
                ?? throw new BadRequestException($"Serial number '{number}' was not found for this SKU.");

            if (row.WarehouseId != inventoryAggregate.WarehouseId)
                throw new BadRequestException($"Serial number '{number}' is not available in the selected warehouse.");

            if (locationId.HasValue && row.WarehouseLocationId.HasValue && row.WarehouseLocationId != locationId)
                throw new BadRequestException($"Serial number '{number}' is not available in the selected location.");

            if (inventoryAggregate.InitialBatchId != Guid.Empty && row.BatchId.HasValue && row.BatchId != inventoryAggregate.InitialBatchId)
                throw new BadRequestException($"Serial number '{number}' does not belong to the selected batch.");
        }
    }

    private static async Task<List<InventorySerialSelectionDto>> AutoSelectSerialsAsync(
        InventoryDbContext dbContext,
        CreateInventoryAggregateDto inventoryAggregate,
        InventoryPackageQuantityResult quantity,
        InventorySerialOperation operation,
        CancellationToken cancellationToken)
    {
        if (operation is InventorySerialOperation.StockIn or InventorySerialOperation.Return or InventorySerialOperation.TransferReceive)
            return [];

        if (quantity.NormalizedQuantity != Math.Truncate(quantity.NormalizedQuantity))
            throw new BadRequestException("Serial-tracked SKU quantity must resolve to a whole number after package and unit conversion.");

        var required = (int)quantity.NormalizedQuantity;
        var locationId = inventoryAggregate.SourceLocationId ?? inventoryAggregate.DestinationLocationId;
        InventorySerialStatus[] allowedStatuses = operation switch
        {
            InventorySerialOperation.Release => [InventorySerialStatus.Reserved],
            InventorySerialOperation.StockOut when inventoryAggregate.ConsumeReservedQuantity => [InventorySerialStatus.Reserved],
            _ => [InventorySerialStatus.Available, InventorySerialStatus.Returned]
        };

        var query = from serial in dbContext.InventorySerialNumbers.AsNoTracking()
                    join batch in dbContext.Batches.AsNoTracking() on serial.BatchId equals batch.Id into batchJoin
                    from batch in batchJoin.DefaultIfEmpty()
                    where serial.CompanyId == inventoryAggregate.CompanyId
                        && serial.ProductSkuId == quantity.ProductSkuId
                        && serial.WarehouseId == inventoryAggregate.WarehouseId
                        && allowedStatuses.Contains(serial.Status)
                    select new { serial, batch };

        if (inventoryAggregate.InitialBatchId != Guid.Empty)
            query = query.Where(x => x.serial.BatchId == inventoryAggregate.InitialBatchId);
        if (locationId.HasValue)
            query = query.Where(x => x.serial.WarehouseLocationId == locationId.Value);

        var rows = await query
            .Where(x => x.batch == null || x.batch.ExpiryDate.Date >= DateTime.UtcNow.Date)
            .OrderBy(x => x.batch == null ? DateTime.MaxValue : x.batch.ExpiryDate)
            .ThenBy(x => x.serial.SerialNumber)
            .Take(required)
            .Select(x => new InventorySerialSelectionDto
            {
                InventorySerialNumberId = x.serial.Id,
                SerialNumber = x.serial.SerialNumber,
                BatchId = x.serial.BatchId,
                WarehouseId = x.serial.WarehouseId,
                WarehouseLocationId = x.serial.WarehouseLocationId
            })
            .ToListAsync(cancellationToken);

        if (rows.Count < required)
            throw new BadRequestException("Insufficient available serial numbers for this operation.");

        return rows;
    }

    private static void EnsureSerialCanMove(
        InventorySerialNumber serial,
        StockMovement movement,
        Guid? locationId,
        InventorySerialOperation operation)
    {
        if (serial.WarehouseId != movement.WarehouseId)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not in the selected warehouse.");

        if (locationId.HasValue && serial.WarehouseLocationId.HasValue && serial.WarehouseLocationId != locationId)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not in the selected location.");

        if (serial.BatchId.HasValue && serial.BatchId != movement.BatchId)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not in the selected batch.");

        if (operation == InventorySerialOperation.Reserve && serial.Status != InventorySerialStatus.Available && serial.Status != InventorySerialStatus.Returned)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not available for reservation.");

        if (operation == InventorySerialOperation.Release && serial.Status != InventorySerialStatus.Reserved)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not reserved.");

        if (operation == InventorySerialOperation.StockOut && serial.Status != InventorySerialStatus.Available && serial.Status != InventorySerialStatus.Returned && serial.Status != InventorySerialStatus.Reserved)
            throw new BadRequestException($"Serial number '{serial.SerialNumber}' is not available for stock-out.");
    }

    private static List<InventorySerialSelectionDto> NormalizeSerials(IReadOnlyCollection<InventorySerialSelectionDto> serials)
        => serials
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
            .GroupBy(x => InventorySerialNumber.Normalize(x.SerialNumber))
            .Select(group => WithNormalizedSerial(group.First(), group.Key))
            .ToList();

    private static InventorySerialSelectionDto WithNormalizedSerial(InventorySerialSelectionDto dto, string serialNumber)
        => new()
        {
            InventorySerialNumberId = dto.InventorySerialNumberId,
            SerialNumber = serialNumber,
            BatchId = dto.BatchId,
            WarehouseId = dto.WarehouseId,
            WarehouseLocationId = dto.WarehouseLocationId
        };
}

public enum InventorySerialOperation
{
    StockIn = 1,
    StockOut = 2,
    Reserve = 3,
    Release = 4,
    Return = 5,
    Scrap = 6,
    TransferShip = 7,
    TransferReceive = 8
}
