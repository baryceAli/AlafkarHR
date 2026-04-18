using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class PositionService : BaseApiService, IPositionService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public PositionService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/employee/positions";
    }

    public async Task<ApiResult<PositionDto>> CreateAsync(PositionDto position)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Position=position
            })
        };
        return await SendAsync<PositionDto>(request, "createdPosition");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<PositionDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<PositionDto>>(request, "positionList");
    }

    public async Task<ApiResult<PositionDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<PositionDto>(request, "position");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(PositionDto position)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                Position = position
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
