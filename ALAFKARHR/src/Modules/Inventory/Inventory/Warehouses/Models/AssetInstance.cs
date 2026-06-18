using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class AssetInstance : Aggregate<Guid>
{
    public string AssetTag { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid MaintenanceAssetId { get; private set; }
    public AssetInstanceStatus Status { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public DateTime? WarrantyEndDate { get; private set; }
    public string? Notes { get; private set; }

    private AssetInstance()
    {
    }

    public static AssetInstance Create(
        Guid id,
        string assetTag,
        string? serialNumber,
        Guid productId,
        Guid productSkuId,
        Guid companyId,
        Guid? branchId,
        Guid? departmentId,
        Guid? employeeId,
        Guid? warehouseId,
        Guid maintenanceAssetId,
        AssetInstanceStatus status,
        DateTime? purchaseDate,
        DateTime? warrantyEndDate,
        string? notes,
        string createdBy)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Asset instance id is required.", nameof(id));
        EnsureRequired(assetTag, productId, productSkuId, companyId, maintenanceAssetId);

        return new AssetInstance
        {
            Id = id,
            AssetTag = assetTag.Trim(),
            SerialNumber = serialNumber?.Trim(),
            ProductId = productId,
            ProductSkuId = productSkuId,
            CompanyId = companyId,
            BranchId = branchId,
            DepartmentId = departmentId,
            EmployeeId = employeeId,
            WarehouseId = warehouseId,
            MaintenanceAssetId = maintenanceAssetId,
            Status = status,
            PurchaseDate = purchaseDate,
            WarrantyEndDate = warrantyEndDate,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string assetTag,
        string? serialNumber,
        Guid productId,
        Guid productSkuId,
        Guid companyId,
        Guid? branchId,
        Guid? departmentId,
        Guid? employeeId,
        Guid? warehouseId,
        Guid maintenanceAssetId,
        AssetInstanceStatus status,
        DateTime? purchaseDate,
        DateTime? warrantyEndDate,
        string? notes,
        string modifiedBy)
    {
        EnsureRequired(assetTag, productId, productSkuId, companyId, maintenanceAssetId);

        AssetTag = assetTag.Trim();
        SerialNumber = serialNumber?.Trim();
        ProductId = productId;
        ProductSkuId = productSkuId;
        CompanyId = companyId;
        BranchId = branchId;
        DepartmentId = departmentId;
        EmployeeId = employeeId;
        WarehouseId = warehouseId;
        MaintenanceAssetId = maintenanceAssetId;
        Status = status;
        PurchaseDate = purchaseDate;
        WarrantyEndDate = warrantyEndDate;
        Notes = notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private static void EnsureRequired(string assetTag, Guid productId, Guid productSkuId, Guid companyId, Guid maintenanceAssetId)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag is required.", nameof(assetTag));
        if (productId == Guid.Empty)
            throw new ArgumentException("Product is required.", nameof(productId));
        if (productSkuId == Guid.Empty)
            throw new ArgumentException("Product SKU is required.", nameof(productSkuId));
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company is required.", nameof(companyId));
        if (maintenanceAssetId == Guid.Empty)
            throw new ArgumentException("Maintenance asset is required.", nameof(maintenanceAssetId));
    }
}
