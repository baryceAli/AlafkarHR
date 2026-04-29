using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;



public interface IVariantService
{

    Task<ApiResult<CreateResponseDto>> CreateAsync(VariantDto variant);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(VariantDto variant);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<VariantDto>> GetByIdAsync(Guid id);
    Task<ApiResult<PaginatedResult<VariantDto>>> GetByCompanyAsync(Guid companyId,int pageIndex, int pageSize);
    //Task<ApiResult<List<VariantDto>>> GetByProductAsync(Guid producId);
}
