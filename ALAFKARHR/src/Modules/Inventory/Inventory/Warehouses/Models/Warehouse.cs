using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class Warehouse : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
    public string Location { get; set; }
    public string? Address { get; private set; } = default!;
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }

    private Warehouse() { }

    public static Warehouse Create(Guid id, string name, string nameEng, string location, string? address, double longitude, double latitude, string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name,"Name is required");
        ArgumentException.ThrowIfNullOrEmpty(location,"Location is required");
        return new Warehouse
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Location = location,
            Address = address,
            Longitude = longitude,
            Latitude = latitude,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };
    }
    public void Update(string name, string nameEng,string location, string? address, double longitude, double latitude, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name,"Name is required");
        ArgumentException.ThrowIfNullOrEmpty(location,"Location is required");

        Name = name;
        NameEng = nameEng;
        Address = address;
        Longitude = longitude;
        Latitude = latitude;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string removedBy)
    {

        DeletedBy = removedBy;
        DeletedAt = DateTime.UtcNow;
    }
}
