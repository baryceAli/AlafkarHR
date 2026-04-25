using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Variants.CreateVariant;

public record CreateVariantCommand(VariantDto Variant) : ICommand<CreateVariantResult>;
public record CreateVariantResult(Guid Id);

public class CreateVariantCommandValidator : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantCommandValidator()
    {
        RuleFor(x=> x.Variant.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=> x.Variant.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class CreateVariantHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateVariantCommand, CreateVariantResult>
{
    public async Task<CreateVariantResult> Handle(CreateVariantCommand command, CancellationToken cancellationToken)
    {
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var variant = Variant.Create(
            Guid.NewGuid(),
            command.Variant.Name,
            command.Variant.NameEng,
            userId,
            command.Variant.Description
            );

        dbContext.Variants.Add(variant);
        await dbContext.SaveChangesAsync();
        return new CreateVariantResult(variant.Id);
    }
}
