using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IBranchService
{
    Task<ApiResult<BranchDto>> CreateAsync(BranchDto branch);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BranchDto branch);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<BranchDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PagedResult<BranchDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<List<BranchDto>>> GetByCompanyIdAsync(Guid companyId);
}
