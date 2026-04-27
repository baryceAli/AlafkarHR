
using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Warehouses.Services;

public interface ICategoryService
{
    //public Task<ApiResult<PaginatedResult<CategoryDto>>> GetAsync(int PageIndex, int PageSize);
    public Task<ApiResult<PaginatedResult<CategoryDto>>> GetByCompanyIdAsync(Guid companyId,int PageIndex, int PageSize);
    public Task<ApiResult<CategoryDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(CategoryDto category);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CategoryDto category);
}
