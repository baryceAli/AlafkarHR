using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class EmergencyLeaveRequest : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Reason { get; private set; }
    public string? AttachmentPath { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? ApproverUserId { get; private set; }
    public DateTime? ApprovalDateUtc { get; private set; }
    public string? ApproverComment { get; private set; }

    private EmergencyLeaveRequest() { }

    public static EmergencyLeaveRequest Create(Guid id, CreateEmergencyLeaveRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new BadRequestException("Reason is required for emergency leave.");
        }

        if (UtcDateTime.Normalize(dto.EndDate).Date < UtcDateTime.Normalize(dto.StartDate).Date)
        {
            throw new BadRequestException("Emergency leave end date must be on or after start date.");
        }

        return new EmergencyLeaveRequest
        {
            Id = id,
            EmployeeId = dto.EmployeeId,
            CompanyId = dto.CompanyId,
            StartDate = UtcDateTime.Normalize(dto.StartDate).Date,
            EndDate = UtcDateTime.Normalize(dto.EndDate).Date,
            Reason = dto.Reason.Trim(),
            AttachmentPath = string.IsNullOrWhiteSpace(dto.AttachmentPath) ? null : dto.AttachmentPath.Trim(),
            Status = AttendanceExceptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(string approverUserId, string? comment)
    {
        EnsurePending();
        Status = AttendanceExceptionStatus.Approved;
        ApproverUserId = approverUserId;
        ApprovalDateUtc = DateTime.UtcNow;
        ApproverComment = comment;
        ModifiedBy = approverUserId;
        ModifiedAt = ApprovalDateUtc;
    }

    public void Reject(string approverUserId, string? comment)
    {
        EnsurePending();
        Status = AttendanceExceptionStatus.Rejected;
        ApproverUserId = approverUserId;
        ApprovalDateUtc = DateTime.UtcNow;
        ApproverComment = comment;
        ModifiedBy = approverUserId;
        ModifiedAt = ApprovalDateUtc;
    }

    private void EnsurePending()
    {
        if (Status != AttendanceExceptionStatus.Pending)
        {
            throw new BadRequestException("Emergency leave request has already been reviewed.");
        }
    }
}
