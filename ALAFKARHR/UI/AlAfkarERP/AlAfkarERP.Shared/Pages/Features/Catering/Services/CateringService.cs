using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.Catering.Dtos;
using SharedWithUI.Catering.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catering.Services;

public class CateringService : BaseApiService, ICateringService
{
    private readonly string path;

    public CateringService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/catering";
    }

    public Task<ApiResult<CateringDashboardDto>> GetDashboardAsync(Guid? companyId = null)
    {
        var url = $"{path}/dashboard";
        if (companyId.HasValue) url += $"?companyId={companyId}";
        return SendAsync<CateringDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public Task<ApiResult<PaginatedResult<MealDefinitionDto>>> GetMealsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true)
        => SendAsync<PaginatedResult<MealDefinitionDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/meals", pageIndex, pageSize, companyId, searchText, $"activeOnly={activeOnly}")), "meals");

    public Task<ApiResult<MealDefinitionDto>> GetMealByIdAsync(Guid id)
        => SendAsync<MealDefinitionDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/meals/{id}"), "meal");

    public Task<ApiResult<CreateResponseDto>> CreateMealAsync(MealDefinitionDto meal)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/meals") { Content = JsonContent.Create(new { Meal = meal }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateMealAsync(MealDefinitionDto meal)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/meals/{meal.Id}") { Content = JsonContent.Create(new { Meal = meal }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteMealAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/meals/{id}"), null);

    public Task<ApiResult<CreateResponseDto>> AddMealComponentAsync(Guid mealId, MealComponentDto component)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/meals/{mealId}/components") { Content = JsonContent.Create(new { Component = component }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteMealComponentAsync(Guid mealId, Guid componentId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/meals/{mealId}/components/{componentId}"), null);

    public Task<ApiResult<PaginatedResult<CateringContractDto>>> GetContractsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, Guid? customerId = null, CateringContractStatus? status = null)
    {
        var extra = new List<string>();
        if (customerId.HasValue) extra.Add($"customerId={customerId}");
        if (status.HasValue) extra.Add($"status={status}");
        return SendAsync<PaginatedResult<CateringContractDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/contracts", pageIndex, pageSize, companyId, searchText, extra.ToArray())), "contracts");
    }

    public Task<ApiResult<CateringContractDto>> GetContractByIdAsync(Guid id)
        => SendAsync<CateringContractDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/contracts/{id}"), "contract");

    public Task<ApiResult<CreateResponseDto>> CreateContractAsync(CateringContractDto contract)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/contracts") { Content = JsonContent.Create(new { Contract = contract }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateContractAsync(CateringContractDto contract)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/contracts/{contract.Id}") { Content = JsonContent.Create(new { Contract = contract }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteContractAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/contracts/{id}"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> CloseContractAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/contracts/{id}/close"), null);

    public Task<ApiResult<CreateResponseDto>> AddAddendumAsync(Guid contractId, CateringContractAddendumDto addendum)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/contracts/{contractId}/addendums") { Content = JsonContent.Create(new { Addendum = addendum }) }, null);

    public Task<ApiResult<PaginatedResult<CateringAreaDto>>> GetAreasAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true)
        => SendAsync<PaginatedResult<CateringAreaDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/areas", pageIndex, pageSize, companyId, searchText, $"activeOnly={activeOnly}")), "areas");

    public Task<ApiResult<CreateResponseDto>> CreateAreaAsync(CateringAreaDto area)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/areas") { Content = JsonContent.Create(new { Area = area }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAreaAsync(CateringAreaDto area)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/areas/{area.Id}") { Content = JsonContent.Create(new { Area = area }) }, null);

    public Task<ApiResult<PaginatedResult<CateringSquareDto>>> GetSquaresAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? areaId = null, string? searchText = null, bool activeOnly = true)
    {
        var extra = new List<string> { $"activeOnly={activeOnly}" };
        if (areaId.HasValue) extra.Add($"areaId={areaId}");
        return SendAsync<PaginatedResult<CateringSquareDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/squares", pageIndex, pageSize, companyId, searchText, extra.ToArray())), "squares");
    }

    public Task<ApiResult<CreateResponseDto>> CreateSquareAsync(CateringSquareDto square)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/squares") { Content = JsonContent.Create(new { Square = square }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateSquareAsync(CateringSquareDto square)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/squares/{square.Id}") { Content = JsonContent.Create(new { Square = square }) }, null);

    public Task<ApiResult<PaginatedResult<CateringDailyScheduleDto>>> GetSchedulesAsync(int pageIndex, int pageSize, Guid? contractId = null, DateTime? fromDate = null, DateTime? toDate = null, Guid? projectId = null, Guid? projectDailyPlanId = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        if (projectId.HasValue) extra.Add($"projectId={projectId}");
        if (projectDailyPlanId.HasValue) extra.Add($"projectDailyPlanId={projectDailyPlanId}");
        if (fromDate.HasValue) extra.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) extra.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        return SendAsync<PaginatedResult<CateringDailyScheduleDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/schedules", pageIndex, pageSize, null, null, extra.ToArray())), "schedules");
    }

    public Task<ApiResult<CreateResponseDto>> CreateScheduleAsync(CateringDailyScheduleDto schedule)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/schedules") { Content = JsonContent.Create(new { Schedule = schedule }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateScheduleAsync(CateringDailyScheduleDto schedule)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/schedules/{schedule.Id}") { Content = JsonContent.Create(new { Schedule = schedule }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> ActivateScheduleAsync(Guid scheduleId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/schedules/{scheduleId}/activate"), null);

    public Task<ApiResult<CreateResponseDto>> CreateAllocationAsync(Guid scheduleId, CateringSquareAllocationDto allocation)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/schedules/{scheduleId}/allocations") { Content = JsonContent.Create(new { Allocation = allocation }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> RecordAllocationActualsAsync(Guid allocationId, decimal receivedQuantity, decimal distributedQuantity, DateTime? actualArrivalTime, Guid? receivingSupervisorEmployeeId, string? receivingSupervisorName, Guid? teamLeaderEmployeeId, string? teamLeaderName, string? varianceNotes)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/allocations/{allocationId}/actuals") { Content = JsonContent.Create(new { ReceivedQuantity = receivedQuantity, DistributedQuantity = distributedQuantity, ActualArrivalTime = actualArrivalTime, ReceivingSupervisorEmployeeId = receivingSupervisorEmployeeId, ReceivingSupervisorName = receivingSupervisorName, TeamLeaderEmployeeId = teamLeaderEmployeeId, TeamLeaderName = teamLeaderName, VarianceNotes = varianceNotes }) }, null);

    public Task<ApiResult<PaginatedResult<CateringPackagingPlanDto>>> GetPackagingPlansAsync(int pageIndex, int pageSize, Guid? scheduleId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var extra = DateQuery(scheduleId, fromDate, toDate);
        return SendAsync<PaginatedResult<CateringPackagingPlanDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/packaging", pageIndex, pageSize, null, null, extra.ToArray())), "packagingPlans");
    }

    public Task<ApiResult<CreateResponseDto>> UpsertPackagingPlanAsync(CateringPackagingPlanDto packagingPlan)
    {
        var method = packagingPlan.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var url = packagingPlan.Id == Guid.Empty ? $"{path}/packaging" : $"{path}/packaging/{packagingPlan.Id}";
        return SendAsync<CreateResponseDto>(new HttpRequestMessage(method, url) { Content = JsonContent.Create(new { PackagingPlan = packagingPlan }) }, null);
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> ReleasePackagingStockAsync(Guid packagingPlanId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/packaging/{packagingPlanId}/release-stock"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> StartPackagingAsync(Guid packagingPlanId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/packaging/{packagingPlanId}/start"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> CompletePackagingAsync(Guid packagingPlanId, decimal preparedMealCount, decimal rejectedMealCount, decimal damagedMealCount, string? varianceReason)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/packaging/{packagingPlanId}/complete") { Content = JsonContent.Create(new { PreparedMealCount = preparedMealCount, RejectedMealCount = rejectedMealCount, DamagedMealCount = damagedMealCount, VarianceReason = varianceReason }) }, null);

    public Task<ApiResult<PaginatedResult<CateringDispatchPlanDto>>> GetDispatchPlansAsync(int pageIndex, int pageSize, Guid? scheduleId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var extra = DateQuery(scheduleId, fromDate, toDate);
        return SendAsync<PaginatedResult<CateringDispatchPlanDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/dispatches", pageIndex, pageSize, null, null, extra.ToArray())), "dispatchPlans");
    }

    public Task<ApiResult<CreateResponseDto>> UpsertDispatchPlanAsync(CateringDispatchPlanDto dispatchPlan)
    {
        var method = dispatchPlan.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var url = dispatchPlan.Id == Guid.Empty ? $"{path}/dispatches" : $"{path}/dispatches/{dispatchPlan.Id}";
        return SendAsync<CreateResponseDto>(new HttpRequestMessage(method, url) { Content = JsonContent.Create(new { DispatchPlan = dispatchPlan }) }, null);
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> CreateDispatchFleetAssignmentAsync(Guid dispatchPlanId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/dispatches/{dispatchPlanId}/fleet-assignment"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> RecordExecutionEventAsync(CateringExecutionEventDto executionEvent)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/execution-events") { Content = JsonContent.Create(new { Event = executionEvent }) }, null);

    public Task<ApiResult<PaginatedResult<CateringVehicleDeliveryDto>>> GetDeliveriesAsync(int pageIndex, int pageSize, Guid? scheduleId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var extra = new List<string>();
        if (scheduleId.HasValue) extra.Add($"scheduleId={scheduleId}");
        if (fromDate.HasValue) extra.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) extra.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        return SendAsync<PaginatedResult<CateringVehicleDeliveryDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/deliveries", pageIndex, pageSize, null, null, extra.ToArray())), "deliveries");
    }

    public Task<ApiResult<CreateResponseDto>> CreateDeliveryAsync(CateringVehicleDeliveryDto delivery)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/deliveries") { Content = JsonContent.Create(new { Delivery = delivery }) }, null);

    public Task<ApiResult<PaginatedResult<CateringAssignmentDto>>> GetAssignmentsAsync(int pageIndex, int pageSize, Guid? contractId = null, CateringAssignmentRole? role = null, Guid? squareId = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        if (role.HasValue) extra.Add($"role={role}");
        if (squareId.HasValue) extra.Add($"squareId={squareId}");
        return SendAsync<PaginatedResult<CateringAssignmentDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/assignments", pageIndex, pageSize, null, null, extra.ToArray())), "assignments");
    }

    public Task<ApiResult<CreateResponseDto>> CreateAssignmentAsync(CateringAssignmentDto assignment)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/assignments") { Content = JsonContent.Create(new { Assignment = assignment }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAssignmentAsync(CateringAssignmentDto assignment)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/assignments/{assignment.Id}") { Content = JsonContent.Create(new { Assignment = assignment }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAssignmentAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/assignments/{id}"), null);

    public Task<ApiResult<PaginatedResult<CateringProjectDto>>> GetProjectsAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? contractId = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        return SendAsync<PaginatedResult<CateringProjectDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/projects", pageIndex, pageSize, companyId, null, extra.ToArray())), "projects");
    }

    public Task<ApiResult<CateringProjectDto>> GetProjectByIdAsync(Guid id)
        => SendAsync<CateringProjectDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/projects/{id}"), "project");

    public Task<ApiResult<CreateResponseDto>> CreateProjectAsync(CateringProjectDto project)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects") { Content = JsonContent.Create(new { Project = project }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateProjectAsync(CateringProjectDto project)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/projects/{project.Id}") { Content = JsonContent.Create(new { Project = project }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteProjectAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/projects/{id}"), null);

    public Task<ApiResult<CreateResponseDto>> UpsertProjectDailyPlanAsync(CateringProjectDailyPlanDto dailyPlan)
    {
        var request = dailyPlan.Id == Guid.Empty
            ? new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/daily-plans")
            : new HttpRequestMessage(HttpMethod.Put, $"{path}/projects/daily-plans/{dailyPlan.Id}");
        request.Content = JsonContent.Create(new { DailyPlan = dailyPlan });
        return SendAsync<CreateResponseDto>(request, null);
    }

    public Task<ApiResult<CateringGenerateDailyPlanResultDto>> GenerateProjectDailyPlansAsync(CateringGenerateProjectDailyPlansRequestDto request)
        => SendAsync<CateringGenerateDailyPlanResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/projects/generate-daily") { Content = JsonContent.Create(new { Request = request }) }, null);

    public Task<ApiResult<PaginatedResult<CateringOperationalPlanDto>>> GetPlansAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? contractId = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        return SendAsync<PaginatedResult<CateringOperationalPlanDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/plans", pageIndex, pageSize, companyId, null, extra.ToArray())), "plans");
    }

    public Task<ApiResult<CreateResponseDto>> CreatePlanAsync(CateringOperationalPlanDto plan)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/plans") { Content = JsonContent.Create(new { Plan = plan }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdatePlanAsync(CateringOperationalPlanDto plan)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/plans/{plan.Id}") { Content = JsonContent.Create(new { Plan = plan }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeletePlanAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/plans/{id}"), null);

    public Task<ApiResult<CreateResponseDto>> AddPlanResourceAsync(Guid planId, CateringPlanResourceAssignmentDto resource)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/plans/{planId}/resources") { Content = JsonContent.Create(new { Resource = resource }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeletePlanResourceAsync(Guid planId, Guid resourceId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/plans/{planId}/resources/{resourceId}"), null);

    public Task<ApiResult<CateringGenerateDailyPlanResultDto>> GenerateDailyPlansAsync(CateringGenerateDailyPlanRequestDto request)
        => SendAsync<CateringGenerateDailyPlanResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/plans/generate-daily") { Content = JsonContent.Create(new { Request = request }) }, null);

    public Task<ApiResult<PaginatedResult<CateringInventoryRequestDto>>> GetInventoryRequestsAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? planId = null, Guid? scheduleId = null, CateringInventoryRequestStatus? status = null)
    {
        var extra = new List<string>();
        if (planId.HasValue) extra.Add($"planId={planId}");
        if (scheduleId.HasValue) extra.Add($"scheduleId={scheduleId}");
        if (status.HasValue) extra.Add($"status={status}");
        return SendAsync<PaginatedResult<CateringInventoryRequestDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/inventory-requests", pageIndex, pageSize, companyId, null, extra.ToArray())), "requests");
    }

    public Task<ApiResult<CreateResponseDto>> CreateInventoryRequestAsync(CateringInventoryRequestDto request)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/inventory-requests") { Content = JsonContent.Create(new { Request = request }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> SubmitInventoryRequestAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/inventory-requests/{id}/submit"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> ApproveInventoryRequestAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/inventory-requests/{id}/approve"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> FulfillInventoryRequestAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/inventory-requests/{id}/fulfill"), null);

    public Task<ApiResult<List<CateringReportRowDto>>> GetOperationsReportAsync(Guid? companyId = null, Guid? contractId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        if (fromDate.HasValue) extra.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) extra.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        return SendAsync<List<CateringReportRowDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/reports/operations", 0, 500, companyId, null, extra.ToArray())), "report");
    }

    private static string Query(string baseUrl, int pageIndex, int pageSize, Guid? companyId, string? searchText, params string[] extra)
    {
        var parts = new List<string> { $"pageIndex={pageIndex}", $"pageSize={pageSize}" };
        if (companyId.HasValue) parts.Add($"companyId={companyId}");
        if (!string.IsNullOrWhiteSpace(searchText)) parts.Add($"searchText={Uri.EscapeDataString(searchText)}");
        parts.AddRange(extra.Where(x => !string.IsNullOrWhiteSpace(x)));
        return $"{baseUrl}?{string.Join("&", parts)}";
    }

    private static List<string> DateQuery(Guid? scheduleId, DateTime? fromDate, DateTime? toDate)
    {
        var extra = new List<string>();
        if (scheduleId.HasValue) extra.Add($"scheduleId={scheduleId}");
        if (fromDate.HasValue) extra.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) extra.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        return extra;
    }
}
