using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface ICurrencyService
{
    public Task<ApiResult<PaginatedResult<CurrencyDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string searchText="");
    Task<ApiResult<CurrencyDto>> CreateAsync(Guid companyId, CurrencyDto currency);
    Task<ApiResult<CurrencyDto>> UpdateAsync(Guid companyId, Guid currencyId, CurrencyDto currency);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid companyId, Guid currencyId);
}
