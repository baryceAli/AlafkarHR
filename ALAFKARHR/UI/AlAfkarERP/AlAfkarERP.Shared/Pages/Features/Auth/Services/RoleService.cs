using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Auth.Dtos;
using System.ComponentModel.Design;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public class RoleService:BaseApiService, IRoleService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public RoleService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/auth";
    }

    public async Task<ApiResult<RoleDto>> GetByName(string name)
    {
        ///api/v1/auth/roles/{roleName}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/roles/{name}");
        return await SendAsync<RoleDto>(request, "role");
    }

    public async Task<ApiResult<List<RoleDto>>> GetRoles(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/roles/company/{companyId}");
        return await SendAsync<List<RoleDto>>(request, "roleList");
    }

    public Task<ApiResult<List<RoleDto>>> GetRolesByEmployeeId(Guid employeeId, int pageIndex, int pageSize)
    {
        throw new NotImplementedException();
    }
}
