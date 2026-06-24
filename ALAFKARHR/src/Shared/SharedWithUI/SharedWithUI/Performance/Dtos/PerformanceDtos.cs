namespace SharedWithUI.Performance.Dtos;

public enum PerformanceWorkflowStatus
{
    Draft,
    InProgress,
    PendingApproval,
    Approved,
    Closed,
    Cancelled
}

public class AppraisalCycleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PerformanceWorkflowStatus Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsClosed { get; set; }
    public bool IsCancelled { get; set; }
    public string? StatusLabel { get; set; }
}

public class AppraisalTemplateDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal GoalWeight { get; set; }
    public decimal CompetencyWeight { get; set; }
    public decimal ManagerFeedbackWeight { get; set; }
}

public class GoalDefinitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Weight { get; set; }
}

public class CompetencyDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
}

public class EmployeeGoalReviewDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid CycleId { get; set; }
    public Guid GoalDefinitionId { get; set; }
    public string Goal { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public decimal? ActualValue { get; set; }
    public int? Rating { get; set; }
    public decimal Weight { get; set; }
    public decimal Score { get; set; }
}

public class EmployeeCompetencyScoreDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid CycleId { get; set; }
    public Guid CompetencyId { get; set; }
    public string? CompetencyName { get; set; }
    public decimal Score { get; set; }
    public decimal Weight { get; set; }
    public decimal WeightedScore { get; set; }
}

public class EmployeeAppraisalDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid CycleId { get; set; }
    public string? CycleName { get; set; }
    public Guid? TemplateId { get; set; }
    public PerformanceWorkflowStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public decimal KpiScore { get; set; }
    public decimal CompetencyScore { get; set; }
    public decimal? FinalScore { get; set; }
    public string? Rating { get; set; }
    public string? ManagerFeedback { get; set; }
    public string? EmployeeFeedback { get; set; }
}

public class UpsertAppraisalCycleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);
}

public class UpsertGoalDefinitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Weight { get; set; }
}

public class UpsertCompetencyDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
}

public class UpsertEmployeeGoalDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CycleId { get; set; }
    public Guid GoalDefinitionId { get; set; }
    public decimal TargetValue { get; set; }
    public decimal Weight { get; set; }
}

public class UpdateEmployeeGoalAchievementDto
{
    public Guid EmployeeGoalId { get; set; }
    public decimal AchievedValue { get; set; }
}

public class UpsertEmployeeCompetencyScoreDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CycleId { get; set; }
    public Guid CompetencyId { get; set; }
    public decimal Score { get; set; }
    public decimal Weight { get; set; }
}

public class CreateEmployeeAppraisalDto
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CycleId { get; set; }
}

public class PerformanceActionDto
{
    public Guid Id { get; set; }
    public string? EmployeeFeedback { get; set; }
    public string? ManagerFeedback { get; set; }
}

public class PerformanceActionResultDto
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
