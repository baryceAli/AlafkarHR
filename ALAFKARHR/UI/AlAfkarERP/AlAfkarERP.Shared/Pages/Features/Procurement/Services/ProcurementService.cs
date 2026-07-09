using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Procurement.Dtos;
using SharedWithUI.Procurement.Enums;
using SharedWithUI.SharedDtos;
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

    public async Task<ApiResult<PaginatedResult<ProcurementDocumentDto>>> GetAsync(ProcurementDocumentKind kind, Guid? companyId, int pageIndex, int pageSize, string? searchText, Guid? supplierId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var query = BuildSmartQuery(supplierId, productId, productSkuId);
        query.Insert(0, $"companyId={companyId}");
        query.Insert(1, $"pageIndex={pageIndex}");
        query.Insert(2, $"pageSize={pageSize}");
        if (!string.IsNullOrWhiteSpace(searchText))
            query.Add($"searchText={Uri.EscapeDataString(searchText)}");

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/{Route(kind)}?{string.Join("&", query)}");
        return await SendAsync<PaginatedResult<ProcurementDocumentDto>>(request, "documents");
    }

    public async Task<ApiResult<SmartLinkSummaryResultDto>> GetSmartLinksAsync(Guid companyId, Guid? supplierId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var query = BuildSmartQuery(supplierId, productId, productSkuId);
        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/smart-links/company/{companyId}{suffix}");
        return await SendAsync<SmartLinkSummaryResultDto>(request, null);
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

    public async Task<ApiResult<ProcurementRecomputeResultDto>> RecomputePurchaseControlsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/recompute-purchase-controls?companyId={companyId}");
        return await SendAsync<ProcurementRecomputeResultDto>(request, "recompute");
    }

    public async Task<ApiResult<List<SupplierItemDto>>> GetSupplierItemsAsync(Guid companyId, Guid? supplierId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/supplier-items/company/{companyId}{BuildSmartSuffix(supplierId, productId, productSkuId)}");
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

    public async Task<ApiResult<List<VendorPricelistDto>>> GetVendorPricelistsAsync(Guid companyId, Guid? supplierId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/vendor-pricelists/company/{companyId}{BuildSmartSuffix(supplierId, productId, productSkuId)}");
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

    public async Task<ApiResult<List<ReorderingRuleDto>>> GetReorderingRulesAsync(Guid companyId, Guid? supplierId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/reordering-rules/company/{companyId}{BuildSmartSuffix(supplierId, productId, productSkuId)}");
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

    public async Task<ApiResult<List<ReplenishmentSuggestionDto>>> GetReplenishmentSuggestionsAsync(Guid companyId, Guid? branchId, Guid? warehouseId, Guid? productSkuId, ReplenishmentTriggerMode? triggerMode = null, bool includeAutomatic = false, bool orderToMax = false)
    {
        var query = new List<string>();
        if (branchId.HasValue)
            query.Add($"branchId={branchId.Value}");
        if (warehouseId.HasValue)
            query.Add($"warehouseId={warehouseId.Value}");
        if (productSkuId.HasValue)
            query.Add($"productSkuId={productSkuId.Value}");
        if (triggerMode.HasValue)
            query.Add($"triggerMode={triggerMode.Value}");
        if (includeAutomatic)
            query.Add("includeAutomatic=true");
        if (orderToMax)
            query.Add("orderToMax=true");

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

    public async Task<ApiResult<List<SupplierScorecardRowDto>>> GetSupplierScorecardAsync(Guid companyId, Guid? supplierId = null)
    {
        var suffix = supplierId.HasValue && supplierId.Value != Guid.Empty ? $"?supplierId={supplierId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/supplier-scorecard/company/{companyId}{suffix}");
        return await SendAsync<List<SupplierScorecardRowDto>>(request, "rows");
    }

    public async Task<ApiResult<List<ProcurementAgreementDto>>> GetPurchaseAgreementsAsync(Guid companyId, ProcurementAgreementType? type = null, Guid? branchId = null)
    {
        var query = new List<string> { $"companyId={companyId}" };
        if (type.HasValue)
            query.Add($"type={type.Value}");
        if (branchId.HasValue)
            query.Add($"branchId={branchId.Value}");

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/procurement/purchase-agreements?{string.Join("&", query)}");
        return await SendAsync<List<ProcurementAgreementDto>>(request, "agreements");
    }

    public async Task<ApiResult<CreateResponseDto>> SavePurchaseAgreementAsync(ProcurementAgreementDto agreement)
    {
        var method = agreement.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var url = agreement.Id == Guid.Empty
            ? $"api/{_apiConfig.Version}/procurement/purchase-agreements"
            : $"api/{_apiConfig.Version}/procurement/purchase-agreements/{agreement.Id}";
        var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Agreement = agreement })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<RunAutomaticReplenishmentResultDto>> RunAutomaticReplenishmentAsync(Guid companyId, Guid? branchId = null, Guid? warehouseId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/replenishment/automatic")
        {
            Content = JsonContent.Create(new RunAutomaticReplenishmentDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                WarehouseId = warehouseId
            })
        };
        return await SendAsync<RunAutomaticReplenishmentResultDto>(request, "result");
    }

    public async Task<ApiResult<string>> DeletePurchaseAgreementAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/procurement/purchase-agreements/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> PurchaseAgreementActionAsync(Guid id, string action)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/procurement/purchase-agreements/{id}/{action}");
        return await SendAsync<string>(request, null);
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

    private static string BuildSmartSuffix(Guid? supplierId, Guid? productId, Guid? productSkuId)
    {
        var query = BuildSmartQuery(supplierId, productId, productSkuId);
        return query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
    }

    private static List<string> BuildSmartQuery(Guid? supplierId, Guid? productId, Guid? productSkuId)
    {
        var query = new List<string>();
        if (supplierId.HasValue && supplierId.Value != Guid.Empty)
            query.Add($"supplierId={supplierId.Value}");
        if (productId.HasValue && productId.Value != Guid.Empty)
            query.Add($"productId={productId.Value}");
        if (productSkuId.HasValue && productSkuId.Value != Guid.Empty)
            query.Add($"productSkuId={productSkuId.Value}");
        return query;
    }
}
