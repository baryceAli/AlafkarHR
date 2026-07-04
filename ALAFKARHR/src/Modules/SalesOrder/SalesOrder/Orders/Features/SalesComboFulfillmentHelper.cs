using Catalog.Contracts.Products.Features.GetProductSkuComponentBreakdown;
using Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;
using Inventory.Contracts.Stock;
using SharedWithUI.Catalog.Enums;

namespace SalesOrder.Orders.Features;

internal static class SalesComboFulfillmentHelper
{
    public static bool IsCombo(GetProductSkuInventoryContextResult context) =>
        context.ProductType == CatalogProductType.Combo ||
        context.ProductionType == SkuProductionType.CompositeBundle;

    public static async Task ReserveAsync(
        ISender sender,
        Guid companyId,
        Guid? branchId,
        Guid warehouseId,
        SalesOrder.Orders.Models.SalesOrderLine line,
        decimal parentQuantity,
        string referenceNumber,
        Guid? sourceDocumentId,
        Guid? sourceDocumentLineId,
        CancellationToken cancellationToken)
    {
        var components = await GetTrackedComponentsAsync(sender, companyId, line, parentQuantity, cancellationToken);
        foreach (var component in components)
        {
            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(companyId, component.ComponentProductSkuId, warehouseId, branchId),
                cancellationToken);

            if (availability.AvailableQuantity < component.RequiredQuantity)
                throw new BadRequestException($"Insufficient available stock for component SKU {component.ComponentSkuCode} in combo {line.SkuCode}.");

            var remaining = component.RequiredQuantity;
            foreach (var batch in availability.Warehouses
                         .SelectMany(x => x.Batches)
                         .Where(x => x.AvailableQuantity > 0)
                         .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue))
            {
                if (remaining <= 0)
                    break;

                var take = Math.Min(batch.AvailableQuantity, remaining);
                await sender.Send(new PostInventoryReservationCommand(
                    component.ComponentProductId,
                    component.ComponentProductSkuId,
                    warehouseId,
                    batch.BatchId,
                    take,
                    companyId,
                    $"Sales order reservation {referenceNumber} for combo {line.SkuCode}",
                    referenceNumber,
                    "SalesOrderReservation",
                    component.UnitId,
                    null,
                    sourceDocumentId,
                    sourceDocumentLineId,
                    line.ProductSkuId,
                    line.Id), cancellationToken);

                remaining -= take;
            }

