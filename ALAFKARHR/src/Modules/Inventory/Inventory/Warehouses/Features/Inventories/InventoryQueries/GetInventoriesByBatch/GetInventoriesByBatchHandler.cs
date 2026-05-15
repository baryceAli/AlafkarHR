using Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventoriesByCompany;

namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventoriesByBatch;


public record GetInventoriesByBatchQuery(Guid BatchId,PaginationRequest PaginationRequest) : IQuery<GetInventoriesByBatchResult>;
public record GetInventoriesByBatchResult(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesByBatchHandler(InventoryDbContext dbContext) : IQueryHandler<GetInventoriesByBatchQuery, GetInventoriesByBatchResult>
{
    public async Task<GetInventoriesByBatchResult> Handle(GetInventoriesByBatchQuery request, CancellationToken cancellationToken)
    {
        var q = dbContext.Inventories
            .Include(x => x.Batches)
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Batches.Any(b => b.Id == request.BatchId));

        string searchText = request.PaginationRequest.SearchText.ToLower();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            //q=q.Where(x=> x.)
        }
        var count = await q.LongCountAsync(cancellationToken);
        var data = await q
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();


        return new GetInventoriesByBatchResult(
            new PaginatedResult<InventoryAggregateDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, data.Adapt<List<InventoryAggregateDto>>()));



    }
}
