namespace Inventory.Warehouses.Features.Warehouses.GetWarehouses;

public record GetWarehousesQuery(PaginationRequest PaginationRequest) : IQuery<GetWarehousesResult>;
public record GetWarehousesResult(PaginatedResult<WarehouseDto> WarehouseList);
public class GetWarehousesHandler (InventoryDbContext dbContext) : IQueryHandler<GetWarehousesQuery, GetWarehousesResult>
{
    public async Task<GetWarehousesResult> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex= request.PaginationRequest.PageIndex;
        var pageSize= request.PaginationRequest.PageSize;
        var totalCount = await dbContext.Warehouses
            .AsNoTracking()
            .LongCountAsync();

        var warehouses= await dbContext.Warehouses
            .AsNoTracking()
            .Where(x=> x.DeletedAt==null)
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
