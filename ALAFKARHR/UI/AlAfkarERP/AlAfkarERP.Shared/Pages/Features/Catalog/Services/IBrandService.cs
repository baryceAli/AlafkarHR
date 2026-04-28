
using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public interface IBrandService
{
    public Task<ApiResult<PaginatedResult<BrandDto>>> GetAsync(int PageIndex, int PageSize);
    public Task<ApiResult<BrandDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(BrandDto brand);
    public Task<ApiResult<PaginatedResult<BrandDto>>> GetByCompanyIdAsync(Guid companyId,int pageIndex, int pageSize);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BrandDto brand);
}
