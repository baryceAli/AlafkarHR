using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class BrandDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; } = default!;

    
    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; } = default!;
    
    
    [Required(ErrorMessage = "Company is required")] 
    public Guid? CompanyId { get; set; }
    
    public string? Description { get; set; }
    
}
