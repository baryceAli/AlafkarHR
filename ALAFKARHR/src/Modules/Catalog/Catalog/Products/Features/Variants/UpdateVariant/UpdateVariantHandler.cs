namespace Catalog.Products.Features.Variants.UpdateVariant;


public record UpdateVariantCommand(VariantDto Variant):ICommand<UpdateVariantResult>;
public record UpdateVariantResult(bool IsSuccess);
public class UpdateVariantCommandValidator : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantCommandValidator()
    {
        RuleFor(x => x.Variant.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Variant.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class UpdateVariantHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateVariantCommand, UpdateVariantResult>
{
    public async Task<UpdateVariantResult> Handle(UpdateVariantCommand command, CancellationToken cancellationToken)
    {
        var variant = await dbContext.Variants.FindAsync([command.Variant.Id],cancellationToken);

        if ((variant is null))
            throw new Exception($"Variant not found: {command.Variant.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        variant.Update(
            command.Variant.Name, 
            command.Variant.NameEng, 
            userId 
            );

        await dbContext.SaveChangesAsync();

        return new UpdateVariantResult(true);
    }
}
