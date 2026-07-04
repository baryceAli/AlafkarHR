using Catalog.Contracts.Products.Features.ResolveCatalogBarcode;

namespace Catalog.Products.Features.Products.ResolveCatalogBarcode;

public class ResolveCatalogBarcodeHandler(CatalogDbContext dbContext)
    : IQueryHandler<ResolveCatalogBarcodeQuery, ResolveCatalogBarcodeResult>
{
    public async Task<ResolveCatalogBarcodeResult> Handle(ResolveCatalogBarcodeQuery request, CancellationToken cancellationToken)
    {
        var barcode = request.Barcode.Trim();
        if (string.IsNullOrWhiteSpace(barcode))
            return new ResolveCatalogBarcodeResult([]);

        var skuMatches = await (
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking() on sku.ProductId equals product.Id
            join category in dbContext.Categories.AsNoTracking() on product.CategoryId equals category.Id
            join brand in dbContext.Brands.AsNoTracking() on sku.BrandId equals brand.Id
            join unit in dbContext.Units.AsNoTracking() on sku.UnitId equals unit.Id
            where sku.CompanyId == request.CompanyId
                && sku.Barcode == barcode
                && !sku.IsDeleted
                && !product.IsDeleted
                && !category.IsDeleted
                && !brand.IsDeleted
                && !unit.IsDeleted
            select new ResolvedCatalogBarcodeItem(
                sku.CompanyId,
                product.Id,
                sku.Id,
                null,
                sku.SkuCode,
                sku.Name,
                sku.NameEng,
                product.ProductType,
                sku.ProductionType,
                product.IsActive,
                sku.IsActive,
                true,
                category.IsActive,
                brand.IsActive,
                unit.IsActive,
                sku.IsInventoryTracked,
                1m,
                unit.Id,
                unit.ConversionFactor))
            .ToListAsync(cancellationToken);

        var packageMatches = await (
            from package in dbContext.ProductPackages.AsNoTracking()
            join link in dbContext.ProductSkuPackages.AsNoTracking() on package.Id equals link.ProductPackageId
            join sku in dbContext.ProductSkus.AsNoTracking() on link.ProductSkuId equals sku.Id
            join product in dbContext.Products.AsNoTracking() on sku.ProductId equals product.Id
            join category in dbContext.Categories.AsNoTracking() on product.CategoryId equals category.Id
            join brand in dbContext.Brands.AsNoTracking() on sku.BrandId equals brand.Id
            join unit in dbContext.Units.AsNoTracking() on sku.UnitId equals unit.Id
            where package.CompanyId == request.CompanyId
                && package.Barcode == barcode
                && !package.IsDeleted
                && !link.IsDeleted
                && !sku.IsDeleted
                && !product.IsDeleted
                && !category.IsDeleted
                && !brand.IsDeleted
                && !unit.IsDeleted
            select new ResolvedCatalogBarcodeItem(
                package.CompanyId,
                product.Id,
                sku.Id,
                package.Id,
                package.Barcode ?? package.NameEng,
                package.Name,
                package.NameEng,
                product.ProductType,
                sku.ProductionType,
                product.IsActive,
                sku.IsActive,
                package.IsActive,
                category.IsActive,
                brand.IsActive,
                unit.IsActive,
                sku.IsInventoryTracked,
                package.Quantity,
                package.UnitId,
                package.UnitId.HasValue
                    ? dbContext.Units.AsNoTracking()
                        .Where(packageUnit => packageUnit.Id == package.UnitId.Value && !packageUnit.IsDeleted)
                        .Select(packageUnit => (decimal?)packageUnit.ConversionFactor)
                        .FirstOrDefault()
                    : null))
            .ToListAsync(cancellationToken);

        return new ResolveCatalogBarcodeResult(skuMatches.Concat(packageMatches).ToList());
    }
}
