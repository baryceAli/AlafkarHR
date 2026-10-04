using GeneralSettings.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;

namespace GeneralSettings.GeneralSettings.Features.SystemSetup;

public sealed class GeneralSettingsReadinessContributor(GeneralSettingsDbContext dbContext)
    : ISetupReadinessContributor
{
    public string ModuleKey => "general-settings";
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("core", "Core setup", "الإعداد الأساسي", "Company, organization, defaults, access, and accounting readiness.", "جاهزية الشركة والهيكل والإعدادات والوصول والمحاسبة.", "bi-building-gear", 10, false)
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("company-defaults", "core", "Currency & operational defaults", "العملة والإعدادات التشغيلية", "Set the default currency, location, and operational defaults.", "حدد العملة والموقع والإعدادات التشغيلية الافتراضية.", "bi-sliders2", "/GeneralSettings/SystemSettings", PermissionList.SystemSettingsPermissions.View, 30, false),
        new("review-summary", "core", "Readiness review", "مراجعة الجاهزية", "Review the live completion result and return to any area that needs attention.", "راجع نتيجة الاكتمال الفعلية وارجع إلى أي مجال يحتاج إلى معالجة.", "bi-clipboard-check", "/GeneralSettings/Setup", PermissionList.SystemSetupPermissions.View, 60, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var settingExists = await dbContext.CompanySettings.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId, cancellationToken);
        var defaultCurrencyExists = await dbContext.Currencies.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsDefault && !x.IsDeleted, cancellationToken);

        var complete = settingExists && defaultCurrencyExists;
        return
        [
            new SetupReadinessResult(
                "company-defaults",
                complete,
                complete ? "Company defaults and a default currency are configured." : "Save system defaults and select a default currency.",
                complete ? "تم ضبط إعدادات الشركة والعملة الافتراضية." : "احفظ إعدادات النظام وحدد العملة الافتراضية.")
        ];
    }
}
