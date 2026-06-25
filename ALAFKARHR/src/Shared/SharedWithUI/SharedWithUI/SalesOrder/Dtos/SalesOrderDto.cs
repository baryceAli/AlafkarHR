using SharedWithUI.SalesOrder.Enums;

namespace SharedWithUI.SalesOrder.Dtos;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string Number { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? PriceListId { get; set; }
    public string? CouponCode { get; set; }
    public string? SalespersonId { get; set; }
    public Guid? SourceQuotationId { get; set; }
    public SalesOrderSourceType SourceType { get; set; } = SalesOrderSourceType.Manual;
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? AccountingDocumentId { get; set; }
    public Guid? ZatcaEInvoiceId { get; set; }

    public SalesOrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public List<SalesOrderLineDto> Lines { get; set; }

    public bool IsCompleted =>
        Status == SalesOrderStatus.Completed;

    public bool IsCancelled =>
        Status == SalesOrderStatus.Cancelled;

    public bool FullyDelivered =>
    Lines.All(x => x.IsFullyDelivered);

    public bool FullyInvoiced =>
        Lines.All(x => x.IsFullyInvoiced);

    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? StoreFrontId { get; set; }
    public Guid? PosCashierSessionId { get; set; }
    public SalesInvoicingPolicy InvoicingPolicy { get; set; } = SalesInvoicingPolicy.InvoiceDeliveredQuantity;

}
