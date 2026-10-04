using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Permissions;

namespace Inventory.Warehouses.Features.SystemSetup;

public sealed class InventoryReadinessContributor(InventoryDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "inventory";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("warehouse-foundations", "supply-chain", "Warehouse foundation", "أساس المستودعات", "Create a branch-scoped warehouse and usable locations.", "أنشئ مستودعاً مرتبطاً بفرع ومواقع قابلة للاستخدام.", "bi-buildings", "/Inventory/Warehouse/List", PermissionList.WarehousePermissions.View, 220, false, ["catalog-foundations"]),
        new("inventory-operations", "supply-chain", "Inventory operations", "عمليات المخزون", "Configure receipt and delivery operation types and valid routes.", "اضبط أنواع عمليات الاستلام والتسليم والمسارات الصالحة.", "bi-signpost-split", "/Inventory/Controls", PermissionList.InventoryPermissions.View, 230, false, ["warehouse-foundations"]),
        new("inventory-advanced", "supply-chain", "Advanced inventory controls", "ضوابط المخزون المتقدمة", "Configure putaway, quality, cycle counting, tracking, and landed costs when needed.", "اضبط التخزين والجودة والجرد والتتبع والتكاليف الإضافية عند الحاجة.", "bi-sliders2", "/Inventory/Controls", PermissionList.InventoryPermissions.View, 235, true, ["inventory-operations"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var warehouses = await dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .Select(x => new { x.Id, x.BranchId, x.Location }).ToListAsync(cancellationToken);
        var validWarehouseIds = warehouses
            .Where(x => x.BranchId.HasValue && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => x.Id).ToHashSet();
        var locations = await dbContext.WarehouseLocations.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.Id, x.WarehouseId }).ToListAsync(cancellationToken);
        var locationIds = locations.Where(x => validWarehouseIds.Contains(x.WarehouseId)).Select(x => x.Id).ToHashSet();
        var warehouseComplete = validWarehouseIds.Count > 0 && locations.Any(x => validWarehouseIds.Contains(x.WarehouseId));

        var operationTypes = await dbContext.InventoryOperationTypes.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.Id, x.WarehouseId, x.OperationKind, x.DefaultSourceLocationId, x.DefaultDestinationLocationId })
            .ToListAsync(cancellationToken);
        var hasReceipt = operationTypes.Any(x => x.OperationKind == InventoryOperationKind.Receipt);
        var hasDelivery = operationTypes.Any(x => x.OperationKind == InventoryOperationKind.Delivery);
        var invalidOperationTypes = operationTypes.Count(x => !validWarehouseIds.Contains(x.WarehouseId)
            || x.DefaultSourceLocationId.HasValue && !locationIds.Contains(x.DefaultSourceLocationId.Value)
            || x.DefaultDestinationLocationId.HasValue && !locationIds.Contains(x.DefaultDestinationLocationId.Value));
        var routes = await dbContext.InventoryRoutes.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.Id, x.WarehouseId }).ToListAsync(cancellationToken);
        var routeIds = routes.Select(x => x.Id).ToHashSet();
        var operationTypeIds = operationTypes.Select(x => x.Id).ToHashSet();
        var rules = await dbContext.InventoryRouteRules.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.RouteId, x.WarehouseId, x.OperationTypeId, x.SourceLocationId, x.DestinationLocationId })
            .ToListAsync(cancellationToken);
        var invalidRoutes = routes.Count(x => x.WarehouseId.HasValue && !validWarehouseIds.Contains(x.WarehouseId.Value));
        var invalidRules = rules.Count(x => !routeIds.Contains(x.RouteId) || !validWarehouseIds.Contains(x.WarehouseId)
            || !operationTypeIds.Contains(x.OperationTypeId) || !locationIds.Contains(x.SourceLocationId) || !locationIds.Contains(x.DestinationLocationId));
        var operationsComplete = warehouseComplete && hasReceipt && hasDelivery && invalidOperationTypes + invalidRoutes + invalidRules == 0;
        var hasAdvanced = await dbContext.PutawayRules.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            || await dbContext.QualityInspections.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            || await dbContext.CycleCounts.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
            || await dbContext.LandedCostVouchers.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);

        return
        [
            new SetupReadinessResult(
                "warehouse-foundations",
                warehouseComplete,
                warehouseComplete ? "A branch-scoped warehouse with usable locations is ready." : "Create a warehouse linked to a branch and at least one active location.",
                warehouseComplete ? "يوجد مستودع مرتبط بفرع مع مواقع قابلة للاستخدام." : "أنشئ مستودعاً مرتبطاً بفرع وموقعاً نشطاً واحداً على الأقل."),
            new SetupReadinessResult(
                "inventory-operations",
                operationsComplete,
                operationsComplete ? "Receipt and delivery operations and enabled routes are valid." : $"Configure receipt and delivery operations; {invalidOperationTypes + invalidRoutes + invalidRules} enabled control reference(s) need attention.",
                operationsComplete ? "عمليات الاستلام والتسليم والمسارات المفعلة صالحة." : $"اضبط عمليات الاستلام والتسليم؛ توجد {invalidOperationTypes + invalidRoutes + invalidRules} مراجع مفعلة تحتاج إلى مراجعة."),
            new SetupReadinessResult(
                "inventory-advanced",
                hasAdvanced,
                hasAdvanced ? "Advanced inventory controls are in use." : "Advanced inventory controls are optional.",
                hasAdvanced ? "ضوابط المخزون المتقدمة مستخدمة." : "ضوابط المخزون المتقدمة اختيارية.")
        ];
    }
}
