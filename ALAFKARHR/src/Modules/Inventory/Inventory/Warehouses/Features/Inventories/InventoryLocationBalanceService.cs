namespace Inventory.Warehouses.Features.Inventories;

internal static class InventoryLocationBalanceService
{
    public static async Task<Guid?> ResolveDestinationLocationAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid warehouseId,
        Guid productId,
        Guid productSkuId,
        Guid? requestedLocationId,
        CancellationToken cancellationToken)
    {
        if (requestedLocationId.HasValue && requestedLocationId.Value != Guid.Empty)
        {
            await EnsureLocationAsync(dbContext, companyId, warehouseId, requestedLocationId.Value, "Destination location", cancellationToken);
            return requestedLocationId.Value;
        }

        var suggestion = await global::Inventory.Warehouses.Features.InventoryControls.PutawaySuggestionResolver.ResolveAsync(
            dbContext,
            companyId,
            warehouseId,
            productId,
            productSkuId,
            cancellationToken);

        return suggestion.DestinationLocationId;
    }

    public static async Task<Guid?> ResolveSourceLocationAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid warehouseId,
        Guid productSkuId,
        Guid batchId,
        Guid? requestedLocationId,
        decimal quantity,
        bool requireReserved,
        CancellationToken cancellationToken)
    {
        if (requestedLocationId.HasValue && requestedLocationId.Value != Guid.Empty)
        {
            await EnsureLocationAsync(dbContext, companyId, warehouseId, requestedLocationId.Value, "Source location", cancellationToken);
            return requestedLocationId.Value;
        }

        var balances = await dbContext.InventoryLocationBalances
            .Include(x => x.Batch)
            .Where(x => x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.ProductSkuId == productSkuId
                && x.BatchId == batchId
                && !x.IsDeleted)
            .OrderBy(x => x.Batch.ExpiryDate)
            .ThenBy(x => x.WarehouseLocationId)
            .ToListAsync(cancellationToken);

        if (balances.Count == 0)
            return null;

        var balance = balances.FirstOrDefault(x => (requireReserved ? x.ReservedQuantity : x.AvailableQuantity) >= quantity);
        if (balance is null)
            throw new BadRequestException("Insufficient stock in any active location for the selected batch.");

        await EnsureLocationAsync(dbContext, companyId, warehouseId, balance.WarehouseLocationId, "Source location", cancellationToken);
        return balance.WarehouseLocationId;
    }

    public static async Task IncreaseAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid? warehouseLocationId,
        Guid batchId,
        decimal quantity,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!warehouseLocationId.HasValue || warehouseLocationId.Value == Guid.Empty)
            return;

        await EnsureLocationAsync(dbContext, companyId, warehouseId, warehouseLocationId.Value, "Destination location", cancellationToken);
        var balance = await LoadOrCreateAsync(dbContext, companyId, productId, productSkuId, warehouseId, warehouseLocationId.Value, batchId, userId, cancellationToken);
        balance.Increase(quantity, userId);
    }

    public static async Task DecreaseAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productSkuId,
        Guid warehouseId,
        Guid? warehouseLocationId,
        Guid batchId,
        decimal quantity,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!warehouseLocationId.HasValue || warehouseLocationId.Value == Guid.Empty)
            return;

        var balance = await LoadExistingAsync(dbContext, companyId, productSkuId, warehouseId, warehouseLocationId.Value, batchId, cancellationToken);
        balance.Decrease(quantity, userId);
    }

    public static async Task ReserveAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productSkuId,
        Guid warehouseId,
        Guid? warehouseLocationId,
        Guid batchId,
        decimal quantity,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!warehouseLocationId.HasValue || warehouseLocationId.Value == Guid.Empty)
            return;

        var balance = await LoadExistingAsync(dbContext, companyId, productSkuId, warehouseId, warehouseLocationId.Value, batchId, cancellationToken);
        balance.Reserve(quantity, userId);
    }

    public static async Task ReleaseAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productSkuId,
        Guid warehouseId,
        Guid? warehouseLocationId,
        Guid batchId,
        decimal quantity,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!warehouseLocationId.HasValue || warehouseLocationId.Value == Guid.Empty)
            return;

        var balance = await LoadExistingAsync(dbContext, companyId, productSkuId, warehouseId, warehouseLocationId.Value, batchId, cancellationToken);
        balance.Release(quantity, userId);
    }

    public static async Task ConsumeReservedAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productSkuId,
        Guid warehouseId,
        Guid? warehouseLocationId,
        Guid batchId,
        decimal quantity,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!warehouseLocationId.HasValue || warehouseLocationId.Value == Guid.Empty)
            return;

        var balance = await LoadExistingAsync(dbContext, companyId, productSkuId, warehouseId, warehouseLocationId.Value, batchId, cancellationToken);
        balance.ConsumeReserved(quantity, userId);
    }

    public static async Task AdjustToAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid warehouseLocationId,
        Guid batchId,
        decimal countedQuantity,
        string userId,
        CancellationToken cancellationToken)
    {
        await EnsureLocationAsync(dbContext, companyId, warehouseId, warehouseLocationId, "Cycle count location", cancellationToken);
        var balance = await LoadOrCreateAsync(dbContext, companyId, productId, productSkuId, warehouseId, warehouseLocationId, batchId, userId, cancellationToken);
        balance.AdjustTo(countedQuantity, userId);
    }

    private static async Task<InventoryLocationBalance> LoadOrCreateAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid warehouseLocationId,
        Guid batchId,
        string userId,
        CancellationToken cancellationToken)
    {
        var balance = await dbContext.InventoryLocationBalances
            .FirstOrDefaultAsync(x => x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.WarehouseLocationId == warehouseLocationId
                && x.ProductSkuId == productSkuId
                && x.BatchId == batchId,
                cancellationToken);

        if (balance is not null)
            return balance;

        balance = InventoryLocationBalance.Create(companyId, productId, productSkuId, warehouseId, warehouseLocationId, batchId, userId);
        await dbContext.InventoryLocationBalances.AddAsync(balance, cancellationToken);
        return balance;
    }

    private static async Task<InventoryLocationBalance> LoadExistingAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid productSkuId,
        Guid warehouseId,
        Guid warehouseLocationId,
        Guid batchId,
        CancellationToken cancellationToken) =>
        await dbContext.InventoryLocationBalances
            .FirstOrDefaultAsync(x => x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.WarehouseLocationId == warehouseLocationId
                && x.ProductSkuId == productSkuId
                && x.BatchId == batchId,
                cancellationToken)
        ?? throw new BadRequestException("No location stock exists for the selected SKU, batch, and location.");

    private static async Task EnsureLocationAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid warehouseId,
        Guid warehouseLocationId,
        string label,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.WarehouseLocations.AsNoTracking()
            .AnyAsync(x => x.Id == warehouseLocationId
                && x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.IsActive
                && !x.IsDeleted,
                cancellationToken);

        if (!exists)
            throw new BadRequestException($"{label} is inactive or does not belong to the selected warehouse.");
    }
}
