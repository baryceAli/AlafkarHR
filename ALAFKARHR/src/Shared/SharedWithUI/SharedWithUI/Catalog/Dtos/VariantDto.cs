using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class VariantDto
    
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }

    [Required(ErrorMessage ="Company is required")]
    public Guid? CompanyId { get; set; }

    public List<VariantValueDto> Values { get; set; } = new();

}



