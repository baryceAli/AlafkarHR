using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Permissions;
using SharedWithUI.Suppliers.Enums;
using SuppliersModule.Data;

namespace SuppliersModule.Suppliers.Features.SystemSetup;

public sealed class SupplierReadinessContributor(SupplierDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "suppliers";

    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("purchasing", "Purchasing", "المشتريات", "Supplier and procurement controls.", "إعداد الموردين وضوابط المشتريات.", "bi-cart3", 50, true, null, ["core"])
    ];

    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("supplier-foundations", "purchasing", "Supplier foundation", "أساس الموردين", "Create a valid supplier group and an active supplier.", "أنشئ مجموعة موردين صالحة ومورداً نشطاً.", "bi-truck", "/Suppliers/Supplier/List", PermissionList.SupplierPermissions.View, 410, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var groupIds = await dbContext.SupplierGroups.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name != "")
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var suppliers = await dbContext.Suppliers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Status == SupplierStatus.Active)
            .Select(x => new { x.Name, x.SupplierCode, x.SupplierGroupId })
            .ToListAsync(cancellationToken);
        var invalid = suppliers.Count(x => string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.SupplierCode) || !x.SupplierGroupId.HasValue || !groupIds.Contains(x.SupplierGroupId.Value));
        var complete = groupIds.Count > 0 && suppliers.Count > 0 && invalid == 0;

        return
        [
            new("supplier-foundations", complete,
                complete ? "A supplier group and active suppliers are ready." : $"Create a valid supplier group and active supplier; {invalid} supplier relationship(s) need attention.",
                complete ? "مجموعة الموردين والموردون النشطون جاهزون." : $"أنشئ مجموعة موردين ومورداً نشطاً صالحين؛ توجد {invalid} علاقة مورد تحتاج إلى مراجعة.",
                invalid > 0)
        ];
    }
}
