using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catering.Dtos;
using SharedWithUI.Catering.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Catering.Services;

public interface ICateringService
{
    Task<ApiResult<CateringDashboardDto>> GetDashboardAsync(Guid? companyId = null);
    Task<ApiResult<PaginatedResult<MealDefinitionDto>>> GetMealsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true);
    Task<ApiResult<MealDefinitionDto>> GetMealByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateMealAsync(MealDefinitionDto meal);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateMealAsync(MealDefinitionDto meal);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteMealAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> AddMealComponentAsync(Guid mealId, MealComponentDto component);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteMealComponentAsync(Guid mealId, Guid componentId);
    Task<ApiResult<PaginatedResult<CateringContractDto>>> GetContractsAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, Guid? customerId = null, CateringContractStatus? status = null);
    Task<ApiResult<CateringContractDto>> GetContractByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateContractAsync(CateringContractDto contract);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateContractAsync(CateringContractDto contract);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteContractAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> CloseContractAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> AddAddendumAsync(Guid contractId, CateringContractAddendumDto addendum);
    Task<ApiResult<PaginatedResult<CateringAreaDto>>> GetAreasAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true);
    Task<ApiResult<CreateResponseDto>> CreateAreaAsync(CateringAreaDto area);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAreaAsync(CateringAreaDto area);
    Task<ApiResult<PaginatedResult<CateringSquareDto>>> GetSquaresAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? areaId = null, string? searchText = null, bool activeOnly = true);
    Task<ApiResult<CreateResponseDto>> CreateSquareAsync(CateringSquareDto square);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateSquareAsync(CateringSquareDto square);
    Task<ApiResult<PaginatedResult<CateringDailyScheduleDto>>> GetSchedulesAsync(int pageIndex, int pageSize, Guid? contractId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<CreateResponseDto>> CreateScheduleAsync(CateringDailyScheduleDto schedule);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateScheduleAsync(CateringDailyScheduleDto schedule);
    Task<ApiResult<CreateResponseDto>> CreateAllocationAsync(Guid scheduleId, CateringSquareAllocationDto allocation);
    Task<ApiResult<UpdateDeleteResponseDto>> RecordAllocationActualsAsync(Guid allocationId, decimal receivedQuantity, decimal distributedQuantity, string? varianceNotes);
    Task<ApiResult<PaginatedResult<CateringVehicleDeliveryDto>>> GetDeliveriesAsync(int pageIndex, int pageSize, Guid? scheduleId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<CreateResponseDto>> CreateDeliveryAsync(CateringVehicleDeliveryDto delivery);
    Task<ApiResult<PaginatedResult<CateringAssignmentDto>>> GetAssignmentsAsync(int pageIndex, int pageSize, Guid? contractId = null, CateringAssignmentRole? role = null, Guid? squareId = null);
    Task<ApiResult<CreateResponseDto>> CreateAssignmentAsync(CateringAssignmentDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAssignmentAsync(CateringAssignmentDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAssignmentAsync(Guid id);
    Task<ApiResult<List<CateringReportRowDto>>> GetOperationsReportAsync(Guid? companyId = null, Guid? contractId = null, DateTime? fromDate = null, DateTime? toDate = null);
}
