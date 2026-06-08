using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public class CompanySetting : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string DefaultLocation { get; private set; }
    public double DefaultLatitude { get; private set; }
    public double DefaultLongitude { get; private set; }

    private CompanySetting()
    {
    }

    public static CompanySetting Create(
        Guid id,
        Guid companyId,
        string defaultLocation,
        double defaultLatitude,
        double defaultLongitude,
        string createdBy)
    {
        return new CompanySetting
        {
            Id = id,
            CompanyId = companyId,
            DefaultLocation = defaultLocation,
            DefaultLatitude = defaultLatitude,
            DefaultLongitude = defaultLongitude,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
