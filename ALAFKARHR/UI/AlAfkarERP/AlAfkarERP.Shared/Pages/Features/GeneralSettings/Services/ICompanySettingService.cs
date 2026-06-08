using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface ICompanySettingService
{
    Task<ApiResult<CompanySettingDto>> GetAsync(Guid companyId);
    Task<ApiResult<CompanySettingDto>> UpdateAsync(Guid companyId, CompanySettingDto companySetting);
}
