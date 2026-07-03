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
        RuleFor(x => x.ProductSku.Calories).NotNull().GreaterThan(0).WithMessage("Calories must be greater than 0");
        //RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        
    }
}
public class UpdateProductSkuHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductSkuCommand, UpdateProductSkuResult>
{
    public async Task<UpdateProductSkuResult> Handle(UpdateProductSkuCommand command, CancellationToken cancellationToken)
    {
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);
        var productSku = await dbContext.ProductSkus
            .Include(sku=> sku.Variants)
            .Include(sku => sku.Packages)
            .Include(sku => sku.Components)
            .FirstOrDefaultAsync(sku=>sku.Id==command.ProductSku.Id && sku.CompanyId == companyId, cancellationToken);
        if (productSku is null)
            throw new Exception($"ProductSku not found: {command.ProductSku.Id}");

        await dbContext.ProductSkuPackages
            .IgnoreQueryFilters()
            .Where(package => package.ProductSkuId == productSku.Id)
            .LoadAsync(cancellationToken);

        await dbContext.ProductSkuComponents
            .IgnoreQueryFilters()
            .Where(component => component.ParentProductSkuId == productSku.Id)
            .LoadAsync(cancellationToken);

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);



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
            .FirstOrDefaultAsync(x => x.Id == command.ProductSku.ProductId && x.CompanyId == companyId && x.IsActive, cancellationToken);

        if ((prd is null))
            throw new NotFoundException($"Product not found: {command.ProductSku.ProductId}");

        //var unit = await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(u => u.Id == prd.UnitId);

        //if (unit is null)
        //throw new NotFoundException($"Unit not found: {command.ProductSku.UnitId}");

        await CatalogOwnershipGuard.EnsureBrandAsync(dbContext, command.ProductSku.BrandId, companyId, cancellationToken);
        await CatalogOwnershipGuard.EnsureUnitAsync(dbContext, command.ProductSku.UnitId, companyId, cancellationToken);
        await CatalogOwnershipGuard.EnsureVariantValuesAsync(dbContext, command.ProductSku.Variants, companyId, cancellationToken);

        var brand = await dbContext.Brands.AsNoTracking()
            .FirstAsync(x => x.Id == command.ProductSku.BrandId && x.CompanyId == companyId && x.IsActive, cancellationToken);


        var packageIds = command.ProductSku.Packages
            .Select(p => p.Id)
            .Where(id => id != Guid.Empty)
            .ToHashSet();

        if (command.ProductSku.PackageId.HasValue && command.ProductSku.PackageId.Value != Guid.Empty)
            packageIds.Add(command.ProductSku.PackageId.Value);

        if (packageIds.Any())
        {
            await CatalogOwnershipGuard.EnsurePackagesAsync(dbContext, packageIds, companyId, cancellationToken);
        }

        List<(Guid variantId, Guid variantValueId)> variantValueIds = new List<(Guid, Guid)>();
        foreach (var v in command.ProductSku.Variants)
        {
            variantValueIds.Add((v.VariantId, v.VariantValueId));
        }
        Guid? primaryPackageId = packageIds.FirstOrDefault() == Guid.Empty ? null : packageIds.First();
        await CatalogOwnershipGuard.EnsureUniqueActiveSkuCombinationAsync(
            dbContext,
            companyId,
            command.ProductSku.ProductId,
            command.ProductSku.BrandId,
            primaryPackageId,
            command.ProductSku.Variants,
            productSku.Id,
            cancellationToken);

        var SkuBaseCntx = new SkuBuildContext(
            command.ProductSku.ProductId,
            command.ProductSku.BrandId,
            primaryPackageId,
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

        var componentDtos = productionType == SkuProductionType.CompositeBundle
            ? command.ProductSku.Components
            : [];

        var componentSkuIds = componentDtos
            .Select(component => component.ComponentProductSkuId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (productionType == SkuProductionType.CompositeBundle)
        {
            if (!componentSkuIds.Any())
                throw new Exception("Composite bundle must have at least one component SKU.");

            if (componentSkuIds.Contains(productSku.Id))
                throw new Exception("A bundle SKU cannot contain itself.");

            if (componentDtos.Any(component => component.Quantity <= 0))
                throw new Exception("Bundle component quantity must be greater than zero.");

            if (componentSkuIds.Any())
            {
                var existingComponentSkuIds = await dbContext.ProductSkus.AsNoTracking()
                    .Where(sku => sku.CompanyId == companyId && componentSkuIds.Contains(sku.Id))
                    .Select(sku => sku.Id)
                    .ToListAsync(cancellationToken);

                var missingComponentSkuId = componentSkuIds.Except(existingComponentSkuIds).FirstOrDefault();
                if (missingComponentSkuId != Guid.Empty)
                    throw new NotFoundException($"Component SKU not found: {missingComponentSkuId}");
            }
        }
        ValidateProductTypeCapabilities(prd.ProductType, productionType, command.ProductSku.IsInventoryTracked);
        var key = ProductSkuGenerator.BuildSkuKey(SkuBaseCntx);

        productSku.Update(
            command.ProductSku.Price, 
            command.ProductSku.Calories,
            command.ProductSku.ShowOnStore,
            finalImagePath,
            command.ProductSku.Barcode,
            command.ProductSku.Name,
            command.ProductSku.NameEng,
            skuCode,
            skuCodeEng,
            key,
            productionType,
            companyId,
            command.ProductSku.IsSellable,
            command.ProductSku.IsPurchasable,
            command.ProductSku.IsInventoryTracked,
            command.ProductSku.IsAssetTrackable,
            command.ProductSku.Variants,
            packageIds.ToList(),
            componentDtos,
            userId);
        if (command.ProductSku.IsActive)
            productSku.Activate(userId);
        else
            productSku.Archive(userId);
        await dbContext.SaveChangesAsync();

        return new UpdateProductSkuResult(true);


    }

    private static void ValidateProductTypeCapabilities(CatalogProductType productType, SkuProductionType productionType, bool isInventoryTracked)
    {
        if (productType == CatalogProductType.Service && isInventoryTracked)
            throw new Exception("Service products cannot be inventory tracked.");

        if (productType == CatalogProductType.Combo && productionType != SkuProductionType.CompositeBundle)
            throw new Exception("Combo products must use Composite Bundle production type.");
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
