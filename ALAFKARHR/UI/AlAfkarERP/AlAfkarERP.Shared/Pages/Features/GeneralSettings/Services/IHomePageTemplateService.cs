using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface IHomePageTemplateService
{
    Task<ApiResult<HomePageTemplateDto>> GetPublicAsync(Guid companyId);
    Task<ApiResult<HomePageTemplateDto>> GetAdminAsync(Guid companyId);
    Task<ApiResult<HomePageTemplateDto>> UpdateActiveTemplateAsync(Guid companyId, string activeTemplateKey);
    Task<ApiResult<HomePageTemplateDto>> UpdateContentAsync(Guid companyId, string templateKey, List<HomePageContentItemDto> contentItems);
}

