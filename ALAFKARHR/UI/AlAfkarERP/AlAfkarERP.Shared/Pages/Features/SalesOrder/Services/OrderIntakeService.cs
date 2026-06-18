using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Orders.Dtos;
using SharedWithUI.Orders.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public class OrderIntakeService : BaseApiService, IOrderIntakeService
{
    private readonly string _path;

    public OrderIntakeService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/orders/intakes";
    }

    public async Task<ApiResult<PaginatedResult<OrderIntakeDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, OrderIntakeStatus? status = null, string? searchText = null)
    {
        var uri = $"{_path}/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var result = await SendAsync<PaginatedResult<OrderIntakeDto>>(request, "orders");

        if (!result.IsSuccess || result.Data is null || (status is null && string.IsNullOrWhiteSpace(searchText)))
            return result;

        var rows = result.Data.Data.Where(order =>
            (status is null || order.Status == status) &&
            (string.IsNullOrWhiteSpace(searchText) ||
             order.Number.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
             (order.CustomerName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)))
            .ToList();

        return ApiResult<PaginatedResult<OrderIntakeDto>>.Success(new PaginatedResult<OrderIntakeDto>(pageIndex, pageSize, rows.Count, rows));
    }

    public async Task<ApiResult<OrderIntakeDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<OrderIntakeDto>(request, "order");
    }

    public async Task<ApiResult<AcceptOrderIntakeResultDto>> AcceptAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{id}/accept");
        return await SendAsync<AcceptOrderIntakeResultDto>(request, null);
    }

    public async Task<ApiResult<bool>> RejectAsync(Guid id, string reason)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{id}/reject")
        {
            Content = JsonContent.Create(new { Reason = reason })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }
}
