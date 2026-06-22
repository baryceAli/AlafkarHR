using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesReturnLine : Entity<Guid>
{
    private SalesReturnLine() { }

    public int LineNumber { get; private set; }
    public Guid SalesOrderLineId { get; private set; }
    public Guid? DeliveryNoteLineId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public string? Notes { get; private set; }
    public decimal DiscountAmount => Quantity * UnitPrice * DiscountRate / 100m;
    public decimal NetAmount => Quantity * UnitPrice - DiscountAmount;
    public decimal TaxAmount => NetAmount * TaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;

    public static SalesReturnLine Create(int lineNumber, SalesReturnLineDto dto, SalesOrderLine orderLine, string userId)
    {
        if (dto.Quantity <= 0) throw new Exception("Return quantity must be greater than zero.");
        if (dto.BatchId == Guid.Empty) throw new Exception("Batch is required.");
        if (dto.CurrencyId == Guid.Empty) throw new Exception("Currency is required.");

        return new SalesReturnLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            SalesOrderLineId = orderLine.Id,
            DeliveryNoteLineId = dto.DeliveryNoteLineId,
            ProductId = orderLine.ProductId,
            ProductSkuId = orderLine.ProductSkuId,
            ProductName = orderLine.ProductName,
            ProductNameEng = orderLine.ProductNameEng,
            SkuCode = orderLine.SkuCode,
            UnitOfMeasureId = orderLine.UnitOfMeasureId,
            BatchId = dto.BatchId,
            CurrencyId = dto.CurrencyId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice <= 0 ? orderLine.UnitPrice : dto.UnitPrice,
            DiscountRate = dto.DiscountRate <= 0 ? orderLine.DiscountRate : dto.DiscountRate,
            TaxRate = dto.TaxRate <= 0 ? orderLine.TaxRate : dto.TaxRate,
            UnitCost = dto.UnitCost,
            TotalCost = dto.TotalCost <= 0 ? dto.Quantity * dto.UnitCost : dto.TotalCost,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}
