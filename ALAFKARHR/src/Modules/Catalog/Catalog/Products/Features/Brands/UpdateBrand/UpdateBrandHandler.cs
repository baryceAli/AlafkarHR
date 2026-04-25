using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.Brands.UpdateBrand;

public record UpdateBrandCommand(BrandDto Brand) : ICommand<UpdateBrandResult>;
public record UpdateBrandResult(bool IsSuccess);

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Brand.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Brand.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Brand.NameEng).NotEmpty().WithMessage("NameEng is required");
    }
}
public class UpdateBrandHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateBrandCommand, UpdateBrandResult>
{
    public async Task<UpdateBrandResult> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
    {

        var brand = await dbContext.Brands
                        .FindAsync([command.Brand.Id], cancellationToken);
        
        if (brand == null)
            throw new Exception($"Brand Not found: {command.Brand.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        brand.Update(command.Brand.Name,command.Brand.NameEng, userId, command.Brand.Description);

        dbContext.Brands.Update(brand);
        await dbContext.SaveChangesAsync();

        return new UpdateBrandResult(true);
    }
}
