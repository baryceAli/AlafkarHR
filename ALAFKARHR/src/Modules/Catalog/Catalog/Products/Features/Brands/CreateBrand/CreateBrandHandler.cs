
namespace Catalog.Products.Features.Brands.CreateBrand;

public record CreateBrandCommand(BrandDto Brand) : ICommand<CreateBrandResult>;
public record CreateBrandResult(Guid Id);
public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Brand.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Brand.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class CreateBrandHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateBrandCommand, CreateBrandResult>
{
    public async Task<CreateBrandResult> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        //var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;

        var brand = Brand.Create(Guid.NewGuid(), command.Brand.Name,command.Brand.NameEng, companyId, userId, command.Brand.Description);
        await dbContext.AddAsync(brand);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateBrandResult(brand.Id);
    }
}
