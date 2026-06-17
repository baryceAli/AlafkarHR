using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;

namespace AttendanceDomain.Attendance.Features.SessionNormalization;

public record EndMissingCheckInAttendanceSessionCommand(EndMissingCheckInAttendanceSessionDto Session)
    : ICommand<EndMissingCheckInAttendanceSessionResult>;

public record EndMissingCheckInAttendanceSessionResult(AttendanceSessionDto Session);

public record NormalizeAttendanceSessionCommand(NormalizeAttendanceSessionDto Session, string NormalizedBy)
    : ICommand<NormalizeAttendanceSessionResult>;

public record NormalizeAttendanceSessionResult(AttendanceSessionDto Session);

public class EndMissingCheckInAttendanceSessionValidator : AbstractValidator<EndMissingCheckInAttendanceSessionCommand>
{
    public EndMissingCheckInAttendanceSessionValidator()
    {
        RuleFor(x => x.Session.EmployeeId).NotEmpty();
        RuleFor(x => x.Session.ShiftStart).NotEmpty();
        RuleFor(x => x.Session.ShiftEnd).GreaterThan(x => x.Session.ShiftStart);
    }
}

public class NormalizeAttendanceSessionValidator : AbstractValidator<NormalizeAttendanceSessionCommand>
{
    public NormalizeAttendanceSessionValidator()
    {
        RuleFor(x => x.Session.SessionId).NotEmpty();
        RuleFor(x => x.NormalizedBy).NotEmpty();
        RuleFor(x => x.Session.ManagerNote).MaximumLength(1000);
    }
}

public class EndMissingCheckInAttendanceSessionHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<EndMissingCheckInAttendanceSessionCommand, EndMissingCheckInAttendanceSessionResult>
{
    public async Task<EndMissingCheckInAttendanceSessionResult> Handle(
        EndMissingCheckInAttendanceSessionCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await sender.Send(
            new GetEmployeeAttendanceProfileQuery(request.Session.EmployeeId),
            cancellationToken);

        if (!employee.IsActive)
        {
            throw new BadRequestException("Inactive employees cannot submit attendance checkout.");
        }

        var shiftStart = UtcDateTime.Normalize(request.Session.ShiftStart);
        var shiftEnd = UtcDateTime.Normalize(request.Session.ShiftEnd);
        var workdayStartUtc = shiftStart.Date;
        var workdayEndUtc = workdayStartUtc.AddDays(1);

        if (await AttendanceSessionLifecycle.AutoCloseStaleSessionsAsync(
            dbContext,
            employee.EmployeeId,
            workdayStartUtc,
            cancellationToken))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var sameDaySessions = await dbContext.AttendanceSessions
            .Where(x => x.EmployeeId == employee.EmployeeId
                && x.ShiftStart < workdayEndUtc
                && x.ShiftEnd > workdayStartUtc)
            .ToListAsync(cancellationToken);

        if (sameDaySessions.Any(x => x.Status is AttendanceSessionStatus.Active or AttendanceSessionStatus.OnBreak))
        {
            throw new BadRequestException("Employee already has an active attendance session.");
        }

        if (sameDaySessions.Any(x => x.Status == AttendanceSessionStatus.Completed))
        {
            throw new BadRequestException("Attendance for this workday is already completed.");
        }

        var session = AttendanceSession.CompleteMissingCheckIn(
            Guid.NewGuid(),
            employee.EmployeeId,
            employee.CompanyId,
            request.Session.ShiftId,
            employee.AttendanceType,
            shiftStart,
            shiftEnd,
            DateTime.UtcNow);

        await dbContext.AttendanceSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new EndMissingCheckInAttendanceSessionResult(session.Adapt<AttendanceSessionDto>());
    }
}

public class NormalizeAttendanceSessionHandler(AttendanceDbContext dbContext)
    : ICommandHandler<NormalizeAttendanceSessionCommand, NormalizeAttendanceSessionResult>
{
    public async Task<NormalizeAttendanceSessionResult> Handle(
        NormalizeAttendanceSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x => x.Id == request.Session.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.Session.SessionId);

        session.Normalize(
            request.Session.CheckInTimeUtc,
            request.Session.CheckOutTimeUtc,
            request.Session.MarkAbsent,
            request.Session.ManagerNote,
            request.NormalizedBy);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new NormalizeAttendanceSessionResult(session.Adapt<AttendanceSessionDto>());
    }
}
