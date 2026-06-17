using MediatR;
using Pricing.Contracts.Pricings.Features.ResolvePrices;

namespace Catalog.Products.Features.Products.GetPublicStoreProductSkus;

public record GetPublicStoreProductSkusQuery(PublicStoreProductSkuRequest Request)
    : IQuery<GetPublicStoreProductSkusResult>;

public record GetPublicStoreProductSkusResult(PaginatedResult<ProductSkuDto> ProductSkus);

public record GetPublicStoreProductSkuFiltersQuery()
    : IQuery<GetPublicStoreProductSkuFiltersResult>;

public record GetPublicStoreProductSkuFiltersResult(PublicStoreProductSkuFilterMetadataDto Metadata);

public class GetPublicStoreProductSkusHandler(CatalogDbContext dbContext, ISender sender)
    : IQueryHandler<GetPublicStoreProductSkusQuery, GetPublicStoreProductSkusResult>,
      IQueryHandler<GetPublicStoreProductSkuFiltersQuery, GetPublicStoreProductSkuFiltersResult>
{
    public async Task<GetPublicStoreProductSkusResult> Handle(
        GetPublicStoreProductSkusQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = Math.Max(request.Request.PageIndex, 0);
        var pageSize = request.Request.PageSize <= 0
            ? 12
            : Math.Min(request.Request.PageSize, 50);

        var productSkus = await ApplyNonPriceFilters(GetVisibleStoreSkuQuery(), request.Request)
            .ToListAsync(cancellationToken);

        if (request.Request.CustomerId.HasValue)
        {
            await ApplyResolvedPricesAsync(productSkus, request.Request.CustomerId.Value, cancellationToken);
        }

        productSkus = ApplyPriceFilters(productSkus, request.Request);
        productSkus = ApplySorting(productSkus, request.Request).ToList();

        var count = productSkus.LongCount();
        var page = productSkus
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList();

        return new GetPublicStoreProductSkusResult(
            new PaginatedResult<ProductSkuDto>(
                pageIndex,
                pageSize,
                count,
                page));
    }

    public async Task<GetPublicStoreProductSkuFiltersResult> Handle(
        GetPublicStoreProductSkuFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var query = GetVisibleStoreSkuQuery();

        var categories = await query
            .Where(sku => sku.CategoryId != Guid.Empty)
            .Select(sku => new PublicStoreFilterOptionDto
            {
                Id = sku.CategoryId,
                Name = sku.CategoryName,
                NameEng = sku.CategoryNameEng
            })
            .Distinct()
            .OrderBy(category => category.NameEng ?? category.Name)
            .ToListAsync(cancellationToken);

        var brands = await query
            .Where(sku => sku.BrandId != Guid.Empty)
            .Select(sku => new PublicStoreFilterOptionDto
            {
                Id = sku.BrandId,
                Name = sku.BrandName,
                NameEng = sku.BrandNameEng
            })
            .Distinct()
            .OrderBy(brand => brand.NameEng ?? brand.Name)
            .ToListAsync(cancellationToken);

        var packages = await (
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking()
                on sku.ProductId equals product.Id
            join category in dbContext.Categories.AsNoTracking()
                on product.CategoryId equals category.Id
            join brand in dbContext.Brands.AsNoTracking()
                on sku.BrandId equals brand.Id
            join skuPackage in dbContext.ProductSkuPackages.AsNoTracking()
                on sku.Id equals skuPackage.ProductSkuId
            where sku.ShowOnStore
                  && !sku.IsDeleted
                  && !product.IsDeleted
                  && !category.IsDeleted
                  && !brand.IsDeleted
            select new PublicStoreFilterOptionDto
            {
                Id = skuPackage.ProductPackageId,
                Name = skuPackage.ProductPackage.Name,
                NameEng = skuPackage.ProductPackage.NameEng
            })
            .Distinct()
            .OrderBy(package => package.NameEng ?? package.Name)
            .ToListAsync(cancellationToken);

        var priceBounds = await query
            .GroupBy(_ => 1)
            .Select(group => new
            {
                MinPrice = group.Min(sku => sku.Price),
                MaxPrice = group.Max(sku => sku.Price)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new GetPublicStoreProductSkuFiltersResult(
            new PublicStoreProductSkuFilterMetadataDto
            {
                Categories = categories,
                Brands = brands,
                Packages = packages,
                MinPrice = priceBounds?.MinPrice,
                MaxPrice = priceBounds?.MaxPrice
            });
    }

    private IQueryable<ProductSkuDto> GetVisibleStoreSkuQuery()
    {
        return
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking()
                on sku.ProductId equals product.Id
            join category in dbContext.Categories.AsNoTracking()
                on product.CategoryId equals category.Id
            join brand in dbContext.Brands.AsNoTracking()
                on sku.BrandId equals brand.Id
            where sku.ShowOnStore
                  && !sku.IsDeleted
                  && !product.IsDeleted
                  && !category.IsDeleted
                  && !brand.IsDeleted
            select new ProductSkuDto
            {
                Id = sku.Id,
                ProductId = sku.ProductId,
                ProductName = product.Name,
                ProductNameEng = product.NameEng,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                CategoryNameEng = category.NameEng,
                BrandId = sku.BrandId,
                BrandName = brand.Name,
                BrandNameEng = brand.NameEng,
                PackageId = dbContext.ProductSkuPackages
                    .Where(p => p.ProductSkuId == sku.Id)
                    .Select(p => p.ProductPackageId)
                    .FirstOrDefault(),
                PackageName = dbContext.ProductSkuPackages
                    .Where(p => p.ProductSkuId == sku.Id)
                    .Select(p => p.ProductPackage.Name)
                    .FirstOrDefault(),
                PackageNameEng = dbContext.ProductSkuPackages
                    .Where(p => p.ProductSkuId == sku.Id)
                    .Select(p => p.ProductPackage.NameEng)
                    .FirstOrDefault(),
                UnitId = sku.UnitId,
                Name = sku.Name,
                NameEng = sku.NameEng,
                SkuCode = sku.SkuCode,
                SkuCodeEng = sku.SkuCodeEng,
                SkuKey = sku.SkuKey,
                Barcode = sku.Barcode ?? string.Empty,
                Price = sku.Price,
                BasePrice = sku.Price,
                PriceSource = "Catalog",
                FinalUnitAmount = sku.Price,
                ProductionType = sku.ProductionType,
                ImageUrl = sku.ImageUrl,
                CompanyId = sku.CompanyId,
                ShowOnStore = sku.ShowOnStore,
                CreatedAt = sku.CreatedAt,
                Packages = dbContext.ProductSkuPackages
                    .Where(p => p.ProductSkuId == sku.Id)
                    .Select(p => new ProductPackageDto
                    {
                        Id = p.ProductPackage.Id,
                        Name = p.ProductPackage.Name,
                        NameEng = p.ProductPackage.NameEng,
                        Quantity = p.ProductPackage.Quantity,
                        CompanyId = p.ProductPackage.CompanyId
                    })
                    .ToList()
            };
    }

    private IQueryable<ProductSkuDto> ApplyNonPriceFilters(
        IQueryable<ProductSkuDto> query,
        PublicStoreProductSkuRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(sku =>
                sku.Name.Contains(searchTerm) ||
                sku.NameEng.Contains(searchTerm) ||
                sku.SkuCode.Contains(searchTerm) ||
                sku.SkuCodeEng.Contains(searchTerm) ||
                (sku.ProductName != null && sku.ProductName.Contains(searchTerm)) ||
                (sku.ProductNameEng != null && sku.ProductNameEng.Contains(searchTerm)) ||
                (sku.BrandName != null && sku.BrandName.Contains(searchTerm)) ||
                (sku.BrandNameEng != null && sku.BrandNameEng.Contains(searchTerm)) ||
                (sku.CategoryName != null && sku.CategoryName.Contains(searchTerm)) ||
                (sku.CategoryNameEng != null && sku.CategoryNameEng.Contains(searchTerm)) ||
                (sku.PackageName != null && sku.PackageName.Contains(searchTerm)) ||
                (sku.PackageNameEng != null && sku.PackageNameEng.Contains(searchTerm)) ||
                dbContext.ProductSkuPackages.Any(package =>
                    package.ProductSkuId == sku.Id &&
                    (package.ProductPackage.Name.Contains(searchTerm) ||
                     package.ProductPackage.NameEng.Contains(searchTerm))));
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
            query = query.Where(sku => sku.CategoryId == request.CategoryId.Value);

        if (request.BrandId.HasValue && request.BrandId.Value != Guid.Empty)
            query = query.Where(sku => sku.BrandId == request.BrandId.Value);

        if (request.PackageId.HasValue && request.PackageId.Value != Guid.Empty)
        {
            query = query.Where(sku => dbContext.ProductSkuPackages.Any(package =>
                package.ProductSkuId == sku.Id &&
                package.ProductPackageId == request.PackageId.Value));
        }

        return query;
    }

    private static List<ProductSkuDto> ApplyPriceFilters(
        List<ProductSkuDto> productSkus,
        PublicStoreProductSkuRequest request)
    {
        if (request.MinPrice.HasValue)
            productSkus = productSkus.Where(sku => sku.Price >= request.MinPrice.Value).ToList();

        if (request.MaxPrice.HasValue)
            productSkus = productSkus.Where(sku => sku.Price <= request.MaxPrice.Value).ToList();

        return productSkus;
    }

    private static IEnumerable<ProductSkuDto> ApplySorting(
        IEnumerable<ProductSkuDto> productSkus,
        PublicStoreProductSkuRequest request)
    {
        return (request.SortBy ?? "newest").Trim().ToLowerInvariant() switch
        {
            "name" => request.SortDescending
                ? productSkus.OrderByDescending(sku => sku.NameEng).ThenByDescending(sku => sku.Name)
                : productSkus.OrderBy(sku => sku.NameEng).ThenBy(sku => sku.Name),
            "price" => request.SortDescending
                ? productSkus.OrderByDescending(sku => sku.Price)
                : productSkus.OrderBy(sku => sku.Price),
            "brand" => request.SortDescending
                ? productSkus.OrderByDescending(sku => sku.BrandNameEng).ThenByDescending(sku => sku.BrandName)
                : productSkus.OrderBy(sku => sku.BrandNameEng).ThenBy(sku => sku.BrandName),
            _ => request.SortDescending
                ? productSkus.OrderByDescending(sku => sku.CreatedAt)
                : productSkus.OrderBy(sku => sku.CreatedAt)
        };
    }

    private async Task ApplyResolvedPricesAsync(
        List<ProductSkuDto> productSkus,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        foreach (var companyGroup in productSkus.GroupBy(sku => sku.CompanyId))
        {
            var skus = companyGroup.ToList();
            var prices = await sender.Send(
                new ResolvePricesQuery(
                    customerId,
                    companyGroup.Key,
                    null,
                    DateTime.UtcNow,
                    skus.Select(sku => new ResolvePriceLineDto(
                        sku.Id,
                        sku.UnitId,
                        1m,
                        sku.TaxRate)).ToList()),
                cancellationToken);

            var priceBySku = prices.Prices.ToDictionary(price => price.ProductSkuId);
            foreach (var sku in skus)
            {
                if (!priceBySku.TryGetValue(sku.Id, out var price))
                    continue;

                sku.Price = price.FinalUnitAmount > 0m ? price.FinalUnitAmount : price.UnitPrice;
                sku.PriceListId = price.PriceListId;
                sku.DiscountRate = price.DiscountRate;
                sku.TaxRate = price.TaxRate;
                sku.PriceSource = price.PriceSource;
                sku.PromotionUnitPrice = price.PromotionUnitPrice;
                sku.FinalUnitAmount = price.FinalUnitAmount;
            }
        }
    }
}
