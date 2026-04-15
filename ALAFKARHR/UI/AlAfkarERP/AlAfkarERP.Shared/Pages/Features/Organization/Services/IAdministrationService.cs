using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IAdministrationService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(AdministrationDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AdministrationDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<AdministrationDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PagedResult<AdministrationDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<List<AdministrationDto>>> GetByBranchAsync(Guid branchId);
}
