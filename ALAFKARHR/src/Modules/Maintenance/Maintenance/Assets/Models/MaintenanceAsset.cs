namespace Maintenance.Assets.Models;

public class MaintenanceAsset : Aggregate<Guid>
{
    public string AssetCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public MaintenanceAssetType AssetType { get; private set; }
    public MaintenanceAssetStatus Status { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? ParentAssetId { get; private set; }
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public string? SerialNumber { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public DateTime? WarrantyEndDate { get; private set; }

    public MaintenanceAsset? ParentAsset { get; private set; }
    public List<MaintenanceAsset> ChildAssets { get; private set; } = [];

    private MaintenanceAsset()
    {
    }

    public static MaintenanceAsset Create(
        string assetCode,
        string name,
        string nameEng,
        MaintenanceAssetType assetType,
        MaintenanceAssetStatus status,
        Guid companyId,
        Guid? branchId,
        Guid? parentAssetId,
        string? description,
        string? location,
        string? serialNumber,
        DateTime? purchaseDate,
        DateTime? warrantyEndDate,
        Guid createdByUserId)
    {
        EnsureRequired(name, nameEng, companyId);

        return new MaintenanceAsset
        {
            Id = Guid.NewGuid(),
            AssetCode = assetCode.Trim(),
            Name = name.Trim(),
            NameEng = nameEng.Trim(),
            AssetType = assetType,
            Status = status,
            CompanyId = companyId,
            BranchId = branchId,
            ParentAssetId = parentAssetId,
            Description = description?.Trim(),
            Location = location?.Trim(),
            SerialNumber = serialNumber?.Trim(),
            PurchaseDate = purchaseDate,
            WarrantyEndDate = warrantyEndDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public void Update(
        string assetCode,
        string name,
        string nameEng,
        MaintenanceAssetType assetType,
        MaintenanceAssetStatus status,
        Guid companyId,
        Guid? branchId,
        Guid? parentAssetId,
        string? description,
        string? location,
        string? serialNumber,
        DateTime? purchaseDate,
        DateTime? warrantyEndDate,
        Guid modifiedByUserId)
    {
        EnsureRequired(name, nameEng, companyId);
        if (parentAssetId == Id)
            throw new BadRequestException("Asset cannot be its own parent.");

        AssetCode = assetCode.Trim();
        Name = name.Trim();
        NameEng = nameEng.Trim();
        AssetType = assetType;
        Status = status;
        CompanyId = companyId;
        BranchId = branchId;
        ParentAssetId = parentAssetId;
        Description = description?.Trim();
        Location = location?.Trim();
        SerialNumber = serialNumber?.Trim();
        PurchaseDate = purchaseDate;
        WarrantyEndDate = warrantyEndDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private static void EnsureRequired(string name, string nameEng, Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Asset name is required.");
        if (string.IsNullOrWhiteSpace(nameEng))
            throw new BadRequestException("Asset English name is required.");
        if (companyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
    }
}
