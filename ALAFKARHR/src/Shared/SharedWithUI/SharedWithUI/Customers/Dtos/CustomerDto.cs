using SharedWithUI.Customers.Enums;
using SharedWithUI.SharedDtos;

namespace SharedWithUI.Customers.Dtos;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CustomerCode { get; set; }
    public string? CommercialName { get; set; }
    public string? VatNumber { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public Guid? CustomerGroupId { get; set; }
    public CustomerStatus Status { get; set; }
    public decimal CreditLimit { get; set; }
    public PaymentTermType PaymentTerm { get; set; }
    public CreditStatus CreditStatus { get; set; }
    public string? CreditHoldReason { get; set; }
    public decimal AvailableCredit { get; set; }
    public string? Notes { get; set; }
    public bool IsTaxExempt { get; set; }
    public Guid? ReceivableAccountId { get; set; }
    public Guid? IncomeAccountId { get; set; }
    public Guid? DefaultCurrencyId { get; set; }
    public string? FiscalPosition { get; set; }
    public string? CustomerPaymentReference { get; set; }
    public List<AddressDto> Addresses { get; set; } = [];
    public List<ContactDto> Contacts { get; set; } = [];
    public PartnerSmartLinkSummaryDto SmartLinks { get; set; } = new();
    public Guid? CompanyId { get; set; }
}
