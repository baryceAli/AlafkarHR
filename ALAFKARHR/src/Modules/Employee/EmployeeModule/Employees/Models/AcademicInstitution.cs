using Shared.DDD;

namespace EmployeeModule.Employees.Models;

public class AcademicInstitution:Aggregate<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public Guid CompanyId { get; set; }
    //public Guid CountryId { get; set; }
    private AcademicInstitution()
    {
        
    }
    public static AcademicInstitution Create(Guid id, string name, string nameEng,Guid companyId, string createdBy)
    {
        return new AcademicInstitution
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId= companyId,
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
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
