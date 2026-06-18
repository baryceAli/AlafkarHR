using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.RealEstate.Dtos;
using SharedWithUI.RealEstate.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.RealEstate.Services;

public class RealEstateService : BaseApiService, IRealEstateService
{
    private readonly string path;

    public RealEstateService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/real-estate";
    }

    public Task<ApiResult<RealEstateDashboardDto>> GetDashboardAsync(Guid? companyId = null)
    {
        var url = $"{path}/dashboard";
        if (companyId.HasValue) url += $"?companyId={companyId}";
        return SendAsync<RealEstateDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public Task<ApiResult<PaginatedResult<PropertyDto>>> GetPropertiesAsync(int pageIndex, int pageSize, string searchText = "", Guid? companyId = null)
    {
        var url = $"{path}/properties?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        return SendAsync<PaginatedResult<PropertyDto>>(new HttpRequestMessage(HttpMethod.Get, url), "properties");
    }

    public Task<ApiResult<PropertyDto>> GetPropertyByIdAsync(Guid id)
        => SendAsync<PropertyDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/properties/{id}"), "property");

    public Task<ApiResult<CreateResponseDto>> CreatePropertyAsync(PropertyDto property)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/properties") { Content = JsonContent.Create(new { Property = property }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdatePropertyAsync(PropertyDto property)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/properties/{property.Id}") { Content = JsonContent.Create(new { Property = property }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeletePropertyAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/properties/{id}"), null);

    public Task<ApiResult<PaginatedResult<PropertyUnitDto>>> GetUnitsAsync(int pageIndex, int pageSize, string searchText = "", Guid? propertyId = null)
    {
        var url = $"{path}/units?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (propertyId.HasValue) url += $"&propertyId={propertyId}";
        return SendAsync<PaginatedResult<PropertyUnitDto>>(new HttpRequestMessage(HttpMethod.Get, url), "units");
    }

    public Task<ApiResult<PropertyUnitDto>> GetUnitByIdAsync(Guid id)
        => SendAsync<PropertyUnitDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/units/{id}"), "unit");

    public Task<ApiResult<CreateResponseDto>> CreateUnitAsync(PropertyUnitDto unit)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/units") { Content = JsonContent.Create(new { Unit = unit }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateUnitAsync(PropertyUnitDto unit)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/units/{unit.Id}") { Content = JsonContent.Create(new { Unit = unit }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteUnitAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/units/{id}"), null);

    public Task<ApiResult<PaginatedResult<LeaseDto>>> GetLeasesAsync(int pageIndex, int pageSize, string searchText = "", LeaseDirection? direction = null, Guid? companyId = null, Guid? propertyId = null, Guid? unitId = null, LeaseStatus? status = null)
    {
        var url = $"{path}/leases?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (direction.HasValue) url += $"&direction={direction}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (propertyId.HasValue) url += $"&propertyId={propertyId}";
        if (unitId.HasValue) url += $"&unitId={unitId}";
        if (status.HasValue) url += $"&status={status}";
        return SendAsync<PaginatedResult<LeaseDto>>(new HttpRequestMessage(HttpMethod.Get, url), "leases");
    }

    public Task<ApiResult<CreateResponseDto>> CreateLeaseAsync(LeaseDto lease)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leases") { Content = JsonContent.Create(new { Lease = lease }) }, null);

    public Task<ApiResult<LeaseDto>> GetLeaseByIdAsync(Guid id)
        => SendAsync<LeaseDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/leases/{id}"), "lease");

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateLeaseAsync(LeaseDto lease)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/leases/{lease.Id}") { Content = JsonContent.Create(new { Lease = lease }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> GenerateInstallmentsAsync(Guid leaseId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leases/{leaseId}/generate-installments"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> ActivateLeaseAsync(Guid leaseId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leases/{leaseId}/activate"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> SuspendLeaseAsync(Guid leaseId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leases/{leaseId}/suspend"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> TerminateLeaseAsync(Guid leaseId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leases/{leaseId}/terminate"), null);

    public Task<ApiResult<PaginatedResult<LeaseInstallmentDto>>> GetInstallmentsAsync(int pageIndex, int pageSize, Guid? leaseId = null, Guid? companyId = null, InstallmentStatus? status = null)
    {
        var url = $"{path}/installments?pageIndex={pageIndex}&pageSize={pageSize}";
        if (leaseId.HasValue) url += $"&leaseId={leaseId}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (status.HasValue) url += $"&status={status}";
        return SendAsync<PaginatedResult<LeaseInstallmentDto>>(new HttpRequestMessage(HttpMethod.Get, url), "installments");
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> RecordRentPaymentAsync(RecordRentPaymentDto payment)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/installments/payments") { Content = JsonContent.Create(new { Payment = payment }) }, null);

    public Task<ApiResult<PaginatedResult<PropertyExpenseDto>>> GetExpensesAsync(int pageIndex, int pageSize, Guid? companyId = null, Guid? propertyId = null, ExpenseCategory? category = null)
    {
        var url = $"{path}/expenses?pageIndex={pageIndex}&pageSize={pageSize}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        if (propertyId.HasValue) url += $"&propertyId={propertyId}";
        if (category.HasValue) url += $"&category={category}";
        return SendAsync<PaginatedResult<PropertyExpenseDto>>(new HttpRequestMessage(HttpMethod.Get, url), "expenses");
    }

    public Task<ApiResult<CreateResponseDto>> CreateExpenseAsync(PropertyExpenseDto expense)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/expenses") { Content = JsonContent.Create(new { Expense = expense }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateExpenseAsync(PropertyExpenseDto expense)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/expenses/{expense.Id}") { Content = JsonContent.Create(new { Expense = expense }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteExpenseAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/expenses/{id}"), null);

    public Task<ApiResult<PaginatedResult<UtilityAccountDto>>> GetUtilityAccountsAsync(int pageIndex, int pageSize, Guid? propertyId = null)
    {
        var url = $"{path}/utility-accounts?pageIndex={pageIndex}&pageSize={pageSize}";
        if (propertyId.HasValue) url += $"&propertyId={propertyId}";
        return SendAsync<PaginatedResult<UtilityAccountDto>>(new HttpRequestMessage(HttpMethod.Get, url), "utilityAccounts");
    }

    public Task<ApiResult<CreateResponseDto>> CreateUtilityAccountAsync(UtilityAccountDto utilityAccount)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/utility-accounts") { Content = JsonContent.Create(new { UtilityAccount = utilityAccount }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateUtilityAccountAsync(UtilityAccountDto utilityAccount)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/utility-accounts/{utilityAccount.Id}") { Content = JsonContent.Create(new { UtilityAccount = utilityAccount }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteUtilityAccountAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/utility-accounts/{id}"), null);

    public Task<ApiResult<PaginatedResult<UtilityBillDto>>> GetUtilityBillsAsync(int pageIndex, int pageSize, Guid? propertyId = null, Guid? utilityAccountId = null, bool? isPaid = null)
    {
        var url = $"{path}/utility-bills?pageIndex={pageIndex}&pageSize={pageSize}";
        if (propertyId.HasValue) url += $"&propertyId={propertyId}";
        if (utilityAccountId.HasValue) url += $"&utilityAccountId={utilityAccountId}";
        if (isPaid.HasValue) url += $"&isPaid={isPaid}";
        return SendAsync<PaginatedResult<UtilityBillDto>>(new HttpRequestMessage(HttpMethod.Get, url), "utilityBills");
    }

    public Task<ApiResult<CreateResponseDto>> CreateUtilityBillAsync(UtilityBillDto utilityBill)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/utility-bills") { Content = JsonContent.Create(new { UtilityBill = utilityBill }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateUtilityBillAsync(UtilityBillDto utilityBill)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/utility-bills/{utilityBill.Id}") { Content = JsonContent.Create(new { UtilityBill = utilityBill }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteUtilityBillAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/utility-bills/{id}"), null);

    public Task<ApiResult<UpdateDeleteResponseDto>> MarkUtilityBillPaidAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/utility-bills/{id}/mark-paid"), null);

    public Task<ApiResult<RealEstateReportsDto>> GetReportsAsync(Guid? companyId = null)
    {
        var url = $"{path}/reports";
        if (companyId.HasValue) url += $"?companyId={companyId}";
        return SendAsync<RealEstateReportsDto>(new HttpRequestMessage(HttpMethod.Get, url), "reports");
    }
}
