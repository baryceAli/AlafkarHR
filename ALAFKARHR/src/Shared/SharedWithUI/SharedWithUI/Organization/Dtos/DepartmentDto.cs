using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Organization.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }


    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }
    
    
    [Required(ErrorMessage ="NameEng is required")]
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



    [Required(ErrorMessage ="Location is required")]
    public string Location { get; set; }



    [Required(ErrorMessage ="Longitude is required")]
    [Range(0.1,100,ErrorMessage ="Longitude must be greator than 0")]
    public double Longitude { get; set; }
    
    
    [Required(ErrorMessage = "Latitude is required")]
    [Range(0.1,100,ErrorMessage = "Latitude must be greator than 0")]
    public double Latitude { get; set; }
    //public Company Company { get; private set; }

}
