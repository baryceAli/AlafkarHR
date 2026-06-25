namespace Inventory.Warehouses.Features.Inventories;

internal static class InventoryBranchScope
{
    public static async Task<Warehouse> EnsureCanMutateWarehouseAsync(
        InventoryDbContext dbContext,
        ISender sender,
        Guid companyId,
        Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        if (!warehouseId.HasValue)
            throw new BadRequestException("Warehouse is required.");

        var warehouse = await dbContext.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == warehouseId.Value && x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Warehouse not found: {warehouseId.Value}");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, warehouse.BranchId))
            throw new ForbiddenException("You do not have permission to change stock in this warehouse branch scope.");

        return warehouse;
    }

    public static async Task<Warehouse> EnsureCanReadWarehouseAsync(
        InventoryDbContext dbContext,
        ISender sender,
        Guid companyId,
        Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        if (!warehouseId.HasValue)
            throw new BadRequestException("Warehouse is required.");

        var warehouse = await dbContext.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == warehouseId.Value && x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Warehouse not found: {warehouseId.Value}");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, warehouse.BranchId))
            throw new ForbiddenException("You do not have permission to view stock in this warehouse branch scope.");

        return warehouse;
    }
}
