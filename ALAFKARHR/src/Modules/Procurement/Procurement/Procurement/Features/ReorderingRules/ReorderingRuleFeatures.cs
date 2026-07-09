namespace Procurement.Procurement.Features;

public record GetReorderingRulesQuery(Guid CompanyId, Guid? SupplierId = null, Guid? ProductId = null, Guid? ProductSkuId = null) : IQuery<GetReorderingRulesResult>;
public record GetReorderingRulesResult(IReadOnlyCollection<ReorderingRuleDto> Items);
public record UpsertReorderingRuleCommand(ReorderingRuleDto Item) : ICommand<CreateProcurementEnhancementResult>;
public record DeleteReorderingRuleCommand(Guid Id) : ICommand;

public class ReorderingRuleValidator : AbstractValidator<UpsertReorderingRuleCommand>
{
    public ReorderingRuleValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.ProductSkuId).NotEmpty();
        RuleFor(x => x.Item.MinimumQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.MaximumQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.MaximumQuantity).GreaterThanOrEqualTo(x => x.Item.MinimumQuantity);
        RuleFor(x => x.Item.ReorderQuantity).GreaterThan(0);
        RuleFor(x => x.Item.MultipleQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.LeadTimeDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.HorizonDays).GreaterThanOrEqualTo(0);
    }
}

public class GetReorderingRulesHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetReorderingRulesQuery, GetReorderingRulesResult>
{
    public async Task<GetReorderingRulesResult> Handle(GetReorderingRulesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ReorderingRules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (request.SupplierId.HasValue)
            query = query.Where(x => x.SupplierId == request.SupplierId.Value);

        if (request.ProductId.HasValue)
            query = query.Where(x => x.ProductId == request.ProductId.Value);

        if (request.ProductSkuId.HasValue)
            query = query.Where(x => x.ProductSkuId == request.ProductSkuId.Value);

        var items = await query
            .OrderBy(x => x.ProductSkuId)
            .ToListAsync(cancellationToken);
        return new GetReorderingRulesResult(items.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertReorderingRuleHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertReorderingRuleCommand, CreateProcurementEnhancementResult>
{
    public async Task<CreateProcurementEnhancementResult> Handle(UpsertReorderingRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.ReorderingRules.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);

        if (entity is null)
        {
            entity = ReorderingRule.Create(request.Item, userId);
            await dbContext.ReorderingRules.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementEnhancementResult(entity.Id);
    }
}

public class DeleteReorderingRuleHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteReorderingRuleCommand>
{
    public async Task<Unit> Handle(DeleteReorderingRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ReorderingRules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Reordering rule", request.Id);
        entity.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
