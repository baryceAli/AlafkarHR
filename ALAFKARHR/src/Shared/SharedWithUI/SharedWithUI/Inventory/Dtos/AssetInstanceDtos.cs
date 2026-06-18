using System.ComponentModel.DataAnnotations;
using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public class AssetInstanceDto
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public string? ProductSkuName { get; set; }
    public string? ProductSkuNameEng { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid MaintenanceAssetId { get; set; }
    public AssetInstanceStatus Status { get; set; } = AssetInstanceStatus.Available;
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAssetInstanceDto
{
    public string? AssetTag { get; set; }
    public string? SerialNumber { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Product SKU is required")]
    public Guid ProductSkuId { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public Guid CompanyId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? WarehouseId { get; set; }
    public AssetInstanceStatus Status { get; set; } = AssetInstanceStatus.Available;
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAssetInstanceDto : CreateAssetInstanceDto
{
    public Guid Id { get; set; }
}

public class AssetInstanceFilterDto
{
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? MaintenanceAssetId { get; set; }
    public AssetInstanceStatus? Status { get; set; }
}
