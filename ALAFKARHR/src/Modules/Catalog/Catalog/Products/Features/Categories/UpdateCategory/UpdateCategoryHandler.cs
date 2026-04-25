using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Categories.UpdateCategory;

public record UpdateCategoryCommand(CategoryDto Category) : ICommand<UpdateCategoryResult>;
public record UpdateCategoryResult(bool IsSuccess);

public class UpdateCategoryCommandValidator: AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x=> x.Category.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x=> x.Category.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=> x.Category.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class UpdateCategoryHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([command.Category.Id], cancellationToken);

        if (category is null)
            throw new Exception($"Category not found: {command.Category.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        category.Update(command.Category.Name, command.Category.NameEng, userId, command.Category.Description);

        dbContext.Categories.Update(category);

        await dbContext.SaveChangesAsync();

        return new UpdateCategoryResult(true);
    }
}
