using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceSession : Aggregate<Guid>
{
    public Guid EmployeeId { get; private set; }
    public EmployeeAttendanceType AttendanceType { get; private set; }
    public DateTime ShiftStart { get; private set; }
    public DateTime ShiftEnd { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public AttendanceSessionStatus Status { get; private set; }
    public decimal TotalHours { get; private set; }
    public decimal TotalDistanceKm { get; private set; }
    public Guid CompanyId { get; private set; }

    private AttendanceSession() { }

    public static AttendanceSession Start(
        Guid id,
        Guid employeeId,
        Guid companyId,
        EmployeeAttendanceType attendanceType,
        DateTime shiftStart,
        DateTime shiftEnd)
    {
        var now = DateTime.UtcNow;

        return new AttendanceSession
        {
            Id = id,
            EmployeeId = employeeId,
            CompanyId = companyId,
            AttendanceType = attendanceType,
            ShiftStart = DateTime.SpecifyKind(shiftStart, DateTimeKind.Utc),
            ShiftEnd = DateTime.SpecifyKind(shiftEnd, DateTimeKind.Utc),
            ActualStartTime = now,
            Status = AttendanceSessionStatus.Active,
            CreatedAt = now
        };
    }

    public void StartBreak()
    {
        if (Status != AttendanceSessionStatus.Active)
        {
            throw new BadRequestException("Only an active attendance session can start a break.");
        }

        Status = AttendanceSessionStatus.OnBreak;
        ModifiedAt = DateTime.UtcNow;
    }

    public void EndBreak()
    {
        if (Status != AttendanceSessionStatus.OnBreak)
        {
            throw new BadRequestException("Only an on-break attendance session can end a break.");
        }

        Status = AttendanceSessionStatus.Active;
        ModifiedAt = DateTime.UtcNow;
    }

    public void End(decimal totalDistanceKm)
    {
        if (Status is AttendanceSessionStatus.Completed or AttendanceSessionStatus.Cancelled)
        {
            throw new BadRequestException("Attendance session is already closed.");
        }

        ActualEndTime = DateTime.UtcNow;
        Status = AttendanceSessionStatus.Completed;
        TotalDistanceKm = decimal.Round(totalDistanceKm, 3);
        TotalHours = ActualStartTime.HasValue
            ? decimal.Round((decimal)(ActualEndTime.Value - ActualStartTime.Value).TotalHours, 2)
            : 0;
        ModifiedAt = DateTime.UtcNow;
    }
}
