using Inventory.Warehouses.Models;
using Shared.Exceptions;

namespace Inventory.Warehouses.Features.WarehouseTransfers;

public record CreateWarehouseTransferCommand(CreateWarehouseTransferDto Transfer) : ICommand<CreateWarehouseTransferResult>;
public record CreateWarehouseTransferResult(Guid Id);
public record AddWarehouseTransferItemCommand(Guid TransferId, WarehouseTransferItemInputDto Item) : ICommand<AddWarehouseTransferItemResult>;
public record AddWarehouseTransferItemResult(Guid Id);
public record RemoveWarehouseTransferItemCommand(Guid TransferId, Guid ItemId) : ICommand<RemoveWarehouseTransferItemResult>;
public record RemoveWarehouseTransferItemResult(bool IsSuccess);
public record ShipWarehouseTransferCommand(Guid TransferId) : ICommand<ShipWarehouseTransferResult>;
public record ShipWarehouseTransferResult(bool IsSuccess);
public record ReceiveWarehouseTransferCommand(Guid TransferId, ReceiveWarehouseTransferItemDto Item) : ICommand<ReceiveWarehouseTransferResult>;
public record ReceiveWarehouseTransferResult(bool IsSuccess);
public record CancelWarehouseTransferCommand(Guid TransferId) : ICommand<CancelWarehouseTransferResult>;
public record CancelWarehouseTransferResult(bool IsSuccess);
public record GetWarehouseTransferByIdQuery(Guid Id) : IQuery<GetWarehouseTransferByIdResult>;
public record GetWarehouseTransferByIdResult(WarehouseTransferDto Transfer);
public record GetWarehouseTransfersQuery(Guid CompanyId, PaginationRequest PaginationRequest, TransferStatus? Status, Guid? BranchId) : IQuery<GetWarehouseTransfersResult>;
public record GetWarehouseTransfersResult(PaginatedResult<WarehouseTransferDto> TransferList);

public class CreateWarehouseTransferValidator : AbstractValidator<CreateWarehouseTransferCommand>
{
    public CreateWarehouseTransferValidator()
    {
        RuleFor(x => x.Transfer.CompanyId).NotEmpty();
        RuleFor(x => x.Transfer.SourceWarehouseId).NotEmpty();
        RuleFor(x => x.Transfer.DestinationWarehouseId).NotEmpty();
        RuleFor(x => x.Transfer.ReferenceNumber).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Transfer.TransferNumber).MaximumLength(80);
    }
}

public class AddWarehouseTransferItemValidator : AbstractValidator<AddWarehouseTransferItemCommand>
{
    public AddWarehouseTransferItemValidator()
    {
        RuleFor(x => x.TransferId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.ProductSkuId).NotEmpty();
        RuleFor(x => x.Item.BatchId).NotEmpty();
        RuleFor(x => x.Item.Quantity).GreaterThan(0);
        RuleFor(x => x.Item.UnitCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.CurrencyId).NotEmpty();
    }
}

public class ReceiveWarehouseTransferValidator : AbstractValidator<ReceiveWarehouseTransferCommand>
{
    public ReceiveWarehouseTransferValidator()
    {
        RuleFor(x => x.TransferId).NotEmpty();
        RuleFor(x => x.Item.ItemId).NotEmpty();
        RuleFor(x => x.Item.Quantity).GreaterThan(0);
    }
}

