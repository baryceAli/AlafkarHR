namespace Catalog.Products.Models;

public class Product : Aggregate<Guid>
{

    public Guid CategoryId { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid UnitId { get; private set; }
    public string Name { get; private set; } = default!;
    public string NameEng { get; set; } = default!;
    public decimal Price { get; private set; }
    public string ImageUrl { get; set; } = default!;


    private readonly List<ProductSKU> _productSkus = new();
    public IReadOnlyCollection<ProductSKU> ProductSkus => _productSkus;

    //private readonly List<ProductPackage> _packages = new();
    //public IReadOnlyCollection<ProductPackage> Packages => _packages;


    private Product() { }
    public static Product Create(Guid id, string name, string nameEng, decimal price, Guid brandId, Guid categoryId, Guid unitId, string imagePath, string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentException.ThrowIfNullOrEmpty(imagePath);
        //ArgumentNullException.ThrowIfNull(categoryId);
        //ArgumentNullException.ThrowIfNull(brandId);
        //ArgumentNullException.ThrowIfNull(unitId);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        var product = new Product
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Price = price,
            BrandId = brandId,
            CategoryId = categoryId,
            UnitId = unitId,
            ImageUrl = imagePath,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        return product;

    }
    public void Update(string name, string nameEng, decimal price, string imagePath, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        ArgumentException.ThrowIfNullOrEmpty(imagePath);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        Name = name;
        NameEng = nameEng;
        Price = price;
        ImageUrl = imagePath;
        //UnitId = unitId;
        //Description = description;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void AddProductVariant(ProductSKU variant)
    {
        _productSkus.Add(variant);
    }
    //public void RemoveVariant(ProductVariant variant) { _variants.Remove(variant); }

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
    //    var pkg = new ProductPackage(id, productId, packageName,packageNameEng, quantityPerPackage, packagePrice, createdBy);
    //    _packages.Add(pkg);
    //}
    //public void RemoveProductPackage(ProductPackage package){_packages.Remove(package);}


}
