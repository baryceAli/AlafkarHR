using Shared.DDD;

namespace LeaveManagement.Leave.Models;


public class LeaveTransaction : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }

    public decimal Days { get; private set; }

    public TransactionType Type { get; private set; } // Accrual / Usage / Adjustment

    public DateTime Date { get; private set; }

    public Guid ReferenceId { get; private set; } // LeaveRequestId

    public Guid CompanyId { get; private set; }

    private LeaveTransaction() { }
}

public enum TransactionType
{
    Accrual,
    Usage,
    Adjustment
}