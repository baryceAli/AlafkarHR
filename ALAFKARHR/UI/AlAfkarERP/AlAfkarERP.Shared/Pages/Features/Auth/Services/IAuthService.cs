using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task<ApiResult<PaginatedResult<UserDto>>> GetUserAsync(int pageIndex, int pageSize);
    Task<ApiResult<UserDto>> GetUserById(Guid Id);
    Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId);

    Task<ApiResult<PaginatedResult<RoleDto>>> GetRoles(int pageIndex, int pageSize);
    Task<ApiResult<PaginatedResult<RoleDto>>> GetRolesByEmployeeId(Guid employeeId,int pageIndex, int pageSize);
}