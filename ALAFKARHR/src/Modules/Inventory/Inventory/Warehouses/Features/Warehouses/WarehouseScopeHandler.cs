namespace Inventory.Warehouses.Features.Warehouses;

public class WarehouseScopeHandler(InventoryDbContext dbContext)
    : IQueryHandler<EnsureWarehouseBranchScopeQuery, EnsureWarehouseBranchScopeResult>
{
    public async Task<EnsureWarehouseBranchScopeResult> Handle(EnsureWarehouseBranchScopeQuery request, CancellationToken cancellationToken)
    {
        var isValid = await dbContext.Warehouses.AsNoTracking()
            .AnyAsync(x => x.Id == request.WarehouseId
                && x.CompanyId == request.CompanyId
                && x.BranchId == request.BranchId
                && !x.IsDeleted, cancellationToken);

        if (!isValid)
            throw new BadRequestException("Default warehouse must belong to the StoreFront branch.");

        return new EnsureWarehouseBranchScopeResult(true);
    }
}
