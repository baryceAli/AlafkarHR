using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class UnitDto
{
    public Guid Id { get; set; }
    
    [Required (ErrorMessage ="UnitName is required")]
    public string UnitName { get; set; }

    [Required (ErrorMessage = "UnitNameEng is required")]
    public string UnitNameEng { get; set; }

    [Required(ErrorMessage = "Unit category is required")]
    public string UnitCategory { get; set; } = "General";

    [Range(0.000001, 10000000, ErrorMessage = "Conversion factor must be greater than 0")]
    public decimal ConversionFactor { get; set; } = 1;

    public bool IsReferenceUnit { get; set; }
    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }

}
