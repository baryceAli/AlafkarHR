using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Attendance.Features;

namespace AttendanceDomain.Attendance.Features.LocationPings;

public record SubmitAttendanceLocationPingCommand(AttendanceLocationPingDto Ping)
    : ICommand<SubmitAttendanceLocationPingResult>;

public record SubmitAttendanceLocationPingResult(bool IsSuccess, bool IsDuplicate, bool IsIdle);

public class SubmitAttendanceLocationPingHandler(AttendanceDbContext dbContext)
    : ICommandHandler<SubmitAttendanceLocationPingCommand, SubmitAttendanceLocationPingResult>
{
    public async Task<SubmitAttendanceLocationPingResult> Handle(
        SubmitAttendanceLocationPingCommand request,
        CancellationToken cancellationToken)
    {
        var result = await SavePingAsync(request.Ping, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    internal async Task<SubmitAttendanceLocationPingResult> SavePingAsync(
        AttendanceLocationPingDto pingDto,
        CancellationToken cancellationToken)
    {
        AttendanceLocationIntegrity.EnsureTrusted(
            pingDto.IsMockedLocation,
            pingDto.LocationIntegrityNote);

        if (pingDto.ClientPingId.HasValue)
        {
            var exists = await dbContext.AttendanceLocationPings.AnyAsync(
                x => x.ClientPingId == pingDto.ClientPingId,
                cancellationToken);

            if (exists)
            {
                return new SubmitAttendanceLocationPingResult(true, true, false);
            }
        }

        var session = await dbContext.AttendanceSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == pingDto.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", pingDto.SessionId);

        if (session.EmployeeId != pingDto.EmployeeId)
        {
            throw new BadRequestException("Location ping employee does not match the attendance session.");
        }

        if (session.Status != AttendanceSessionStatus.Active)
        {
            throw new BadRequestException("Location pings are accepted only while the session is active.");
        }

        var isIdle = await DetectIdleAsync(pingDto, cancellationToken);

        await dbContext.AttendanceLocationPings.AddAsync(
            AttendanceLocationPing.Create(
                Guid.NewGuid(),
                pingDto.ClientPingId,
                pingDto.SessionId,
                pingDto.EmployeeId,
                pingDto.Latitude,
                pingDto.Longitude,
                pingDto.AccuracyMeters,
                pingDto.RecordedAtUtc,
                isIdle),
            cancellationToken);

        return new SubmitAttendanceLocationPingResult(true, false, isIdle);
    }

    private async Task<bool> DetectIdleAsync(AttendanceLocationPingDto pingDto, CancellationToken cancellationToken)
    {
        var cutoff = pingDto.RecordedAtUtc.AddMinutes(-15);
        var recentPings = await dbContext.AttendanceLocationPings
            .AsNoTracking()
            .Where(x => x.SessionId == pingDto.SessionId
                && x.RecordedAtUtc >= cutoff
                && x.RecordedAtUtc <= pingDto.RecordedAtUtc)
            .OrderBy(x => x.RecordedAtUtc)
            .Select(x => new { x.Latitude, x.Longitude, x.RecordedAtUtc })
            .ToListAsync(cancellationToken);

        if (recentPings.Count == 0)
        {
            return false;
        }

        var observedMinutes = (pingDto.RecordedAtUtc - recentPings[0].RecordedAtUtc).TotalMinutes;
        if (observedMinutes < 15)
        {
            return false;
        }

        return recentPings.All(x =>
            AttendanceGeo.DistanceMeters(x.Latitude, x.Longitude, pingDto.Latitude, pingDto.Longitude) <= 50);
    }
}
