namespace Catalog.Products.Features.Variants.CreateVariant;

public record CreateVariantCommand(VariantDto Variant) : ICommand<CreateVariantResult>;
public record CreateVariantResult(Guid Id);

public class CreateVariantCommandValidator : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantCommandValidator()
    {
        RuleFor(x=> x.Variant.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=> x.Variant.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Variant)
            .Must(x => x.DisplayType != VariantDisplayType.MultiCheckbox || x.CreationMode == VariantCreationMode.Never)
            .WithMessage("Multi-checkbox variants must use Never creation mode");
    }
}
public class CreateVariantHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateVariantCommand, CreateVariantResult>
{
    public async Task<CreateVariantResult> Handle(CreateVariantCommand command, CancellationToken cancellationToken)
    {
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        
        var newVariant= CreateNewVariant(command.Variant, companyId, userId);
        
        dbContext.Variants.Add(newVariant);

        await dbContext.SaveChangesAsync();
        
        
        return new CreateVariantResult(newVariant.Id);
    }

    private Variant CreateNewVariant(VariantDto variantDto, Guid companyId, string userId)
    {
        var newVariant = Variant.Create(
            Guid.NewGuid(),
            variantDto.Name,
            variantDto.NameEng,
            variantDto.DisplayType,
            variantDto.CreationMode,
            companyId,
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
