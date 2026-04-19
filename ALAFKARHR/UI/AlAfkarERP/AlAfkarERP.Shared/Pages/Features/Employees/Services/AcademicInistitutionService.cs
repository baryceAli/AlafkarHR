using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using System.ComponentModel.Design;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class AcademicInistitutionService : BaseApiService, IAcademicInistitutionService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public AcademicInistitutionService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Employee/AcademicInstitutions";
    }

    public async Task<ApiResult<AcademicInstitutionDto>> CreateAsync(AcademicInstitutionDto academicInstitution)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                AcademicInstitution = academicInstitution
            })
        };
        return await SendAsync<AcademicInstitutionDto>(request, "createdAcademicInstitution");
    }//                                                          CreatedAcademicInstitution

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<AcademicInstitutionDto>>> GetByCompanyAsync(Guid companiId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companiId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<AcademicInstitutionDto>>(request, "academicInstitutionList");
    }

    public async Task<ApiResult<AcademicInstitutionDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<AcademicInstitutionDto>(request, "academicInstitution");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AcademicInstitutionDto academicInstitution)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                AcademicInstitution = academicInstitution
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
