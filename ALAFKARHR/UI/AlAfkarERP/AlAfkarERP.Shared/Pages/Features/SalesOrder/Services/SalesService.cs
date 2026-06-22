using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Sales.Dtos;
using SharedWithUI.SalesOrder.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public class SalesService : BaseApiService, ISalesService
{
    private readonly string _path;

    public SalesService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/sales";
    }

    public async Task<ApiResult<SalesDashboardDto>> GetDashboardAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/dashboard/company/{companyId}");
        return await SendAsync<SalesDashboardDto>(request, "dashboard");
    }

    public async Task<ApiResult<PaginatedResult<SalesOrderDto>>> GetOrdersByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/orders/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<SalesOrderDto>>(request, "salesOrders");
    }

    public async Task<ApiResult<SalesOrderDto>> GetOrderByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/orders/{id}");
        return await SendAsync<SalesOrderDto>(request, "salesOrder");
    }

    public async Task<ApiResult<PaginatedResult<SalesQuotationDto>>> GetQuotationsByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/quotations/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<SalesQuotationDto>>(request, "quotations");
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

    public async Task<ApiResult<Guid?>> ConvertQuotationAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/quotations/{id}/action")
        {
            Content = JsonContent.Create(new { Action = "convert", Reason = (string?)null })
        };
        return await SendAsync<Guid?>(request, "salesOrderId");
    }

    public async Task<ApiResult<PaginatedResult<SalesDeliveryNoteDto>>> GetDeliveryNotesByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/delivery-notes/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<SalesDeliveryNoteDto>>(request, "deliveryNotes");
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
}
