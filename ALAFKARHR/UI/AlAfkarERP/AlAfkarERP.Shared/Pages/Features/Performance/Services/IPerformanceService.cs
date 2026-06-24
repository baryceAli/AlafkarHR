using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Performance.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Performance.Services;

public interface IPerformanceService
{
    Task<ApiResult<List<AppraisalCycleDto>>> GetCyclesAsync(Guid companyId);
    Task<ApiResult<PerformanceActionResultDto>> CreateCycleAsync(UpsertAppraisalCycleDto cycle);
    Task<ApiResult<PerformanceActionResultDto>> UpdateCycleAsync(UpsertAppraisalCycleDto cycle);
    Task<ApiResult<PerformanceActionResultDto>> StartCycleAsync(Guid id);
    Task<ApiResult<PerformanceActionResultDto>> CloseCycleAsync(Guid id);
    Task<ApiResult<PerformanceActionResultDto>> CancelCycleAsync(Guid id);

    Task<ApiResult<List<GoalDefinitionDto>>> GetGoalDefinitionsAsync(Guid companyId);
    Task<ApiResult<PerformanceActionResultDto>> CreateGoalDefinitionAsync(UpsertGoalDefinitionDto goal);
    Task<ApiResult<PerformanceActionResultDto>> UpdateGoalDefinitionAsync(UpsertGoalDefinitionDto goal);
    Task<ApiResult<PerformanceActionResultDto>> DeleteGoalDefinitionAsync(Guid id);

    Task<ApiResult<List<CompetencyDto>>> GetCompetenciesAsync(Guid companyId);
    Task<ApiResult<PerformanceActionResultDto>> CreateCompetencyAsync(UpsertCompetencyDto competency);
    Task<ApiResult<PerformanceActionResultDto>> UpdateCompetencyAsync(UpsertCompetencyDto competency);
    Task<ApiResult<PerformanceActionResultDto>> DeleteCompetencyAsync(Guid id);

    Task<ApiResult<List<EmployeeGoalReviewDto>>> GetEmployeeGoalsAsync(Guid employeeId, Guid cycleId);
    Task<ApiResult<PerformanceActionResultDto>> UpsertEmployeeGoalAsync(UpsertEmployeeGoalDto employeeGoal);
    Task<ApiResult<PerformanceActionResultDto>> UpdateEmployeeGoalAchievementAsync(UpdateEmployeeGoalAchievementDto achievement);

    Task<ApiResult<List<EmployeeCompetencyScoreDto>>> GetEmployeeCompetencyScoresAsync(Guid employeeId, Guid cycleId);
    Task<ApiResult<PerformanceActionResultDto>> UpsertEmployeeCompetencyScoreAsync(UpsertEmployeeCompetencyScoreDto competencyScore);

    Task<ApiResult<List<EmployeeAppraisalDto>>> GetEvaluationsAsync(Guid companyId, Guid? cycleId = null, Guid? employeeId = null);
    Task<ApiResult<PerformanceActionResultDto>> CreateEvaluationAsync(CreateEmployeeAppraisalDto evaluation);
    Task<ApiResult<PerformanceActionResultDto>> RecalculateEvaluationAsync(Guid id);
    Task<ApiResult<PerformanceActionResultDto>> SubmitEvaluationAsync(Guid id, string? employeeFeedback);
    Task<ApiResult<PerformanceActionResultDto>> ReviewEvaluationAsync(Guid id, string? managerFeedback);
    Task<ApiResult<PerformanceActionResultDto>> ApproveEvaluationAsync(Guid id, string? managerFeedback);
    Task<ApiResult<PerformanceActionResultDto>> CancelEvaluationAsync(Guid id);
}
