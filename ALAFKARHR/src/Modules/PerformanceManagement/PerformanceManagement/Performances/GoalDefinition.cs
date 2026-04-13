using Shared.DDD;

namespace PerformanceManagement.Performances;


public class GoalDefinition : Aggregate<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    public decimal Weight { get; private set; } // %

    public Guid CompanyId { get; private set; }

    private GoalDefinition() { }
}