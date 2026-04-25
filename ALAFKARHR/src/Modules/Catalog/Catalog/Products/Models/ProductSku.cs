

using Catalog.Products.Helpers;

namespace Catalog.Products.Models;

public class ProductSKU : Entity<Guid>
{
    public Guid VariantId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid PackageId { get; set; }
    public string Sku { get; private set; } = default!;
    public string SkuEng { get; private set; } = default!;
    public string VariantValue { get; set; } = default!;
    public decimal? Price { get; set; }
    //public string Barcode { get; set; }
    public bool ShowOnStore { get; set; }

    //private readonly List<VariantOption> _options = new();
    //public IReadOnlyCollection<VariantOption> Options => _options;

    //[Brand]-[Product]-[Variant]
    //SHOE-NIKE-RED-42
    //LAP-DELL-I7-16GB

    private ProductSKU() { }

    internal ProductSKU(Guid id, Guid productId,Guid variantId,Guid packageId, string variantValue,decimal?price, string sku,string skuEng,bool showOnStore, string createdBy)
    {
        Id = id;
        ProductId = productId;
        VariantId = variantId;
        PackageId = packageId;
        VariantValue = variantValue;
        //_options = options.ToList();
        Sku = sku;
        SkuEng = skuEng;
        Price = price;
        ShowOnStore = showOnStore;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    //[JsonConstructor]
    //public ProductVariant(Guid variantId, Guid productId, string variantValue, string sku, string skuEng)
    //{
    //    VariantId = variantId;
    //    ProductId = productId;
    //    VariantValue = variantValue;
    //    Sku = sku;
    //    SkuEng = skuEng;
    //}

    public static ProductSKU Create(Guid id, Guid productId, Guid variantId,Guid packageId, string variantValue, decimal? price,string sku, string skuEng,bool showOnStore, string createdBy)
    {

        return new ProductSKU()
        {
            Id = id,
            ProductId = productId,
            VariantId = variantId,
            PackageId = packageId,
            VariantValue = variantValue,
            Price = price,
            Sku = sku,
            SkuEng= skuEng,
            ShowOnStore=showOnStore,
            CreatedBy = createdBy
        };
        //Id = id;
        //ProductId = productId;
        //VariantId = variantId;
        //VariantValue = variantValue;
        ////_options = options.ToList();
        ////Sku = sKU;
        //Price = price;
        //CreatedAt = DateTime.UtcNow;
        //CreatedBy = createdBy;
    }
    public void Update(string variantValue,decimal? price,bool showOnStore, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(variantValue);
        //VariantId = variantId;
        //ProductId = productId;
        VariantValue = variantValue;
        Price= price;
        ShowOnStore = showOnStore;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        DeletedAt=DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    
}

    

