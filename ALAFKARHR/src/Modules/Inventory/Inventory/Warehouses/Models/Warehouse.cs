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
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; private set; }
    public WarehouseType WarehouseType { get; private set; } = WarehouseType.Commercial;
    public string? ShortCode { get; private set; }
    public WarehouseOperationFlow InboundFlow { get; private set; } = WarehouseOperationFlow.OneStep;
    public WarehouseOperationFlow OutboundFlow { get; private set; } = WarehouseOperationFlow.OneStep;
    public Guid? DefaultSourceLocationId { get; private set; }
    public Guid? DefaultDestinationLocationId { get; private set; }
    public Guid? DefaultQualityLocationId { get; private set; }
    public Guid? DefaultPackingLocationId { get; private set; }
    public Guid? DefaultOutputLocationId { get; private set; }
    public Guid? DefaultTransitLocationId { get; private set; }

    private readonly List<WarehouseResupplyLink> _resupplyFromLinks = [];
    public IReadOnlyCollection<WarehouseResupplyLink> ResupplyFromLinks => _resupplyFromLinks.AsReadOnly();

    private Warehouse() { }

    public static Warehouse Create(Guid id, string name, string nameEng, string location, string? address, double longitude, double latitude, Guid companyId, Guid? branchId, WarehouseType warehouseType, string createdBy)
    {
        return Create(id, name, nameEng, location, address, longitude, latitude, companyId, branchId, warehouseType, null, WarehouseOperationFlow.OneStep, WarehouseOperationFlow.OneStep, null, null, null, null, null, null, createdBy);
    }

    public static Warehouse Create(Guid id, string name, string nameEng, string location, string? address, double longitude, double latitude, Guid companyId, Guid? branchId, WarehouseType warehouseType, string? shortCode, WarehouseOperationFlow inboundFlow, WarehouseOperationFlow outboundFlow, Guid? defaultSourceLocationId, Guid? defaultDestinationLocationId, Guid? defaultQualityLocationId, Guid? defaultPackingLocationId, Guid? defaultOutputLocationId, Guid? defaultTransitLocationId, string createdBy)
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
            CompanyId = companyId,
            BranchId = branchId,
            WarehouseType = warehouseType,
            ShortCode = shortCode,
            InboundFlow = inboundFlow,
            OutboundFlow = outboundFlow,
            DefaultSourceLocationId = defaultSourceLocationId,
            DefaultDestinationLocationId = defaultDestinationLocationId,
            DefaultQualityLocationId = defaultQualityLocationId,
            DefaultPackingLocationId = defaultPackingLocationId,
            DefaultOutputLocationId = defaultOutputLocationId,
            DefaultTransitLocationId = defaultTransitLocationId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(string name, string nameEng, string location, string? address, double longitude, double latitude, Guid? branchId, WarehouseType warehouseType, string modifiedBy)
    {
        Update(name, nameEng, location, address, longitude, latitude, branchId, warehouseType, ShortCode, InboundFlow, OutboundFlow, DefaultSourceLocationId, DefaultDestinationLocationId, DefaultQualityLocationId, DefaultPackingLocationId, DefaultOutputLocationId, DefaultTransitLocationId, modifiedBy);
    }

    public void Update(string name, string nameEng, string location, string? address, double longitude, double latitude, Guid? branchId, WarehouseType warehouseType, string? shortCode, WarehouseOperationFlow inboundFlow, WarehouseOperationFlow outboundFlow, Guid? defaultSourceLocationId, Guid? defaultDestinationLocationId, Guid? defaultQualityLocationId, Guid? defaultPackingLocationId, Guid? defaultOutputLocationId, Guid? defaultTransitLocationId, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name,"Name is required");
        ArgumentException.ThrowIfNullOrEmpty(location,"Location is required");

        Name = name;
        NameEng = nameEng;
        Location = location;
        Address = address;
        Longitude = longitude;
        Latitude = latitude;
        BranchId = branchId;
        WarehouseType = warehouseType;
        ShortCode = shortCode;
        InboundFlow = inboundFlow;
        OutboundFlow = outboundFlow;
        DefaultSourceLocationId = defaultSourceLocationId;
        DefaultDestinationLocationId = defaultDestinationLocationId;
        DefaultQualityLocationId = defaultQualityLocationId;
        DefaultPackingLocationId = defaultPackingLocationId;
        DefaultOutputLocationId = defaultOutputLocationId;
        DefaultTransitLocationId = defaultTransitLocationId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void SetResupplyFrom(IEnumerable<Guid> sourceWarehouseIds, string modifiedBy)
    {
        var desiredIds = sourceWarehouseIds.Distinct().ToHashSet();

        foreach (var link in _resupplyFromLinks.Where(x => !desiredIds.Contains(x.SourceWarehouseId)).ToList())
        {
            _resupplyFromLinks.Remove(link);
        }

        foreach (var sourceWarehouseId in desiredIds)
        {
            if (_resupplyFromLinks.Any(x => x.SourceWarehouseId == sourceWarehouseId))
                continue;

            _resupplyFromLinks.Add(WarehouseResupplyLink.Create(CompanyId, Id, sourceWarehouseId, modifiedBy));
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string removedBy)
    {

        IsDeleted = true;
        DeletedBy = removedBy;
        DeletedAt = DateTime.UtcNow;
    }
}

public class WarehouseResupplyLink : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid SourceWarehouseId { get; private set; }

    private WarehouseResupplyLink() { }

    public static WarehouseResupplyLink Create(Guid companyId, Guid warehouseId, Guid sourceWarehouseId, string createdBy)
    {
        if (warehouseId == sourceWarehouseId)
            throw new ArgumentException("A warehouse cannot resupply itself.", nameof(sourceWarehouseId));

        return new WarehouseResupplyLink
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            WarehouseId = warehouseId,
            SourceWarehouseId = sourceWarehouseId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
