namespace SharedWithUI.SalesOrder.Dtos;

public class SalesOrderLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }

    public Guid ProductSkuId { get; set; }

    public string ProductName { get; set; }
    public string ProductNameEng { get; set; }

    public string SkuCode { get; set; }

    public decimal Quantity { get; set; }

    //public decimal ReservedQuantity { get; private set; }

    public decimal DeliveredQuantity { get; set; }

    public decimal InvoicedQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }

    public decimal UnitPrice { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal DiscountAmount =>
    (Quantity * UnitPrice) * DiscountRate / 100m;

    public decimal TaxRate { get; set; }

    public decimal TaxAmount =>
        NetAmount * TaxRate / 100m;
    public decimal NetAmount =>
        (Quantity * UnitPrice) - DiscountAmount;

    public string? Notes { get; set; }
    public decimal TotalAmount =>
        NetAmount + TaxAmount;
    public SalesPricingSnapshotDto Pricing { get; set; } = new();

    //public bool IsFullyReserved =>
    //    ReservedQuantity >= Quantity;

    public bool IsFullyDelivered =>
        DeliveredQuantity >= Quantity;

    public bool IsFullyInvoiced =>
        InvoicedQuantity >= Quantity;
}
