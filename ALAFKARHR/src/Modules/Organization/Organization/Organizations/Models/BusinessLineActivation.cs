using Shared.DDD;

namespace Organization.Organizations.Models;

public class BusinessLineActivation : Entity<Guid>
{
    public Guid ParentCompanyId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid BusinessLineId { get; private set; }
    public BusinessLine BusinessLine { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private BusinessLineActivation()
    {
    }

    public static BusinessLineActivation Create(Guid parentCompanyId, Guid companyId, Guid businessLineId, string createdBy)
    {
        if (parentCompanyId == Guid.Empty)
            throw new ArgumentException("Parent company is required", nameof(parentCompanyId));
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company is required", nameof(companyId));
        if (businessLineId == Guid.Empty)
            throw new ArgumentException("Business line is required", nameof(businessLineId));

        return new BusinessLineActivation
        {
            Id = Guid.NewGuid(),
            ParentCompanyId = parentCompanyId,
            CompanyId = companyId,
            BusinessLineId = businessLineId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void SetActive(bool isActive, string modifiedBy)
    {
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
