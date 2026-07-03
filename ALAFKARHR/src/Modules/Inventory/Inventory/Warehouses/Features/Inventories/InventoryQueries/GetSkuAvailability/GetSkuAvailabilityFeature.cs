using Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;
using Inventory.Contracts.Stock;

namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetSkuAvailability;

public class GetSkuAvailabilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/availability/company/{companyId:guid}/sku/{productSkuId:guid}", async (
                Guid companyId,
                Guid productSkuId,
                Guid? warehouseId,
                Guid? branchId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetSkuAvailabilityQuery(companyId, productSkuId, warehouseId, branchId));
                return Results.Ok(new GetSkuAvailabilityResponse(result));
            })
            .WithName("GetSkuAvailability")
            .Produces<GetSkuAvailabilityResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }
}

public record GetSkuAvailabilityResponse(GetSkuAvailabilityResult Availability);

public class GetSkuAvailabilityHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetSkuAvailabilityQuery, GetSkuAvailabilityResult>
{
    public async Task<GetSkuAvailabilityResult> Handle(GetSkuAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var skuContext = await sender.Send(
            new GetProductSkuInventoryContextQuery(request.CompanyId, request.ProductSkuId),
            cancellationToken);

        if (skuContext.ProductType == SharedWithUI.Catalog.Enums.CatalogProductType.Service || !skuContext.IsInventoryTracked)
            throw new BadRequestException("Availability can only be checked for inventory-tracked goods SKUs.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's stock availability.");

        if (request.WarehouseId.HasValue)
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext,
                sender,
                request.CompanyId,
                request.WarehouseId,
                cancellationToken);
        }

        var warehouseQuery = dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (request.WarehouseId.HasValue)
            warehouseQuery = warehouseQuery.Where(x => x.Id == request.WarehouseId.Value);

        if (branchAccess.CanViewAllBranches)
        {
            if (request.BranchId.HasValue)
                warehouseQuery = warehouseQuery.Where(x => x.BranchId == request.BranchId.Value);
        }
        else
        {
            warehouseQuery = request.BranchId.HasValue
                ? warehouseQuery.Where(x => x.BranchId == null || x.BranchId == request.BranchId.Value)
                : warehouseQuery.Where(x => x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)));
        }

        var warehouseRows = await warehouseQuery
            .Select(warehouse => new
            {
                warehouse.Id,
                warehouse.Name,
                warehouse.NameEng,
                warehouse.BranchId,
                Inventories = dbContext.Inventories
                    .Where(inventory => inventory.CompanyId == request.CompanyId
                        && inventory.ProductSkuId == request.ProductSkuId
                        && inventory.WarehouseId == warehouse.Id
                        && !inventory.IsDeleted)
                    .Select(inventory => new
                    {
                        inventory.TotalQuantity,
                        inventory.TotalReserved,
                        inventory.TotalAvailable,
                        Batches = inventory.Batches
                            .Where(batch => !batch.IsDeleted)
                            .Select(batch => new SkuAvailabilityBatchRow(
                                batch.BatchId,
                                batch.Batch.BatchNumber,
                                batch.Batch.ExpiryDate,
                                batch.Quantity,
                                batch.ReservedQuantity,
                                batch.Available))
                            .ToList()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var warehouses = warehouseRows
            .Select(warehouse =>
            {
                var batches = warehouse.Inventories
                    .SelectMany(inventory => inventory.Batches)
                    .OrderBy(batch => batch.ExpiryDate ?? DateTime.MaxValue)
                    .ThenBy(batch => batch.BatchNumber)
                    .ToList();

                return new SkuAvailabilityWarehouseRow(
                    warehouse.Id,
                    warehouse.Name,
                    warehouse.NameEng,
                    warehouse.BranchId,
                    warehouse.Inventories.Sum(inventory => inventory.TotalQuantity),
                    warehouse.Inventories.Sum(inventory => inventory.TotalReserved),
                    warehouse.Inventories.Sum(inventory => inventory.TotalAvailable),
                    batches);
            })
            .Where(warehouse => warehouse.TotalQuantity != 0 || warehouse.ReservedQuantity != 0 || request.WarehouseId.HasValue)
            .ToList();

        return new GetSkuAvailabilityResult(
            request.CompanyId,
            request.ProductSkuId,
            skuContext.UnitId,
            skuContext.UnitName,
            skuContext.UnitNameEng,
            skuContext.UnitCategory,
            skuContext.UnitConversionFactor,
            warehouses.Sum(warehouse => warehouse.TotalQuantity),
            warehouses.Sum(warehouse => warehouse.ReservedQuantity),
            warehouses.Sum(warehouse => warehouse.AvailableQuantity),
            warehouses);
    }
}
