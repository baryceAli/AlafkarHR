namespace Inventory.Warehouses.Models;

public class InventoryOperation : Aggregate<Guid>
{
    private readonly List<InventoryOperationLine> _lines = [];

    private InventoryOperation() { }

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public InventoryOperationFlowDirection FlowDirection { get; private set; }
    public InventoryOperationKind OperationKind { get; private set; }
    public InventoryOperationStatus Status { get; private set; }
    public int Sequence { get; private set; }
    public bool IsStockPostingStep { get; private set; }
    public bool StockPosted { get; private set; }
    public string SourceDocumentType { get; private set; } = string.Empty;
    public Guid SourceDocumentId { get; private set; }
    public string SourceDocumentNumber { get; private set; } = string.Empty;
    public Guid? BackorderOfOperationId { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletedBy { get; private set; }
    public IReadOnlyCollection<InventoryOperationLine> Lines => _lines.AsReadOnly();
    public decimal PlannedQuantity => _lines.Sum(x => x.PlannedQuantity);
    public decimal DoneQuantity => _lines.Sum(x => x.DoneQuantity);

    public static InventoryOperation Create(
        Guid companyId,
        Guid? branchId,
        Guid warehouseId,
        InventoryOperationFlowDirection flowDirection,
        InventoryOperationKind operationKind,
        int sequence,
        bool isStockPostingStep,
        string sourceDocumentType,
        Guid sourceDocumentId,
        string sourceDocumentNumber,
        IEnumerable<InventoryOperationLine> lines,
        string userId,
        Guid? backorderOfOperationId = null)
    {
        var operation = new InventoryOperation
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            BranchId = branchId,
            WarehouseId = warehouseId,
            FlowDirection = flowDirection,
            OperationKind = operationKind,
            Sequence = sequence,
            IsStockPostingStep = isStockPostingStep,
            SourceDocumentType = sourceDocumentType,
            SourceDocumentId = sourceDocumentId,
            SourceDocumentNumber = sourceDocumentNumber,
            BackorderOfOperationId = backorderOfOperationId,
            Status = InventoryOperationStatus.Ready,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        operation._lines.AddRange(lines);
        return operation;
    }

    public void Complete(Dictionary<Guid, decimal> doneQuantities, string userId)
    {
        if (Status is InventoryOperationStatus.Done or InventoryOperationStatus.Cancelled)
            throw new BadRequestException("Inventory operation is already closed.");

        var totalPlanned = PlannedQuantity;
        if (totalPlanned <= 0)
            throw new BadRequestException("Inventory operation has no planned quantity.");

        foreach (var line in _lines)
        {
            var doneQuantity = doneQuantities.TryGetValue(line.Id, out var quantity)
                ? quantity
                : line.PlannedQuantity;
            line.SetDoneQuantity(doneQuantity, userId);
        }

        Status = DoneQuantity >= totalPlanned
            ? InventoryOperationStatus.Done
            : InventoryOperationStatus.PartiallyDone;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void MarkCompletedWithoutPosting(string userId)
    {
        foreach (var line in _lines)
            line.SetDoneQuantity(line.PlannedQuantity, userId);

        Status = InventoryOperationStatus.Done;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void MarkStockPosted(string userId)
    {
        StockPosted = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public InventoryOperationDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        BranchId = BranchId,
        WarehouseId = WarehouseId,
        FlowDirection = FlowDirection,
        OperationKind = OperationKind,
        Status = Status,
        Sequence = Sequence,
        IsStockPostingStep = IsStockPostingStep,
        StockPosted = StockPosted,
        SourceDocumentType = SourceDocumentType,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        BackorderOfOperationId = BackorderOfOperationId,
        PlannedQuantity = PlannedQuantity,
        DoneQuantity = DoneQuantity,
        CompletedAt = CompletedAt,
        CompletedBy = CompletedBy,
        Lines = _lines.Select(x => x.ToDto()).ToList()
    };
}

public class InventoryOperationLine : Entity<Guid>
{
    private InventoryOperationLine() { }

    public Guid InventoryOperationId { get; private set; }
    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public Guid? UnitId { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    public Guid? SourceDocumentLineId { get; private set; }
    public Guid? StockMovementId { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal DoneQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public Guid CurrencyId { get; private set; }
    public string? Notes { get; private set; }
    public bool ConsumeReservedQuantity { get; private set; }

    public static InventoryOperationLine Create(
        int lineNumber,
        InventoryOperationChainLine source,
        Guid? sourceLocationId,
        Guid? destinationLocationId,
        string userId)
    {
        return new InventoryOperationLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = source.ProductId,
            ProductSkuId = source.ProductSkuId,
            ProductPackageId = source.ProductPackageId,
            UnitId = source.UnitId,
            BatchId = source.BatchId,
            SourceLocationId = sourceLocationId,
            DestinationLocationId = destinationLocationId,
            SourceDocumentLineId = source.SourceDocumentLineId,
            PlannedQuantity = source.Quantity,
            UnitCost = source.UnitCost,
            TotalCost = source.TotalCost,
            CurrencyId = source.CurrencyId,
            Notes = source.Notes,
            ConsumeReservedQuantity = source.ConsumeReservedQuantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void SetDoneQuantity(decimal quantity, string userId)
    {
        if (quantity < 0 || quantity > PlannedQuantity)
            throw new BadRequestException("Done quantity must be between zero and the planned operation quantity.");

        DoneQuantity = quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void AttachStockMovement(Guid stockMovementId, string userId)
    {
        StockMovementId = stockMovementId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public InventoryOperationLineDto ToDto() => new()
    {
        Id = Id,
        InventoryOperationId = InventoryOperationId,
        LineNumber = LineNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductPackageId = ProductPackageId,
        UnitId = UnitId,
        BatchId = BatchId,
        SourceLocationId = SourceLocationId,
        DestinationLocationId = DestinationLocationId,
        SourceDocumentLineId = SourceDocumentLineId,
        StockMovementId = StockMovementId,
        PlannedQuantity = PlannedQuantity,
        DoneQuantity = DoneQuantity,
        UnitCost = UnitCost,
        TotalCost = TotalCost,
        CurrencyId = CurrencyId,
        Notes = Notes,
        ConsumeReservedQuantity = ConsumeReservedQuantity
    };
}
