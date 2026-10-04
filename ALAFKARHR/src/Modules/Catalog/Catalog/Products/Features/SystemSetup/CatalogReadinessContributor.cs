using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace Catalog.Products.Features.SystemSetup;

public sealed class CatalogReadinessContributor(CatalogDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "catalog";
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("supply-chain", "Catalog & inventory", "الكتالوج والمخزون", "Product master data, warehouses, and inventory controls.", "البيانات الأساسية للمنتجات والمستودعات وضوابط المخزون.", "bi-box-seam", 30, true, null, ["core"])
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("catalog-foundations", "supply-chain", "Catalog foundations", "أسس الكتالوج", "Configure units, categories, products, and SKUs.", "اضبط الوحدات والفئات والمنتجات ووحدات حفظ المخزون.", "bi-tags", "/Warehouse/Product/Dashboard", PermissionList.ProductPermissions.View, 210, false),
        new("catalog-advanced", "supply-chain", "Brands, variants & packages", "العلامات والمتغيرات والعبوات", "Add optional catalog classifications and packaging.", "أضف تصنيفات الكتالوج والعبوات الاختيارية.", "bi-diagram-2", "/Warehouse/Product/Brand/List", PermissionList.ProductPermissions.View, 215, true, ["catalog-foundations"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var categoryIds = await dbContext.Categories.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => x.Id).ToListAsync(cancellationToken);
        var unitIds = await dbContext.Units.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => x.Id).ToListAsync(cancellationToken);
        var products = await dbContext.Products.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.Id, x.CategoryId }).ToListAsync(cancellationToken);
        var productIds = products.Where(x => categoryIds.Contains(x.CategoryId)).Select(x => x.Id).ToHashSet();
        var skus = await dbContext.ProductSkus.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.ProductId, x.UnitId }).ToListAsync(cancellationToken);
        var invalidSkuCount = skus.Count(x => !productIds.Contains(x.ProductId) || !unitIds.Contains(x.UnitId));
        var complete = categoryIds.Count > 0 && unitIds.Count > 0 && productIds.Count > 0 && skus.Count > 0 && invalidSkuCount == 0;
        var hasAdvanced = await dbContext.Brands.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            || await dbContext.Variants.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            || await dbContext.ProductPackages.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);

        return
        [
            new SetupReadinessResult(
                "catalog-foundations",
                complete,
                complete ? "Units, categories, active products, and valid SKUs are ready." : $"Create a unit, category, active product, and SKU; {invalidSkuCount} SKU relationship(s) need attention.",
                complete ? "الوحدات والفئات والمنتجات النشطة ووحدات حفظ المخزون الصالحة جاهزة." : $"أنشئ وحدة وفئة ومنتجاً نشطاً ووحدة حفظ مخزون؛ توجد {invalidSkuCount} علاقات تحتاج إلى مراجعة."),
            new SetupReadinessResult(
                "catalog-advanced",
                hasAdvanced,
                hasAdvanced ? "Optional catalog classifications are available." : "Brands, variants, and packages are optional.",
                hasAdvanced ? "تتوفر تصنيفات كتالوج اختيارية." : "العلامات والمتغيرات والعبوات اختيارية.")
        ];
    }
}
