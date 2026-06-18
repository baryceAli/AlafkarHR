using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Sales.Dtos;
using SharedWithUI.SalesOrder.Dtos;

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
}
