using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductPackageDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } // 250ml, 1L, 500g
    public string NameEng { get; set; } // 250ml, 1L, 500g
    public decimal Quantity { get; set; }
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitNameEng { get; set; }
    public string? UnitCategory { get; set; }
    public decimal UnitConversionFactor { get; set; } = 1;
    public string? Barcode { get; set; }
    [Range(0, 10000000, ErrorMessage = "Weight cannot be negative")]
    public decimal? Weight { get; set; }
    [Range(0, 10000000, ErrorMessage = "Length cannot be negative")]
    public decimal? Length { get; set; }
    [Range(0, 10000000, ErrorMessage = "Width cannot be negative")]
    public decimal? Width { get; set; }
    [Range(0, 10000000, ErrorMessage = "Height cannot be negative")]
    public decimal? Height { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CompanyId { get; set; }

}


