using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Pricing.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public interface IPriceListService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(PriceListDto priceList);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(PriceListDto priceList);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<PriceListDto>> GetByIdAsync(Guid id);
    Task<ApiResult<PaginatedResult<PriceListDto>>> GetByCompanyId(Guid companyId, int pageIndex, int pageSize);
}
