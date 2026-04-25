using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class CreateProductDto
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage ="Category is required")]
    public Guid? CategoryId { get; set; }

    
    [Required(ErrorMessage = "Brand is required")]
    public Guid? BrandId { get; set; }
    
    
    [Required(ErrorMessage = "Unit is required")]
    public Guid? UnitId { get; set; }
    
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    
    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }
    [Required(ErrorMessage ="Price is required")]
    [Range(0.01,10000000,ErrorMessage ="Price must be greator than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Image is required")]
    public string ImageUrl { get; set; } = default!;
    
    }
    

