namespace AttendanceDomain.Attendance.Features.LocationPings;

public record SubmitAttendanceLocationPingBatchCommand(IReadOnlyCollection<AttendanceLocationPingDto> Pings)
    : ICommand<SubmitAttendanceLocationPingBatchResult>;

public record SubmitAttendanceLocationPingBatchResult(int Accepted, int Duplicates, int IdleCount);

public class SubmitAttendanceLocationPingBatchHandler(AttendanceDbContext dbContext)
    : ICommandHandler<SubmitAttendanceLocationPingBatchCommand, SubmitAttendanceLocationPingBatchResult>
{
    public async Task<SubmitAttendanceLocationPingBatchResult> Handle(
        SubmitAttendanceLocationPingBatchCommand request,
        CancellationToken cancellationToken)
    {
        var helper = new SubmitAttendanceLocationPingHandler(dbContext);
        var accepted = 0;
        var duplicates = 0;
        var idle = 0;

        foreach (var ping in request.Pings.OrderBy(x => x.RecordedAtUtc))
        {
            var result = await helper.SavePingAsync(ping, cancellationToken);
            if (result.IsDuplicate)
            {
                duplicates++;
                continue;
            }

            accepted++;
            if (result.IsIdle)
            {
                idle++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitAttendanceLocationPingBatchResult(accepted, duplicates, idle);
    }
}
