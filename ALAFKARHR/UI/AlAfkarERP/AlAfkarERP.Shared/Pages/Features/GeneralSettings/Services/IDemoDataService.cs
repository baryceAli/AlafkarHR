using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface IDemoDataService
{
    Task<ApiResult<List<DemoDataSummaryDto>>> ListAsync();
    Task<ApiResult<DemoDataStatusDto>> GetStatusAsync(string companyCode);
    Task<ApiResult<DemoDataOperationResultDto>> CreateAsync(DemoDataCreateRequestDto request);
    Task<ApiResult<DemoDataOperationResultDto>> ResetAsync(string companyCode);
    Task<ApiResult<DemoDataOperationResultDto>> DeleteAsync(string companyCode);
    Task<ApiResult<DemoDataOperationResultDto>> ResetAdminPasswordAsync(string companyCode);
}
