using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Categories.CreateCategory;

public record CreateCategoryCommand(CategoryDto Category) : ICommand<CreateCategoryResult>;
public record CreateCategoryResult(Guid Id);

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x=> x.Category.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=> x.Category.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class CreateCategoryHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCategoryCommand, CreateCategoryResult>
{
    public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var category = Category.Create(Guid.NewGuid(), command.Category.Name,command.Category.NameEng, command.Category.Description, userId);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResult(category.Id);
    }
}
