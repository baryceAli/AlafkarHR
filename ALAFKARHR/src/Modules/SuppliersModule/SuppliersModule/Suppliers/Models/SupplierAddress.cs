namespace SuppliersModule.Suppliers.Models;

public class SupplierAddress : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string? AddressLine2 { get; private set; }
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public bool IsDefaultBilling { get; private set; }

    protected SupplierAddress() { }

    internal SupplierAddress(Guid id, string title, string addressLine1, string? addressLine2, double longitude, double latitude, string city, string state, string country, string postalCode, bool isDefaultBilling)
    {
        Id = id;
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
    }

    public static SupplierAddress Create(string title, string addressLine1, string? addressLine2, double longitude, double latitude, string city, string state, string country, string postalCode, bool isDefaultBilling, string createdBy)
    {
        return new SupplierAddress
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
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string title, string addressLine1, string? addressLine2, double longitude, double latitude, string city, string state, string country, string postalCode, bool isDefaultBilling, string modifiedBy)
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
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
