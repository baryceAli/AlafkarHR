using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class TransferItem:Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid? SourceLocationId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    //public Guid WarehouseId { get; set; }
    public decimal Quantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public Guid CurrencyId { get; private set; }
    public string? SerialNumbersCsv { get; private set; }
    public IReadOnlyList<string> SerialNumbers => string.IsNullOrWhiteSpace(SerialNumbersCsv)
        ? []
        : SerialNumbersCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    public bool IsCompleted => ReceivedQuantity >= Quantity;

    public TransferItem(){}
    internal TransferItem(Guid productId,
        Guid productSkuId,
        Guid batchId,
        Guid? sourceLocationId,
        Guid? destinationLocationId,
        //Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        Guid currencyId,
        IEnumerable<string>? serialNumbers,
        //decimal receivedQuantity,
        //bool isCompleted,
        string createdBy)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");
        ProductId = productId;
        ProductSkuId = productSkuId;
        BatchId = batchId;
        SourceLocationId = sourceLocationId;
        DestinationLocationId = destinationLocationId;
        //WarehouseId = warehouseId,
        Quantity = quantity;
        UnitCost = unitCost;
        CurrencyId = currencyId;
        SerialNumbersCsv = NormalizeSerialNumbers(serialNumbers);
        ReceivedQuantity = 0;
        //IsCompleted = receivedQuantity==quantity,
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
    public static TransferItem Create(
        //Guid id,
        Guid productId,
        Guid productSkuId,
        Guid batchId,
        Guid? sourceLocationId,
        Guid? destinationLocationId,
        //Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        Guid currencyId,
        IEnumerable<string>? serialNumbers,
        //decimal receivedQuantity,
        //bool isCompleted,
        string createdBy
        )
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");
        return new TransferItem
        {
            //Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            BatchId = batchId,
            SourceLocationId = sourceLocationId,
            DestinationLocationId = destinationLocationId,
            //WarehouseId = warehouseId,
            Quantity = quantity,
            UnitCost = unitCost,
            CurrencyId = currencyId,
            SerialNumbersCsv = NormalizeSerialNumbers(serialNumbers),
            ReceivedQuantity =0,
            //IsCompleted = receivedQuantity==quantity,
            CreatedAt= DateTime.UtcNow,
            CreatedBy= createdBy
        };
    }

    public void SetDestinationLocation(Guid? destinationLocationId, string user)
    {
        DestinationLocationId = destinationLocationId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void SetSourceLocation(Guid? sourceLocationId, string user)
    {
        SourceLocationId = sourceLocationId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void SetSerialNumbers(IEnumerable<string>? serialNumbers, string user)
    {
        SerialNumbersCsv = NormalizeSerialNumbers(serialNumbers);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void Receive(decimal quantity, string user)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");

        if (IsCompleted)
            throw new InvalidOperationException(
                "Item already completed");

        if (ReceivedQuantity + quantity > Quantity)
            throw new InvalidOperationException(
                "Cannot receive more than shipped quantity");

        ReceivedQuantity += quantity;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    private static string? NormalizeSerialNumbers(IEnumerable<string>? serialNumbers)
    {
        var cleaned = serialNumbers?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        return cleaned.Count == 0 ? null : string.Join(",", cleaned);
    }
}
