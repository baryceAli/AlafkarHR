using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryNameEng { get; set; }
    public Guid BrandId { get; set; }
    public string BrandName { get; set; }
    public string BrandNameEng { get; set; }
    public Guid UnitId { get; set; }
    public string UnitName { get; set; }
    public string UnitNameEng { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }

    public List<ProductSkuDto> ProductSkus { get; set; }

}
