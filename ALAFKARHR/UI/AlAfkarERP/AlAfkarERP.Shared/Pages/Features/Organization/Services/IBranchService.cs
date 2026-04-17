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
    Task<ApiResult<PaginatedResult<BranchDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize);
}
