namespace SharedWithUI.Inventory.Dtos;

public class SkuAvailabilityDto
{
    public Guid CompanyId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitNameEng { get; set; }
    public string? UnitCategory { get; set; }
    public decimal UnitConversionFactor { get; set; } = 1;
    public decimal TotalQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public List<SkuAvailabilityWarehouseDto> Warehouses { get; set; } = [];
}

public class SkuAvailabilityWarehouseDto
{
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid? BranchId { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public List<SkuAvailabilityBatchDto> Batches { get; set; } = [];
    public List<SkuAvailabilityLocationDto> Locations { get; set; } = [];
}

public class SkuAvailabilityBatchDto
{
    public Guid BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
}

public class SkuAvailabilityLocationDto
{
    public Guid WarehouseLocationId { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationName { get; set; }
    public string? LocationNameEng { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
}
