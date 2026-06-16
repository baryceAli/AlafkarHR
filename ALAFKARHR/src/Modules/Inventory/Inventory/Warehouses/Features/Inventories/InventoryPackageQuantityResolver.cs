using Catalog.Contracts.Products.Features.GetProductById;
using Shared.Exceptions;
using SharedWithUI.Catalog.Dtos;

namespace Inventory.Warehouses.Features.Inventories;

public sealed record InventoryPackageQuantityResult(
    ProductSkuDto Sku,
    Guid? ProductPackageId,
    decimal EnteredQuantity,
    decimal PackageMultiplier,
    decimal NormalizedQuantity);

public static class InventoryPackageQuantityResolver
{
    public static async Task<InventoryPackageQuantityResult> ResolveAsync(
        ISender sender,
        CreateInventoryAggregateDto inventoryAggregate,
        CancellationToken cancellationToken)
    {
        if (!inventoryAggregate.ProductId.HasValue || inventoryAggregate.ProductId.Value == Guid.Empty)
            throw new NotFoundException("Product is required");

        if (!inventoryAggregate.ProductSkuId.HasValue || inventoryAggregate.ProductSkuId.Value == Guid.Empty)
            throw new NotFoundException("SKU is required");

        var productId = inventoryAggregate.ProductId.Value;
        var productSkuId = inventoryAggregate.ProductSkuId.Value;

        var prodRes = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);
        if (prodRes.Product is null)
            throw new NotFoundException($"Product not found: {productId}");

        var sku = prodRes.Product.Skus.FirstOrDefault(s => s.Id == productSkuId);
        if (sku is null)
            throw new NotFoundException($"SKU not found: {productSkuId}");

        var packageMultiplier = 1m;
        Guid? productPackageId = null;

        if (inventoryAggregate.ProductPackageId.HasValue && inventoryAggregate.ProductPackageId.Value != Guid.Empty)
        {
            var selectedPackage = sku.Packages.FirstOrDefault(p => p.Id == inventoryAggregate.ProductPackageId.Value);
            if (selectedPackage is null)
                throw new NotFoundException($"Package ({inventoryAggregate.ProductPackageId}) is not linked to SKU ({productSkuId})");

            productPackageId = selectedPackage.Id;
            packageMultiplier = selectedPackage.Quantity;
        }

        return new InventoryPackageQuantityResult(
            sku,
            productPackageId,
            inventoryAggregate.InitialQuantity,
            packageMultiplier,
            inventoryAggregate.InitialQuantity * packageMultiplier);
    }
}
