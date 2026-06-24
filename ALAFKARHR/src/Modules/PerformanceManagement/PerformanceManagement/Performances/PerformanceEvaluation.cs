using PerformanceManagement.Performances.Enums;
using Shared.DDD;

namespace PerformanceManagement.Performances;

public class PerformanceEvaluation : Aggregate<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid PerformanceCycleId { get; private set; }

    public decimal KpiScore { get; private set; }
    public decimal CompetencyScore { get; private set; }

    public decimal FinalScore { get; private set; }

    public RatingLevel Rating { get; private set; }

    public string? ManagerComment { get; private set; }
    public string? EmployeeComment { get; private set; }

    public EvaluationStatus Status { get; private set; }

    public Guid CompanyId { get; private set; }

    private PerformanceEvaluation() { }

    public static PerformanceEvaluation Create(Guid id, Guid companyId, Guid employeeId, Guid performanceCycleId, string createdBy)
    {
        return new PerformanceEvaluation
        {
            Id = id,
            CompanyId = companyId,
            EmployeeId = employeeId,
            PerformanceCycleId = performanceCycleId,
            Status = EvaluationStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Calculate(
        IEnumerable<EmployeeGoal> goals,
        IEnumerable<EmployeeCompetencyScore> competencies)
    {
        KpiScore = goals.Sum(g => g.GetScore());

        CompetencyScore = competencies.Sum(c => c.Score * (c.Weight / 100));

        FinalScore = KpiScore + CompetencyScore;

        Rating = CalculateRating(FinalScore);
        ModifiedAt = DateTime.UtcNow;
    }

    private RatingLevel CalculateRating(decimal score)
    {
        return score switch
        {
            >= 90 => RatingLevel.Excellent,
            >= 75 => RatingLevel.VeryGood,
            >= 60 => RatingLevel.Good,
            >= 50 => RatingLevel.Average,
            _ => RatingLevel.Poor
        };
    }

    public void Submit(string employeeComment, string modifiedBy)
    {
        EmployeeComment = employeeComment;
        Status = EvaluationStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Review(string managerComment, string modifiedBy)
    {
        ManagerComment = managerComment;
        Status = EvaluationStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Approve(string managerComment, string modifiedBy)
    {
        ManagerComment = managerComment;
        Status = EvaluationStatus.Approved;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Cancel(string modifiedBy)
    {
        if (Status == EvaluationStatus.Approved)
            throw new InvalidOperationException("Approved evaluations cannot be cancelled");

        Status = EvaluationStatus.Rejected;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
