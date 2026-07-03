using Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;
using SharedWithUI.Catalog.Enums;

namespace SalesOrder.Orders.Features.SalesOrderReservations;

public record ReserveSalesOrderCommand(Guid OrderId, SalesOrderReservationRequestDto Request)
    : ICommand<ReserveSalesOrderResult>;

public record ReserveSalesOrderResult(bool IsSuccess);

public record ReleaseSalesOrderReservationCommand(Guid OrderId, SalesOrderReservationRequestDto Request)
    : ICommand<ReleaseSalesOrderReservationResult>;

public record ReleaseSalesOrderReservationResult(bool IsSuccess);

public record SalesOrderReservationRequest(SalesOrderReservationRequestDto Reservation);
public record SalesOrderReservationResponse(bool IsSuccess);

public class ReserveSalesOrderHandler(SalesOrderDbContext dbContext, ISender sender)
    : ICommandHandler<ReserveSalesOrderCommand, ReserveSalesOrderResult>
{
    public async Task<ReserveSalesOrderResult> Handle(ReserveSalesOrderCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.WarehouseId == Guid.Empty)
            throw new BadRequestException("Warehouse is required.");

        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {command.OrderId}");

        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, order.CompanyId, order.BranchId, cancellationToken);

        var requestedLines = command.Request.Lines
            .Where(x => x.SalesOrderLineId != Guid.Empty)
            .ToDictionary(x => x.SalesOrderLineId, x => x.Quantity);

        foreach (var line in order.Lines.Where(x => !x.IsDeleted).OrderBy(x => x.LineNumber))
        {
            var quantity = requestedLines.TryGetValue(line.Id, out var requestedQuantity)
                ? requestedQuantity
                : line.UnreservedQuantity;

            if (quantity <= 0)
                continue;

            var skuContext = await GetReservableSkuContextAsync(sender, order.CompanyId, line, cancellationToken);
            if (!skuContext.ShouldReserve)
                continue;

            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(order.CompanyId, line.ProductSkuId, command.Request.WarehouseId, order.BranchId),
                cancellationToken);

            if (availability.AvailableQuantity < quantity)
                throw new BadRequestException($"Insufficient available stock for SKU {line.SkuCode}.");

            var remaining = quantity;
            foreach (var batch in availability.Warehouses
                         .SelectMany(x => x.Batches)
                         .Where(x => x.AvailableQuantity > 0)
                         .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue))
            {
                if (remaining <= 0)
                    break;

                var take = Math.Min(batch.AvailableQuantity, remaining);
                await sender.Send(new PostInventoryReservationCommand(
                    line.ProductId,
                    line.ProductSkuId,
                    command.Request.WarehouseId,
                    batch.BatchId,
                    take,
                    order.CompanyId,
                    $"Sales order reservation {order.Number}",
                    order.Number,
                    "SalesOrderReservation",
                    line.UnitOfMeasureId), cancellationToken);

                remaining -= take;
            }

            if (remaining > 0)
                throw new BadRequestException($"Insufficient batch stock for SKU {line.SkuCode}.");

            order.ReserveLine(line.Id, quantity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReserveSalesOrderResult(true);
    }

    internal static async Task<(bool ShouldReserve, GetProductSkuInventoryContextResult Context)> GetReservableSkuContextAsync(
        ISender sender,
        Guid companyId,
        SalesOrder.Orders.Models.SalesOrderLine line,
        CancellationToken cancellationToken)
    {
        var context = await sender.Send(new GetProductSkuInventoryContextQuery(companyId, line.ProductSkuId), cancellationToken);

        if (!context.ProductIsActive || !context.SkuIsActive || !context.CategoryIsActive || !context.BrandIsActive || !context.UnitIsActive)
            throw new BadRequestException($"SKU {line.SkuCode} is archived or has archived Catalog references.");

        if (context.ProductType == CatalogProductType.Service || !context.IsInventoryTracked)
            return (false, context);

        if (context.ProductType == CatalogProductType.Combo || context.ProductionType == SkuProductionType.CompositeBundle)
            throw new BadRequestException($"SKU {line.SkuCode} is a combo/composite bundle and cannot be reserved in this tranche.");

        return (true, context);
    }
}

public class ReleaseSalesOrderReservationHandler(SalesOrderDbContext dbContext, ISender sender)
    : ICommandHandler<ReleaseSalesOrderReservationCommand, ReleaseSalesOrderReservationResult>
{
    public async Task<ReleaseSalesOrderReservationResult> Handle(ReleaseSalesOrderReservationCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.WarehouseId == Guid.Empty)
            throw new BadRequestException("Warehouse is required.");

        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {command.OrderId}");

        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, order.CompanyId, order.BranchId, cancellationToken);

        var requestedLines = command.Request.Lines
            .Where(x => x.SalesOrderLineId != Guid.Empty)
            .ToDictionary(x => x.SalesOrderLineId, x => x.Quantity);

        foreach (var line in order.Lines.Where(x => !x.IsDeleted && x.ReservedQuantity > 0).OrderBy(x => x.LineNumber))
        {
            var quantity = requestedLines.TryGetValue(line.Id, out var requestedQuantity)
                ? requestedQuantity
                : line.ReservedQuantity;

            if (quantity <= 0)
                continue;

            if (quantity > line.ReservedQuantity)
                throw new BadRequestException($"Cannot release more than reserved quantity for SKU {line.SkuCode}.");

            var availability = await sender.Send(
                new GetSkuAvailabilityQuery(order.CompanyId, line.ProductSkuId, command.Request.WarehouseId, order.BranchId),
                cancellationToken);

            var remaining = quantity;
            foreach (var batch in availability.Warehouses
                         .SelectMany(x => x.Batches)
                         .Where(x => x.ReservedQuantity > 0)
                         .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue))
            {
                if (remaining <= 0)
                    break;

                var take = Math.Min(batch.ReservedQuantity, remaining);
                await sender.Send(new PostInventoryReleaseCommand(
                    line.ProductId,
                    line.ProductSkuId,
                    command.Request.WarehouseId,
                    batch.BatchId,
                    take,
                    order.CompanyId,
                    $"Sales order reservation release {order.Number}",
                    order.Number,
                    "SalesOrderReservationRelease",
                    line.UnitOfMeasureId), cancellationToken);

                remaining -= take;
            }

            if (remaining > 0)
                throw new BadRequestException($"Reserved stock could not be released for SKU {line.SkuCode}.");

            order.ReleaseLineReservation(line.Id, quantity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReleaseSalesOrderReservationResult(true);
    }
}

public class SalesOrderReservationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/sales/orders/{orderId:guid}/reserve", async (
            Guid orderId,
            SalesOrderReservationRequest request,
            ISender sender) =>
        {
            var result = await sender.Send(new ReserveSalesOrderCommand(orderId, request.Reservation));
            return Results.Ok(result.Adapt<SalesOrderReservationResponse>());
        })
        .WithName("ReserveSalesOrder")
        .Produces<SalesOrderReservationResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.Reserve);

        app.MapPut("/api/v1/sales/orders/{orderId:guid}/release-reservation", async (
            Guid orderId,
            SalesOrderReservationRequest request,
            ISender sender) =>
        {
            var result = await sender.Send(new ReleaseSalesOrderReservationCommand(orderId, request.Reservation));
            return Results.Ok(result.Adapt<SalesOrderReservationResponse>());
        })
        .WithName("ReleaseSalesOrderReservation")
        .Produces<SalesOrderReservationResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.Reserve);
    }
}
