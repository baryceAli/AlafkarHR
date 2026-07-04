using Inventory.Contracts.Stock;

namespace Inventory.Warehouses.Features.StockMovements.GetStockMovementsBySource;

public class GetStockMovementsBySourceHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetStockMovementsBySourceQuery, GetStockMovementsBySourceResult>
{
    public async Task<GetStockMovementsBySourceResult> Handle(GetStockMovementsBySourceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceDocumentType))
            throw new BadRequestException("Source document type is required.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);

        var warehouseQuery = dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (!branchAccess.CanViewAllBranches)
            warehouseQuery = warehouseQuery.Where(x => x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)));

        var warehouseIds = await warehouseQuery.Select(x => x.Id).ToListAsync(cancellationToken);

        var movements = await dbContext.StockMovements.AsNoTracking()
            .Where(x => !x.IsDeleted
                && warehouseIds.Contains(x.WarehouseId)
                && x.SourceDocumentType == request.SourceDocumentType
                && x.SourceDocumentId == request.SourceDocumentId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new StockMovementTraceRow(
                x.Id,
                x.ProductId,
                x.ProductSkuId,
                x.WarehouseId,
                x.BatchId,
                x.UnitId,
                x.ReferenceNumber,
                x.SourceDocumentType,
                x.SourceDocumentId,
                x.SourceDocumentLineId,
                x.ParentProductSkuId,
                x.ParentSalesOrderLineId,
                x.QuantityBefore,
                x.QuantityAfter,
                x.NormalizedQuantity,
                x.ReservedBefore,
                x.ReservedAfter,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new GetStockMovementsBySourceResult(movements);
    }
}
