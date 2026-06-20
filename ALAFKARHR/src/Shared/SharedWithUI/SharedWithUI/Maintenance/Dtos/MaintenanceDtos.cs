using System.ComponentModel.DataAnnotations;
using SharedWithUI.Maintenance.Enums;

namespace SharedWithUI.Maintenance.Dtos;

public class MaintenanceAssetDto
{
    public Guid Id { get; set; }
    public string AssetCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "English name is required")]
    public string NameEng { get; set; } = string.Empty;

    public MaintenanceAssetType AssetType { get; set; } = MaintenanceAssetType.Other;
    public MaintenanceAssetStatus Status { get; set; } = MaintenanceAssetStatus.Active;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? ParentAssetId { get; set; }
    public string? ParentAssetName { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntityName { get; set; }
    public Guid? SourceEntityId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMaintenanceAssetDto
{
    public string? AssetCode { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "English name is required")]
    public string NameEng { get; set; } = string.Empty;

    public MaintenanceAssetType AssetType { get; set; } = MaintenanceAssetType.Other;
    public MaintenanceAssetStatus Status { get; set; } = MaintenanceAssetStatus.Active;

    [Required(ErrorMessage = "Company is required")]
    public Guid CompanyId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? ParentAssetId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
}

public class UpdateMaintenanceAssetDto : CreateMaintenanceAssetDto
{
    public Guid Id { get; set; }
}

public class MaintenanceAssetFilterDto
{
    public MaintenanceAssetType? AssetType { get; set; }
    public MaintenanceAssetStatus? Status { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? ParentAssetId { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntityName { get; set; }
    public Guid? SourceEntityId { get; set; }
}

public class MaintenanceWorkOrderDto
{
    public Guid Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public MaintenanceAssetType AssetType { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string? AssignedToUserId { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceWorkOrderStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Category { get; set; }
    public string? InternalNotes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ApprovedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? CurrencyCode { get; set; }
    public string? VendorName { get; set; }
    public Guid? SupplierId { get; set; }
    public MaintenanceCostApprovalStatus CostApprovalStatus { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalNotes { get; set; }
    public List<MaintenanceCommentDto> Comments { get; set; } = [];
    public List<MaintenanceAttachmentDto> Attachments { get; set; } = [];
    public List<MaintenanceHistoryDto> History { get; set; } = [];
}

public class CreateMaintenanceWorkOrderDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Asset is required")]
    public Guid AssetId { get; set; }

    public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;
    public DateTime? DueDate { get; set; }
    public string? Category { get; set; }
    public string? InternalNotes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    [Required(ErrorMessage = "Currency is required")]
    public Guid? CurrencyId { get; set; }
    public string? CurrencyCode { get; set; }
    public string? VendorName { get; set; }
    public Guid? SupplierId { get; set; }
}

public class UpdateMaintenanceWorkOrderDto : CreateMaintenanceWorkOrderDto
{
    public Guid Id { get; set; }
}

public class MaintenanceWorkOrderFilterDto
{
    public Guid? AssetId { get; set; }
    public Guid? BranchId { get; set; }
    public MaintenanceAssetType? AssetType { get; set; }
    public MaintenancePriority? Priority { get; set; }
    public MaintenanceWorkOrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class AssignMaintenanceWorkOrderDto
{
    public string AssignedToUserId { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}

public class ChangeMaintenanceWorkOrderStatusDto
{
    public MaintenanceWorkOrderStatus Status { get; set; }
}

public class ApproveMaintenanceCostDto
{
    public bool IsApproved { get; set; }
    public decimal? ApprovedCost { get; set; }
    public string? ApprovalNotes { get; set; }
}

public class CreateMaintenanceCommentDto
{
    public string Comment { get; set; } = string.Empty;
}

public class MaintenanceCommentDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceAttachmentDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceHistoryDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Guid PerformedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceDashboardDto
{
    public int TotalAssets { get; set; }
    public int ActiveAssets { get; set; }
    public int OpenWorkOrders { get; set; }
    public int InProgressWorkOrders { get; set; }
    public int PendingApprovals { get; set; }
    public int CompletedThisMonth { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
}

public class MaintenanceSummaryReportDto : MaintenanceDashboardDto
{
    public List<MaintenanceStatusSummaryDto> StatusSummary { get; set; } = [];
    public List<MaintenanceAssetTypeSummaryDto> AssetTypeSummary { get; set; } = [];
}

public class MaintenanceStatusSummaryDto
{
    public MaintenanceWorkOrderStatus Status { get; set; }
    public int Count { get; set; }
}

public class MaintenanceAssetTypeSummaryDto
{
    public MaintenanceAssetType AssetType { get; set; }
    public int Count { get; set; }
}
