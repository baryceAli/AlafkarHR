using SalesOrder.Orders.Features;
using SalesOrder.Orders.Features.SalesOrderReservations;
using SalesOrder.Orders.Models;
using Shared.Pagination;

namespace SalesOrder.Orders.Features.SalesReturns;

public record GetSalesReturnsQuery(Guid CompanyId, PaginationRequest PaginationRequest, SalesReturnStatus? Status = null) : IQuery<GetSalesReturnsResult>;
public record GetSalesReturnsResult(PaginatedResult<SalesReturnDto> Returns);
public record GetSalesReturnByIdQuery(Guid Id) : IQuery<GetSalesReturnByIdResult>;
public record GetSalesReturnByIdResult(SalesReturnDto Return);
public record CreateSalesReturnCommand(SalesReturnDto Return) : ICommand<CreateSalesReturnResult>;
public record CreateSalesReturnResult(Guid Id);
public record UpdateSalesReturnCommand(SalesReturnDto Return) : ICommand<UpdateSalesReturnResult>;
public record UpdateSalesReturnResult(bool IsSuccess);
public record PostSalesReturnCommand(Guid Id) : ICommand<PostSalesReturnResult>;
public record PostSalesReturnResult(bool IsSuccess);
public record CancelSalesReturnCommand(Guid Id) : ICommand<CancelSalesReturnResult>;
public record CancelSalesReturnResult(bool IsSuccess);

public class GetSalesReturnsHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesReturnsQuery, GetSalesReturnsResult>
{
    public async Task<GetSalesReturnsResult> Handle(GetSalesReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesReturns.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText;
            query = query.Where(x => x.Number.Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.ReturnDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetSalesReturnsResult(SalesDocumentFeatureHelpers.Page(data.Select(x => x.ToDto()).ToList(), request.PaginationRequest, count));
    }
}

public class GetSalesReturnByIdHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesReturnByIdQuery, GetSalesReturnByIdResult>
{
    public async Task<GetSalesReturnByIdResult> Handle(GetSalesReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var salesReturn = await dbContext.SalesReturns.Include(x => x.Lines).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales return not found: {request.Id}");

        return new GetSalesReturnByIdResult(salesReturn.ToDto());
    }
}

public class CreateSalesReturnHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSalesReturnCommand, CreateSalesReturnResult>
{
    public async Task<CreateSalesReturnResult> Handle(CreateSalesReturnCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Return.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {request.Return.SalesOrderId}");

        var salesReturn = SalesReturn.Create(request.Return, order, userId);
        await dbContext.SalesReturns.AddAsync(salesReturn, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateSalesReturnResult(salesReturn.Id);
    }
}

public class UpdateSalesReturnHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSalesReturnCommand, UpdateSalesReturnResult>
{
    public async Task<UpdateSalesReturnResult> Handle(UpdateSalesReturnCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var salesReturn = await dbContext.SalesReturns.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Return.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales return not found: {request.Return.Id}");
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == salesReturn.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {salesReturn.SalesOrderId}");

        salesReturn.Update(request.Return, order, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSalesReturnResult(true);
    }
}

public class PostSalesReturnHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<PostSalesReturnCommand, PostSalesReturnResult>
{
    public async Task<PostSalesReturnResult> Handle(PostSalesReturnCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var salesReturn = await dbContext.SalesReturns.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales return not found: {request.Id}");
        var order = await dbContext.SalesOrders.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == salesReturn.SalesOrderId, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {salesReturn.SalesOrderId}");

        var returnedLines = salesReturn.Post(userId);
        foreach (var line in salesReturn.Lines)
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

            if (SalesComboFulfillmentHelper.IsCombo(skuContext.Context))
            {
                await SalesComboFulfillmentHelper.StockInReturnAsync(
                    sender,
                    salesReturn.CompanyId,
                    salesReturn.WarehouseId,
                    orderLine,
                    salesReturn.DeliveryNoteId,
                    line.DeliveryNoteLineId,
                    salesReturn.Id,
                    line.Id,
                    line.CurrencyId,
                    line.Quantity,
                    await GetDeliveredParentQuantityAsync(dbContext, salesReturn.DeliveryNoteId, line.DeliveryNoteLineId, cancellationToken),
                    salesReturn.Number,
                    cancellationToken);

                continue;
            }

            await sender.Send(new PostInventoryStockInCommand(
                line.ProductId,
                line.ProductSkuId,
                null,
                salesReturn.WarehouseId,
                line.BatchId,
                line.Quantity,
                line.UnitCost,
                line.TotalCost,
                line.CurrencyId,
                salesReturn.CompanyId,
                line.Notes,
                salesReturn.Number,
                "SalesReturn",
                line.UnitOfMeasureId,
                salesReturn.Id,
                line.Id), cancellationToken);
        }

