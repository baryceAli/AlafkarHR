namespace Inventory.Warehouses.Features.StockMovements.GetStockMovements;

public record GetStockMovementsQuery(StockMovementFilterDto Filter, PaginationRequest PaginationRequest) : IQuery<GetStockMovementsResult>;
public record GetStockMovementsResult(PaginatedResult<StockMovementDto> MovementList);

public class GetStockMovementsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/stock-movements/company/{companyId:guid}", async (
                Guid companyId,
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

public class GetStockMovementsHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetStockMovementsQuery, GetStockMovementsResult>
{
    public async Task<GetStockMovementsResult> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var warehouseIds = await dbContext.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

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
