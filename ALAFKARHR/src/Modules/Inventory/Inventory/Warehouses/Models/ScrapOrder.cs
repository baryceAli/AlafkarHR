namespace Inventory.Warehouses.Models;

public class ScrapOrder : Aggregate<Guid>
{
    private readonly List<ScrapOrderLine> _lines = [];

    private ScrapOrder() { }

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid ScrapLocationId { get; private set; }
    public string? SourceDocumentType { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public Guid? SourceDocumentLineId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public Guid? SourceInventoryOperationId { get; private set; }
    public Guid? SourceInventoryOperationLineId { get; private set; }
    public string ScrapOrderNumber { get; private set; } = string.Empty;
    public ScrapOrderStatus Status { get; private set; }
    public bool ReplenishQuantity { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ValidatedAt { get; private set; }
    public string? ValidatedBy { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancelledBy { get; private set; }
    public IReadOnlyCollection<ScrapOrderLine> Lines => _lines.AsReadOnly();
    public decimal TotalQuantity => _lines.Sum(x => x.Quantity);
    public decimal TotalCost => _lines.Sum(x => x.TotalCost);

    public static ScrapOrder Create(CreateScrapOrderDto dto, string scrapOrderNumber, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new ArgumentNullException(nameof(dto.CompanyId));
        if (dto.WarehouseId == Guid.Empty) throw new ArgumentNullException(nameof(dto.WarehouseId));
        if (!dto.ScrapLocationId.HasValue || dto.ScrapLocationId == Guid.Empty)
            throw new BadRequestException("Scrap location is required.");
        if (dto.Lines.Count == 0)
            throw new BadRequestException("At least one scrap line is required.");

        var order = new ScrapOrder
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            WarehouseId = dto.WarehouseId,
            SourceLocationId = dto.SourceLocationId,
            ScrapLocationId = dto.ScrapLocationId.Value,
            SourceDocumentType = dto.SourceDocumentType,
            SourceDocumentId = dto.SourceDocumentId,
            SourceDocumentLineId = dto.SourceDocumentLineId,
            SourceDocumentNumber = dto.SourceDocumentNumber,
            SourceInventoryOperationId = dto.SourceInventoryOperationId,
            SourceInventoryOperationLineId = dto.SourceInventoryOperationLineId,
            ScrapOrderNumber = scrapOrderNumber,
            Status = ScrapOrderStatus.Draft,
            ReplenishQuantity = dto.ReplenishQuantity,
            Reason = dto.Reason,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        order._lines.AddRange(dto.Lines.Select((line, index) => ScrapOrderLine.Create(index + 1, line, order.SourceLocationId, order.ScrapLocationId, userId)));
        return order;
    }

    public void Validate(string userId)
    {
        if (Status != ScrapOrderStatus.Draft)
            throw new BadRequestException("Only draft scrap orders can be validated.");

        Status = ScrapOrderStatus.Validated;
        ValidatedAt = DateTime.UtcNow;
        ValidatedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Cancel(string userId)
    {
        if (Status != ScrapOrderStatus.Draft)
            throw new BadRequestException("Only draft scrap orders can be cancelled.");

        Status = ScrapOrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public ScrapOrderDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        BranchId = BranchId,
        WarehouseId = WarehouseId,
        SourceLocationId = SourceLocationId,
        ScrapLocationId = ScrapLocationId,
        SourceDocumentType = SourceDocumentType,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentLineId = SourceDocumentLineId,
        SourceDocumentNumber = SourceDocumentNumber,
        SourceInventoryOperationId = SourceInventoryOperationId,
        SourceInventoryOperationLineId = SourceInventoryOperationLineId,
        ScrapOrderNumber = ScrapOrderNumber,
        Status = Status,
        ReplenishQuantity = ReplenishQuantity,
        Reason = Reason,
        Notes = Notes,
        ValidatedAt = ValidatedAt,
        ValidatedBy = ValidatedBy,
        CancelledAt = CancelledAt,
        CancelledBy = CancelledBy,
        TotalQuantity = TotalQuantity,
        TotalCost = TotalCost,
        Lines = _lines.Select(x => x.ToDto()).ToList()
    };
}

public class ScrapOrderLine : Entity<Guid>
{
    private ScrapOrderLine() { }

    private readonly List<ScrapOrderLineSerial> _serials = [];

    public Guid ScrapOrderId { get; private set; }
    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public Guid? UnitId { get; private set; }
    public Guid BatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public Guid CurrencyId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid? ScrapLocationId { get; private set; }
    public Guid? SourceDocumentLineId { get; private set; }
    public Guid? SourceInventoryOperationLineId { get; private set; }
    public Guid? StockMovementId { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ScrapOrderLineSerial> Serials => _serials.AsReadOnly();

    public static ScrapOrderLine Create(int lineNumber, CreateScrapOrderLineDto dto, Guid? orderSourceLocationId, Guid orderScrapLocationId, string userId)
    {
        if (dto.ProductId == Guid.Empty) throw new ArgumentNullException(nameof(dto.ProductId));
        if (dto.ProductSkuId == Guid.Empty) throw new ArgumentNullException(nameof(dto.ProductSkuId));
        if (dto.BatchId == Guid.Empty) throw new ArgumentNullException(nameof(dto.BatchId));
        if (dto.Quantity <= 0) throw new BadRequestException("Scrap quantity must be greater than zero.");

        var line = new ScrapOrderLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            UnitId = dto.UnitId,
            BatchId = dto.BatchId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            TotalCost = dto.TotalCost,
            CurrencyId = dto.CurrencyId,
            SourceLocationId = dto.SourceLocationId ?? orderSourceLocationId,
            ScrapLocationId = dto.ScrapLocationId ?? orderScrapLocationId,
            SourceDocumentLineId = dto.SourceDocumentLineId,
            SourceInventoryOperationLineId = dto.SourceInventoryOperationLineId,
            Reason = dto.Reason,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        line._serials.AddRange(dto.SerialNumbers
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
            .Select(x => ScrapOrderLineSerial.Create(x, userId)));

        return line;
    }

    public void AttachStockMovement(Guid stockMovementId, string userId)
    {
        StockMovementId = stockMovementId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public ScrapOrderLineDto ToDto() => new()
    {
        Id = Id,
        ScrapOrderId = ScrapOrderId,
        LineNumber = LineNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductPackageId = ProductPackageId,
        UnitId = UnitId,
        BatchId = BatchId,
        Quantity = Quantity,
        UnitCost = UnitCost,
        TotalCost = TotalCost,
        CurrencyId = CurrencyId,
        SourceLocationId = SourceLocationId,
        ScrapLocationId = ScrapLocationId,
        SourceDocumentLineId = SourceDocumentLineId,
        SourceInventoryOperationLineId = SourceInventoryOperationLineId,
        StockMovementId = StockMovementId,
        Reason = Reason,
        Notes = Notes,
        SerialNumbers = _serials.Select(x => x.ToDto()).ToList()
    };
}

public class ScrapOrderLineSerial : Entity<Guid>
{
    private ScrapOrderLineSerial() { }

    public Guid ScrapOrderLineId { get; private set; }
    public Guid? InventorySerialNumberId { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public Guid? BatchId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? WarehouseLocationId { get; private set; }

    public static ScrapOrderLineSerial Create(InventorySerialSelectionDto dto, string userId) => new()
    {
        Id = Guid.NewGuid(),
        InventorySerialNumberId = dto.InventorySerialNumberId,
        SerialNumber = InventorySerialNumber.Normalize(dto.SerialNumber),
        BatchId = dto.BatchId,
        WarehouseId = dto.WarehouseId,
        WarehouseLocationId = dto.WarehouseLocationId,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    public InventorySerialSelectionDto ToDto() => new()
    {
        InventorySerialNumberId = InventorySerialNumberId,
        SerialNumber = SerialNumber,
        BatchId = BatchId,
        WarehouseId = WarehouseId,
        WarehouseLocationId = WarehouseLocationId
    };
}