public class WarehouseTransferEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/warehouse-transfers/company/{companyId:guid}", async (
                Guid companyId,
                TransferStatus? status,
                Guid? branchId,
                [AsParameters] PaginationRequest paginationRequest,
                ISender sender) =>
            {
                var result = await sender.Send(new GetWarehouseTransfersQuery(companyId, paginationRequest, status, branchId));
                return Results.Ok(result);
            })
            .WithName("GetWarehouseTransfers")
            .Produces<GetWarehouseTransfersResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.View);

        app.MapGet("/api/v1/inventory/warehouse-transfers/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetWarehouseTransferByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetWarehouseTransferById")
            .Produces<GetWarehouseTransferByIdResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.View);

        app.MapPost("/api/v1/inventory/warehouse-transfers", async (CreateWarehouseTransferDto transfer, ISender sender) =>
            {
                var result = await sender.Send(new CreateWarehouseTransferCommand(transfer));
                return Results.Created($"/api/v1/inventory/warehouse-transfers/{result.Id}", result);
            })
            .WithName("CreateWarehouseTransfer")
            .Produces<CreateWarehouseTransferResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Create);

        app.MapPost("/api/v1/inventory/warehouse-transfers/{transferId:guid}/items", async (Guid transferId, WarehouseTransferItemInputDto item, ISender sender) =>
            {
                var result = await sender.Send(new AddWarehouseTransferItemCommand(transferId, item));
                return Results.Created($"/api/v1/inventory/warehouse-transfers/{transferId}", result);
            })
            .WithName("AddWarehouseTransferItem")
            .Produces<AddWarehouseTransferItemResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Edit);

        app.MapDelete("/api/v1/inventory/warehouse-transfers/{transferId:guid}/items/{itemId:guid}", async (Guid transferId, Guid itemId, ISender sender) =>
            {
                var result = await sender.Send(new RemoveWarehouseTransferItemCommand(transferId, itemId));
                return Results.Ok(result);
            })
            .WithName("RemoveWarehouseTransferItem")
            .Produces<RemoveWarehouseTransferItemResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Edit);

        app.MapPost("/api/v1/inventory/warehouse-transfers/{transferId:guid}/ship", async (Guid transferId, ISender sender) =>
            {
                var result = await sender.Send(new ShipWarehouseTransferCommand(transferId));
                return Results.Ok(result);
            })
            .WithName("ShipWarehouseTransfer")
            .Produces<ShipWarehouseTransferResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Ship);

        app.MapPost("/api/v1/inventory/warehouse-transfers/{transferId:guid}/receive", async (Guid transferId, ReceiveWarehouseTransferItemDto item, ISender sender) =>
            {
                var result = await sender.Send(new ReceiveWarehouseTransferCommand(transferId, item));
                return Results.Ok(result);
            })
            .WithName("ReceiveWarehouseTransfer")
            .Produces<ReceiveWarehouseTransferResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Receive);

        app.MapPost("/api/v1/inventory/warehouse-transfers/{transferId:guid}/cancel", async (Guid transferId, ISender sender) =>
            {
                var result = await sender.Send(new CancelWarehouseTransferCommand(transferId));
                return Results.Ok(result);
            })
            .WithName("CancelWarehouseTransfer")
            .Produces<CancelWarehouseTransferResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.WarehouseTransferPermissions.Cancel);
    }
}

public class CreateWarehouseTransferHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateWarehouseTransferCommand, CreateWarehouseTransferResult>
{
    public async Task<CreateWarehouseTransferResult> Handle(CreateWarehouseTransferCommand request, CancellationToken cancellationToken)
    {
        var userId = WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor);
        var transferNumber = string.IsNullOrWhiteSpace(request.Transfer.TransferNumber)
            ? $"TR-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
            : request.Transfer.TransferNumber!;

