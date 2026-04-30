namespace Catalog.Products.Models;

public class ProductSkuVariant:Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public Guid VariantId { get; private set; }
    public Guid VariantValueId { get; private set; }


    private ProductSkuVariant() { }

    public static ProductSkuVariant Create( Guid productSkuId, Guid variantId, Guid variantValueId, string createdBy)
    {
        return new ProductSkuVariant
        {
            //Id = id,
            ProductSkuId = productSkuId,
            VariantId = variantId,
            VariantValueId = variantValueId,
            CreatedAt=DateTime.UtcNow,
            CreatedBy=createdBy
        };
    }

    public void Remove(string deletedBy)
    {
        IsDeleted=true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

}
