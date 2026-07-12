using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;
using SharedWithUI.Organization.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface ICompanyService
{
    Task<ApiResult<CompanyDto>> CreateAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<CompanyDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PagedResult<CompanyDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<PagedResult<CompanyDto>>> GetChildCompaniesAsync(int pageIndex, int pageSize, string? searchText = null);
    Task<ApiResult<OrganizationStructureDto>> GetOrganizationStructureAsync();
    Task<ApiResult<CompanyLicenseSummaryDto>> GetCurrentLicenseAsync();
    Task<ApiResult<CompanyDto>> CreateChildAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateChildAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> SetChildStatusAsync(Guid id, bool isActive);
    Task<ApiResult<UpdateDeleteResponseDto>> ResetChildAdminPasswordAsync(Guid id, string temporaryPassword);
}
