using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Organization.Contracts.Departments.Features.GetDepartmentAttendanceLocation;
using Shared.Pagination;

namespace AttendanceDomain.Attendance.Features.Queries;

public record GetAttendanceDashboardQuery(Guid? EmployeeId) : IQuery<GetAttendanceDashboardResult>;
public record GetAttendanceDashboardResult(AttendanceDashboardDto Dashboard);

public record GetAttendanceSessionsQuery(Guid? EmployeeId, DateTime? FromUtc, DateTime? ToUtc, PaginationRequest PaginationRequest)
    : IQuery<GetAttendanceSessionsResult>;
public record GetAttendanceSessionsResult(PaginatedResult<AttendanceSessionDto> SessionList);

public record GetAttendanceShiftsQuery(Guid? CompanyId) : IQuery<GetAttendanceShiftsResult>;
public record GetAttendanceShiftsResult(List<ShiftDto> ShiftList);

public record GetAttendanceCheckInPreviewQuery(
    Guid EmployeeId,
    double? Latitude,
    double? Longitude,
    double? AccuracyMeters,
    bool IsMockedLocation,
    string? LocationIntegrityNote,
    DateTime? WorkDateUtc) : IQuery<GetAttendanceCheckInPreviewResult>;
public record GetAttendanceCheckInPreviewResult(AttendanceCheckInPreviewDto Preview);

public record GetLateCheckInRequestsQuery(AttendanceExceptionStatus? Status, Guid? EmployeeId, PaginationRequest PaginationRequest)
    : IQuery<GetLateCheckInRequestsResult>;
public record GetLateCheckInRequestsResult(PaginatedResult<LateCheckInRequestDto> RequestList);

public record GetShiftAssignmentsQuery(Guid? CompanyId, ShiftAssignmentScope? Scope, PaginationRequest PaginationRequest)
    : IQuery<GetShiftAssignmentsResult>;
public record GetShiftAssignmentsResult(PaginatedResult<ShiftAssignmentDto> AssignmentList);

public class GetAttendanceDashboardHandler(AttendanceDbContext dbContext, ISender sender)
    : IQueryHandler<GetAttendanceDashboardQuery, GetAttendanceDashboardResult>
{
    public async Task<GetAttendanceDashboardResult> Handle(GetAttendanceDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var sessions = dbContext.AttendanceSessions.AsNoTracking();
        var lateRequests = dbContext.LateCheckInRequests.AsNoTracking();

        if (request.EmployeeId.HasValue)
        {
            sessions = sessions.Where(x => x.EmployeeId == request.EmployeeId.Value);
            lateRequests = lateRequests.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        var recentSessions = await sessions
            .Where(x => x.ShiftStart >= today.AddDays(-7))
            .OrderByDescending(x => x.ActualStartTime ?? x.ShiftStart)
            .Take(8)
            .ProjectToType<AttendanceSessionDto>()
            .ToListAsync(cancellationToken);

        await PopulateEmployeeNamesAsync(recentSessions, cancellationToken);

        var pendingRequests = await lateRequests
            .Where(x => x.Status == AttendanceExceptionStatus.Pending)
            .OrderByDescending(x => x.RequestedCheckInTimeUtc)
            .Take(8)
            .ProjectToType<LateCheckInRequestDto>()
            .ToListAsync(cancellationToken);

        var dashboard = new AttendanceDashboardDto
        {
            ActiveSessions = await sessions.CountAsync(x => x.Status == AttendanceSessionStatus.Active, cancellationToken),
            OnBreakSessions = await sessions.CountAsync(x => x.Status == AttendanceSessionStatus.OnBreak, cancellationToken),
            CompletedToday = await sessions.CountAsync(x => x.Status == AttendanceSessionStatus.Completed && x.ActualEndTime >= today && x.ActualEndTime < tomorrow, cancellationToken),
            PendingLateCheckInRequests = await lateRequests.CountAsync(x => x.Status == AttendanceExceptionStatus.Pending, cancellationToken),
            FixedLocationSessionsToday = await sessions.CountAsync(x => x.ShiftStart >= today && x.ShiftStart < tomorrow && x.AttendanceType == EmployeeAttendanceType.FixedLocation, cancellationToken),
            MobileSessionsToday = await sessions.CountAsync(x => x.ShiftStart >= today && x.ShiftStart < tomorrow && x.AttendanceType == EmployeeAttendanceType.Mobile, cancellationToken),
            RecentSessions = recentSessions,
            PendingRequests = pendingRequests
        };

        return new GetAttendanceDashboardResult(dashboard);
    }

    private async Task PopulateEmployeeNamesAsync(List<AttendanceSessionDto> sessions, CancellationToken cancellationToken)
    {
        var employeeIds = sessions
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToList();

        foreach (var employeeId in employeeIds)
        {
            GetEmployeeAttendanceProfileResult employee;

            try
            {
                employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId), cancellationToken);
            }
            catch (NotFoundException)
            {
                continue;
            }

            var employeeSessions = sessions.Where(x => x.EmployeeId == employeeId);

            foreach (var session in employeeSessions)
            {
                session.EmployeeName = employee.FullName;
            }
        }
    }
}

