namespace Catalog.Products.Models;

public class ProductSkuPackage : Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public ProductSku ProductSku { get; private set; } = default!;

    public Guid ProductPackageId { get; private set; }
    public ProductPackage ProductPackage { get; private set; } = default!;

    private ProductSkuPackage() { }

    public static ProductSkuPackage Create(Guid productSkuId, Guid productPackageId, string createdBy)
    {
        if (productSkuId == Guid.Empty) throw new ArgumentNullException(nameof(productSkuId));
        if (productPackageId == Guid.Empty) throw new ArgumentNullException(nameof(productPackageId));
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new ProductSkuPackage
        {
            ProductSkuId = productSkuId,
            ProductPackageId = productPackageId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore(string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
