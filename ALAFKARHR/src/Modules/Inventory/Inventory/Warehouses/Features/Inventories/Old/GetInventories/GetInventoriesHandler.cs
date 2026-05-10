namespace Inventory.Warehouses.Features.Inventories.GetInventories;

public record GetInventoriesQuery(PaginationRequest PaginationRequest) : IQuery<GetInventoriesResult>;
public record GetInventoriesResult(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesHandler(InventoryDbContext dbContext) : IQueryHandler<GetInventoriesQuery, GetInventoriesResult>
{
    public async Task<GetInventoriesResult> Handle(GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        var q = dbContext.Inventories
            .AsNoTracking()
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt);

        var count = await q.LongCountAsync(cancellationToken);
        var data = await q
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync();


        return new GetInventoriesResult(
            new PaginatedResult<InventoryAggregateDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, data.Adapt<List<InventoryAggregateDto>>()));




    }
}
