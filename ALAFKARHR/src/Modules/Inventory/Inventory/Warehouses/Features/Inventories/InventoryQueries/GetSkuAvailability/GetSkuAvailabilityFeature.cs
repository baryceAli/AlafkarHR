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

        app.MapGet("/api/v1/inventory/location-availability/company/{companyId:guid}/sku/{productSkuId:guid}", async (
                Guid companyId,
                Guid productSkuId,
                Guid? warehouseId,
                Guid? warehouseLocationId,
                Guid? batchId,
                Guid? branchId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetSkuLocationAvailabilityQuery(companyId, productSkuId, warehouseId, warehouseLocationId, batchId, branchId));
                return Results.Ok(new { availability = result });
            })
            .WithName("GetSkuLocationAvailability")
            .Produces<GetSkuLocationAvailabilityResult>(StatusCodes.Status200OK)
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

        var today = DateTime.UtcNow.Date;
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
                                batch.Batch.ExpiryDate.Date < today ? 0m : batch.Available))
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

                var locations = dbContext.InventoryLocationBalances.AsNoTracking()
                    .Where(balance => balance.CompanyId == request.CompanyId
                        && balance.ProductSkuId == request.ProductSkuId
                        && balance.WarehouseId == warehouse.Id
                        && !balance.IsDeleted
                        && !balance.Batch.IsDeleted)
                    .Join(dbContext.WarehouseLocations.AsNoTracking().Where(location => location.IsActive && !location.IsDeleted),
                        balance => balance.WarehouseLocationId,
                        location => location.Id,
                        (balance, location) => new { balance, location })
                    .Select(row => new SkuAvailabilityLocationRow(
                        row.location.Id,
                        row.location.Code,
                        row.location.Name,
                        row.location.NameEng,
                        row.balance.BatchId,
                        row.balance.Batch.BatchNumber,
                        row.balance.Batch.ExpiryDate,
                        row.balance.Quantity,
                        row.balance.ReservedQuantity,
                        row.balance.Batch.ExpiryDate.Date < today ? 0m : row.balance.AvailableQuantity))
                    .OrderBy(row => row.ExpiryDate ?? DateTime.MaxValue)
                    .ThenBy(row => row.LocationCode)
                    .ToList();

                return new SkuAvailabilityWarehouseRow(
                    warehouse.Id,
                    warehouse.Name,
                    warehouse.NameEng,
                    warehouse.BranchId,
                    warehouse.Inventories.Sum(inventory => inventory.TotalQuantity),
                    warehouse.Inventories.Sum(inventory => inventory.TotalReserved),
                    batches.Sum(batch => batch.AvailableQuantity),
                    batches,
                    locations);
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

public class GetSkuLocationAvailabilityHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetSkuLocationAvailabilityQuery, GetSkuLocationAvailabilityResult>
{
    public async Task<GetSkuLocationAvailabilityResult> Handle(GetSkuLocationAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var skuContext = await sender.Send(
            new GetProductSkuInventoryContextQuery(request.CompanyId, request.ProductSkuId),
            cancellationToken);

        if (skuContext.ProductType == CatalogProductType.Service || !skuContext.IsInventoryTracked)
            throw new BadRequestException("Location availability can only be checked for inventory-tracked goods SKUs.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's location stock.");

        if (request.WarehouseId.HasValue)
        {
            await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
                dbContext,
                sender,
                request.CompanyId,
                request.WarehouseId,
                cancellationToken);
        }

        var query = dbContext.InventoryLocationBalances.AsNoTracking()
            .Where(balance => balance.CompanyId == request.CompanyId
                && balance.ProductSkuId == request.ProductSkuId
                && !balance.IsDeleted);

        if (request.WarehouseId.HasValue)
            query = query.Where(balance => balance.WarehouseId == request.WarehouseId.Value);
        if (request.WarehouseLocationId.HasValue)
            query = query.Where(balance => balance.WarehouseLocationId == request.WarehouseLocationId.Value);
        if (request.BatchId.HasValue)
            query = query.Where(balance => balance.BatchId == request.BatchId.Value);

        var warehouseQuery = dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

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

        var warehouseIds = warehouseQuery.Select(x => x.Id);
        var today = DateTime.UtcNow.Date;
        var rows = await query
            .Where(balance => warehouseIds.Contains(balance.WarehouseId))
            .Join(dbContext.WarehouseLocations.AsNoTracking().Where(location => location.IsActive && !location.IsDeleted),
                balance => balance.WarehouseLocationId,
                location => location.Id,
                (balance, location) => new { balance, location })
            .Join(dbContext.Warehouses.AsNoTracking(),
                row => row.balance.WarehouseId,
                warehouse => warehouse.Id,
                (row, warehouse) => new { row.balance, row.location, warehouse })
            .Select(row => new SkuLocationAvailabilityRow(
                row.balance.Id,
                row.balance.CompanyId,
                row.balance.ProductId,
                row.balance.ProductSkuId,
                row.balance.WarehouseId,
                row.warehouse.Name,
                row.warehouse.NameEng,
                row.balance.WarehouseLocationId,
                row.location.Code,
                row.location.Name,
                row.location.NameEng,
                row.balance.BatchId,
                row.balance.Batch.BatchNumber,
                row.balance.Batch.ExpiryDate,
                row.balance.Quantity,
                row.balance.ReservedQuantity,
                row.balance.Batch.ExpiryDate.Date < today ? 0m : row.balance.AvailableQuantity))
            .OrderBy(row => row.WarehouseNameEng)
            .ThenBy(row => row.WarehouseLocationCode)
            .ThenBy(row => row.ExpiryDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken);

        return new GetSkuLocationAvailabilityResult(
            request.CompanyId,
            request.ProductSkuId,
            request.WarehouseId,
            request.WarehouseLocationId,
            request.BatchId,
            rows.Sum(x => x.Quantity),
            rows.Sum(x => x.ReservedQuantity),
            rows.Sum(x => x.AvailableQuantity),
            rows);
    }
}
