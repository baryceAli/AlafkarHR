namespace Catalog.Products.Models;

public class Product : Aggregate<Guid>
{

    public Guid CategoryId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;

    private readonly List<ProductSku> _skus = new();
    public IReadOnlyCollection<ProductSku> Skus => _skus;

    //private readonly List<ProductPackage> _packages = new();
    //public IReadOnlyCollection<ProductPackage> Packages => _packages;


    private Product() { }
    public static Product Create(Guid id, 
        string name, 
        string nameEng, 
        Guid categoryId, 
        Guid unitId,Guid 
        companyId, 
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        var product = new Product
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CategoryId = categoryId,
            UnitId = unitId,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        return product;

    }
    public void Update(string name, 
        string nameEng, 
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        Name = name;
        NameEng = nameEng;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void AddProductVariant(ProductSku variant)
    {
        _skus.Add(variant);
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
