namespace Procurement.Procurement.Features;

public record GetVendorPricelistsQuery(Guid CompanyId, Guid? SupplierId = null, Guid? ProductId = null, Guid? ProductSkuId = null) : IQuery<GetVendorPricelistsResult>;
public record GetVendorPricelistsResult(IReadOnlyCollection<VendorPricelistDto> Items);
public record UpsertVendorPricelistCommand(VendorPricelistDto Item) : ICommand<CreateProcurementEnhancementResult>;
public record DeleteVendorPricelistCommand(Guid Id) : ICommand;

public class VendorPricelistValidator : AbstractValidator<UpsertVendorPricelistCommand>
{
    public VendorPricelistValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.SupplierId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.ProductSkuId).NotEmpty();
        RuleFor(x => x.Item.MinimumQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.UnitCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.DiscountRate).InclusiveBetween(0, 100);
    }
}

public class GetVendorPricelistsHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetVendorPricelistsQuery, GetVendorPricelistsResult>
{
    public async Task<GetVendorPricelistsResult> Handle(GetVendorPricelistsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.VendorPricelists.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (request.SupplierId.HasValue)
            query = query.Where(x => x.SupplierId == request.SupplierId.Value);

        if (request.ProductId.HasValue)
            query = query.Where(x => x.ProductId == request.ProductId.Value);

        if (request.ProductSkuId.HasValue)
            query = query.Where(x => x.ProductSkuId == request.ProductSkuId.Value);

        var items = await query
            .OrderBy(x => x.SupplierName).ThenByDescending(x => x.ValidFrom)
            .ToListAsync(cancellationToken);
        return new GetVendorPricelistsResult(items.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertVendorPricelistHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertVendorPricelistCommand, CreateProcurementEnhancementResult>
{
    public async Task<CreateProcurementEnhancementResult> Handle(UpsertVendorPricelistCommand request, CancellationToken cancellationToken)
    {
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.VendorPricelists.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);

        if (entity is null)
        {
            entity = VendorPricelist.Create(request.Item, userId);
            await dbContext.VendorPricelists.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProcurementEnhancementResult(entity.Id);
    }
}

public class DeleteVendorPricelistHandler(ProcurementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteVendorPricelistCommand>
{
    public async Task<Unit> Handle(DeleteVendorPricelistCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.VendorPricelists.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Vendor pricelist", request.Id);
        entity.Remove(CreateProcurementDocumentHandler.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
