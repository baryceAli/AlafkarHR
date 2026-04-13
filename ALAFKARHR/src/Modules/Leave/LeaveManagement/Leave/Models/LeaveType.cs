using Shared.DDD;



namespace LeaveManagement.Leave.Models;

public class LeaveType : Aggregate<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    public int DefaultDaysPerYear { get; private set; }

    public bool IsPaid { get; private set; }
    public bool RequiresApproval { get; private set; }

    public bool AllowCarryForward { get; private set; }
    public int MaxCarryForwardDays { get; private set; }

    public Guid CompanyId { get; private set; }

    private LeaveType() { }

    public static LeaveType Create(
        Guid id,
        string name,
        string code,
        int defaultDays,
        bool isPaid,
        bool requiresApproval,
        bool allowCarryForward,
        int maxCarryForward,
        Guid companyId)
    {
        return new LeaveType
        {
            Id = id,
            Name = name,
            Code = code,
            DefaultDaysPerYear = defaultDays,
            IsPaid = isPaid,
            RequiresApproval = requiresApproval,
            AllowCarryForward = allowCarryForward,
            MaxCarryForwardDays = maxCarryForward,
            CompanyId = companyId
        };
    }
}