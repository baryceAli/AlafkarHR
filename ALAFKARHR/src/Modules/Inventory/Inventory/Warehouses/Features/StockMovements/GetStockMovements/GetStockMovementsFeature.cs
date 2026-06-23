namespace Inventory.Warehouses.Features.StockMovements.GetStockMovements;

public record GetStockMovementsQuery(StockMovementFilterDto Filter, PaginationRequest PaginationRequest) : IQuery<GetStockMovementsResult>;
public record GetStockMovementsResult(PaginatedResult<StockMovementDto> MovementList);

public class GetStockMovementsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/stock-movements/company/{companyId:guid}", async (
                Guid companyId,
                Guid? branchId,
                Guid? warehouseId,
                Guid? productSkuId,
                Guid? batchId,
                string? sourceDocumentType,
                string? referenceNumber,
                DateTime? fromDate,
                DateTime? toDate,
                [AsParameters] PaginationRequest paginationRequest,
                ISender sender) =>
            {
                var result = await sender.Send(new GetStockMovementsQuery(
                    new StockMovementFilterDto
                    {
                        CompanyId = companyId,
                        BranchId = branchId,
                        WarehouseId = warehouseId,
                        ProductSkuId = productSkuId,
                        BatchId = batchId,
                        SourceDocumentType = sourceDocumentType,
                        ReferenceNumber = referenceNumber,
                        FromDate = fromDate,
                        ToDate = toDate
                    },
                    paginationRequest));

                return Results.Ok(result);
            })
            .WithName("GetStockMovements")
            .Produces<GetStockMovementsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.StockTransactionPermissions.View);
    }
}

public class GetStockMovementsHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetStockMovementsQuery, GetStockMovementsResult>
{
    public async Task<GetStockMovementsResult> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.Filter.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's stock movements.");

        var warehouseQuery = dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId && !x.IsDeleted)
            .AsQueryable();

        if (branchAccess.CanViewAllBranches)
        {
            if (request.Filter.BranchId.HasValue)
                warehouseQuery = warehouseQuery.Where(x => x.BranchId == request.Filter.BranchId.Value);
        }
        else
        {
            warehouseQuery = request.Filter.BranchId.HasValue
                ? warehouseQuery.Where(x => x.BranchId == null || x.BranchId == request.Filter.BranchId.Value)
                : warehouseQuery.Where(x => x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)));
        }

        var warehouseIds = await warehouseQuery.Select(x => x.Id).ToListAsync(cancellationToken);

        if (request.Filter.WarehouseId.HasValue && !warehouseIds.Contains(request.Filter.WarehouseId.Value))
        {
            var warehouseExists = await dbContext.Warehouses.AsNoTracking()
                .AnyAsync(x => x.Id == request.Filter.WarehouseId.Value && x.CompanyId == request.Filter.CompanyId && !x.IsDeleted, cancellationToken);
            if (!warehouseExists)
                throw new NotFoundException($"Warehouse not found: {request.Filter.WarehouseId.Value}");

            throw new ForbiddenException("You do not have permission to view stock movements for this warehouse.");
        }

        var query = dbContext.StockMovements.AsNoTracking()
            .Where(x => warehouseIds.Contains(x.WarehouseId) && !x.IsDeleted);

        if (request.Filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == request.Filter.WarehouseId.Value);

        if (request.Filter.ProductSkuId.HasValue)
            query = query.Where(x => x.ProductSkuId == request.Filter.ProductSkuId.Value);

        if (request.Filter.BatchId.HasValue)
            query = query.Where(x => x.BatchId == request.Filter.BatchId.Value);

        if (!string.IsNullOrWhiteSpace(request.Filter.SourceDocumentType))
            query = query.Where(x => x.SourceDocumentType == request.Filter.SourceDocumentType);

        if (!string.IsNullOrWhiteSpace(request.Filter.ReferenceNumber))
            query = query.Where(x => x.ReferenceNumber.Contains(request.Filter.ReferenceNumber));

        if (request.Filter.FromDate.HasValue)
            query = query.Where(x => x.CreatedAt >= request.Filter.FromDate.Value);

        if (request.Filter.ToDate.HasValue)
            query = query.Where(x => x.CreatedAt <= request.Filter.ToDate.Value);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
            query = query.Where(x => x.ReferenceNumber.Contains(searchText) || x.SourceDocumentType.Contains(searchText) || x.Notes.Contains(searchText));

        var count = await query.LongCountAsync(cancellationToken);
        var movements = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ProjectToType<StockMovementDto>()
            .ToListAsync(cancellationToken);

        return new GetStockMovementsResult(new PaginatedResult<StockMovementDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            movements));
    }
}
