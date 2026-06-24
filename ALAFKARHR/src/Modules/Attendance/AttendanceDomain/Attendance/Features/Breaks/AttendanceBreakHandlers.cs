using AttendanceDomain.Attendance.Models;

namespace AttendanceDomain.Attendance.Features.Breaks;

public record StartAttendanceBreakCommand(Guid SessionId) : ICommand<AttendanceBreakResult>;
public record EndAttendanceBreakCommand(Guid SessionId) : ICommand<AttendanceBreakResult>;
public record AttendanceBreakResult(AttendanceSessionDto Session);

public class StartAttendanceBreakHandler(AttendanceDbContext dbContext)
    : ICommandHandler<StartAttendanceBreakCommand, AttendanceBreakResult>
{
    public async Task<AttendanceBreakResult> Handle(StartAttendanceBreakCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.SessionId);

        await ValidateShiftBreakRulesAsync(session, cancellationToken);

        session.StartBreak();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AttendanceBreakResult(session.Adapt<AttendanceSessionDto>());
    }

    private async Task ValidateShiftBreakRulesAsync(AttendanceSession session, CancellationToken cancellationToken)
    {
        if (!session.ShiftId.HasValue)
        {
            throw new BadRequestException("Break cannot be started because this attendance session is not linked to a shift.");
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == session.ShiftId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", session.ShiftId.Value);

        if (shift.BreakMinutes <= 0)
        {
            throw new BadRequestException("Break is disabled for the assigned shift.");
        }

        if (shift.BreakMode != AttendanceBreakMode.Strict)
        {
            return;
        }

        if (!shift.BreakStartTime.HasValue || !shift.BreakEndTime.HasValue)
        {
            throw new BadRequestException("Strict break mode requires shift break start and end times.");
        }

        var now = DateTime.UtcNow.TimeOfDay;
        if (now < shift.BreakStartTime.Value || now > shift.BreakEndTime.Value)
        {
            throw new BadRequestException("Break can only be started during the assigned shift break time.");
        }
    }
}

public class EndAttendanceBreakHandler(AttendanceDbContext dbContext)
    : ICommandHandler<EndAttendanceBreakCommand, AttendanceBreakResult>
{
    public async Task<AttendanceBreakResult> Handle(EndAttendanceBreakCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.SessionId);

        session.EndBreak();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AttendanceBreakResult(session.Adapt<AttendanceSessionDto>());
    }
}
