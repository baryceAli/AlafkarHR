using SharedWithUI.SalesOrder.Enums;

namespace SharedWithUI.SalesOrder.Dtos;

public class SalesQuotationDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? PriceListId { get; set; }
    public Guid? QuotationTemplateId { get; set; }
    public string? CouponCode { get; set; }
    public string? SalespersonId { get; set; }
    public SalesQuotationStatus Status { get; set; } = SalesQuotationStatus.Draft;
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? SalesOrderId { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? ConvertedAt { get; set; }
    public string? RejectionReason { get; set; }
    public bool RequiresCustomerSignature { get; set; }
    public bool RequiresOnlinePayment { get; set; }
    public decimal DownPaymentAmount { get; set; }
    public decimal DownPaymentPercent { get; set; }
    public bool IsProForma { get; set; }
    public List<SalesQuotationLineDto> Lines { get; set; } = [];
}

public class SalesQuotationLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal DiscountAmount => Quantity * UnitPrice * DiscountRate / 100m;
    public decimal TaxRate { get; set; }
    public decimal NetAmount => Quantity * UnitPrice - DiscountAmount;
    public decimal TaxAmount => NetAmount * TaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;
    public string? Notes { get; set; }
    public bool IsOptional { get; set; }
    public SalesPricingSnapshotDto Pricing { get; set; } = new();
}
