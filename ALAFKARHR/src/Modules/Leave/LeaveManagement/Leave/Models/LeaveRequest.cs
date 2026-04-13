using Shared.DDD;

namespace LeaveManagement.Leave.Models;


public class LeaveRequest : Aggregate<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public decimal TotalDays { get; private set; }

    public LeaveStatus Status { get; private set; }

    public string? Reason { get; private set; }

    public Guid CompanyId { get; private set; }

    private readonly List<LeaveApproval> _approvals = new();
    public IReadOnlyCollection<LeaveApproval> Approvals => _approvals;

    private LeaveRequest() { }

    public static LeaveRequest Create(
        Guid id,
        Guid employeeId,
        Guid leaveTypeId,
        DateTime start,
        DateTime end,
        string reason,
        Guid companyId)
    {
        var days = (end - start).TotalDays + 1;

        return new LeaveRequest
        {
            Id = id,
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            StartDate = start,
            EndDate = end,
            TotalDays = (decimal)days,
            Status = LeaveStatus.Pending,
            Reason = reason,
            CompanyId = companyId
        };
    }

    public void Approve(Guid approverId)
    {
        Status = LeaveStatus.Approved;

        _approvals.Add(LeaveApproval.Approve(Id, approverId));
    }

    public void Reject(Guid approverId, string reason)
    {
        Status = LeaveStatus.Rejected;

        _approvals.Add(LeaveApproval.Reject(Id, approverId, reason));
    }
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}
