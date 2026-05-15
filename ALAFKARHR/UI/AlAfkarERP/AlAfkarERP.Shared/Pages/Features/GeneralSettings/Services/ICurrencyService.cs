using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface ICurrencyService
{
    public Task<ApiResult<PaginatedResult<CurrencyDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string searchText="");
}
