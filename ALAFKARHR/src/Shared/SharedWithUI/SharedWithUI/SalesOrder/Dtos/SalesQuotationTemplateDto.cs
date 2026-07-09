namespace SharedWithUI.SalesOrder.Dtos;

public class SalesQuotationTemplateDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ValidityDays { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public bool RequiresCustomerSignature { get; set; }
    public bool RequiresOnlinePayment { get; set; }
    public decimal DownPaymentAmount { get; set; }
    public decimal DownPaymentPercent { get; set; }
    public bool IsProForma { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SalesQuotationTemplateLineDto> Lines { get; set; } = [];
}

public class SalesQuotationTemplateLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public bool IsOptional { get; set; }
}
