namespace Inventory.Warehouses.Features.InventoryControls;

public static class PutawaySuggestionResolver
{
    public static async Task<PutawaySuggestionDto> ResolveAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid warehouseId,
        Guid productId,
        Guid productSkuId,
        CancellationToken cancellationToken)
    {
        var candidates = await dbContext.PutawayRules.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.IsActive
                && !x.IsDeleted
                && x.DestinationLocationId.HasValue
                && (
                    x.ProductSkuId == productSkuId
                    || (x.ProductSkuId == null && x.ProductId == productId)
                    || (x.ProductSkuId == null && x.ProductId == null)))
            .Select(x => new
            {
                Rule = x,
                Specificity = x.ProductSkuId == productSkuId ? 0 : x.ProductId == productId ? 1 : 2
            })
            .OrderBy(x => x.Specificity)
            .ThenBy(x => x.Rule.Priority)
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            var locationId = candidate.Rule.DestinationLocationId!.Value;
            var location = await dbContext.WarehouseLocations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == locationId
                    && x.CompanyId == companyId
                    && x.WarehouseId == warehouseId
                    && x.IsActive
                    && !x.IsDeleted,
                    cancellationToken);

            if (location is null)
                continue;

            return new PutawaySuggestionDto
            {
                CompanyId = companyId,
                WarehouseId = warehouseId,
                ProductId = productId,
                ProductSkuId = productSkuId,
                PutawayRuleId = candidate.Rule.Id,
                DestinationLocationId = location.Id,
                DestinationLocationCode = location.Code,
                DestinationLocationName = location.Name,
                DestinationLocationNameEng = location.NameEng,
                Priority = candidate.Rule.Priority
            };
        }

        return new PutawaySuggestionDto
        {
            CompanyId = companyId,
            WarehouseId = warehouseId,
            ProductId = productId,
            ProductSkuId = productSkuId,
            Warning = "No active putaway destination is configured for this SKU/product/warehouse."
        };
    }
}
