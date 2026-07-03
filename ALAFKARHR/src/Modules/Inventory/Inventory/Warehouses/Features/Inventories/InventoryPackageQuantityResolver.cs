using Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;
using Shared.Exceptions;
using SharedWithUI.Catalog.Enums;

namespace Inventory.Warehouses.Features.Inventories;

public sealed record InventoryPackageQuantityResult(
    Guid ProductId,
    Guid ProductSkuId,
    Guid? ProductPackageId,
    Guid UnitId,
    decimal EnteredQuantity,
    decimal PackageMultiplier,
    decimal UnitMultiplier,
    decimal NormalizedQuantity);

public static class InventoryPackageQuantityResolver
{
    public static async Task<InventoryPackageQuantityResult> ResolveAsync(
        ISender sender,
        CreateInventoryAggregateDto inventoryAggregate,
        CancellationToken cancellationToken)
    {
        if (!inventoryAggregate.ProductSkuId.HasValue || inventoryAggregate.ProductSkuId.Value == Guid.Empty)
            throw new NotFoundException("SKU is required");

        var productSkuId = inventoryAggregate.ProductSkuId.Value;

        var skuContext = await sender.Send(
            new GetProductSkuInventoryContextQuery(inventoryAggregate.CompanyId, productSkuId),
            cancellationToken);

        if (inventoryAggregate.ProductId.HasValue
            && inventoryAggregate.ProductId.Value != Guid.Empty
            && inventoryAggregate.ProductId.Value != skuContext.ProductId)
            throw new Exception("Selected product does not match the selected SKU.");

        ValidateCatalogState(skuContext);

        var packageMultiplier = 1m;
        Guid? productPackageId = null;
        GetProductSkuInventoryPackageResult? selectedPackage = null;

        if (inventoryAggregate.ProductPackageId.HasValue && inventoryAggregate.ProductPackageId.Value != Guid.Empty)
        {
            selectedPackage = skuContext.Packages.FirstOrDefault(p => p.ProductPackageId == inventoryAggregate.ProductPackageId.Value);
            if (selectedPackage is null)
                throw new NotFoundException($"Package ({inventoryAggregate.ProductPackageId}) is not linked to SKU ({productSkuId})");

            if (!selectedPackage.IsActive)
                throw new Exception("Selected package is archived and cannot be used for new inventory operations.");

            if (!selectedPackage.UnitIsActive)
                throw new Exception("Selected package unit is archived and cannot be used for new inventory operations.");

            productPackageId = selectedPackage.ProductPackageId;
            packageMultiplier = selectedPackage.Quantity;
        }

        var unit = ResolveSelectedUnit(inventoryAggregate.UnitId, skuContext, selectedPackage);
        var unitMultiplier = unit.ConversionFactor / skuContext.UnitConversionFactor;

        return new InventoryPackageQuantityResult(
            skuContext.ProductId,
            skuContext.ProductSkuId,
            productPackageId,
            unit.UnitId,
            inventoryAggregate.InitialQuantity,
            packageMultiplier,
            unitMultiplier,
            inventoryAggregate.InitialQuantity * packageMultiplier * unitMultiplier);
    }

    private static void ValidateCatalogState(GetProductSkuInventoryContextResult skuContext)
    {
        if (!skuContext.ProductIsActive)
            throw new Exception("Product is archived and cannot be used for new inventory operations.");

        if (!skuContext.SkuIsActive)
            throw new Exception("SKU is archived and cannot be used for new inventory operations.");

        if (!skuContext.CategoryIsActive)
            throw new Exception("Product category is archived and cannot be used for new inventory operations.");

        if (!skuContext.BrandIsActive)
            throw new Exception("Product brand is archived and cannot be used for new inventory operations.");

        if (!skuContext.UnitIsActive)
            throw new Exception("SKU unit is archived and cannot be used for new inventory operations.");

        if (skuContext.ProductType == CatalogProductType.Service)
            throw new Exception("Service products cannot be used in inventory operations.");

        if (skuContext.ProductType == CatalogProductType.Combo || skuContext.ProductionType == SkuProductionType.CompositeBundle)
            throw new Exception("Combo and composite bundle SKUs cannot be moved directly in inventory operations.");

        if (!skuContext.IsInventoryTracked)
            throw new Exception("SKU is not inventory-tracked.");
    }

    private static InventorySelectedUnit ResolveSelectedUnit(
        Guid? selectedUnitId,
        GetProductSkuInventoryContextResult skuContext,
        GetProductSkuInventoryPackageResult? selectedPackage)
    {
        var requestedUnitId = selectedUnitId.GetValueOrDefault();

        if (requestedUnitId == Guid.Empty || requestedUnitId == skuContext.UnitId)
        {
            return new InventorySelectedUnit(skuContext.UnitId, skuContext.UnitCategory, skuContext.UnitConversionFactor);
        }

        if (selectedPackage?.UnitId == requestedUnitId)
        {
            if (!string.Equals(selectedPackage.UnitCategory, skuContext.UnitCategory, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Selected package unit must use the same unit category as the SKU base unit.");

            if (!selectedPackage.UnitConversionFactor.HasValue || selectedPackage.UnitConversionFactor <= 0)
                throw new Exception("Selected package unit has an invalid conversion factor.");

            return new InventorySelectedUnit(requestedUnitId, selectedPackage.UnitCategory!, selectedPackage.UnitConversionFactor.Value);
        }

        throw new Exception("Selected unit is not valid for this SKU/package.");
    }

    private sealed record InventorySelectedUnit(Guid UnitId, string UnitCategory, decimal ConversionFactor);
}
