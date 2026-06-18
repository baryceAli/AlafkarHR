using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Orders.Dtos;
using SharedWithUI.Orders.Enums;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public interface IOrderIntakeService
{
    Task<ApiResult<PaginatedResult<OrderIntakeDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, OrderIntakeStatus? status = null, string? searchText = null);
    Task<ApiResult<OrderIntakeDto>> GetByIdAsync(Guid id);
    Task<ApiResult<AcceptOrderIntakeResultDto>> AcceptAsync(Guid id);
    Task<ApiResult<bool>> RejectAsync(Guid id, string reason);
}

public class AcceptOrderIntakeResultDto
{
    public Guid Id { get; set; }
    public Guid SalesOrderId { get; set; }
}
