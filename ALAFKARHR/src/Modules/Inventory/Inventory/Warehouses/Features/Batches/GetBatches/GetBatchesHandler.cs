using Catalog.Contracts.Products.Features.GetProductByCompany;

namespace Inventory.Warehouses.Features.Batches.GetBatches;

public record GetBatchesQuery(Guid CompanyId,PaginationRequest PaginationRequest) : IQuery<GetBatchesResult>;
public record GetBatchesResult(PaginatedResult<BatchDto> BatchList);

public class GetBatchesHandler(InventoryDbContext dbContext, ISender sender) : IQueryHandler<GetBatchesQuery, GetBatchesResult>
{
    public async Task<GetBatchesResult> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Batches.AsNoTracking().AsQueryable();
        var prodResult = await sender.Send(new GetProductByCompanyQuery(request.CompanyId, new PaginationRequest(0, 10000)));
        var products = prodResult.ProductList.Data;
        query=query.Where(b=>!b.IsDeleted && b.CompanyId==request.CompanyId);

        var searchText=request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query=query.Where(b=>b.BatchNumber.ToLower().Contains(searchText.ToLower()));
        }

        //var q = dbContext.Batches
        //    .AsNoTracking()
        //    .Where(x => x.DeletedAt == null)
        //    .OrderByDescending(x => x.CreatedAt);

        var count = await query.LongCountAsync(cancellationToken);

        var data = await query
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(b => new BatchDto{
                Id= b.Id,
                //b.WarehouseId,
                ProductId= b.ProductId,
                ProductSkuId= b.ProductSkuId,
                BatchNumber= b.BatchNumber,
               ManufacturingDate= b.ManufacturingDate,
                ExpiryDate= b.ExpiryDate,
                CompanyId= b.CompanyId,
                CreatedAt= b.CreatedAt ?? DateTime.MinValue,
                CreatedBy= b.CreatedBy ?? string.Empty,
                LastModified= b.ModifiedAt,
                LastModifiedBy= b.ModifiedBy,
                DeletedAt= b.DeletedAt,
                DeletedBy= b.DeletedBy
            })
            .ToListAsync(cancellationToken);
        //data.OrderBy(x => x.ProductId);
        List<BatchDto> batchDto = (from b in data
                       join p in products on b.ProductId equals p.Id
                       select new BatchDto
                       {
                           BatchNumber = b.BatchNumber,
                           CompanyId = b.CompanyId,
                           Id = b.Id,
                           ExpiryDate = b.ExpiryDate,
                           ManufacturingDate = b.ManufacturingDate,
                           ProductId = b.ProductId,
                           ProductName = p.Name,
                           ProductNameEng = p.NameEng,
                           ProductSkuId = b.ProductSkuId,
                           SkuName = p.Skus.FirstOrDefault(s => s.Id == b.ProductSkuId)?.Name ?? null,
                           SkuNameEng = p.Skus.FirstOrDefault(s => s.Id == b.ProductSkuId)?.NameEng ?? null,
                           
                           
                       }).ToList();
        return new GetBatchesResult(
            new PaginatedResult<BatchDto>(
                request.PaginationRequest.PageIndex, 
                request.PaginationRequest.PageSize, 
                count, 
                batchDto));
    }
}