public class GetAttendanceSessionsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceSessionsQuery, GetAttendanceSessionsResult>
{
    public async Task<GetAttendanceSessionsResult> Handle(GetAttendanceSessionsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AttendanceSessions.AsNoTracking();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        if (request.FromUtc.HasValue)
        {
            var fromUtc = UtcDateTime.Normalize(request.FromUtc.Value);
            query = query.Where(x => x.ShiftStart >= fromUtc);
        }

        if (request.ToUtc.HasValue)
        {
            var toUtc = UtcDateTime.Normalize(request.ToUtc.Value);
            query = query.Where(x => x.ShiftStart <= toUtc);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var sessions = await query
            .OrderByDescending(x => x.ActualStartTime ?? x.ShiftStart)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ProjectToType<AttendanceSessionDto>()
            .ToListAsync(cancellationToken);

        return new GetAttendanceSessionsResult(
            new PaginatedResult<AttendanceSessionDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                total,
                sessions));
    }
}

public class GetAttendanceShiftsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceShiftsQuery, GetAttendanceShiftsResult>
{
    public async Task<GetAttendanceShiftsResult> Handle(GetAttendanceShiftsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Shifts.AsNoTracking().Where(x => !x.IsDeleted);

        if (request.CompanyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == request.CompanyId.Value);
        }

        var shifts = await query
            .OrderBy(x => x.StartTime)
            .ProjectToType<ShiftDto>()
            .ToListAsync(cancellationToken);

        return new GetAttendanceShiftsResult(shifts);
    }
}

