using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductSkuDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Variant is required")]
    public Guid? VariantId { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public Guid? ProductId { get; set; }

    [Required(ErrorMessage = "Package is required")]
    public Guid? PackageId { get; set; }

    [Required(ErrorMessage = "Sku is required")]
    public string Sku { get; set; }

    [Required(ErrorMessage = "SkuEng is required")]
    public string SkuEng { get; set; }

    [Required(ErrorMessage = "VariantValue is required")]
    public string VariantValue { get; set; }


    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }


    public decimal Price { get; set; }
    public bool ShowOnStore { get; set; }
    

}

