using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Attendance.Enums;
using SharedWithUI.Permissions;

namespace AttendanceDomain.Attendance.Features.SystemSetup;

public sealed class AttendanceReadinessContributor(AttendanceDbContext dbContext, ISender sender) : ISetupReadinessContributor
{
    public string ModuleKey => "attendance";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("attendance-foundations", "workforce", "Attendance setup", "إعداد الحضور", "Configure the calendar, shifts, and employee shift coverage.", "اضبط التقويم والورديات وتغطية ورديات الموظفين.", "bi-calendar2-check", "/Attendance/Shifts", PermissionList.AttendancePermissions.Edit, 120, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var profiles = (await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(companyId), cancellationToken)).Employees
            .Where(x => x.IsActive)
            .ToList();
        var hasConfiguration = await dbContext.AttendanceConfigurations.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);
        var hasShift = await dbContext.Shifts.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);
        var assignments = await dbContext.EmployeeShifts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.EffectiveFrom <= today && (!x.EffectiveTo.HasValue || x.EffectiveTo >= today))
            .Select(x => new { x.Scope, x.EmployeeId, x.DepartmentId, x.AdministrationId })
            .ToListAsync(cancellationToken);
        var scheduledEmployeeIds = await dbContext.ShiftScheduleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.WorkDate >= today && x.WorkDate <= today.AddDays(7))
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var uncovered = profiles.Count(employee => !scheduledEmployeeIds.Contains(employee.EmployeeId)
            && !assignments.Any(assignment => assignment.Scope switch
            {
                ShiftAssignmentScope.Company => true,
                ShiftAssignmentScope.Administration => assignment.AdministrationId == employee.AdministrationId,
                ShiftAssignmentScope.Department => assignment.DepartmentId == employee.DepartmentId,
                ShiftAssignmentScope.Employee => assignment.EmployeeId == employee.EmployeeId,
                _ => false
            }));
        var complete = hasConfiguration && hasShift && profiles.Count > 0 && uncovered == 0;

        return
        [
            new SetupReadinessResult(
                "attendance-foundations",
                complete,
                complete ? "Attendance calendar, shifts, and employee coverage are configured." : $"Save the attendance calendar, create a shift, and cover {uncovered} active employee(s).",
                complete ? "تم إعداد تقويم الحضور والورديات وتغطية الموظفين." : $"احفظ تقويم الحضور وأنشئ وردية وغطِّ {uncovered} موظفاً نشطاً.")
        ];
    }
}
