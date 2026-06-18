namespace Maintenance.WorkOrders.Models;

public class MaintenanceWorkOrder : Aggregate<Guid>
{
    public string WorkOrderNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid AssetId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public string? AssignedToUserId { get; private set; }
    public MaintenancePriority Priority { get; private set; }
    public MaintenanceWorkOrderStatus Status { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Category { get; private set; }
    public string? InternalNotes { get; private set; }
    public decimal? EstimatedCost { get; private set; }
    public decimal? ApprovedCost { get; private set; }
    public decimal? ActualCost { get; private set; }
    public string? CurrencyCode { get; private set; }
    public string? VendorName { get; private set; }
    public Guid? SupplierId { get; private set; }
    public MaintenanceCostApprovalStatus CostApprovalStatus { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovalNotes { get; private set; }

    public MaintenanceAsset Asset { get; private set; } = default!;
    public List<MaintenanceComment> Comments { get; private set; } = [];
    public List<MaintenanceAttachment> Attachments { get; private set; } = [];
    public List<MaintenanceHistory> History { get; private set; } = [];

    private MaintenanceWorkOrder()
    {
    }

    public static MaintenanceWorkOrder Create(
        string workOrderNumber,
        string title,
        string description,
        Guid assetId,
        Guid requestedByUserId,
        MaintenancePriority priority,
        DateTime? dueDate,
        string? category,
        string? internalNotes,
        decimal? estimatedCost,
        decimal? actualCost,
        string? currencyCode,
        string? vendorName,
        Guid? supplierId)
    {
        EnsureRequired(title, assetId);

        return new MaintenanceWorkOrder
        {
            Id = Guid.NewGuid(),
            WorkOrderNumber = workOrderNumber.Trim(),
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            AssetId = assetId,
            RequestedByUserId = requestedByUserId,
            Priority = priority,
            Status = MaintenanceWorkOrderStatus.Open,
            RequestedDate = DateTime.UtcNow,
            DueDate = dueDate,
            Category = category?.Trim(),
            InternalNotes = internalNotes?.Trim(),
            EstimatedCost = estimatedCost,
            ActualCost = actualCost,
            CurrencyCode = currencyCode?.Trim(),
            VendorName = vendorName?.Trim(),
            SupplierId = supplierId,
            CostApprovalStatus = estimatedCost.HasValue && estimatedCost.Value > 0
                ? MaintenanceCostApprovalStatus.Pending
                : MaintenanceCostApprovalStatus.NotRequired,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = requestedByUserId.ToString()
        };
    }

    public void Update(
        string title,
        string description,
        Guid assetId,
        MaintenancePriority priority,
        DateTime? dueDate,
        string? category,
        string? internalNotes,
        decimal? estimatedCost,
        decimal? actualCost,
        string? currencyCode,
        string? vendorName,
        Guid? supplierId,
        Guid modifiedByUserId)
    {
        EnsureRequired(title, assetId);

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        AssetId = assetId;
        Priority = priority;
        DueDate = dueDate;
        Category = category?.Trim();
        InternalNotes = internalNotes?.Trim();
        EstimatedCost = estimatedCost;
        ActualCost = actualCost;
        CurrencyCode = currencyCode?.Trim();
        VendorName = vendorName?.Trim();
        SupplierId = supplierId;
        if (estimatedCost.HasValue && estimatedCost.Value > 0 && CostApprovalStatus == MaintenanceCostApprovalStatus.NotRequired)
            CostApprovalStatus = MaintenanceCostApprovalStatus.Pending;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Assign(string assignedToUserId, DateTime? dueDate, Guid assignedByUserId)
    {
        if (string.IsNullOrWhiteSpace(assignedToUserId))
            throw new BadRequestException("Assigned user is required.");

        AssignedToUserId = assignedToUserId.Trim();
        DueDate = dueDate ?? DueDate;
        Status = MaintenanceWorkOrderStatus.Assigned;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = assignedByUserId.ToString();
    }

    public void ChangeStatus(MaintenanceWorkOrderStatus status, Guid modifiedByUserId)
    {
        Status = status;
        if (status == MaintenanceWorkOrderStatus.InProgress && !StartedAt.HasValue)
            StartedAt = DateTime.UtcNow;
        if (status == MaintenanceWorkOrderStatus.Completed)
            CompletedAt = DateTime.UtcNow;
        if (status is MaintenanceWorkOrderStatus.Cancelled or MaintenanceWorkOrderStatus.Rejected)
            CompletedAt = null;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void ApproveCost(bool isApproved, decimal? approvedCost, string? approvalNotes, Guid approvedByUserId)
    {
        CostApprovalStatus = isApproved ? MaintenanceCostApprovalStatus.Approved : MaintenanceCostApprovalStatus.Rejected;
        ApprovedCost = isApproved ? approvedCost ?? EstimatedCost : null;
        ApprovalNotes = approvalNotes?.Trim();
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        Status = isApproved ? MaintenanceWorkOrderStatus.Approved : MaintenanceWorkOrderStatus.Rejected;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedByUserId.ToString();
    }

    public void AddComment(MaintenanceComment comment) => Comments.Add(comment);
    public void AddAttachment(MaintenanceAttachment attachment) => Attachments.Add(attachment);
    public void AddHistory(MaintenanceHistory history) => History.Add(history);

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private static void EnsureRequired(string title, Guid assetId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BadRequestException("Work order title is required.");
        if (assetId == Guid.Empty)
            throw new BadRequestException("Asset is required.");
    }
}
