using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class UnitDto
{
    public Guid Id { get; set; }
    
    [Required (ErrorMessage ="UnitName is required")]
    public string UnitName { get; set; }

    [Required (ErrorMessage = "UnitNameEng is required")]
    public string UnitNameEng { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }

}
