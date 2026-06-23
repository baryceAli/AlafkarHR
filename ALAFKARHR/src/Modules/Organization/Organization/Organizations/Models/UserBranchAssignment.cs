using Shared.DDD;

namespace Organization.Organizations.Models;

public class UserBranchAssignment : Entity<Guid>
{
    private UserBranchAssignment() { }

    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public bool IsDefault { get; private set; }

    public static UserBranchAssignment Create(Guid userId, Guid companyId, Guid branchId, bool isDefault, string createdBy)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required.", nameof(userId));
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company is required.", nameof(companyId));
        if (branchId == Guid.Empty)
            throw new ArgumentException("Branch is required.", nameof(branchId));

        return new UserBranchAssignment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            BranchId = branchId,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void SetDefault(bool isDefault, string modifiedBy)
    {
        IsDefault = isDefault;
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

