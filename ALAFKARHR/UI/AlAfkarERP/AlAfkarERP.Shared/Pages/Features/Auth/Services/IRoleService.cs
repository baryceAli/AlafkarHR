using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IRoleService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(RoleDto role);
    Task<ApiResult<List<RoleDto>>> GetRoles(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<RoleDto>> GetByName(string name);
    Task<ApiResult<List<RoleDto>>> GetRolesByEmployeeId(Guid employeeId, int pageIndex, int pageSize);
}
