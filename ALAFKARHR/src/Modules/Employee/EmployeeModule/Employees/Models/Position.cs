using Shared.DDD;

namespace EmployeeModule.Employees.Models;

public class Position:Entity<Guid>
{
    public string Title { get; private set; }
    public string Code { get; private set; }

    public decimal BaseSalary { get; private set; }

    public Guid CompanyId { get; private set; }

    private Position() { }

    public static Position Create(Guid id, string title, string code, decimal baseSalary, Guid companyId, string createdBy)
    {
        return new Position
        {
            Id = id,
            Title = title,
            Code = code,
            BaseSalary = baseSalary,
            CompanyId = companyId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string title,
        decimal baseSalary,
        string modifiedBy
        )
    {
        Title = title;
        BaseSalary= baseSalary;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(Guid id, string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }
}
