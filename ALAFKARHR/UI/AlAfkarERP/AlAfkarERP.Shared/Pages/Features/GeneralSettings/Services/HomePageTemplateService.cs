using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class HomePageTemplateService : BaseApiService, IHomePageTemplateService
{
    private readonly string _path;

    public HomePageTemplateService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
        : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/Settings";
    }

    public async Task<ApiResult<HomePageTemplateDto>> GetPublicAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/public/homepage?companyId={companyId}");
        return await SendAsync<HomePageTemplateDto>(request, "homePage");
    }

    public async Task<ApiResult<HomePageTemplateDto>> GetAdminAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}/homepage");
        return await SendAsync<HomePageTemplateDto>(request, "homePage");
    }

    public async Task<ApiResult<HomePageTemplateDto>> UpdateActiveTemplateAsync(Guid companyId, string activeTemplateKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/company/{companyId}/homepage/active-template")
        {
            Content = JsonContent.Create(new
            {
                ActiveTemplate = new UpdateHomePageActiveTemplateDto
                {
                    ActiveTemplateKey = activeTemplateKey
                }
            })
        };

        return await SendAsync<HomePageTemplateDto>(request, "homePage");
    }

    public async Task<ApiResult<HomePageTemplateDto>> UpdateContentAsync(Guid companyId, string templateKey, List<HomePageContentItemDto> contentItems)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/company/{companyId}/homepage/{templateKey}/content")
        {
            Content = JsonContent.Create(new
            {
                ContentItems = contentItems
            })
        };

        return await SendAsync<HomePageTemplateDto>(request, "homePage");
    }
}

