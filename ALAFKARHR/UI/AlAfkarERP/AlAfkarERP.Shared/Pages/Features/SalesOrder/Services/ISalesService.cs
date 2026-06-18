using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Sales.Dtos;
using SharedWithUI.SalesOrder.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public interface ISalesService
{
    Task<ApiResult<SalesDashboardDto>> GetDashboardAsync(Guid companyId);
    Task<ApiResult<PaginatedResult<SalesOrderDto>>> GetOrdersByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<SalesOrderDto>> GetOrderByIdAsync(Guid id);
}
