using Shared.DDD;

namespace PerformanceManagement.Performances;


public class GoalDefinition : Aggregate<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    public decimal Weight { get; private set; } // %

    public Guid CompanyId { get; private set; }

    private GoalDefinition() { }

    public static GoalDefinition Create(Guid id, Guid companyId, string name, string code, decimal weight, string createdBy)
    {
        return new GoalDefinition
        {
            Id = id,
            CompanyId = companyId,
            Name = name,
            Code = code,
            Weight = weight,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string code, decimal weight, string modifiedBy)
    {
        Name = name;
        Code = code;
        Weight = weight;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
