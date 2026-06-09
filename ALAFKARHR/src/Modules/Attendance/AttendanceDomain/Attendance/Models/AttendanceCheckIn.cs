using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceCheckIn : Entity<Guid>
{
    public Guid? ClientCheckInId { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string SiteName { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime ArrivedAtUtc { get; private set; }
    public DateTime? DepartedAtUtc { get; private set; }
    public string? Notes { get; private set; }

    private AttendanceCheckIn() { }

    public static AttendanceCheckIn Create(
        Guid id,
        Guid? clientCheckInId,
        Guid sessionId,
        Guid employeeId,
        string siteName,
        double latitude,
        double longitude,
        DateTime arrivedAtUtc,
        DateTime? departedAtUtc,
        string? notes)
    {
        return new AttendanceCheckIn
        {
            Id = id,
            ClientCheckInId = clientCheckInId,
            SessionId = sessionId,
            EmployeeId = employeeId,
            SiteName = siteName,
            Latitude = latitude,
            Longitude = longitude,
            ArrivedAtUtc = UtcDateTime.Normalize(arrivedAtUtc),
            DepartedAtUtc = UtcDateTime.Normalize(departedAtUtc),
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }
}
