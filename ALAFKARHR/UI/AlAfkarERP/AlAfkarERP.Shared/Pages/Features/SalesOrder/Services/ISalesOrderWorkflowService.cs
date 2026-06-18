using AlAfkarERP.Shared.Dtos;
using SharedWithUI.SalesOrder.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public interface ISalesOrderWorkflowService
{
    Task<ApiResult<bool>> DeliverAsync(SalesOrderDto order);
    Task<ApiResult<bool>> InvoiceAsync(SalesOrderDto order);
    Task<ApiResult<bool>> CancelAsync(Guid id, string reason);
}
