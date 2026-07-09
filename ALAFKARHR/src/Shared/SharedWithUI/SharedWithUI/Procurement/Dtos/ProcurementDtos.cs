using SharedWithUI.Procurement.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Procurement.Dtos;

public class ProcurementDocumentDto
{
    public Guid Id { get; set; }
    public ProcurementDocumentKind Kind { get; set; }

    [Required]
    public string Number { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; } = DateTime.UtcNow;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public DateTime? SentAt { get; set; }
    public string? SentBy { get; set; }
    public Guid? PurchaseTemplateId { get; set; }
    public Guid? BlanketOrderId { get; set; }
    public Guid? TenderId { get; set; }
    public PurchaseBillControlPolicy BillControlPolicy { get; set; } = PurchaseBillControlPolicy.OrderedQuantities;
    public ThreeWayMatchStatus ThreeWayMatchStatus { get; set; } = ThreeWayMatchStatus.NotRequired;
    public bool IsBillable { get; set; }
    public string? Notes { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ProcurementDocumentLineDto> Lines { get; set; } = new();
}

public class ProcurementDocumentLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid? UnitOfMeasureId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? ReorderingRuleId { get; set; }
    public PurchaseBillControlPolicy? BillControlPolicy { get; set; }
    public ThreeWayMatchStatus ThreeWayMatchStatus { get; set; } = ThreeWayMatchStatus.NotRequired;
    public decimal ReceivedQuantity { get; set; }
    public decimal BilledQuantity { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ProcurementDashboardDto
{
    public int PurchaseRequests { get; set; }
    public int RequestsForQuotation { get; set; }
    public int SupplierQuotations { get; set; }
    public int PurchaseOrders { get; set; }
    public int GoodsReceipts { get; set; }
    public int PurchaseReturns { get; set; }
    public int SupplierInvoices { get; set; }
    public int SentRequestsForQuotation { get; set; }
    public int PurchaseOrdersAwaitingReceipt { get; set; }
    public int BillablePurchaseDocuments { get; set; }
    public int BillableReceipts { get; set; }
    public int ThreeWayMatchExceptions { get; set; }
    public int ActiveTenders { get; set; }
    public int ActiveBlanketOrders { get; set; }
    public List<ProcurementDashboardSupplierDto> TopSuppliers { get; set; } = [];
}

public class ProcurementDashboardSupplierDto
{
    public Guid? SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int PurchaseOrders { get; set; }
    public decimal PurchasedValue { get; set; }
}
