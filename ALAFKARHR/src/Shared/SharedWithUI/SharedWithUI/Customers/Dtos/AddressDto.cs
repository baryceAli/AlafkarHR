namespace SharedWithUI.Customers.Dtos;

public class AddressDto
{
    public Guid Id { get; set; }
    public string Title { get;  set; }

    public string AddressLine1 { get;  set; }

    public string? AddressLine2 { get; set; }

    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string PostalCode { get; set; }

    public bool IsDefaultBilling { get; set; }

    public bool IsDefaultShipping { get; set; }

}
