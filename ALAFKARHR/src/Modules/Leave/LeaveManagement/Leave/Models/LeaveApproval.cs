using Shared.DDD;

namespace LeaveManagement.Leave.Models;


public class LeaveApproval : Entity<Guid>
{
    public Guid LeaveRequestId { get; private set; }
    public Guid ApproverId { get; private set; }

    public ApprovalStatus Status { get; private set; }
    public string? Remarks { get; private set; }

    public DateTime ActionDate { get; private set; }

    private LeaveApproval() { }

    public static LeaveApproval Approve(Guid requestId, Guid approverId)
    {
        return new LeaveApproval
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = requestId,
            ApproverId = approverId,
            Status = ApprovalStatus.Approved,
            ActionDate = DateTime.UtcNow
        };
    }

    public static LeaveApproval Reject(Guid requestId, Guid approverId, string reason)
    {
        return new LeaveApproval
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = requestId,
            ApproverId = approverId,
            Status = ApprovalStatus.Rejected,
            Remarks = reason,
            ActionDate = DateTime.UtcNow
        };
    }
}

public enum ApprovalStatus
{
    Approved,
    Rejected
}