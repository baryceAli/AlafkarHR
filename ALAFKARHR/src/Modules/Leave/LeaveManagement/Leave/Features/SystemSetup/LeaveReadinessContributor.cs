using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using LeaveManagement.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Attendance.Enums;
using SharedWithUI.Permissions;

namespace LeaveManagement.Leave.Features.SystemSetup;

public sealed class LeaveReadinessContributor(LeaveDbContext dbContext, ISender sender) : ISetupReadinessContributor
{
    public string ModuleKey => "leave";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("leave-foundations", "workforce", "Leave policies", "سياسات الإجازات", "Configure leave types, the current period, policies, and employee coverage.", "اضبط أنواع الإجازات والفترة الحالية والسياسات وتغطية الموظفين.", "bi-calendar-heart", "/HR/LeavePolicies", PermissionList.LeavePolicyPermissions.View, 130, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var employees = (await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(companyId), cancellationToken)).Employees
            .Where(x => x.IsActive)
            .ToList();
        var hasType = await dbContext.LeaveTypes.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted, cancellationToken);
        var hasPeriod = await dbContext.LeavePeriods.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsClosed && !x.IsDeleted && x.StartDate <= today && x.EndDate >= today, cancellationToken);
        var validPolicyIds = await dbContext.LeavePolicies.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.Lines.Any())
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var assignments = await dbContext.LeavePolicyAssignments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && validPolicyIds.Contains(x.PolicyId) && x.EffectiveFrom <= today && (!x.EffectiveTo.HasValue || x.EffectiveTo >= today))
            .Select(x => new { x.Target, x.EmployeeId, x.DepartmentId })
            .ToListAsync(cancellationToken);
        var uncovered = employees.Count(employee => !assignments.Any(assignment => assignment.Target switch
        {
            LeavePolicyAssignmentTarget.Employee => assignment.EmployeeId == employee.EmployeeId,
            LeavePolicyAssignmentTarget.Department => assignment.DepartmentId == employee.DepartmentId,
            _ => false
        }));
        var complete = hasType && hasPeriod && validPolicyIds.Count > 0 && employees.Count > 0 && uncovered == 0;

        return
        [
            new SetupReadinessResult(
                "leave-foundations",
                complete,
                complete ? "Leave types, current period, policies, and assignments are ready." : $"Complete leave types, the current period, a policy with lines, and coverage for {uncovered} active employee(s).",
                complete ? "أنواع الإجازات والفترة الحالية والسياسات والتعيينات جاهزة." : $"أكمل أنواع الإجازات والفترة الحالية وسياسة ببنود وتغطية {uncovered} موظفاً نشطاً.")
        ];
    }
}
