using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Procurement.Dtos;
using SharedWithUI.Procurement.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Procurement.Services;

public interface IProcurementService
{
    Task<ApiResult<ProcurementDashboardDto>> GetDashboardAsync(Guid? companyId);
    Task<ApiResult<PaginatedResult<ProcurementDocumentDto>>> GetAsync(ProcurementDocumentKind kind, Guid? companyId, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<ProcurementDocumentDto>> GetByIdAsync(ProcurementDocumentKind kind, Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(ProcurementDocumentDto document);
    Task<ApiResult<string>> UpdateAsync(ProcurementDocumentDto document);
    Task<ApiResult<string>> DeleteAsync(ProcurementDocumentKind kind, Guid id);
    Task<ApiResult<string>> WorkflowAsync(ProcurementDocumentKind kind, Guid id, string action);
    Task<ApiResult<List<SupplierItemDto>>> GetSupplierItemsAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveSupplierItemAsync(SupplierItemDto item);
    Task<ApiResult<string>> DeleteSupplierItemAsync(Guid id);
    Task<ApiResult<List<VendorPricelistDto>>> GetVendorPricelistsAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveVendorPricelistAsync(VendorPricelistDto item);
    Task<ApiResult<string>> DeleteVendorPricelistAsync(Guid id);
    Task<ApiResult<List<ReorderingRuleDto>>> GetReorderingRulesAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveReorderingRuleAsync(ReorderingRuleDto item);
    Task<ApiResult<string>> DeleteReorderingRuleAsync(Guid id);
    Task<ApiResult<List<ReplenishmentSuggestionDto>>> GetReplenishmentSuggestionsAsync(Guid companyId, Guid? branchId, Guid? warehouseId, Guid? productSkuId);
    Task<ApiResult<CreateResponseDto>> CreatePurchaseRequestFromReplenishmentAsync(CreatePurchaseRequestFromReplenishmentDto request);
    Task<ApiResult<List<ProcurementTrackerRowDto>>> GetTrackerAsync(Guid companyId);
    Task<ApiResult<List<SupplierScorecardRowDto>>> GetSupplierScorecardAsync(Guid companyId);
}
