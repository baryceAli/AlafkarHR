namespace Inventory.Warehouses.Features.InventoryControls;

public record GetWarehouseLocationsQuery(Guid CompanyId) : IQuery<GetWarehouseLocationsResult>;
public record GetWarehouseLocationsResult(IReadOnlyCollection<WarehouseLocationDto> Items);
public record UpsertWarehouseLocationCommand(WarehouseLocationDto Item) : ICommand<CreateInventoryControlResult>;
public record DeleteWarehouseLocationCommand(Guid Id) : ICommand;

public record GetPutawayRulesQuery(Guid CompanyId) : IQuery<GetPutawayRulesResult>;
public record GetPutawayRulesResult(IReadOnlyCollection<PutawayRuleDto> Items);
public record UpsertPutawayRuleCommand(PutawayRuleDto Item) : ICommand<CreateInventoryControlResult>;
public record DeletePutawayRuleCommand(Guid Id) : ICommand;

public record GetQualityInspectionsQuery(Guid CompanyId) : IQuery<GetQualityInspectionsResult>;
public record GetQualityInspectionsResult(IReadOnlyCollection<QualityInspectionDto> Items);
public record UpsertQualityInspectionCommand(QualityInspectionDto Item) : ICommand<CreateInventoryControlResult>;
public record DeleteQualityInspectionCommand(Guid Id) : ICommand;

public record GetLandedCostVouchersQuery(Guid CompanyId) : IQuery<GetLandedCostVouchersResult>;
public record GetLandedCostVouchersResult(IReadOnlyCollection<LandedCostVoucherDto> Items);
public record UpsertLandedCostVoucherCommand(LandedCostVoucherDto Item) : ICommand<CreateInventoryControlResult>;
public record PostLandedCostVoucherCommand(Guid Id) : ICommand;
public record DeleteLandedCostVoucherCommand(Guid Id) : ICommand;

public record GetInventoryValuationLayersQuery(Guid CompanyId) : IQuery<GetInventoryValuationLayersResult>;
public record GetInventoryValuationLayersResult(IReadOnlyCollection<InventoryValuationLayerDto> Items);
public record GetProjectedStockQuery(Guid CompanyId) : IQuery<GetProjectedStockResult>;
public record GetProjectedStockResult(IReadOnlyCollection<ProjectedStockRowDto> Rows);
public record CreateInventoryControlResult(Guid Id);

public class WarehouseLocationValidator : AbstractValidator<UpsertWarehouseLocationCommand>
{
    public WarehouseLocationValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.WarehouseId).NotEmpty();
        RuleFor(x => x.Item.Code).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Item.Name).NotEmpty().MaximumLength(200);
    }
}

public class PutawayRuleValidator : AbstractValidator<UpsertPutawayRuleCommand>
{
    public PutawayRuleValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.WarehouseId).NotEmpty();
        RuleFor(x => x.Item.Priority).GreaterThanOrEqualTo(0);
    }
}

public class QualityInspectionValidator : AbstractValidator<UpsertQualityInspectionCommand>
{
    public QualityInspectionValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.ProductSkuId).NotEmpty();
        RuleFor(x => x.Item.Quantity).GreaterThan(0);
    }
}

public class LandedCostVoucherValidator : AbstractValidator<UpsertLandedCostVoucherCommand>
{
    public LandedCostVoucherValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.SourceDocumentId).NotEmpty();
        RuleFor(x => x.Item.SourceDocumentNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Item.FreightAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.CustomsAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.HandlingAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Item.OtherAmount).GreaterThanOrEqualTo(0);
    }
}

