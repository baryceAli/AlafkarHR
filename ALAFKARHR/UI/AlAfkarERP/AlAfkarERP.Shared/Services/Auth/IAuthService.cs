using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Auth.Dtos;

namespace AlAfkarERP.Shared.Services.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task<ApiResult<PaginatedResult<UserDto>>> GetAsync(int pageIndex, int pageSize);
}