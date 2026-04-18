using Shared.DDD;

namespace Payroll.Salaries.Models;

public class Contract:Entity<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string? Description { get; set; }
    //public bool IsTaxable { get; private set; }
    //public bool IsEarning { get; private set; } // earning vs deduction
    private Contract(){}

    public static Contract Create(Guid id, string name, string nameEng, string? description,string createdBy)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required");
        return new Contract
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string name, string nameEng, string? notes, string modifiedBy)
    {
        Name=name;
        NameEng=nameEng;
        Description=notes;
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
