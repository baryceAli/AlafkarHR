using Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;
using GeneralSettings.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.SystemSetup;

public sealed class PosDefaultsReadinessContributor(GeneralSettingsDbContext dbContext, ISender sender) : ISetupReadinessContributor
{
    public string ModuleKey => "general-settings-pos";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("pos-defaults", "store-front", "POS defaults", "افتراضات نقاط البيع", "Complete currency, location, and default POS customer settings.", "أكمل إعدادات العملة والموقع وعميل نقطة البيع الافتراضي.", "bi-credit-card", "/GeneralSettings/SystemSettings", PermissionList.SystemSettingsPermissions.View, 540, false, ["store-sellable-catalog"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var setting = await dbContext.CompanySettings.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Select(x => new { x.DefaultLocation, x.DefaultPosCustomerId })
            .FirstOrDefaultAsync(cancellationToken);
        var defaultCurrencyExists = await dbContext.Currencies.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsDefault && !x.IsDeleted, cancellationToken);
        var customerValid = false;
        if (setting?.DefaultPosCustomerId is { } customerId && customerId != Guid.Empty)
        {
            var customer = await sender.Send(new GetCustomerSalesEligibilityQuery(customerId, companyId, 0), cancellationToken);
            customerValid = customer.Exists && customer.IsActive;
        }
        var complete = defaultCurrencyExists && !string.IsNullOrWhiteSpace(setting?.DefaultLocation) && customerValid;
        return
        [
            new("pos-defaults", complete,
                complete ? "POS currency, location, and default customer are valid." : "Select a valid active default POS customer and complete currency and location defaults.",
                complete ? "عملة نقطة البيع والموقع والعميل الافتراضي صالحة." : "حدد عميلاً افتراضياً نشطاً وصالحاً لنقطة البيع وأكمل افتراضات العملة والموقع.")
        ];
    }
}
