namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventories;

public record GetInventoriesQuery(Guid CompanyId, PaginationRequest PaginationRequest, Guid? BranchId) : IQuery<GetInventoriesResult>;
public record GetInventoriesResult(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesHandler(InventoryDbContext dbContext, ISender sender) : IQueryHandler<GetInventoriesQuery, GetInventoriesResult>
{
    public async Task<GetInventoriesResult> Handle(GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        var q = dbContext.Inventories
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.CreatedAt);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's inventory.");

        var warehouseQuery = dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (branchAccess.CanViewAllBranches)
        {
            if (request.BranchId.HasValue)
                warehouseQuery = warehouseQuery.Where(x => x.BranchId == request.BranchId.Value);
        }
        else
        {
            warehouseQuery = request.BranchId.HasValue
                ? warehouseQuery.Where(x => x.BranchId == null || x.BranchId == request.BranchId.Value)
                : warehouseQuery.Where(x => x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)));
        }

        var readableWarehouseIds = warehouseQuery.Select(x => x.Id);
        q = q.Where(x => readableWarehouseIds.Contains(x.WarehouseId))
            .OrderByDescending(x => x.CreatedAt);

        var count = await q.LongCountAsync(cancellationToken);
        var data = await q
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync();

        var warehouses = await warehouseQuery
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
        var inventoryDtos = data.Adapt<List<InventoryAggregateDto>>();
        foreach (var dto in inventoryDtos)
        {
            if (!warehouses.TryGetValue(dto.WarehouseId, out var warehouse))
                continue;

            dto.WarehouseName = warehouse.Name;
            dto.WarehouseNameEng = warehouse.NameEng;
            dto.BranchId = warehouse.BranchId;
        }


        return new GetInventoriesResult(
            new PaginatedResult<InventoryAggregateDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, inventoryDtos));




    }
}
