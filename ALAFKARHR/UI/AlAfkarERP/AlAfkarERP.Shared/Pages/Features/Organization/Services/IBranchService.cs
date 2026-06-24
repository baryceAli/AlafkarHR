using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;
//using AlAfkarERP.Shared.Pages.Reuable2;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IBranchService
{
    Task<ApiResult<BranchDto>> CreateAsync(BranchDto branch);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BranchDto branch);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<BranchDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PaginatedResult<BranchDto>>> GetAsync(int pageIndex, int pageSize);
    //Task<ApiResult<PagedResult<BranchDto>>> GetAsync(Guid companyId ,int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<PaginatedResult<BranchDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null);
    Task<ApiResult<CurrentUserBranchAccessDto>> GetCurrentUserBranchAccessAsync(Guid companyId);
    Task<ApiResult<UserBranchAssignmentsDto>> GetUserBranchAssignmentsAsync(Guid userId, Guid companyId);
    Task<ApiResult<AssignUserBranchesResultDto>> AssignUserBranchesAsync(AssignUserBranchesDto assignment);
    Task<ApiResult<List<BranchRoleProfileDto>>> GetBranchRoleProfilesAsync();
    Task<ApiResult<CurrentUserBranchRoleAccessDto>> GetCurrentUserBranchRoleAccessAsync(Guid companyId);
    Task<ApiResult<List<BranchRoleAssignmentDto>>> GetUserBranchRoleAssignmentsAsync(Guid userId, Guid companyId);
    Task<ApiResult<List<BranchRoleAssignmentDto>>> GetCompanyBranchRoleAssignmentsAsync(Guid companyId, Guid? branchId = null);
    Task<ApiResult<Guid>> AssignUserBranchRoleAsync(AssignBranchRoleDto assignment);
    Task<ApiResult<bool>> RemoveUserBranchRoleAsync(Guid assignmentId);
}
