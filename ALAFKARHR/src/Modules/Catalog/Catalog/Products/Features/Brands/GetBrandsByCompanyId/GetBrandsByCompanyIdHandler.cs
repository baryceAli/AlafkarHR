namespace Catalog.Products.Features.Brands.GetBrandsByCompanyId;


public record GetBrandsByCompanyIdQuery(Guid companyId, PaginationRequest PaginationRequest) : IQuery<GetBrandsByCompanyIdResult>;
public record GetBrandsByCompanyIdResult(PaginatedResult<BrandDto> BrandList);
public class GetBrandsByCompanyIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetBrandsByCompanyIdQuery, GetBrandsByCompanyIdResult>
{
    public async Task<GetBrandsByCompanyIdResult> Handle(GetBrandsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Brands.AsQueryable();
        query = query.Where(b => b.CompanyId == request.companyId);

    
        if(!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.Where(b => b.Name.Contains(request.PaginationRequest.SearchText)
                                    || b.NameEng.Contains(request.PaginationRequest.SearchText)

                    );
        }

        long count =await query.LongCountAsync(cancellationToken);

        var brands = await query
            .OrderBy(b => b.Name) // default sorting (important!)
            .ThenBy(b => b.NameEng)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                NameEng = b.NameEng,
                Description = b.Description,
                CompanyId = b.CompanyId
            })
            .ToListAsync(cancellationToken);


        return new GetBrandsByCompanyIdResult(
                                new PaginatedResult<BrandDto>(
                                            request.PaginationRequest.PageIndex,
                                            request.PaginationRequest.PageSize,
                                            count, brands));
    }
}
