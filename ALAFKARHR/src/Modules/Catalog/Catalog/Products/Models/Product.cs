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

    //private readonly List<ProductPackageLink> _packages = new();
    //public IReadOnlyCollection<ProductPackageLink> Packages => _packages;


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
        Guid categoryId,
        Guid unitId,
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        Name = name;
        NameEng = nameEng;
        CategoryId = categoryId;
        UnitId = unitId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void AddProductVariant(ProductSku sku)
    {
        _skus.Add(sku);
    }
    //public void AddPackage(Guid packageId)
    //{
    //    if (_packages.Any(p => p.PackageId == packageId))
    //        throw new Exception("Package already added");

    //    _packages.Add(ProductPackageLink.Create(Id, packageId));
    //}
    //public void RemoveVariant(ProductVariant variant) { _variants.Remove(variant); }

    

}