        var sourceWarehouse = await WarehouseTransferFeatureHelpers.LoadWarehouseAsync(dbContext, request.Transfer.SourceWarehouseId, request.Transfer.CompanyId, cancellationToken);
        var destinationWarehouse = await WarehouseTransferFeatureHelpers.LoadWarehouseAsync(dbContext, request.Transfer.DestinationWarehouseId, request.Transfer.CompanyId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(sender, sourceWarehouse, destinationWarehouse, cancellationToken);

        var transfer = WarehouseTransfer.Create(
            Guid.NewGuid(),
            request.Transfer.SourceWarehouseId,
            request.Transfer.DestinationWarehouseId,
            request.Transfer.CompanyId,
            transferNumber,
            request.Transfer.Reason,
            request.Transfer.ReferenceNumber!,
            request.Transfer.ExpectedDeliveryDate == default ? DateTime.UtcNow : request.Transfer.ExpectedDeliveryDate,
            userId);

        await dbContext.WarehouseTransfers.AddAsync(transfer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateWarehouseTransferResult(transfer.Id);
    }
}

public class AddWarehouseTransferItemHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<AddWarehouseTransferItemCommand, AddWarehouseTransferItemResult>
{
    public async Task<AddWarehouseTransferItemResult> Handle(AddWarehouseTransferItemCommand request, CancellationToken cancellationToken)
    {
        var transfer = await WarehouseTransferFeatureHelpers.LoadTransferAsync(dbContext, request.TransferId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(dbContext, sender, transfer, cancellationToken);
        var userId = WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor);

        transfer.AddItem(
            request.Item.ProductId,
            request.Item.ProductSkuId,
            request.Item.BatchId,
            transfer.SourceWarehouseId,
            request.Item.Quantity,
            request.Item.UnitCost,
            request.Item.CurrencyId,
            null,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AddWarehouseTransferItemResult(transfer.Items.Last().Id);
    }
}

public class RemoveWarehouseTransferItemHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<RemoveWarehouseTransferItemCommand, RemoveWarehouseTransferItemResult>
{
    public async Task<RemoveWarehouseTransferItemResult> Handle(RemoveWarehouseTransferItemCommand request, CancellationToken cancellationToken)
    {
        var transfer = await WarehouseTransferFeatureHelpers.LoadTransferAsync(dbContext, request.TransferId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(dbContext, sender, transfer, cancellationToken);
        transfer.RemoveItem(request.ItemId, WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveWarehouseTransferItemResult(true);
    }
}

public class ShipWarehouseTransferHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ShipWarehouseTransferCommand, ShipWarehouseTransferResult>
{
    public async Task<ShipWarehouseTransferResult> Handle(ShipWarehouseTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await WarehouseTransferFeatureHelpers.LoadTransferAsync(dbContext, request.TransferId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(dbContext, sender, transfer, cancellationToken);
        var userId = WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor);

        foreach (var item in transfer.Items)
        {
            var inventory = await WarehouseTransferFeatureHelpers.LoadInventoryAsync(dbContext, transfer.SourceWarehouseId, item.ProductSkuId, cancellationToken);
            var quantityBefore = inventory.TotalQuantity;
            var reservedBefore = inventory.TotalReserved;
            inventory.StockOut(new BatchStock(item.BatchId, transfer.SourceWarehouseId, item.Quantity, userId));
            await WarehouseTransferFeatureHelpers.AddTransferMovementAsync(dbContext, transfer, item, transfer.SourceWarehouseId, quantityBefore, inventory.TotalQuantity, reservedBefore, inventory.TotalReserved, MovementType.TransferOut, MovementDirection.OUT, userId, cancellationToken);
        }

        transfer.Ship(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ShipWarehouseTransferResult(true);
    }
}

public class ReceiveWarehouseTransferHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ReceiveWarehouseTransferCommand, ReceiveWarehouseTransferResult>
{
    public async Task<ReceiveWarehouseTransferResult> Handle(ReceiveWarehouseTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await WarehouseTransferFeatureHelpers.LoadTransferAsync(dbContext, request.TransferId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(dbContext, sender, transfer, cancellationToken);
        var userId = WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor);
        var item = transfer.Items.FirstOrDefault(x => x.Id == request.Item.ItemId)
            ?? throw new NotFoundException($"Transfer item not found: {request.Item.ItemId}");

        var inventory = await dbContext.Inventories.Include(i => i.Batches).ThenInclude(b => b.Batch)
            .FirstOrDefaultAsync(i => i.WarehouseId == transfer.DestinationWarehouseId && i.ProductSkuId == item.ProductSkuId, cancellationToken);

        var quantityBefore = inventory?.TotalQuantity ?? 0;
        var reservedBefore = inventory?.TotalReserved ?? 0;
        if (inventory is null)
        {
            inventory = InventoryAggregate.Create(
                Guid.NewGuid(),
                item.ProductId,
                item.ProductSkuId,
                transfer.DestinationWarehouseId,
                item.BatchId,
                request.Item.Quantity,
                transfer.CompanyId,
                userId);
            await dbContext.Inventories.AddAsync(inventory, cancellationToken);
        }
        else
        {
            inventory.StockIn(new BatchStock(item.BatchId, transfer.DestinationWarehouseId, request.Item.Quantity, userId));
        }

        transfer.Receive(request.Item.ItemId, request.Item.Quantity, userId);
        await WarehouseTransferFeatureHelpers.AddTransferMovementAsync(dbContext, transfer, item, transfer.DestinationWarehouseId, quantityBefore, inventory.TotalQuantity, reservedBefore, inventory.TotalReserved, MovementType.TransferIn, MovementDirection.IN, userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReceiveWarehouseTransferResult(true);
    }
}

public class CancelWarehouseTransferHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CancelWarehouseTransferCommand, CancelWarehouseTransferResult>
{
    public async Task<CancelWarehouseTransferResult> Handle(CancelWarehouseTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await WarehouseTransferFeatureHelpers.LoadTransferAsync(dbContext, request.TransferId, cancellationToken);
        await WarehouseTransferFeatureHelpers.EnsureCanMutateTransferAsync(dbContext, sender, transfer, cancellationToken);
        transfer.Cancel(WarehouseTransferFeatureHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CancelWarehouseTransferResult(true);
    }
}

public class GetWarehouseTransferByIdHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetWarehouseTransferByIdQuery, GetWarehouseTransferByIdResult>
{
    public async Task<GetWarehouseTransferByIdResult> Handle(GetWarehouseTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var transfer = await dbContext.WarehouseTransfers.Include(x => x.Items).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Warehouse transfer not found: {request.Id}");

        await WarehouseTransferFeatureHelpers.EnsureCanReadTransferAsync(dbContext, sender, transfer, cancellationToken);

        return new GetWarehouseTransferByIdResult(await WarehouseTransferFeatureHelpers.MapTransferAsync(dbContext, transfer, cancellationToken));
    }
}

public class GetWarehouseTransfersHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetWarehouseTransfersQuery, GetWarehouseTransfersResult>
{
    public async Task<GetWarehouseTransfersResult> Handle(GetWarehouseTransfersQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.WarehouseTransfers.Include(x => x.Items).AsNoTracking()
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's warehouse transfers.");

        if (branchAccess.CanViewAllBranches)
        {
            if (request.BranchId.HasValue)
            {
                var filteredWarehouseIds = dbContext.Warehouses.AsNoTracking()
                    .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted && x.BranchId == request.BranchId.Value)
                    .Select(x => x.Id);
                query = query.Where(x => filteredWarehouseIds.Contains(x.SourceWarehouseId) || filteredWarehouseIds.Contains(x.DestinationWarehouseId));
            }
        }
        else
        {
            var readableWarehouseIds = dbContext.Warehouses.AsNoTracking()
                .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted &&
                    (x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value))));

            if (request.BranchId.HasValue)
                readableWarehouseIds = readableWarehouseIds.Where(x => x.BranchId == null || x.BranchId == request.BranchId.Value);

            var ids = readableWarehouseIds.Select(x => x.Id);
            query = query.Where(x => ids.Contains(x.SourceWarehouseId) && ids.Contains(x.DestinationWarehouseId));
        }

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
            query = query.Where(x => x.TransferNumber.Contains(searchText) || x.ReferenceNumber.Contains(searchText));

        var count = await query.LongCountAsync(cancellationToken);
        var transfers = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var data = new List<WarehouseTransferDto>();
        foreach (var transfer in transfers)
            data.Add(await WarehouseTransferFeatureHelpers.MapTransferAsync(dbContext, transfer, cancellationToken));

        return new GetWarehouseTransfersResult(new PaginatedResult<WarehouseTransferDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            data));
    }
}

file static class WarehouseTransferFeatureHelpers
{
    public static string GetUserId(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static async Task EnsureWarehouseAsync(InventoryDbContext dbContext, Guid warehouseId, Guid companyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Warehouses.AsNoTracking()
            .AnyAsync(x => x.Id == warehouseId && x.CompanyId == companyId && !x.IsDeleted, cancellationToken);
        if (!exists)
            throw new NotFoundException($"Warehouse not found: {warehouseId}");
    }

    public static async Task<Warehouse> LoadWarehouseAsync(InventoryDbContext dbContext, Guid warehouseId, Guid companyId, CancellationToken cancellationToken) =>
        await dbContext.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == warehouseId && x.CompanyId == companyId && !x.IsDeleted, cancellationToken)
        ?? throw new NotFoundException($"Warehouse not found: {warehouseId}");

    public static async Task<WarehouseTransfer> LoadTransferAsync(InventoryDbContext dbContext, Guid transferId, CancellationToken cancellationToken) =>
        await dbContext.WarehouseTransfers.Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == transferId && !x.IsDeleted, cancellationToken)
        ?? throw new NotFoundException($"Warehouse transfer not found: {transferId}");

