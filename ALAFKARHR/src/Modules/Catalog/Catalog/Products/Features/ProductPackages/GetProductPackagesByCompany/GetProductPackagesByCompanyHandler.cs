namespace Catalog.Products.Features.ProductPackages.GetProductPackagesByCompany;


public record GetProductPackagesByCompanyQuery(Guid CompanyId,PaginationRequest PaginationRequest) : IQuery<GetProductPackagesByCompanyResult>;
public record GetProductPackagesByCompanyResult(PaginatedResult<ProductPackageDto> ProductPackageList);
public class GetProductPackagesByCompanyHandler(CatalogDbContext dbContext) 
    : IQueryHandler<GetProductPackagesByCompanyQuery, GetProductPackagesByCompanyResult>
{
    public async Task<GetProductPackagesByCompanyResult> Handle(GetProductPackagesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var pageIndex=request.PaginationRequest.PageIndex;
        var pageSize=request.PaginationRequest.PageSize;


        var query = dbContext.ProductPackages.AsQueryable();
        query = query.Where(x => x.IsDeleted == false && x.CompanyId == request.CompanyId);

        var totalCount = await dbContext.ProductPackages.AsNoTracking().LongCountAsync();
        
        var productPackages= await query
            .AsNoTracking()
            .Skip(pageSize*pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return new GetProductPackagesByCompanyResult(
            new PaginatedResult<ProductPackageDto> (pageIndex,pageSize,totalCount,productPackages.Adapt<List<ProductPackageDto>>()));
    }
}
