using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Suppliers.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Suppliers.Services;

public interface ISupplierGroupService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(SupplierGroupDto supplierGroup);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SupplierGroupDto supplierGroup);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<List<SupplierGroupDto>>> GetByCompanyAsync(Guid companyId);
    Task<ApiResult<SupplierGroupDto>> GetByIdAsync(Guid id);
}
