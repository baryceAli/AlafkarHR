using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryComponent:Entity<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string? Notes { get; set; }
    public bool IsTaxable { get; private set; }
    public bool IsEarning { get; private set; } // earning vs deduction
    private SalaryComponent(){}

    public static SalaryComponent Create(Guid id, string name, string nameEng, string? notes,string createdBy)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required");
        return new SalaryComponent
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string name, string nameEng, string? notes, string modifiedBy)
    {
        Name=name;
        NameEng=nameEng;
        Notes=notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy=deletedBy;
        DeletedAt = DateTime.UtcNow;
    }

}
