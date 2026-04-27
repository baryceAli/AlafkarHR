using Catalog.Products.Features.Categories.GetCategories;

namespace Catalog.Products.Features.Categories.GetCategoriesByCompany;


public record GetCategoriesByCompanyQuery(Guid companyId, PaginationRequest PaginationRequest) : IQuery<GetCategoriesByCompanyResult>;
public record GetCategoriesByCompanyResult(PaginatedResult<CategoryDto> CategoryList);
public class GetCategoriesByCompanyHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetCategoriesByCompanyQuery, GetCategoriesByCompanyResult>
{
    public async Task<GetCategoriesByCompanyResult> Handle(GetCategoriesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Categories.AsQueryable();
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;

        query = query.Where(x => x.CompanyId == request.companyId && x.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            string searchText = request.PaginationRequest.SearchText.ToLower();
            query= query.Where(x=> 
                                    x.Name.ToLower().Contains(searchText)||
                                    x.NameEng.ToLower().Contains(searchText));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var categories = await query
            .OrderBy(b => b.Name) // default sorting (important!)
            .ThenBy(b => b.NameEng)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(b => new CategoryDto
            {
                Id = b.Id,
                Name = b.Name,
                NameEng = b.NameEng,
                Description = b.Description,
                CompanyId = b.CompanyId
            })
            .ToListAsync(cancellationToken);

        //var categoryDtos = categories.Adapt<List<CategoryDto>>();

        return new GetCategoriesByCompanyResult(
            new PaginatedResult<CategoryDto>
            (pageIndex, pageSize, totalCount, categories));



    }
}
