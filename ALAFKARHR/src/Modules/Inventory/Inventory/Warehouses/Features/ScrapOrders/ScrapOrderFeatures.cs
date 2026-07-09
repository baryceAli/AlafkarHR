namespace Inventory.Warehouses.Features.ScrapOrders;

using Inventory.Warehouses.Features.Inventories;

public record GetScrapOrdersQuery(ScrapOrderFilterDto Filter) : IQuery<GetScrapOrdersResult>;
public record GetScrapOrdersResult(IReadOnlyCollection<ScrapOrderDto> Items);
public record GetScrapOrderByIdQuery(Guid Id) : IQuery<GetScrapOrderByIdResult>;
public record GetScrapOrderByIdResult(ScrapOrderDto Item);
public record CreateScrapOrderCommand(CreateScrapOrderDto Item) : ICommand<CreateScrapOrderResult>;
public record CreateScrapOrderResult(Guid Id);
public record ValidateScrapOrderCommand(Guid Id, ValidateScrapOrderDto Validation) : ICommand;
public record CancelScrapOrderCommand(Guid Id) : ICommand;

public class CreateScrapOrderValidator : AbstractValidator<CreateScrapOrderCommand>
{
    public CreateScrapOrderValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.WarehouseId).NotEmpty();
        RuleFor(x => x.Item.Lines).NotEmpty();
        RuleForEach(x => x.Item.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.BatchId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.CurrencyId).NotEmpty();
        });
    }
}

public class GetScrapOrdersHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetScrapOrdersQuery, GetScrapOrdersResult>
{
    public async Task<GetScrapOrdersResult> Handle(GetScrapOrdersQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.Filter.BranchId))
            throw new ForbiddenException("You do not have permission to filter scrap orders by this branch.");

        var query = dbContext.ScrapOrders.Include(x => x.Lines).ThenInclude(x => x.Serials).AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId);

        if (!access.CanViewAllBranches)
            query = query.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
        else if (request.Filter.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);

        if (request.Filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == request.Filter.WarehouseId.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (!string.IsNullOrWhiteSpace(request.Filter.SourceDocumentType))
            query = query.Where(x => x.SourceDocumentType == request.Filter.SourceDocumentType);
        if (request.Filter.SourceDocumentId.HasValue)
            query = query.Where(x => x.SourceDocumentId == request.Filter.SourceDocumentId.Value || x.SourceInventoryOperationId == request.Filter.SourceDocumentId.Value);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(500)
            .ToListAsync(cancellationToken);

        return new GetScrapOrdersResult(items.Select(x => x.ToDto()).ToList());
    }
}

public class GetScrapOrderByIdHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetScrapOrderByIdQuery, GetScrapOrderByIdResult>
{
    public async Task<GetScrapOrderByIdResult> Handle(GetScrapOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.ScrapOrders.Include(x => x.Lines).ThenInclude(x => x.Serials).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Scrap order", request.Id);

        await InventoryBranchScope.EnsureCanReadWarehouseAsync(dbContext, sender, order.CompanyId, order.WarehouseId, cancellationToken);
        return new GetScrapOrderByIdResult(order.ToDto());
    }
}

public class CreateScrapOrderHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateScrapOrderCommand, CreateScrapOrderResult>
{
    public async Task<CreateScrapOrderResult> Handle(CreateScrapOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = ScrapOrderHelpers.GetUserId(httpContextAccessor);
        var warehouse = await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, command.Item.CompanyId, command.Item.WarehouseId, cancellationToken);
        command.Item.BranchId = warehouse.BranchId;

        var scrapLocationId = command.Item.ScrapLocationId ?? await ScrapOrderHelpers.ResolveDefaultScrapLocationAsync(dbContext, command.Item.CompanyId, command.Item.WarehouseId, cancellationToken);
        command.Item.ScrapLocationId = scrapLocationId;
        await ScrapOrderHelpers.EnsureScrapLocationAsync(dbContext, command.Item.CompanyId, command.Item.WarehouseId, scrapLocationId, cancellationToken);

        if (command.Item.SourceLocationId.HasValue)
            await ScrapOrderHelpers.EnsureActiveLocationAsync(dbContext, command.Item.CompanyId, command.Item.WarehouseId, command.Item.SourceLocationId.Value, "Source location", cancellationToken);

        if (command.Item.SourceInventoryOperationId.HasValue)
            await ScrapOrderHelpers.EnsureOperationSourceAsync(dbContext, command.Item, cancellationToken);

        var order = ScrapOrder.Create(command.Item, ScrapOrderHelpers.GenerateOrderNumber(), userId);
        await dbContext.ScrapOrders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateScrapOrderResult(order.Id);
    }
}

