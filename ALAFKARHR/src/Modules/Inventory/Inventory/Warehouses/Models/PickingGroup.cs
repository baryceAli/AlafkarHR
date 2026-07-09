namespace Inventory.Warehouses.Models;

public class PickingGroup : Aggregate<Guid>
{
    private readonly List<PickingGroupLine> _lines = [];

    private PickingGroup() { }

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public PickingGroupType GroupType { get; private set; }
    public PickingGroupStatus Status { get; private set; }
    public string GroupNumber { get; private set; } = string.Empty;
    public string? Name { get; private set; }
    public string? ResponsibleUserId { get; private set; }
    public string? DockLocation { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public IReadOnlyCollection<PickingGroupLine> Lines => _lines.AsReadOnly();
    public decimal PlannedQuantity => _lines.Sum(x => x.PlannedQuantity);
    public decimal DoneQuantity => _lines.Sum(x => x.DoneQuantity);

    public static PickingGroup Create(CreatePickingGroupDto dto, string groupNumber, IEnumerable<InventoryOperation> operations, string userId)
    {
        var group = new PickingGroup
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            WarehouseId = dto.WarehouseId,
            GroupType = dto.GroupType,
            Status = PickingGroupStatus.Ready,
            GroupNumber = groupNumber,
            Name = string.IsNullOrWhiteSpace(dto.Name) ? null : dto.Name.Trim(),
            ResponsibleUserId = string.IsNullOrWhiteSpace(dto.ResponsibleUserId) ? null : dto.ResponsibleUserId.Trim(),
            DockLocation = string.IsNullOrWhiteSpace(dto.DockLocation) ? null : dto.DockLocation.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        group._lines.AddRange(operations.Select(PickingGroupLine.Create));
        return group;
    }

    public void MarkProcessed(string userId)
    {
        if (Status == PickingGroupStatus.Processed)
            throw new BadRequestException("Picking group is already processed.");
        if (Status == PickingGroupStatus.Cancelled)
            throw new BadRequestException("Cancelled picking groups cannot be processed.");

        Status = PickingGroupStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public PickingGroupDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        BranchId = BranchId,
        WarehouseId = WarehouseId,
        GroupType = GroupType,
        Status = Status,
        GroupNumber = GroupNumber,
        Name = Name,
        ResponsibleUserId = ResponsibleUserId,
        DockLocation = DockLocation,
        Notes = Notes,
        PlannedQuantity = PlannedQuantity,
        DoneQuantity = DoneQuantity,
        CreatedAt = CreatedAt ?? DateTime.UtcNow,
        ProcessedAt = ProcessedAt,
        Lines = _lines.Select(x => x.ToDto()).ToList()
    };
}

public class PickingGroupLine : Entity<Guid>
{
    private PickingGroupLine() { }

    public Guid PickingGroupId { get; private set; }
    public Guid InventoryOperationId { get; private set; }
    public string SourceDocumentType { get; private set; } = string.Empty;
    public Guid SourceDocumentId { get; private set; }
    public string SourceDocumentNumber { get; private set; } = string.Empty;
    public InventoryOperationKind OperationKind { get; private set; }
    public InventoryOperationStatus OperationStatus { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal DoneQuantity { get; private set; }

    public static PickingGroupLine Create(InventoryOperation operation) => new()
    {
        Id = Guid.NewGuid(),
        InventoryOperationId = operation.Id,
        SourceDocumentType = operation.SourceDocumentType,
        SourceDocumentId = operation.SourceDocumentId,
        SourceDocumentNumber = operation.SourceDocumentNumber,
        OperationKind = operation.OperationKind,
        OperationStatus = operation.Status,
        PlannedQuantity = operation.PlannedQuantity,
        DoneQuantity = operation.DoneQuantity,
        CreatedAt = DateTime.UtcNow
    };

    public void RefreshFromOperation(InventoryOperation operation, string userId)
    {
        OperationStatus = operation.Status;
        DoneQuantity = operation.DoneQuantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public PickingGroupLineDto ToDto() => new()
    {
        Id = Id,
        PickingGroupId = PickingGroupId,
        InventoryOperationId = InventoryOperationId,
        SourceDocumentType = SourceDocumentType,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        OperationKind = OperationKind,
        OperationStatus = OperationStatus,
        PlannedQuantity = PlannedQuantity,
        DoneQuantity = DoneQuantity
    };
}
