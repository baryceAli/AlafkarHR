using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Customers.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public interface ICustomerService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerDto customer);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerDto customer);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<CustomerDto>> GetByIdAsync(Guid id);
    Task<ApiResult<PaginatedResult<CustomerDto>>> GetByCompany(Guid companyId, int pageIndex, int pageSize, string searchText="");
}
