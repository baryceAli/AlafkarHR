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
        if (!request.PaginationRequest.IncludeInactive)
            query = query.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.Trim();
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.NameEng.Contains(search) ||
                (x.Barcode != null && x.Barcode.Contains(search)) ||
                (x.Notes != null && x.Notes.Contains(search)));
        }

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
                Weight = package.Weight,
                Length = package.Length,
                Width = package.Width,
                Height = package.Height,
                Notes = package.Notes,
                IsActive = package.IsActive,
                CompanyId = package.CompanyId
            })
            .ToListAsync(cancellationToken);

        return new GetProductPackagesByCompanyResult(
            new PaginatedResult<ProductPackageDto> (pageIndex,pageSize,totalCount,productPackages));
    }
}
