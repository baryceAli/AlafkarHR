using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceLocationPing : Entity<Guid>
{
    public Guid? ClientPingId { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double? AccuracyMeters { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }
    public bool IsIdle { get; private set; }

    private AttendanceLocationPing() { }

    public static AttendanceLocationPing Create(
        Guid id,
        Guid? clientPingId,
        Guid sessionId,
        Guid employeeId,
        double latitude,
        double longitude,
        double? accuracyMeters,
        DateTime recordedAtUtc,
        bool isIdle)
    {
        return new AttendanceLocationPing
        {
            Id = id,
            ClientPingId = clientPingId,
            SessionId = sessionId,
            EmployeeId = employeeId,
            Latitude = latitude,
            Longitude = longitude,
            AccuracyMeters = accuracyMeters,
            RecordedAtUtc = DateTime.SpecifyKind(recordedAtUtc, DateTimeKind.Utc),
            IsIdle = isIdle,
            CreatedAt = DateTime.UtcNow
        };
    }
}
