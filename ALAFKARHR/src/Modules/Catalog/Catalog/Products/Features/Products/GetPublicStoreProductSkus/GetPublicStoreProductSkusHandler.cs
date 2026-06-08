namespace Catalog.Products.Features.Products.GetPublicStoreProductSkus;

public record GetPublicStoreProductSkusQuery(PaginationRequest PaginationRequest)
    : IQuery<GetPublicStoreProductSkusResult>;

public record GetPublicStoreProductSkusResult(PaginatedResult<ProductSkuDto> ProductSkus);

public class GetPublicStoreProductSkusHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetPublicStoreProductSkusQuery, GetPublicStoreProductSkusResult>
{
    public async Task<GetPublicStoreProductSkusResult> Handle(
        GetPublicStoreProductSkusQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = Math.Max(request.PaginationRequest.PageIndex, 0);
        var pageSize = request.PaginationRequest.PageSize <= 0
            ? 12
            : Math.Min(request.PaginationRequest.PageSize, 50);

        var query =
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking()
                on sku.ProductId equals product.Id
            join brand in dbContext.Brands.AsNoTracking()
                on sku.BrandId equals brand.Id
            join package in dbContext.ProductPackages.AsNoTracking()
                on sku.PackageId equals package.Id into packageJoin
            from package in packageJoin.DefaultIfEmpty()
            where sku.ShowOnStore && !product.IsDeleted && !brand.IsDeleted
            orderby sku.CreatedAt descending
            select new ProductSkuDto
            {
                Id = sku.Id,
                ProductId = sku.ProductId,
                ProductName = product.Name,
                BrandId = sku.BrandId,
                BrandName = brand.Name,
                BrandNameEng = brand.NameEng,
                PackageId = sku.PackageId,
                PackageName = package == null ? null : package.Name,
                UnitId = sku.UnitId,
                Name = sku.Name,
                NameEng = sku.NameEng,
                SkuCode = sku.SkuCode,
                SkuCodeEng = sku.SkuCodeEng,
                SkuKey = sku.SkuKey,
                Barcode = sku.Barcode ?? string.Empty,
                Price = sku.Price,
                ImageUrl = sku.ImageUrl,
                CompanyId = sku.CompanyId,
                ShowOnStore = sku.ShowOnStore
            };

        var count = await query.LongCountAsync(cancellationToken);
        var productSkus = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetPublicStoreProductSkusResult(
            new PaginatedResult<ProductSkuDto>(
                pageIndex,
                pageSize,
                count,
                productSkus));
    }
}
