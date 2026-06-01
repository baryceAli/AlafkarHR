using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Customers.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public interface ICustomerGroupService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerGroupDto customerGroup);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerGroupDto customerGroup);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<PaginatedResult<CustomerGroupDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "");
    Task<ApiResult<CustomerGroupDto>> GetByIdAsync(Guid id);
}
