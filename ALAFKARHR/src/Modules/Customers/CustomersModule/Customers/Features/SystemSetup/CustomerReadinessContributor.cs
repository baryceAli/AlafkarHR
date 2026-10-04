using CustomersModule.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Customers.Enums;
using SharedWithUI.Permissions;

namespace CustomersModule.Customers.Features.SystemSetup;

public sealed class CustomerReadinessContributor(CustomerDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "customers";

    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("sales", "Sales", "المبيعات", "Customer, quotation, and sales defaults.", "إعداد العملاء وعروض الأسعار وافتراضات المبيعات.", "bi-receipt", 40, true, null, ["core"])
    ];

    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("customer-foundations", "sales", "Customer foundation", "أساس العملاء", "Create a valid customer group and an active customer.", "أنشئ مجموعة عملاء صالحة وعميلاً نشطاً.", "bi-people", "/Customers/Customer/List", PermissionList.CustomerPermissions.View, 310, false),
        new("customer-pricing", "sales", "Pricing and customer defaults", "التسعير وافتراضات العملاء", "Add optional customer pricing profiles and defaults.", "أضف ملفات تسعير العملاء والافتراضات الاختيارية.", "bi-tags", "/Customers/CustomerPricingProfile/List", PermissionList.CustomerPricingProfilePermissions.View, 320, true, ["customer-foundations"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var groupIds = await dbContext.CustomerGroups.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name != "" && x.NameEng != "")
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var customers = await dbContext.Customers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Status == CustomerStatus.Active)
            .Select(x => new { x.Name, x.CustomerGroupId })
            .ToListAsync(cancellationToken);
        var invalidCustomers = customers.Count(x => string.IsNullOrWhiteSpace(x.Name) || !x.CustomerGroupId.HasValue || !groupIds.Contains(x.CustomerGroupId.Value));
        var complete = groupIds.Count > 0 && customers.Count > 0 && invalidCustomers == 0;

        var today = DateTime.UtcNow.Date;
        var pricingReady = await dbContext.CustomerPricingProfiles.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.PriceListId != Guid.Empty && x.EffectiveFrom <= today && (!x.EffectiveTo.HasValue || x.EffectiveTo >= today), cancellationToken);

        return
        [
            new("customer-foundations", complete,
                complete ? "A customer group and active customers are ready." : $"Create a valid customer group and active customer; {invalidCustomers} customer relationship(s) need attention.",
                complete ? "مجموعة العملاء والعملاء النشطون جاهزون." : $"أنشئ مجموعة عملاء وعميلاً نشطاً صالحين؛ توجد {invalidCustomers} علاقة عميل تحتاج إلى مراجعة.",
                invalidCustomers > 0),
            new("customer-pricing", pricingReady,
                pricingReady ? "An active customer pricing profile is available." : "Customer pricing profiles are optional.",
                pricingReady ? "يتوفر ملف تسعير عملاء ساري." : "ملفات تسعير العملاء اختيارية.")
        ];
    }
}
