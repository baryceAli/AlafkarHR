using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Auth.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public class UserService : BaseApiService, IUserService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public UserService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/auth";
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> AssignRoles(UserRoleDto assignRolesToUser)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/Users/AssignRoles")
        {
            Content = JsonContent.Create(new
            {
                AssignRolesToUser = assignRolesToUser
            })
        };

        return SendAsync<UpdateDeleteResponseDto>(request,null);
    }

    public Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<UserDto>> GetUserById(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<PaginatedResult<UserDto>>> GetUsersByCompany(Guid companyId,int pageIndex, int pageSize)
    {
        ///api/v1/auth/Users/company/{companyId}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/users/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<UserDto>>(request, "userList");

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/auth/Users/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        //return await SendAsync<List<UserDto>>(request, "user");
    }

    
}
