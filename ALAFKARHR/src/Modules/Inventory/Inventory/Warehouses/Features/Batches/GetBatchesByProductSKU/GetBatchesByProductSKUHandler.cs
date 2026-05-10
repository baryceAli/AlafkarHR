namespace Inventory.Warehouses.Features.Batches.GetBatchesByProductSKU;

public record GetBatchesByProductSKUQuery(Guid ProductSKUId, PaginationRequest PaginationRequest) : IQuery<GetBatchesByProductSKUResult>;
public record GetBatchesByProductSKUResult(PaginatedResult<BatchDto> BatchList);

public class GetBatchesByProductSKUHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetBatchesByProductSKUQuery, GetBatchesByProductSKUResult>
{
    public async Task<GetBatchesByProductSKUResult> Handle(GetBatchesByProductSKUQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Batches.AsNoTracking().AsQueryable();

        query = query.Where(b => !b.IsDeleted && b.ProductSkuId == request.ProductSKUId);

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
            .OrderByDescending(b => b.CreatedAt)
            .ThenBy(b => b.BatchNumber)
            .ToListAsync(cancellationToken);
        //data.OrderBy(x => x.ProductId);

        return new GetBatchesByProductSKUResult(
            new PaginatedResult<BatchDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                data));
    }
}

