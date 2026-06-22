using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesReturn : Aggregate<Guid>
{
    private readonly List<SalesReturnLine> _lines = [];

    private SalesReturn() { }

    public string Number { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid SalesOrderId { get; private set; }
    public Guid? DeliveryNoteId { get; private set; }
    public Guid? AccountingDocumentId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public SalesReturnStatus Status { get; private set; }
    public bool CreateCreditNote { get; private set; }
    public string? Reason { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public IReadOnlyCollection<SalesReturnLine> Lines => _lines;

    public static SalesReturn Create(SalesReturnDto dto, SalesOrder sourceOrder, string userId)
    {
        if (dto.SalesOrderId == Guid.Empty) throw new Exception("Sales order is required.");
        if (dto.WarehouseId == Guid.Empty) throw new Exception("Warehouse is required.");
        if (!dto.Lines.Any()) throw new Exception("Sales return must include at least one line.");

        var salesReturn = new SalesReturn
        {
            Id = Guid.NewGuid(),
            Number = string.IsNullOrWhiteSpace(dto.Number) ? $"SR-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.Number,
            CompanyId = sourceOrder.CompanyId,
            CustomerId = sourceOrder.CustomerId,
            SalesOrderId = sourceOrder.Id,
            DeliveryNoteId = dto.DeliveryNoteId,
            AccountingDocumentId = dto.AccountingDocumentId,
            WarehouseId = dto.WarehouseId,
            ReturnDate = dto.ReturnDate == default ? DateTime.UtcNow : dto.ReturnDate,
            Status = SalesReturnStatus.Draft,
            CreateCreditNote = dto.CreateCreditNote,
            Reason = dto.Reason,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        salesReturn.ReplaceLines(dto.Lines, sourceOrder, userId);
        return salesReturn;
    }

    public void Update(SalesReturnDto dto, SalesOrder sourceOrder, string userId)
    {
        EnsureDraft();
        DeliveryNoteId = dto.DeliveryNoteId;
        AccountingDocumentId = dto.AccountingDocumentId;
        WarehouseId = dto.WarehouseId;
        ReturnDate = dto.ReturnDate == default ? ReturnDate : dto.ReturnDate;
        CreateCreditNote = dto.CreateCreditNote;
        Reason = dto.Reason;
        ReplaceLines(dto.Lines, sourceOrder, userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public Dictionary<Guid, decimal> Post(string userId)
    {
        EnsureDraft();
        Status = SalesReturnStatus.Posted;
        PostedAt = DateTime.UtcNow;
        PostedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        return _lines.ToDictionary(x => x.SalesOrderLineId, x => x.Quantity);
    }

    public void Cancel(string userId)
    {
        EnsureDraft();
        Status = SalesReturnStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void ReplaceLines(List<SalesReturnLineDto> lines, SalesOrder sourceOrder, string userId)
    {
        if (!lines.Any())
            throw new Exception("Sales return must include at least one line.");

        _lines.Clear();
        var lineNumber = 1;
        foreach (var line in lines)
        {
            var orderLine = sourceOrder.Lines.FirstOrDefault(x => x.Id == line.SalesOrderLineId)
                ?? throw new Exception($"Sales order line not found: {line.SalesOrderLineId}");

            if (orderLine.ReturnedQuantity + line.Quantity > orderLine.DeliveredQuantity)
                throw new Exception("Cannot return more than delivered quantity.");

            if (CreateCreditNote && orderLine.ReturnedQuantity + line.Quantity > orderLine.InvoicedQuantity)
                throw new Exception("Cannot credit more than invoiced quantity.");

            _lines.Add(SalesReturnLine.Create(lineNumber++, line, orderLine, userId));
        }

        Subtotal = _lines.Sum(x => x.NetAmount);
        TaxAmount = _lines.Sum(x => x.TaxAmount);
        TotalAmount = Subtotal + TaxAmount;
    }

    private void EnsureDraft()
    {
        if (Status != SalesReturnStatus.Draft)
            throw new Exception("Only draft returns can be changed.");
    }
}
