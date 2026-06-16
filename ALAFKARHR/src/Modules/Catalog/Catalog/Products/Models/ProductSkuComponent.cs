namespace Catalog.Products.Models;

public class ProductSkuComponent : Entity<Guid>
{
    public Guid ParentProductSkuId { get; private set; }
    public ProductSku ParentProductSku { get; private set; } = default!;

    public Guid ComponentProductSkuId { get; private set; }
    public ProductSku ComponentProductSku { get; private set; } = default!;

    public decimal Quantity { get; private set; }

    private ProductSkuComponent() { }

    public static ProductSkuComponent Create(
        Guid parentProductSkuId,
        Guid componentProductSkuId,
        decimal quantity,
        string createdBy)
    {
        if (parentProductSkuId == Guid.Empty) throw new ArgumentNullException(nameof(parentProductSkuId));
        if (componentProductSkuId == Guid.Empty) throw new ArgumentNullException(nameof(componentProductSkuId));
        if (parentProductSkuId == componentProductSkuId) throw new ArgumentException("A bundle SKU cannot contain itself.");
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new ProductSkuComponent
        {
            Id = Guid.NewGuid(),
            ParentProductSkuId = parentProductSkuId,
            ComponentProductSkuId = componentProductSkuId,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(decimal quantity, string modifiedBy)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        Quantity = quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deletedBy);

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore(decimal quantity, string modifiedBy)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        Quantity = quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
