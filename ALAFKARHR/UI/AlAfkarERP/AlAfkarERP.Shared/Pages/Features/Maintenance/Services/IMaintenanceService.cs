using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Maintenance.Dtos;
using SharedWithUI.Maintenance.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Maintenance.Services;

public interface IMaintenanceService
{
    Task<ApiResult<PaginatedResult<MaintenanceAssetDto>>> GetAssetsAsync(int pageIndex, int pageSize, string searchText = "", MaintenanceAssetType? assetType = null, MaintenanceAssetStatus? status = null, Guid? companyId = null, Guid? branchId = null, Guid? parentAssetId = null);
    Task<ApiResult<MaintenanceAssetDto>> GetAssetByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAssetAsync(CreateMaintenanceAssetDto asset);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAssetAsync(UpdateMaintenanceAssetDto asset);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAssetAsync(Guid id);
    Task<ApiResult<PaginatedResult<MaintenanceWorkOrderDto>>> GetWorkOrdersAsync(int pageIndex, int pageSize, string searchText = "", Guid? assetId = null, Guid? branchId = null, MaintenanceAssetType? assetType = null, MaintenancePriority? priority = null, MaintenanceWorkOrderStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<PaginatedResult<MaintenanceWorkOrderDto>>> GetMyWorkOrdersAsync(int pageIndex, int pageSize, string searchText = "");
    Task<ApiResult<MaintenanceWorkOrderDto>> GetWorkOrderByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateWorkOrderAsync(CreateMaintenanceWorkOrderDto workOrder);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateWorkOrderAsync(UpdateMaintenanceWorkOrderDto workOrder);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteWorkOrderAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> AssignWorkOrderAsync(Guid id, AssignMaintenanceWorkOrderDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> ChangeWorkOrderStatusAsync(Guid id, MaintenanceWorkOrderStatus status);
    Task<ApiResult<UpdateDeleteResponseDto>> ApproveCostAsync(Guid id, ApproveMaintenanceCostDto approval);
    Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid id, string comment);
    Task<ApiResult<CreateResponseDto>> UploadAttachmentAsync(Guid id, Stream fileStream, string fileName, string contentType);
    Task<ApiResult<MaintenanceDashboardDto>> GetDashboardAsync();
    Task<ApiResult<MaintenanceSummaryReportDto>> GetSummaryReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
}
