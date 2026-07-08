namespace Catalog.Products.Models;

public class Product : Aggregate<Guid>
{

    public Guid CategoryId { get; private set; }
    //public Guid UnitId { get; private set; }
    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
    public CatalogProductType ProductType { get; private set; } = CatalogProductType.Goods;
    public bool IsActive { get; private set; } = true;
    public string? SalesDescription { get; private set; }
    public string? PurchaseDescription { get; private set; }
    public decimal CustomerTaxRate { get; private set; }
    public decimal VendorTaxRate { get; private set; }
    public Guid? IncomeAccountId { get; private set; }
    public Guid? ExpenseAccountId { get; private set; }
    public ProductCostingPolicy CostingPolicy { get; private set; } = ProductCostingPolicy.Standard;
    public string? ImageUrl { get; private set; }

    private readonly List<ProductSku> _skus = new();
    public IReadOnlyCollection<ProductSku> Skus => _skus;

    //private readonly List<ProductPackageLink> _packages = new();
    //public IReadOnlyCollection<ProductPackageLink> Packages => _packages;


    private Product() { }
    public static Product Create(Guid id,
        string name,
        string nameEng,
        Guid categoryId,
        CatalogProductType productType,
        Guid companyId,
        string createdBy)
    {
        return Create(
            id,
            name,
            nameEng,
            categoryId,
            productType,
            null,
            null,
            0,
            0,
            null,
            null,
            ProductCostingPolicy.Standard,
            null,
            companyId,
            createdBy);
    }

    public static Product Create(Guid id, 
        string name, 
        string nameEng, 
        Guid categoryId, 
        //Guid unitId,
        CatalogProductType productType,
        string? salesDescription,
        string? purchaseDescription,
        decimal customerTaxRate,
        decimal vendorTaxRate,
        Guid? incomeAccountId,
        Guid? expenseAccountId,
        ProductCostingPolicy costingPolicy,
        string? imageUrl,
        Guid companyId, 
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
            ProductType = NormalizeProductType(productType),
            SalesDescription = salesDescription,
            PurchaseDescription = purchaseDescription,
            CustomerTaxRate = customerTaxRate,
            VendorTaxRate = vendorTaxRate,
            IncomeAccountId = incomeAccountId,
            ExpenseAccountId = expenseAccountId,
            CostingPolicy = NormalizeCostingPolicy(costingPolicy),
            ImageUrl = imageUrl,
            IsActive = true,
            //UnitId = unitId,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        return product;

    }
    public void Update(string name, 
        string nameEng, 
        Guid categoryId,
        //Guid unitId,
        CatalogProductType productType,
        string? salesDescription,
        string? purchaseDescription,
        decimal customerTaxRate,
        decimal vendorTaxRate,
        Guid? incomeAccountId,
        Guid? expenseAccountId,
        ProductCostingPolicy costingPolicy,
        string? imageUrl,
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        Name = name;
        NameEng = nameEng;
        CategoryId = categoryId;
        ProductType = NormalizeProductType(productType);
        SalesDescription = salesDescription;
        PurchaseDescription = purchaseDescription;
        CustomerTaxRate = customerTaxRate;
        VendorTaxRate = vendorTaxRate;
        IncomeAccountId = incomeAccountId;
        ExpenseAccountId = expenseAccountId;
        CostingPolicy = NormalizeCostingPolicy(costingPolicy);
        ImageUrl = imageUrl;
        //UnitId = unitId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Archive(string modifiedBy)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void AddProductVariant(ProductSku sku)
    {
        _skus.Add(sku);
    }

    private static CatalogProductType NormalizeProductType(CatalogProductType productType)
    {
        return productType == default
            ? CatalogProductType.Goods
            : productType;
    }

    private static ProductCostingPolicy NormalizeCostingPolicy(ProductCostingPolicy costingPolicy)
    {
        return costingPolicy == default
            ? ProductCostingPolicy.Standard
            : costingPolicy;
    }
    //public void AddPackage(Guid packageId)
    //{
    //    if (_packages.Any(p => p.PackageId == packageId))
    //        throw new Exception("Package already added");

    //    _packages.Add(ProductPackageLink.Create(Id, packageId));
    //}
    //public void RemoveVariant(ProductVariant variant) { _variants.Remove(variant); }

    

}
