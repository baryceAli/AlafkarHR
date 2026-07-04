using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Procurement.Dtos;
using SharedWithUI.Procurement.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Procurement.Services;

public class ProcurementService : BaseApiService, IProcurementService
{
    private readonly ApiConfig _apiConfig;

    public ProcurementService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
    }

    public async Task<ApiResult<ProcurementDashboardDto>> GetDashboardAsync(Guid? companyId)
    {
        var query = companyId.HasValue ? $"?companyId={companyId}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/dashboard{query}");
        return await SendAsync<ProcurementDashboardDto>(request, "dashboard");
    }

    public async Task<ApiResult<PaginatedResult<ProcurementDocumentDto>>> GetAsync(ProcurementDocumentKind kind, Guid? companyId, int pageIndex, int pageSize, string? searchText)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/{Route(kind)}?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<ProcurementDocumentDto>>(request, "documents");
    }

    public async Task<ApiResult<ProcurementDocumentDto>> GetByIdAsync(ProcurementDocumentKind kind, Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/{Route(kind)}/{id}");
        return await SendAsync<ProcurementDocumentDto>(request, "document");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(ProcurementDocumentDto document)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/{Route(document.Kind)}")
        {
            Content = JsonContent.Create(new { Document = document })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> UpdateAsync(ProcurementDocumentDto document)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/procurement/{Route(document.Kind)}/{document.Id}")
        {
            Content = JsonContent.Create(new { Document = document })
        };
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> DeleteAsync(ProcurementDocumentKind kind, Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/procurement/{Route(kind)}/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> WorkflowAsync(ProcurementDocumentKind kind, Guid id, string action)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/{Route(kind)}/{id}/{action}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<List<SupplierItemDto>>> GetSupplierItemsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/supplier-items/company/{companyId}");
        return await SendAsync<List<SupplierItemDto>>(request, "items");
    }

    public async Task<ApiResult<CreateResponseDto>> SaveSupplierItemAsync(SupplierItemDto item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/supplier-items")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> DeleteSupplierItemAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/procurement/supplier-items/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<List<VendorPricelistDto>>> GetVendorPricelistsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/vendor-pricelists/company/{companyId}");
        return await SendAsync<List<VendorPricelistDto>>(request, "items");
    }

    public async Task<ApiResult<CreateResponseDto>> SaveVendorPricelistAsync(VendorPricelistDto item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/vendor-pricelists")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> DeleteVendorPricelistAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/procurement/vendor-pricelists/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<List<ReorderingRuleDto>>> GetReorderingRulesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/reordering-rules/company/{companyId}");
        return await SendAsync<List<ReorderingRuleDto>>(request, "items");
    }

    public async Task<ApiResult<CreateResponseDto>> SaveReorderingRuleAsync(ReorderingRuleDto item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/reordering-rules")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> DeleteReorderingRuleAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/procurement/reordering-rules/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<List<ReplenishmentSuggestionDto>>> GetReplenishmentSuggestionsAsync(Guid companyId, Guid? branchId, Guid? warehouseId, Guid? productSkuId)
    {
        var query = new List<string>();
        if (branchId.HasValue)
            query.Add($"branchId={branchId.Value}");
        if (warehouseId.HasValue)
            query.Add($"warehouseId={warehouseId.Value}");
        if (productSkuId.HasValue)
            query.Add($"productSkuId={productSkuId.Value}");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/replenishment/company/{companyId}{suffix}");
        return await SendAsync<List<ReplenishmentSuggestionDto>>(request, "items");
    }

    public async Task<ApiResult<CreateResponseDto>> CreatePurchaseRequestFromReplenishmentAsync(CreatePurchaseRequestFromReplenishmentDto replenishmentRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/replenishment/purchase-requests")
        {
            Content = JsonContent.Create(replenishmentRequest)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<List<ProcurementTrackerRowDto>>> GetTrackerAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/tracker/company/{companyId}");
        return await SendAsync<List<ProcurementTrackerRowDto>>(request, "rows");
    }

    public async Task<ApiResult<List<SupplierScorecardRowDto>>> GetSupplierScorecardAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/supplier-scorecard/company/{companyId}");
        return await SendAsync<List<SupplierScorecardRowDto>>(request, "rows");
    }

    private static string Route(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => "purchase-requests",
            ProcurementDocumentKind.RequestForQuotation => "requests-for-quotation",
            ProcurementDocumentKind.SupplierQuotation => "supplier-quotations",
            ProcurementDocumentKind.PurchaseOrder => "purchase-orders",
            ProcurementDocumentKind.GoodsReceipt => "goods-receipts",
            ProcurementDocumentKind.PurchaseReturn => "purchase-returns",
            ProcurementDocumentKind.SupplierInvoice => "supplier-invoices",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported procurement document kind.")
        };
}
