namespace Inventory.Warehouses.Features.Warehouses.GetWarehouses;

public record GetWarehousesQuery(Guid CompanyId, PaginationRequest PaginationRequest, WarehouseType? WarehouseType, Guid? BranchId) : IQuery<GetWarehousesResult>;
public record GetWarehousesResult(PaginatedResult<WarehouseDto> WarehouseList);
public class GetWarehousesHandler (InventoryDbContext dbContext, ISender sender) : IQueryHandler<GetWarehousesQuery, GetWarehousesResult>
{
    public async Task<GetWarehousesResult> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex= request.PaginationRequest.PageIndex;
        var pageSize= request.PaginationRequest.PageSize;

        var query = dbContext.Warehouses.AsNoTracking().AsQueryable();
        query=query.Where(w=>!w.IsDeleted && w.CompanyId==request.CompanyId);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (branchAccess.CanViewAllBranches)
        {
            if (request.BranchId.HasValue)
                query = query.Where(w => w.BranchId == request.BranchId.Value);
        }
        else
        {
            if (!BranchScopePolicy.CanFilter(branchAccess, request.BranchId))
                throw new ForbiddenException("You do not have permission to view this branch's warehouses.");

            query = request.BranchId.HasValue
                ? query.Where(w => w.BranchId == null || w.BranchId == request.BranchId.Value)
                : query.Where(w => w.BranchId == null || (w.BranchId.HasValue && branchAccess.BranchIds.Contains(w.BranchId.Value)));
        }

        if (request.WarehouseType.HasValue)
        {
            query = query.Where(w => w.WarehouseType == request.WarehouseType.Value);
        }
        var searchText=request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace( searchText))
        {
            query=query.Where(w=> w.Name.ToLower().Contains(searchText.ToLower()));
        }

        var totalCount = await query
            .LongCountAsync();

        var warehouses = await query
            .AsNoTracking()
            .OrderBy(x=>x.Name)
            .Skip(pageIndex*pageSize)
            .Take(pageSize)
            .ToListAsync();

        var warehouseDtos = warehouses.Adapt<List<WarehouseDto>>();
        return new GetWarehousesResult(new PaginatedResult<WarehouseDto>(
            pageIndex: pageIndex,
            pageSize: pageSize,
            count: totalCount,
            data: warehouseDtos
        ));
    }
}
