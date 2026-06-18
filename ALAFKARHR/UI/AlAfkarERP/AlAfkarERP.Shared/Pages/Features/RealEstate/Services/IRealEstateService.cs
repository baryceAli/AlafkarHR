using AlAfkarERP.Shared.Dtos;
using SharedWithUI.RealEstate.Dtos;
using SharedWithUI.RealEstate.Enums;

namespace AlAfkarERP.Shared.Pages.Features.RealEstate.Services;

public interface IRealEstateService
{
    Task<ApiResult<RealEstateDashboardDto>> GetDashboardAsync(Guid? companyId = null);
    Task<ApiResult<PaginatedResult<PropertyDto>>> GetPropertiesAsync(int pageIndex, int pageSize, string searchText = "", Guid? companyId = null);
    Task<ApiResult<PropertyDto>> GetPropertyByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreatePropertyAsync(PropertyDto property);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdatePropertyAsync(PropertyDto property);
    Task<ApiResult<UpdateDeleteResponseDto>> DeletePropertyAsync(Guid id);
    Task<ApiResult<PaginatedResult<PropertyUnitDto>>> GetUnitsAsync(int pageIndex, int pageSize, string searchText = "", Guid? propertyId = null);
    Task<ApiResult<PropertyUnitDto>> GetUnitByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateUnitAsync(PropertyUnitDto unit);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateUnitAsync(PropertyUnitDto unit);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteUnitAsync(Guid id);
    Task<ApiResult<PaginatedResult<LeaseDto>>> GetLeasesAsync(int pageIndex, int pageSize, string searchText = "", LeaseDirection? direction = null, Guid? companyId = null, Guid? propertyId = null, Guid? unitId = null, LeaseStatus? status = null);
    Task<ApiResult<LeaseDto>> GetLeaseByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateLeaseAsync(LeaseDto lease);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateLeaseAsync(LeaseDto lease);
    Task<ApiResult<UpdateDeleteResponseDto>> GenerateInstallmentsAsync(Guid leaseId);
    Task<ApiResult<UpdateDeleteResponseDto>> ActivateLeaseAsync(Guid leaseId);
    Task<ApiResult<UpdateDeleteResponseDto>> SuspendLeaseAsync(Guid leaseId);
    Task<ApiResult<UpdateDeleteResponseDto>> TerminateLeaseAsync(Guid leaseId);
    Task<ApiResult<PaginatedResult<LeaseInstallmentDto>>> GetInstallmentsAsync(int pageIndex, int pageSize, Guid? leaseId = null, Guid? companyId = null, InstallmentStatus? status = null);
    Task<ApiResult<UpdateDeleteResponseDto>> RecordRentPaymentAsync(RecordRentPaymentDto payment);
    Task<ApiResult<PaginatedResult<PropertyExpenseDto>>> GetExpensesAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? propertyId = null, ExpenseCategory? category = null);
    Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(PropertyExpenseDto expense);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateExpenseAsync(PropertyExpenseDto expense);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteExpenseAsync(Guid id);
    Task<ApiResult<PaginatedResult<UtilityAccountDto>>> GetUtilityAccountsAsync(int pageIndex, int pageSize, Guid? propertyId = null);
    Task<ApiResult<CreateResponseDto>> CreateUtilityAccountAsync(UtilityAccountDto utilityAccount);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateUtilityAccountAsync(UtilityAccountDto utilityAccount);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteUtilityAccountAsync(Guid id);
    Task<ApiResult<PaginatedResult<UtilityBillDto>>> GetUtilityBillsAsync(int pageIndex, int pageSize, Guid? propertyId = null, Guid? utilityAccountId = null, bool? isPaid = null);
    Task<ApiResult<CreateResponseDto>> CreateUtilityBillAsync(UtilityBillDto utilityBill);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateUtilityBillAsync(UtilityBillDto utilityBill);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteUtilityBillAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> MarkUtilityBillPaidAsync(Guid id);
    Task<ApiResult<RealEstateReportsDto>> GetReportsAsync(Guid? companyId = null);
}
