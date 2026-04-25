using Mapster;

namespace Catalog.Products.Features.ProductPackages.GetProductPackages;


public record GetProductPackagesQuery(PaginationRequest PaginationRequest) : IQuery<GetProductPackagesResult>;
public record GetProductPackagesResult(PaginatedResult<ProductPackageDto> ProductPackageList);
public class GetProductPackagesHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductPackagesQuery, GetProductPackagesResult>
{
    public async Task<GetProductPackagesResult> Handle(GetProductPackagesQuery query, CancellationToken cancellationToken)
    {
        var pageIndex=query.PaginationRequest.PageIndex;
        var pageSize=query.PaginationRequest.PageSize;
        var totalCount = await dbContext.ProductPackages.AsNoTracking().LongCountAsync();

        var productPackages=await dbContext.ProductPackages
            .AsNoTracking()
            .Where(x=> x.DeletedAt==null)
            .Skip(pageSize*pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return new GetProductPackagesResult(
            new PaginatedResult<ProductPackageDto> (pageIndex,pageSize,totalCount,productPackages.Adapt<List<ProductPackageDto>>()));
    }
}
