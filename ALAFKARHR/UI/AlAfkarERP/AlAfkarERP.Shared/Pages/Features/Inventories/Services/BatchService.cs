using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class BatchService : BaseApiService, IBatchService
{
    private readonly ApiConfig _apiConfig;
    private string _path;

    public BatchService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/inventory/batches";
        
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CreateBatchDto createBatch)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Batch = createBatch
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<BatchDto>>> GetAsync(Guid companyId,int PageIndex, int PageSize, string? searchText="")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={PageIndex}&pageSize={PageSize}&searchText={searchText}");
        ///api/v1/inventory/batches/company/{companyId}
        return await SendAsync<PaginatedResult<BatchDto>>(request, "batchList");
    }

    public async Task<ApiResult<BatchDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<BatchDto>(request, "batch");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateBatchDto updateBatch)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Batch = updateBatch
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
