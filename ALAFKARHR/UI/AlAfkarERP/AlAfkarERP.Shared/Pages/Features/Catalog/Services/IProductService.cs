
using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public interface IProductService
{
    public Task<ApiResult<PaginatedResult<ProductDto>>> GetAsync(Guid CategoryId, int PageIndex, int PageSize);
    public Task<ApiResult<PaginatedResult<ProductDto>>> GetByCompanyAsync(Guid companyId, int PageIndex, int PageSize);
    public Task<ApiResult<PaginatedResult<ProductDto>>> GetAsync( int PageIndex, int PageSize);
    public Task<ApiResult<ProductDto>> GetByIdAsync(Guid productId);
    public Task<ApiResult<ProductSkuDto>> GetProductSkuByIdAsync(Guid productSkuId);
    
    public Task<ApiResult<CreateResponseDto>> CreateAsync(ProductDto product);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ProductDto product);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    
    public Task<ApiResult<List<ProductDto>>> GetBySKUIds(List<Guid> skuIds);
    

    public Task<ApiResult<CreateResponseDto>> AddProductSkuAsync(ProductSkuDto productSku);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateProductSkuAsync(ProductSkuDto productSku);
    public Task<ApiResult<UpdateDeleteResponseDto>> RemoveProductSkuAsync(Guid id);

    public Task<ApiResult<PaginatedResult<ProductDto>>> SearchProductsAsync(string searchTerm, int page, int size);

}
