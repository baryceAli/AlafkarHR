using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductSkuDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; private set; }
    public Guid BrandId { get; private set; }

    public Guid? PackageId { get; private set; } // optional (size: 250ml, 1L)

    public string SkuCode { get; private set; } = default!;
    public string SkuCodeEng { get; private set; } = default!;
    public string Barcode { get; private set; } = default!;

    public decimal Price { get; private set; }
    public string ImageUrl { get; set; }
    public Guid CompanyId { get; set; }
    public bool ShowOnStore { get; private set; }

    public List<ProductSkuVariantDto> Variants { get; set; } = new();
    

    //SKU1 Milk    Almarai      Full Cream	    2
    //SKU2 Milk    Almarai      No Cream	    3
    //SKU3 Milk    Alsafi       Full Cream	    1.5

}

