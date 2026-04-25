using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductPackageDto
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }

    [Required(ErrorMessage = "UnitsCount is required")]
    [Range(0.01,1000000,ErrorMessage = "UnitsCount must be greator than 0")]
    public double UnitsCount { get; set; }


    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }

}


