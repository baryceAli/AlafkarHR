namespace Procurement.Procurement.Features;

public record GetSupplierItemsQuery(Guid CompanyId, Guid? SupplierId = null, Guid? ProductId = null, Guid? ProductSkuId = null) : IQuery<GetSupplierItemsResult>;
public record GetSupplierItemsResult(IReadOnlyCollection<SupplierItemDto> Items);
public record UpsertSupplierItemCommand(SupplierItemDto Item) : ICommand<CreateProcurementEnhancementResult>;
public record DeleteSupplierItemCommand(Guid Id) : ICommand;

public class SupplierItemValidator : AbstractValidator<UpsertSupplierItemCommand>
{
    public SupplierItemValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.SupplierId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.ProductSkuId).NotEmpty();
        RuleFor(x => x.Item.SupplierSku).MaximumLength(100);
        RuleFor(x => x.Item.LeadTimeDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.MinimumOrderQuantity).GreaterThanOrEqualTo(0);
    }
}

public class GetSupplierItemsHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetSupplierItemsQuery, GetSupplierItemsResult>
{
    public async Task<GetSupplierItemsResult> Handle(GetSupplierItemsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SupplierItems.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (request.SupplierId.HasValue)
            query = query.Where(x => x.SupplierId == request.SupplierId.Value);

        if (request.ProductId.HasValue)
            query = query.Where(x => x.ProductId == request.ProductId.Value);

        if (request.ProductSkuId.HasValue)
            query = query.Where(x => x.ProductSkuId == request.ProductSkuId.Value);

        var items = await query
            .OrderBy(x => x.SupplierName).ThenBy(x => x.ProductNameEng ?? x.ProductName)
            .ToListAsync(cancellationToken);
        return new GetSupplierItemsResult(items.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertSupplierItemHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertSupplierItemCommand, CreateProcurementEnhancementResult>
{
    public async Task<CreateProcurementEnhancementResult> Handle(UpsertSupplierItemCommand request, CancellationToken cancellationToken)
    {
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.SupplierItems.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);

        if (entity is null)
        {
            entity = SupplierItem.Create(request.Item, userId);
            await dbContext.SupplierItems.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementEnhancementResult(entity.Id);
    }
}

public class DeleteSupplierItemHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteSupplierItemCommand>
{
    public async Task<Unit> Handle(DeleteSupplierItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SupplierItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Supplier item", request.Id);
        entity.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
