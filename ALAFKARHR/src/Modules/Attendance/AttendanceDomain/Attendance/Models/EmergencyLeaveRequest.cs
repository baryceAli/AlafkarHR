using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class LegacyAttendanceEmergencyLeaveRequest : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Status { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? AttachmentPath { get; private set; }
    public string? ApproverUserId { get; private set; }
    public DateTime? ApprovalDateUtc { get; private set; }
    public string? ApproverComment { get; private set; }
}