            if (remaining > 0)
                throw new BadRequestException($"Insufficient batch stock for component SKU {component.ComponentSkuCode} in combo {line.SkuCode}.");
        }
    }

    public static async Task ReleaseAsync(
        ISender sender,
        Guid companyId,
        Guid? branchId,
        Guid warehouseId,
        SalesOrder.Orders.Models.SalesOrderLine line,
        decimal parentQuantity,
        string referenceNumber,
        Guid? sourceDocumentId,
        Guid? sourceDocumentLineId,
        CancellationToken cancellationToken)
    {
        var components = await GetTrackedComponentsAsync(sender, companyId, line, parentQuantity, cancellationToken);
        foreach (var component in components)
        {
            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(companyId, component.ComponentProductSkuId, warehouseId, branchId),
                cancellationToken);

            var remaining = component.RequiredQuantity;
            foreach (var batch in availability.Warehouses
                         .SelectMany(x => x.Batches)
                         .Where(x => x.ReservedQuantity > 0)
                         .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue))
            {
                if (remaining <= 0)
                    break;

                var take = Math.Min(batch.ReservedQuantity, remaining);
                await sender.Send(new PostInventoryReleaseCommand(
                    component.ComponentProductId,
                    component.ComponentProductSkuId,
                    warehouseId,
                    batch.BatchId,
                    take,
                    companyId,
                    $"Sales order reservation release {referenceNumber} for combo {line.SkuCode}",
                    referenceNumber,
                    "SalesOrderReservationRelease",
                    component.UnitId,
                    null,
                    sourceDocumentId,
                    sourceDocumentLineId,
                    line.ProductSkuId,
                    line.Id), cancellationToken);

                remaining -= take;
            }

            if (remaining > 0)
                throw new BadRequestException($"Reserved stock could not be released for component SKU {component.ComponentSkuCode} in combo {line.SkuCode}.");
        }
    }

    public static async Task ConsumeReservedAsync(
        ISender sender,
        Guid companyId,
        Guid? branchId,
        Guid warehouseId,
        SalesOrder.Orders.Models.SalesOrderLine orderLine,
        Guid currencyId,
        decimal parentQuantity,
        string referenceNumber,
        Guid sourceDocumentId,
        Guid sourceDocumentLineId,
        CancellationToken cancellationToken)
    {
        var components = await GetTrackedComponentsAsync(sender, companyId, orderLine, parentQuantity, cancellationToken);
        foreach (var component in components)
        {
            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(companyId, component.ComponentProductSkuId, warehouseId, branchId),
                cancellationToken);

            var remaining = component.RequiredQuantity;
            foreach (var batch in availability.Warehouses
                         .SelectMany(x => x.Batches)
                         .Where(x => x.ReservedQuantity > 0)
                         .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue))
            {
                if (remaining <= 0)
                    break;

                var take = Math.Min(batch.ReservedQuantity, remaining);
                await sender.Send(new PostInventoryStockOutCommand(
                    component.ComponentProductId,
                    component.ComponentProductSkuId,
                    null,
                    warehouseId,
                    batch.BatchId,
                    take,
                    0m,
                    0m,
                    currencyId,
                    companyId,
                    $"Sales delivery {referenceNumber} for combo {orderLine.SkuCode}",
                    referenceNumber,
                    "SalesDeliveryNote",
                    true,
                    component.UnitId,
                    sourceDocumentId,
                    sourceDocumentLineId,
                    orderLine.ProductSkuId,
                    orderLine.Id), cancellationToken);

                remaining -= take;
            }

            if (remaining > 0)
                throw new BadRequestException($"Reserved component stock could not be consumed for component SKU {component.ComponentSkuCode} in combo {orderLine.SkuCode}.");
        }
    }

    public static async Task StockInReturnAsync(
        ISender sender,
        Guid companyId,
        Guid warehouseId,
        SalesOrder.Orders.Models.SalesOrderLine orderLine,
        Guid? deliveryNoteId,
        Guid? deliveryNoteLineId,
        Guid salesReturnId,
        Guid salesReturnLineId,
        Guid currencyId,
        decimal parentQuantity,
        decimal deliveredParentQuantity,
        string referenceNumber,
        CancellationToken cancellationToken)
    {
        if (!deliveryNoteId.HasValue || !deliveryNoteLineId.HasValue)
            throw new BadRequestException($"Combo return for SKU {orderLine.SkuCode} must reference the original delivery note line.");

        var deliveredMovements = await sender.Send(
            new GetStockMovementsBySourceQuery(companyId, "SalesDeliveryNote", deliveryNoteId.Value),
            cancellationToken);

        var componentMovements = deliveredMovements.Movements
            .Where(x => x.SourceDocumentLineId == deliveryNoteLineId.Value
                && x.ParentProductSkuId == orderLine.ProductSkuId
                && x.ParentSalesOrderLineId == orderLine.Id
                && x.QuantityAfter < x.QuantityBefore)
            .ToList();

        if (componentMovements.Count == 0)
            throw new BadRequestException($"Original component movement trace was not found for combo return {orderLine.SkuCode}. Create a return from a traced delivery note line.");

        if (deliveredParentQuantity <= 0)
            throw new BadRequestException($"Original delivered quantity is required for combo return {orderLine.SkuCode}.");

        var ratio = parentQuantity / deliveredParentQuantity;
        foreach (var movement in componentMovements)
        {
            var returnQuantity = decimal.Round(movement.NormalizedQuantity * ratio, 4, MidpointRounding.AwayFromZero);
            if (returnQuantity <= 0)
                continue;

            await sender.Send(new PostInventoryStockInCommand(
                movement.ProductId,
                movement.ProductSkuId,
                null,
                warehouseId,
                movement.BatchId,
                returnQuantity,
                0m,
                0m,
                currencyId,
                companyId,
                $"Sales return {referenceNumber} for combo {orderLine.SkuCode}",
                referenceNumber,
                "SalesReturn",
                movement.UnitId,
                salesReturnId,
                salesReturnLineId,
                orderLine.ProductSkuId,
                orderLine.Id), cancellationToken);
        }
    }

    public static async Task EnsurePosAvailableAsync(
        ISender sender,
        Guid companyId,
        Guid? branchId,
        Guid warehouseId,
        SalesOrderLineDto line,
        CancellationToken cancellationToken)
    {
        var components = await GetTrackedComponentsAsync(sender, companyId, line.ProductSkuId, line.SkuCode, line.Quantity, cancellationToken);
        foreach (var component in components)
        {
            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(companyId, component.ComponentProductSkuId, warehouseId, branchId),
                cancellationToken);

            if (availability.AvailableQuantity < component.RequiredQuantity)
                throw new BadRequestException($"Insufficient available stock for component SKU {component.ComponentSkuCode} in combo {line.SkuCode}.");
        }
    }

    public static async Task ConsumePosAsync(
        ISender sender,
        Guid companyId,
        Guid warehouseId,
        SalesOrder.Orders.Models.SalesOrderLine line,
        Guid sourceDocumentId,
        Guid sourceDocumentLineId,
        string referenceNumber,
        CancellationToken cancellationToken)
    {
        var components = await GetTrackedComponentsAsync(sender, companyId, line, line.Quantity, cancellationToken);
        foreach (var component in components)
        {
            await sender.Send(new PostInventoryStockOutBySkuCommand(
                component.ComponentProductId,
                component.ComponentProductSkuId,
                null,
                warehouseId,
                component.RequiredQuantity,
                0m,
                0m,
                null,
                companyId,
                $"POS sale {referenceNumber} for combo {line.SkuCode}",
                referenceNumber,
                "POSDirectSale",
                component.UnitId,
                sourceDocumentId,
                sourceDocumentLineId,
                line.ProductSkuId,
                line.Id), cancellationToken);
        }
    }

    private static Task<IReadOnlyList<ProductSkuComponentBreakdownRow>> GetTrackedComponentsAsync(
        ISender sender,
        Guid companyId,
        SalesOrder.Orders.Models.SalesOrderLine line,
        decimal parentQuantity,
        CancellationToken cancellationToken) =>
        GetTrackedComponentsAsync(sender, companyId, line.ProductSkuId, line.SkuCode, parentQuantity, cancellationToken);

    private static async Task<IReadOnlyList<ProductSkuComponentBreakdownRow>> GetTrackedComponentsAsync(
        ISender sender,
        Guid companyId,
        Guid productSkuId,
        string skuCode,
        decimal parentQuantity,
        CancellationToken cancellationToken)
    {
        var breakdown = await sender.Send(
            new GetProductSkuComponentBreakdownQuery(companyId, productSkuId, parentQuantity),
            cancellationToken);

        if (!breakdown.ParentProductIsActive || !breakdown.ParentSkuIsActive)
            throw new BadRequestException($"Combo SKU {skuCode} is archived or inactive.");

        var trackedComponents = new List<ProductSkuComponentBreakdownRow>();
        foreach (var component in breakdown.Components)
        {
            if (!component.ProductIsActive ||
                !component.SkuIsActive ||
                !component.CategoryIsActive ||
                !component.BrandIsActive ||
                !component.UnitIsActive)
            {
                throw new BadRequestException($"Component SKU {component.ComponentSkuCode} in combo {skuCode} is archived or has archived Catalog references.");
            }

            if (component.ProductType == CatalogProductType.Combo ||
                component.ProductionType == SkuProductionType.CompositeBundle)
            {
                throw new BadRequestException($"Combo SKU {skuCode} contains nested combo component {component.ComponentSkuCode}, which is not supported in this tranche.");
            }

            if (component.ProductType == CatalogProductType.Service || !component.IsInventoryTracked)
                continue;

            if (component.RequiredQuantity <= 0)
                throw new BadRequestException($"Component SKU {component.ComponentSkuCode} in combo {skuCode} has invalid quantity.");

            trackedComponents.Add(component);
        }

        return trackedComponents;
    }
}
