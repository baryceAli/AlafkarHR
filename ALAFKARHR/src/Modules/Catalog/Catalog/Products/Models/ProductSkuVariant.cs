namespace Catalog.Products.Models;

public class ProductSkuVariant : Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public Guid VariantId { get; private set; }
    public Guid VariantValueId { get; private set; }


    private ProductSkuVariant() { }

    public static ProductSkuVariant Create(Guid productSkuId, Guid variantId, Guid variantValueId, string createdBy)
    {
        return new ProductSkuVariant
        {
            //Id = id,
            ProductSkuId = productSkuId,
            VariantId = variantId,
            VariantValueId = variantValueId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    internal void Update(Guid variantId, Guid variantvalueId, string modifiedBy)
    {
        VariantId = variantId;
        VariantValueId = variantvalueId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void UndoRemove()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
