using SalesOrder.Orders.Features;
using SalesOrder.Orders.Models;
using FluentValidation;

namespace SalesOrder.Orders.Features.SalesQuotationTemplates;

public record GetSalesQuotationTemplatesQuery(Guid CompanyId, bool ActiveOnly = false) : IQuery<GetSalesQuotationTemplatesResult>;
public record GetSalesQuotationTemplatesResult(IReadOnlyCollection<SalesQuotationTemplateDto> Templates);
public record GetSalesQuotationTemplateByIdQuery(Guid Id) : IQuery<GetSalesQuotationTemplateByIdResult>;
public record GetSalesQuotationTemplateByIdResult(SalesQuotationTemplateDto Template);
public record UpsertSalesQuotationTemplateCommand(SalesQuotationTemplateDto Template) : ICommand<UpsertSalesQuotationTemplateResult>;
public record UpsertSalesQuotationTemplateResult(Guid Id);
public record DeleteSalesQuotationTemplateCommand(Guid Id) : ICommand;

public class SalesQuotationTemplateValidator : AbstractValidator<UpsertSalesQuotationTemplateCommand>
{
    public SalesQuotationTemplateValidator()
    {
        RuleFor(x => x.Template.CompanyId).NotEmpty();
        RuleFor(x => x.Template.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Template.ValidityDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Template.DownPaymentAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Template.DownPaymentPercent).InclusiveBetween(0, 100);
        RuleForEach(x => x.Template.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.UnitOfMeasureId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            line.RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
            line.RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
        });
    }
}

public class GetSalesQuotationTemplatesHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesQuotationTemplatesQuery, GetSalesQuotationTemplatesResult>
{
    public async Task<GetSalesQuotationTemplatesResult> Handle(GetSalesQuotationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesQuotationTemplates.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        if (request.ActiveOnly)
            query = query.Where(x => x.IsActive);

        var templates = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
        return new GetSalesQuotationTemplatesResult(templates.Select(x => x.ToDto()).ToList());
    }
}

public class GetSalesQuotationTemplateByIdHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesQuotationTemplateByIdQuery, GetSalesQuotationTemplateByIdResult>
{
    public async Task<GetSalesQuotationTemplateByIdResult> Handle(GetSalesQuotationTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await dbContext.SalesQuotationTemplates.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Quotation template not found: {request.Id}");

        return new GetSalesQuotationTemplateByIdResult(template.ToDto());
    }
}

public class UpsertSalesQuotationTemplateHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertSalesQuotationTemplateCommand, UpsertSalesQuotationTemplateResult>
{
    public async Task<UpsertSalesQuotationTemplateResult> Handle(UpsertSalesQuotationTemplateCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var entity = request.Template.Id == Guid.Empty
            ? null
            : await dbContext.SalesQuotationTemplates.Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == request.Template.Id && !x.IsDeleted, cancellationToken);

        if (entity is null)
        {
            entity = SalesQuotationTemplate.Create(request.Template, userId);
            await dbContext.SalesQuotationTemplates.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Template, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertSalesQuotationTemplateResult(entity.Id);
    }
}

public class DeleteSalesQuotationTemplateHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteSalesQuotationTemplateCommand>
{
    public async Task<Unit> Handle(DeleteSalesQuotationTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SalesQuotationTemplates.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Quotation template not found: {request.Id}");
        entity.Remove(SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public record UpsertSalesQuotationTemplateRequest(SalesQuotationTemplateDto Template);

public class SalesQuotationTemplateEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/quotation-templates", async (Guid companyId, bool? activeOnly, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesQuotationTemplatesQuery(companyId, activeOnly ?? false));
            return Results.Ok(new { templates = result.Templates });
        })
            .WithName("GetSalesQuotationTemplates")
            .Produces<GetSalesQuotationTemplatesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.SalesQuotationPermissions.View);

        app.MapGet("/api/v1/sales/quotation-templates/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesQuotationTemplateByIdQuery(id));
            return Results.Ok(new { template = result.Template });
        })
            .WithName("GetSalesQuotationTemplateById")
            .Produces<GetSalesQuotationTemplateByIdResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.SalesQuotationPermissions.View);

        app.MapPost("/api/v1/sales/quotation-templates", async (UpsertSalesQuotationTemplateRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpsertSalesQuotationTemplateCommand(request.Template));
            return Results.Created($"/api/v1/sales/quotation-templates/{result.Id}", result);
        })
            .WithName("CreateSalesQuotationTemplate")
            .Produces<UpsertSalesQuotationTemplateResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.SalesQuotationPermissions.Create);

        app.MapPut("/api/v1/sales/quotation-templates/{id:guid}", async (Guid id, UpsertSalesQuotationTemplateRequest request, ISender sender) =>
        {
            request.Template.Id = id;
            var result = await sender.Send(new UpsertSalesQuotationTemplateCommand(request.Template));
            return Results.Ok(result);
        })
            .WithName("UpdateSalesQuotationTemplate")
            .Produces<UpsertSalesQuotationTemplateResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.SalesQuotationPermissions.Edit);

        app.MapDelete("/api/v1/sales/quotation-templates/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteSalesQuotationTemplateCommand(id));
            return Results.Ok("OK");
        })
            .WithName("DeleteSalesQuotationTemplate")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.SalesQuotationPermissions.Delete);
    }
}
