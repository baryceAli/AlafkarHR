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
                Guid? sourceLocationId,
                Guid? destinationLocationId,
                string? sourceDocumentType,
                Guid? sourceDocumentId,
                string? referenceNumber,
                Guid? parentProductSkuId,
                string? serialNumber,
                bool? expiredOnly,
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
                        SourceLocationId = sourceLocationId,
                        DestinationLocationId = destinationLocationId,
                        SourceDocumentType = sourceDocumentType,
                        SourceDocumentId = sourceDocumentId,
                        ReferenceNumber = referenceNumber,
                        ParentProductSkuId = parentProductSkuId,
                        SerialNumber = serialNumber,
                        ExpiredOnly = expiredOnly,
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

        if (request.Filter.SourceLocationId.HasValue)
            query = query.Where(x => x.SourceLocationId == request.Filter.SourceLocationId.Value);

        if (request.Filter.DestinationLocationId.HasValue)
            query = query.Where(x => x.DestinationLocationId == request.Filter.DestinationLocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Filter.SourceDocumentType))
            query = query.Where(x => x.SourceDocumentType == request.Filter.SourceDocumentType);

        if (request.Filter.SourceDocumentId.HasValue)
            query = query.Where(x => x.SourceDocumentId == request.Filter.SourceDocumentId.Value);

        if (!string.IsNullOrWhiteSpace(request.Filter.ReferenceNumber))
            query = query.Where(x => x.ReferenceNumber.Contains(request.Filter.ReferenceNumber));

        if (request.Filter.ParentProductSkuId.HasValue)
            query = query.Where(x => x.ParentProductSkuId == request.Filter.ParentProductSkuId.Value);

        if (!string.IsNullOrWhiteSpace(request.Filter.SerialNumber))
        {
            var normalizedSerial = InventorySerialNumber.Normalize(request.Filter.SerialNumber);
            var movementIds = dbContext.StockMovementSerials.AsNoTracking()
                .Where(x => x.SerialNumber == normalizedSerial)
                .Select(x => x.StockMovementId);
            query = query.Where(x => movementIds.Contains(x.Id));
        }

        if (request.Filter.ExpiredOnly.HasValue)
        {
            var today = DateTime.UtcNow.Date;
            query = request.Filter.ExpiredOnly.Value
                ? query.Where(x => x.Batch.ExpiryDate.Date < today)
                : query.Where(x => x.Batch.ExpiryDate.Date >= today);
        }

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

        var locationIds = movements
            .SelectMany(x => new[] { x.SourceLocationId, x.DestinationLocationId })
            .Where(x => x.HasValue && x.Value != Guid.Empty)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();
        if (locationIds.Count > 0)
        {
            var locations = await dbContext.WarehouseLocations.AsNoTracking()
                .Where(x => locationIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            foreach (var movement in movements)
            {
                if (movement.SourceLocationId.HasValue && locations.TryGetValue(movement.SourceLocationId.Value, out var sourceLocation))
                {
                    movement.SourceLocationName = sourceLocation.Name;
                    movement.SourceLocationNameEng = sourceLocation.NameEng;
                }

                if (movement.DestinationLocationId.HasValue && locations.TryGetValue(movement.DestinationLocationId.Value, out var destinationLocation))
                {
                    movement.DestinationLocationName = destinationLocation.Name;
                    movement.DestinationLocationNameEng = destinationLocation.NameEng;
                }
            }
        }

        var movementIdsForSerials = movements.Select(x => x.Id).ToList();
        if (movementIdsForSerials.Count > 0)
        {
            var serialRows = await dbContext.StockMovementSerials.AsNoTracking()
                .Where(x => movementIdsForSerials.Contains(x.StockMovementId))
                .OrderBy(x => x.SerialNumber)
                .ProjectToType<StockMovementSerialDto>()
                .ToListAsync(cancellationToken);

            foreach (var movement in movements)
            {
                movement.Serials = serialRows.Where(x => x.StockMovementId == movement.Id).ToList();
                movement.SerialNumberSummary = movement.Serials.Count == 0
                    ? null
                    : string.Join(", ", movement.Serials.Select(x => x.SerialNumber));
            }
        }

        return new GetStockMovementsResult(new PaginatedResult<StockMovementDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            movements));
    }
}
