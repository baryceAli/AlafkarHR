using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.ProjectManagement.Dtos;
using SharedWithUI.ProjectManagement.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.ProjectManagement.Services;

public class ProjectManagementService : BaseApiService, IProjectManagementService
{
    private readonly string path;

    public ProjectManagementService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/projectmanagement";
    }

    public Task<ApiResult<ProjectDashboardDto>> GetDashboardAsync(Guid? companyId = null)
    {
        var url = $"{path}/dashboard";
        if (companyId.HasValue) url += $"?companyId={companyId}";
        return SendAsync<ProjectDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public Task<ApiResult<PaginatedResult<ProjectDto>>> GetProjectsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, ProjectStatus? status = null)
    {
        var url = $"{path}/projects?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (status.HasValue) url += $"&status={status}";
        return SendAsync<PaginatedResult<ProjectDto>>(new HttpRequestMessage(HttpMethod.Get, url), "projects");
    }

    public Task<ApiResult<ProjectDto>> GetProjectByIdAsync(Guid id)
        => SendAsync<ProjectDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{id}"), "project");

    public Task<ApiResult<CreateResponseDto>> CreateProjectAsync(ProjectDto project)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects") { Content = JsonContent.Create(new { Project = project }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateProjectAsync(ProjectDto project)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/projects/{project.Id}") { Content = JsonContent.Create(new { Project = project }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteProjectAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/projects/{id}"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> ChangeProjectStatusAsync(Guid id, ProjectStatus status)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/projects/{id}/status") { Content = JsonContent.Create(new { Status = status }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateCustomerAsync(Guid projectId, ProjectCustomerDto customer)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/customers") { Content = JsonContent.Create(new { Customer = customer }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateCustomerProductPlanAsync(Guid projectId, Guid projectCustomerId, ProjectCustomerProductPlanDto productPlan)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/customers/{projectCustomerId}/product-plans") { Content = JsonContent.Create(new { ProductPlan = productPlan }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateDeliverableAsync(Guid projectId, ProjectDeliverableDto deliverable)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/deliverables") { Content = JsonContent.Create(new { Deliverable = deliverable }) }, null);

    public Task<ApiResult<List<ProjectMaterialRequirementDto>>> GenerateMaterialRequirementsAsync(Guid projectId, Guid deliverableId)
        => SendAsync<List<ProjectMaterialRequirementDto>>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/deliverables/{deliverableId}/generate-material-requirements"), "requirements");

    public Task<ApiResult<List<ProjectMaterialRequirementDto>>> GetMaterialRequirementsAsync(Guid projectId)
        => SendAsync<List<ProjectMaterialRequirementDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{projectId}/material-requirements"), "requirements");

    public Task<ApiResult<PaginatedResult<DistributionPlaceDto>>> GetPlacesAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null)
    {
        var url = $"{path}/distribution-places?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        return SendAsync<PaginatedResult<DistributionPlaceDto>>(new HttpRequestMessage(HttpMethod.Get, url), "places");
    }

    public Task<ApiResult<CreateResponseDto>> CreatePlaceAsync(DistributionPlaceDto place)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/distribution-places") { Content = JsonContent.Create(new { Place = place }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdatePlaceAsync(DistributionPlaceDto place)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/distribution-places/{place.Id}") { Content = JsonContent.Create(new { Place = place }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateScheduleAsync(Guid projectId, ProjectDistributionScheduleDto schedule)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/distribution-schedules") { Content = JsonContent.Create(new { Schedule = schedule }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateAllocationAsync(Guid projectId, Guid scheduleId, ProjectDistributionAllocationDto allocation)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/distribution-schedules/{scheduleId}/allocations") { Content = JsonContent.Create(new { Allocation = allocation }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> RecordAllocationActualsAsync(Guid allocationId, decimal shippedQuantity, decimal deliveredQuantity, decimal actualQuantity, string? notes)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/distribution-allocations/{allocationId}/actuals") { Content = JsonContent.Create(new { ShippedQuantity = shippedQuantity, DeliveredQuantity = deliveredQuantity, ActualQuantity = actualQuantity, Notes = notes }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateResourceAsync(Guid projectId, ProjectResourceDto resource)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/resources") { Content = JsonContent.Create(new { Resource = resource }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(Guid projectId, ProjectExpenseDto expense)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/expenses") { Content = JsonContent.Create(new { Expense = expense }) }, null);

    public Task<ApiResult<CreateResponseDto>> PostHandoffAsync(Guid projectId, ProjectHandoffDto handoff)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/handoffs") { Content = JsonContent.Create(new { Handoff = handoff }) }, null);

    public Task<ApiResult<List<ProjectHandoffDto>>> GetHandoffsAsync(Guid projectId)
        => SendAsync<List<ProjectHandoffDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{projectId}/handoffs"), "handoffs");

    public Task<ApiResult<CreateResponseDto>> CreateTaskLinkAsync(Guid projectId, ProjectTaskLinkDto taskLink)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/{projectId}/task-links") { Content = JsonContent.Create(new { TaskLink = taskLink }) }, null);

    public Task<ApiResult<List<ProjectTaskLinkDto>>> GetTaskLinksAsync(Guid projectId)
        => SendAsync<List<ProjectTaskLinkDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{projectId}/task-links"), "taskLinks");

    public Task<ApiResult<ProjectBudgetSummaryDto>> GetBudgetSummaryAsync(Guid projectId)
        => SendAsync<ProjectBudgetSummaryDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{projectId}/budget-summary"), "budget");

    public Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetCustomerDistributionReportAsync(Guid? companyId, Guid? projectId, Guid? customerId, Guid? placeId, Guid? deliverableId, DateTime? fromDate, DateTime? toDate, ProjectReportGroupBy groupBy)
    {
        var url = $"{path}/reports/customer-distribution?groupBy={groupBy}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (projectId.HasValue) url += $"&projectId={projectId}";
        if (customerId.HasValue) url += $"&customerId={customerId}";
        if (placeId.HasValue) url += $"&placeId={placeId}";
        if (deliverableId.HasValue) url += $"&deliverableId={deliverableId}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return SendAsync<List<ProjectDistributionReportRowDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }

    public Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetPlaceDistributionReportAsync(Guid? companyId, Guid? projectId, Guid? placeId, DateTime? fromDate, DateTime? toDate)
        => GetCustomerDistributionReportAsync(companyId, projectId, null, placeId, null, fromDate, toDate, ProjectReportGroupBy.Day);

    public Task<ApiResult<List<ProjectDistributionReportRowDto>>> GetDailyDistributionReportAsync(Guid? companyId, Guid? projectId, DateTime date)
        => GetCustomerDistributionReportAsync(companyId, projectId, null, null, null, date.Date, date.Date, ProjectReportGroupBy.Day);

    public Task<ApiResult<List<PlannedProductDemandRowDto>>> GetPlannedProductDemandReportAsync(Guid? companyId, Guid? projectId, Guid? customerId, Guid? productSkuId, DateTime? fromDate, DateTime? toDate, ProjectReportGroupBy groupBy)
    {
        var url = $"{path}/reports/planned-product-demand?groupBy={groupBy}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (projectId.HasValue) url += $"&projectId={projectId}";
        if (customerId.HasValue) url += $"&customerId={customerId}";
        if (productSkuId.HasValue) url += $"&productSkuId={productSkuId}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return SendAsync<List<PlannedProductDemandRowDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }
}
