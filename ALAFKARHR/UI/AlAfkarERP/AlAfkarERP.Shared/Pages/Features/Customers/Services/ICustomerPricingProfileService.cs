using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Customers.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public interface ICustomerPricingProfileService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerPricingProfileDto customerPricingProfile);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerPricingProfileDto customerPricingProfile);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<CustomerPricingProfileDto>> GetByIdAsync(Guid id);
    Task<ApiResult<PaginatedResult<CustomerPricingProfileDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "");
}
