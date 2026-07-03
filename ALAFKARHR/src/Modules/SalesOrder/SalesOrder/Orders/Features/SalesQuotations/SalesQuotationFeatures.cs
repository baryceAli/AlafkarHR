using SalesOrder.Orders.Features.CreateOrder;
using SalesOrder.Orders.Features;
using SalesOrder.Orders.Models;
using Shared.Pagination;

namespace SalesOrder.Orders.Features.SalesQuotations;

public record GetSalesQuotationsQuery(Guid CompanyId, PaginationRequest PaginationRequest, SalesQuotationStatus? Status = null) : IQuery<GetSalesQuotationsResult>;
public record GetSalesQuotationsResult(PaginatedResult<SalesQuotationDto> Quotations);
public record GetSalesQuotationByIdQuery(Guid Id) : IQuery<GetSalesQuotationByIdResult>;
public record GetSalesQuotationByIdResult(SalesQuotationDto Quotation);
public record CreateSalesQuotationCommand(SalesQuotationDto Quotation) : ICommand<CreateSalesQuotationResult>;
public record CreateSalesQuotationResult(Guid Id);
public record UpdateSalesQuotationCommand(SalesQuotationDto Quotation) : ICommand<UpdateSalesQuotationResult>;
public record UpdateSalesQuotationResult(bool IsSuccess);
public record SalesQuotationActionCommand(Guid Id, string Action, string? Reason) : ICommand<SalesQuotationActionResult>;
public record SalesQuotationActionResult(bool IsSuccess, Guid? SalesOrderId = null);

