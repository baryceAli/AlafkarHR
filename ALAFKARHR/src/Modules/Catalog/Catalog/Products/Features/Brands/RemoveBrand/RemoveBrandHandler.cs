namespace Catalog.Products.Features.Brands.RemoveBrand;

public record RemoveBrandCommand(Guid Id, string DeletedBy) : ICommand<RemoveBrandResult>;
public record RemoveBrandResult(bool IsSuccess);
public class DeleteBrandCommandValidator : AbstractValidator<RemoveBrandCommand>
{
    public DeleteBrandCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required");
        RuleFor(x => x.DeletedBy).NotEmpty().WithMessage("DeletedBy is required");
    }
}
public class RemoveBrandHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<RemoveBrandCommand, RemoveBrandResult>
{
    public async Task<RemoveBrandResult> Handle(RemoveBrandCommand request, CancellationToken cancellationToken)
    {
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var brand = await dbContext.Brands.FindAsync([request.Id], cancellationToken);
        if(brand is null)
        {
            throw new Exception("Brand not found");
        }

        brand.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new RemoveBrandResult(true);
    }
}
