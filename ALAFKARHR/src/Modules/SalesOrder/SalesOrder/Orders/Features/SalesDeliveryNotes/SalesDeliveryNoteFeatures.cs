using SalesOrder.Orders.Features;
using SalesOrder.Orders.Features.SalesOrderReservations;
using SalesOrder.Orders.Models;
using Shared.Pagination;

namespace SalesOrder.Orders.Features.SalesDeliveryNotes;

public record GetSalesDeliveryNotesQuery(Guid CompanyId, PaginationRequest PaginationRequest, SalesDeliveryNoteStatus? Status = null) : IQuery<GetSalesDeliveryNotesResult>;
public record GetSalesDeliveryNotesResult(PaginatedResult<SalesDeliveryNoteDto> DeliveryNotes);
public record GetSalesDeliveryNoteByIdQuery(Guid Id) : IQuery<GetSalesDeliveryNoteByIdResult>;
public record GetSalesDeliveryNoteByIdResult(SalesDeliveryNoteDto DeliveryNote);
public record CreateSalesDeliveryNoteCommand(SalesDeliveryNoteDto DeliveryNote) : ICommand<CreateSalesDeliveryNoteResult>;
public record CreateSalesDeliveryNoteResult(Guid Id);
public record UpdateSalesDeliveryNoteCommand(SalesDeliveryNoteDto DeliveryNote) : ICommand<UpdateSalesDeliveryNoteResult>;
public record UpdateSalesDeliveryNoteResult(bool IsSuccess);
public record PostSalesDeliveryNoteCommand(Guid Id) : ICommand<PostSalesDeliveryNoteResult>;
public record PostSalesDeliveryNoteResult(bool IsSuccess);
public record CancelSalesDeliveryNoteCommand(Guid Id) : ICommand<CancelSalesDeliveryNoteResult>;
public record CancelSalesDeliveryNoteResult(bool IsSuccess);

public class GetSalesDeliveryNotesHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesDeliveryNotesQuery, GetSalesDeliveryNotesResult>
{
    public async Task<GetSalesDeliveryNotesResult> Handle(GetSalesDeliveryNotesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesDeliveryNotes.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText;
            query = query.Where(x => x.Number.Contains(search) || (x.SalesOrderNumber != null && x.SalesOrderNumber.Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.DeliveryDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetSalesDeliveryNotesResult(SalesDocumentFeatureHelpers.Page(data.Select(x => x.ToDto()).ToList(), request.PaginationRequest, count));
    }
}

public class GetSalesDeliveryNoteByIdHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesDeliveryNoteByIdQuery, GetSalesDeliveryNoteByIdResult>
{
    public async Task<GetSalesDeliveryNoteByIdResult> Handle(GetSalesDeliveryNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = await dbContext.SalesDeliveryNotes.Include(x => x.Lines).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Delivery note not found: {request.Id}");

        return new GetSalesDeliveryNoteByIdResult(note.ToDto());
    }
}

public class CreateSalesDeliveryNoteHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSalesDeliveryNoteCommand, CreateSalesDeliveryNoteResult>
{
    public async Task<CreateSalesDeliveryNoteResult> Handle(CreateSalesDeliveryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.DeliveryNote.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {request.DeliveryNote.SalesOrderId}");

        var note = SalesDeliveryNote.Create(request.DeliveryNote, order, userId);
        await dbContext.SalesDeliveryNotes.AddAsync(note, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateSalesDeliveryNoteResult(note.Id);
    }
}

public class UpdateSalesDeliveryNoteHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSalesDeliveryNoteCommand, UpdateSalesDeliveryNoteResult>
{
    public async Task<UpdateSalesDeliveryNoteResult> Handle(UpdateSalesDeliveryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var note = await dbContext.SalesDeliveryNotes.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.DeliveryNote.Id, cancellationToken)
            ?? throw new NotFoundException($"Delivery note not found: {request.DeliveryNote.Id}");
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == note.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {note.SalesOrderId}");

        note.Update(request.DeliveryNote, order, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSalesDeliveryNoteResult(true);
    }
}

public class PostSalesDeliveryNoteHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<PostSalesDeliveryNoteCommand, PostSalesDeliveryNoteResult>
{
    public async Task<PostSalesDeliveryNoteResult> Handle(PostSalesDeliveryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var note = await dbContext.SalesDeliveryNotes.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Delivery note not found: {request.Id}");
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == note.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {note.SalesOrderId}");

        var deliveredLines = note.Post(userId);
        foreach (var line in note.Lines)
        {
            var orderLine = order.Lines.FirstOrDefault(x => x.Id == line.SalesOrderLineId)
                ?? throw new NotFoundException($"Sales order line not found: {line.SalesOrderLineId}");
            var skuContext = await ReserveSalesOrderHandler.GetReservableSkuContextAsync(
                sender,
                order.CompanyId,
                orderLine,
                cancellationToken);

            if (!skuContext.ShouldReserve)
                continue;

            if (line.Quantity > orderLine.ReservedQuantity)
                throw new BadRequestException($"Delivery quantity exceeds reserved quantity for SKU {line.SkuCode}.");

            if (SalesComboFulfillmentHelper.IsCombo(skuContext.Context))
            {
                await SalesComboFulfillmentHelper.ConsumeReservedAsync(
                    sender,
                    order.CompanyId,
                    order.BranchId,
                    note.WarehouseId,
                    orderLine,
                    line.CurrencyId,
                    line.Quantity,
                    note.Number,
                    cancellationToken);

                order.ConsumeLineReservation(orderLine.Id, line.Quantity);
                continue;
            }

            await sender.Send(new PostInventoryStockOutCommand(
                line.ProductId,
                line.ProductSkuId,
                null,
                note.WarehouseId,
                line.BatchId,
                line.Quantity,
                line.UnitCost,
                line.TotalCost,
                line.CurrencyId,
                note.CompanyId,
                line.Notes,
                note.Number,
                "SalesDeliveryNote",
                true), cancellationToken);

            order.ConsumeLineReservation(orderLine.Id, line.Quantity);
        }

        order.Deliver(deliveredLines);
        note.MarkPostedAgainstOrder(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PostSalesDeliveryNoteResult(true);
    }
}

public class CancelSalesDeliveryNoteHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CancelSalesDeliveryNoteCommand, CancelSalesDeliveryNoteResult>
{
    public async Task<CancelSalesDeliveryNoteResult> Handle(CancelSalesDeliveryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var note = await dbContext.SalesDeliveryNotes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Delivery note not found: {request.Id}");

        note.Cancel(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CancelSalesDeliveryNoteResult(true);
    }
}

public record CreateSalesDeliveryNoteRequest(SalesDeliveryNoteDto DeliveryNote);
public record UpdateSalesDeliveryNoteRequest(SalesDeliveryNoteDto DeliveryNote);

public class SalesDeliveryNoteEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/delivery-notes/company/{companyId}", async (
            Guid companyId,
            int PageIndex,
            int PageSize,
            string? searchText,
            SalesDeliveryNoteStatus? status,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalesDeliveryNotesQuery(companyId, new PaginationRequest(PageIndex, PageSize, searchText), status));
            return Results.Ok(result);
        })
        .WithName("GetSalesDeliveryNotes")
        .Produces<GetSalesDeliveryNotesResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.View);

        app.MapGet("/api/v1/sales/delivery-notes/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesDeliveryNoteByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetSalesDeliveryNoteById")
        .Produces<GetSalesDeliveryNoteByIdResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.View);

        app.MapPost("/api/v1/sales/delivery-notes", async (CreateSalesDeliveryNoteRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateSalesDeliveryNoteCommand(request.DeliveryNote));
            return Results.Created($"/api/v1/sales/delivery-notes/{result.Id}", result);
        })
        .WithName("CreateSalesDeliveryNote")
        .Produces<CreateSalesDeliveryNoteResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.Create);

        app.MapPut("/api/v1/sales/delivery-notes", async (UpdateSalesDeliveryNoteRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateSalesDeliveryNoteCommand(request.DeliveryNote));
            return Results.Ok(result);
        })
        .WithName("UpdateSalesDeliveryNote")
        .Produces<UpdateSalesDeliveryNoteResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.Edit);

        app.MapPut("/api/v1/sales/delivery-notes/{id:guid}/post", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new PostSalesDeliveryNoteCommand(id));
            return Results.Ok(result);
        })
        .WithName("PostSalesDeliveryNote")
        .Produces<PostSalesDeliveryNoteResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.Post);

        app.MapPut("/api/v1/sales/delivery-notes/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelSalesDeliveryNoteCommand(id));
            return Results.Ok(result);
        })
        .WithName("CancelSalesDeliveryNote")
        .Produces<CancelSalesDeliveryNoteResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesDeliveryNotePermissions.Cancel);
    }
}
