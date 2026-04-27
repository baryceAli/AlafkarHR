namespace Catalog.Products.Models;

public class ProductSkuVariant:Entity<Guid>
{
    public Guid ProductSkuId { get; private set; }
    public Guid VariantId { get; private set; }
    public Guid VariantValueId { get; private set; }
}
