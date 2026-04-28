namespace Catalog.Products.Models;

public class ProductSkuVariant:Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public Guid VariantId { get; private set; }
    public Guid VariantValueId { get; private set; }


    private ProductSkuVariant() { }

    public static ProductSkuVariant Create(Guid id, Guid productSkuId, Guid variantId, Guid variantValueId)
    {
        return new ProductSkuVariant
        {
            Id = id,
            ProductSkuId = productSkuId,
            VariantId = variantId,
            VariantValueId = variantValueId
        };
    }

    

}
