using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class InventoryAggregate : Aggregate<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }

    private readonly List<BatchStock> _batches = new();
    public IReadOnlyCollection<BatchStock> Batches => _batches.AsReadOnly();

    public decimal TotalQuantity => _batches.Sum(x => x.Quantity);
    public decimal TotalReserved => _batches.Sum(x => x.ReservedQuantity);
    public decimal TotalAvailable => TotalQuantity - TotalReserved;

    public Guid CompanyId { get; set; }
    public byte[] RowVersion { get; private set; } = [];
    private InventoryAggregate() { }

    public static InventoryAggregate Create(
        Guid id,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid batchId,
        decimal quantity,
        Guid companyId,
        string createdBy)
    {
        if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
        if (productSkuId == Guid.Empty) throw new ArgumentNullException(nameof(productSkuId));
        if (productId == Guid.Empty) throw new ArgumentNullException(nameof(productId));
        if (warehouseId == Guid.Empty) throw new ArgumentNullException(nameof(warehouseId));
        if (string.IsNullOrWhiteSpace(createdBy)) throw new ArgumentNullException(nameof(createdBy));



        var inventory = new InventoryAggregate
        {
            Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        inventory.StockIn(new BatchStock(batchId, warehouseId, quantity, createdBy));

        return inventory;
    }

    // FIFO reservation
    public List<(Guid BatchId, decimal Quantity)> ReserveFIFO(
    decimal qty,
    //List<(Guid BatchId, DateTime ExpiryDate)> batchExpiries,
    string updatedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        //if (batchExpiries == null || !batchExpiries.Any())
        //    throw new InvalidOperationException("No batch expiry info provided");

        var remaining = qty;
        var allocations = new List<(Guid BatchId, decimal Quantity)>();

        // Order available batches by expiry date provided externally
        //var orderedBatches = _batches
        //    .Where(b => b.Available > 0)
        //    .OrderBy(b =>
        //    {
        //        var expiry = batchExpiries.FirstOrDefault(be => be.BatchId == b.BatchId);
        //        if (expiry == default) throw new InvalidOperationException($"Expiry info missing for batch {b.BatchId}");
        //        return expiry.ExpiryDate;
        //    })
        //    .ToList();
        var orderedBatches = _batches.Where(b => b.Available > 0).OrderBy(b => b.Batch.ExpiryDate).ToList();
        //_batches[0].
        foreach (var batch in orderedBatches)
        {
            if (remaining <= 0) break;

            var take = Math.Min(batch.Available, remaining);
            batch.Reserve(take, updatedBy);
            allocations.Add((batch.BatchId, take));
            remaining -= take;
        }

        if (remaining > 0)
            throw new InvalidOperationException($"Insufficient stock. Missing quantity: {remaining}");

        return allocations;
    }

    // Transfer in/out (just calls batch-level Increase/Decrease)
    public void TransferIn(Guid batchId, decimal qty, string updatedBy)
    {
        var batch = FindBatch(batchId);
        batch.Increase(qty, updatedBy);
    }

    private void TransferOut(Guid batchId, decimal qty, string updatedBy)
    {
        var batch = FindBatch(batchId);
        batch.Decrease(qty, updatedBy);
    }

    // Reserve/release single batch
    public void Reserve(Guid batchId, decimal qty, string updatedBy)
    {
        var batch = FindBatch(batchId);
        batch.Reserve(qty, updatedBy);
    }

    public void Release(Guid batchId, decimal qty, string updatedBy)
    {
        var batch = FindBatch(batchId);
        batch.Release(qty, updatedBy);
    }

    public void ConsumeReserved(Guid batchId, decimal qty, string updatedBy)
    {
        var batch = FindBatch(batchId);
        batch.ConsumeReserved(qty, updatedBy);
    }


    // Add or remove batch stocks
    public void StockIn(BatchStock stock)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock));
        
        var batch = _batches.FirstOrDefault(b => b.BatchId == stock.BatchId && b.WarehouseId == stock.WarehouseId);
        
        if (batch == null)
        {
            _batches.Add(stock);
        }
        else
        {
            batch.Increase(stock.Quantity, stock.CreatedBy);

        }
    }
    public void StockOut(BatchStock stock)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock));

        var batch = _batches.FirstOrDefault(b => b.BatchId == stock.BatchId && b.WarehouseId == stock.WarehouseId);

        if (batch != null)
        {
            batch.Decrease(stock.Quantity, stock.ModifiedBy);

        }
        else
        {
            throw new InvalidOperationException($"No stock could be found");
        }
    }

    public void RemoveBatchStock(Guid batchId, string deletedBy)
    {
        var batch = FindBatch(batchId);
        if (batch.ReservedQuantity > 0)
            throw new InvalidOperationException("Cannot remove batch with reserved stock");
        batch.Remove(deletedBy);
        _batches.Remove(batch);
    }
    public BatchStock FindBatch(Guid batchId) =>
        _batches.FirstOrDefault(b => b.BatchId == batchId)
        ?? throw new InvalidOperationException($"BatchStock not found: {batchId}");

}
