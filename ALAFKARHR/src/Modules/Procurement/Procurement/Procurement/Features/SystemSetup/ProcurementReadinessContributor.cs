using Microsoft.EntityFrameworkCore;
using Procurement.Data;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace Procurement.Procurement.Features.SystemSetup;

public sealed class ProcurementReadinessContributor(ProcurementDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "procurement";

    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("procurement-controls", "purchasing", "Procurement controls", "ضوابط المشتريات", "Configure a valid supplier-item mapping or vendor price list.", "اضبط ربطاً صالحاً بين المورد والصنف أو قائمة أسعار مورد.", "bi-sliders", "/Procurement/Enhancements", PermissionList.RequestForQuotationPermissions.View, 415, false, ["supplier-foundations"]),
        new("reordering-controls", "purchasing", "Reordering and agreements", "إعادة الطلب والاتفاقيات", "Configure optional reordering, preferred suppliers, and purchase agreements.", "اضبط قواعد إعادة الطلب والموردين المفضلين واتفاقيات الشراء الاختيارية.", "bi-arrow-repeat", "/Procurement/ReorderingRules", PermissionList.RequestForQuotationPermissions.View, 420, true, ["procurement-controls"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var mappings = await dbContext.SupplierItems.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .Select(x => new { x.SupplierId, x.ProductId, x.ProductSkuId, x.SupplierSku })
            .ToListAsync(cancellationToken);
        var priceLists = await dbContext.VendorPricelists.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .Select(x => new { x.SupplierId, x.ProductId, x.ProductSkuId, x.UnitCost, x.ValidFrom, x.ValidTo })
            .ToListAsync(cancellationToken);
        var invalidMappings = mappings.Count(x => x.SupplierId == Guid.Empty || x.ProductId == Guid.Empty || x.ProductSkuId == Guid.Empty || string.IsNullOrWhiteSpace(x.SupplierSku));
        var invalidPrices = priceLists.Count(x => x.SupplierId == Guid.Empty || x.ProductId == Guid.Empty || x.ProductSkuId == Guid.Empty || x.UnitCost < 0 || (x.ValidTo.HasValue && x.ValidTo < x.ValidFrom));
        var controlsComplete = (mappings.Count > 0 || priceLists.Count > 0) && invalidMappings + invalidPrices == 0;

        var enabledRules = await dbContext.ReorderingRules.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.IsActive)
            .Select(x => new { x.ProductId, x.ProductSkuId, x.WarehouseId, x.SupplierId, x.MinimumQuantity, x.MaximumQuantity, x.ReorderQuantity })
            .ToListAsync(cancellationToken);
        var invalidRules = enabledRules.Count(x => x.ProductId == Guid.Empty || x.ProductSkuId == Guid.Empty || !x.WarehouseId.HasValue || x.MinimumQuantity < 0 || x.MaximumQuantity < x.MinimumQuantity || x.ReorderQuantity <= 0);
        var optionalReady = enabledRules.Count > 0 && invalidRules == 0;

        return
        [
            new("procurement-controls", controlsComplete,
                controlsComplete ? "Supplier mappings or vendor price lists are ready." : $"Create a valid supplier mapping or vendor price list; {invalidMappings + invalidPrices} record(s) need attention.",
                controlsComplete ? "روابط الموردين أو قوائم أسعار الموردين جاهزة." : $"أنشئ ربط مورد أو قائمة أسعار مورد صالحة؛ توجد {invalidMappings + invalidPrices} سجلات تحتاج إلى مراجعة.",
                invalidMappings + invalidPrices > 0),
            new("reordering-controls", optionalReady,
                optionalReady ? "Enabled reordering rules are valid." : invalidRules > 0 ? $"{invalidRules} enabled reordering rule(s) need attention." : "Reordering and purchase agreements are optional.",
                optionalReady ? "قواعد إعادة الطلب المفعلة صالحة." : invalidRules > 0 ? $"توجد {invalidRules} قواعد إعادة طلب مفعلة تحتاج إلى مراجعة." : "إعادة الطلب واتفاقيات الشراء اختيارية.",
                invalidRules > 0)
        ];
    }
}
