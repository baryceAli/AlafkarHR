using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public class CompanySetting : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string DefaultLocation { get; private set; } = default!;
    public double DefaultLatitude { get; private set; }
    public double DefaultLongitude { get; private set; }
    public Guid? DefaultPosCustomerId { get; private set; }

    private CompanySetting()
    {
    }

    public static CompanySetting Create(
        Guid id,
        Guid companyId,
        string defaultLocation,
        double defaultLatitude,
        double defaultLongitude,
        Guid? defaultPosCustomerId,
        string createdBy)
    {
        return new CompanySetting
        {
            Id = id,
            CompanyId = companyId,
            DefaultLocation = defaultLocation,
            DefaultLatitude = defaultLatitude,
            DefaultLongitude = defaultLongitude,
            DefaultPosCustomerId = defaultPosCustomerId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string defaultLocation,
        double defaultLatitude,
        double defaultLongitude,
        Guid? defaultPosCustomerId,
        string modifiedBy)
    {
        DefaultLocation = defaultLocation;
        DefaultLatitude = defaultLatitude;
        DefaultLongitude = defaultLongitude;
        DefaultPosCustomerId = defaultPosCustomerId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
