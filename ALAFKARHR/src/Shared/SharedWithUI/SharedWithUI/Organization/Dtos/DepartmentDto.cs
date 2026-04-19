using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Organization.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    [Required(ErrorMessage ="Administration is required")]
    public Guid? AdministrationId { get; set; }
    //public Administration Administration { get; private set; }
    public Guid? HeadOfDepartment { get; set; }


    public string Code { get; set; }
    public Guid? ParentDepartmentId { get; set; } // hierarchy
    //public Department? ParentDepartment { get; private set; }
    public bool IsActive { get; set; }

    public Guid CompanyId { get; set; }
    //public Company Company { get; private set; }

}
