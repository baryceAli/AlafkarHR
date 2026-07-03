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
        RuleFor(x => x.ProductPackage.Weight).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Weight.HasValue).WithMessage("Weight cannot be negative");
        RuleFor(x => x.ProductPackage.Length).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Length.HasValue).WithMessage("Length cannot be negative");
        RuleFor(x => x.ProductPackage.Width).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Width.HasValue).WithMessage("Width cannot be negative");
        RuleFor(x => x.ProductPackage.Height).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Height.HasValue).WithMessage("Height cannot be negative");
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
            command.ProductPackage.Weight,
            command.ProductPackage.Length,
            command.ProductPackage.Width,
            command.ProductPackage.Height,
            command.ProductPackage.Notes,
            //command.ProductPackage.PackagePrice, 
            userId);

        if (command.ProductPackage.IsActive)
            package.Activate(userId);
        else
            package.Archive(userId);

        await dbContext.SaveChangesAsync();

        return new UpdateProductPackageResult(true);

    }
}