public class GetAttendanceCheckInPreviewHandler(AttendanceDbContext dbContext, ISender sender)
    : IQueryHandler<GetAttendanceCheckInPreviewQuery, GetAttendanceCheckInPreviewResult>
{
    public async Task<GetAttendanceCheckInPreviewResult> Handle(
        GetAttendanceCheckInPreviewQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await sender.Send(
            new GetEmployeeAttendanceProfileQuery(request.EmployeeId),
            cancellationToken);

        var now = DateTime.UtcNow;
        var workDateUtc = request.WorkDateUtc.HasValue
            ? UtcDateTime.Normalize(request.WorkDateUtc.Value)
            : now;

        var shiftWindow = await ResolveShiftWindowAsync(employee, workDateUtc, cancellationToken);
        var workDayStartUtc = shiftWindow.ShiftStart?.Date ?? workDateUtc.Date;
        var workDayEndUtc = workDayStartUtc.AddDays(1);

        if (await AttendanceSessionLifecycle.AutoCloseStaleSessionsAsync(
            dbContext,
            request.EmployeeId,
            workDayStartUtc,
            cancellationToken))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var activeSessionCandidates = await dbContext.AttendanceSessions
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId
                && (x.Status == AttendanceSessionStatus.Active || x.Status == AttendanceSessionStatus.OnBreak))
            .OrderByDescending(x => x.ActualStartTime ?? x.ShiftStart)
            .ProjectToType<AttendanceSessionDto>()
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionCandidates
            .FirstOrDefault(x => AttendanceSessionLifecycle.IsCurrentWorkdaySession(x, workDayStartUtc, workDayEndUtc));

        var preview = new AttendanceCheckInPreviewDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.FullName,
            EmployeeCode = employee.Code,
            EmployeeEmail = employee.Email,
            AttendanceType = employee.AttendanceType,
            ShiftId = shiftWindow.ShiftId,
            ShiftName = shiftWindow.ShiftName,
            ShiftStart = shiftWindow.ShiftStart,
            ShiftEnd = shiftWindow.ShiftEnd,
            LateAfterUtc = shiftWindow.LateAfterUtc,
            ProhibitCheckInAfterUtc = shiftWindow.ProhibitCheckInAfterUtc,
            ActiveSession = activeSession,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            AccuracyMeters = request.AccuracyMeters,
            HasLocation = request.Latitude.HasValue && request.Longitude.HasValue,
            IsMockedLocation = request.IsMockedLocation,
            LocationIntegrityNote = request.LocationIntegrityNote
        };

        if (!employee.IsActive)
        {
            preview.Message = "Your employee profile is inactive, so attendance check-in is not available.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (preview.IsMockedLocation)
        {
            preview.Message = $"Attendance location rejected. {AttendanceLocationIntegrity.SuspiciousReason(preview.LocationIntegrityNote)}";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (preview.HasLocation)
        {
            if (employee.AttendanceType == EmployeeAttendanceType.FixedLocation)
            {
                await ApplyFixedLocationPreviewAsync(request, employee, preview, cancellationToken);
            }
            else
            {
                preview.IsWithinAllowedRadius = true;
            }
        }

        if (activeSession is not null)
        {
            preview.Message = activeSession.Status == AttendanceSessionStatus.OnBreak
                ? "You already have an attendance session on break. Resume or end it before starting another session."
                : "You are already checked in. End the active session before starting another one.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (!shiftWindow.ShiftId.HasValue || !shiftWindow.ShiftStart.HasValue || !shiftWindow.ShiftEnd.HasValue)
        {
            preview.Message = "No effective shift was found for you today. Ask the administrator to assign a shift to your employee, department, administration, or company.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        var completedSession = await dbContext.AttendanceSessions
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId
                && x.Status == AttendanceSessionStatus.Completed
                && (
                    (shiftWindow.ShiftId.HasValue
                        && x.ShiftId == shiftWindow.ShiftId.Value
                        && x.ShiftStart >= workDayStartUtc
                        && x.ShiftStart < workDayEndUtc)
                    || (x.ShiftStart < shiftWindow.ShiftEnd.Value
                        && x.ShiftEnd > shiftWindow.ShiftStart.Value)
                    || (x.ActualStartTime.HasValue
                        && x.ActualStartTime.Value >= workDayStartUtc
                        && x.ActualStartTime.Value < workDayEndUtc)
                    || (x.ActualEndTime.HasValue
                        && x.ActualEndTime.Value >= workDayStartUtc
                        && x.ActualEndTime.Value < workDayEndUtc)))
            .OrderByDescending(x => x.ActualEndTime ?? x.ShiftEnd)
            .ProjectToType<AttendanceSessionDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (completedSession is not null)
        {
            preview.IsAttendanceCompleted = true;
            preview.CanSubmitLateRequest = false;
            preview.CanCheckIn = false;
            preview.Message = "Your attendance for this shift is already completed.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (!preview.HasLocation)
        {
            preview.Message = employee.AttendanceType == EmployeeAttendanceType.FixedLocation
                ? "Allow location access so the system can confirm that you are inside your department attendance radius."
                : "Allow location access so the system can record your starting location.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        preview.IsBeforeShiftStart = now < shiftWindow.ShiftStart.Value;
        preview.IsLate = shiftWindow.LateAfterUtc.HasValue && now > shiftWindow.LateAfterUtc.Value;
        preview.IsProhibitedByTime = shiftWindow.ProhibitCheckInAfterUtc.HasValue && now > shiftWindow.ProhibitCheckInAfterUtc.Value;
        preview.CanSubmitLateRequest = preview.IsProhibitedByTime;

        if (preview.IsBeforeShiftStart)
        {
            preview.Message = "Check-in is not open yet. See the effective shift start time on this page.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (preview.IsProhibitedByTime)
        {
            preview.Message = "Check-in is closed because the allowed check-in time has ended. Submit a late check-in request for admin review.";
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        if (!preview.IsWithinAllowedRadius)
        {
            return new GetAttendanceCheckInPreviewResult(preview);
        }

        preview.CanCheckIn = true;
        preview.Message = preview.IsLate && shiftWindow.ProhibitCheckInAfterUtc.HasValue
            ? "You are late, but check-in is still allowed."
            : "You are within the allowed time and location range. You can check in now.";

        return new GetAttendanceCheckInPreviewResult(preview);
    }

    private async Task ApplyFixedLocationPreviewAsync(
        GetAttendanceCheckInPreviewQuery request,
        GetEmployeeAttendanceProfileResult employee,
        AttendanceCheckInPreviewDto preview,
        CancellationToken cancellationToken)
    {
        if (!employee.DepartmentId.HasValue)
        {
            preview.Message = "Fixed-location attendance requires an assigned department with an attendance location.";
            return;
        }

        var department = await sender.Send(
            new GetDepartmentAttendanceLocationQuery(employee.DepartmentId.Value),
            cancellationToken);

        if (!department.IsActive)
        {
            preview.Message = "Your assigned department is inactive, so attendance check-in is not available.";
            return;
        }

        var allowedRadiusMeters = employee.AllowedRadiusMeters ?? department.AllowedRadiusMeters;
        preview.AllowedRadiusMeters = allowedRadiusMeters;
        preview.DistanceMeters = AttendanceGeo.DistanceMeters(
            request.Latitude!.Value,
            request.Longitude!.Value,
            department.Latitude,
            department.Longitude);
        preview.IsWithinAllowedRadius = preview.DistanceMeters <= allowedRadiusMeters;

        if (!preview.IsWithinAllowedRadius)
        {
            preview.Message = $"You are {preview.DistanceMeters:N0} meters away from department '{department.Name}'. The allowed radius is {allowedRadiusMeters:N0} meters.";
        }
    }

    private async Task<ShiftWindow> ResolveShiftWindowAsync(
        GetEmployeeAttendanceProfileResult employee,
        DateTime workDateUtc,
        CancellationToken cancellationToken)
    {
        var assignedShiftId = await ResolveAssignedShiftIdAsync(employee, workDateUtc, cancellationToken);
        if (!assignedShiftId.HasValue)
        {
            return new ShiftWindow(null, null, null, null, null, null);
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == assignedShiftId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", assignedShiftId.Value);

        var shiftStart = shift.BuildShiftStart(workDateUtc);

        return new ShiftWindow(
            shift.Id,
            shift.Name,
            shiftStart,
            shift.BuildShiftEnd(workDateUtc),
            shift.LateAfter(shiftStart),
            shift.ProhibitCheckInAfter(shiftStart));
    }

    private async Task<Guid?> ResolveAssignedShiftIdAsync(
        GetEmployeeAttendanceProfileResult employee,
        DateTime workDateUtc,
        CancellationToken cancellationToken)
    {
        var assignments = await dbContext.EmployeeShifts
            .AsNoTracking()
            .Where(x => x.IsActive
                && !x.IsDeleted
                && x.EffectiveFrom <= workDateUtc
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= workDateUtc)
                && (
                    (x.Scope == ShiftAssignmentScope.Employee && x.EmployeeId == employee.EmployeeId)
                    || (x.Scope == ShiftAssignmentScope.Department && employee.DepartmentId.HasValue && x.DepartmentId == employee.DepartmentId.Value)
                    || (x.Scope == ShiftAssignmentScope.Administration && x.AdministrationId == employee.AdministrationId)
                    || (x.Scope == ShiftAssignmentScope.Company && x.CompanyId == employee.CompanyId)))
            .Select(x => new ShiftAssignmentCandidate(
                x.ShiftId,
                x.Scope,
                x.EffectiveFrom))
            .ToListAsync(cancellationToken);

        return assignments
            .OrderByDescending(x => Priority(x.Scope))
            .ThenByDescending(x => x.EffectiveFrom)
            .Select(x => (Guid?)x.ShiftId)
            .FirstOrDefault();
    }

    private static int Priority(ShiftAssignmentScope scope) => scope switch
    {
        ShiftAssignmentScope.Employee => 4,
        ShiftAssignmentScope.Department => 3,
        ShiftAssignmentScope.Administration => 2,
        ShiftAssignmentScope.Company => 1,
        _ => 0
    };

    private sealed record ShiftWindow(
        Guid? ShiftId,
        string? ShiftName,
        DateTime? ShiftStart,
        DateTime? ShiftEnd,
        DateTime? LateAfterUtc,
        DateTime? ProhibitCheckInAfterUtc);

    private sealed record ShiftAssignmentCandidate(
        Guid ShiftId,
        ShiftAssignmentScope Scope,
        DateTime EffectiveFrom);
}

public class GetLateCheckInRequestsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetLateCheckInRequestsQuery, GetLateCheckInRequestsResult>
{
    public async Task<GetLateCheckInRequestsResult> Handle(GetLateCheckInRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.LateCheckInRequests.AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var requests = await query
            .OrderByDescending(x => x.RequestedCheckInTimeUtc)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ProjectToType<LateCheckInRequestDto>()
            .ToListAsync(cancellationToken);

        return new GetLateCheckInRequestsResult(
            new PaginatedResult<LateCheckInRequestDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                total,
                requests));
    }
}

public class GetShiftAssignmentsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetShiftAssignmentsQuery, GetShiftAssignmentsResult>
{
    public async Task<GetShiftAssignmentsResult> Handle(GetShiftAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmployeeShifts.AsNoTracking().Where(x => !x.IsDeleted);

        if (request.CompanyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == request.CompanyId.Value);
        }

        if (request.Scope.HasValue)
        {
            query = query.Where(x => x.Scope == request.Scope.Value);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var assignments = await query
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.EffectiveFrom)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ProjectToType<ShiftAssignmentDto>()
            .ToListAsync(cancellationToken);

        return new GetShiftAssignmentsResult(
            new PaginatedResult<ShiftAssignmentDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                total,
                assignments));
    }
}