public class ValidateScrapOrderHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ValidateScrapOrderCommand>
{
    public async Task<Unit> Handle(ValidateScrapOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = ScrapOrderHelpers.GetUserId(httpContextAccessor);
        var order = await dbContext.ScrapOrders.Include(x => x.Lines).ThenInclude(x => x.Serials)
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Scrap order", command.Id);

        await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, order.CompanyId, order.WarehouseId, cancellationToken);
        await ScrapOrderHelpers.EnsureScrapLocationAsync(dbContext, order.CompanyId, order.WarehouseId, order.ScrapLocationId, cancellationToken);
        await ScrapOrderHelpers.EnsureOperationRemainingQuantitiesAsync(dbContext, order, cancellationToken);

        foreach (var line in order.Lines)
            await PostScrapLineAsync(order, line, userId, cancellationToken);

        order.Validate(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task PostScrapLineAsync(ScrapOrder order, ScrapOrderLine line, string userId, CancellationToken cancellationToken)
    {
        var sourceLocationId = line.SourceLocationId ?? order.SourceLocationId;
        var scrapLocationId = line.ScrapLocationId ?? order.ScrapLocationId;

        await ScrapOrderHelpers.EnsureScrapLocationAsync(dbContext, order.CompanyId, order.WarehouseId, scrapLocationId, cancellationToken);
        if (sourceLocationId.HasValue)
            await ScrapOrderHelpers.EnsureActiveLocationAsync(dbContext, order.CompanyId, order.WarehouseId, sourceLocationId.Value, "Source location", cancellationToken);

        var aggregate = new CreateInventoryAggregateDto
        {
            ProductId = line.ProductId,
            ProductSkuId = line.ProductSkuId,
            ProductPackageId = line.ProductPackageId,
            UnitId = line.UnitId,
            WarehouseId = order.WarehouseId,
            InitialBatchId = line.BatchId,
            InitialQuantity = line.Quantity,
            MovementType = MovementType.Scrap,
            UnitCost = line.UnitCost,
            TotalCost = line.TotalCost,
            CurrencyId = line.CurrencyId,
            CompanyId = order.CompanyId,
            Notes = ScrapOrderHelpers.BuildMovementNotes(order, line),
            ReferenceNumber = order.ScrapOrderNumber,
            SourceDocumentType = InventorySourceDocumentTypes.InventoryScrapOrder,
            SourceDocumentId = order.Id,
            SourceDocumentLineId = line.Id,
            SourceLocationId = sourceLocationId,
            DestinationLocationId = scrapLocationId,
            SerialNumbers = line.Serials.Select(x => x.ToDto()).ToList()
        };

        var tracking = await InventoryTrackingModeGuard.ResolveAndValidateAsync(dbContext, sender, aggregate, InventorySerialOperation.Scrap, userId, cancellationToken);
        var quantity = tracking.Quantity;

        var inventory = await dbContext.Inventories.Include(x => x.Batches).ThenInclude(x => x.Batch)
            .FirstOrDefaultAsync(x => x.WarehouseId == order.WarehouseId && x.ProductSkuId == line.ProductSkuId, cancellationToken)
            ?? throw new NotFoundException("Inventory", line.ProductSkuId);

        InventoryBatchExpiryGuard.EnsureUsableForOutbound(inventory, line.BatchId, InventorySourceDocumentTypes.InventoryScrapOrder);
        sourceLocationId = await InventoryLocationBalanceService.ResolveSourceLocationAsync(
            dbContext,
            order.CompanyId,
            order.WarehouseId,
            line.ProductSkuId,
            line.BatchId,
            sourceLocationId,
            quantity.NormalizedQuantity,
            requireReserved: false,
            cancellationToken);

        if (!sourceLocationId.HasValue)
            throw new BadRequestException("A source location is required for scrap validation.");

        decimal quantityBefore = inventory.TotalQuantity;
        decimal reservedBefore = inventory.TotalReserved;
        inventory.StockOut(new BatchStock(line.BatchId, order.WarehouseId, quantity.NormalizedQuantity, userId));

        await InventoryLocationBalanceService.DecreaseAsync(
            dbContext,
            order.CompanyId,
            line.ProductSkuId,
            order.WarehouseId,
            sourceLocationId,
            line.BatchId,
            quantity.NormalizedQuantity,
            userId,
            cancellationToken);

        await InventoryLocationBalanceService.IncreaseAsync(
            dbContext,
            order.CompanyId,
            line.ProductId,
            line.ProductSkuId,
            order.WarehouseId,
            scrapLocationId,
            line.BatchId,
            quantity.NormalizedQuantity,
            userId,
            cancellationToken);

        var movement = StockMovement.Create(
            Guid.NewGuid(),
            order.WarehouseId,
            line.BatchId,
            line.ProductId,
            line.ProductSkuId,
            quantityBefore,
            inventory.TotalQuantity,
            reservedBefore,
            inventory.TotalReserved,
            line.UnitCost,
            Math.Round(line.UnitCost * quantity.NormalizedQuantity, 2),
            line.CurrencyId,
            order.ScrapOrderNumber,
            InventorySourceDocumentTypes.InventoryScrapOrder,
            MovementType.Scrap,
            MovementDirection.OUT,
            userId,
            aggregate.Notes ?? string.Empty,
            productPackageId: quantity.ProductPackageId,
            unitId: quantity.UnitId,
            enteredQuantity: quantity.EnteredQuantity,
            packageMultiplier: quantity.PackageMultiplier,
            unitMultiplier: quantity.UnitMultiplier,
            normalizedQuantity: quantity.NormalizedQuantity,
            sourceDocumentId: order.Id,
            sourceDocumentLineId: line.Id,
            sourceLocationId: sourceLocationId,
            destinationLocationId: scrapLocationId);

        await dbContext.StockMovements.AddAsync(movement, cancellationToken);
        await InventoryTrackingModeGuard.ApplySerialMovementAsync(
            dbContext,
            movement,
            order.CompanyId,
            sourceLocationId,
            InventorySerialOperation.Scrap,
            tracking.Serials,
            userId,
            cancellationToken);
        await dbContext.InventoryValuationLayers.AddAsync(InventoryValuationLayer.FromMovement(movement, order.CompanyId, userId), cancellationToken);
        line.AttachStockMovement(movement.Id, userId);
    }
}

public class CancelScrapOrderHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CancelScrapOrderCommand>
{
    public async Task<Unit> Handle(CancelScrapOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = ScrapOrderHelpers.GetUserId(httpContextAccessor);
        var order = await dbContext.ScrapOrders.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Scrap order", command.Id);

        await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, order.CompanyId, order.WarehouseId, cancellationToken);
        order.Cancel(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class ScrapOrderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/scrap-orders/company/{companyId:guid}", async (
            Guid companyId,
            Guid? branchId,
            Guid? warehouseId,
            ScrapOrderStatus? status,
            string? sourceDocumentType,
            Guid? sourceDocumentId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetScrapOrdersQuery(new ScrapOrderFilterDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                WarehouseId = warehouseId,
                Status = status,
                SourceDocumentType = sourceDocumentType,
                SourceDocumentId = sourceDocumentId
            }));
            return Results.Ok(new { items = result.Items });
        }).RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapGet("/api/v1/inventory/scrap-orders/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetScrapOrderByIdQuery(id));
            return Results.Ok(new { item = result.Item });
        }).RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost("/api/v1/inventory/scrap-orders", async (CreateScrapOrderDto item, ISender sender) =>
        {
            var result = await sender.Send(new CreateScrapOrderCommand(item));
            return Results.Created($"/api/v1/inventory/scrap-orders/{result.Id}", result);
        }).RequireAuthorization(PermissionList.InventoryPermissions.Create);

        app.MapPost("/api/v1/inventory/scrap-orders/from-operation", async (CreateScrapOrderDto item, ISender sender) =>
        {
            var result = await sender.Send(new CreateScrapOrderCommand(item));
            return Results.Created($"/api/v1/inventory/scrap-orders/{result.Id}", result);
        }).RequireAuthorization(PermissionList.InventoryPermissions.Create);

        app.MapPost("/api/v1/inventory/scrap-orders/{id:guid}/validate", async (Guid id, ValidateScrapOrderDto validation, ISender sender) =>
        {
            await sender.Send(new ValidateScrapOrderCommand(id, validation));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);

        app.MapPost("/api/v1/inventory/scrap-orders/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            await sender.Send(new CancelScrapOrderCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);
    }
}

