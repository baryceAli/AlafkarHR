using Catalog.Products.Helpers;
using Shared.Exceptions;

namespace Catalog.Products.Features.Products.AddProductSku;

public record AddProductSkuCommand(ProductSkuDto ProductSku) : ICommand<AddProductSkuResult>;
public record AddProductSkuResult(Guid Id);

public class AddProductSkuCommandValidator : AbstractValidator<AddProductSkuCommand>
{
    public AddProductSkuCommandValidator()
    {
        //RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        RuleFor(x => x.ProductSku.Price).GreaterThan(0).WithMessage("Price is required");
        RuleFor(x => x.ProductSku.Variants.Count).GreaterThan(0).WithMessage("ProductSKU must have variants");
        RuleFor(x => x.ProductSku.Barcode).NotEmpty().WithMessage("Bar code is required");
        RuleFor(x => x.ProductSku.SkuCode).NotEmpty().WithMessage("SkuCode is required");
        RuleFor(x => x.ProductSku.SkuCode).NotEmpty().WithMessage("SkuCode is required");
        RuleFor(x => x.ProductSku.SkuCodeEng).NotEmpty().WithMessage("SkuCodeEng is required");
    }
}
public class AddProductSkuHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AddProductSkuCommand, AddProductSkuResult>
{
    public async Task<AddProductSkuResult> Handle(AddProductSkuCommand command, CancellationToken cancellationToken)
    {
        var prd = await dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == command.ProductSku.ProductId, cancellationToken);

        if ((prd is null))
            throw new NotFoundException($"Product not found: {command.ProductSku.ProductId}");

        //var unit = await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(u => u.Id == prd.UnitId);

        //if (unit is null)
            //throw new NotFoundException($"Unit not found: {command.ProductSku.UnitId}");

        var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ProductSku.BrandId);
        if ((brand is null))
            throw new NotFoundException($"brand not found: {command.ProductSku.BrandId}");
        

        var package =command.ProductSku.PackageId.HasValue? 
                        await dbContext.ProductPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ProductSku.PackageId)
                        :null;
        if(command.ProductSku.PackageId.HasValue && package == null) 
            throw new NotFoundException($"Package not found: {command.ProductSku.PackageId}");
        
        
        var userId = httpContextAccessor.HttpContext?.User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var baseSku = GenerateSKU.Generate(
            prd.Name,
            brand.Name,
            "variant.Name",
            "variantValue",
            "unit.UnitName",
            package.Name);

        var baseSkuEng = GenerateSKU.Generate(
            prd.NameEng, "brndName", "variant.NameEng",
            "command.ProductSku.VariantValue",
            "unit.UnitNameEng", package.NameEng);

        // Fetch all similar SKUs in ONE query
        

        // Helper to extract max suffix
        int GetNextSuffix(IEnumerable<string> skus, string baseValue)
        {
            return skus
                .Where(s => s.StartsWith(baseValue))
                .Select(s =>
                {
                    var parts = s.Split('-');
                    return parts.Length > 1 && int.TryParse(parts[^1], out var num) ? num : 0;
                })
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        

        
        var productSku = ProductSku.Create(
            Guid.NewGuid(),
            command.ProductSku.ProductId,
            command.ProductSku.BrandId,
            command.ProductSku.UnitId.Value,
            command.ProductSku.PackageId.Value,
            //baseSku,
            //baseSkuEng,
            command.ProductSku.SkuCode,
            command.ProductSku.SkuCodeEng,
            Guid.NewGuid().ToString(),
            command.ProductSku.Barcode,
            command.ProductSku.ImageUrl,
            command.ProductSku.Price,
            command.ProductSku.ShowOnStore,
            command.ProductSku.CompanyId,
            userId);
        
        foreach(var variant in command.ProductSku.Variants)
        {

            productSku.AddVariant(variant.VariantId, variant.VariantValueId, userId);
        }

        dbContext.ProductSkus.Add(productSku);

        await dbContext.SaveChangesAsync();

        return new AddProductSkuResult(productSku.Id);

    }
}
