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

        var totalCount = await query.AsNoTracking().LongCountAsync(cancellationToken);
        
        var productPackages= await query
            .AsNoTracking()
            .Skip(pageSize*pageIndex)
            .Take(pageSize)
            .Select(package => new ProductPackageDto
            {
                Id = package.Id,
                Name = package.Name,
                NameEng = package.NameEng,
                Quantity = package.Quantity,
                UnitId = package.UnitId,
                Barcode = package.Barcode,
                CompanyId = package.CompanyId
            })
            .ToListAsync(cancellationToken);

        return new GetProductPackagesByCompanyResult(
            new PaginatedResult<ProductPackageDto> (pageIndex,pageSize,totalCount,productPackages));
    }
}
