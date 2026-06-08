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

        if (employee.AttendanceType == EmployeeAttendanceType.FixedLocation)
        {
            await ValidateFixedLocationAsync(request.Session, employee.DepartmentId, cancellationToken);
        }

        var session = AttendanceSession.Start(
            Guid.NewGuid(),
            employee.EmployeeId,
            employee.CompanyId,
            employee.AttendanceType,
            request.Session.ShiftStart,
            request.Session.ShiftEnd);

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
}
