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
public record GetLocationBalancesQuery(Guid CompanyId) : IQuery<GetLocationBalancesResult>;
public record GetLocationBalancesResult(IReadOnlyCollection<InventoryLocationBalanceDto> Rows);
public record GetCycleCountsQuery(Guid CompanyId) : IQuery<GetCycleCountsResult>;
public record GetCycleCountsResult(IReadOnlyCollection<CycleCountDto> Items);
public record UpsertCycleCountCommand(CycleCountDto Item) : ICommand<CreateInventoryControlResult>;
public record PostCycleCountCommand(Guid Id) : ICommand;
public record DeleteCycleCountCommand(Guid Id) : ICommand;
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

        var outgoingTransfers = await dbContext.WarehouseTransfers.AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.CompanyId == request.CompanyId && (x.Status == TransferStatus.Pending || x.Status == TransferStatus.Shipped || x.Status == TransferStatus.PartiallyReceived))
            .SelectMany(x => x.Items.Select(item => new { x.SourceWarehouseId, item.ProductSkuId, Quantity = item.Quantity - item.ReceivedQuantity }))
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

        foreach (var group in outgoingTransfers.GroupBy(x => new { x.SourceWarehouseId, x.ProductSkuId }))
        {
            var row = inventoryRows.FirstOrDefault(x => x.WarehouseId == group.Key.SourceWarehouseId && x.ProductSkuId == group.Key.ProductSkuId);
            if (row is null)
            {
                row = new ProjectedStockRowDto { WarehouseId = group.Key.SourceWarehouseId, ProductSkuId = group.Key.ProductSkuId };
                inventoryRows.Add(row);
            }

            row.OutgoingQuantity = group.Sum(x => Math.Max(x.Quantity, 0));
        }

        foreach (var row in inventoryRows)
            row.ForecastedQuantity = row.AvailableQuantity + row.IncomingQuantity - row.OutgoingQuantity;

        return new GetProjectedStockResult(inventoryRows);
    }
}

public class CycleCountValidator : AbstractValidator<UpsertCycleCountCommand>
{
    public CycleCountValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.WarehouseId).NotEmpty();
        RuleFor(x => x.Item.WarehouseLocationId).NotEmpty();
        RuleFor(x => x.Item.Lines).NotEmpty();
        RuleForEach(x => x.Item.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.BatchId).NotEmpty();
            line.RuleFor(x => x.CountedQuantity).GreaterThanOrEqualTo(0);
        });
    }
}

public class GetLocationBalancesHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetLocationBalancesQuery, GetLocationBalancesResult>
{
    public async Task<GetLocationBalancesResult> Handle(GetLocationBalancesQuery request, CancellationToken cancellationToken)
    {
        var rows = await dbContext.InventoryLocationBalances.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Join(dbContext.WarehouseLocations.AsNoTracking(),
                balance => balance.WarehouseLocationId,
                location => location.Id,
                (balance, location) => new { balance, location })
            .Join(dbContext.Warehouses.AsNoTracking(),
                row => row.balance.WarehouseId,
                warehouse => warehouse.Id,
                (row, warehouse) => new { row.balance, row.location, warehouse })
            .Select(row => new InventoryLocationBalanceDto
            {
                Id = row.balance.Id,
                CompanyId = row.balance.CompanyId,
                ProductId = row.balance.ProductId,
                ProductSkuId = row.balance.ProductSkuId,
                WarehouseId = row.balance.WarehouseId,
                WarehouseName = row.warehouse.Name,
                WarehouseNameEng = row.warehouse.NameEng,
                WarehouseLocationId = row.balance.WarehouseLocationId,
                WarehouseLocationCode = row.location.Code,
                WarehouseLocationName = row.location.Name,
                WarehouseLocationNameEng = row.location.NameEng,
                BatchId = row.balance.BatchId,
                BatchNumber = row.balance.Batch.BatchNumber,
                ExpiryDate = row.balance.Batch.ExpiryDate,
                Quantity = row.balance.Quantity,
                ReservedQuantity = row.balance.ReservedQuantity,
                AvailableQuantity = row.balance.AvailableQuantity
            })
            .OrderBy(x => x.WarehouseNameEng)
            .ThenBy(x => x.WarehouseLocationCode)
            .ThenBy(x => x.BatchNumber)
            .ToListAsync(cancellationToken);

        return new GetLocationBalancesResult(rows);
    }
}

