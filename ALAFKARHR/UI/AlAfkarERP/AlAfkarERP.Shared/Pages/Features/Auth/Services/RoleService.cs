using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Auth.Dtos;
using System.ComponentModel.Design;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public class RoleService : BaseApiService, IRoleService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public RoleService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/auth";
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> AssignRole(UserRoleDto userRole)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/Users/AssignRole")
        {
            Content = JsonContent.Create(new
            {
                UserRole = userRole
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(RoleDto role)
    {
        ///api/v1/auth/roles/{roleName}
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/roles")
        {
            Content = JsonContent.Create(new
            {
                Role = role
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
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

    public async Task<ApiResult<List<RoleDto>>> GetRolesByUserName(string userName)
    {///api/v1/auth/roles/GetByUserName/{userName}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/roles/GetByUserName/{userName}");
        return await SendAsync<List<RoleDto>>(request, "roleList");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UnassignRole(UserRoleDto removeRolesFromUser)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/Users/unassignRole")
        {
            Content = JsonContent.Create(new
            {
                UserRole = removeRolesFromUser
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
