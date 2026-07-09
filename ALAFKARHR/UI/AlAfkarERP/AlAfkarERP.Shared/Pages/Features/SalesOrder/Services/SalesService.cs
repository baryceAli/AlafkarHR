using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Sales.Dtos;
using SharedWithUI.SalesOrder.Dtos;
using SharedWithUI.SharedDtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public class SalesService : BaseApiService, ISalesService
{
    private readonly string _path;
    private readonly string _apiVersion;

    public SalesService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiVersion = apiConfig.Version;
        _path = $"api/{apiConfig.Version}/sales";
    }

    public async Task<ApiResult<SalesDashboardDto>> GetDashboardAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/dashboard/company/{companyId}");
        return await SendAsync<SalesDashboardDto>(request, "dashboard");
    }

    public async Task<ApiResult<PaginatedResult<SalesOrderDto>>> GetOrdersByCompanyAsync(Guid companyId, int pageIndex, int pageSize, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/orders/company/{companyId}?{BuildPagedSmartQuery(pageIndex, pageSize, null, customerId, productId, productSkuId)}");
        return await SendAsync<PaginatedResult<SalesOrderDto>>(request, "salesOrders");
    }

    public async Task<ApiResult<SmartLinkSummaryResultDto>> GetOrderSmartLinksAsync(Guid companyId, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/orders/smart-links/company/{companyId}?{BuildSmartQuery(customerId, productId, productSkuId)}");
        return await SendAsync<SmartLinkSummaryResultDto>(request, null);
    }

    public async Task<ApiResult<SalesOrderDto>> GetOrderByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/orders/{id}");
        return await SendAsync<SalesOrderDto>(request, "salesOrder");
    }

    public async Task<ApiResult<CreateManualSalesOrderResponseDto>> CreateManualOrderAsync(CreateManualSalesOrderDto order)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiVersion}/SalesOrders/manual")
        {
            Content = JsonContent.Create(new { SalesOrder = order })
        };
        return await SendAsync<CreateManualSalesOrderResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<SalesQuotationDto>>> GetQuotationsByCompanyAsync(Guid companyId, int pageIndex, int pageSize, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/quotations/company/{companyId}?{BuildPagedSmartQuery(pageIndex, pageSize, null, customerId, productId, productSkuId)}");
        return await SendAsync<PaginatedResult<SalesQuotationDto>>(request, "quotations");
    }

    public async Task<ApiResult<SmartLinkSummaryResultDto>> GetQuotationSmartLinksAsync(Guid companyId, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/quotations/smart-links/company/{companyId}?{BuildSmartQuery(customerId, productId, productSkuId)}");
        return await SendAsync<SmartLinkSummaryResultDto>(request, null);
    }

    public async Task<ApiResult<SalesQuotationDto>> GetQuotationByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/quotations/{id}");
        return await SendAsync<SalesQuotationDto>(request, "quotation");
    }

    public async Task<ApiResult<Guid>> CreateQuotationAsync(SalesQuotationDto quotation)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/quotations")
        {
            Content = JsonContent.Create(new { Quotation = quotation })
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<bool>> UpdateQuotationAsync(SalesQuotationDto quotation)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/quotations")
        {
            Content = JsonContent.Create(new { Quotation = quotation })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> QuotationActionAsync(Guid id, string action, string? reason = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/quotations/{id}/action")
        {
            Content = JsonContent.Create(new { Action = action, Reason = reason })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> SendQuotationAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/quotations/{id}/send");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<Guid?>> ConvertQuotationAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/quotations/{id}/convert");
        return await SendAsync<Guid?>(request, "salesOrderId");
    }

    public async Task<ApiResult<SalesQuotationExpiryResultDto>> ExpireOverdueQuotationsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/quotations/expire-overdue?companyId={companyId}");
        return await SendAsync<SalesQuotationExpiryResultDto>(request, "expiry");
    }

    public async Task<ApiResult<PaginatedResult<SalesDeliveryNoteDto>>> GetDeliveryNotesByCompanyAsync(Guid companyId, int pageIndex, int pageSize, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/delivery-notes/company/{companyId}?{BuildPagedSmartQuery(pageIndex, pageSize, null, customerId, productId, productSkuId)}");
        return await SendAsync<PaginatedResult<SalesDeliveryNoteDto>>(request, "deliveryNotes");
    }

    public async Task<ApiResult<SmartLinkSummaryResultDto>> GetDeliveryNoteSmartLinksAsync(Guid companyId, Guid? customerId = null, Guid? productId = null, Guid? productSkuId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/delivery-notes/smart-links/company/{companyId}?{BuildSmartQuery(customerId, productId, productSkuId)}");
        return await SendAsync<SmartLinkSummaryResultDto>(request, null);
    }

    public async Task<ApiResult<SalesDeliveryNoteDto>> GetDeliveryNoteByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/delivery-notes/{id}");
        return await SendAsync<SalesDeliveryNoteDto>(request, "deliveryNote");
    }

    public async Task<ApiResult<Guid>> CreateDeliveryNoteAsync(SalesDeliveryNoteDto deliveryNote)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/delivery-notes")
        {
            Content = JsonContent.Create(new { DeliveryNote = deliveryNote })
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<bool>> UpdateDeliveryNoteAsync(SalesDeliveryNoteDto deliveryNote)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/delivery-notes")
        {
            Content = JsonContent.Create(new { DeliveryNote = deliveryNote })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> PostDeliveryNoteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/delivery-notes/{id}/post");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> CancelDeliveryNoteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/delivery-notes/{id}/cancel");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<PaginatedResult<SalesReturnDto>>> GetReturnsByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/returns/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<SalesReturnDto>>(request, "returns");
    }

    public async Task<ApiResult<SalesReturnDto>> GetReturnByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/returns/{id}");
        return await SendAsync<SalesReturnDto>(request, "return");
    }

    public async Task<ApiResult<Guid>> CreateReturnAsync(SalesReturnDto salesReturn)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/returns")
        {
            Content = JsonContent.Create(new { Return = salesReturn })
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<bool>> UpdateReturnAsync(SalesReturnDto salesReturn)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/returns")
        {
            Content = JsonContent.Create(new { Return = salesReturn })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> PostReturnAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/returns/{id}/post");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> CancelReturnAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/returns/{id}/cancel");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<SalesSettingsDto>> GetSettingsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/settings/company/{companyId}");
        return await SendAsync<SalesSettingsDto>(request, "settings");
    }

    public async Task<ApiResult<bool>> UpdateSettingsAsync(SalesSettingsDto settings)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/settings")
        {
            Content = JsonContent.Create(new { Settings = settings })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<SalesQuotationTemplateDto>>> GetQuotationTemplatesAsync(Guid companyId, bool activeOnly = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/quotation-templates?companyId={companyId}&activeOnly={activeOnly}");
        return await SendAsync<List<SalesQuotationTemplateDto>>(request, "templates");
    }

    public async Task<ApiResult<Guid>> SaveQuotationTemplateAsync(SalesQuotationTemplateDto template)
    {
        var method = template.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var url = template.Id == Guid.Empty
            ? $"{_path}/quotation-templates"
            : $"{_path}/quotation-templates/{template.Id}";
        var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Template = template })
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<string>> DeleteQuotationTemplateAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/quotation-templates/{id}");
        return await SendAsync<string>(request, null);
    }

    private static string BuildPagedSmartQuery(int pageIndex, int pageSize, string? searchText, Guid? customerId, Guid? productId, Guid? productSkuId)
    {
        var query = new List<string>
        {
            $"PageIndex={pageIndex}",
            $"PageSize={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(searchText))
            query.Add($"searchText={Uri.EscapeDataString(searchText)}");

        AddSmartQueryParts(query, customerId, productId, productSkuId);
        return string.Join("&", query);
    }

    private static string BuildSmartQuery(Guid? customerId, Guid? productId, Guid? productSkuId)
    {
        var query = new List<string>();
        AddSmartQueryParts(query, customerId, productId, productSkuId);
        return string.Join("&", query);
    }

    private static void AddSmartQueryParts(List<string> query, Guid? customerId, Guid? productId, Guid? productSkuId)
    {
        if (customerId.HasValue && customerId.Value != Guid.Empty)
            query.Add($"customerId={customerId.Value}");
        if (productId.HasValue && productId.Value != Guid.Empty)
            query.Add($"productId={productId.Value}");
        if (productSkuId.HasValue && productSkuId.Value != Guid.Empty)
            query.Add($"productSkuId={productSkuId.Value}");
    }
}
