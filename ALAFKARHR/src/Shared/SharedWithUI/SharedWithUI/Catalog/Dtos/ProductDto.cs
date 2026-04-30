using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }


    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }


    //[Required(ErrorMessage = "Category is required")]
    public Guid? CategoryId { get;  set; }


    public string? CategoryName { get; set; }
    public string? CategoryNameEng { get; set; }


    //[Required(ErrorMessage = "Unit is required")]
    public Guid? UnitId { get;  set; }
    public string? UnitName { get; set; }
    public string? UnitNameEng { get; set; }
    public Guid CompanyId { get; set; }


    public List<ProductSkuDto> Skus { get; set; } = new();
    


    
}
