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
    public decimal MultipleQuantity { get; set; }
    public int LeadTimeDays { get; set; }
    public int HorizonDays { get; set; }
    public ReplenishmentTriggerMode TriggerMode { get; set; } = ReplenishmentTriggerMode.Manual;
    public bool AutoCreatePurchaseRequest { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastRunAt { get; set; }
    public Guid? LastGeneratedDocumentId { get; set; }
    public string? LastGeneratedDocumentNumber { get; set; }
    public DateTime? LastGeneratedAt { get; set; }
}

public class ReplenishmentSuggestionDto
{
    public Guid ReorderingRuleId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal UnitCost { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal MaximumQuantity { get; set; }
    public decimal ReorderQuantity { get; set; }
    public decimal MinimumOrderQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal IncomingQuantity { get; set; }
    public decimal OutgoingQuantity { get; set; }
    public decimal ForecastedQuantity { get; set; }
    public decimal SuggestedQuantity { get; set; }
    public int LeadTimeDays { get; set; }
    public int HorizonDays { get; set; }
    public DateTime ExpectedDate { get; set; }
    public ReplenishmentTriggerMode TriggerMode { get; set; } = ReplenishmentTriggerMode.Manual;
    public bool IsBelowMinimum { get; set; }
    public bool IsOrderToMaxEligible { get; set; }
    public bool CanCreatePurchaseRequest { get; set; }
    public string WarningCode { get; set; } = string.Empty;
    public string WarningMessage { get; set; } = string.Empty;
}

public class CreatePurchaseRequestFromReplenishmentDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? CurrencyId { get; set; }
    public bool OrderToMax { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseRequestFromReplenishmentLineDto> Lines { get; set; } = new();
}

public class CreatePurchaseRequestFromReplenishmentLineDto
{
    public Guid ReorderingRuleId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal Quantity { get; set; }
}

public class RunAutomaticReplenishmentDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
}

public class RunAutomaticReplenishmentResultDto
{
    public int DocumentsCreated { get; set; }
    public int LinesCreated { get; set; }
    public List<Guid> DocumentIds { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
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

public class ProcurementAgreementDto
{
    public Guid Id { get; set; }
    public ProcurementAgreementType Type { get; set; } = ProcurementAgreementType.PurchaseTemplate;
    public ProcurementAgreementStatus Status { get; set; } = ProcurementAgreementStatus.Draft;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public Guid? CurrencyId { get; set; }
    public DateTime AgreementDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public List<ProcurementAgreementLineDto> Lines { get; set; } = [];
}

public class ProcurementAgreementLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid? UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitCost { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
}

public class ProcurementRecomputeResultDto
{
    public int UpdatedDocuments { get; set; }
    public int ExceptionDocuments { get; set; }
}
