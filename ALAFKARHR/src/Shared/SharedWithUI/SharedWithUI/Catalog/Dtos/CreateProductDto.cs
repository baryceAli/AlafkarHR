using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

using SharedWithUI.Catalog.Enums;

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
    public CatalogProductType ProductType { get; set; } = CatalogProductType.Goods;
    public string? SalesDescription { get; set; }
    public string? PurchaseDescription { get; set; }
    public decimal CustomerTaxRate { get; set; }
    public decimal VendorTaxRate { get; set; }
    public Guid? IncomeAccountId { get; set; }
    public Guid? ExpenseAccountId { get; set; }
    public ProductCostingPolicy CostingPolicy { get; set; } = ProductCostingPolicy.Standard;

    [Required(ErrorMessage ="Price is required")]
    [Range(0.01,10000000,ErrorMessage ="Price must be greator than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Image is required")]
    public string ImageUrl { get; set; } = default!;
    
    }
    

