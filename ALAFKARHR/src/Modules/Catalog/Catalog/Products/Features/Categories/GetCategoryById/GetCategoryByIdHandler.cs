namespace Catalog.Products.Features.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id):IQuery<GetCategoryByIdResult>;
public record GetCategoryByIdResult(CategoryDto Category);

//public class GetCategoryByIdCommandValidator:AbstractValidator<GetCategoryByIdQuery>
public class GetCategoryByIdHandler (CatalogDbContext dbContext)
    : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
{
    public async Task<GetCategoryByIdResult> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category=await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x=> x.Id==request.Id && x.IsDeleted==false, cancellationToken);
        if (category is null)
            throw new Exception($"Category not found: {request.Id}");

        return new GetCategoryByIdResult(category.Adapt<CategoryDto>());
    }
}
