using SharedWithUI.SalesOrder.Enums;

namespace SharedWithUI.SalesOrder.Dtos;

public class SalesReturnDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SalesOrderId { get; set; }
    public Guid? DeliveryNoteId { get; set; }
    public Guid? AccountingDocumentId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public SalesReturnStatus Status { get; set; } = SalesReturnStatus.Draft;
    public bool CreateCreditNote { get; set; }
    public string? Reason { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public List<SalesReturnLineDto> Lines { get; set; } = [];
}

public class SalesReturnLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid SalesOrderLineId { get; set; }
    public Guid? DeliveryNoteLineId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public Guid BatchId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal DiscountAmount => Quantity * UnitPrice * DiscountRate / 100m;
    public decimal NetAmount => Quantity * UnitPrice - DiscountAmount;
    public decimal TaxAmount => NetAmount * TaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;
    public string? Notes { get; set; }
}
