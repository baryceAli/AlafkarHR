using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Procurement.Dtos;
using SharedWithUI.Procurement.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Procurement.Services;

public interface IProcurementService
{
    Task<ApiResult<ProcurementDashboardDto>> GetDashboardAsync(Guid? companyId);
    Task<ApiResult<PaginatedResult<ProcurementDocumentDto>>> GetAsync(ProcurementDocumentKind kind, Guid? companyId, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<ProcurementDocumentDto>> GetByIdAsync(ProcurementDocumentKind kind, Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(ProcurementDocumentDto document);
    Task<ApiResult<string>> UpdateAsync(ProcurementDocumentDto document);
    Task<ApiResult<string>> DeleteAsync(ProcurementDocumentKind kind, Guid id);
    Task<ApiResult<string>> WorkflowAsync(ProcurementDocumentKind kind, Guid id, string action);
}
