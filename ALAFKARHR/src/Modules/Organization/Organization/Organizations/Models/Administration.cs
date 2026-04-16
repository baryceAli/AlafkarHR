using Shared.DDD;

namespace Organization.Organizations.Models;

public class Administration:Entity<Guid>
{
    public string Name { get;private set; }
    public string NameEng { get; private set; }
    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; }

    public string Code { get; private set; }
    public Guid? ManagerId { get; private set; } // future employee reference
    public bool IsActive { get; private set; }

    public Guid CompanyId { get; private set; }


    private readonly List<Department> _departments = new();
    public IReadOnlyCollection<Department> Departments=> _departments;

    private Administration() { }

    public void AddDepartment(Department department)
    {
        if (_departments.Any(b => b.Id == department.Id))
            throw new Exception("Department already exists");

        _departments.Add(department);
    }
    public void RemoveDepartment(Department department)
    {
        _departments.Remove(department);
    }
    public static Administration Create(Guid id,
        string name,
        string nameEng,
        string code,
        Guid branchId,
        Guid?managerId,
        bool isActive,
        Guid comapnyId,
        string createdBy)
    {

        ArgumentNullException.ThrowIfNullOrEmpty(name, "Name is required");
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng, "NameEng is required");
        ArgumentNullException.ThrowIfNullOrEmpty(code, "Code is required");
        return new Administration
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Code = code,
            BranchId = branchId,
            ManagerId=managerId,
            IsActive=isActive,
            CompanyId = comapnyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(
        string name,
        string nameEng,
        bool isActive,
        Guid? managerId,
        string modifiedBy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, "Name is required");
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng, "NameEng is required");
        Name = name; 
        NameEng=nameEng;
        IsActive = isActive;
        ManagerId = managerId;
        ModifiedAt=DateTime.UtcNow;
        ModifiedBy=modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        DeletedAt=DateTime.UtcNow;
        DeletedBy = deletedBy.Trim();
        IsDeleted = true;
    }
}
