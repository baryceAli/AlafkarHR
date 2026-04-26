using Shared.DDD;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Warehouses.Models;

public class Batch : Aggregate<Guid>
{
    //public Guid WarehouseId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string BatchNumber { get; private set; }
    public DateTime ManufacturingDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public Guid CompanyId { get; set; }
    private Batch() { }
    public static Batch Create(Guid id,
        //Guid warehouseId,
        Guid productId,
        Guid productSkuId,
        string batchNumber,
        DateTime manufacturingDate,
        DateTime expiryDate,
        Guid companyId,
        string createdBy)
    {
        
        var batch= new Batch()
        {
            Id = id,
            //WarehouseId = warehouseId,
            ProductId=productId,
            ProductSkuId = productSkuId,
            BatchNumber = batchNumber,
            ManufacturingDate = manufacturingDate,
            ExpiryDate = expiryDate,
            CreatedAt = DateTime.UtcNow,
            CompanyId = companyId,
            CreatedBy = createdBy
        };
        
        batch.ValidateDates();
        
        return batch;
    }
    public void Update(
        string batchNumber,
        DateTime manufacturingDate,
        DateTime expiryDate,
        string modifiedBy)
    {
        ValidateDates();
        BatchNumber = batchNumber;
        ManufacturingDate = manufacturingDate;
        ExpiryDate = expiryDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

    }
    public bool IsExpired(DateTime now)
    {
        return ExpiryDate < now;
    }
    private void ValidateDates()
    {
        if (ExpiryDate <= ManufacturingDate)
            throw new InvalidOperationException("Expiry must be after manufacturing date");
    }

    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
