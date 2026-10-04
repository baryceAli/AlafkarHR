using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using Shared.Setup;
using SharedWithUI.Organization;
using SharedWithUI.Permissions;

namespace RealEstate.Features.SystemSetup;

public sealed class RealEstateReadinessContributor(RealEstateDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "real-estate";
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("real-estate", "Real Estate", "العقارات", "Property and unit master-data foundations.", "إعداد البيانات الأساسية للعقارات والوحدات.", "bi-buildings", 80, true, BusinessLineKeys.RealEstate, ["core"])
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("property-foundations", "real-estate", "Property foundation", "أساس العقارات", "Create at least one valid property.", "أنشئ عقاراً صالحاً واحداً على الأقل.", "bi-building", "/RealEstate/Properties", PermissionList.RealEstatePropertyPermissions.View, 710, false),
        new("property-units", "real-estate", "Unit structure", "هيكل الوحدات", "Create a valid unit linked to a property.", "أنشئ وحدة صالحة مرتبطة بعقار.", "bi-door-open", "/RealEstate/Units", PermissionList.RealEstateUnitPermissions.View, 720, false, ["property-foundations"]),
        new("property-utilities", "real-estate", "Utilities", "الخدمات", "Utility accounts are optional; enabled references must remain valid.", "حسابات الخدمات اختيارية ويجب أن تبقى المراجع المفعلة صالحة.", "bi-lightning-charge", "/RealEstate/Utilities", PermissionList.RealEstateUtilityPermissions.View, 730, true, ["property-units"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var propertyIds = await dbContext.Properties.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name != "" && x.Code != "")
            .Select(x => x.Id).ToListAsync(cancellationToken);
        var units = await dbContext.PropertyUnits.AsNoTracking()
            .Where(x => !x.IsDeleted && propertyIds.Contains(x.PropertyId))
            .Select(x => new { x.Id, x.PropertyId, x.UnitNumber }).ToListAsync(cancellationToken);
        var invalidUnits = units.Count(x => string.IsNullOrWhiteSpace(x.UnitNumber) || !propertyIds.Contains(x.PropertyId));

        var utilityAccounts = await dbContext.UtilityAccounts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.IsActive)
            .Select(x => new { x.PropertyId, x.UnitId, x.AccountNumber }).ToListAsync(cancellationToken);
        var unitIds = units.Select(x => x.Id).ToHashSet();
        var invalidUtilities = utilityAccounts.Count(x => !propertyIds.Contains(x.PropertyId) || (x.UnitId.HasValue && !unitIds.Contains(x.UnitId.Value)) || string.IsNullOrWhiteSpace(x.AccountNumber));

        var propertyComplete = propertyIds.Count > 0;
        var unitsComplete = units.Count > 0 && invalidUnits == 0;
        var utilitiesComplete = utilityAccounts.Count > 0 && invalidUtilities == 0;
        return
        [
            new("property-foundations", propertyComplete, propertyComplete ? "A valid property is ready." : "Create at least one valid property.", propertyComplete ? "يتوفر عقار صالح." : "أنشئ عقاراً صالحاً واحداً على الأقل."),
            new("property-units", unitsComplete, unitsComplete ? "A valid property unit structure is ready." : $"Create a unit linked to a valid property; {invalidUnits} unit(s) need attention.", unitsComplete ? "هيكل وحدات العقار صالح وجاهز." : $"أنشئ وحدة مرتبطة بعقار صالح؛ توجد {invalidUnits} وحدات تحتاج إلى مراجعة.", invalidUnits > 0),
            new("property-utilities", utilitiesComplete, utilitiesComplete ? "Enabled utility accounts have valid property and unit references." : invalidUtilities > 0 ? $"{invalidUtilities} enabled utility account(s) need attention." : "Utility accounts are optional.", utilitiesComplete ? "حسابات الخدمات المفعلة مرتبطة بعقارات ووحدات صالحة." : invalidUtilities > 0 ? $"توجد {invalidUtilities} حسابات خدمات مفعلة تحتاج إلى مراجعة." : "حسابات الخدمات اختيارية.", invalidUtilities > 0)
        ];
    }
}
