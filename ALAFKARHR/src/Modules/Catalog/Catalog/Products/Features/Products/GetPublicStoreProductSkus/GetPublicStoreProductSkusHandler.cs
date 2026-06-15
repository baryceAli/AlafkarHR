namespace Catalog.Products.Features.Products.GetPublicStoreProductSkus;

public record GetPublicStoreProductSkusQuery(PublicStoreProductSkuRequest Request)
    : IQuery<GetPublicStoreProductSkusResult>;

public record GetPublicStoreProductSkusResult(PaginatedResult<ProductSkuDto> ProductSkus);

public record GetPublicStoreProductSkuFiltersQuery()
    : IQuery<GetPublicStoreProductSkuFiltersResult>;

public record GetPublicStoreProductSkuFiltersResult(PublicStoreProductSkuFilterMetadataDto Metadata);

public class GetPublicStoreProductSkusHandler(CatalogDbContext dbContext)
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

        var query = ApplyFilters(GetVisibleStoreSkuQuery(), request.Request);
        query = ApplySorting(query, request.Request);

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

        var packages = await query
            .Where(sku => sku.PackageId.HasValue)
            .Select(sku => new PublicStoreFilterOptionDto
            {
                Id = sku.PackageId!.Value,
                Name = sku.PackageName,
                NameEng = sku.PackageNameEng
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
            join package in dbContext.ProductPackages.AsNoTracking()
                on sku.PackageId equals package.Id into packageJoin
            from package in packageJoin.DefaultIfEmpty()
            where sku.ShowOnStore
                  && !sku.IsDeleted
                  && !product.IsDeleted
                  && !category.IsDeleted
                  && !brand.IsDeleted
                  && (package == null || !package.IsDeleted)
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
                PackageId = sku.PackageId,
                PackageName = package == null ? null : package.Name,
                PackageNameEng = package == null ? null : package.NameEng,
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
                ShowOnStore = sku.ShowOnStore,
                CreatedAt = sku.CreatedAt
            };
    }

    private static IQueryable<ProductSkuDto> ApplyFilters(
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
                (sku.PackageNameEng != null && sku.PackageNameEng.Contains(searchTerm)));
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            query = query.Where(sku => sku.CategoryId == request.CategoryId.Value);
        }

        if (request.BrandId.HasValue && request.BrandId.Value != Guid.Empty)
        {
            query = query.Where(sku => sku.BrandId == request.BrandId.Value);
        }

        if (request.PackageId.HasValue && request.PackageId.Value != Guid.Empty)
        {
            query = query.Where(sku => sku.PackageId == request.PackageId.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(sku => sku.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(sku => sku.Price <= request.MaxPrice.Value);
        }

        return query;
    }

    private static IQueryable<ProductSkuDto> ApplySorting(
        IQueryable<ProductSkuDto> query,
        PublicStoreProductSkuRequest request)
    {
        return (request.SortBy ?? "newest").Trim().ToLowerInvariant() switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(sku => sku.NameEng).ThenByDescending(sku => sku.Name)
                : query.OrderBy(sku => sku.NameEng).ThenBy(sku => sku.Name),
            "price" => request.SortDescending
                ? query.OrderByDescending(sku => sku.Price)
                : query.OrderBy(sku => sku.Price),
            "brand" => request.SortDescending
                ? query.OrderByDescending(sku => sku.BrandNameEng).ThenByDescending(sku => sku.BrandName)
                : query.OrderBy(sku => sku.BrandNameEng).ThenBy(sku => sku.BrandName),
            _ => request.SortDescending
                ? query.OrderByDescending(sku => sku.CreatedAt)
                : query.OrderBy(sku => sku.CreatedAt)
        };
    }
}
