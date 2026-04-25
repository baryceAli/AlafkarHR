using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IRoleService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(RoleDto role);
    Task<ApiResult<List<RoleDto>>> GetRoles(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<RoleDto>> GetByName(string name);

    Task<ApiResult<List<RoleDto>>> GetRolesByUserName(string userName);
    Task<ApiResult<UpdateDeleteResponseDto>> AssignRole(UserRoleDto assignRolesToUser);
    Task<ApiResult<UpdateDeleteResponseDto>> UnassignRole(UserRoleDto removeRolesFromUser);

}
