using Catalog.Products.Helpers;

namespace Catalog.Products.Features.Products.AddProductSku;

public record AddProductSkuCommand(ProductSkuDto ProductSku) : ICommand<AddProductSkuResult>;
public record AddProductSkuResult(Guid Id);

public class AddProductSkuCommandValidator : AbstractValidator<AddProductSkuCommand>
{
    public AddProductSkuCommandValidator()
    {
        //RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        RuleFor(x => x.ProductSku.Price).GreaterThan(0).WithMessage("Price is required");
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
            throw new Exception($"Product not found: {command.ProductSku.ProductId}");

        //var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == prd.BrandId);
        var unit = await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(x => x.Id == prd.UnitId);
        //var variant = await dbContext.Variants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ProductSku.VariantId);
        var package = await dbContext.ProductPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ProductSku.PackageId);
        //if ((brand is null))
        //throw new Exception($"brand not found: {command.ProductVariant.ProductId}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var baseSku = GenerateSKU.Generate(
            prd.Name,
            "brndName",
            "variant.Name",
            "variantValue",
            unit.UnitName, 
            package.Name);

        var baseSkuEng = GenerateSKU.Generate(
            prd.NameEng, "brndName", "variant.NameEng",
            "command.ProductSku.VariantValue",
            unit.UnitNameEng, package.NameEng);

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
            command.ProductSku.PackageId,
            baseSku,
            baseSkuEng,
            command.ProductSku.Barcode,
            command.ProductSku.ImageUrl,
            command.ProductSku.Price,
            command.ProductSku.ShowOnStore,
            userId);

        dbContext.ProductSkus.Add(productSku);

        await dbContext.SaveChangesAsync();

        return new AddProductSkuResult(productSku.Id);

    }
}
