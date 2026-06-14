using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Attendance.Features;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Organization.Contracts.Departments.Features.GetDepartmentAttendanceLocation;

namespace AttendanceDomain.Attendance.Features.StartSession;

public record StartAttendanceSessionCommand(StartAttendanceSessionDto Session)
    : ICommand<StartAttendanceSessionResult>;

public record StartAttendanceSessionResult(AttendanceSessionDto Session);

public class StartAttendanceSessionHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<StartAttendanceSessionCommand, StartAttendanceSessionResult>
{
    public async Task<StartAttendanceSessionResult> Handle(
        StartAttendanceSessionCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await sender.Send(
            new GetEmployeeAttendanceProfileQuery(request.Session.EmployeeId),
            cancellationToken);

        if (!employee.IsActive)
        {
            throw new BadRequestException("Inactive employees cannot start attendance sessions.");
        }

        AttendanceLocationIntegrity.EnsureTrusted(
            request.Session.IsMockedLocation,
            request.Session.LocationIntegrityNote);

        var shiftWindow = await ResolveShiftWindowAsync(request.Session, employee, cancellationToken);

        var hasActiveSession = await dbContext.AttendanceSessions.AnyAsync(
            x => x.EmployeeId == request.Session.EmployeeId
                && (x.Status == AttendanceSessionStatus.Active || x.Status == AttendanceSessionStatus.OnBreak),
            cancellationToken);

        if (hasActiveSession)
        {
            throw new BadRequestException("Employee already has an active attendance session.");
        }

        await ValidateShiftCheckInWindowAsync(request.Session.EmployeeId, shiftWindow, cancellationToken);

        if (employee.AttendanceType == EmployeeAttendanceType.FixedLocation)
        {
            await ValidateFixedLocationAsync(request.Session, employee, cancellationToken);
        }

        var session = AttendanceSession.Start(
            Guid.NewGuid(),
            employee.EmployeeId,
            employee.CompanyId,
            shiftWindow.ShiftId,
            employee.AttendanceType,
            shiftWindow.ShiftStart,
            shiftWindow.ShiftEnd);

        await dbContext.AttendanceSessions.AddAsync(session, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Session.ManualOverrideReason))
        {
            await dbContext.AttendanceExceptions.AddAsync(
                AttendanceException.Create(
                    Guid.NewGuid(),
                    employee.EmployeeId,
                    session.Id,
                    AttendanceExceptionType.ManualOverride,
                    request.Session.ManualOverrideReason),
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new StartAttendanceSessionResult(session.Adapt<AttendanceSessionDto>());
    }

    private async Task<ShiftWindow> ResolveShiftWindowAsync(
        StartAttendanceSessionDto session,
        GetEmployeeAttendanceProfileResult employee,
        CancellationToken cancellationToken)
    {
        var workDateUtc = session.ShiftStart == default ? DateTime.UtcNow : UtcDateTime.Normalize(session.ShiftStart);
        var assignedShiftId = await ResolveAssignedShiftIdAsync(employee, workDateUtc, cancellationToken);
        var effectiveShiftId = assignedShiftId ?? session.ShiftId;

        if (!effectiveShiftId.HasValue)
        {
            var configuredWindow = await ResolveConfiguredWorkdayWindowAsync(employee.CompanyId, workDateUtc, cancellationToken);
            if (configuredWindow is not null)
            {
                return configuredWindow;
            }

            return new ShiftWindow(
                null,
                UtcDateTime.Normalize(session.ShiftStart),
                UtcDateTime.Normalize(session.ShiftEnd),
                null,
                null);
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == effectiveShiftId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", effectiveShiftId.Value);

        var shiftStart = shift.BuildShiftStart(workDateUtc);
        var shiftEnd = shift.BuildShiftEnd(workDateUtc);

        return new ShiftWindow(
            shift.Id,
            shiftStart,
            shiftEnd,
            shift.LateAfter(shiftStart),
            shift.ProhibitCheckInAfter(shiftStart));
    }

    private async Task<ShiftWindow?> ResolveConfiguredWorkdayWindowAsync(
        Guid companyId,
        DateTime workDateUtc,
        CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        var configurationDto = configuration?.ToDto() ?? AttendanceConfiguration.DefaultDto(companyId);
        var schedule = configurationDto.DaySchedules.First(x => x.DayOfWeek == workDateUtc.DayOfWeek);
        if (!schedule.IsWorkingDay || configurationDto.WeekendDays.Contains(workDateUtc.DayOfWeek))
        {
            throw new BadRequestException("Attendance cannot be started on a configured non-working day.");
        }

        if (!schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
        {
            return null;
        }

        var workDate = UtcDateTime.Normalize(workDateUtc).Date;
        return new ShiftWindow(
            null,
            workDate.Add(schedule.StartTime.Value),
            workDate.Add(schedule.EndTime.Value),
            null,
            null);
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

    private async Task ValidateShiftCheckInWindowAsync(
        Guid employeeId,
        ShiftWindow shiftWindow,
        CancellationToken cancellationToken)
    {
        if (!shiftWindow.LateAfterUtc.HasValue || !shiftWindow.ProhibitCheckInAfterUtc.HasValue)
        {
            return;
        }

        var now = DateTime.UtcNow;
        if (now > shiftWindow.ProhibitCheckInAfterUtc.Value)
        {
            throw new BadRequestException("Check-in is prohibited because the employee is too late. Submit a late check-in request for admin review.");
        }

        if (now > shiftWindow.LateAfterUtc.Value)
        {
            await dbContext.AttendanceExceptions.AddAsync(
                AttendanceException.Create(
                    Guid.NewGuid(),
                    employeeId,
                    null,
                    AttendanceExceptionType.Late,
                    "Employee checked in after the allowed late threshold."),
                cancellationToken);
        }
    }

    private async Task ValidateFixedLocationAsync(
        StartAttendanceSessionDto session,
        GetEmployeeAttendanceProfileResult employee,
        CancellationToken cancellationToken)
    {
        if (!employee.DepartmentId.HasValue)
        {
            throw new BadRequestException("Fixed-location employees must be assigned to a department.");
        }

        if (!session.Latitude.HasValue || !session.Longitude.HasValue)
        {
            throw new BadRequestException("Latitude and longitude are required for fixed-location attendance.");
        }

        var department = await sender.Send(
            new GetDepartmentAttendanceLocationQuery(employee.DepartmentId.Value),
            cancellationToken);

        if (!department.IsActive)
        {
            throw new BadRequestException("The assigned department is inactive.");
        }

        var distanceMeters = AttendanceGeo.DistanceMeters(
            session.Latitude.Value,
            session.Longitude.Value,
            department.Latitude,
            department.Longitude);
        var allowedRadiusMeters = employee.AllowedRadiusMeters ?? department.AllowedRadiusMeters;

        if (distanceMeters <= allowedRadiusMeters)
        {
            return;
        }

        await dbContext.AttendanceExceptions.AddAsync(
            AttendanceException.Create(
                Guid.NewGuid(),
                session.EmployeeId,
                null,
                AttendanceExceptionType.OutsideZone,
                $"Submitted location is {distanceMeters:N0} meters from department '{department.Name}'. Allowed radius is {allowedRadiusMeters} meters."),
            cancellationToken);

        if (string.IsNullOrWhiteSpace(session.ManualOverrideReason))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("Employee is outside the assigned department geofence.");
        }
    }

    private sealed record ShiftWindow(
        Guid? ShiftId,
        DateTime ShiftStart,
        DateTime ShiftEnd,
        DateTime? LateAfterUtc,
        DateTime? ProhibitCheckInAfterUtc);

    private sealed record ShiftAssignmentCandidate(
        Guid ShiftId,
        ShiftAssignmentScope Scope,
        DateTime EffectiveFrom);
}
