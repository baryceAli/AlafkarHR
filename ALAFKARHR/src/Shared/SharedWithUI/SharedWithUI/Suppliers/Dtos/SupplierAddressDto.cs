namespace SharedWithUI.Suppliers.Dtos;

using SharedWithUI.Customers.Enums;

public class SupplierAddressDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefaultBilling { get; set; }
    public PartnerAddressType AddressType { get; set; } = PartnerAddressType.Contact;
}
