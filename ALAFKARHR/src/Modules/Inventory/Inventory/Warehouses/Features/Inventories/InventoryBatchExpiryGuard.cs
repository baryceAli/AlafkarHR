namespace Inventory.Warehouses.Features.Inventories;

internal static class InventoryBatchExpiryGuard
{
    public static void EnsureUsableForOutbound(InventoryAggregate inventory, Guid batchId, string? sourceDocumentType = null)
    {
        var batchStock = inventory.Batches.FirstOrDefault(x => x.BatchId == batchId)
            ?? throw new NotFoundException($"Batch stock not found: {batchId}");

        if (string.Equals(sourceDocumentType, "SupplierReturn", StringComparison.OrdinalIgnoreCase))
            return;

        if (batchStock.Batch.ExpiryDate.Date < DateTime.UtcNow.Date)
            throw new BadRequestException($"Batch {batchStock.Batch.BatchNumber} is expired and cannot be used for stock-out or reservation.");
    }
}
