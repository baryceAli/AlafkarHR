using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductPackageDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } // 250ml, 1L, 500g
    public string NameEng { get; set; } // 250ml, 1L, 500g
    public decimal Quantity { get; set; }
    //public Guid UnitId { get; set; }
    public Guid CompanyId { get; set; }

}


