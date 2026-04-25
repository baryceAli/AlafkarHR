namespace Catalog.Products.Features.Categories.RemoveCategory;

public record RemoveCategoryCommand(Guid Id, string DeletedBy) : ICommand<RemoveCategoryResult>;
public record RemoveCategoryResult(bool IsSuccess);

public class RemoveCategoryCommandValidator: AbstractValidator<RemoveCategoryCommand>
{
    public RemoveCategoryCommandValidator()
    {
        RuleFor(x=> x.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x=> x.DeletedBy).NotEmpty().WithMessage("DeletedBy is required");
    }
}
public class RemoveCategoryHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveCategoryCommand, RemoveCategoryResult>
{
    public async Task<RemoveCategoryResult> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var category=await dbContext.Categories.FindAsync([request.Id], cancellationToken);
        if ((category is null))
            throw new Exception($"Category not found: {request.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        category.Remove(userId);
        await dbContext.SaveChangesAsync();
        return new RemoveCategoryResult(true);
    }
}
