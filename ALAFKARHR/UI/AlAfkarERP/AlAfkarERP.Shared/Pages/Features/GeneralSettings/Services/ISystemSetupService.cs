using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface ISystemSetupService
{
    Task<ApiResult<SetupSummaryDto>> GetCurrentAsync();
    Task<ApiResult<SetupSummaryDto>> UpdateProgressAsync(UpdateSetupProgressDto progress);
}
