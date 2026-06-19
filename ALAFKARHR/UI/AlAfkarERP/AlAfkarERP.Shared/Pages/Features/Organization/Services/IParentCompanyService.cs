using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;
using SharedWithUI.Organization.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IParentCompanyService
{
    Task<ApiResult<PagedResult<ParentCompanyDto>>> GetAsync(int pageIndex, int pageSize, string? searchText = null);
    Task<ApiResult<ParentCompanyDto>> GetByIdAsync(Guid id);
    Task<ApiResult<ParentCompanyDto>> CreateAsync(ParentCompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ParentCompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateLicenseAsync(Guid id, CompanyLicenseDto license);
    Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive);
    Task<ApiResult<UpdateDeleteResponseDto>> ResetAdminPasswordAsync(Guid id, string temporaryPassword);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
}
