using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Organization.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class LicenseCategoryService : BaseApiService, ILicenseCategoryService
{
    private readonly string _path;

    public LicenseCategoryService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/organization/license-categories";
    }

    public async Task<ApiResult<List<LicenseCategoryDto>>> GetAsync(bool includeInactive = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?includeInactive={includeInactive}");
        return await SendAsync<List<LicenseCategoryDto>>(request, "categories");
    }

    public async Task<ApiResult<LicenseCategoryDto>> CreateAsync(LicenseCategoryDto category)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { Category = category })
        };
        return await SendAsync<LicenseCategoryDto>(request, "category");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(LicenseCategoryDto category)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new { Category = category })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_path}/{id}/status")
        {
            Content = JsonContent.Create(new { IsActive = isActive })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
