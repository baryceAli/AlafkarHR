using Shared.DDD;

namespace Organization.Organizations.Models;

public class CompanyLicenseBusinessLine : Entity<Guid>
{
    public Guid CompanyLicenseId { get; private set; }
    public CompanyLicense CompanyLicense { get; private set; } = default!;
    public Guid BusinessLineId { get; private set; }
    public BusinessLine BusinessLine { get; private set; } = default!;
    public int ActivationLimit { get; private set; } = 1;

    private CompanyLicenseBusinessLine()
    {
    }

    public static CompanyLicenseBusinessLine Create(Guid companyLicenseId, Guid businessLineId, int activationLimit = 1)
    {
        if (companyLicenseId == Guid.Empty)
            throw new ArgumentException("Company license is required", nameof(companyLicenseId));
        if (businessLineId == Guid.Empty)
            throw new ArgumentException("Business line is required", nameof(businessLineId));
        if (activationLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(activationLimit), "Activation limit must be greater than zero");

        return new CompanyLicenseBusinessLine
        {
            Id = Guid.NewGuid(),
            CompanyLicenseId = companyLicenseId,
            BusinessLineId = businessLineId,
            ActivationLimit = activationLimit
        };
    }

    public void UpdateActivationLimit(int activationLimit)
    {
        if (activationLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(activationLimit), "Activation limit must be greater than zero");

        ActivationLimit = activationLimit;
    }
}
