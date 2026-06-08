using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface ICompanySettingService
{
    Task<ApiResult<CompanySettingDto>> GetAsync(Guid companyId);
}
