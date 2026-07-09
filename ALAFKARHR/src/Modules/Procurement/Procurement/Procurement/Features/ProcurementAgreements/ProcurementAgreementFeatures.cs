using Procurement.Procurement.Features;

namespace Procurement.Procurement.Features.ProcurementAgreements;

public record GetProcurementAgreementsQuery(Guid CompanyId, ProcurementAgreementType? Type, Guid? BranchId) : IQuery<GetProcurementAgreementsResult>;
public record GetProcurementAgreementsResult(IReadOnlyCollection<ProcurementAgreementDto> Agreements);
public record GetProcurementAgreementByIdQuery(Guid Id) : IQuery<GetProcurementAgreementByIdResult>;
public record GetProcurementAgreementByIdResult(ProcurementAgreementDto Agreement);
public record UpsertProcurementAgreementCommand(ProcurementAgreementDto Agreement) : ICommand<CreateProcurementEnhancementResult>;
public record DeleteProcurementAgreementCommand(Guid Id) : ICommand;
public record ProcurementAgreementActionCommand(Guid Id, string Action) : ICommand;

public class ProcurementAgreementValidator : AbstractValidator<UpsertProcurementAgreementCommand>
{
    public ProcurementAgreementValidator()
    {
        RuleFor(x => x.Agreement.CompanyId).NotEmpty();
        RuleFor(x => x.Agreement.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Agreement.SupplierName).MaximumLength(250);
        RuleFor(x => x.Agreement.Reference).MaximumLength(100);
        RuleForEach(x => x.Agreement.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
            line.RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
            line.RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
        });
    }
}

public class GetProcurementAgreementsHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetProcurementAgreementsQuery, GetProcurementAgreementsResult>
{
    public async Task<GetProcurementAgreementsResult> Handle(GetProcurementAgreementsQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.BranchId))
            throw new ForbiddenException("You do not have permission to filter purchase agreements by this branch.");

        var query = dbContext.ProcurementAgreements.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        if (request.Type.HasValue)
            query = query.Where(x => x.Type == request.Type.Value);

        query = access.CanViewAllBranches
            ? request.BranchId.HasValue ? query.Where(x => x.BranchId == request.BranchId.Value) : query
            : request.BranchId.HasValue
                ? query.Where(x => x.BranchId == null || x.BranchId == request.BranchId.Value)
                : query.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));

        var agreements = await query
            .OrderBy(x => x.Type)
            .ThenByDescending(x => x.AgreementDate)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new GetProcurementAgreementsResult(agreements.Select(x => x.ToDto()).ToList());
    }
}

public class GetProcurementAgreementByIdHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetProcurementAgreementByIdQuery, GetProcurementAgreementByIdResult>
{
    public async Task<GetProcurementAgreementByIdResult> Handle(GetProcurementAgreementByIdQuery request, CancellationToken cancellationToken)
    {
        var agreement = await dbContext.ProcurementAgreements.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Purchase agreement", request.Id);

        await CreateProcurementDocumentHandler.EnsureCanReadBranchAsync(sender, agreement.CompanyId, agreement.BranchId, cancellationToken);
        return new GetProcurementAgreementByIdResult(agreement.ToDto());
    }
}

public class UpsertProcurementAgreementHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpsertProcurementAgreementCommand, CreateProcurementEnhancementResult>
{
    public async Task<CreateProcurementEnhancementResult> Handle(UpsertProcurementAgreementCommand request, CancellationToken cancellationToken)
    {
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, request.Agreement.CompanyId, request.Agreement.BranchId, cancellationToken);

        var entity = request.Agreement.Id == Guid.Empty
            ? null
            : await dbContext.ProcurementAgreements.Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == request.Agreement.Id && !x.IsDeleted, cancellationToken);

        if (entity is null)
        {
            entity = ProcurementAgreement.Create(request.Agreement, userId);
            await dbContext.ProcurementAgreements.AddAsync(entity, cancellationToken);
        }
        else
        {
            await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, entity.CompanyId, entity.BranchId, cancellationToken);
            entity.Update(request.Agreement, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementEnhancementResult(entity.Id);
    }
}

public class DeleteProcurementAgreementHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<DeleteProcurementAgreementCommand>
{
    public async Task<Unit> Handle(DeleteProcurementAgreementCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProcurementAgreements.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Purchase agreement", request.Id);
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, entity.CompanyId, entity.BranchId, cancellationToken);
        entity.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class ProcurementAgreementActionHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ProcurementAgreementActionCommand>
{
    public async Task<Unit> Handle(ProcurementAgreementActionCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProcurementAgreements.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Purchase agreement", request.Id);
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, entity.CompanyId, entity.BranchId, cancellationToken);

        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        switch (request.Action.Trim().ToLowerInvariant())
        {
            case "confirm":
                entity.Confirm(userId);
                break;
            case "close":
                entity.Close(userId);
                break;
            case "cancel":
                entity.Cancel(userId);
                break;
            default:
                throw new BadRequestException($"Unsupported purchase agreement action '{request.Action}'.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public record UpsertProcurementAgreementRequest(ProcurementAgreementDto Agreement);

public class ProcurementAgreementEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/procurement/purchase-agreements", async (
            Guid companyId,
            ProcurementAgreementType? type,
            Guid? branchId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementAgreementsQuery(companyId, type, branchId));
            return Results.Ok(new { agreements = result.Agreements });
        })
            .WithName("GetProcurementAgreements")
            .Produces<GetProcurementAgreementsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.RequestForQuotationPermissions.View);

        app.MapGet("/api/v1/procurement/purchase-agreements/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementAgreementByIdQuery(id));
            return Results.Ok(new { agreement = result.Agreement });
        })
            .WithName("GetProcurementAgreementById")
            .Produces<GetProcurementAgreementByIdResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.RequestForQuotationPermissions.View);

        app.MapPost("/api/v1/procurement/purchase-agreements", async (UpsertProcurementAgreementRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpsertProcurementAgreementCommand(request.Agreement));
            return Results.Created($"/api/v1/procurement/purchase-agreements/{result.Id}", result);
        })
            .WithName("CreateProcurementAgreement")
            .Produces<CreateProcurementEnhancementResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.RequestForQuotationPermissions.Create);

        app.MapPut("/api/v1/procurement/purchase-agreements/{id:guid}", async (Guid id, UpsertProcurementAgreementRequest request, ISender sender) =>
        {
            request.Agreement.Id = id;
            var result = await sender.Send(new UpsertProcurementAgreementCommand(request.Agreement));
            return Results.Ok(result);
        })
            .WithName("UpdateProcurementAgreement")
            .Produces<CreateProcurementEnhancementResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.RequestForQuotationPermissions.Edit);

        app.MapDelete("/api/v1/procurement/purchase-agreements/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteProcurementAgreementCommand(id));
            return Results.Ok("OK");
        })
            .WithName("DeleteProcurementAgreement")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.RequestForQuotationPermissions.Delete);

        MapAction(app, "confirm", PermissionList.RequestForQuotationPermissions.Submit);
        MapAction(app, "close", PermissionList.RequestForQuotationPermissions.Close);
        MapAction(app, "cancel", PermissionList.RequestForQuotationPermissions.Cancel);
    }

    private static void MapAction(IEndpointRouteBuilder app, string action, string permission)
    {
        app.MapPost($"/api/v1/procurement/purchase-agreements/{{id:guid}}/{action}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new ProcurementAgreementActionCommand(id, action));
            return Results.Ok("OK");
        })
            .WithName($"{action}ProcurementAgreement")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(permission);
    }
}
