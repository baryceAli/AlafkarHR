using Shared.DDD;

namespace PerformanceManagement.Performances;

public class EmployeeCompetencyScore : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompetencyId { get; private set; }

    public decimal Score { get; private set; } // 1–5 or 1–10

    public Guid PerformanceCycleId { get; private set; }

    public decimal Weight { get; private set; }

    private EmployeeCompetencyScore() { }

    public static EmployeeCompetencyScore Create(Guid id, Guid employeeId, Guid competencyId, Guid cycleId, decimal score, decimal weight)
    {
        return new EmployeeCompetencyScore
        {
            Id = id,
            EmployeeId = employeeId,
            CompetencyId = competencyId,
            PerformanceCycleId = cycleId,
            Score = score,
            Weight = weight
        };
    }

    public void Update(decimal score, decimal weight)
    {
        Score = score;
        Weight = weight;
    }
}
