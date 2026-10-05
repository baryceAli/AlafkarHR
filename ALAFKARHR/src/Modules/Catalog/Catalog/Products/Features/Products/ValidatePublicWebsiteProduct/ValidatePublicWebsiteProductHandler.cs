using Catalog.Contracts.Products.Features.ValidatePublicWebsiteProduct;

namespace Catalog.Products.Features.Products.ValidatePublicWebsiteProduct;

public class ValidatePublicWebsiteProductHandler(CatalogDbContext db)
    : IQueryHandler<ValidatePublicWebsiteProductQuery, ValidatePublicWebsiteProductResult>
{
    public async Task<ValidatePublicWebsiteProductResult> Handle(ValidatePublicWebsiteProductQuery request, CancellationToken cancellationToken)
    {
        var visible = await (from sku in db.ProductSkus.AsNoTracking()
                             join product in db.Products.AsNoTracking() on sku.ProductId equals product.Id
                             join category in db.Categories.AsNoTracking() on product.CategoryId equals category.Id
                             join brand in db.Brands.AsNoTracking() on sku.BrandId equals brand.Id
                             join unit in db.Units.AsNoTracking() on sku.UnitId equals unit.Id
                             where sku.Id == request.ProductSkuId && sku.CompanyId == request.CompanyId && product.CompanyId == request.CompanyId
                             && !sku.IsDeleted && !product.IsDeleted && sku.ShowOnStore && sku.IsActive && sku.IsSellable && product.IsActive
                             && !category.IsDeleted && category.IsActive && !brand.IsDeleted && brand.IsActive && !unit.IsDeleted && unit.IsActive
                             select sku.Id).AnyAsync(cancellationToken);
        return new(visible);
    }
}
