namespace CustomersModule.Customers.Models;

public class Address:Entity<Guid>
{
    public string Title { get; private set; }

    public string AddressLine1 { get; private set; }

    public string? AddressLine2 { get; private set; }

    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public string City { get; private set; }

    public string State { get; private set; }

    public string Country { get; private set; }

    public string PostalCode { get; private set; }

    public bool IsDefaultBilling { get; private set; }

    public bool IsDefaultShipping { get; private set; }
    public PartnerAddressType AddressType { get; private set; } = PartnerAddressType.Contact;
    protected Address(){}
    internal Address(
        string title, 
        string addressLine1, 
        string? addressLine2, 
        double longitude,
        double latitude,
        string city, 
        string state, 
        string country, 
        string postalCode,
        bool isDefaultBilling,
        bool isDefaultShipping,
        PartnerAddressType addressType)
    {
        Id = Guid.Empty;
        Title = title;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        Longitude = longitude;
        Latitude = latitude;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        IsDefaultBilling = isDefaultBilling;
        IsDefaultShipping = isDefaultShipping;
        AddressType = addressType;
    }
    internal static Address Create(
        string title,
        string addressLine1,
        string? addressLine2,
        double longitude,
        double latitude,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefaultBilling,
        bool isDefaultShipping,
        PartnerAddressType addressType,
        string createdBy)
    {
        return new Address
        {
            Id = Guid.NewGuid(),
            Title = title,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            Longitude = longitude,
            Latitude = latitude,
            City = city,
            State = state,
            Country = country,
            PostalCode = postalCode,
            IsDefaultBilling = isDefaultBilling,
            IsDefaultShipping = isDefaultShipping,
            AddressType = addressType,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy

        };
    }
    public void Update(
        string title,
        string addressLine1,
        string? addressLine2,
        double longitude,
        double latitude,
        string city,
        string state,
        string country,
        string postalCode,
        bool isDefaultBilling, 
        bool isDefaultShipping,
        PartnerAddressType addressType)
    {
        Title = title;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        Longitude = longitude;
        Latitude = latitude;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        IsDefaultBilling = isDefaultBilling;
        IsDefaultShipping = isDefaultShipping;
        AddressType = addressType;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
