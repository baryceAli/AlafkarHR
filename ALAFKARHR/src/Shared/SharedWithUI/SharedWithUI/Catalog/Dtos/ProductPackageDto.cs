using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Catalog.Dtos;

public class ProductPackageDto
{
    public Guid Id { get; set; }

    public string Name { get; private set; } // 250ml, 1L, 500g
    public string NameEng { get; private set; } // 250ml, 1L, 500g
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid CompanyId { get; set; }

}


