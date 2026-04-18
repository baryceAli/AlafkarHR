using Shared.DDD;

namespace EmployeeModule.Employees.Models;

public class Specialization:Aggregate<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public Guid CompanyId { get; set; }

    private Specialization()
    {
        
    }

    public static Specialization Create(Guid id,  string name, string nameEng, Guid companyId, string createdBy)
    {
        return new Specialization
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string nameEng, string modifiedBy)
    {
        Name = name;
        NameEng = nameEng;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }

}
