using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.ProductPackages.UpdateProductPackage;

public record UpdateProductPackageCommand(ProductPackageDto ProductPackage) : ICommand<UpdateProductPackageResult>;
public record UpdateProductPackageResult(bool IsSuccess);

public class UpdateProductPackageCommandValidator : AbstractValidator<UpdateProductPackageCommand>
{
    public UpdateProductPackageCommandValidator()
    {
        RuleFor(x=> x.ProductPackage.Name).NotEmpty().WithMessage("PackageName is required");
        RuleFor(x=> x.ProductPackage.NameEng).NotEmpty().WithMessage("PackageNameEng is required");
        RuleFor(x => x.ProductPackage.UnitsCount).GreaterThan(0).WithMessage("UnitsCount must be greator than 0");
        //RuleFor(x => x.ProductPackage.PackagePrice).GreaterThan(0).WithMessage("PackagePrice must be greator than 0");
        //RuleFor(x => x.ProductPackage.QuantityPerPackage).GreaterThan(0).WithMessage("UnitRate must be greator than 0");
    }
}
public class UpdateProductPackageHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductPackageCommand, UpdateProductPackageResult>
{
    public async Task<UpdateProductPackageResult> Handle(UpdateProductPackageCommand command, CancellationToken cancellationToken)
    {
        var package=await dbContext.ProductPackages.FindAsync([command.ProductPackage.Id], cancellationToken);
        if (package is null)
            throw new Exception($"ProductPackage not found: {command.ProductPackage.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        package.Update(
            command.ProductPackage.Name, 
            command.ProductPackage.NameEng,
            command.ProductPackage.UnitsCount,
            //command.ProductPackage.PackagePrice, 
            userId);

        await dbContext.SaveChangesAsync();

        return new UpdateProductPackageResult(true);

    }
}
