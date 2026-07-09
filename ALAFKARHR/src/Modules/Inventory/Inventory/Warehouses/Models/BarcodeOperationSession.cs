namespace Inventory.Warehouses.Models;

public class BarcodeOperationSession : Aggregate<Guid>
{
    private readonly List<BarcodeOperationLine> _lines = [];

    private BarcodeOperationSession() { }

    public Guid CompanyId { get; private set; }
    public BarcodeOperationType OperationType { get; private set; }
    public BarcodeSessionStatus Status { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    public Guid? InventoryOperationId { get; private set; }
    public string? SourceDocumentType { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? Notes { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? AppliedAt { get; private set; }
    public string? WarningsJson { get; private set; }
    public IReadOnlyCollection<BarcodeOperationLine> Lines => _lines.AsReadOnly();

    public static BarcodeOperationSession Create(BarcodeOperationSessionDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new ArgumentNullException(nameof(dto.CompanyId));

        return new BarcodeOperationSession
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            OperationType = dto.OperationType,
            Status = BarcodeSessionStatus.Draft,
            WarehouseId = dto.WarehouseId,
            SourceLocationId = dto.SourceLocationId,
            DestinationLocationId = dto.DestinationLocationId,
            InventoryOperationId = dto.InventoryOperationId,
            SourceDocumentType = dto.SourceDocumentType,
            SourceDocumentId = dto.SourceDocumentId,
            ReferenceNumber = string.IsNullOrWhiteSpace(dto.ReferenceNumber)
                ? $"BAR-{dto.OperationType}-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
                : dto.ReferenceNumber.Trim(),
            Notes = dto.Notes,
            StartedAt = DateTime.UtcNow,
            WarningsJson = SerializeWarnings(dto.Warnings),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void AddLine(BarcodeOperationLine line, string userId)
    {
        if (Status == BarcodeSessionStatus.Applied || Status == BarcodeSessionStatus.Cancelled)
            throw new InvalidOperationException("Applied or cancelled barcode sessions cannot be changed.");

        _lines.Add(line);
        Status = _lines.Any(x => x.Status == BarcodeLineStatus.Rejected)
            ? BarcodeSessionStatus.Draft
            : BarcodeSessionStatus.Ready;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void UpdateContextFromScan(BarcodeScanResultDto scan, string userId)
    {
        if (Status == BarcodeSessionStatus.Applied || Status == BarcodeSessionStatus.Cancelled)
            throw new InvalidOperationException("Applied or cancelled barcode sessions cannot be changed.");

        if (scan.WarehouseId.HasValue)
            WarehouseId ??= scan.WarehouseId;

        if (scan.WarehouseLocationId.HasValue)
        {
            if (OperationType is BarcodeOperationType.StockOut or BarcodeOperationType.Delivery or BarcodeOperationType.TransferShip)
                SourceLocationId = scan.WarehouseLocationId;
            else
                DestinationLocationId = scan.WarehouseLocationId;
        }

        if (scan.EntityType is BarcodeScanEntityType.WarehouseTransfer or BarcodeScanEntityType.CycleCount)
        {
            SourceDocumentId ??= scan.EntityId;
            ReferenceNumber ??= scan.Code;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void MarkApplied(string userId)
    {
        if (_lines.Count == 0)
            throw new InvalidOperationException("Barcode session has no scan lines.");
        if (_lines.Any(x => x.Status == BarcodeLineStatus.Rejected))
            throw new InvalidOperationException("Barcode session contains rejected scan lines.");

        Status = BarcodeSessionStatus.Applied;
        AppliedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void AddWarning(string warning, string userId)
    {
        var warnings = DeserializeWarnings();
        warnings.Add(warning);
        WarningsJson = SerializeWarnings(warnings);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public BarcodeOperationSessionDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        OperationType = OperationType,
        Status = Status,
        WarehouseId = WarehouseId,
        SourceLocationId = SourceLocationId,
        DestinationLocationId = DestinationLocationId,
        InventoryOperationId = InventoryOperationId,
        SourceDocumentType = SourceDocumentType,
        SourceDocumentId = SourceDocumentId,
        ReferenceNumber = ReferenceNumber,
        Notes = Notes,
        StartedAt = StartedAt,
        AppliedAt = AppliedAt,
        Warnings = DeserializeWarnings(),
        Lines = _lines.Select(x => x.ToDto()).ToList()
    };

    private List<string> DeserializeWarnings() =>
        string.IsNullOrWhiteSpace(WarningsJson)
            ? []
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(WarningsJson) ?? [];

    private static string? SerializeWarnings(IEnumerable<string>? warnings)
    {
        var list = warnings?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList() ?? [];
        return list.Count == 0 ? null : System.Text.Json.JsonSerializer.Serialize(list);
    }
}

public class BarcodeOperationLine : Entity<Guid>
{
    private BarcodeOperationLine() { }

    public Guid BarcodeOperationSessionId { get; private set; }
    public string RawBarcode { get; private set; } = string.Empty;
    public BarcodeScanEntityType EntityType { get; private set; }
    public BarcodeLineStatus Status { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public Guid? BatchId { get; private set; }
    public Guid? InventorySerialNumberId { get; private set; }
    public string? SerialNumber { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    public Guid? InventoryOperationLineId { get; private set; }
    public decimal EnteredQuantity { get; private set; }
    public decimal PackageMultiplier { get; private set; }
    public decimal UnitMultiplier { get; private set; }
    public decimal NormalizedQuantity { get; private set; }
    public string? DisplayLabel { get; private set; }
    public string? DisplayLabelEng { get; private set; }
    public string? BatchNumber { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Warning { get; private set; }
    public DateTime ScannedAt { get; private set; }

    public static BarcodeOperationLine Create(Guid sessionId, BarcodeOperationLineDto dto, string userId)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentNullException(nameof(sessionId));
        if (string.IsNullOrWhiteSpace(dto.RawBarcode))
            throw new ArgumentException("Barcode is required.", nameof(dto.RawBarcode));

        return new BarcodeOperationLine
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            BarcodeOperationSessionId = sessionId,
            RawBarcode = dto.RawBarcode.Trim(),
            EntityType = dto.EntityType,
            Status = dto.Status,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            BatchId = dto.BatchId,
            InventorySerialNumberId = dto.InventorySerialNumberId,
            SerialNumber = string.IsNullOrWhiteSpace(dto.SerialNumber) ? null : dto.SerialNumber.Trim().ToUpperInvariant(),
            WarehouseId = dto.WarehouseId,
            SourceLocationId = dto.SourceLocationId,
            DestinationLocationId = dto.DestinationLocationId,
            InventoryOperationLineId = dto.InventoryOperationLineId,
            EnteredQuantity = dto.EnteredQuantity <= 0 ? 1m : dto.EnteredQuantity,
            PackageMultiplier = dto.PackageMultiplier <= 0 ? 1m : dto.PackageMultiplier,
            UnitMultiplier = dto.UnitMultiplier <= 0 ? 1m : dto.UnitMultiplier,
            NormalizedQuantity = dto.NormalizedQuantity <= 0 ? 1m : dto.NormalizedQuantity,
            DisplayLabel = dto.DisplayLabel,
            DisplayLabelEng = dto.DisplayLabelEng,
            BatchNumber = dto.BatchNumber,
            ExpiryDate = dto.ExpiryDate,
            Warning = dto.Warning,
            ScannedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public BarcodeOperationLineDto ToDto() => new()
    {
        Id = Id,
        BarcodeOperationSessionId = BarcodeOperationSessionId,
        RawBarcode = RawBarcode,
        EntityType = EntityType,
        Status = Status,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductPackageId = ProductPackageId,
        BatchId = BatchId,
        InventorySerialNumberId = InventorySerialNumberId,
        SerialNumber = SerialNumber,
        WarehouseId = WarehouseId,
        SourceLocationId = SourceLocationId,
        DestinationLocationId = DestinationLocationId,
        InventoryOperationLineId = InventoryOperationLineId,
        EnteredQuantity = EnteredQuantity,
        PackageMultiplier = PackageMultiplier,
        UnitMultiplier = UnitMultiplier,
        NormalizedQuantity = NormalizedQuantity,
        DisplayLabel = DisplayLabel,
        DisplayLabelEng = DisplayLabelEng,
        BatchNumber = BatchNumber,
        ExpiryDate = ExpiryDate,
        Warning = Warning,
        ScannedAt = ScannedAt
    };
}
