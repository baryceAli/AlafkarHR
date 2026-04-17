namespace SharedWithUI.Organization.Dtos;

public class AdministrationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    public Guid BranchId { get; set; }
    
    //public Branch Branch { get; private set; }

    public string Code { get; set; }
    public Guid? ManagerId { get; set; } // future employee reference
    public bool IsActive { get; set; }

    public Guid CompanyId { get; set; }
    //public Company Company { get; private set; }

    //private  List<DepartmentDto> _departments = new();
    public IReadOnlyCollection<DepartmentDto> Departments;

}
