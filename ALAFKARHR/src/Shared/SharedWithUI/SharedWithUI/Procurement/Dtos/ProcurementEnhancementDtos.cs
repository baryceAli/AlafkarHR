using SharedWithUI.Procurement.Enums;

namespace SharedWithUI.Procurement.Dtos;

public class SupplierItemDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public string? SkuCode { get; set; }
    public string SupplierSku { get; set; } = string.Empty;
    public int LeadTimeDays { get; set; }
    public decimal MinimumOrderQuantity { get; set; }
    public bool IsPreferred { get; set; }
    public string? Notes { get; set; }
}

public class VendorPricelistDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountRate { get; set; }
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow.Date;
    public DateTime? ValidTo { get; set; }
    public bool IsPreferred { get; set; }
}

public class ReorderingRuleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal MaximumQuantity { get; set; }
    public decimal ReorderQuantity { get; set; }
    public int LeadTimeDays { get; set; }
    public bool AutoCreatePurchaseRequest { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProcurementTrackerRowDto
{
    public Guid Id { get; set; }
    public ProcurementDocumentKind Kind { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
    public decimal OpenQuantity { get; set; }
    public int AgeDays { get; set; }
}

public class SupplierScorecardRowDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int PurchaseOrders { get; set; }
    public int GoodsReceipts { get; set; }
    public int SupplierInvoices { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
    public decimal ReceiptCompletionRate { get; set; }
    public decimal InvoiceMatchRate { get; set; }
}
