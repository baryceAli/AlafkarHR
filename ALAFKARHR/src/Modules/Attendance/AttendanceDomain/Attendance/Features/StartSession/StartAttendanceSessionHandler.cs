using AttendanceDomain.Attendance.Models;
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
        var shiftWindow = await ResolveShiftWindowAsync(request.Session, cancellationToken);

        var employee = await sender.Send(
            new GetEmployeeAttendanceProfileQuery(request.Session.EmployeeId),
            cancellationToken);

        if (!employee.IsActive)
        {
            throw new BadRequestException("Inactive employees cannot start attendance sessions.");
        }

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
            await ValidateFixedLocationAsync(request.Session, employee.DepartmentId, cancellationToken);
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

    private async Task<ShiftWindow> ResolveShiftWindowAsync(StartAttendanceSessionDto session, CancellationToken cancellationToken)
    {
        if (!session.ShiftId.HasValue)
        {
            return new ShiftWindow(
                null,
                DateTime.SpecifyKind(session.ShiftStart, DateTimeKind.Utc),
                DateTime.SpecifyKind(session.ShiftEnd, DateTimeKind.Utc),
                null,
                null);
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == session.ShiftId.Value, cancellationToken)
            ?? throw new NotFoundException("Shift", session.ShiftId.Value);

        var workDateUtc = session.ShiftStart == default ? DateTime.UtcNow : session.ShiftStart;
        var shiftStart = shift.BuildShiftStart(workDateUtc);
        var shiftEnd = shift.BuildShiftEnd(workDateUtc);

        return new ShiftWindow(
            shift.Id,
            shiftStart,
            shiftEnd,
            shift.LateAfter(shiftStart),
            shift.ProhibitCheckInAfter(shiftStart));
    }

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
                    $"Employee checked in after the allowed late threshold. Late after: {shiftWindow.LateAfterUtc.Value:u}."),
                cancellationToken);
        }
    }

    private async Task ValidateFixedLocationAsync(
        StartAttendanceSessionDto session,
        Guid? departmentId,
        CancellationToken cancellationToken)
    {
        if (!departmentId.HasValue)
        {
            throw new BadRequestException("Fixed-location employees must be assigned to a department.");
        }

        if (!session.Latitude.HasValue || !session.Longitude.HasValue)
        {
            throw new BadRequestException("Latitude and longitude are required for fixed-location attendance.");
        }

        var department = await sender.Send(
            new GetDepartmentAttendanceLocationQuery(departmentId.Value),
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

        if (distanceMeters <= department.AllowedRadiusMeters)
        {
            return;
        }

        await dbContext.AttendanceExceptions.AddAsync(
            AttendanceException.Create(
                Guid.NewGuid(),
                session.EmployeeId,
                null,
                AttendanceExceptionType.OutsideZone,
                $"Submitted location is {distanceMeters:N0} meters from department '{department.Name}'. Allowed radius is {department.AllowedRadiusMeters} meters."),
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
}
