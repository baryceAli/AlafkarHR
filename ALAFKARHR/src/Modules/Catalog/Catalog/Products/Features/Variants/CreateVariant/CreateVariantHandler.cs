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
        var userId = httpContextAccessor.HttpContext?
                        .User?.FindFirst(ClaimTypes.NameIdentifier)?
                        .Value??
                        throw new UnauthorizedAccessException("User is not authenticated");
        
        var newVariant= CreateNewVariant(command.Variant, userId);
        
        dbContext.Variants.Add(newVariant);

        await dbContext.SaveChangesAsync();
        
        
        return new CreateVariantResult(newVariant.Id);
    }

    private Variant CreateNewVariant(VariantDto variantDto, string userId)
    {
        var newVariant = Variant.Create(
            Guid.NewGuid(),
            variantDto.Name,
            variantDto.NameEng,
            variantDto.CompanyId.Value,
            userId);
        variantDto.Values.ForEach(value =>
        {
            newVariant.AddVariantValue(
                
                value.Value,
                value.ValueEng,userId);
        });
        

        return newVariant;
    }
}
