namespace Inventory.Warehouses.Features.Warehouses.GetWarehouses;

public record GetWarehousesQuery(Guid CompanyId, PaginationRequest PaginationRequest, WarehouseType? WarehouseType) : IQuery<GetWarehousesResult>;
public record GetWarehousesResult(PaginatedResult<WarehouseDto> WarehouseList);
public class GetWarehousesHandler (InventoryDbContext dbContext) : IQueryHandler<GetWarehousesQuery, GetWarehousesResult>
{
    public async Task<GetWarehousesResult> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex= request.PaginationRequest.PageIndex;
        var pageSize= request.PaginationRequest.PageSize;

        var query = dbContext.Warehouses.AsNoTracking().AsQueryable();
        query=query.Where(w=>!w.IsDeleted && w.CompanyId==request.CompanyId);
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
            .Skip(pageIndex*pageSize)
            .Take(pageSize)
            .OrderBy(x=>x.Name)
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
