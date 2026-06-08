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

        session.StartBreak();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AttendanceBreakResult(session.Adapt<AttendanceSessionDto>());
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
