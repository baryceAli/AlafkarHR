namespace Catalog.Products.Features.Variants.UpdateVariant;


public record UpdateVariantCommand(VariantDto Variant):ICommand<UpdateVariantResult>;
public record UpdateVariantResult(bool IsSuccess);
public class UpdateVariantCommandValidator : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantCommandValidator()
    {
        RuleFor(x => x.Variant.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Variant.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Variant)
            .Must(x => x.DisplayType != VariantDisplayType.MultiCheckbox || x.CreationMode == VariantCreationMode.Never)
            .WithMessage("Multi-checkbox variants must use Never creation mode");
    }
}
public class UpdateVariantHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateVariantCommand, UpdateVariantResult>
{
    public async Task<UpdateVariantResult> Handle(UpdateVariantCommand command, CancellationToken cancellationToken)
    {
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        var variant = await dbContext.Variants
                        .Include(x=>x.Values)
                        .FirstOrDefaultAsync(x => x.Id == command.Variant.Id && x.CompanyId == companyId && !x.IsDeleted, cancellationToken);

        if ((variant is null))
            throw new Exception($"Variant not found: {command.Variant.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);

        variant.Update(command.Variant, userId );

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
        }
        
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Exception("This record was modified by another user. Please reload and try again.", ex);
        }
        return new UpdateVariantResult(true);
    }
}