file static class ScrapOrderHelpers
{
    public static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static string GenerateOrderNumber() => $"SP-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

    public static async Task<Guid> ResolveDefaultScrapLocationAsync(InventoryDbContext dbContext, Guid companyId, Guid warehouseId, CancellationToken cancellationToken)
    {
        var location = await dbContext.WarehouseLocations.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.LocationUsage == WarehouseLocationUsage.VirtualScrap
                && x.IsActive
                && !x.IsDeleted)
            .OrderBy(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException("No active virtual scrap location exists for the selected warehouse.");

        return location.Id;
    }

    public static async Task EnsureScrapLocationAsync(InventoryDbContext dbContext, Guid companyId, Guid warehouseId, Guid? locationId, CancellationToken cancellationToken)
    {
        if (!locationId.HasValue || locationId.Value == Guid.Empty)
            throw new BadRequestException("Scrap location is required.");

        var exists = await dbContext.WarehouseLocations.AsNoTracking()
            .AnyAsync(x => x.Id == locationId.Value
                && x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.LocationUsage == WarehouseLocationUsage.VirtualScrap
                && x.IsActive
                && !x.IsDeleted,
                cancellationToken);

        if (!exists)
            throw new BadRequestException("Scrap location must be an active virtual scrap location in the selected warehouse.");
    }

    public static async Task EnsureActiveLocationAsync(InventoryDbContext dbContext, Guid companyId, Guid warehouseId, Guid locationId, string label, CancellationToken cancellationToken)
    {
        var exists = await dbContext.WarehouseLocations.AsNoTracking()
            .AnyAsync(x => x.Id == locationId
                && x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.IsActive
                && !x.IsDeleted,
                cancellationToken);

        if (!exists)
            throw new BadRequestException($"{label} is inactive or does not belong to the selected warehouse.");
    }

    public static async Task EnsureOperationSourceAsync(InventoryDbContext dbContext, CreateScrapOrderDto dto, CancellationToken cancellationToken)
    {
        var operation = await dbContext.InventoryOperations.Include(x => x.Lines).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.SourceInventoryOperationId.Value && x.CompanyId == dto.CompanyId, cancellationToken)
            ?? throw new NotFoundException("Inventory operation", dto.SourceInventoryOperationId.Value);

        if (operation.WarehouseId != dto.WarehouseId)
            throw new BadRequestException("Source operation does not belong to the selected warehouse.");

        dto.SourceDocumentType ??= operation.SourceDocumentType;
        dto.SourceDocumentId ??= operation.SourceDocumentId;
        dto.SourceDocumentNumber ??= operation.SourceDocumentNumber;
        dto.BranchId ??= operation.BranchId;

        foreach (var line in dto.Lines.Where(x => x.SourceInventoryOperationLineId.HasValue))
        {
            var operationLine = operation.Lines.FirstOrDefault(x => x.Id == line.SourceInventoryOperationLineId.Value)
                ?? throw new BadRequestException("Selected source operation line does not belong to the source operation.");
            if (operationLine.ProductSkuId != line.ProductSkuId || operationLine.BatchId != line.BatchId)
                throw new BadRequestException("Scrap line SKU and batch must match the selected source operation line.");
            line.SourceDocumentLineId ??= operationLine.SourceDocumentLineId;
        }
    }

    public static async Task EnsureOperationRemainingQuantitiesAsync(InventoryDbContext dbContext, ScrapOrder order, CancellationToken cancellationToken)
    {
        var lineIds = order.Lines
            .Where(x => x.SourceInventoryOperationLineId.HasValue)
            .Select(x => x.SourceInventoryOperationLineId!.Value)
            .Distinct()
            .ToList();

        if (lineIds.Count == 0)
            return;

        var operationLines = await dbContext.InventoryOperationLines.AsNoTracking()
            .Where(x => lineIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var line in order.Lines.Where(x => x.SourceInventoryOperationLineId.HasValue))
        {
            if (!operationLines.TryGetValue(line.SourceInventoryOperationLineId!.Value, out var operationLine))
                throw new BadRequestException("Selected source operation line was not found.");

            var alreadyScrapped = await dbContext.ScrapOrderLines.AsNoTracking()
                .Where(x => x.SourceInventoryOperationLineId == operationLine.Id
                    && x.ScrapOrderId != order.Id
                    && dbContext.ScrapOrders.Any(o => o.Id == x.ScrapOrderId && o.Status == ScrapOrderStatus.Validated))
                .SumAsync(x => x.Quantity, cancellationToken);

            var remaining = operationLine.PlannedQuantity - operationLine.DoneQuantity - alreadyScrapped;
            if (line.Quantity > remaining)
                throw new BadRequestException("Scrap quantity exceeds the remaining quantity on the source operation line.");
        }
    }

    public static string BuildMovementNotes(ScrapOrder order, ScrapOrderLine line)
    {
        var reason = string.IsNullOrWhiteSpace(line.Reason) ? order.Reason : line.Reason;
        var notes = string.IsNullOrWhiteSpace(line.Notes) ? order.Notes : line.Notes;
        var replenish = order.ReplenishQuantity ? " Replenishment requested." : string.Empty;
        return $"Scrap order {order.ScrapOrderNumber}.{replenish} {reason} {notes}".Trim();
    }
}
