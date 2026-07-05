using Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;

namespace Catalog.Products.Features.Products.GetProductSkuInventoryContext;

public class GetProductSkuInventoryContextHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductSkuInventoryContextQuery, GetProductSkuInventoryContextResult>
{
    public async Task<GetProductSkuInventoryContextResult> Handle(
        GetProductSkuInventoryContextQuery request,
        CancellationToken cancellationToken)
    {
        var context = await (
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking() on sku.ProductId equals product.Id
            join category in dbContext.Categories.AsNoTracking() on product.CategoryId equals category.Id
            join brand in dbContext.Brands.AsNoTracking() on sku.BrandId equals brand.Id
            join unit in dbContext.Units.AsNoTracking() on sku.UnitId equals unit.Id
            where sku.Id == request.ProductSkuId
                  && sku.CompanyId == request.CompanyId
                  && product.CompanyId == request.CompanyId
                  && !sku.IsDeleted
                  && !product.IsDeleted
                  && !category.IsDeleted
                  && !brand.IsDeleted
                  && !unit.IsDeleted
            select new GetProductSkuInventoryContextResult(
                sku.CompanyId,
                product.Id,
                sku.Id,
                product.ProductType,
                sku.ProductionType,
                sku.TrackingMode,
                product.IsActive,
                sku.IsActive,
                category.IsActive,
                brand.IsActive,
                unit.IsActive,
                sku.IsInventoryTracked,
                unit.Id,
                unit.UnitName,
                unit.UnitNameEng,
                unit.UnitCategory,
                unit.ConversionFactor,
                dbContext.ProductSkuPackages.AsNoTracking()
                    .Where(link => link.ProductSkuId == sku.Id && !link.IsDeleted && !link.ProductPackage.IsDeleted)
                    .Select(link => new GetProductSkuInventoryPackageResult(
                        link.ProductPackageId,
                        link.ProductPackage.Name,
                        link.ProductPackage.NameEng,
                        link.ProductPackage.Quantity,
                        link.ProductPackage.UnitId,
                        link.ProductPackage.UnitId.HasValue
                            ? dbContext.Units.AsNoTracking()
                                .Where(packageUnit => packageUnit.Id == link.ProductPackage.UnitId.Value && !packageUnit.IsDeleted)
                                .Select(packageUnit => packageUnit.UnitName)
                                .FirstOrDefault()
                            : null,
                        link.ProductPackage.UnitId.HasValue
                            ? dbContext.Units.AsNoTracking()
                                .Where(packageUnit => packageUnit.Id == link.ProductPackage.UnitId.Value && !packageUnit.IsDeleted)
                                .Select(packageUnit => packageUnit.UnitNameEng)
                                .FirstOrDefault()
                            : null,
                        link.ProductPackage.UnitId.HasValue
                            ? dbContext.Units.AsNoTracking()
                                .Where(packageUnit => packageUnit.Id == link.ProductPackage.UnitId.Value && !packageUnit.IsDeleted)
                                .Select(packageUnit => packageUnit.UnitCategory)
                                .FirstOrDefault()
                            : null,
                        link.ProductPackage.UnitId.HasValue
                            ? dbContext.Units.AsNoTracking()
                                .Where(packageUnit => packageUnit.Id == link.ProductPackage.UnitId.Value && !packageUnit.IsDeleted)
                                .Select(packageUnit => (decimal?)packageUnit.ConversionFactor)
                                .FirstOrDefault()
                            : null,
                        !link.ProductPackage.UnitId.HasValue
                            || dbContext.Units.AsNoTracking()
                                .Where(packageUnit => packageUnit.Id == link.ProductPackage.UnitId.Value && !packageUnit.IsDeleted)
                                .Select(packageUnit => packageUnit.IsActive)
                                .FirstOrDefault(),
                        link.ProductPackage.Barcode,
                        link.ProductPackage.IsActive))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (context is null)
            throw new Exception($"Product SKU not found for company: {request.ProductSkuId}");

        return context;
    }
}
