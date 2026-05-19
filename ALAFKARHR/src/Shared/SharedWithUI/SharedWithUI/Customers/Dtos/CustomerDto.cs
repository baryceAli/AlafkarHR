using SharedWithUI.Customers.Enums;
using System.Net;

namespace SharedWithUI.Customers.Dtos;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get;  set; }

    public string? CommercialName { get;  set; }

    public Guid? CustomerGroupId { get; set; }

    public CustomerStatus Status { get; set; }

    public CustomerType Type { get; set; }

    public decimal CreditLimit { get; set; }

    public PaymentTermType PaymentTerm { get; set; }

    public string? Notes { get; set; }

    public bool IsTaxExempt { get; set; }
    
    public List<AddressDto> Addresses { get; set; }

    public List<ContactDto> Contacts { get; set; }
    public Guid? companyId { get; set; }
}
