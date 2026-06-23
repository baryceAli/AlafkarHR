using Shared.DDD;

namespace Organization.Organizations.Models;

public class LicenseCategoryBusinessLine : Entity<Guid>
{
    public Guid LicenseCategoryId { get; private set; }
    public LicenseCategory LicenseCategory { get; private set; } = default!;
    public Guid BusinessLineId { get; private set; }
    public BusinessLine BusinessLine { get; private set; } = default!;
    public int ActivationLimit { get; private set; } = 1;

    private LicenseCategoryBusinessLine()
    {
    }

    public static LicenseCategoryBusinessLine Create(Guid licenseCategoryId, Guid businessLineId, int activationLimit = 1)
    {
        if (licenseCategoryId == Guid.Empty)
            throw new ArgumentException("License category is required", nameof(licenseCategoryId));
        if (businessLineId == Guid.Empty)
            throw new ArgumentException("Business line is required", nameof(businessLineId));
        if (activationLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(activationLimit), "Activation limit must be greater than zero");

        return new LicenseCategoryBusinessLine
        {
            Id = Guid.NewGuid(),
            LicenseCategoryId = licenseCategoryId,
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
