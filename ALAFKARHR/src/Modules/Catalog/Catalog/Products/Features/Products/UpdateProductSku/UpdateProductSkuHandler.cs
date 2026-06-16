using Shared.Exceptions;
using Shared.SaveImages;
using SharedWithUI.Catalog.SKUGenerator;

namespace Catalog.Products.Features.Products.UpdateProductSku;

public record UpdateProductSkuCommand(ProductSkuDto ProductSku) : ICommand<UpdateProductSkuResult>;
public record UpdateProductSkuResult(bool IsSuccess);

public class UpdateProductSkuCommandValidator : AbstractValidator<UpdateProductSkuCommand>
{
    public UpdateProductSkuCommandValidator()
    {
        RuleFor(x => x.ProductSku.Price).GreaterThan(0).WithMessage("Price must be greator than 0");
        //RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        
    }
}
public class UpdateProductSkuHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductSkuCommand, UpdateProductSkuResult>
{
    public async Task<UpdateProductSkuResult> Handle(UpdateProductSkuCommand command, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus
            .Include(sku=> sku.Variants)
            .Include(sku => sku.Packages)
            .FirstOrDefaultAsync(sku=>sku.Id==command.ProductSku.Id, cancellationToken);
        if (productSku is null)
            throw new Exception($"ProductSku not found: {command.ProductSku.Id}");

        await dbContext.ProductSkuPackages
            .IgnoreQueryFilters()
            .Where(package => package.ProductSkuId == productSku.Id)
            .LoadAsync(cancellationToken);

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User is not authorized");



        string finalImagePath = productSku.ImageUrl;
        var incomingImage = command.ProductSku.ImageUrl;

        if (!string.IsNullOrWhiteSpace(incomingImage))
        {
            if (IsBase64Image(incomingImage))
            {
                string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
                finalImagePath = SaveImages.SaveBase64Image($"{productSku.Id}", PATH_SEGEMNT, command.ProductSku.ImageUrl);
            }
        }


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


        var packageIds = command.ProductSku.Packages
            .Select(p => p.Id)
            .Where(id => id != Guid.Empty)
            .ToHashSet();

        if (command.ProductSku.PackageId.HasValue && command.ProductSku.PackageId.Value != Guid.Empty)
            packageIds.Add(command.ProductSku.PackageId.Value);

        if (packageIds.Any())
        {
            var existingPackageIds = await dbContext.ProductPackages.AsNoTracking()
                .Where(x => packageIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var missingPackageId = packageIds.Except(existingPackageIds).FirstOrDefault();
            if (missingPackageId != Guid.Empty)
                throw new NotFoundException($"Package not found: {missingPackageId}");
        }

        List<(Guid variantId, Guid variantValueId)> variantValueIds = new List<(Guid, Guid)>();
        foreach (var v in command.ProductSku.Variants)
        {
            variantValueIds.Add((v.VariantId, v.VariantValueId));
        }
        var SkuBaseCntx = new SkuBuildContext(
            command.ProductSku.ProductId,
            command.ProductSku.BrandId,
            packageIds.FirstOrDefault() == Guid.Empty ? null : packageIds.First(),
            variantValueIds);


        //var key = ProductSkuGenerator.BuildSkuKey(SkuBaseCntx);
        var variants = await dbContext.Variants.AsNoTracking().ToListAsync(cancellationToken);
        var variantValues = await dbContext.VariantValues.AsNoTracking().ToListAsync(cancellationToken);
        Dictionary<Guid, string> variantNames = variants.ToDictionary(x => x.Id, x => x.Name);
        Dictionary<Guid, string> variantNamesEng = variants.ToDictionary(x => x.Id, x => x.NameEng);
        Dictionary<Guid, string> valueNames = variantValues.ToDictionary(x => x.Id, x => x.Value);
        Dictionary<Guid, string> valueNamesEng = variantValues.ToDictionary(x => x.Id, x => x.ValueEng);

        var skuCode = ProductSkuGenerator.GenerateSkuCode(SkuBaseCntx, variantNames, valueNames, prd.Name, brand.Name);
        var skuCodeEng = ProductSkuGenerator.GenerateSkuCode(SkuBaseCntx, variantNamesEng, valueNamesEng, prd.NameEng, brand.NameEng);

        var productionType = command.ProductSku.ProductionType == default
            ? SkuProductionType.PurchasedRawMaterial
            : command.ProductSku.ProductionType;

        productSku.Update(
            command.ProductSku.Price, 
            command.ProductSku.ShowOnStore,
            finalImagePath,
            command.ProductSku.Barcode,
            command.ProductSku.Name,
            command.ProductSku.NameEng,
            skuCode,
            skuCodeEng,
            productionType,
            command.ProductSku.CompanyId,
            command.ProductSku.Variants,
            packageIds.ToList(),
            userId);
        await dbContext.SaveChangesAsync();

        return new UpdateProductSkuResult(true);


    }
    private bool IsBase64Image(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return true;

        Span<byte> buffer = new Span<byte>(new byte[input.Length]);

        return Convert.TryFromBase64String(input, buffer, out _);
    }
}
