using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.Fleet.Dtos;
using SharedWithUI.Fleet.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Fleet.Services;

public class FleetService : BaseApiService, IFleetService
{
    private readonly string path;

    public FleetService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/fleet";
    }

    public async Task<ApiResult<FleetDashboardDto>> GetDashboardAsync()
        => await SendAsync<FleetDashboardDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/dashboard"), "dashboard");

    public async Task<ApiResult<PaginatedResult<FleetVehicleDto>>> GetVehiclesAsync(int pageIndex, int pageSize, string searchText = "", Guid? companyId = null, Guid? branchId = null, FleetVehicleOwnershipType? ownershipType = null, FleetVehicleStatus? status = null, FleetVehicleType? vehicleType = null)
    {
        var url = $"{path}/vehicles?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (branchId.HasValue) url += $"&branchId={branchId}";
        if (ownershipType.HasValue) url += $"&ownershipType={ownershipType}";
        if (status.HasValue) url += $"&status={status}";
        if (vehicleType.HasValue) url += $"&vehicleType={vehicleType}";
        return await SendAsync<PaginatedResult<FleetVehicleDto>>(new HttpRequestMessage(HttpMethod.Get, url), "vehicles");
    }

    public async Task<ApiResult<FleetVehicleDetailsDto>> GetVehicleByIdAsync(Guid id)
        => await SendAsync<FleetVehicleDetailsDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/vehicles/{id}"), "vehicleDetails");

    public async Task<ApiResult<CreateResponseDto>> CreateVehicleAsync(CreateFleetVehicleDto vehicle)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/vehicles") { Content = JsonContent.Create(new { Vehicle = vehicle }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateVehicleAsync(UpdateFleetVehicleDto vehicle)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/vehicles") { Content = JsonContent.Create(new { Vehicle = vehicle }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteVehicleAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/vehicles/{id}"), null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateOdometerAsync(Guid id, int odometer)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/vehicles/{id}/odometer") { Content = JsonContent.Create(new { Odometer = odometer }) }, null);

    public async Task<ApiResult<CreateResponseDto>> CreateEmergencyMaintenanceAsync(CreateEmergencyFleetMaintenanceRequestDto request)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/vehicles/maintenance/emergency") { Content = JsonContent.Create(request) }, null);

    public async Task<ApiResult<PaginatedResult<FleetVehicleAssignmentDto>>> GetAssignmentsAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetAssignmentStatus? status = null)
    {
        var url = $"{path}/assignments?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (vehicleId.HasValue) url += $"&vehicleId={vehicleId}";
        if (status.HasValue) url += $"&status={status}";
        return await SendAsync<PaginatedResult<FleetVehicleAssignmentDto>>(new HttpRequestMessage(HttpMethod.Get, url), "assignments");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAssignmentAsync(CreateFleetVehicleAssignmentDto assignment)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/assignments") { Content = JsonContent.Create(new { Assignment = assignment }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> ReturnAssignmentAsync(Guid id, ReturnFleetVehicleAssignmentDto assignmentReturn)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/assignments/{id}/return") { Content = JsonContent.Create(new { Return = assignmentReturn }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> CancelAssignmentAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/assignments/{id}/cancel"), null);

    public async Task<ApiResult<PaginatedResult<FleetVehicleExpenseDto>>> GetExpensesAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetExpenseCategory? category = null, FleetExpenseApprovalStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/expenses?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (vehicleId.HasValue) url += $"&vehicleId={vehicleId}";
        if (category.HasValue) url += $"&category={category}";
        if (status.HasValue) url += $"&status={status}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return await SendAsync<PaginatedResult<FleetVehicleExpenseDto>>(new HttpRequestMessage(HttpMethod.Get, url), "expenses");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(CreateFleetVehicleExpenseDto expense)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/expenses") { Content = JsonContent.Create(new { Expense = expense }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateExpenseAsync(UpdateFleetVehicleExpenseDto expense)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/expenses") { Content = JsonContent.Create(new { Expense = expense }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteExpenseAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/expenses/{id}"), null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> SubmitExpenseAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/expenses/{id}/submit"), null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> ApproveExpenseAsync(Guid id, ApproveFleetVehicleExpenseDto approval)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/expenses/{id}/approval") { Content = JsonContent.Create(new { Approval = approval }) }, null);

    public async Task<ApiResult<PaginatedResult<FleetVehicleDocumentDto>>> GetDocumentsAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetDocumentType? documentType = null, FleetDocumentStatus? status = null)
    {
        var url = $"{path}/documents?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (vehicleId.HasValue) url += $"&vehicleId={vehicleId}";
        if (documentType.HasValue) url += $"&documentType={documentType}";
        if (status.HasValue) url += $"&status={status}";
        return await SendAsync<PaginatedResult<FleetVehicleDocumentDto>>(new HttpRequestMessage(HttpMethod.Get, url), "documents");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateDocumentAsync(CreateFleetVehicleDocumentDto document)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/documents") { Content = JsonContent.Create(new { Document = document }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateDocumentAsync(UpdateFleetVehicleDocumentDto document)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/documents") { Content = JsonContent.Create(new { Document = document }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> RenewDocumentAsync(Guid id, RenewFleetVehicleDocumentDto renewal)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/documents/{id}/renew") { Content = JsonContent.Create(new { Renewal = renewal }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteDocumentAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/documents/{id}"), null);

    public async Task<ApiResult<PaginatedResult<FleetVehicleServiceRuleDto>>> GetServiceRulesAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, bool? dueOnly = null)
    {
        var url = $"{path}/service-rules?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (vehicleId.HasValue) url += $"&vehicleId={vehicleId}";
        if (dueOnly.HasValue) url += $"&dueOnly={dueOnly}";
        return await SendAsync<PaginatedResult<FleetVehicleServiceRuleDto>>(new HttpRequestMessage(HttpMethod.Get, url), "serviceRules");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateServiceRuleAsync(CreateFleetVehicleServiceRuleDto serviceRule)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/service-rules") { Content = JsonContent.Create(new { ServiceRule = serviceRule }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateServiceRuleAsync(UpdateFleetVehicleServiceRuleDto serviceRule)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/service-rules") { Content = JsonContent.Create(new { ServiceRule = serviceRule }) }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> CompleteServiceRuleAsync(Guid id, CompleteFleetVehicleServiceRuleDto completion)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/service-rules/{id}/complete") { Content = JsonContent.Create(completion) }, null);

    public async Task<ApiResult<CreateResponseDto>> CreateMaintenanceFromServiceRuleAsync(Guid id)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/service-rules/{id}/maintenance-work-order"), null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteServiceRuleAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/service-rules/{id}"), null);

    public async Task<ApiResult<FleetSummaryReportDto>> GetSummaryReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/reports/summary?";
        if (fromDate.HasValue) url += $"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}&";
        if (toDate.HasValue) url += $"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}&";
        return await SendAsync<FleetSummaryReportDto>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }
}
