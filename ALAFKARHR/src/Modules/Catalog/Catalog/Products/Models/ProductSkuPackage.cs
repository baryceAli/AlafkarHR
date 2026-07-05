namespace Catalog.Products.Models;

public class ProductSkuPackage : Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public ProductSku ProductSku { get; private set; } = default!;

    public Guid ProductPackageId { get; private set; }
    public ProductPackage ProductPackage { get; private set; } = default!;
    public decimal Quantity { get; private set; } = 1;
    public Guid? UnitId { get; private set; }
    public string? Barcode { get; private set; }
    public bool SalesEnabled { get; private set; } = true;
    public bool PurchaseEnabled { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    private ProductSkuPackage() { }

    public static ProductSkuPackage Create(
        Guid productSkuId,
        Guid productPackageId,
        decimal quantity,
        Guid? unitId,
        string? barcode,
        bool salesEnabled,
        bool purchaseEnabled,
        bool isActive,
        string createdBy)
    {
        if (productSkuId == Guid.Empty) throw new ArgumentNullException(nameof(productSkuId));
        if (productPackageId == Guid.Empty) throw new ArgumentNullException(nameof(productPackageId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new ProductSkuPackage
        {
            Id = Guid.NewGuid(),
            ProductSkuId = productSkuId,
            ProductPackageId = productPackageId,
            Quantity = quantity,
            UnitId = unitId,
            Barcode = NormalizeBarcode(barcode),
            SalesEnabled = salesEnabled,
            PurchaseEnabled = purchaseEnabled,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public static ProductSkuPackage Create(Guid productSkuId, Guid productPackageId, string createdBy)
        => Create(productSkuId, productPackageId, 1m, null, null, true, true, true, createdBy);

    public void Update(
        decimal quantity,
        Guid? unitId,
        string? barcode,
        bool salesEnabled,
        bool purchaseEnabled,
        bool isActive,
        string modifiedBy)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        Quantity = quantity;
        UnitId = unitId;
        Barcode = NormalizeBarcode(barcode);
        SalesEnabled = salesEnabled;
        PurchaseEnabled = purchaseEnabled;
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore(string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private static string? NormalizeBarcode(string? barcode)
        => string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
}
