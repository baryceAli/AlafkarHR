using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.SalesOrder.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public class SalesOrderWorkflowService : BaseApiService, ISalesOrderWorkflowService
{
    private readonly string _path;

    public SalesOrderWorkflowService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}";
    }

    public async Task<ApiResult<bool>> ConfirmAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/SalesOrders/Order/{id}/Confirm");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> DeliverAsync(SalesOrderDto order)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/SalesOrders/Order/Deliver")
        {
            Content = JsonContent.Create(new { SalesOrder = order })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> InvoiceAsync(SalesOrderDto order)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/SalesOrder/Order/Invoice")
        {
            Content = JsonContent.Create(new { SalesOrder = order })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> CancelAsync(Guid id, string reason)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/SalesOrders/Order/Cancel")
        {
            Content = JsonContent.Create(new { Id = id, Reason = reason })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }
}