    public static async Task EnsureCanReadTransferAsync(InventoryDbContext dbContext, ISender sender, WarehouseTransfer transfer, CancellationToken cancellationToken)
    {
        var (sourceWarehouse, destinationWarehouse) = await LoadTransferWarehousesAsync(dbContext, transfer, cancellationToken);
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(transfer.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, sourceWarehouse.BranchId) ||
            !BranchScopePolicy.CanRead(branchAccess, destinationWarehouse.BranchId))
        {
            throw new ForbiddenException("You do not have permission to view this warehouse transfer.");
        }
    }

    public static async Task EnsureCanMutateTransferAsync(InventoryDbContext dbContext, ISender sender, WarehouseTransfer transfer, CancellationToken cancellationToken)
    {
        var (sourceWarehouse, destinationWarehouse) = await LoadTransferWarehousesAsync(dbContext, transfer, cancellationToken);
        await EnsureCanMutateTransferAsync(sender, sourceWarehouse, destinationWarehouse, cancellationToken);
    }

    public static async Task EnsureCanMutateTransferAsync(ISender sender, Warehouse sourceWarehouse, Warehouse destinationWarehouse, CancellationToken cancellationToken)
    {
        if (sourceWarehouse.CompanyId != destinationWarehouse.CompanyId)
            throw new ForbiddenException("Warehouse transfer warehouses must belong to the same company.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(sourceWarehouse.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, sourceWarehouse.BranchId) ||
            !BranchScopePolicy.CanMutate(branchAccess, destinationWarehouse.BranchId))
        {
            throw new ForbiddenException("You do not have permission to change this warehouse transfer branch scope.");
        }
    }

