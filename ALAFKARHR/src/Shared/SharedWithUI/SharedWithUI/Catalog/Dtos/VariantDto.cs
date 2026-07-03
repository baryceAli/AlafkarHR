using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

using SharedWithUI.Catalog.Enums;

public class VariantDto
    
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }

    public VariantDisplayType DisplayType { get; set; } = VariantDisplayType.Pills;
    public VariantCreationMode CreationMode { get; set; } = VariantCreationMode.Instant;

    [Required(ErrorMessage ="Company is required")]
    public Guid? CompanyId { get; set; }

    public List<VariantValueDto> Values { get; set; } = new();

}



