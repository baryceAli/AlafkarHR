using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface ILicenseCategoryService
{
    Task<ApiResult<List<LicenseCategoryDto>>> GetAsync(bool includeInactive = false);
    Task<ApiResult<LicenseCategoryDto>> CreateAsync(LicenseCategoryDto category);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(LicenseCategoryDto category);
    Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive);
}
