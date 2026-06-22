using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesDeliveryNote : Aggregate<Guid>
{
    private readonly List<SalesDeliveryNoteLine> _lines = [];

    private SalesDeliveryNote() { }

    public string Number { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid SalesOrderId { get; private set; }
    public string? SalesOrderNumber { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateTime DeliveryDate { get; private set; }
    public SalesDeliveryNoteStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public IReadOnlyCollection<SalesDeliveryNoteLine> Lines => _lines;

    public static SalesDeliveryNote Create(SalesDeliveryNoteDto dto, SalesOrder sourceOrder, string userId)
    {
        if (dto.SalesOrderId == Guid.Empty) throw new Exception("Sales order is required.");
        if (dto.WarehouseId == Guid.Empty) throw new Exception("Warehouse is required.");
        if (!dto.Lines.Any()) throw new Exception("Delivery note must include at least one line.");

        var note = new SalesDeliveryNote
        {
            Id = Guid.NewGuid(),
            Number = string.IsNullOrWhiteSpace(dto.Number) ? $"DN-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.Number,
            CompanyId = sourceOrder.CompanyId,
            CustomerId = sourceOrder.CustomerId,
            SalesOrderId = sourceOrder.Id,
            SalesOrderNumber = sourceOrder.Number,
            WarehouseId = dto.WarehouseId,
            DeliveryDate = dto.DeliveryDate == default ? DateTime.UtcNow : dto.DeliveryDate,
            Status = SalesDeliveryNoteStatus.Draft,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        note.ReplaceLines(dto.Lines, sourceOrder, userId);
        return note;
    }

    public void Update(SalesDeliveryNoteDto dto, SalesOrder sourceOrder, string userId)
    {
        EnsureDraft();
        WarehouseId = dto.WarehouseId;
        DeliveryDate = dto.DeliveryDate == default ? DeliveryDate : dto.DeliveryDate;
        Notes = dto.Notes;
        ReplaceLines(dto.Lines, sourceOrder, userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public Dictionary<Guid, decimal> Post(string userId)
    {
        EnsureDraft();
        if (!_lines.Any()) throw new Exception("Delivery note has no lines.");

        Status = SalesDeliveryNoteStatus.Submitted;
        PostedAt = DateTime.UtcNow;
        PostedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;

        return _lines.ToDictionary(x => x.SalesOrderLineId, x => x.Quantity);
    }

    public void MarkPostedAgainstOrder(SalesOrder sourceOrder)
    {
        Status = sourceOrder.FullyDelivered
            ? SalesDeliveryNoteStatus.Delivered
            : SalesDeliveryNoteStatus.PartiallyDelivered;
    }

    public void Cancel(string userId)
    {
        EnsureDraft();
        Status = SalesDeliveryNoteStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void ReplaceLines(List<SalesDeliveryNoteLineDto> lines, SalesOrder sourceOrder, string userId)
    {
        if (!lines.Any())
            throw new Exception("Delivery note must include at least one line.");

        _lines.Clear();
        var lineNumber = 1;
        foreach (var line in lines)
        {
            var orderLine = sourceOrder.Lines.FirstOrDefault(x => x.Id == line.SalesOrderLineId)
                ?? throw new Exception($"Sales order line not found: {line.SalesOrderLineId}");

            if (orderLine.DeliveredQuantity + line.Quantity > orderLine.Quantity)
                throw new Exception("Cannot deliver more than ordered quantity.");

            _lines.Add(SalesDeliveryNoteLine.Create(lineNumber++, line, orderLine, userId));
        }
    }

    private void EnsureDraft()
    {
        if (Status != SalesDeliveryNoteStatus.Draft)
            throw new Exception("Only draft delivery notes can be changed.");
    }
}
