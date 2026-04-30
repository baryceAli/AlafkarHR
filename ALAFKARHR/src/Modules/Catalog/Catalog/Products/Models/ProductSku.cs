

using Catalog.Products.Helpers;

namespace Catalog.Products.Models;

public class ProductSku : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid BrandId { get; private set; }

    //public Guid PackageId { get; private set; } // optional (size: 250ml, 1L)

    public string SkuCode { get; private set; } = default!;
    public string Barcode { get; private set; } = default!;

    public decimal Price { get; private set; }
    public string ImageUrl { get; set; }

    public bool ShowOnStore { get; private set; }

    private readonly List<ProductSkuVariant> _variants = new();
    public IReadOnlyCollection<ProductSkuVariant> Variants => _variants;

    //SKU1 Milk    Almarai      Full Cream	    2
    //SKU2 Milk    Almarai      No Cream	    3
    //SKU3 Milk    Alsafi       Full Cream	    1.5

    private ProductSku() { }

    internal ProductSku(Guid id,
        Guid productId,
        Guid brandId,
        //Guid packageId,
        string skuCode,
   string barcode,
   string imageUrl,
        decimal price,
        bool showOnStore
        )
    {
        Id = id;
        ProductId = productId;
        BrandId = brandId;
        //PackageId = packageId;
        SkuCode = skuCode;
        Barcode = barcode;
        ImageUrl= imageUrl;
        //_options = options.ToList();
        Price = price;
        ShowOnStore = showOnStore;

    }

    public static ProductSku Create(
    Guid id,
    Guid productId,
    Guid brandId,
    //Guid packageId,
    string skuCode,
    string skuCodeEng,
    string barcode,
    string imageUrl,
    decimal price,
    bool showOnStore,
    string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skuCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(skuCodeEng);

        return new ProductSku
        {
            Id = id,
            ProductId = productId,
            BrandId = brandId,
            //PackageId = packageId,
            SkuCode = skuCode,
            ImageUrl = imageUrl,
            Barcode = barcode,
            Price = price,
            ShowOnStore = showOnStore,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void Update(decimal price, bool showOnStore, string imageUrl, string modifiedBy)
    {
        Price = price;
        ImageUrl= imageUrl;
        ShowOnStore = showOnStore;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void AddVariant(Guid variantId, Guid variantValueId)
    {
        if (_variants.Any(v => v.VariantId == variantId))
            throw new Exception("Variant already exists for this SKU");

        _variants.Add(ProductSkuVariant.Create(Guid.NewGuid(), Id, variantId, variantValueId));
    }

}



