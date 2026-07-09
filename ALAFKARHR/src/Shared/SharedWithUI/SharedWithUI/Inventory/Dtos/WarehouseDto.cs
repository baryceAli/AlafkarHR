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
    public string? ShortCode { get; set; }
    public WarehouseOperationFlow InboundFlow { get; set; } = WarehouseOperationFlow.OneStep;
    public WarehouseOperationFlow OutboundFlow { get; set; } = WarehouseOperationFlow.OneStep;
    public Guid? DefaultSourceLocationId { get; set; }
    public Guid? DefaultDestinationLocationId { get; set; }
    public Guid? DefaultQualityLocationId { get; set; }
    public Guid? DefaultPackingLocationId { get; set; }
    public Guid? DefaultOutputLocationId { get; set; }
    public Guid? DefaultTransitLocationId { get; set; }
    public List<Guid> ResupplyFromWarehouseIds { get; set; } = [];

}