public class GetCycleCountsHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetCycleCountsQuery, GetCycleCountsResult>
{
    public async Task<GetCycleCountsResult> Handle(GetCycleCountsQuery request, CancellationToken cancellationToken)
    {
        var counts = await dbContext.CycleCounts.Include(x => x.Lines).AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.CountDate)
            .ToListAsync(cancellationToken);

        return new GetCycleCountsResult(counts.Select(x => x.ToDto()).ToList());
    }
}

public class UpsertCycleCountHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpsertCycleCountCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(UpsertCycleCountCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext,
            sender,
            request.Item.CompanyId,
            request.Item.WarehouseId,
            cancellationToken);
        await InventoryControlHelpers.EnsureActiveLocationAsync(dbContext, request.Item.CompanyId, request.Item.WarehouseId, request.Item.WarehouseLocationId, cancellationToken);

        foreach (var line in request.Item.Lines)
        {
            var context = await sender.Send(new GetProductSkuInventoryContextQuery(request.Item.CompanyId, line.ProductSkuId), cancellationToken);
            if (context.ProductId != line.ProductId)
                throw new BadRequestException("Cycle count line product does not match the selected SKU.");
            if (!context.IsInventoryTracked || context.ProductType == CatalogProductType.Service || context.ProductType == CatalogProductType.Combo || context.ProductionType == SkuProductionType.CompositeBundle)
                throw new BadRequestException("Cycle count lines must be inventory-tracked goods SKUs.");
            var batchExists = await dbContext.Batches.AsNoTracking()
                .AnyAsync(x => x.Id == line.BatchId && x.CompanyId == request.Item.CompanyId && !x.IsDeleted, cancellationToken);
            if (!batchExists)
                throw new NotFoundException($"Batch not found: {line.BatchId}");
        }

        var entity = request.Item.Id == Guid.Empty
            ? null
            : await dbContext.CycleCounts.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == request.Item.Id, cancellationToken);
        if (entity is null)
        {
            entity = CycleCount.Create(request.Item, userId);
            await dbContext.CycleCounts.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(request.Item, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(entity.Id);
    }
}

public class PostCycleCountHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<PostCycleCountCommand>
{
    public async Task<Unit> Handle(PostCycleCountCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryControlHelpers.GetUserId(httpContextAccessor);
        var count = await dbContext.CycleCounts.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cycle count", request.Id);

        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext,
            sender,
            count.CompanyId,
            count.WarehouseId,
            cancellationToken);
        await InventoryControlHelpers.EnsureActiveLocationAsync(dbContext, count.CompanyId, count.WarehouseId, count.WarehouseLocationId, cancellationToken);

        foreach (var line in count.Lines)
        {
            var inventory = await dbContext.Inventories.Include(x => x.Batches).ThenInclude(x => x.Batch)
                .FirstOrDefaultAsync(x => x.CompanyId == count.CompanyId && x.WarehouseId == count.WarehouseId && x.ProductSkuId == line.ProductSkuId, cancellationToken);
            var batchStock = inventory?.Batches.FirstOrDefault(x => x.BatchId == line.BatchId);
            var beforeQuantity = batchStock?.Quantity ?? 0m;
            var beforeReserved = batchStock?.ReservedQuantity ?? 0m;
            var delta = line.CountedQuantity - beforeQuantity;

            if (delta == 0)
            {
                await global::Inventory.Warehouses.Features.Inventories.InventoryLocationBalanceService.AdjustToAsync(
                    dbContext, count.CompanyId, line.ProductId, line.ProductSkuId, count.WarehouseId, count.WarehouseLocationId, line.BatchId, line.CountedQuantity, userId, cancellationToken);
                continue;
            }

            if (inventory is null)
            {
                if (delta < 0)
                    throw new BadRequestException("Cannot reduce stock for an SKU with no warehouse inventory.");
                inventory = InventoryAggregate.Create(Guid.NewGuid(), line.ProductId, line.ProductSkuId, count.WarehouseId, line.BatchId, line.CountedQuantity, count.CompanyId, userId);
                await dbContext.Inventories.AddAsync(inventory, cancellationToken);
            }
            else if (delta > 0)
            {
                inventory.StockIn(new BatchStock(line.BatchId, count.WarehouseId, delta, userId));
            }
            else
            {
                inventory.StockOut(new BatchStock(line.BatchId, count.WarehouseId, Math.Abs(delta), userId));
            }

            await global::Inventory.Warehouses.Features.Inventories.InventoryLocationBalanceService.AdjustToAsync(
                dbContext, count.CompanyId, line.ProductId, line.ProductSkuId, count.WarehouseId, count.WarehouseLocationId, line.BatchId, line.CountedQuantity, userId, cancellationToken);

            var movement = StockMovement.Create(
                Guid.NewGuid(),
                count.WarehouseId,
                line.BatchId,
                line.ProductId,
                line.ProductSkuId,
                beforeQuantity,
                line.CountedQuantity,
                beforeReserved,
                beforeReserved,
                0m,
                0m,
                Guid.Empty,
                count.CountNumber,
                global::Inventory.Warehouses.Features.Inventories.InventorySourceDocumentTypes.CycleCount,
                delta > 0 ? MovementType.AdjustmentIncrease : MovementType.AdjustmentDecrease,
                delta > 0 ? MovementDirection.IN : MovementDirection.OUT,
                userId,
                line.Notes ?? count.Reason ?? string.Empty,
                enteredQuantity: Math.Abs(delta),
                packageMultiplier: 1m,
                unitMultiplier: 1m,
                normalizedQuantity: Math.Abs(delta),
                sourceDocumentId: count.Id,
                sourceDocumentLineId: line.Id,
                sourceLocationId: delta < 0 ? count.WarehouseLocationId : null,
                destinationLocationId: delta > 0 ? count.WarehouseLocationId : null);
            await dbContext.StockMovements.AddAsync(movement, cancellationToken);
        }

