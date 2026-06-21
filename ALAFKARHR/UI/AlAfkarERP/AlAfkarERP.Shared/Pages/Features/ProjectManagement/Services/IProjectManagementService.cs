using AlAfkarERP.Shared.Dtos;
using SharedWithUI.ProjectManagement.Dtos;
using SharedWithUI.ProjectManagement.Enums;

namespace AlAfkarERP.Shared.Pages.Features.ProjectManagement.Services;

public interface IProjectManagementService
{
    Task<ApiResult<ProjectDashboardDto>> GetDashboardAsync(Guid? companyId = null);
    Task<ApiResult<PaginatedResult<ProjectDto>>> GetProjectsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, ProjectStatus? status = null);
    Task<ApiResult<ProjectDto>> GetProjectByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateProjectAsync(ProjectDto project);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateProjectAsync(ProjectDto project);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteProjectAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> ChangeProjectStatusAsync(Guid id, ProjectStatus status);
    Task<ApiResult<CreateResponseDto>> CreateCustomerAsync(Guid projectId, ProjectCustomerDto customer);
    Task<ApiResult<CreateResponseDto>> CreateCustomerProductPlanAsync(Guid projectId, Guid projectCustomerId, ProjectCustomerProductPlanDto productPlan);
    Task<ApiResult<CreateResponseDto>> CreateDeliverableAsync(Guid projectId, ProjectDeliverableDto deliverable);
    Task<ApiResult<List<ProjectMaterialRequirementDto>>> GenerateMaterialRequirementsAsync(Guid projectId, Guid deliverableId);
    Task<ApiResult<List<ProjectMaterialRequirementDto>>> GetMaterialRequirementsAsync(Guid projectId);
    Task<ApiResult<PaginatedResult<DistributionPlaceDto>>> GetPlacesAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null);
    Task<ApiResult<CreateResponseDto>> CreatePlaceAsync(DistributionPlaceDto place);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdatePlaceAsync(DistributionPlaceDto place);
    Task<ApiResult<CreateResponseDto>> CreateScheduleAsync(Guid projectId, ProjectDistributionScheduleDto schedule);
    Task<ApiResult<CreateResponseDto>> CreateAllocationAsync(Guid projectId, Guid scheduleId, ProjectDistributionAllocationDto allocation);
    Task<ApiResult<UpdateDeleteResponseDto>> RecordAllocationActualsAsync(Guid allocationId, decimal shippedQuantity, decimal deliveredQuantity, decimal actualQuantity, string? notes);
    Task<ApiResult<CreateResponseDto>> CreateResourceAsync(Guid projectId, ProjectResourceDto resource);
    Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(Guid projectId, ProjectExpenseDto expense);
    Task<ApiResult<CreateResponseDto>> PostHandoffAsync(Guid projectId, ProjectHandoffDto handoff);
    Task<ApiResult<List<ProjectHandoffDto>>> GetHandoffsAsync(Guid projectId);
    Task<ApiResult<CreateResponseDto>> CreateTaskLinkAsync(Guid projectId, ProjectTaskLinkDto taskLink);
    Task<ApiResult<List<ProjectTaskLinkDto>>> GetTaskLinksAsync(Guid projectId);
    Task<ApiResult<ProjectBudgetSummaryDto>> GetBudgetSummaryAsync(Guid projectId);
    Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetCustomerDistributionReportAsync(Guid? companyId, Guid? projectId, Guid? customerId, Guid? placeId, Guid? deliverableId, DateTime? fromDate, DateTime? toDate, ProjectReportGroupBy groupBy);
    Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetPlaceDistributionReportAsync(Guid? companyId, Guid? projectId, Guid? placeId, DateTime? fromDate, DateTime? toDate);
    Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetDailyDistributionReportAsync(Guid? companyId, Guid? projectId, DateTime date);
    Task<ApiResult<List<PlannedProductDemandRowDto>>> GetPlannedProductDemandReportAsync(Guid? companyId, Guid? projectId, Guid? customerId, Guid? productSkuId, DateTime? fromDate, DateTime? toDate, ProjectReportGroupBy groupBy);
}

public class ProjectDashboardDto
{
    public int ActiveProjects { get; set; }
    public int ScheduledDistributions { get; set; }
    public decimal PlannedMeals { get; set; }
    public decimal ActualMeals { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
}
