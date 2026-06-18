using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.Maintenance.Dtos;
using SharedWithUI.Maintenance.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Maintenance.Services;

public class MaintenanceService : BaseApiService, IMaintenanceService
{
    private readonly string path;

    public MaintenanceService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/maintenance";
    }

    public async Task<ApiResult<PaginatedResult<MaintenanceAssetDto>>> GetAssetsAsync(int pageIndex, int pageSize, string searchText = "", MaintenanceAssetType? assetType = null, MaintenanceAssetStatus? status = null, Guid? companyId = null, Guid? branchId = null, Guid? parentAssetId = null)
    {
        var url = $"{path}/assets?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (assetType.HasValue) url += $"&assetType={assetType}";
        if (status.HasValue) url += $"&status={status}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (branchId.HasValue) url += $"&branchId={branchId}";
        if (parentAssetId.HasValue) url += $"&parentAssetId={parentAssetId}";
        return await SendAsync<PaginatedResult<MaintenanceAssetDto>>(new HttpRequestMessage(HttpMethod.Get, url), "assets");
    }

    public async Task<ApiResult<MaintenanceAssetDto>> GetAssetByIdAsync(Guid id)
        => await SendAsync<MaintenanceAssetDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/assets/{id}"), "asset");

    public async Task<ApiResult<CreateResponseDto>> CreateAssetAsync(CreateMaintenanceAssetDto asset)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/assets")
        {
            Content = JsonContent.Create(new { Asset = asset })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAssetAsync(UpdateMaintenanceAssetDto asset)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/assets")
        {
            Content = JsonContent.Create(new { Asset = asset })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAssetAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/assets/{id}"), null);

    public async Task<ApiResult<PaginatedResult<MaintenanceWorkOrderDto>>> GetWorkOrdersAsync(int pageIndex, int pageSize, string searchText = "", Guid? assetId = null, Guid? branchId = null, MaintenanceAssetType? assetType = null, MaintenancePriority? priority = null, MaintenanceWorkOrderStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/work-orders?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (assetId.HasValue) url += $"&assetId={assetId}";
        if (branchId.HasValue) url += $"&branchId={branchId}";
        if (assetType.HasValue) url += $"&assetType={assetType}";
        if (priority.HasValue) url += $"&priority={priority}";
        if (status.HasValue) url += $"&status={status}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return await SendAsync<PaginatedResult<MaintenanceWorkOrderDto>>(new HttpRequestMessage(HttpMethod.Get, url), "workOrders");
    }

    public async Task<ApiResult<PaginatedResult<MaintenanceWorkOrderDto>>> GetMyWorkOrdersAsync(int pageIndex, int pageSize, string searchText = "")
        => await SendAsync<PaginatedResult<MaintenanceWorkOrderDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/work-orders/my?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}"), "workOrders");

    public async Task<ApiResult<MaintenanceWorkOrderDto>> GetWorkOrderByIdAsync(Guid id)
        => await SendAsync<MaintenanceWorkOrderDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/work-orders/{id}"), "workOrder");

    public async Task<ApiResult<CreateResponseDto>> CreateWorkOrderAsync(CreateMaintenanceWorkOrderDto workOrder)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-orders")
        {
            Content = JsonContent.Create(new { WorkOrder = workOrder })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateWorkOrderAsync(UpdateMaintenanceWorkOrderDto workOrder)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/work-orders")
        {
            Content = JsonContent.Create(new { WorkOrder = workOrder })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteWorkOrderAsync(Guid id)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/work-orders/{id}"), null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> AssignWorkOrderAsync(Guid id, AssignMaintenanceWorkOrderDto assignment)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/work-orders/{id}/assign")
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> ChangeWorkOrderStatusAsync(Guid id, MaintenanceWorkOrderStatus status)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/work-orders/{id}/status")
        {
            Content = JsonContent.Create(new { WorkOrderStatus = new ChangeMaintenanceWorkOrderStatusDto { Status = status } })
        }, null);

    public async Task<ApiResult<UpdateDeleteResponseDto>> ApproveCostAsync(Guid id, ApproveMaintenanceCostDto approval)
        => await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/work-orders/{id}/cost-approval")
        {
            Content = JsonContent.Create(new { CostApproval = approval })
        }, null);

    public async Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid id, string comment)
        => await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-orders/{id}/comments")
        {
            Content = JsonContent.Create(new { Comment = new CreateMaintenanceCommentDto { Comment = comment } })
        }, null);

    public async Task<ApiResult<CreateResponseDto>> UploadAttachmentAsync(Guid id, Stream fileStream, string fileName, string contentType)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-orders/{id}/attachments")
        {
            Content = content
        }, null);
    }

    public async Task<ApiResult<MaintenanceDashboardDto>> GetDashboardAsync()
        => await SendAsync<MaintenanceDashboardDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/dashboard"), "dashboard");

    public async Task<ApiResult<MaintenanceSummaryReportDto>> GetSummaryReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/reports/summary?";
        if (fromDate.HasValue) url += $"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}&";
        if (toDate.HasValue) url += $"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}&";
        return await SendAsync<MaintenanceSummaryReportDto>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }
}
