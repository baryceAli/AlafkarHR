using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Catering.Enums;
using SharedWithUI.Organization;
using SharedWithUI.Permissions;

namespace Catering.Features.SystemSetup;

public sealed class CateringReadinessContributor(CateringDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "catering";
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("catering", "Catering", "الإعاشة", "Catering locations, meals, and service foundations.", "إعداد مواقع الإعاشة والوجبات وأسس الخدمة.", "bi-cup-hot", 70, true, BusinessLineKeys.Catering, ["supply-chain", "sales"])
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("catering-locations", "catering", "Catering locations", "مواقع الإعاشة", "Create a valid area and square structure.", "أنشئ هيكل مناطق ومربعات صالحاً.", "bi-geo-alt", "/Catering/Locations", PermissionList.CateringLocationPermissions.View, 610, false),
        new("catering-meals", "catering", "Meal foundation", "أساس الوجبات", "Create an active meal with valid catalog components.", "أنشئ وجبة نشطة بمكونات كتالوج صالحة.", "bi-cup-straw", "/Catering/Meals", PermissionList.CateringMealPermissions.View, 620, false),
        new("catering-service", "catering", "Service foundation", "أساس الخدمة", "Review customer-facing service prerequisites without requiring operational contracts.", "راجع متطلبات الخدمة الموجهة للعملاء دون اشتراط عقود تشغيلية.", "bi-clipboard-check", "/Catering/Dashboard", PermissionList.CateringContractPermissions.View, 630, false, ["catering-locations", "catering-meals"]),
        new("catering-resources", "catering", "Packaging & resources", "التغليف والموارد", "Packaging and resource assignments are optional during initial setup.", "التغليف وتعيينات الموارد اختيارية أثناء الإعداد الأولي.", "bi-box2-heart", "/Catering/Packaging", PermissionList.CateringPackagingPermissions.View, 640, true, ["catering-service"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var areaIds = await dbContext.CateringAreas.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .Select(x => x.Id).ToListAsync(cancellationToken);
        var squares = await dbContext.CateringSquares.AsNoTracking()
            .Where(x => x.IsActive && areaIds.Contains(x.AreaId))
            .Select(x => new { x.AreaId, x.Code, x.Name }).ToListAsync(cancellationToken);
        var locationsComplete = areaIds.Count > 0 && squares.Count > 0 && squares.All(x => !string.IsNullOrWhiteSpace(x.Code) && !string.IsNullOrWhiteSpace(x.Name));

        var meals = await dbContext.MealDefinitions.AsNoTracking().Include(x => x.Components)
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .ToListAsync(cancellationToken);
        var invalidMeals = meals.Count(meal => meal.StructureType == CateringMealStructureType.Product
            ? !meal.ProductSkuId.HasValue || meal.ProductSkuId == Guid.Empty
            : meal.Components.Count == 0 || meal.Components.Any(component => component.ProductSkuId == Guid.Empty || component.QuantityPerMeal <= 0));
        var mealsComplete = meals.Count > 0 && invalidMeals == 0;
        var serviceComplete = locationsComplete && mealsComplete;

        return
        [
            new("catering-locations", locationsComplete, locationsComplete ? "An active area and square are ready." : "Create an active area with at least one valid square.", locationsComplete ? "منطقة ومربع نشطان جاهزان." : "أنشئ منطقة نشطة تحتوي على مربع صالح واحد على الأقل."),
            new("catering-meals", mealsComplete, mealsComplete ? "Active meals and their catalog components are valid." : $"Create a valid active meal; {invalidMeals} meal(s) need attention.", mealsComplete ? "الوجبات النشطة ومكوناتها في الكتالوج صالحة." : $"أنشئ وجبة نشطة صالحة؛ توجد {invalidMeals} وجبات تحتاج إلى مراجعة.", invalidMeals > 0),
            new("catering-service", serviceComplete, serviceComplete ? "Customer-facing catering prerequisites are ready." : "Complete catering locations and meal foundations. Operational contracts are not required.", serviceComplete ? "متطلبات خدمة الإعاشة الموجهة للعملاء جاهزة." : "أكمل مواقع الإعاشة وأسس الوجبات. لا تُشترط العقود التشغيلية."),
            new("catering-resources", false, "Packaging and resource assignments are optional.", "التغليف وتعيينات الموارد اختيارية.")
        ];
    }
}
