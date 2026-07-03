

using Catalog.Products.Helpers;

namespace Catalog.Products.Models;

public class ProductSku : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid BrandId { get; private set; }

    public Guid? PackageId { get; private set; } // optional (size: 250ml, 1L)
    public Guid UnitId { get; set; }
    public bool IsPackage => PackageId.HasValue || _packages.Any(p => !p.IsDeleted);

    public string Name { get; set; }
    public string NameEng { get; set; }
    public string SkuCode { get; private set; } = default!;
    public string SkuCodeEng { get; private set; } = default!;
    public string SkuKey { get; private set; } = default!;
    public string? Barcode { get; private set; } = default!;

    public decimal Price { get; private set; }
    public decimal? Calories { get; private set; }
    public SkuProductionType ProductionType { get; private set; } = SkuProductionType.PurchasedRawMaterial;
    public string ImageUrl { get; set; }
    public Guid CompanyId { get; set; }
    public bool ShowOnStore { get; private set; }
    public bool IsSellable { get; private set; } = true;
    public bool IsPurchasable { get; private set; } = true;
    public bool IsInventoryTracked { get; private set; } = true;
    public bool IsAssetTrackable { get; private set; }

    private readonly List<ProductSkuVariant> _variants = new();
    public IReadOnlyCollection<ProductSkuVariant> Variants => _variants;

    private readonly List<ProductSkuPackage> _packages = new();
    public IReadOnlyCollection<ProductSkuPackage> Packages => _packages;

    private readonly List<ProductSkuComponent> _components = new();
    public IReadOnlyCollection<ProductSkuComponent> Components => _components;



    //SKU1 Milk    Almarai      Full Cream	    2
    //SKU2 Milk    Almarai      No Cream	    3
    //SKU3 Milk    Alsafi       Full Cream	    1.5

    private ProductSku() { }

    internal ProductSku(Guid id,
        Guid productId,
        Guid brandId,
        //Guid packageId,
        string skuCode,
   string? barcode,
   string imageUrl,
        decimal price,
        decimal? calories,
        SkuProductionType productionType,
        bool showOnStore,
        Guid companyId
        )
    {
        Id = id;
        ProductId = productId;
        BrandId = brandId;
        //PackageId = packageId;
        SkuCode = skuCode;
        Barcode = barcode;
        ImageUrl = imageUrl;
        //_options = options.ToList();
        Price = price;
        Calories = calories;
        ProductionType = NormalizeProductionType(productionType);
        ShowOnStore = showOnStore;
        CompanyId = companyId;
    }

    public static ProductSku Create(
    Guid id,
    Guid productId,
    Guid brandId,
    Guid unitId,
    Guid? packageId,
    string name,
    string nameEng,
    string skuCode,
    string skuCodeEng,
    string skuKey,
    string? barcode,
    string imageUrl,
        decimal price,
        decimal? calories,
        SkuProductionType productionType,
        bool showOnStore,
        bool isSellable,
        bool isPurchasable,
        bool isInventoryTracked,
        bool isAssetTrackable,
        Guid companyId,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skuCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(skuCodeEng);
        ValidateCapabilityFlags(productionType, isInventoryTracked);

        return new ProductSku
        {
            Id = id,
            ProductId = productId,
            BrandId = brandId,
            UnitId = unitId,
            PackageId = packageId,
            Name = name,
            NameEng = nameEng,
            SkuCode = skuCode,
            SkuCodeEng = skuCodeEng,
            SkuKey = skuKey,
            ImageUrl = imageUrl,
            Barcode = barcode,
            Price = price,
            Calories = calories,
            ProductionType = NormalizeProductionType(productionType),
            IsSellable = showOnStore || isSellable,
            IsPurchasable = isPurchasable,
            IsInventoryTracked = isInventoryTracked,
            IsAssetTrackable = isAssetTrackable,
            ShowOnStore = showOnStore,
            CompanyId = companyId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void Update(
        decimal price,
        decimal? calories,
        bool showOnStore,
        string imageUrl,
        string? barcode,
        string name,
        string nameEng,
        string skuCode, 
        string skuCodeEng,
        string skuKey,
        SkuProductionType productionType,
        Guid companyId,
        bool isSellable,
        bool isPurchasable,
        bool isInventoryTracked,
        bool isAssetTrackable,
        List<ProductSkuVariantDto> variantDtos,
        List<Guid> packageIds,
        List<ProductSkuComponentDto> componentDtos,
        string modifiedBy)
    {
        Name = name;
        NameEng = nameEng;
        SkuCode=skuCode;
        SkuCodeEng=skuCodeEng;
        SkuKey = skuKey;
        Price = price;
        Calories = calories;
        ProductionType = NormalizeProductionType(productionType);
        ValidateCapabilityFlags(ProductionType, isInventoryTracked);
        ImageUrl = imageUrl;
        IsSellable = showOnStore || isSellable;
        IsPurchasable = isPurchasable;
        IsInventoryTracked = isInventoryTracked;
        IsAssetTrackable = isAssetTrackable;
        ShowOnStore = showOnStore;
        CompanyId = companyId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        SetPackages(packageIds, modifiedBy);
        SetComponents(
            ProductionType == SkuProductionType.CompositeBundle
                ? componentDtos
                : [],
            modifiedBy);
        //176AF7D6-4C28-40AD-BBE4-314DEB3E9755
        var activeValues = _variants.Where(v => v.ProductSkuId == Id && !v.IsDeleted).ToList();
        var activeIds = activeValues.Select(v => v.Id).ToHashSet();

        // Add + Update
        foreach (var v in variantDtos)
        {
            if (v.Id == Guid.Empty)
            {

                AddVariants(v.VariantId, v.VariantValueId, modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!activeIds.Contains(v.Id))
                throw new Exception($"Invalid or deleted Variant Id: {v.Id}");


            var existingValue = activeValues.First(ev => ev.Id == v.Id);
            existingValue.Update(v.VariantId, v.VariantValueId, modifiedBy);
        }

        // Remove
        var dtoIds = variantDtos
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var valuesToRemove = dtoIds.Any() ? activeValues
            .Where(ev => !dtoIds.Contains(ev.Id))
            .ToList() : [];

        foreach (var value in valuesToRemove)
        {
            RemoveVariant(value.VariantId,value.VariantValueId);
            //value.Remove(modifiedBy);
        }
    }
    public void AddVariants(Guid variantId, Guid variantValueId, string createdBy)
    {
        var exists = _variants.FirstOrDefault(v => v.VariantId == variantId && v.VariantValueId == variantValueId);
        if (exists == null)
        {
            var newVariant = ProductSkuVariant.Create(Id, variantId, variantValueId, createdBy);//(Guid.NewGuid(), Id, value, valueEng, createdBy);
                                                                                                //newVariantValue  =VariantValue.Create(Guid.NewGuid(), Id, value, valueEng, createdBy);
            _variants.Add(newVariant);
        }

    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void AddVariant(Guid variantId, Guid variantValueId, string addedBy)
    {
        if (!_variants.Any(v => v.VariantId == variantId && v.VariantValueId == variantValueId))
            _variants.Add(ProductSkuVariant.Create(Id, variantId, variantValueId, addedBy));
        //throw new Exception("Variant and Value are already exists for this SKU");

    }

    public void SetPackages(IEnumerable<Guid> packageIds, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        var requestedIds = packageIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToHashSet();

        foreach (var packageId in requestedIds)
        {
            var existingPackage = _packages.FirstOrDefault(p => p.ProductPackageId == packageId);
            if (existingPackage is null)
            {
                _packages.Add(ProductSkuPackage.Create(Id, packageId, modifiedBy));
                continue;
            }

            if (existingPackage.IsDeleted)
            {
                existingPackage.Restore(modifiedBy);
            }
        }

        foreach (var existingPackage in _packages.Where(p => !p.IsDeleted && !requestedIds.Contains(p.ProductPackageId)).ToList())
        {
            existingPackage.Remove(modifiedBy);
        }

        PackageId = requestedIds.FirstOrDefault() == Guid.Empty
            ? null
            : requestedIds.First();
    }

    public void SetComponents(IEnumerable<ProductSkuComponentDto> componentDtos, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        var requestedComponents = componentDtos
            .Where(component => component.ComponentProductSkuId != Guid.Empty)
            .GroupBy(component => component.ComponentProductSkuId)
            .Select(group => new
            {
                ComponentProductSkuId = group.Key,
                Quantity = group.Sum(component => component.Quantity)
            })
            .ToList();

        foreach (var component in requestedComponents)
        {
            if (component.ComponentProductSkuId == Id)
                throw new Exception("A bundle SKU cannot contain itself.");

            if (component.Quantity <= 0)
                throw new Exception("Bundle component quantity must be greater than zero.");

            var existingComponent = _components.FirstOrDefault(x => x.ComponentProductSkuId == component.ComponentProductSkuId);
            if (existingComponent is null)
            {
                _components.Add(ProductSkuComponent.Create(Id, component.ComponentProductSkuId, component.Quantity, modifiedBy));
                continue;
            }

            if (existingComponent.IsDeleted)
            {
                existingComponent.Restore(component.Quantity, modifiedBy);
                continue;
            }

            existingComponent.Update(component.Quantity, modifiedBy);
        }

        var requestedIds = requestedComponents
            .Select(component => component.ComponentProductSkuId)
            .ToHashSet();

        foreach (var existingComponent in _components.Where(x => !x.IsDeleted && !requestedIds.Contains(x.ComponentProductSkuId)).ToList())
        {
            existingComponent.Remove(modifiedBy);
        }
    }

    public void RemoveVariant(Guid variantId, Guid variantValueId)
    {
        var existing = _variants.FirstOrDefault(v => v.VariantId == variantId && v.VariantValueId == variantValueId);
        if (existing is null)
            throw new Exception("Variant and Value not found for this SKU");
        
        _variants.Remove(existing);
        
    }

    private static SkuProductionType NormalizeProductionType(SkuProductionType productionType)
    {
        return productionType == default
            ? SkuProductionType.PurchasedRawMaterial
            : productionType;
    }

    public static void ValidateCapabilityFlags(SkuProductionType productionType, bool isInventoryTracked)
    {
        if (NormalizeProductionType(productionType) == SkuProductionType.CompositeBundle && !isInventoryTracked)
            throw new Exception("Composite bundle SKUs must be inventory tracked.");
    }
    //public void AddProductPackage(Guid id, Guid productId, string packageName, string packageNameEng, double quantityPerPackage, decimal packagePrice, bool showOnStore, string createdBy)
    //{
    //    ArgumentNullException.ThrowIfNullOrEmpty(packageName);
    //    ArgumentNullException.ThrowIfNullOrEmpty(packageNameEng);
    //    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityPerPackage);
    //    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(packagePrice);

    //    //ArgumentNullException.ThrowIfNullOrEmpty(package.ProductId);
    //    var existingPackage = _packages.FirstOrDefault(p => p.Id == id);
    //    if (existingPackage is not null)
    //    {
    //        throw new Exception($"Package exists: {id}");
    //    }
    //    var pkg = new ProductPackage(packageName, packageNameEng, quantityPerPackage, packagePrice, createdBy);
    //    _packages.Add(pkg);
    //}
    //public void RemoveProductPackage(ProductPackage package){_packages.Remove(package);}

}



