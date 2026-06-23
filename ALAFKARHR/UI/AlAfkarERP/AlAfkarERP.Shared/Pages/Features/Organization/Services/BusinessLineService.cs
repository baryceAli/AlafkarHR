using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Organization.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class BusinessLineService : BaseApiService, IBusinessLineService
{
    private readonly string _path;

    public BusinessLineService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/organization/business-lines";
    }

    public async Task<ApiResult<List<BusinessLineDto>>> GetAsync(bool includeInactive = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?includeInactive={includeInactive}");
        return await SendAsync<List<BusinessLineDto>>(request, "businessLines");
    }

    public async Task<ApiResult<BusinessLineDto>> CreateAsync(BusinessLineDto dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(dto)
        };

        return await SendAsync<BusinessLineDto>(request, "businessLine");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BusinessLineDto dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{dto.Id}")
        {
            Content = JsonContent.Create(dto)
        };

        return await SendAsync<UpdateDeleteResponseDto>(request, "result");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{id}/status")
        {
            Content = JsonContent.Create(isActive)
        };

        return await SendAsync<UpdateDeleteResponseDto>(request, "result");
    }
}
