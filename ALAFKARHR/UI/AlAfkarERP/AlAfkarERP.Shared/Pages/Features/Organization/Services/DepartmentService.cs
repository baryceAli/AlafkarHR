using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class DepartmentService : BaseApiService, IDepartmentService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public DepartmentService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"{apiConfig.BaseURL}/api/{apiConfig.Version}/organization/departments";
    }

    public async Task<ApiResult<DepartmentDto>> CreateAsync(DepartmentDto department)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content= JsonContent.Create(new 
            {
                Department=department
            })
        };
        return await SendAsync<DepartmentDto>(request, "createdDepartment");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<DepartmentDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<DepartmentDto>>(request, "departmentList");
    }

    public async Task<ApiResult<List<DepartmentDto>>> GetByAdministrationAsync(Guid administrationId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/getByAdministrationId/{administrationId}");
        return await SendAsync<List<DepartmentDto>>(request, "departmentList");
    }

    public async Task<ApiResult<PaginatedResult<DepartmentDto>>> GetByAdministrationAsync(Guid AdministrationId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/getByAdministrationId/{AdministrationId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<DepartmentDto>>(request, "departmentList");
    }

    public async Task<ApiResult<DepartmentDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<DepartmentDto>(request, "department");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(DepartmentDto department)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Department = department
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
