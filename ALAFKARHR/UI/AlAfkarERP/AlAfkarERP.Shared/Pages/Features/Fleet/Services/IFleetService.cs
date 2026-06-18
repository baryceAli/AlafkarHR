using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Fleet.Dtos;
using SharedWithUI.Fleet.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Fleet.Services;

public interface IFleetService
{
    Task<ApiResult<FleetDashboardDto>> GetDashboardAsync();
    Task<ApiResult<PaginatedResult<FleetVehicleDto>>> GetVehiclesAsync(int pageIndex, int pageSize, string searchText = "", Guid? companyId = null, Guid? branchId = null, FleetVehicleOwnershipType? ownershipType = null, FleetVehicleStatus? status = null, FleetVehicleType? vehicleType = null);
    Task<ApiResult<FleetVehicleDetailsDto>> GetVehicleByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateVehicleAsync(CreateFleetVehicleDto vehicle);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateVehicleAsync(UpdateFleetVehicleDto vehicle);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteVehicleAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateOdometerAsync(Guid id, int odometer);
    Task<ApiResult<CreateResponseDto>> CreateEmergencyMaintenanceAsync(CreateEmergencyFleetMaintenanceRequestDto request);
    Task<ApiResult<PaginatedResult<FleetVehicleAssignmentDto>>> GetAssignmentsAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetAssignmentStatus? status = null);
    Task<ApiResult<CreateResponseDto>> CreateAssignmentAsync(CreateFleetVehicleAssignmentDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> ReturnAssignmentAsync(Guid id, ReturnFleetVehicleAssignmentDto assignmentReturn);
    Task<ApiResult<UpdateDeleteResponseDto>> CancelAssignmentAsync(Guid id);
    Task<ApiResult<PaginatedResult<FleetVehicleExpenseDto>>> GetExpensesAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetExpenseCategory? category = null, FleetExpenseApprovalStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(CreateFleetVehicleExpenseDto expense);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateExpenseAsync(UpdateFleetVehicleExpenseDto expense);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteExpenseAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> SubmitExpenseAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> ApproveExpenseAsync(Guid id, ApproveFleetVehicleExpenseDto approval);
    Task<ApiResult<PaginatedResult<FleetVehicleDocumentDto>>> GetDocumentsAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, FleetDocumentType? documentType = null, FleetDocumentStatus? status = null);
    Task<ApiResult<CreateResponseDto>> CreateDocumentAsync(CreateFleetVehicleDocumentDto document);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateDocumentAsync(UpdateFleetVehicleDocumentDto document);
    Task<ApiResult<UpdateDeleteResponseDto>> RenewDocumentAsync(Guid id, RenewFleetVehicleDocumentDto renewal);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteDocumentAsync(Guid id);
    Task<ApiResult<PaginatedResult<FleetVehicleServiceRuleDto>>> GetServiceRulesAsync(int pageIndex, int pageSize, string searchText = "", Guid? vehicleId = null, bool? dueOnly = null);
    Task<ApiResult<CreateResponseDto>> CreateServiceRuleAsync(CreateFleetVehicleServiceRuleDto serviceRule);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateServiceRuleAsync(UpdateFleetVehicleServiceRuleDto serviceRule);
    Task<ApiResult<UpdateDeleteResponseDto>> CompleteServiceRuleAsync(Guid id, CompleteFleetVehicleServiceRuleDto completion);
    Task<ApiResult<CreateResponseDto>> CreateMaintenanceFromServiceRuleAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteServiceRuleAsync(Guid id);
    Task<ApiResult<FleetSummaryReportDto>> GetSummaryReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

public class CreateEmergencyFleetMaintenanceRequestDto
{
    public Guid VehicleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SharedWithUI.Maintenance.Enums.MaintenancePriority Priority { get; set; } = SharedWithUI.Maintenance.Enums.MaintenancePriority.High;
    public decimal? EstimatedCost { get; set; }
    public string? VendorName { get; set; }
    public Guid? SupplierId { get; set; }
}

public class CompleteFleetVehicleServiceRuleDto
{
    public int? Odometer { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.UtcNow.Date;
}
