using Shared.DDD;

namespace EmployeeModule.Employees.Models;

public class Position:Entity<Guid>
{
    public string Title { get; private set; }
    public string TitleEng { get; private set; } = default!;
    public string Code { get; private set; }

    public decimal BaseSalary { get; private set; }

    public Guid CompanyId { get; private set; }

    private Position() { }

    public static Position Create(Guid id, string title, string titleEng, string code, decimal baseSalary, Guid companyId, string createdBy)
    {
        return new Position
        {
            Id = id,
            Title = title,
            TitleEng = titleEng,
            Code = code,
            BaseSalary = baseSalary,
            CompanyId = companyId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string title,
        string titleEng,
        decimal baseSalary,
        string modifiedBy
        )
    {
        Title = title;
        TitleEng = titleEng;
        BaseSalary= baseSalary;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove( string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }
}
