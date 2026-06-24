using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Data;
using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using LeaveManagement.Data;
using SharedWithUI.Attendance.Dtos;

namespace LeaveManagement.Leave.Features.RosterControl;

public record GetAttendanceRosterControlQuery(AttendanceRosterControlFilterDto Filter)
    : IQuery<GetAttendanceRosterControlResult>;

public record GetAttendanceRosterControlResult(AttendanceRosterControlDto Roster);

public class GetAttendanceRosterControlHandler(
    LeaveDbContext leaveDbContext,
    AttendanceDbContext attendanceDbContext,
    ISender sender)
    : IQueryHandler<GetAttendanceRosterControlQuery, GetAttendanceRosterControlResult>
{
    public async Task<GetAttendanceRosterControlResult> Handle(
        GetAttendanceRosterControlQuery request,
        CancellationToken cancellationToken)
    {
        var filter = NormalizeFilter(request.Filter);
        var employeeResult = await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(filter.CompanyId), cancellationToken);
        var employees = employeeResult.Employees
            .Where(x => !filter.DepartmentId.HasValue || x.DepartmentId == filter.DepartmentId.Value)
            .ToList();

        var employeeIds = employees.Select(x => x.EmployeeId).ToHashSet();
        var shifts = await attendanceDbContext.Shifts.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId && !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var explicitAssignments = await attendanceDbContext.ShiftScheduleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId
                && x.WorkDate >= filter.FromDate
                && x.WorkDate <= filter.ToDate
                && !x.IsDeleted
                && employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);

        var baseAssignments = await attendanceDbContext.EmployeeShifts.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId
                && x.IsActive
                && !x.IsDeleted
                && x.EffectiveFrom.Date <= filter.ToDate
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= filter.FromDate)
                && (!filter.ShiftId.HasValue || x.ShiftId == filter.ShiftId.Value))
            .ToListAsync(cancellationToken);

        var sessions = await attendanceDbContext.AttendanceSessions.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId
                && x.ShiftStart.Date >= filter.FromDate
                && x.ShiftStart.Date <= filter.ToDate
                && employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);

        var leaveApplications = await leaveDbContext.LeaveApplications.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId
                && x.Status == LeaveApplicationStatus.Approved
                && x.StartDate.Date <= filter.ToDate
                && x.EndDate.Date >= filter.FromDate
                && employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);

        var leaveTypeIds = leaveApplications.Select(x => x.LeaveTypeId).Distinct().ToList();
        var leaveTypes = await leaveDbContext.LeaveTypes.AsNoTracking()
            .Where(x => leaveTypeIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var emergencyLeaves = await leaveDbContext.EmergencyLeaveRequests.AsNoTracking()
            .Where(x => x.CompanyId == filter.CompanyId
                && x.Status == AttendanceExceptionStatus.Approved
                && x.StartDate.Date <= filter.ToDate
                && x.EndDate.Date >= filter.FromDate
                && employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);

        var rows = new List<AttendanceRosterControlRowDto>();
        for (var date = filter.FromDate; date <= filter.ToDate; date = date.AddDays(1))
        {
            foreach (var employee in employees)
            {
                var plannedShiftId = ResolvePlannedShiftId(employee, date, explicitAssignments, baseAssignments);
                if (!plannedShiftId.HasValue || !shifts.TryGetValue(plannedShiftId.Value, out var shift))
                {
                    continue;
                }

                if (filter.ShiftId.HasValue && plannedShiftId.Value != filter.ShiftId.Value)
                {
                    continue;
                }

                var session = sessions
                    .Where(x => x.EmployeeId == employee.EmployeeId && x.ShiftStart.Date == date)
                    .OrderByDescending(x => x.ActualStartTime ?? x.ShiftStart)
                    .FirstOrDefault();
                var leave = ResolveApprovedLeave(employee.EmployeeId, date, leaveApplications, leaveTypes, emergencyLeaves);
                var status = ResolveStatus(session, leave);
                var row = new AttendanceRosterControlRowDto
                {
                    WorkDate = date,
                    EmployeeId = employee.EmployeeId,
                    EmployeeNo = employee.EmployeeNo,
                    EmployeeCode = employee.Code,
                    EmployeeName = employee.FullName,
                    EmployeeNameEng = employee.FullNameEng,
                    BranchId = employee.BranchId,
                    AdministrationId = employee.AdministrationId,
                    DepartmentId = employee.DepartmentId,
                    PositionId = employee.PositionId,
                    PositionName = employee.PositionName,
                    PositionNameEng = employee.PositionNameEng,
                    ShiftId = shift.Id,
                    ShiftName = shift.Name,
                    PlannedShiftStartUtc = shift.BuildShiftStart(date),
                    PlannedShiftEndUtc = shift.BuildShiftEnd(date),
                    Status = status,
                    AttendanceSessionId = session?.Id,
                    ActualStartUtc = session?.ActualStartTime,
                    ActualEndUtc = session?.ActualEndTime,
                    SessionStatus = session?.Status,
                    LeaveTypeName = leave?.LeaveTypeName,
                    LeaveTypeNameEng = leave?.LeaveTypeNameEng,
                    AbsenceReason = ResolveAbsenceReason(status, leave)
                };

                rows.Add(row);
            }
        }

        rows = rows
            .Where(x => !filter.Status.HasValue || x.Status == filter.Status.Value)
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.DepartmentName)
            .ThenBy(x => x.EmployeeNameEng ?? x.EmployeeName)
            .ToList();

        AddSubstituteCandidates(rows, employees, leaveApplications, emergencyLeaves);

        return new GetAttendanceRosterControlResult(new AttendanceRosterControlDto
        {
            Summary = BuildSummary(rows),
            Rows = rows
        });
    }

    private static AttendanceRosterControlFilterDto NormalizeFilter(AttendanceRosterControlFilterDto filter)
    {
        var from = DateTime.SpecifyKind(filter.FromDate.Date, DateTimeKind.Utc);
        var to = DateTime.SpecifyKind(filter.ToDate.Date, DateTimeKind.Utc);
        if (to < from)
        {
            throw new BadRequestException("Roster end date must be on or after start date.");
        }

        if ((to - from).TotalDays > 31)
        {
            throw new BadRequestException("Roster control range cannot exceed 31 days.");
        }

        return new AttendanceRosterControlFilterDto
        {
            CompanyId = filter.CompanyId,
            FromDate = from,
            ToDate = to,
            DepartmentId = filter.DepartmentId,
            ShiftId = filter.ShiftId,
            Status = filter.Status
        };
    }

    private static Guid? ResolvePlannedShiftId(
        EmployeeRosterProfileDto employee,
        DateTime date,
        List<ShiftScheduleAssignment> explicitAssignments,
        List<EmployeeShift> baseAssignments)
    {
        var explicitAssignment = explicitAssignments
            .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.WorkDate == date);
        if (explicitAssignment is not null)
        {
            return explicitAssignment.ShiftId;
        }

        return baseAssignments
            .Where(x => x.EffectiveFrom.Date <= date && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= date))
            .Where(x =>
                (x.Scope == ShiftAssignmentScope.Employee && x.EmployeeId == employee.EmployeeId)
                || (x.Scope == ShiftAssignmentScope.Department && employee.DepartmentId.HasValue && x.DepartmentId == employee.DepartmentId.Value)
                || (x.Scope == ShiftAssignmentScope.Administration && x.AdministrationId == employee.AdministrationId)
                || (x.Scope == ShiftAssignmentScope.Company && x.CompanyId == employee.CompanyId))
            .OrderByDescending(x => ScopePriority(x.Scope))
            .ThenByDescending(x => x.EffectiveFrom)
            .Select(x => (Guid?)x.ShiftId)
            .FirstOrDefault();
    }

    private static int ScopePriority(ShiftAssignmentScope scope) => scope switch
    {
        ShiftAssignmentScope.Employee => 4,
        ShiftAssignmentScope.Department => 3,
        ShiftAssignmentScope.Administration => 2,
        _ => 1
    };

    private static AttendanceRosterControlStatus ResolveStatus(AttendanceSession? session, ApprovedLeaveInfo? leave)
    {
        if (session?.Status == AttendanceSessionStatus.Active) return AttendanceRosterControlStatus.Active;
        if (session?.Status == AttendanceSessionStatus.OnBreak) return AttendanceRosterControlStatus.OnBreak;
        if (session?.Status == AttendanceSessionStatus.Completed) return AttendanceRosterControlStatus.Completed;
        if (leave is not null) return AttendanceRosterControlStatus.OnApprovedLeave;
        return AttendanceRosterControlStatus.Absent;
    }

    private static ApprovedLeaveInfo? ResolveApprovedLeave(
        Guid employeeId,
        DateTime date,
        List<LeaveManagement.Leave.Models.LeaveApplication> leaveApplications,
        Dictionary<Guid, LeaveManagement.Leave.Models.LeaveType> leaveTypes,
        List<LeaveManagement.Leave.Models.EmergencyLeaveRequest> emergencyLeaves)
    {
        var application = leaveApplications.FirstOrDefault(x =>
            x.EmployeeId == employeeId && x.StartDate.Date <= date && x.EndDate.Date >= date);
        if (application is not null)
        {
            leaveTypes.TryGetValue(application.LeaveTypeId, out var leaveType);
            return new ApprovedLeaveInfo(
                leaveType?.Name ?? "Approved leave",
                leaveType?.NameEng ?? "Approved leave",
                application.Reason);
        }

        var emergencyLeave = emergencyLeaves.FirstOrDefault(x =>
            x.EmployeeId == employeeId && x.StartDate.Date <= date && x.EndDate.Date >= date);
        return emergencyLeave is null
            ? null
            : new ApprovedLeaveInfo("Emergency leave", "Emergency leave", emergencyLeave.Reason);
    }

    private static string? ResolveAbsenceReason(AttendanceRosterControlStatus status, ApprovedLeaveInfo? leave)
        => status switch
        {
            AttendanceRosterControlStatus.OnApprovedLeave => leave?.Reason ?? leave?.LeaveTypeName,
            AttendanceRosterControlStatus.Absent => "Unexplained absence",
            _ => null
        };

    private static void AddSubstituteCandidates(
        List<AttendanceRosterControlRowDto> rows,
        List<EmployeeRosterProfileDto> employees,
        List<LeaveManagement.Leave.Models.LeaveApplication> leaveApplications,
        List<LeaveManagement.Leave.Models.EmergencyLeaveRequest> emergencyLeaves)
    {
        foreach (var row in rows.Where(x => x.Status is AttendanceRosterControlStatus.Absent or AttendanceRosterControlStatus.OnApprovedLeave))
        {
            var busyEmployeeIds = rows
                .Where(x => x.WorkDate == row.WorkDate)
                .Select(x => x.EmployeeId)
                .ToHashSet();

            row.SubstituteCandidates = employees
                .Where(x => x.EmployeeId != row.EmployeeId)
                .Where(x => !busyEmployeeIds.Contains(x.EmployeeId))
                .Where(x => !HasApprovedLeave(x.EmployeeId, row.WorkDate, leaveApplications, emergencyLeaves))
                .OrderByDescending(x => row.DepartmentId.HasValue && x.DepartmentId == row.DepartmentId)
                .ThenByDescending(x => row.PositionId.HasValue && x.PositionId == row.PositionId)
                .ThenBy(x => x.FullNameEng ?? x.FullName)
                .Take(5)
                .Select(x => new AttendanceRosterSubstituteCandidateDto
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeNo = x.EmployeeNo,
                    EmployeeCode = x.Code,
                    EmployeeName = x.FullName,
                    EmployeeNameEng = x.FullNameEng,
                    DepartmentId = x.DepartmentId,
                    PositionId = x.PositionId,
                    PositionName = x.PositionName,
                    PositionNameEng = x.PositionNameEng
                })
                .ToList();
        }
    }

    private static bool HasApprovedLeave(
        Guid employeeId,
        DateTime date,
        List<LeaveManagement.Leave.Models.LeaveApplication> leaveApplications,
        List<LeaveManagement.Leave.Models.EmergencyLeaveRequest> emergencyLeaves)
        => leaveApplications.Any(x => x.EmployeeId == employeeId && x.StartDate.Date <= date && x.EndDate.Date >= date)
            || emergencyLeaves.Any(x => x.EmployeeId == employeeId && x.StartDate.Date <= date && x.EndDate.Date >= date);

    private static AttendanceRosterControlSummaryDto BuildSummary(List<AttendanceRosterControlRowDto> rows)
    {
        var departmentGroups = rows
            .Where(x => x.DepartmentId.HasValue)
            .GroupBy(x => new { x.WorkDate, x.DepartmentId });

        return new AttendanceRosterControlSummaryDto
        {
            Planned = rows.Count,
            Present = rows.Count(x => x.Status is AttendanceRosterControlStatus.Active or AttendanceRosterControlStatus.OnBreak or AttendanceRosterControlStatus.Completed),
            Active = rows.Count(x => x.Status == AttendanceRosterControlStatus.Active),
            OnBreak = rows.Count(x => x.Status == AttendanceRosterControlStatus.OnBreak),
            Completed = rows.Count(x => x.Status == AttendanceRosterControlStatus.Completed),
            Absent = rows.Count(x => x.Status == AttendanceRosterControlStatus.Absent),
            OnApprovedLeave = rows.Count(x => x.Status == AttendanceRosterControlStatus.OnApprovedLeave),
            UncoveredDepartments = departmentGroups.Count(g => g.Any() && g.All(x => x.Status is AttendanceRosterControlStatus.Absent or AttendanceRosterControlStatus.OnApprovedLeave))
        };
    }

    private sealed record ApprovedLeaveInfo(string? LeaveTypeName, string? LeaveTypeNameEng, string? Reason);
}
