using Catalog.Products.Helpers;
using Shared.Exceptions;
using Shared.SaveImages;
using SharedWithUI.Catalog.SKUGenerator;

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
        RuleFor(x => x.ProductSku.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.ProductSku.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.ProductSku.Calories).NotNull().GreaterThan(0).WithMessage("Calories must be greater than 0");
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

            if (componentDtos.Any(component => component.Quantity <= 0))
                throw new Exception("Bundle component quantity must be greater than zero.");

            if (componentSkuIds.Any())
            {
                var existingComponentSkuIds = await dbContext.ProductSkus.AsNoTracking()
                    .Where(sku => sku.CompanyId == command.ProductSku.CompanyId && componentSkuIds.Contains(sku.Id))
                    .Select(sku => sku.Id)
                    .ToListAsync(cancellationToken);

                var missingComponentSkuId = componentSkuIds.Except(existingComponentSkuIds).FirstOrDefault();
                if (missingComponentSkuId != Guid.Empty)
                    throw new NotFoundException($"Component SKU not found: {missingComponentSkuId}");
            }
        }
        
        
        var userId = httpContextAccessor.HttpContext?.User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value??
                        throw new UnauthorizedAccessException("User is not authenticated");
       
        List<(Guid variantId,Guid variantValueId)> variantValueIds=new List<(Guid,Guid)>();
        foreach(var v in command.ProductSku.Variants)
        {
            variantValueIds.Add((v.VariantId,v.VariantValueId));
        }
        var SkuBaseCntx = new SkuBuildContext(
            command.ProductSku.ProductId, 
            command.ProductSku.BrandId, 
            packageIds.FirstOrDefault() == Guid.Empty ? null : packageIds.First(), 
            variantValueIds);

        //var key = ProductSkuGenerator.BuildSkuKey(SkuBaseCntx);
        var variants=await dbContext.Variants.AsNoTracking().ToListAsync(cancellationToken);
        var variantValues=await dbContext.VariantValues.AsNoTracking().ToListAsync(cancellationToken);
        Dictionary<Guid, string> variantNames=variants.ToDictionary(x => x.Id, x => x.Name);
        Dictionary<Guid, string> variantNamesEng=variants.ToDictionary(x => x.Id, x => x.NameEng);
        Dictionary<Guid, string> valueNames=variantValues.ToDictionary(x => x.Id, x => x.Value);
        Dictionary<Guid, string> valueNamesEng=variantValues.ToDictionary(x => x.Id, x => x.ValueEng);
    
        var skuCode = ProductSkuGenerator.GenerateSkuCode(SkuBaseCntx,variantNames, valueNames, prd.Name,brand.Name);
        var skuCodeEng = ProductSkuGenerator.GenerateSkuCode(SkuBaseCntx,variantNamesEng, valueNamesEng, prd.NameEng,brand.NameEng);
        

        var skuId = Guid.NewGuid();
        string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
        var img = SaveImages.SaveBase64Image($"{skuId}", PATH_SEGEMNT, command.ProductSku.ImageUrl);

        var productSku = ProductSku.Create(
            skuId,
            command.ProductSku.ProductId,
            command.ProductSku.BrandId,
            command.ProductSku.UnitId.Value,
            packageIds.FirstOrDefault() == Guid.Empty ? null : packageIds.First(),
            command.ProductSku.Name,
            command.ProductSku.NameEng,
            skuCode,
            skuCodeEng,
            Guid.NewGuid().ToString(),//key
            command.ProductSku.Barcode,
            img,
            command.ProductSku.Price,
            command.ProductSku.Calories,
            productionType,
            command.ProductSku.ShowOnStore,
            command.ProductSku.IsSellable,
            command.ProductSku.IsPurchasable,
            command.ProductSku.IsInventoryTracked,
            command.ProductSku.IsAssetTrackable,
            command.ProductSku.CompanyId,
            userId);
        
        foreach(var variant in command.ProductSku.Variants)
        {

            productSku.AddVariant(variant.VariantId, variant.VariantValueId, userId);
        }

        productSku.SetPackages(packageIds, userId);
        productSku.SetComponents(componentDtos, userId);

        dbContext.ProductSkus.Add(productSku);

        await dbContext.SaveChangesAsync();

        return new AddProductSkuResult(productSku.Id);

    }
}
