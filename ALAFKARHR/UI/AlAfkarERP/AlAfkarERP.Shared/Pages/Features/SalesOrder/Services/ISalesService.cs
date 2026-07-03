using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Sales.Dtos;
using SharedWithUI.SalesOrder.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public interface ISalesService
{
    Task<ApiResult<SalesDashboardDto>> GetDashboardAsync(Guid companyId);
    Task<ApiResult<PaginatedResult<SalesOrderDto>>> GetOrdersByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<SalesOrderDto>> GetOrderByIdAsync(Guid id);
    Task<ApiResult<CreateManualSalesOrderResponseDto>> CreateManualOrderAsync(CreateManualSalesOrderDto order);
    Task<ApiResult<PaginatedResult<SalesQuotationDto>>> GetQuotationsByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<SalesQuotationDto>> GetQuotationByIdAsync(Guid id);
    Task<ApiResult<Guid>> CreateQuotationAsync(SalesQuotationDto quotation);
    Task<ApiResult<bool>> UpdateQuotationAsync(SalesQuotationDto quotation);
    Task<ApiResult<bool>> QuotationActionAsync(Guid id, string action, string? reason = null);
    Task<ApiResult<bool>> SendQuotationAsync(Guid id);
    Task<ApiResult<Guid?>> ConvertQuotationAsync(Guid id);
    Task<ApiResult<PaginatedResult<SalesDeliveryNoteDto>>> GetDeliveryNotesByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<SalesDeliveryNoteDto>> GetDeliveryNoteByIdAsync(Guid id);
    Task<ApiResult<Guid>> CreateDeliveryNoteAsync(SalesDeliveryNoteDto deliveryNote);
    Task<ApiResult<bool>> UpdateDeliveryNoteAsync(SalesDeliveryNoteDto deliveryNote);
    Task<ApiResult<bool>> PostDeliveryNoteAsync(Guid id);
    Task<ApiResult<bool>> CancelDeliveryNoteAsync(Guid id);
    Task<ApiResult<PaginatedResult<SalesReturnDto>>> GetReturnsByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<SalesReturnDto>> GetReturnByIdAsync(Guid id);
    Task<ApiResult<Guid>> CreateReturnAsync(SalesReturnDto salesReturn);
    Task<ApiResult<bool>> UpdateReturnAsync(SalesReturnDto salesReturn);
    Task<ApiResult<bool>> PostReturnAsync(Guid id);
    Task<ApiResult<bool>> CancelReturnAsync(Guid id);
    Task<ApiResult<SalesSettingsDto>> GetSettingsAsync(Guid companyId);
    Task<ApiResult<bool>> UpdateSettingsAsync(SalesSettingsDto settings);
}

public class CreateManualSalesOrderResponseDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
}
