using Shared.DDD;

namespace Organization.Organizations.Models;

public class Department:Entity<Guid>
{
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public Guid AdministrationId { get; private set; }
    public Administration Administration { get; private set; }
    public Guid? HeadOfDepartment { get; private set; }


    public string Code { get; private set; }
    public Guid? ParentDepartmentId { get; private set; } // hierarchy
    //public Department? ParentDepartment { get; private set; }
    public bool IsActive { get; private set; }

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; }
    private Department() { }

    public static Department Create(Guid id,
        string name,
        string nameEng,
        string code,
        Guid administrationId,
        Guid? headOfDepartment,
        Guid companyId,
        bool isActive,
        Guid? parentDepartmentId,
        string createdBy)
    {
        return new Department
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Code = code,
            AdministrationId = administrationId,
            HeadOfDepartment = headOfDepartment,
            CompanyId = companyId,
            IsActive = isActive,
            ParentDepartmentId = parentDepartmentId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(
        string name,
        string nameEng,
        //Guid administrationId,
        Guid? headOfDepartment,
        bool isActive,
        string modifiedBy)
    {
        Name=name;
        NameEng=nameEng;
        //AdministraitonId=administrationId;
        HeadOfDepartment = headOfDepartment;
        IsActive = isActive;
        ModifiedAt= DateTime.UtcNow;
        ModifiedBy=modifiedBy;
    }
    public void AssignHeadOfDepartment(Guid headOfDepartment, string modifiedBy)
    {
        HeadOfDepartment = headOfDepartment;
        ModifiedBy=modifiedBy.Trim();
        ModifiedAt = DateTime.UtcNow;
    }
    public void Remove(string deletedBy)
    {
        DeletedBy=deletedBy;
        DeletedAt=DateTime.UtcNow;
        IsDeleted = true;
    }
}
