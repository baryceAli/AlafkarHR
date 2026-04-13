using Shared.DDD;

namespace PerformanceManagement.Performances;

public class EmployeeCompetencyScore : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompetencyId { get; private set; }

    public decimal Score { get; private set; } // 1–5 or 1–10

    public Guid PerformanceCycleId { get; private set; }

    public decimal Weight { get; private set; }
}