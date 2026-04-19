using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class SpecializationService : BaseApiService, ISpecializationService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public SpecializationService(HttpClient http,ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Employee/Specializations";
    }

    public async Task<ApiResult<SpecializationDto>> CreateAsync(SpecializationDto specialization)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new 
            { 
                Specialization=specialization
            })
        };
        return await SendAsync<SpecializationDto>(request, "createdSpecialization");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<SpecializationDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<SpecializationDto>>(request, "specializationList");
    }

    public async Task<ApiResult<SpecializationDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<SpecializationDto>(request, "specialization");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SpecializationDto specialization)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Specialization = specialization
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
