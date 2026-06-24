using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Performance.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Performance.Services;

public class PerformanceService : BaseApiService, IPerformanceService
{
    private readonly string _path;

    public PerformanceService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/performance";
    }

    public async Task<ApiResult<List<AppraisalCycleDto>>> GetCyclesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/cycles/company/{companyId}");
        return await SendAsync<List<AppraisalCycleDto>>(request, "cycleList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> CreateCycleAsync(UpsertAppraisalCycleDto cycle)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/cycles") { Content = JsonContent.Create(cycle) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpdateCycleAsync(UpsertAppraisalCycleDto cycle)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/cycles/{cycle.Id}") { Content = JsonContent.Create(cycle) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public Task<ApiResult<PerformanceActionResultDto>> StartCycleAsync(Guid id) => PostActionAsync($"cycles/{id}/start");
    public Task<ApiResult<PerformanceActionResultDto>> CloseCycleAsync(Guid id) => PostActionAsync($"cycles/{id}/close");
    public Task<ApiResult<PerformanceActionResultDto>> CancelCycleAsync(Guid id) => PostActionAsync($"cycles/{id}/cancel");

    public async Task<ApiResult<List<GoalDefinitionDto>>> GetGoalDefinitionsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/goals/company/{companyId}");
        return await SendAsync<List<GoalDefinitionDto>>(request, "goalList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> CreateGoalDefinitionAsync(UpsertGoalDefinitionDto goal)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/goals") { Content = JsonContent.Create(goal) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpdateGoalDefinitionAsync(UpsertGoalDefinitionDto goal)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/goals/{goal.Id}") { Content = JsonContent.Create(goal) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> DeleteGoalDefinitionAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/goals/{id}");
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<CompetencyDto>>> GetCompetenciesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/competencies/company/{companyId}");
        return await SendAsync<List<CompetencyDto>>(request, "competencyList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> CreateCompetencyAsync(UpsertCompetencyDto competency)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/competencies") { Content = JsonContent.Create(competency) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpdateCompetencyAsync(UpsertCompetencyDto competency)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/competencies/{competency.Id}") { Content = JsonContent.Create(competency) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> DeleteCompetencyAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/competencies/{id}");
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeGoalReviewDto>>> GetEmployeeGoalsAsync(Guid employeeId, Guid cycleId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/employee-goals?employeeId={employeeId}&cycleId={cycleId}");
        return await SendAsync<List<EmployeeGoalReviewDto>>(request, "employeeGoalList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpsertEmployeeGoalAsync(UpsertEmployeeGoalDto employeeGoal)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/employee-goals") { Content = JsonContent.Create(employeeGoal) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpdateEmployeeGoalAchievementAsync(UpdateEmployeeGoalAchievementDto achievement)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/employee-goals/achievement") { Content = JsonContent.Create(achievement) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeCompetencyScoreDto>>> GetEmployeeCompetencyScoresAsync(Guid employeeId, Guid cycleId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/competency-scores?employeeId={employeeId}&cycleId={cycleId}");
        return await SendAsync<List<EmployeeCompetencyScoreDto>>(request, "competencyScoreList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> UpsertEmployeeCompetencyScoreAsync(UpsertEmployeeCompetencyScoreDto competencyScore)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/competency-scores") { Content = JsonContent.Create(competencyScore) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeAppraisalDto>>> GetEvaluationsAsync(Guid companyId, Guid? cycleId = null, Guid? employeeId = null)
    {
        var query = new List<string>();
        if (cycleId.HasValue && cycleId.Value != Guid.Empty) query.Add($"cycleId={cycleId.Value}");
        if (employeeId.HasValue && employeeId.Value != Guid.Empty) query.Add($"employeeId={employeeId.Value}");
        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/evaluations/company/{companyId}{suffix}");
        return await SendAsync<List<EmployeeAppraisalDto>>(request, "evaluationList");
    }

    public async Task<ApiResult<PerformanceActionResultDto>> CreateEvaluationAsync(CreateEmployeeAppraisalDto evaluation)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/evaluations") { Content = JsonContent.Create(evaluation) };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }

    public Task<ApiResult<PerformanceActionResultDto>> RecalculateEvaluationAsync(Guid id) => PostActionAsync($"evaluations/{id}/recalculate");
    public Task<ApiResult<PerformanceActionResultDto>> SubmitEvaluationAsync(Guid id, string? employeeFeedback) => PostActionAsync($"evaluations/{id}/submit", new PerformanceActionDto { EmployeeFeedback = employeeFeedback });
    public Task<ApiResult<PerformanceActionResultDto>> ReviewEvaluationAsync(Guid id, string? managerFeedback) => PostActionAsync($"evaluations/{id}/review", new PerformanceActionDto { ManagerFeedback = managerFeedback });
    public Task<ApiResult<PerformanceActionResultDto>> ApproveEvaluationAsync(Guid id, string? managerFeedback) => PostActionAsync($"evaluations/{id}/approve", new PerformanceActionDto { ManagerFeedback = managerFeedback });
    public Task<ApiResult<PerformanceActionResultDto>> CancelEvaluationAsync(Guid id) => PostActionAsync($"evaluations/{id}/cancel");

    private async Task<ApiResult<PerformanceActionResultDto>> PostActionAsync(string relativePath, PerformanceActionDto? body = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{relativePath}")
        {
            Content = JsonContent.Create(body ?? new PerformanceActionDto())
        };
        return await SendAsync<PerformanceActionResultDto>(request, null);
    }
}
