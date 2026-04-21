using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Organization.Dtos;

public class BranchDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage ="NameEng is required")]
    public string NameEng { get; set; }


    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; }


    [Required(ErrorMessage = "Longitude is required")]
    [Range(0.1,500,ErrorMessage = "Longitude Must be greator than 0")]
    public double Longitude { get; set; }
    
    
    [Required(ErrorMessage = "Latitude is required")]
    [Range(0.1, 500, ErrorMessage = "Latitude Must be greator than 0")]
    public double Latitude { get; set; }


    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; }
    
    
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }


    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    public bool IsMainBranch { get; set; }
    public Guid CompanyId { get; set; } // 🔴 VERY IMPORTANT
    //public Company Company { get; set; }

    //private readonly List<Administration> _administrations = new();
    public IReadOnlyCollection<AdministrationDto> Administrations ;

}
