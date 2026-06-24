using Shared.DDD;

namespace Organization.Organizations.Models;

public class UserBranchRoleAssignment : Entity<Guid>
{
    private UserBranchRoleAssignment() { }

    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public string TemplateKey { get; private set; } = string.Empty;

    public static UserBranchRoleAssignment Create(Guid userId, Guid companyId, Guid branchId, string templateKey, string createdBy)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User is required.", nameof(userId));
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required.", nameof(companyId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch is required.", nameof(branchId));
        if (string.IsNullOrWhiteSpace(templateKey)) throw new ArgumentException("Template key is required.", nameof(templateKey));

        return new UserBranchRoleAssignment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            BranchId = branchId,
            TemplateKey = templateKey.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
