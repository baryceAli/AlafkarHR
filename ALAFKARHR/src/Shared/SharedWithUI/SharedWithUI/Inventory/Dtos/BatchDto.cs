using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for displaying Batch information (metadata only)
/// </summary>
public class BatchDto
{

    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public Guid? ProductId { get; set; }


    [Required(ErrorMessage = "ProductSku is required")]
    public Guid? ProductSkuId { get; set; }


    [Required(ErrorMessage = "BatchNumber is required")]
    public string BatchNumber { get; set; }


    [Required(ErrorMessage = "ManufacturingDate is required")]
    public DateTime ManufacturingDate { get; set; }


    [Required(ErrorMessage = "ExpiryDate is required")]
    public DateTime ExpiryDate { get; set; }


    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

}

/// <summary>
/// DTO for creating a Batch
/// </summary>
public class CreateBatchDto
{
    //Guid WarehouseId,

    [Required(ErrorMessage = "Product is required")]
    public Guid ProductId { get; set; }


    [Required(ErrorMessage = "ProductSku is required")]
    public Guid ProductSkuId { get; set; }


    [Required(ErrorMessage = "BatchNumber is required")]
    public string BatchNumber { get; set; }


    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }


    [Required(ErrorMessage = "ManufacturingDate is required")]
    public DateTime ManufacturingDate { get; set; }


    [Required(ErrorMessage = "ExpiryDate is required")]
    public DateTime ExpiryDate { get; set; }

}


/// <summary>
/// DTO for updating a Batch
/// </summary>
public class UpdateBatchDto{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "BatchNumber is required")]
    public string BatchNumber { get; set; }
    public DateTime ManufacturingDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    
}
