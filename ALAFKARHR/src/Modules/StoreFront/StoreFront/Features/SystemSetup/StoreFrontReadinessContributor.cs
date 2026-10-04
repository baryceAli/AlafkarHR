using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Inventory;
using Shared.Contracts.Organization;
using Shared.Setup;
using SharedWithUI.Organization;
using SharedWithUI.Permissions;

namespace StoreFront.Features.SystemSetup;

public sealed class StoreFrontReadinessContributor(StoreFrontDbContext dbContext, ISender sender) : ISetupReadinessContributor
{
    public string ModuleKey => "store-front";

    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("store-front", "StoreFront & POS", "واجهة المتجر ونقاط البيع", "Stores, sellable catalog, POS defaults, and scoped access.", "إعداد المتاجر والكتالوج القابل للبيع وافتراضات نقاط البيع والوصول المقيد.", "bi-shop", 60, true, BusinessLineKeys.StoreFront, ["supply-chain", "sales"])
    ];

    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("store-organization", "store-front", "Store organization", "هيكل المتجر", "Activate a store linked to a StoreFront branch and departments.", "فعّل متجراً مرتبطاً بفرع واجهة متجر وأقسامه.", "bi-shop-window", "/StoreFront/Stores", PermissionList.StoreFrontStorePermissions.View, 510, false),
        new("store-inventory", "store-front", "Store inventory", "مخزون المتجر", "Validate each active store warehouse and branch association.", "تحقق من مستودع كل متجر نشط وارتباطه بالفرع.", "bi-box-seam", "/StoreFront/Stores", PermissionList.StoreFrontStorePermissions.View, 520, false, ["store-organization"]),
        new("store-sellable-catalog", "store-front", "Sellable catalog", "الكتالوج القابل للبيع", "Select at least one valid active SKU for sale.", "حدد وحدة حفظ مخزون نشطة وصالحة واحدة على الأقل للبيع.", "bi-upc-scan", "/StoreFront/Stores", PermissionList.StoreFrontItemPermissions.View, 530, false, ["store-inventory"]),
        new("store-access", "store-front", "Store access", "صلاحيات المتجر", "Assign store manager and cashier branch roles.", "عيّن أدوار مدير المتجر والكاشير على الفرع.", "bi-person-badge", "/StoreFront/Stores", PermissionList.StoreFrontStorePermissions.View, 550, false, ["store-organization"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var typeIds = await dbContext.StoreFrontTypes.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .Select(x => x.Id).ToListAsync(cancellationToken);
        var stores = await dbContext.StoreFronts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .Select(x => new { x.Id, x.BranchId, x.StoreFrontTypeId, x.DefaultWarehouseId, x.StoreManagerEmployeeId })
            .ToListAsync(cancellationToken);
        var storeIds = stores.Select(x => x.Id).ToHashSet();
        var departments = await dbContext.StoreFrontDepartments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && storeIds.Contains(x.StoreFrontId))
            .Select(x => x.StoreFrontId).Distinct().ToListAsync(cancellationToken);

        var invalidOrganization = stores.Count(x => !x.BranchId.HasValue || !typeIds.Contains(x.StoreFrontTypeId) || !x.StoreManagerEmployeeId.HasValue || !departments.Contains(x.Id));
        var invalidInventory = 0;
        foreach (var store in stores.Where(x => x.BranchId.HasValue))
        {
            try
            {
                var branch = await sender.Send(new GetBranchScopeInfoQuery(companyId, store.BranchId!.Value), cancellationToken);
                var warehouse = await sender.Send(new EnsureWarehouseBranchScopeQuery(companyId, store.DefaultWarehouseId, store.BranchId.Value), cancellationToken);
                if (branch.Specialization != 1 || !warehouse.IsValid) invalidInventory++;
            }
            catch
            {
                invalidInventory++;
            }
        }
        invalidInventory += stores.Count(x => !x.BranchId.HasValue || x.DefaultWarehouseId == Guid.Empty);

        var items = await dbContext.StoreFrontSellableItems.AsNoTracking()
            .Where(x => x.IsActive && storeIds.Contains(x.StoreFrontId))
            .Select(x => new { x.StoreFrontId, x.ProductSkuId, x.SkuCode }).ToListAsync(cancellationToken);
        var invalidItems = items.Count(x => x.ProductSkuId == Guid.Empty || string.IsNullOrWhiteSpace(x.SkuCode));

        var roleAssignments = await sender.Send(new GetCompanyBranchRoleAssignmentsForDashboardQuery(companyId), cancellationToken);
        var branchIds = stores.Where(x => x.BranchId.HasValue).Select(x => x.BranchId!.Value).Distinct().ToList();
        var missingRoleBranches = branchIds.Count(branchId =>
            !roleAssignments.Assignments.Any(x => x.BranchId == branchId && x.TemplateKey == "store-admin")
            || !roleAssignments.Assignments.Any(x => x.BranchId == branchId && x.TemplateKey == "store-cashier"));

        var organizationComplete = stores.Count > 0 && invalidOrganization == 0;
        var inventoryComplete = stores.Count > 0 && invalidInventory == 0;
        var catalogComplete = items.Count > 0 && invalidItems == 0;
        var accessComplete = branchIds.Count > 0 && missingRoleBranches == 0;
        return
        [
            Result("store-organization", organizationComplete, invalidOrganization, "Store organization is ready.", "Create an active store with a specialized branch, manager, type, and department.", "هيكل المتجر جاهز.", "أنشئ متجراً نشطاً بفرع متخصص ومدير ونوع وقسم."),
            Result("store-inventory", inventoryComplete, invalidInventory, "Store warehouse associations are valid.", "Link every active store to a valid warehouse in its StoreFront branch.", "ارتباطات مستودعات المتاجر صالحة.", "اربط كل متجر نشط بمستودع صالح في فرع واجهة المتجر."),
            Result("store-sellable-catalog", catalogComplete, invalidItems, "The sellable catalog is ready.", "Select at least one active item with a valid SKU.", "الكتالوج القابل للبيع جاهز.", "حدد صنفاً نشطاً واحداً على الأقل بوحدة حفظ مخزون صالحة."),
            Result("store-access", accessComplete, missingRoleBranches, "Store manager and cashier access is ready.", "Assign manager and cashier roles to every active StoreFront branch.", "صلاحيات مدير المتجر والكاشير جاهزة.", "عيّن أدوار المدير والكاشير لكل فرع واجهة متجر نشط.")
        ];
    }

    private static SetupReadinessResult Result(string key, bool complete, int invalid, string completeEn, string incompleteEn, string completeAr, string incompleteAr) =>
        new(key, complete, complete ? completeEn : $"{incompleteEn} {invalid} record(s) need attention.", complete ? completeAr : $"{incompleteAr} توجد {invalid} سجلات تحتاج إلى مراجعة.", invalid > 0);
}