        count.Post(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class DeleteCycleCountHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteCycleCountCommand>
{
    public async Task<Unit> Handle(DeleteCycleCountCommand request, CancellationToken cancellationToken)
    {
        var count = await dbContext.CycleCounts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cycle count", request.Id);
        count.Remove(InventoryControlHelpers.GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class GetPutawaySuggestionHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetPutawaySuggestionQuery, GetPutawaySuggestionResult>
{
    public async Task<GetPutawaySuggestionResult> Handle(GetPutawaySuggestionQuery request, CancellationToken cancellationToken)
    {
        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
            dbContext,
            sender,
            request.CompanyId,
            request.WarehouseId,
            cancellationToken);

        var suggestion = await PutawaySuggestionResolver.ResolveAsync(
            dbContext,
            request.CompanyId,
            request.WarehouseId,
            request.ProductId,
            request.ProductSkuId,
            cancellationToken);

        return new GetPutawaySuggestionResult(new PutawaySuggestionContractDto(
            suggestion.CompanyId,
            suggestion.WarehouseId,
            suggestion.ProductId,
            suggestion.ProductSkuId,
            suggestion.PutawayRuleId,
            suggestion.DestinationLocationId,
            suggestion.DestinationLocationCode,
            suggestion.DestinationLocationName,
            suggestion.DestinationLocationNameEng,
            suggestion.Priority,
            suggestion.Warning));
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
        MapCrud<CycleCountDto, GetCycleCountsQuery, GetCycleCountsResult, UpsertCycleCountCommand, DeleteCycleCountCommand>(
            app, "cycle-counts", query => query.Items, companyId => new GetCycleCountsQuery(companyId), item => new UpsertCycleCountCommand(item), id => new DeleteCycleCountCommand(id));

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

        app.MapGet("/api/v1/inventory/controls/location-balances/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { rows = (await sender.Send(new GetLocationBalancesQuery(companyId))).Rows }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost("/api/v1/inventory/controls/cycle-counts/{id:guid}/post", async (Guid id, ISender sender) =>
        {
            await sender.Send(new PostCycleCountCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);

        app.MapGet("/api/v1/inventory/controls/putaway-suggestion/company/{companyId:guid}/warehouse/{warehouseId:guid}/product/{productId:guid}/sku/{productSkuId:guid}", async (
            Guid companyId,
            Guid warehouseId,
            Guid productId,
            Guid productSkuId,
            ISender sender) =>
            Results.Ok(await sender.Send(new GetPutawaySuggestionQuery(companyId, warehouseId, productId, productSkuId))))
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

    public static async Task EnsureActiveLocationAsync(
        InventoryDbContext dbContext,
        Guid companyId,
        Guid warehouseId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.WarehouseLocations.AsNoTracking()
            .AnyAsync(x => x.Id == locationId
                && x.CompanyId == companyId
                && x.WarehouseId == warehouseId
                && x.IsActive
                && !x.IsDeleted,
                cancellationToken);

        if (!exists)
            throw new BadRequestException("Warehouse location is inactive or does not belong to the selected warehouse.");
    }
}
