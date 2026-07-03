using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public interface IPackageService
{
    public Task<ApiResult<PaginatedResult<ProductPackageDto>>> GetAsync(int PageIndex, int PageSize);
    public Task<ApiResult<PaginatedResult<ProductPackageDto>>> GetByCompanyAsync(Guid companyId, int PageIndex, int PageSize, string? searchText = "", bool includeInactive = false);
    public Task<ApiResult<ProductPackageDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(ProductPackageDto package);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ProductPackageDto package);
}
