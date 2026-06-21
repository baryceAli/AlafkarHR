namespace ProjectManagement.Projects.Models;

public class ProjectMaterialRequirement : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid DeliverableId { get; private set; }
    public Guid ComponentProductSkuId { get; private set; }
    public string ComponentSkuName { get; private set; } = string.Empty;
    public string? ComponentSkuNameEng { get; private set; }
    public decimal RequiredQuantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal VarianceQuantity { get; private set; }

    private ProjectMaterialRequirement() { }

    public static ProjectMaterialRequirement Create(Guid projectId, Guid deliverableId, Guid componentProductSkuId, string name, string? nameEng, decimal requiredQuantity, string createdBy)
    {
        return new ProjectMaterialRequirement
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            DeliverableId = deliverableId,
            ComponentProductSkuId = componentProductSkuId,
            ComponentSkuName = name,
            ComponentSkuNameEng = nameEng,
            RequiredQuantity = requiredQuantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class ProjectResource : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public ProjectResourceType ResourceType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal PlannedQuantity { get; private set; }
    public decimal PlannedRate { get; private set; }
    public decimal ActualQuantity { get; private set; }
    public decimal ActualRate { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public string? Notes { get; private set; }

    private ProjectResource() { }

    public static ProjectResource Create(Guid projectId, ProjectResourceDto dto, string createdBy)
    {
        return new ProjectResource
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ResourceType = dto.ResourceType,
            ReferenceId = dto.ReferenceId,
            Name = dto.Name.Trim(),
            PlannedQuantity = dto.PlannedQuantity,
            PlannedRate = dto.PlannedRate,
            ActualQuantity = dto.ActualQuantity,
            ActualRate = dto.ActualRate,
            CurrencyId = dto.CurrencyId,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class ProjectExpense : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public ProjectExpenseCategory Category { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal PlannedAmount { get; private set; }
    public decimal ActualAmount { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public string? Notes { get; private set; }

    private ProjectExpense() { }

    public static ProjectExpense Create(Guid projectId, ProjectExpenseDto dto, string createdBy)
    {
        return new ProjectExpense
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Category = dto.Category,
            ExpenseDate = dto.ExpenseDate.Date,
            Description = dto.Description.Trim(),
            PlannedAmount = dto.PlannedAmount,
            ActualAmount = dto.ActualAmount,
            CurrencyId = dto.CurrencyId,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class ProjectHandoff : Entity<Guid>
{
    private readonly List<ProjectHandoffLine> _lines = [];

    public Guid ProjectId { get; private set; }
    public ProjectHandoffType HandoffType { get; private set; }
    public string ReferenceNumber { get; private set; } = string.Empty;
    public DateTime HandoffDate { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ProjectHandoffLine> Lines => _lines;

    private ProjectHandoff() { }

    public static ProjectHandoff Create(Guid projectId, ProjectHandoffType handoffType, string referenceNumber, DateTime handoffDate, string? notes, string createdBy)
    {
        return new ProjectHandoff
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            HandoffType = handoffType,
            ReferenceNumber = referenceNumber,
            HandoffDate = handoffDate,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddLine(ProjectHandoffLine line) => _lines.Add(line);
}

public class ProjectHandoffLine : Entity<Guid>
{
    public Guid HandoffId { get; private set; }
    public Guid? AllocationId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public Guid? WarehouseId { get; private set; }
    public Guid? BatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public Guid? InventoryMovementId { get; private set; }

    private ProjectHandoffLine() { }

    public static ProjectHandoffLine Create(Guid handoffId, ProjectHandoffLineDto dto, string createdBy)
    {
        return new ProjectHandoffLine
        {
            Id = Guid.NewGuid(),
            HandoffId = handoffId,
            AllocationId = dto.AllocationId,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            ItemName = dto.ItemName,
            WarehouseId = dto.WarehouseId,
            BatchId = dto.BatchId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            TotalCost = dto.TotalCost == 0 ? dto.Quantity * dto.UnitCost : dto.TotalCost,
            CurrencyId = dto.CurrencyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void LinkInventoryMovement(Guid inventoryMovementId)
    {
        InventoryMovementId = inventoryMovementId;
    }
}

public class ProjectTaskLink : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid? TaskId { get; private set; }
    public string? TaskNumber { get; private set; }
    public string Title { get; private set; } = string.Empty;

    private ProjectTaskLink() { }

    public static ProjectTaskLink Create(Guid projectId, ProjectTaskLinkDto dto, string createdBy)
    {
        return new ProjectTaskLink
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            TaskId = dto.TaskId,
            TaskNumber = dto.TaskNumber,
            Title = dto.Title.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
