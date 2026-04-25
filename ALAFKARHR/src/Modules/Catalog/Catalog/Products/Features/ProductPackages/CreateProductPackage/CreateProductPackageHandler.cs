using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Products.Features.ProductPackages.CreateProductPackage;

public record AddProductPackageCommand(ProductPackageDto ProductPackage) : ICommand<AddProductPackageResult>;
public record AddProductPackageResult(Guid Id);

public class AddProductPackageCommandValidator : AbstractValidator<AddProductPackageCommand>
{
    public AddProductPackageCommandValidator()
    {
        RuleFor(x => x.ProductPackage.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.ProductPackage.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.ProductPackage.UnitsCount).GreaterThan(0).WithMessage("UnitRate is required");
    }
}
public class CreateProductPackageHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AddProductPackageCommand, AddProductPackageResult>
{
    public async Task<AddProductPackageResult> Handle(AddProductPackageCommand command, CancellationToken cancellationToken)
    {

        
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var prdpkg = ProductPackage.Create(
            Guid.NewGuid(),
            command.ProductPackage.Name,
            command.ProductPackage.NameEng,
            command.ProductPackage.UnitsCount,
            userId);

        dbContext.ProductPackages.Add(prdpkg);


        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddProductPackageResult(prdpkg.Id);


    }
}
