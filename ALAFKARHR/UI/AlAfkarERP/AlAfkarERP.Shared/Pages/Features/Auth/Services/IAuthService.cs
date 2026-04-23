using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task<ApiResult<PaginatedResult<UserDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<UserDto>> GetById(Guid Id);
    Task<ApiResult<UserDto>> GetByEmployeeId(Guid employeeId);
}