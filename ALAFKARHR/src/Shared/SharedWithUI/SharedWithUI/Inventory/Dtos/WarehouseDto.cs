using System.ComponentModel.DataAnnotations;

using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public class WarehouseDto
{
    public Guid Id { get; set; }


    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "NameEng is required")]
    public string NameEng { get; set; }
    
    public string Location { get; set; }
    
    public string? Address { get; set; }
    
    [Required(ErrorMessage = "Longitude is required")]
    public double Longitude { get; set; }
    
    
    [Required(ErrorMessage = "Latitude is required")]
    public double Latitude { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public WarehouseType WarehouseType { get; set; } = WarehouseType.Commercial;

}
