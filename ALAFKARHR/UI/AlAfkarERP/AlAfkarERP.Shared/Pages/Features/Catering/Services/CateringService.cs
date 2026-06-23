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

    public Task<ApiResult<PaginatedResult<CateringDailyScheduleDto>>> GetSchedulesAsync(int pageIndex, int pageSize, Guid? contractId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var extra = new List<string>();
        if (contractId.HasValue) extra.Add($"contractId={contractId}");
        if (fromDate.HasValue) extra.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) extra.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        return SendAsync<PaginatedResult<CateringDailyScheduleDto>>(new HttpRequestMessage(HttpMethod.Get, Query($"{path}/schedules", pageIndex, pageSize, null, null, extra.ToArray())), "schedules");
    }

    public Task<ApiResult<CreateResponseDto>> CreateScheduleAsync(CateringDailyScheduleDto schedule)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/schedules") { Content = JsonContent.Create(new { Schedule = schedule }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateScheduleAsync(CateringDailyScheduleDto schedule)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/schedules/{schedule.Id}") { Content = JsonContent.Create(new { Schedule = schedule }) }, null);

    public Task<ApiResult<CreateResponseDto>> CreateAllocationAsync(Guid scheduleId, CateringSquareAllocationDto allocation)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/schedules/{scheduleId}/allocations") { Content = JsonContent.Create(new { Allocation = allocation }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> RecordAllocationActualsAsync(Guid allocationId, decimal receivedQuantity, decimal distributedQuantity, string? varianceNotes)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/allocations/{allocationId}/actuals") { Content = JsonContent.Create(new { ReceivedQuantity = receivedQuantity, DistributedQuantity = distributedQuantity, VarianceNotes = varianceNotes }) }, null);

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
}
