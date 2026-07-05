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

    public static async Task EnsureSkuPackageAssignmentsAsync(
        CatalogDbContext dbContext,
        IEnumerable<ProductSkuPackageDto> packageAssignments,
        Guid? skuUnitId,
        Guid companyId,
        Guid? excludingSkuId,
        CancellationToken cancellationToken)
    {
        var assignments = packageAssignments
            .Where(assignment => assignment.ProductPackageId != Guid.Empty)
            .ToList();

        if (!assignments.Any())
            return;

        await EnsurePackagesAsync(
            dbContext,
            assignments.Select(assignment => assignment.ProductPackageId),
            companyId,
            cancellationToken);

        var unitIds = assignments
            .Select(assignment => assignment.UnitId)
            .Where(id => id.HasValue && id.Value != Guid.Empty)
            .Select(id => id!.Value)
            .ToHashSet();

        if (skuUnitId.HasValue && skuUnitId.Value != Guid.Empty)
            unitIds.Add(skuUnitId.Value);

        var units = await dbContext.Units.AsNoTracking()
            .Where(unit => unit.CompanyId == companyId && unitIds.Contains(unit.Id) && unit.IsActive)
            .ToDictionaryAsync(unit => unit.Id, cancellationToken);

        foreach (var unitId in unitIds)
        {
            if (!units.ContainsKey(unitId))
                throw new Exception($"Unit not found for company: {unitId}");
        }

        if (skuUnitId.HasValue && skuUnitId.Value != Guid.Empty && units.TryGetValue(skuUnitId.Value, out var skuUnit))
        {
            foreach (var assignment in assignments.Where(x => x.UnitId.HasValue && x.UnitId.Value != Guid.Empty))
            {
                var packageUnit = units[assignment.UnitId!.Value];
                if (!string.Equals(skuUnit.UnitCategory, packageUnit.UnitCategory, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("SKU unit and packaging unit must use the same unit category.");
            }
        }

        foreach (var assignment in assignments)
        {
            if (assignment.Quantity <= 0)
                throw new Exception("Packaging quantity must be greater than zero.");

            if (!string.IsNullOrWhiteSpace(assignment.Barcode))
            {
                await EnsureBarcodeAvailableAsync(
                    dbContext,
                    companyId,
                    assignment.Barcode,
                    excludingSkuId,
                    assignment.Id == Guid.Empty ? null : assignment.Id,
                    cancellationToken);
            }
        }
    }

    public static async Task EnsureBarcodeAvailableAsync(
        CatalogDbContext dbContext,
        Guid companyId,
        string? barcode,
        Guid? excludingSkuId,
        Guid? excludingSkuPackageId,
        CancellationToken cancellationToken)
    {
        var result = await GetBarcodeValidationResultAsync(
            dbContext,
            companyId,
            barcode,
            excludingSkuId,
            excludingSkuPackageId,
            cancellationToken);

        if (!result.IsAvailable)
            throw new Exception($"Barcode is already used by {result.ConflictType}: {result.ConflictLabel ?? result.ConflictId?.ToString()}");
    }

    public static async Task<CatalogBarcodeValidationResultDto> GetBarcodeValidationResultAsync(
        CatalogDbContext dbContext,
        Guid companyId,
        string? barcode,
        Guid? excludingSkuId,
        Guid? excludingSkuPackageId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return new CatalogBarcodeValidationResultDto { IsAvailable = true };

        var normalizedBarcode = barcode.Trim();

        var skuConflict = await dbContext.ProductSkus.AsNoTracking()
            .Where(sku => sku.CompanyId == companyId
                && sku.Barcode == normalizedBarcode
                && (!excludingSkuId.HasValue || sku.Id != excludingSkuId.Value))
            .Select(sku => new CatalogBarcodeValidationResultDto
            {
                IsAvailable = false,
                ConflictType = "SKU",
                ConflictId = sku.Id,
                ConflictLabel = sku.SkuCodeEng
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (skuConflict is not null)
            return skuConflict;

        var packageConflict = await dbContext.ProductPackages.AsNoTracking()
            .Where(package => package.CompanyId == companyId && package.Barcode == normalizedBarcode)
            .Select(package => new CatalogBarcodeValidationResultDto
            {
                IsAvailable = false,
                ConflictType = "Package",
                ConflictId = package.Id,
                ConflictLabel = package.NameEng
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (packageConflict is not null)
            return packageConflict;

        var skuPackageConflict = await (
            from assignment in dbContext.ProductSkuPackages.AsNoTracking()
            join sku in dbContext.ProductSkus.AsNoTracking() on assignment.ProductSkuId equals sku.Id
            where sku.CompanyId == companyId
                && assignment.Barcode == normalizedBarcode
                && (!excludingSkuPackageId.HasValue || assignment.Id != excludingSkuPackageId.Value)
            select new CatalogBarcodeValidationResultDto
            {
                IsAvailable = false,
                ConflictType = "SKU Packaging",
                ConflictId = assignment.Id,
                ConflictLabel = sku.SkuCodeEng
            })
            .FirstOrDefaultAsync(cancellationToken);

        return skuPackageConflict ?? new CatalogBarcodeValidationResultDto { IsAvailable = true };
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
