using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace EmployeeModule.Employees.Features.SystemSetup;

public sealed class EmployeeReadinessContributor(EmployeeDbContext dbContext) : ISetupReadinessContributor
{
    public string ModuleKey => "employee";
    public IReadOnlyCollection<SetupTrackDefinition> Tracks =>
    [
        new("workforce", "Workforce", "القوى العاملة", "Employee, attendance, leave, and payroll foundations.", "أسس الموظفين والحضور والإجازات والرواتب.", "bi-people", 20, true, null, ["core"])
    ];
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("employee-foundations", "workforce", "Employee foundations", "أسس الموظفين", "Create positions and place active employees in the organization structure.", "أنشئ الوظائف واربط الموظفين النشطين بالهيكل التنظيمي.", "bi-person-vcard", "/Employee/Dashboard", PermissionList.EmployeePermissions.View, 110, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var hasPosition = await dbContext.Positions.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);
        var employees = await dbContext.Employees.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .Select(x => new { x.BranchId, x.DepartmentId, x.PositionId })
            .ToListAsync(cancellationToken);
        var invalidPlacements = employees.Count(x => x.BranchId == Guid.Empty || !x.DepartmentId.HasValue || x.PositionId == Guid.Empty);
        var complete = hasPosition && employees.Count > 0 && invalidPlacements == 0;

        return
        [
            new SetupReadinessResult(
                "employee-foundations",
                complete,
                complete ? $"{employees.Count} active employee(s) have valid organization placement." : $"Create a position and an active employee; {invalidPlacements} active employee placement(s) need attention.",
                complete ? $"لدى {employees.Count} موظف نشط ارتباط تنظيمي صالح." : $"أنشئ وظيفة وموظفاً نشطاً؛ توجد {invalidPlacements} ارتباطات تنظيمية تحتاج إلى مراجعة.")
        ];
    }
}
