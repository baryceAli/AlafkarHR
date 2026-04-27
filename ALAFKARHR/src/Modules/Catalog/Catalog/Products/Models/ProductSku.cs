

using Catalog.Products.Helpers;

namespace Catalog.Products.Models;

public class ProductSku : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid BrandId { get; private set; }

    public Guid PackageId { get; private set; } // optional (size: 250ml, 1L)

    public string SkuCode { get; private set; } = default!;
    public string Barcode { get; private set; } = default!;

    public decimal Price { get; private set; }

    public bool ShowOnStore { get; private set; }

    private readonly List<ProductSkuVariant> _variants = new();
    public IReadOnlyCollection<ProductSkuVariant> Variants => _variants;

    //SKU1 Milk    Almarai Full Cream	2
    //SKU2 Milk    Almarai No Cream	3
    //SKU3 Milk    Alsafi Full Cream	1.5

    private ProductSku() { }

    internal ProductSku(Guid id,
        Guid productId,
        Guid packageId,
        decimal price,
        bool showOnStore,
        string createdBy)
    {
        Id = id;
        ProductId = productId;
        PackageId = packageId;
        //_options = options.ToList();
        Price = price;
        ShowOnStore = showOnStore;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }


    public static ProductSku Create(Guid id,
        Guid productId,
        Guid packageId,
        decimal price,
        bool showOnStore,
        string createdBy)
    {

        return new ProductSku()
        {
            Id = id,
            ProductId = productId,
            PackageId = packageId,
            Price = price,
            ShowOnStore = showOnStore,
            CreatedBy = createdBy
        };

    }
    public void Update(decimal price, bool showOnStore, string modifiedBy)
    {
        Price = price;
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


}



