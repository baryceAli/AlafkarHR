using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;


public class AttendanceLog : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public AttendanceType Type { get; private set; } // CheckIn / CheckOut

    public string? DeviceId { get; private set; }
    public string? Location { get; private set; }

    public Guid CompanyId { get; private set; }

    private AttendanceLog() { }

    public static AttendanceLog CheckIn(Guid id, Guid employeeId, Guid companyId, string? location = null)
    {
        return new AttendanceLog
        {
            Id = id,
            EmployeeId = employeeId,
            Timestamp = DateTime.UtcNow,
            Type = AttendanceType.CheckIn,
            Location = location,
            CompanyId = companyId
        };
    }

    public static AttendanceLog CheckOut(Guid id, Guid employeeId, Guid companyId, string? location = null)
    {
        return new AttendanceLog
        {
            Id = id,
            EmployeeId = employeeId,
            Timestamp = DateTime.UtcNow,
            Type = AttendanceType.CheckOut,
            Location = location,
            CompanyId = companyId
        };
    }
}

public enum AttendanceType
{
    CheckIn,
    CheckOut
}