        order.Return(returnedLines);

        if (salesReturn.CreateCreditNote)
        {
            var accountingDocument = new AccountingDocumentDto
            {
                CompanyId = salesReturn.CompanyId,
                Type = AccountingDocumentType.SalesCreditNote,
                DocumentDate = DateTime.UtcNow,
                PartyId = salesReturn.CustomerId,
                SourceModule = "SalesReturn",
                SourceDocumentId = salesReturn.Id,
                SourceDocumentNumber = salesReturn.Number,
                Lines = salesReturn.Lines.Select(x => new AccountingDocumentLineDto
                {
                    Description = string.IsNullOrWhiteSpace(x.ProductNameEng) ? x.ProductName : x.ProductNameEng,
                    ProductId = x.ProductId,
                    ProductSkuId = x.ProductSkuId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    DiscountAmount = x.DiscountAmount,
                    TaxRate = x.TaxRate,
                    NetAmount = x.NetAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount
                }).ToList()
            };

            var created = await sender.Send(new CreateAccountingDocumentCommand(accountingDocument), cancellationToken);
            await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
            await sender.Send(new GenerateZatcaInvoiceCommand(created.Id, ZatcaInvoiceType.CreditNote), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new PostSalesReturnResult(true);
    }

    private static async Task<decimal> GetDeliveredParentQuantityAsync(
        SalesOrderDbContext dbContext,
        Guid? deliveryNoteId,
        Guid? deliveryNoteLineId,
        CancellationToken cancellationToken)
    {
        if (!deliveryNoteId.HasValue || !deliveryNoteLineId.HasValue)
            return 0m;

        return await dbContext.SalesDeliveryNotes.AsNoTracking()
            .Where(x => x.Id == deliveryNoteId.Value)
            .SelectMany(x => x.Lines)
            .Where(x => x.Id == deliveryNoteLineId.Value)
            .Select(x => x.Quantity)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public class CancelSalesReturnHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CancelSalesReturnCommand, CancelSalesReturnResult>
{
    public async Task<CancelSalesReturnResult> Handle(CancelSalesReturnCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var salesReturn = await dbContext.SalesReturns
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales return not found: {request.Id}");

        salesReturn.Cancel(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CancelSalesReturnResult(true);
    }
}

public record CreateSalesReturnRequest(SalesReturnDto Return);
public record UpdateSalesReturnRequest(SalesReturnDto Return);

public class SalesReturnEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/returns/company/{companyId}", async (
            Guid companyId,
            int PageIndex,
            int PageSize,
            string? searchText,
            SalesReturnStatus? status,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalesReturnsQuery(companyId, new PaginationRequest(PageIndex, PageSize, searchText), status));
            return Results.Ok(result);
        })
        .WithName("GetSalesReturns")
        .Produces<GetSalesReturnsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.View);

        app.MapGet("/api/v1/sales/returns/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesReturnByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetSalesReturnById")
        .Produces<GetSalesReturnByIdResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.View);

        app.MapPost("/api/v1/sales/returns", async (CreateSalesReturnRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateSalesReturnCommand(request.Return));
            return Results.Created($"/api/v1/sales/returns/{result.Id}", result);
        })
        .WithName("CreateSalesReturn")
        .Produces<CreateSalesReturnResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.Create);

        app.MapPut("/api/v1/sales/returns", async (UpdateSalesReturnRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateSalesReturnCommand(request.Return));
            return Results.Ok(result);
        })
        .WithName("UpdateSalesReturn")
        .Produces<UpdateSalesReturnResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.Edit);

        app.MapPut("/api/v1/sales/returns/{id:guid}/post", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new PostSalesReturnCommand(id));
            return Results.Ok(result);
        })
        .WithName("PostSalesReturn")
        .Produces<PostSalesReturnResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.Post);

        app.MapPut("/api/v1/sales/returns/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelSalesReturnCommand(id));
            return Results.Ok(result);
        })
        .WithName("CancelSalesReturn")
        .Produces<CancelSalesReturnResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesReturnPermissions.Cancel);
    }
}
