using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Suppliers.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Suppliers.Services;

public interface ISupplierService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(SupplierDto supplier);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SupplierDto supplier);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<SupplierDto>> GetByIdAsync(Guid id);
    Task<ApiResult<PaginatedResult<SupplierDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "");
}
