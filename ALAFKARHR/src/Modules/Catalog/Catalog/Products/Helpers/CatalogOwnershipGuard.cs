namespace Catalog.Products.Helpers;

internal static class CatalogOwnershipGuard
{
    public static async Task EnsureCategoryAsync(CatalogDbContext dbContext, Guid? categoryId, Guid companyId, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue || categoryId.Value == Guid.Empty)
            throw new Exception("Category is required");

        var exists = await dbContext.Categories.AsNoTracking()
            .AnyAsync(x => x.Id == categoryId.Value && x.CompanyId == companyId && x.IsActive, cancellationToken);

        if (!exists)
            throw new Exception($"Category not found for company: {categoryId.Value}");
    }

    public static async Task EnsureBrandAsync(CatalogDbContext dbContext, Guid brandId, Guid companyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Brands.AsNoTracking()
            .AnyAsync(x => x.Id == brandId && x.CompanyId == companyId && x.IsActive, cancellationToken);

        if (!exists)
            throw new Exception($"Brand not found for company: {brandId}");
    }

    public static async Task EnsureUnitAsync(CatalogDbContext dbContext, Guid? unitId, Guid companyId, CancellationToken cancellationToken)
    {
        if (!unitId.HasValue || unitId.Value == Guid.Empty)
            throw new Exception("Unit is required");

        var exists = await dbContext.Units.AsNoTracking()
            .AnyAsync(x => x.Id == unitId.Value && x.CompanyId == companyId && x.IsActive, cancellationToken);

        if (!exists)
            throw new Exception($"Unit not found for company: {unitId.Value}");
    }

    public static async Task EnsurePackagesAsync(CatalogDbContext dbContext, IEnumerable<Guid> packageIds, Guid companyId, CancellationToken cancellationToken)
    {
        var ids = packageIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (!ids.Any())
            return;

        var existingIds = await dbContext.ProductPackages.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && ids.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingId = ids.Except(existingIds).FirstOrDefault();
        if (missingId != Guid.Empty)
            throw new Exception($"Package not found for company: {missingId}");
    }

    public static async Task EnsureProductAsync(CatalogDbContext dbContext, Guid productId, Guid companyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Products.AsNoTracking()
            .AnyAsync(x => x.Id == productId && x.CompanyId == companyId && x.IsActive, cancellationToken);

        if (!exists)
            throw new Exception($"Product not found for company: {productId}");
    }

    public static async Task EnsureVariantValuesAsync(CatalogDbContext dbContext, IEnumerable<ProductSkuVariantDto> variants, Guid companyId, CancellationToken cancellationToken)
    {
        var requested = variants
            .Where(x => x.VariantId != Guid.Empty && x.VariantValueId != Guid.Empty)
            .Select(x => new { x.VariantId, x.VariantValueId })
            .Distinct()
            .ToList();

        if (!requested.Any())
            return;

        var variantIds = requested.Select(x => x.VariantId).Distinct().ToList();
        var valueIds = requested.Select(x => x.VariantValueId).Distinct().ToList();

        var validPairs = await dbContext.VariantValues.AsNoTracking()
            .Where(value => valueIds.Contains(value.Id))
            .Join(
                dbContext.Variants.AsNoTracking().Where(variant => variant.CompanyId == companyId && variantIds.Contains(variant.Id)),
                value => value.VariantId,
                variant => variant.Id,
                (value, variant) => new { VariantId = variant.Id, VariantValueId = value.Id, variant.CreationMode, VariantIsActive = variant.IsActive, ValueIsActive = value.IsActive })
            .ToListAsync(cancellationToken);

        var missing = requested.FirstOrDefault(x => !validPairs.Any(pair =>
            pair.VariantId == x.VariantId &&
            pair.VariantValueId == x.VariantValueId &&
            pair.VariantIsActive &&
            pair.ValueIsActive &&
            pair.CreationMode != VariantCreationMode.Never));

        if (missing is not null)
            throw new Exception($"Variant value not found for company: {missing.VariantValueId}");

        var duplicateVariant = requested
            .GroupBy(x => x.VariantId)
            .FirstOrDefault(group => group.Select(x => x.VariantValueId).Distinct().Count() > 1);

        if (duplicateVariant is not null)
            throw new Exception($"Only one value can be selected for variant: {duplicateVariant.Key}");
    }

    public static async Task EnsureUniqueActiveSkuCombinationAsync(
        CatalogDbContext dbContext,
        Guid companyId,
        Guid productId,
        Guid brandId,
        Guid? packageId,
        IEnumerable<ProductSkuVariantDto> variants,
        Guid? excludingSkuId,
        CancellationToken cancellationToken)
    {
        var requestedPairs = variants
            .Where(x => x.VariantId != Guid.Empty && x.VariantValueId != Guid.Empty)
            .Select(x => $"{x.VariantId:N}:{x.VariantValueId:N}")
            .OrderBy(x => x)
            .ToArray();

        var candidates = await dbContext.ProductSkus
            .AsNoTracking()
            .Include(x => x.Variants)
            .Where(x => x.CompanyId == companyId
                && x.ProductId == productId
                && x.BrandId == brandId
                && x.PackageId == packageId
                && x.IsActive
                && (!excludingSkuId.HasValue || x.Id != excludingSkuId.Value))
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            var candidatePairs = candidate.Variants
                .Where(x => !x.IsDeleted)
                .Select(x => $"{x.VariantId:N}:{x.VariantValueId:N}")
                .OrderBy(x => x)
                .ToArray();

            if (requestedPairs.SequenceEqual(candidatePairs))
                throw new Exception("An active SKU with the same product, brand, package, and variant combination already exists.");
        }
    }
}