public class GetSalesQuotationsHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesQuotationsQuery, GetSalesQuotationsResult>
{
    public async Task<GetSalesQuotationsResult> Handle(GetSalesQuotationsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesQuotations.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText;
            query = query.Where(x => x.Number.Contains(search) || (x.CustomerName != null && x.CustomerName.Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.QuotationDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetSalesQuotationsResult(SalesDocumentFeatureHelpers.Page(
            data.Select(x => x.ToDto()).ToList(),
            request.PaginationRequest,
            count));
    }
}

public class GetSalesQuotationByIdHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesQuotationByIdQuery, GetSalesQuotationByIdResult>
{
    public async Task<GetSalesQuotationByIdResult> Handle(GetSalesQuotationByIdQuery request, CancellationToken cancellationToken)
    {
        var quotation = await dbContext.SalesQuotations.Include(x => x.Lines).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Quotation not found: {request.Id}");

        return new GetSalesQuotationByIdResult(quotation.ToDto());
    }
}

public class CreateSalesQuotationHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateSalesQuotationCommand, CreateSalesQuotationResult>
{
    public async Task<CreateSalesQuotationResult> Handle(CreateSalesQuotationCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        await SalesDocumentFeatureHelpers.ResolveQuotationPricingAsync(request.Quotation, sender, cancellationToken);
        var quotation = SalesQuotation.Create(request.Quotation, userId);
        await dbContext.SalesQuotations.AddAsync(quotation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateSalesQuotationResult(quotation.Id);
    }
}

public class UpdateSalesQuotationHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateSalesQuotationCommand, UpdateSalesQuotationResult>
{
    public async Task<UpdateSalesQuotationResult> Handle(UpdateSalesQuotationCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var quotation = await dbContext.SalesQuotations.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Quotation.Id, cancellationToken)
            ?? throw new NotFoundException($"Quotation not found: {request.Quotation.Id}");

        await SalesDocumentFeatureHelpers.ResolveQuotationPricingAsync(request.Quotation, sender, cancellationToken);
        quotation.Update(request.Quotation, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSalesQuotationResult(true);
    }
}

public class SalesQuotationActionHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<SalesQuotationActionCommand, SalesQuotationActionResult>
{
    public async Task<SalesQuotationActionResult> Handle(SalesQuotationActionCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var quotation = await dbContext.SalesQuotations.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Quotation not found: {request.Id}");

        switch (request.Action.Trim().ToLowerInvariant())
        {
            case "send":
                quotation.Send(userId);
                break;
            case "accept":
                quotation.Accept(userId);
                break;
            case "reject":
                quotation.Reject(request.Reason ?? "Rejected", userId);
                break;
            case "cancel":
                quotation.Cancel(userId);
                break;
            case "expire":
                quotation.ExpireIfNeeded(userId);
                break;
            case "convert":
                var orderDto = quotation.ToDto();
                var order = new SalesOrderDto
                {
                    Number = $"SO-{quotation.Number}",
                    CustomerId = quotation.CustomerId,
                    PriceListId = quotation.PriceListId,
                    CouponCode = quotation.CouponCode,
                    SalespersonId = quotation.SalespersonId,
                    SourceQuotationId = quotation.Id,
                    SourceType = SalesOrderSourceType.QuotationConversion,
                    SourceDocumentId = quotation.Id,
                    SourceDocumentNumber = quotation.Number,
                    CompanyId = quotation.CompanyId,
                    Lines = orderDto.Lines.Select(x => new SalesOrderLineDto
                    {
                        ProductId = x.ProductId,
                        ProductSkuId = x.ProductSkuId,
                        ProductName = x.ProductName,
                        ProductNameEng = x.ProductNameEng,
                        SkuCode = x.SkuCode,
                        UnitOfMeasureId = x.UnitOfMeasureId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        DiscountRate = x.DiscountRate,
                        TaxRate = x.TaxRate,
                        Notes = x.Notes,
                        Pricing = x.Pricing
                    }).ToList()
                };
                var created = await sender.Send(new CreateOrderCommand(order), cancellationToken);
                quotation.MarkConverted(created.Id, userId);
                await dbContext.SaveChangesAsync(cancellationToken);
                return new SalesQuotationActionResult(true, created.Id);
            default:
                throw new Exception($"Unsupported quotation action: {request.Action}");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new SalesQuotationActionResult(true);
    }
}

public record CreateSalesQuotationRequest(SalesQuotationDto Quotation);
public record UpdateSalesQuotationRequest(SalesQuotationDto Quotation);
public record SalesQuotationActionRequest(string Action, string? Reason);

public class SalesQuotationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/quotations/company/{companyId}", async (
            Guid companyId,
            int PageIndex,
            int PageSize,
            string? searchText,
            SalesQuotationStatus? status,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalesQuotationsQuery(companyId, new PaginationRequest(PageIndex, PageSize, searchText), status));
            return Results.Ok(result);
        })
        .WithName("GetSalesQuotations")
        .Produces<GetSalesQuotationsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.View);

        app.MapGet("/api/v1/sales/quotations/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesQuotationByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetSalesQuotationById")
        .Produces<GetSalesQuotationByIdResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.View);

        app.MapPost("/api/v1/sales/quotations", async (CreateSalesQuotationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateSalesQuotationCommand(request.Quotation));
            return Results.Created($"/api/v1/sales/quotations/{result.Id}", result);
        })
        .WithName("CreateSalesQuotation")
        .Produces<CreateSalesQuotationResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Create);

        app.MapPut("/api/v1/sales/quotations", async (UpdateSalesQuotationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateSalesQuotationCommand(request.Quotation));
            return Results.Ok(result);
        })
        .WithName("UpdateSalesQuotation")
        .Produces<UpdateSalesQuotationResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Edit);

        app.MapPut("/api/v1/sales/quotations/{id:guid}/action", async (Guid id, SalesQuotationActionRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SalesQuotationActionCommand(id, request.Action, request.Reason));
            return Results.Ok(result);
        })
        .WithName("SalesQuotationAction")
        .Produces<SalesQuotationActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Edit);

        app.MapPut("/api/v1/sales/quotations/{id:guid}/send", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new SalesQuotationActionCommand(id, "send", null));
            return Results.Ok(result);
        })
        .WithName("SendSalesQuotation")
        .Produces<SalesQuotationActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Send);

        app.MapPut("/api/v1/sales/quotations/{id:guid}/convert", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new SalesQuotationActionCommand(id, "convert", null));
            return Results.Ok(result);
        })
        .WithName("ConvertSalesQuotation")
        .Produces<SalesQuotationActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Convert);

        app.MapPut("/api/v1/sales/quotations/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new SalesQuotationActionCommand(id, "cancel", null));
            return Results.Ok(result);
        })
        .WithName("CancelSalesQuotation")
        .Produces<SalesQuotationActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Cancel);

        app.MapPut("/api/v1/sales/quotations/{id:guid}/reject", async (Guid id, SalesQuotationActionRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SalesQuotationActionCommand(id, "reject", request.Reason));
            return Results.Ok(result);
        })
        .WithName("RejectSalesQuotation")
        .Produces<SalesQuotationActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesQuotationPermissions.Reject);
    }
}
