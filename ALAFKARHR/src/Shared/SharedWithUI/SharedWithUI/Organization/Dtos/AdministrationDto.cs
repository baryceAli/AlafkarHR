using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Organization.Dtos;

public class AdministrationDto
{
    public Guid Id { get; set; }

    
    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }


    [Required(ErrorMessage = "Name is required")]
    public string NameEng { get; set; }

    [Required(ErrorMessage ="Branch is required")]
    public Guid? BranchId { get; set; }
    
    //public Branch Branch { get; private set; }

    public string Code { get; set; }
    public Guid? ManagerId { get; set; } // future employee reference
    public bool IsActive { get; set; }

    public Guid CompanyId { get; set; }
    //public Company Company { get; private set; }

    //private  List<DepartmentDto> _departments = new();
    public IReadOnlyCollection<DepartmentDto> Departments;

}
