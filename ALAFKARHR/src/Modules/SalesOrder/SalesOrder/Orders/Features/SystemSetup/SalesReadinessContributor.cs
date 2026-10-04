using Microsoft.EntityFrameworkCore;
using SalesOrder.Data;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace SalesOrder.Orders.Features.SystemSetup;

public sealed class SalesReadinessContributor(SalesOrderDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "sales-order";

    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("quotation-defaults", "sales", "Quotation defaults", "افتراضات عروض الأسعار", "Create an active quotation template with valid optional SKU lines.", "أنشئ قالب عرض أسعار نشطاً ببنود أصناف صالحة عند إضافتها.", "bi-file-earmark-text", "/Sales/QuotationTemplates", PermissionList.SalesQuotationPermissions.View, 315, false, ["customer-foundations"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var templates = await dbContext.SalesQuotationTemplates.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.IsActive)
            .ToListAsync(cancellationToken);
        var invalidLines = templates.SelectMany(x => x.Lines)
            .Count(x => x.ProductId == Guid.Empty || x.ProductSkuId == Guid.Empty || x.UnitOfMeasureId == Guid.Empty || string.IsNullOrWhiteSpace(x.SkuCode));
        var complete = templates.Count > 0 && invalidLines == 0;
        return
        [
            new("quotation-defaults", complete,
                complete ? "An active quotation template is ready." : $"Create an active quotation template; {invalidLines} template line(s) need attention.",
                complete ? "قالب عرض أسعار نشط جاهز." : $"أنشئ قالب عرض أسعار نشطاً؛ توجد {invalidLines} بنود قالب تحتاج إلى مراجعة.",
                invalidLines > 0)
        ];
    }
}
