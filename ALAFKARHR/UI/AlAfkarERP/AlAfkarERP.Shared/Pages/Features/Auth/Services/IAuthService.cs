using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IAuthService
{
    Task<ApiResult<LoginResponseDto>> LoginAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    //Task<ApiResult<PaginatedResult<UserDto>>> GetUsersAsync(Guid companyId,int pageIndex, int pageSize);
    //Task<ApiResult<UserDto>> GetUserById(Guid Id);
    //Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId);
    //Task<ApiResult<bool>> AssignUserToRole();

}