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
            companyId,
            userId);

        dbContext.ProductPackages.Add(prdpkg);


        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddProductPackageResult(prdpkg.Id);


    }
}
