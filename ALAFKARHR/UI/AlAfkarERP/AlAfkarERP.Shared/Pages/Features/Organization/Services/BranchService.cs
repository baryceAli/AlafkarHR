using AlAfkarERP.Shared.Dtos;
//using AlAfkarERP.Shared.Pages.Reuable2;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;
using SharedWithUI.Organization.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class BranchService : BaseApiService, IBranchService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public BranchService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
        _path = $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branches";
    }

    public async Task<ApiResult<BranchDto>> CreateAsync(BranchDto branch)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Branch = branch
            })
        };
        return await SendAsync<BranchDto>(request, "createdBranch");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<BranchDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<BranchDto>>(request, "branchList");
    }

    public async Task<ApiResult<PaginatedResult<BranchDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null)
    {
        var searchQuery = string.IsNullOrWhiteSpace(searchText)
            ? string.Empty
            : $"&SearchText={Uri.EscapeDataString(searchText.Trim())}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/GetByCompanyId/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}{searchQuery}");
        return await SendAsync<PaginatedResult<BranchDto>>(request, "branchList");
    }

    public async Task<ApiResult<BranchDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<BranchDto>(request, "branch");
    }

    public async Task<ApiResult<CurrentUserBranchAccessDto>> GetCurrentUserBranchAccessAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/current?companyId={companyId}");
        return await SendAsync<CurrentUserBranchAccessDto>(request, null);
    }

    public async Task<ApiResult<UserBranchAssignmentsDto>> GetUserBranchAssignmentsAsync(Guid userId, Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/users/{userId}?companyId={companyId}");
        return await SendAsync<UserBranchAssignmentsDto>(request, null);
    }

    public async Task<ApiResult<AssignUserBranchesResultDto>> AssignUserBranchesAsync(AssignUserBranchesDto assignment)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/users/{assignment.UserId}")
        {
            Content = JsonContent.Create(assignment)
        };
        return await SendAsync<AssignUserBranchesResultDto>(request, null);
    }

    public async Task<ApiResult<List<BranchRoleProfileDto>>> GetBranchRoleProfilesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/role-profiles");
        return await SendAsync<List<BranchRoleProfileDto>>(request, "profiles");
    }

    public async Task<ApiResult<CurrentUserBranchRoleAccessDto>> GetCurrentUserBranchRoleAccessAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/current/roles?companyId={companyId}");
        return await SendAsync<CurrentUserBranchRoleAccessDto>(request, null);
    }

    public async Task<ApiResult<List<BranchRoleAssignmentDto>>> GetUserBranchRoleAssignmentsAsync(Guid userId, Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/users/{userId}/roles?companyId={companyId}");
        return await SendAsync<List<BranchRoleAssignmentDto>>(request, "assignments");
    }

    public async Task<ApiResult<List<BranchRoleAssignmentDto>>> GetCompanyBranchRoleAssignmentsAsync(Guid companyId, Guid? branchId = null)
    {
        var branchQuery = branchId.HasValue ? $"&branchId={branchId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/roles?companyId={companyId}{branchQuery}");
        return await SendAsync<List<BranchRoleAssignmentDto>>(request, "assignments");
    }

    public async Task<ApiResult<Guid>> AssignUserBranchRoleAsync(AssignBranchRoleDto assignment)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/users/{assignment.UserId}/roles")
        {
            Content = JsonContent.Create(assignment)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<bool>> RemoveUserBranchRoleAsync(Guid assignmentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branch-access/users/roles/{assignmentId}");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BranchDto branch)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content= JsonContent.Create(new
            {
                Branch=branch
            })
        };

        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
