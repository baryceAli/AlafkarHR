using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface ICompanyService
{
    Task<ApiResult<CompanyDto>> CreateAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CompanyDto company);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<CompanyDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PagedResult<CompanyDto>>> GetAsync(int pageIndex, int pageSize);
}
