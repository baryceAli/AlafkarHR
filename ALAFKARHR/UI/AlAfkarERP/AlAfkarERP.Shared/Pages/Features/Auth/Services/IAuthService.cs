using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IAuthService
{
    Task<ApiResult<LoginResponseDto>> LoginAsync(string email, string password, bool rememberDevice);
    Task<ApiResult<Guid>> RegisterAsync(RegisterDto register);
    Task<ApiResult<bool>> GenerateResetPasswordOtpAsync(string userIdentifier);
    Task<ApiResult<bool>> ConfirmOtpAsync(string userIdentifier, string otp);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    //Task<ApiResult<PaginatedResult<UserDto>>> GetUsersAsync(Guid companyId,int pageIndex, int pageSize);
    //Task<ApiResult<UserDto>> GetUserById(Guid Id);
    //Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId);
    //Task<ApiResult<bool>> AssignUserToRole();

}
