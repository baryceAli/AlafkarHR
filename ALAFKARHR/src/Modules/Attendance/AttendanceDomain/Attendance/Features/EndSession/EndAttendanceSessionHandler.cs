namespace AttendanceDomain.Attendance.Features.EndSession;

public record EndAttendanceSessionCommand(EndAttendanceSessionDto Session)
    : ICommand<EndAttendanceSessionResult>;

public record EndAttendanceSessionResult(AttendanceSessionDto Session);

public class EndAttendanceSessionHandler(AttendanceDbContext dbContext)
    : ICommandHandler<EndAttendanceSessionCommand, EndAttendanceSessionResult>
{
    public async Task<EndAttendanceSessionResult> Handle(
        EndAttendanceSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x => x.Id == request.Session.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.Session.SessionId);

        var pings = await dbContext.AttendanceLocationPings
            .AsNoTracking()
            .Where(x => x.SessionId == session.Id)
            .OrderBy(x => x.RecordedAtUtc)
            .Select(x => new { x.Latitude, x.Longitude })
            .ToListAsync(cancellationToken);

        double totalMeters = 0;
        for (var i = 1; i < pings.Count; i++)
        {
            totalMeters += AttendanceGeo.DistanceMeters(
                pings[i - 1].Latitude,
                pings[i - 1].Longitude,
                pings[i].Latitude,
                pings[i].Longitude);
        }

        session.End((decimal)(totalMeters / 1000));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new EndAttendanceSessionResult(session.Adapt<AttendanceSessionDto>());
    }
}
