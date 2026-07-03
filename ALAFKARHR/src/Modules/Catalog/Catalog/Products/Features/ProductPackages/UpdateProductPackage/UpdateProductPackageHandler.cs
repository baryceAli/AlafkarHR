namespace Catalog.Products.Features.ProductPackages.UpdateProductPackage;

public record UpdateProductPackageCommand(ProductPackageDto ProductPackage) : ICommand<UpdateProductPackageResult>;
public record UpdateProductPackageResult(bool IsSuccess);

public class UpdateProductPackageCommandValidator : AbstractValidator<UpdateProductPackageCommand>
{
    public UpdateProductPackageCommandValidator()
    {
        RuleFor(x=> x.ProductPackage.Name).NotEmpty().WithMessage("PackageName is required");
        RuleFor(x=> x.ProductPackage.NameEng).NotEmpty().WithMessage("PackageNameEng is required");
        RuleFor(x => x.ProductPackage.Quantity).GreaterThan(0).WithMessage("UnitsCount must be greator than 0");
        //RuleFor(x => x.ProductPackage.PackagePrice).GreaterThan(0).WithMessage("PackagePrice must be greator than 0");
        //RuleFor(x => x.ProductPackage.QuantityPerPackage).GreaterThan(0).WithMessage("UnitRate must be greator than 0");
    }
}
public class UpdateProductPackageHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductPackageCommand, UpdateProductPackageResult>
{
    public async Task<UpdateProductPackageResult> Handle(UpdateProductPackageCommand command, CancellationToken cancellationToken)
    {
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        var package=await dbContext.ProductPackages.FirstOrDefaultAsync(x => x.Id == command.ProductPackage.Id && x.CompanyId == companyId, cancellationToken);
        if (package is null)
            throw new Exception($"ProductPackage not found: {command.ProductPackage.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        if (command.ProductPackage.UnitId.HasValue && command.ProductPackage.UnitId.Value != Guid.Empty)
            await CatalogOwnershipGuard.EnsureUnitAsync(dbContext, command.ProductPackage.UnitId, companyId, cancellationToken);

        package.Update(
            command.ProductPackage.Name, 
            command.ProductPackage.NameEng,
            command.ProductPackage.Quantity,
            command.ProductPackage.UnitId,
            command.ProductPackage.Barcode,
            //command.ProductPackage.PackagePrice, 
            userId);

        await dbContext.SaveChangesAsync();

        return new UpdateProductPackageResult(true);

    }
}