    private static async Task<(Warehouse SourceWarehouse, Warehouse DestinationWarehouse)> LoadTransferWarehousesAsync(InventoryDbContext dbContext, WarehouseTransfer transfer, CancellationToken cancellationToken)
    {
        var warehouses = await dbContext.Warehouses.AsNoTracking()
            .Where(x => !x.IsDeleted && x.CompanyId == transfer.CompanyId &&
                (x.Id == transfer.SourceWarehouseId || x.Id == transfer.DestinationWarehouseId))
            .ToListAsync(cancellationToken);

        var sourceWarehouse = warehouses.FirstOrDefault(x => x.Id == transfer.SourceWarehouseId)
            ?? throw new NotFoundException($"Warehouse not found: {transfer.SourceWarehouseId}");
        var destinationWarehouse = warehouses.FirstOrDefault(x => x.Id == transfer.DestinationWarehouseId)
            ?? throw new NotFoundException($"Warehouse not found: {transfer.DestinationWarehouseId}");

        return (sourceWarehouse, destinationWarehouse);
    }

    public static async Task<InventoryAggregate> LoadInventoryAsync(InventoryDbContext dbContext, Guid warehouseId, Guid productSkuId, CancellationToken cancellationToken) =>
        await dbContext.Inventories.Include(i => i.Batches).ThenInclude(b => b.Batch)
            .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductSkuId == productSkuId && !i.IsDeleted, cancellationToken)
        ?? throw new NotFoundException($"Inventory not found for sku ({productSkuId}) in warehouse ({warehouseId})");

    public static async Task AddTransferMovementAsync(
        InventoryDbContext dbContext,
        WarehouseTransfer transfer,
        TransferItem item,
        Guid warehouseId,
        decimal quantityBefore,
        decimal quantityAfter,
        decimal reservedBefore,
        decimal reservedAfter,
        MovementType movementType,
        MovementDirection direction,
        string userId,
        CancellationToken cancellationToken)
    {
        var movement = StockMovement.Create(
            Guid.NewGuid(),
            warehouseId,
            item.BatchId,
            item.ProductId,
            item.ProductSkuId,
            quantityBefore,
            quantityAfter,
            reservedBefore,
            reservedAfter,
            item.UnitCost,
            item.UnitCost * item.Quantity,
            item.CurrencyId,
            transfer.TransferNumber,
            "WarehouseTransfer",
            movementType,
            direction,
            userId,
            transfer.Reason ?? string.Empty,
            enteredQuantity: item.Quantity,
            packageMultiplier: 1m,
            normalizedQuantity: item.Quantity);

        await dbContext.StockMovements.AddAsync(movement, cancellationToken);
        await dbContext.InventoryValuationLayers.AddAsync(
            InventoryValuationLayer.FromMovement(movement, transfer.CompanyId, userId),
            cancellationToken);
    }

    public static async Task<WarehouseTransferDto> MapTransferAsync(InventoryDbContext dbContext, WarehouseTransfer transfer, CancellationToken cancellationToken)
    {
        var warehouses = await dbContext.Warehouses.AsNoTracking()
            .Where(x => x.Id == transfer.SourceWarehouseId || x.Id == transfer.DestinationWarehouseId)
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var dto = transfer.Adapt<WarehouseTransferDto>();
        dto.Items = transfer.Items.Adapt<List<TransferItemDto>>();

        if (warehouses.TryGetValue(transfer.SourceWarehouseId, out var source))
        {
            dto.SourceWarehouseName = source.Name;
            dto.SourceWarehouseNameEng = source.NameEng;
        }

        if (warehouses.TryGetValue(transfer.DestinationWarehouseId, out var destination))
        {
            dto.DestinationWarehouseName = destination.Name;
            dto.DestinationWarehouseNameEng = destination.NameEng;
        }

        return dto;
    }
}
