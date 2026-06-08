using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceException : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid? SessionId { get; private set; }
    public AttendanceExceptionType ExceptionType { get; private set; }
    public string Reason { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? ManagerNote { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    private AttendanceException() { }

    public static AttendanceException Create(
        Guid id,
        Guid employeeId,
        Guid? sessionId,
        AttendanceExceptionType exceptionType,
        string reason)
    {
        return new AttendanceException
        {
            Id = id,
            EmployeeId = employeeId,
            SessionId = sessionId,
            ExceptionType = exceptionType,
            Reason = reason,
            Status = AttendanceExceptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
