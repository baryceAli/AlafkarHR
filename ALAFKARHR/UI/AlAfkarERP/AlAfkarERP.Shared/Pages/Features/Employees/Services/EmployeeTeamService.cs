using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using SharedWithUI.Employees.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class EmployeeTeamService : BaseApiService, IEmployeeTeamService
{
    private readonly string _path;

    public EmployeeTeamService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/Employee/Teams";
    }

    public Task<ApiResult<PaginatedResult<EmployeeTeamDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null, EmployeeTeamCategory? category = null, bool? isActive = null)
    {
        var url = $"{_path}?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (category.HasValue) url += $"&category={category}";
        if (isActive.HasValue) url += $"&isActive={isActive}";
        return SendAsync<PaginatedResult<EmployeeTeamDto>>(new HttpRequestMessage(HttpMethod.Get, url), "teamList");
    }

    public Task<ApiResult<PaginatedResult<EmployeeTeamDto>>> GetProjectTeamsAsync(Guid companyId)
        => SendAsync<PaginatedResult<EmployeeTeamDto>>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/project?companyId={companyId}"), "teamList");

    public Task<ApiResult<EmployeeTeamDto>> GetByIdAsync(Guid id)
        => SendAsync<EmployeeTeamDto>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}"), "team");

    public Task<ApiResult<EmployeeTeamDto>> CreateAsync(EmployeeTeamDto team)
        => SendAsync<EmployeeTeamDto>(new HttpRequestMessage(HttpMethod.Post, _path) { Content = JsonContent.Create(new { Team = team }) }, "createdTeam");

    public Task<ApiResult<EmployeeTeamDto>> CreateProjectTeamAsync(EmployeeTeamDto team)
        => SendAsync<EmployeeTeamDto>(new HttpRequestMessage(HttpMethod.Post, $"{_path}/project") { Content = JsonContent.Create(new { Team = team }) }, "createdTeam");

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeTeamDto team)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{_path}/{team.Id}") { Content = JsonContent.Create(new { Team = team }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}"), null);
}
