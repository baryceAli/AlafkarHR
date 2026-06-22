using SharedWithUI.SalesOrder.Enums;

namespace SharedWithUI.SalesOrder.Dtos;

public class SalesDeliveryNoteDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SalesOrderId { get; set; }
    public string? SalesOrderNumber { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime DeliveryDate { get; set; } = DateTime.UtcNow;
    public SalesDeliveryNoteStatus Status { get; set; } = SalesDeliveryNoteStatus.Draft;
    public string? Notes { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public List<SalesDeliveryNoteLineDto> Lines { get; set; } = [];
}

public class SalesDeliveryNoteLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid SalesOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public Guid BatchId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
}
