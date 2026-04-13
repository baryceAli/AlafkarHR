using Shared.DDD;

namespace LeaveManagement.Leave.Models;

public class LeaveBalance : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }

    public int Year { get; private set; }

    public decimal Allocated { get; private set; }
    public decimal Used { get; private set; }
    public decimal Remaining => Allocated - Used;

    public Guid CompanyId { get; private set; }

    private LeaveBalance() { }

    public void Allocate(decimal days)
    {
        Allocated += days;
    }

    public void Use(decimal days)
    {
        if (Remaining < days)
            throw new Exception("Insufficient leave balance");

        Used += days;
    }
}