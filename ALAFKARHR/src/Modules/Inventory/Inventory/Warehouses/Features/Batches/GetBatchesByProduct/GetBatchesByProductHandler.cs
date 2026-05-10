namespace Inventory.Warehouses.Features.Batches.GetBatchesByProduct;

public record GetBatchesByProductQuery(Guid ProductId, PaginationRequest PaginationRequest) : IQuery<GetBatchesByProductResult>;
public record GetBatchesByProductResult(PaginatedResult<BatchDto> BatchList);

public class GetBatchesByProductHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetBatchesByProductQuery, GetBatchesByProductResult>
{
    public async Task<GetBatchesByProductResult> Handle(GetBatchesByProductQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Batches.AsNoTracking().AsQueryable();

        query = query.Where(b => !b.IsDeleted && b.ProductId == request.ProductId);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(b => b.BatchNumber.ToLower().Contains(searchText.ToLower()));
        }

        //var q = dbContext.Batches
        //    .AsNoTracking()
        //    .Where(x => x.DeletedAt == null)
        //    .OrderByDescending(x => x.CreatedAt);

        var count = await query.LongCountAsync(cancellationToken);

        var data = await query
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(b => new BatchDto
            {
                Id = b.Id,
                //b.WarehouseId,
                ProductId = b.ProductId,
                ProductSkuId = b.ProductSkuId,
                BatchNumber = b.BatchNumber,
                ManufacturingDate = b.ManufacturingDate,
                ExpiryDate = b.ExpiryDate,
                CreatedAt = b.CreatedAt ?? DateTime.MinValue,
                CreatedBy = b.CreatedBy ?? string.Empty,
                LastModified = b.ModifiedAt,
                LastModifiedBy = b.ModifiedBy,
                DeletedAt = b.DeletedAt,
                DeletedBy = b.DeletedBy
            })
            .OrderByDescending(b=> b.CreatedAt)
            .ThenBy(b=>b.BatchNumber)
            .ToListAsync(cancellationToken);
        //data.OrderBy(x => x.ProductId);

        return new GetBatchesByProductResult(
            new PaginatedResult<BatchDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                data));
    }
}

