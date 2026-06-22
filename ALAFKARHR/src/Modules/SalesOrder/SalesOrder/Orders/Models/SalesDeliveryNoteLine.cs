using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesDeliveryNoteLine : Entity<Guid>
{
    private SalesDeliveryNoteLine() { }

    public int LineNumber { get; private set; }
    public Guid SalesOrderLineId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public string? Notes { get; private set; }

    public static SalesDeliveryNoteLine Create(int lineNumber, SalesDeliveryNoteLineDto dto, SalesOrderLine orderLine, string userId)
    {
        if (dto.Quantity <= 0) throw new Exception("Delivery quantity must be greater than zero.");
        if (dto.BatchId == Guid.Empty) throw new Exception("Batch is required.");
        if (dto.CurrencyId == Guid.Empty) throw new Exception("Currency is required.");

        return new SalesDeliveryNoteLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            SalesOrderLineId = orderLine.Id,
            ProductId = orderLine.ProductId,
            ProductSkuId = orderLine.ProductSkuId,
            ProductName = orderLine.ProductName,
            ProductNameEng = orderLine.ProductNameEng,
            SkuCode = orderLine.SkuCode,
            UnitOfMeasureId = orderLine.UnitOfMeasureId,
            BatchId = dto.BatchId,
            CurrencyId = dto.CurrencyId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            TotalCost = dto.TotalCost <= 0 ? dto.Quantity * dto.UnitCost : dto.TotalCost,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}
