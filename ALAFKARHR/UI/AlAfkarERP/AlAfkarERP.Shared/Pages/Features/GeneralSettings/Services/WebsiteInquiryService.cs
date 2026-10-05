using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class WebsiteInquiryService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
    : BaseApiService(http, tokenService, apiConfig), IWebsiteInquiryService
{
    public Task<ApiResult<Guid>> CreateAsync(CreateWebsiteInquiryDto inquiry) => SendAsync<Guid>(new HttpRequestMessage(HttpMethod.Post, $"api/{apiConfig.Version}/Settings/public/inquiries") { Content = JsonContent.Create(inquiry) }, "inquiryId");
}
