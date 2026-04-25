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

    private InventoryAggregate() { }

    public static InventoryAggregate Create(Guid id, Guid productId, Guid productSkuId, Guid warehouseId, string createdBy)
    {
        if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
        if (productSkuId == Guid.Empty) throw new ArgumentNullException(nameof(productSkuId));
        if (productId == Guid.Empty) throw new ArgumentNullException(nameof(productId));
        if (warehouseId == Guid.Empty) throw new ArgumentNullException(nameof(warehouseId));
        if (string.IsNullOrWhiteSpace(createdBy)) throw new ArgumentNullException(nameof(createdBy));

        return new InventoryAggregate
        {
            Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    // FIFO reservation
    public List<(Guid BatchId, decimal Quantity)> ReserveFIFO(
    decimal qty,
    List<(Guid BatchId, DateTime ExpiryDate)> batchExpiries,
    string updatedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        if (batchExpiries == null || !batchExpiries.Any())
            throw new InvalidOperationException("No batch expiry info provided");

        var remaining = qty;
        var allocations = new List<(Guid BatchId, decimal Quantity)>();

        // Order available batches by expiry date provided externally
        var orderedBatches = _batches
            .Where(b => b.Available > 0)
            .OrderBy(b =>
            {
                var expiry = batchExpiries.FirstOrDefault(be => be.BatchId == b.BatchId);
                if (expiry == default) throw new InvalidOperationException($"Expiry info missing for batch {b.BatchId}");
                return expiry.ExpiryDate;
            })
            .ToList();

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

    public void TransferOut(Guid batchId, decimal qty, string updatedBy)
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

    public BatchStock FindBatch(Guid batchId) =>
        _batches.FirstOrDefault(b => b.BatchId == batchId)
        ?? throw new InvalidOperationException($"BatchStock not found: {batchId}");

    // Add or remove batch stocks
    public void AddBatch(BatchStock stock)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock));
        _batches.Add(stock);
    }

    public void RemoveBatch(Guid batchId, string deletedBy)
    {
        var batch = FindBatch(batchId);
        if (batch.ReservedQuantity > 0)
            throw new InvalidOperationException("Cannot remove batch with reserved stock");
        batch.Remove(deletedBy);
        _batches.Remove(batch);
    }
}