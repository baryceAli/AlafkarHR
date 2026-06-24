using Shared.DDD;

namespace PerformanceManagement.Performances;

public class EmployeeGoal : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid GoalDefinitionId { get; private set; }

    public decimal TargetValue { get; private set; }
    public decimal AchievedValue { get; private set; }

    public decimal Weight { get; private set; }

    public Guid PerformanceCycleId { get; private set; }

    private EmployeeGoal() { }

    public static EmployeeGoal Create(Guid id, Guid employeeId, Guid goalDefinitionId, Guid cycleId, decimal targetValue, decimal weight)
    {
        return new EmployeeGoal
        {
            Id = id,
            EmployeeId = employeeId,
            GoalDefinitionId = goalDefinitionId,
            PerformanceCycleId = cycleId,
            TargetValue = targetValue,
            Weight = weight,
            AchievedValue = 0
        };
    }

    public void Update(decimal targetValue, decimal weight)
    {
        TargetValue = targetValue;
        Weight = weight;
    }

    public void UpdateAchievement(decimal value)
    {
        AchievedValue = value;
    }

    public decimal GetScore()
    {
        if (TargetValue == 0) return 0;

        var percentage = (AchievedValue / TargetValue) * 100;
        return percentage * (Weight / 100);
    }
}