public class GetWarehouseLocationsHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetWarehouseLocationsQuery, GetWarehouseLocationsResult>
{
    public async Task<GetWarehouseLocationsResult> Handle(GetWarehouseLocationsQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.WarehouseLocations.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        return new GetWarehouseLocationsResult(data.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertWarehouseLocationHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertWarehouseLocationCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(UpsertWarehouseLocationCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.WarehouseLocations.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);
        if (entity is null)
        {
            entity = WarehouseLocation.Create(request.Item, userId);
            await dbContext.WarehouseLocations.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(entity.Id);
    }
}

public class DeleteWarehouseLocationHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteWarehouseLocationCommand>
{
    public async Task<Unit> Handle(DeleteWarehouseLocationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.WarehouseLocations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Warehouse location", request.Id);
        entity.Remove(InventoryControlHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetPutawayRulesHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetPutawayRulesQuery, GetPutawayRulesResult>
{
    public async Task<GetPutawayRulesResult> Handle(GetPutawayRulesQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.PutawayRules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
        return new GetPutawayRulesResult(data.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertPutawayRuleHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertPutawayRuleCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(UpsertPutawayRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.PutawayRules.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);
        if (entity is null)
        {
            entity = PutawayRule.Create(request.Item, userId);
            await dbContext.PutawayRules.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(entity.Id);
    }
}

public class DeletePutawayRuleHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeletePutawayRuleCommand>
{
    public async Task<Unit> Handle(DeletePutawayRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PutawayRules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Putaway rule", request.Id);
        entity.Remove(InventoryControlHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetQualityInspectionsHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetQualityInspectionsQuery, GetQualityInspectionsResult>
{
    public async Task<GetQualityInspectionsResult> Handle(GetQualityInspectionsQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.QualityInspections.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.InspectionDate)
            .ToListAsync(cancellationToken);
        return new GetQualityInspectionsResult(data.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertQualityInspectionHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertQualityInspectionCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(UpsertQualityInspectionCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.QualityInspections.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);
        if (entity is null)
        {
            entity = QualityInspection.Create(request.Item, userId);
            await dbContext.QualityInspections.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(entity.Id);
    }
}

public class DeleteQualityInspectionHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteQualityInspectionCommand>
{
    public async Task<Unit> Handle(DeleteQualityInspectionCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.QualityInspections.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Quality inspection", request.Id);
        entity.Remove(InventoryControlHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetLandedCostVouchersHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetLandedCostVouchersQuery, GetLandedCostVouchersResult>
{
    public async Task<GetLandedCostVouchersResult> Handle(GetLandedCostVouchersQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.LandedCostVouchers.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.VoucherDate)
            .ToListAsync(cancellationToken);
        return new GetLandedCostVouchersResult(data.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertLandedCostVoucherHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertLandedCostVoucherCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(UpsertLandedCostVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.LandedCostVouchers.FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);
        if (entity is null)
        {
            entity = LandedCostVoucher.Create(request.Item, userId);
            await dbContext.LandedCostVouchers.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(entity.Id);
    }
}

public class PostLandedCostVoucherHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<PostLandedCostVoucherCommand>
{
    public async Task<Unit> Handle(PostLandedCostVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await dbContext.LandedCostVouchers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Landed cost voucher", request.Id);
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        voucher.Post(userId);

        var movements = await dbContext.StockMovements.AsNoTracking()
            .Where(x => x.ReferenceNumber == voucher.SourceDocumentNumber)
            .ToListAsync(cancellationToken);

        var totalBase = voucher.AllocationMethod == LandedCostAllocationMethod.ByQuantity
            ? movements.Sum(x => Math.Abs(x.NormalizedQuantity))
            : movements.Sum(x => Math.Abs(x.TotalCost));

        foreach (var movement in movements)
        {
            var baseValue = voucher.AllocationMethod switch
            {
                LandedCostAllocationMethod.ByQuantity => Math.Abs(movement.NormalizedQuantity),
                LandedCostAllocationMethod.Equal => movements.Count == 0 ? 0 : 1m,
                _ => Math.Abs(movement.TotalCost)
            };

            var divisor = voucher.AllocationMethod == LandedCostAllocationMethod.Equal ? movements.Count : totalBase;
            var allocated = divisor <= 0 ? 0 : voucher.TotalAmount * (baseValue / divisor);
            await dbContext.InventoryValuationLayers.AddAsync(InventoryValuationLayer.Create(new InventoryValuationLayerDto
            {
                CompanyId = voucher.CompanyId,
                ProductId = movement.ProductId,
                ProductSkuId = movement.ProductSkuId,
                WarehouseId = movement.WarehouseId,
                BatchId = movement.BatchId,
                SourceDocumentType = "LandedCostVoucher",
                ReferenceNumber = voucher.SourceDocumentNumber,
                Quantity = 0,
                UnitCost = 0,
                TotalCost = allocated,
                LayerDate = voucher.VoucherDate
            }, userId), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class DeleteLandedCostVoucherHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteLandedCostVoucherCommand>
{
    public async Task<Unit> Handle(DeleteLandedCostVoucherCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LandedCostVouchers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Landed cost voucher", request.Id);
        entity.Remove(InventoryControlHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetInventoryValuationLayersHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetInventoryValuationLayersQuery, GetInventoryValuationLayersResult>
{
    public async Task<GetInventoryValuationLayersResult> Handle(GetInventoryValuationLayersQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.InventoryValuationLayers.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.LayerDate)
            .Take(500)
            .ToListAsync(cancellationToken);
        return new GetInventoryValuationLayersResult(data.Select(x => x.ToDto()).ToList());
    }
}

public class GetProjectedStockHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetProjectedStockQuery, GetProjectedStockResult>
{
    public async Task<GetProjectedStockResult> Handle(GetProjectedStockQuery request, CancellationToken cancellationToken)
    {
        var inventoryRows = await dbContext.Inventories.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Select(x => new ProjectedStockRowDto
            {
                ProductSkuId = x.ProductSkuId,
                WarehouseId = x.WarehouseId,
                OnHandQuantity = x.TotalQuantity,
                ReservedQuantity = x.TotalReserved,
                AvailableQuantity = x.TotalAvailable
            })
            .ToListAsync(cancellationToken);

        var incomingTransfers = await dbContext.WarehouseTransfers.AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.CompanyId == request.CompanyId && (x.Status == TransferStatus.Pending || x.Status == TransferStatus.Shipped || x.Status == TransferStatus.PartiallyReceived))
            .SelectMany(x => x.Items.Select(item => new { x.DestinationWarehouseId, item.ProductSkuId, Quantity = item.Quantity - item.ReceivedQuantity }))
            .ToListAsync(cancellationToken);

        foreach (var group in incomingTransfers.GroupBy(x => new { x.DestinationWarehouseId, x.ProductSkuId }))
        {
            var row = inventoryRows.FirstOrDefault(x => x.WarehouseId == group.Key.DestinationWarehouseId && x.ProductSkuId == group.Key.ProductSkuId);
            if (row is null)
            {
                row = new ProjectedStockRowDto { WarehouseId = group.Key.DestinationWarehouseId, ProductSkuId = group.Key.ProductSkuId };
                inventoryRows.Add(row);
            }

            row.IncomingQuantity = group.Sum(x => Math.Max(x.Quantity, 0));
        }

        foreach (var row in inventoryRows)
            row.ForecastedQuantity = row.AvailableQuantity + row.IncomingQuantity - row.OutgoingQuantity;

        return new GetProjectedStockResult(inventoryRows);
    }
}

public class InventoryControlEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        MapCrud<WarehouseLocationDto, GetWarehouseLocationsQuery, GetWarehouseLocationsResult, UpsertWarehouseLocationCommand, DeleteWarehouseLocationCommand>(
            app, "warehouse-locations", query => query.Items, companyId => new GetWarehouseLocationsQuery(companyId), item => new UpsertWarehouseLocationCommand(item), id => new DeleteWarehouseLocationCommand(id));
        MapCrud<PutawayRuleDto, GetPutawayRulesQuery, GetPutawayRulesResult, UpsertPutawayRuleCommand, DeletePutawayRuleCommand>(
            app, "putaway-rules", query => query.Items, companyId => new GetPutawayRulesQuery(companyId), item => new UpsertPutawayRuleCommand(item), id => new DeletePutawayRuleCommand(id));
        MapCrud<QualityInspectionDto, GetQualityInspectionsQuery, GetQualityInspectionsResult, UpsertQualityInspectionCommand, DeleteQualityInspectionCommand>(
            app, "quality-inspections", query => query.Items, companyId => new GetQualityInspectionsQuery(companyId), item => new UpsertQualityInspectionCommand(item), id => new DeleteQualityInspectionCommand(id));
        MapCrud<LandedCostVoucherDto, GetLandedCostVouchersQuery, GetLandedCostVouchersResult, UpsertLandedCostVoucherCommand, DeleteLandedCostVoucherCommand>(
            app, "landed-cost-vouchers", query => query.Items, companyId => new GetLandedCostVouchersQuery(companyId), item => new UpsertLandedCostVoucherCommand(item), id => new DeleteLandedCostVoucherCommand(id));

        app.MapPost("/api/v1/inventory/controls/landed-cost-vouchers/{id:guid}/post", async (Guid id, ISender sender) =>
        {
            await sender.Send(new PostLandedCostVoucherCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);

        app.MapGet("/api/v1/inventory/controls/valuation-layers/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetInventoryValuationLayersQuery(companyId))).Items }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapGet("/api/v1/inventory/controls/projected-stock/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { rows = (await sender.Send(new GetProjectedStockQuery(companyId))).Rows }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }

    private static void MapCrud<TDto, TQuery, TResult, TUpsert, TDelete>(
        IEndpointRouteBuilder app,
        string route,
        Func<TResult, IReadOnlyCollection<TDto>> unwrap,
        Func<Guid, TQuery> query,
        Func<TDto, TUpsert> upsert,
        Func<Guid, TDelete> delete)
        where TQuery : IQuery<TResult>
        where TUpsert : ICommand<CreateInventoryControlResult>
        where TDelete : ICommand
    {
        app.MapGet($"/api/v1/inventory/controls/{route}/company/{{companyId:guid}}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { items = unwrap(await sender.Send(query(companyId))) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost($"/api/v1/inventory/controls/{route}", async (TDto item, ISender sender) =>
            Results.Ok(await sender.Send(upsert(item))))
            .RequireAuthorization(PermissionList.InventoryPermissions.Create);

        app.MapDelete($"/api/v1/inventory/controls/{route}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            await sender.Send(delete(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Delete);
    }
}

file static class InventoryControlHelpers
{
    public static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");
}
