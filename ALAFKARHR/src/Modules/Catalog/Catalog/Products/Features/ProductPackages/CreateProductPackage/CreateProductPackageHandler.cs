namespace Catalog.Products.Features.ProductPackages.CreateProductPackage;

public record AddProductPackageCommand(ProductPackageDto ProductPackage) : ICommand<AddProductPackageResult>;
public record AddProductPackageResult(Guid Id);

public class AddProductPackageCommandValidator : AbstractValidator<AddProductPackageCommand>
{
    public AddProductPackageCommandValidator()
    {
        RuleFor(x => x.ProductPackage.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.ProductPackage.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.ProductPackage.Quantity).GreaterThan(0).WithMessage("UnitRate is required");
        RuleFor(x => x.ProductPackage.Weight).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Weight.HasValue).WithMessage("Weight cannot be negative");
        RuleFor(x => x.ProductPackage.Length).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Length.HasValue).WithMessage("Length cannot be negative");
        RuleFor(x => x.ProductPackage.Width).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Width.HasValue).WithMessage("Width cannot be negative");
        RuleFor(x => x.ProductPackage.Height).GreaterThanOrEqualTo(0).When(x => x.ProductPackage.Height.HasValue).WithMessage("Height cannot be negative");
    }
}
public class CreateProductPackageHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AddProductPackageCommand, AddProductPackageResult>
{
    public async Task<AddProductPackageResult> Handle(AddProductPackageCommand command, CancellationToken cancellationToken)
    {

        
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        if (command.ProductPackage.UnitId.HasValue && command.ProductPackage.UnitId.Value != Guid.Empty)
            await CatalogOwnershipGuard.EnsureUnitAsync(dbContext, command.ProductPackage.UnitId, companyId, cancellationToken);
        
        var prdpkg = ProductPackage.Create(
            Guid.NewGuid(),
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
            companyId,
            userId);

        dbContext.ProductPackages.Add(prdpkg);


        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddProductPackageResult(prdpkg.Id);


    }
}
