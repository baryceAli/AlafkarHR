using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for BatchStock (quantity tracking per batch)
/// </summary>
public class BatchStockDto
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal Available { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

}


public record CreateBatchStockDto
{
    public Guid Id { get; set; }

    
    [Required(ErrorMessage = "Batch is required")]
    public Guid? BatchId { get; set; }
    
    public decimal Quantity { get; set; }


    [Required(ErrorMessage ="Company is required")]
    public Guid CompanyId { get; set; }
}

public class RemoveBatchStockDto
{
    [Required (ErrorMessage ="Inventory is required")]
    public Guid InventoryId { get; set; }


    [Required(ErrorMessage = "Batch is required")]
    public Guid BatchId { get; set; }

    public string? Notes { get; set; }

}

