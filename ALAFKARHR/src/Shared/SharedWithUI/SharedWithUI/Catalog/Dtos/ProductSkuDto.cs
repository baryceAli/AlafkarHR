using System.ComponentModel.DataAnnotations;

using SharedWithUI.Catalog.Enums;

namespace SharedWithUI.Catalog.Dtos;

public class ProductSkuDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public CatalogProductType ProductType { get; set; } = CatalogProductType.Goods;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryNameEng { get; set; }
    public Guid BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? BrandNameEng { get; set; }

    public Guid? PackageId { get; set; } // optional (size: 250ml, 1L)
    public string? PackageName { get; set; }
    public string? PackageNameEng { get; set; }
    public Guid? PackageUnitId { get; set; }
    public string? PackageUnitName { get; set; }
    public string? PackageUnitNameEng { get; set; }
    public string? PackageBarcode { get; set; }
    public bool IsPackage => PackageId.HasValue || Packages.Any();

    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitNameEng { get; set; }

    [Required(ErrorMessage = "Calories is required")]
    [Range(0.01, 10000000, ErrorMessage = "Calories must be greater than 0")]
    public decimal? Calories { get; set; }


    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    public string SkuCode { get;  set; } = default!;


    [Required(ErrorMessage = "SkuCodeEng is required")]
    public string NameEng { get; set; }
    public string SkuCodeEng { get;  set; } = default!;
    
    public string SkuKey { get;  set; } = default!;
    

    [Required(ErrorMessage ="Barcode is required")] 
    public string Barcode { get; set; } = default!;

    [Range(0.1,10000000,ErrorMessage ="Price must be greator than 0")]
    public decimal Price { get; set; }
    public decimal BasePrice { get; set; }
    public Guid? PriceListId { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string? PriceSource { get; set; }
    public decimal? PromotionUnitPrice { get; set; }
    public decimal FinalUnitAmount { get; set; }

    public SkuProductionType ProductionType { get; set; } = SkuProductionType.PurchasedRawMaterial;


    public string ImageUrl { get; set; }
    public Guid CompanyId { get; set; }
    public bool ShowOnStore { get; set; }
    public bool IsSellable { get; set; } = true;
    public bool IsPurchasable { get; set; } = true;
    public bool IsInventoryTracked { get; set; } = true;
    public bool IsAssetTrackable { get; set; }
    public DateTime? CreatedAt { get; set; }

    public List<ProductSkuVariantDto> Variants { get; set; } = new();
    public List<ProductPackageDto> Packages { get; set; } = new();
    public List<ProductSkuComponentDto> Components { get; set; } = new();
    

    //SKU1 Milk    Almarai      Full Cream	    2
    //SKU2 Milk    Almarai      No Cream	    3
    //SKU3 Milk    Alsafi       Full Cream	    1.5

}

public class ProductSkuComponentDto
{
    public Guid Id { get; set; }
    public Guid ParentProductSkuId { get; set; }
    public Guid ComponentProductSkuId { get; set; }
    public string? ComponentSkuName { get; set; }
    public string? ComponentSkuNameEng { get; set; }
    public string? ComponentSkuCode { get; set; }
    public string? ComponentSkuCodeEng { get; set; }

    [Range(0.0001, 10000000, ErrorMessage = "Quantity must be greator than 0")]
    public decimal Quantity { get; set; } = 1;
}

public class PublicStoreProductSkuRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; } = 12;
    public Guid? CustomerId { get; set; }
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? PackageId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "newest";
    public bool SortDescending { get; set; } = true;
}

public class PublicStoreProductSkuFilterMetadataDto
{
    public List<PublicStoreFilterOptionDto> Categories { get; set; } = [];
    public List<PublicStoreFilterOptionDto> Brands { get; set; } = [];
    public List<PublicStoreFilterOptionDto> Packages { get; set; } = [];
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public class PublicStoreFilterOptionDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? NameEng { get; set; }
}

