namespace Catalog.Products.Models;

public class ProductPackageLink:Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid PackageId { get; private set; }

    private ProductPackageLink() { }

    public static ProductPackageLink Create(Guid productId, Guid packageId)
    {
        return new ProductPackageLink
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            PackageId = packageId
        };
    }

}
