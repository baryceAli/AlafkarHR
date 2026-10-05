using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface IWebsiteInquiryService
{
    Task<ApiResult<Guid>> CreateAsync(CreateWebsiteInquiryDto inquiry);
}
