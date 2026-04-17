using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IAdministrationService
{
    Task<ApiResult<AdministrationDto>> CreateAsync(AdministrationDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AdministrationDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<AdministrationDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PaginatedResult<AdministrationDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<PaginatedResult<AdministrationDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize);
}
