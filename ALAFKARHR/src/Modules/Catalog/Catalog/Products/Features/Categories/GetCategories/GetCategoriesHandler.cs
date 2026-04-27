namespace Catalog.Products.Features.Categories.GetCategories;

public record GetCategoriesQuery(PaginationRequest PaginationRequest) : IQuery<GetCategoriesResult>;
public record GetCategoriesResult(PaginatedResult<CategoryDto> CategoryList);

public class GetCategoriesHandler (CatalogDbContext dbContext)
    : IQueryHandler<GetCategoriesQuery, GetCategoriesResult>
{
    public async Task<GetCategoriesResult> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex=request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;
        var totalCount = await dbContext.Categories.LongCountAsync(cancellationToken);

        var categories= await dbContext.Categories
            .Where(x=> x.IsDeleted==false)
            .AsNoTracking()
            .OrderBy(x=> x.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();
        
        var categoryDtos = categories.Adapt<List<CategoryDto>>();

        return new GetCategoriesResult(
            new PaginatedResult<CategoryDto>
            (pageIndex, pageSize, totalCount, categoryDtos));
    }
}
