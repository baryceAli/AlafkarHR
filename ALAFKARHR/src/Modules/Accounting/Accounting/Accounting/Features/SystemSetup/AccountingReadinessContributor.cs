using MediatR;
using Shared.Setup;
using Accounting.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Accounting.Features.SystemSetup;

public sealed class AccountingReadinessContributor(ISender sender, AccountingDbContext dbContext) : ISetupCountryPackContributor
{
    public string ModuleKey => "accounting";
    public IReadOnlyCollection<string> ApplicableCountryCodes => ["SA"];
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("compliance", "Compliance", "الامتثال", "Recommended regulatory configuration that does not block core readiness.", "إعدادات تنظيمية موصى بها لا تمنع اكتمال الإعداد الأساسي.", "bi-shield-check", 90, true)
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("accounting-readiness", "core", "Accounting readiness", "الجاهزية المحاسبية", "Complete posting defaults, accounts, tax, journals, and fiscal periods.", "أكمل إعدادات الترحيل والحسابات والضرائب واليوميات والفترات المالية.", "bi-calculator", "/Accounting/Setup", PermissionList.AccountingDashboardPermissions.View, 50, false),
        new("compliance-country", "compliance", "Country applicability", "نطاق التطبيق حسب الدولة", "Resolve supported compliance packs from persisted company configuration.", "حدد حزم الامتثال المدعومة من إعدادات الشركة المحفوظة.", "bi-globe2", "/Accounting/Zatca/Settings", PermissionList.ZatcaSettingsPermissions.View, 890, true),
        new("zatca-compliance", "compliance", "ZATCA seller identity", "هوية بائع زاتكا", "Validate bilingual seller identity, VAT, address, and invoice settings when applicable.", "تحقق من هوية البائع ثنائية اللغة والرقم الضريبي والعنوان وإعدادات الفاتورة عند التطبيق.", "bi-shield-check", "/Accounting/Zatca/Settings", PermissionList.ZatcaSettingsPermissions.View, 900, false, ["compliance-country"]),
        new("zatca-device", "compliance", "ZATCA device readiness", "جاهزية جهاز زاتكا", "Validate an active device and certificate when Saudi e-invoicing is enabled.", "تحقق من جهاز نشط وشهادة عند تفعيل الفوترة الإلكترونية السعودية.", "bi-pc-display", "/Accounting/Zatca/Settings", PermissionList.ZatcaSettingsPermissions.View, 910, false, ["zatca-compliance"]),
        new("zatca-submission", "compliance", "Submission readiness", "جاهزية الإرسال", "Review configuration readiness without requiring historical submissions.", "راجع جاهزية الإعداد دون اشتراط عمليات إرسال سابقة.", "bi-send-check", "/Accounting/Zatca/Submissions", PermissionList.ZatcaEInvoicePermissions.View, 920, true, ["zatca-device"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAccountingSetupStatusQuery(companyId), cancellationToken);
        var status = result.Status;
        var settings = await dbContext.ZatcaSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var applicable = settings is not null && string.Equals(settings.CountryCode, "SA", StringComparison.OrdinalIgnoreCase);
        var sellerReady = applicable
            && !string.IsNullOrWhiteSpace(settings!.SellerName)
            && !string.IsNullOrWhiteSpace(settings.SellerNameAr)
            && !string.IsNullOrWhiteSpace(settings.VatNumber)
            && !string.IsNullOrWhiteSpace(settings.CommercialRegistrationNumber)
            && !string.IsNullOrWhiteSpace(settings.BuildingNumber)
            && !string.IsNullOrWhiteSpace(settings.StreetName)
            && !string.IsNullOrWhiteSpace(settings.District)
            && !string.IsNullOrWhiteSpace(settings.City)
            && !string.IsNullOrWhiteSpace(settings.PostalCode);
        var deviceReady = !applicable || await dbContext.ZatcaDevices.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && x.IsActive
            && x.Csid != null && x.Csid != "" && x.CertificatePem != null && x.CertificatePem != "" && x.PrivateKeyReference != null && x.PrivateKeyReference != "", cancellationToken);

        return
        [
            new SetupReadinessResult(
                "accounting-readiness",
                status.ReadyToPost,
                status.ReadyToPost ? "Accounting is ready to post." : string.Join(", ", status.MissingItems.Where(x => !x.Contains("ZATCA", StringComparison.OrdinalIgnoreCase))),
                status.ReadyToPost ? "المحاسبة جاهزة للترحيل." : "أكمل المتطلبات المحاسبية الأساسية قبل الترحيل."),
            new SetupReadinessResult(
                "zatca-compliance",
                !applicable || sellerReady,
                !applicable ? "Not applicable: no supported Saudi compliance pack is enabled." : sellerReady ? "ZATCA seller identity and address settings are complete." : "Complete the bilingual seller identity, VAT, registration, and national address fields.",
                !applicable ? "غير منطبق: لم يتم تفعيل حزمة امتثال سعودية مدعومة." : sellerReady ? "هوية بائع زاتكا وإعدادات العنوان مكتملة." : "أكمل هوية البائع ثنائية اللغة والرقم الضريبي والسجل والعنوان الوطني.",
                applicable && !sellerReady,
                applicable),
            new SetupReadinessResult("compliance-country", true,
                applicable ? "Saudi Arabia compliance pack is applicable from saved ZATCA country configuration." : "No supported country compliance pack is currently enabled.",
                applicable ? "حزمة امتثال المملكة العربية السعودية منطبقة وفق إعداد دولة زاتكا المحفوظ." : "لا توجد حزمة امتثال مدعومة مفعلة حالياً.", false, applicable),
            new SetupReadinessResult("zatca-device", deviceReady,
                !applicable ? "Not applicable until the Saudi e-invoicing pack is enabled." : deviceReady ? "An active ZATCA device with certificate credentials is ready." : "Add an active device with CSID, certificate, and private-key reference.",
                !applicable ? "غير منطبق حتى يتم تفعيل حزمة الفوترة الإلكترونية السعودية." : deviceReady ? "جهاز زاتكا نشط ببيانات شهادة صالحة جاهز." : "أضف جهازاً نشطاً يحتوي على CSID وشهادة ومرجع مفتاح خاص.",
                applicable && !deviceReady,
                applicable),
            new SetupReadinessResult("zatca-submission", !applicable || sellerReady && deviceReady,
                !applicable ? "Not applicable." : "Configuration readiness is measured without requiring historical submissions or production invoices.",
                !applicable ? "غير منطبق." : "تُقاس جاهزية الإعداد دون اشتراط إرسال سابق أو فواتير إنتاجية.", false, applicable)
        ];
    }
}
