using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IUserService
{
    Task<ApiResult<PaginatedResult<UserDto>>> GetUsersByCompany(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<UpdateDeleteResponseDto>> AssignRoles(UserRoleDto assignRolesToUser);

    //Task<ApiResult<PaginatedResult<UserDto>>> GetUsersAsync(Guid companyId,int pageIndex, int pageSize);
    Task<ApiResult<UserDto>> GetUserById(Guid Id);
    Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId);
    //Task<ApiResult<bool>> AssignUserToRole();

}
