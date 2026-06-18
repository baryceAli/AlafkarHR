namespace Maintenance.Assets.Features;

internal static class MaintenanceAssetMappings
{
    public static MaintenanceAssetDto ToDto(MaintenanceAsset asset)
    {
        return new MaintenanceAssetDto
        {
            Id = asset.Id,
            AssetCode = asset.AssetCode,
            Name = asset.Name,
            NameEng = asset.NameEng,
            AssetType = asset.AssetType,
            Status = asset.Status,
            CompanyId = asset.CompanyId,
            BranchId = asset.BranchId,
            ParentAssetId = asset.ParentAssetId,
            ParentAssetName = asset.ParentAsset == null ? null : asset.ParentAsset.Name,
            Description = asset.Description,
            Location = asset.Location,
            SerialNumber = asset.SerialNumber,
            PurchaseDate = asset.PurchaseDate,
            WarrantyEndDate = asset.WarrantyEndDate,
            CreatedAt = asset.CreatedAt ?? DateTime.UtcNow
        };
    }
}
