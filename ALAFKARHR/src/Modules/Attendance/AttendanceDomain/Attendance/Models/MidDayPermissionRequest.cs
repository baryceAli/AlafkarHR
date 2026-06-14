using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class MidDayPermissionRequest : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime RequestedStartUtc { get; private set; }
    public DateTime RequestedEndUtc { get; private set; }
    public DateTime? ApprovedStartUtc { get; private set; }
    public DateTime? ApprovedEndUtc { get; private set; }
    public string Reason { get; private set; }
    public string? Notes { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? ApproverUserId { get; private set; }
    public DateTime? ApprovalDateUtc { get; private set; }
    public string? ApproverComment { get; private set; }

    private MidDayPermissionRequest() { }

    public static MidDayPermissionRequest Create(Guid id, CreateMidDayPermissionRequestDto dto)
    {
        ValidateRequestedTimes(dto.RequestedStartUtc, dto.RequestedEndUtc);

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new BadRequestException("Reason is required for mid-day permission.");
        }

        return new MidDayPermissionRequest
        {
            Id = id,
            EmployeeId = dto.EmployeeId,
            CompanyId = dto.CompanyId,
            Date = UtcDateTime.Normalize(dto.Date).Date,
            RequestedStartUtc = UtcDateTime.Normalize(dto.RequestedStartUtc),
            RequestedEndUtc = UtcDateTime.Normalize(dto.RequestedEndUtc),
            Reason = dto.Reason.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            Status = AttendanceExceptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(DateTime approvedStartUtc, DateTime approvedEndUtc, string approverUserId, string? comment)
    {
        EnsurePending();
        ValidateRequestedTimes(approvedStartUtc, approvedEndUtc);

        ApprovedStartUtc = UtcDateTime.Normalize(approvedStartUtc);
        ApprovedEndUtc = UtcDateTime.Normalize(approvedEndUtc);
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
            throw new BadRequestException("Mid-day permission request has already been reviewed.");
        }
    }

    private static void ValidateRequestedTimes(DateTime startUtc, DateTime endUtc)
    {
        if (UtcDateTime.Normalize(endUtc) <= UtcDateTime.Normalize(startUtc))
        {
            throw new BadRequestException("Permission end time must be after start time.");
        }
    }
}
