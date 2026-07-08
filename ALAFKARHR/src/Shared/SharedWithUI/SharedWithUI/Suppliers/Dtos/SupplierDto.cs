using SharedWithUI.SharedDtos;
using SharedWithUI.Suppliers.Enums;

namespace SharedWithUI.Suppliers.Dtos;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CommercialName { get; set; }
    public string? SupplierCode { get; set; }
    public Guid? SupplierGroupId { get; set; }
    public SupplierStatus Status { get; set; }
    public SupplierType Type { get; set; }
    public SupplierPaymentTermType PaymentTerm { get; set; }
    public string? TaxNumber { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal OpeningBalance { get; set; }
    public Guid? PayableAccountId { get; set; }
    public Guid? ExpenseAccountId { get; set; }
    public Guid? DefaultCurrencyId { get; set; }
    public string? FiscalPosition { get; set; }
    public string? VendorPaymentReference { get; set; }
    public string? Notes { get; set; }
    public List<SupplierAddressDto> Addresses { get; set; } = [];
    public List<SupplierContactDto> Contacts { get; set; } = [];
    public PartnerSmartLinkSummaryDto SmartLinks { get; set; } = new();
    public Guid? CompanyId { get; set; }
}
