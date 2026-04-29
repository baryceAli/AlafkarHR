using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class VariantValueDto
{
    public Guid Id { get; set; }
    public Guid VariantId { get; set; }
    
    
    [Required(ErrorMessage = "Value is required")]
    public string Value { get; set; }


    [Required(ErrorMessage = "ValueEng is required")]
    public string ValueEng { get; set; }
}
