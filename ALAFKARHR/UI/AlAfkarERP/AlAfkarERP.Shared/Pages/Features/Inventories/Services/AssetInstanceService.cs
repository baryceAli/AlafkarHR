using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class AssetInstanceService : BaseApiService, IAssetInstanceService
{
    private readonly string _path;

    public AssetInstanceService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/inventory/asset-instances";
    }

    public async Task<ApiResult<PaginatedResult<AssetInstanceDto>>> GetAsync(
        int pageIndex,
        int pageSize,
        string? searchText = null,
        Guid? companyId = null,
        Guid? warehouseId = null,
        Guid? productSkuId = null,
        Guid? maintenanceAssetId = null,
        AssetInstanceStatus? status = null)
    {
        var query = $"pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue)
            query += $"&companyId={companyId.Value}";
        if (warehouseId.HasValue)
            query += $"&warehouseId={warehouseId.Value}";
        if (productSkuId.HasValue)
            query += $"&productSkuId={productSkuId.Value}";
        if (maintenanceAssetId.HasValue)
            query += $"&maintenanceAssetId={maintenanceAssetId.Value}";
        if (status.HasValue)
            query += $"&status={status.Value}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?{query}");
        return await SendAsync<PaginatedResult<AssetInstanceDto>>(request, "assetInstances");
    }

    public async Task<ApiResult<CreateAssetInstanceResultDto>> CreateAsync(CreateAssetInstanceDto assetInstance)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { assetInstance })
        };
        return await SendAsync<CreateAssetInstanceResultDto>(request, null);
    }

    public async Task<ApiResult<AssetInstanceActionResultDto>> UpdateAsync(UpdateAssetInstanceDto assetInstance)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new { assetInstance })
        };
        return await SendAsync<AssetInstanceActionResultDto>(request, null);
    }

    public async Task<ApiResult<AssetInstanceActionResultDto>> RetireAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<AssetInstanceActionResultDto>(request, null);
    }
}
