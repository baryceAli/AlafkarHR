namespace Inventory.Warehouses.Features.Batches.GetBatches;

public record GetBatchesQuery(PaginationRequest Request) : IQuery<GetBatchesResult>;
public record GetBatchesResult(PaginatedResult<BatchDto> BatchList);

public class GetBatchesHandler(InventoryDbContext dbContext) : IQueryHandler<GetBatchesQuery, GetBatchesResult>
{
    public async Task<GetBatchesResult> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        var q = dbContext.Batches
            .AsNoTracking()
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt);

        var count = await q.LongCountAsync(cancellationToken);

        var data = await q
            .Skip(request.Request.PageIndex * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(b => new BatchDto(
                b.Id,
                //b.WarehouseId,
                b.ProductId,
                b.ProductSkuId,
                b.BatchNumber,
                b.ManufacturingDate,
                b.ExpiryDate,
                b.CreatedAt ?? DateTime.MinValue,
                b.CreatedBy ?? string.Empty,
                b.ModifiedAt,
                b.ModifiedBy,
                b.DeletedAt,
                b.DeletedBy
            ))
            .ToListAsync(cancellationToken);
            //data.OrderBy(x => x.ProductId);

        return new GetBatchesResult(
            new PaginatedResult<BatchDto>(
                request.Request.PageIndex, 
                request.Request.PageSize, 
                count, 
                data));
    }
}
