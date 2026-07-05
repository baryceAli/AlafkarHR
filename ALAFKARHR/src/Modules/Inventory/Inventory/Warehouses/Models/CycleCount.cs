namespace Inventory.Warehouses.Models;

public class CycleCount : Aggregate<Guid>
{
    private readonly List<CycleCountLine> _lines = [];
    private CycleCount() { }

    public Guid CompanyId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid WarehouseLocationId { get; private set; }
    public string CountNumber { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public bool IsPosted { get; private set; }
    public DateTime CountDate { get; private set; }
    public IReadOnlyCollection<CycleCountLine> Lines => _lines.AsReadOnly();

    public static CycleCount Create(CycleCountDto dto, string userId)
    {
        var count = new CycleCount
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        count.Apply(dto, userId);
        return count;
    }

    public void Update(CycleCountDto dto, string userId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Posted cycle counts cannot be edited.");
        Apply(dto, userId);
    }

    public void Post(string userId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cycle count is already posted.");
        if (_lines.Count == 0)
            throw new InvalidOperationException("Cycle count requires at least one line.");

        IsPosted = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Posted cycle counts cannot be deleted.");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public CycleCountDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        WarehouseId = WarehouseId,
        WarehouseLocationId = WarehouseLocationId,
        CountNumber = CountNumber,
        Reason = Reason,
        IsPosted = IsPosted,
        CountDate = CountDate,
        Lines = _lines.Select(x => x.ToDto()).ToList()
    };

    private void Apply(CycleCountDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new ArgumentNullException(nameof(dto.CompanyId));
        if (dto.WarehouseId == Guid.Empty) throw new ArgumentNullException(nameof(dto.WarehouseId));
        if (dto.WarehouseLocationId == Guid.Empty) throw new ArgumentNullException(nameof(dto.WarehouseLocationId));

        CompanyId = dto.CompanyId;
        WarehouseId = dto.WarehouseId;
        WarehouseLocationId = dto.WarehouseLocationId;
        CountNumber = string.IsNullOrWhiteSpace(dto.CountNumber)
            ? $"CC-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
            : dto.CountNumber.Trim();
        Reason = dto.Reason;
        CountDate = dto.CountDate == default ? DateTime.UtcNow : dto.CountDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;

        _lines.Clear();
        foreach (var line in dto.Lines)
            _lines.Add(CycleCountLine.Create(Id, line, userId));
    }
}

public class CycleCountLine : Entity<Guid>
{
    private CycleCountLine() { }

    public Guid CycleCountId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid BatchId { get; private set; }
    public decimal CountedQuantity { get; private set; }
    public string? SerialNumbersCsv { get; private set; }
    public IReadOnlyList<string> SerialNumbers => string.IsNullOrWhiteSpace(SerialNumbersCsv)
        ? []
        : SerialNumbersCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    public string? Notes { get; private set; }

    public static CycleCountLine Create(Guid cycleCountId, CycleCountLineDto dto, string userId)
    {
        if (cycleCountId == Guid.Empty) throw new ArgumentNullException(nameof(cycleCountId));
        if (dto.ProductId == Guid.Empty) throw new ArgumentNullException(nameof(dto.ProductId));
        if (dto.ProductSkuId == Guid.Empty) throw new ArgumentNullException(nameof(dto.ProductSkuId));
        if (dto.BatchId == Guid.Empty) throw new ArgumentNullException(nameof(dto.BatchId));
        if (dto.CountedQuantity < 0) throw new ArgumentOutOfRangeException(nameof(dto.CountedQuantity));

        return new CycleCountLine
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CycleCountId = cycleCountId,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            BatchId = dto.BatchId,
            CountedQuantity = dto.CountedQuantity,
            SerialNumbersCsv = NormalizeSerialNumbers(dto.SerialNumbers),
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public CycleCountLineDto ToDto() => new()
    {
        Id = Id,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        BatchId = BatchId,
        CountedQuantity = CountedQuantity,
        SerialNumbers = SerialNumbers.ToList(),
        Notes = Notes
    };

    private static string? NormalizeSerialNumbers(IEnumerable<string>? serialNumbers)
    {
        var cleaned = serialNumbers?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        return cleaned.Count == 0 ? null : string.Join(",", cleaned);
    }
}
