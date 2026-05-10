using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class WarehouseService : BaseApiService, IWarehouseService
{
    private readonly ApiConfig _apiConfig;
    private readonly string path;
    public WarehouseService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        path = $"api/{_apiConfig.Version}/inventory/warehouses";
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<WarehouseDto>>> GetAsync(Guid companyId,int PageIndex, int PageSize,string searchText="")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{path}/company/{companyId}?PageIndex={PageIndex}&PageSize={PageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<WarehouseDto>>(request, "warehouseList");
    }

    public async Task<ApiResult<WarehouseDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{path}/{id}");
        return await SendAsync<WarehouseDto>(request, "warehouse");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(WarehouseDto warehouse)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(new
            {
                Warehouse = warehouse
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(WarehouseDto warehouse)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{path}")
        {
            Content = JsonContent.Create(new
            {
                Warehouse = warehouse
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
