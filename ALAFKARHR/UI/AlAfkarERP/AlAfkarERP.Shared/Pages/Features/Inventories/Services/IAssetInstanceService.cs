using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IAssetInstanceService
{
    Task<ApiResult<PaginatedResult<AssetInstanceDto>>> GetAsync(
        int pageIndex,
        int pageSize,
        string? searchText = null,
        Guid? companyId = null,
        Guid? warehouseId = null,
        Guid? productSkuId = null,
        Guid? maintenanceAssetId = null,
        AssetInstanceStatus? status = null);

    Task<ApiResult<CreateAssetInstanceResultDto>> CreateAsync(CreateAssetInstanceDto assetInstance);
    Task<ApiResult<AssetInstanceActionResultDto>> UpdateAsync(UpdateAssetInstanceDto assetInstance);
    Task<ApiResult<AssetInstanceActionResultDto>> RetireAsync(Guid id);
}

public record CreateAssetInstanceResultDto(Guid Id, Guid MaintenanceAssetId);
public record AssetInstanceActionResultDto(bool IsSuccess);
