using Inventory.Warehouses.Features.InventoryControls;

namespace Inventory.Warehouses.Features.InventoryOperations;

public record GetInventoryExecutionDashboardQuery(Guid CompanyId, Guid? BranchId) : IQuery<InventoryExecutionDashboardDto>;
public record GetPickingGroupsQuery(PickingGroupFilterDto Filter) : IQuery<IReadOnlyCollection<PickingGroupDto>>;
public record CreatePickingGroupCommand(CreatePickingGroupDto Item) : ICommand<CreateInventoryControlResult>;
public record ProcessPickingGroupCommand(Guid Id) : ICommand;

public class CreatePickingGroupValidator : AbstractValidator<CreatePickingGroupCommand>
{
    public CreatePickingGroupValidator()
    {
        RuleFor(x => x.Item.CompanyId).NotEmpty();
        RuleFor(x => x.Item.WarehouseId).NotEmpty();
        RuleFor(x => x.Item.InventoryOperationIds).NotEmpty();
    }
}

public class GetInventoryExecutionDashboardHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetInventoryExecutionDashboardQuery, InventoryExecutionDashboardDto>
{
    public async Task<InventoryExecutionDashboardDto> Handle(GetInventoryExecutionDashboardQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.BranchId))
            throw new ForbiddenException("You do not have permission to filter inventory execution by this branch.");

        var operationsQuery = WarehouseExecutionHelpers.ApplyBranchScope(
            dbContext.InventoryOperations.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
            access,
            request.BranchId);

        var openStatuses = WarehouseExecutionHelpers.OpenOperationStatuses;
        var operationsToProcess = await operationsQuery
            .Where(x => openStatuses.Contains(x.Status))
            .OrderBy(x => x.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var backorders = await operationsQuery
            .Where(x => x.BackorderOfOperationId.HasValue && openStatuses.Contains(x.Status))
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var operationCounts = await operationsQuery
            .Where(x => openStatuses.Contains(x.Status))
            .GroupBy(x => x.OperationKind)
            .Select(x => new { Kind = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        var backorderCount = await operationsQuery.CountAsync(x => x.BackorderOfOperationId.HasValue && openStatuses.Contains(x.Status), cancellationToken);
        var routeBottleneckCount = await operationsQuery.CountAsync(x => openStatuses.Contains(x.Status) && x.CreatedAt < DateTime.UtcNow.AddDays(-2), cancellationToken);

        var scrapQuery = WarehouseExecutionHelpers.ApplyWarehouseBranchScope(
            dbContext.ScrapOrders.AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
            dbContext,
            access,
            request.BranchId);
        var scrapTotal = await scrapQuery.CountAsync(x => x.Status == ScrapOrderStatus.Validated, cancellationToken);

        var replenishmentExceptions = await WarehouseExecutionHelpers.ApplyWarehouseBranchScope(
                dbContext.InventoryLocationBalances.AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
                dbContext,
                access,
                request.BranchId)
            .CountAsync(x => x.Quantity - x.ReservedQuantity < 0, cancellationToken);

        var groups = await WarehouseExecutionHelpers.ApplyBranchScope(
                dbContext.PickingGroups.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
                access,
                request.BranchId)
            .Where(x => x.Status == PickingGroupStatus.Ready)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var sessions = await WarehouseExecutionHelpers.ApplyBarcodeScope(
                dbContext.BarcodeOperationSessions.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId),
                dbContext,
                access,
                request.BranchId)
            .OrderByDescending(x => x.StartedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var metrics = new List<InventoryExecutionMetricDto>
        {
            Metric("operations", "عمليات قيد التنفيذ", "Operations To Process", operationsToProcess.Count, "/Inventory/Operations/Execution", "bi-list-check", "primary"),
            Metric("backorders", "أوامر مؤجلة", "Backorders", backorderCount, "/Inventory/Operations/Execution?status=Ready", "bi-arrow-return-right", "warning"),
            Metric("replenishment", "استثناءات التوريد", "Replenishment Exceptions", replenishmentExceptions, "/Procurement/Replenishment", "bi-exclamation-triangle", "danger"),
            Metric("scrap", "إتلاف معتمد", "Validated Scrap", scrapTotal, "/Inventory/Operations/Scrap", "bi-scissors", "secondary"),
            Metric("bottlenecks", "اختناقات المسارات", "Route Bottlenecks", routeBottleneckCount, "/Inventory/Operations/Execution", "bi-signpost-split", "info"),
            Metric("groups", "مجموعات الالتقاط", "Open Picking Groups", groups.Count, "/Inventory/Operations/PickingGroups", "bi-collection", "success")
        };

        foreach (var item in operationCounts)
        {
            metrics.Add(Metric($"operation-{item.Kind}", item.Kind.ToString(), item.Kind.ToString(), item.Count, $"/Inventory/Operations/Execution?operationKind={item.Kind}", "bi-box-arrow-in-right", null));
        }

        return new InventoryExecutionDashboardDto
        {
            Metrics = metrics,
            OperationsToProcess = operationsToProcess.Select(x => x.ToDto()).ToList(),
            Backorders = backorders.Select(x => x.ToDto()).ToList(),
            OpenPickingGroups = groups.Select(x => x.ToDto()).ToList(),
            RecentBarcodeSessions = sessions.Select(x => x.ToDto()).ToList()
        };
    }

    private static InventoryExecutionMetricDto Metric(string key, string label, string labelEng, decimal value, string? url, string icon, string? tone) => new()
    {
        Key = key,
        Label = label,
        LabelEng = labelEng,
        Value = value,
        Url = url,
        Icon = icon,
        Tone = tone
    };
}

public class GetPickingGroupsHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetPickingGroupsQuery, IReadOnlyCollection<PickingGroupDto>>
{
    public async Task<IReadOnlyCollection<PickingGroupDto>> Handle(GetPickingGroupsQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.Filter.BranchId))
            throw new ForbiddenException("You do not have permission to filter picking groups by this branch.");

        var query = WarehouseExecutionHelpers.ApplyBranchScope(
            dbContext.PickingGroups.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.Filter.CompanyId),
            access,
            request.Filter.BranchId);

        if (request.Filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == request.Filter.WarehouseId.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (request.Filter.GroupType.HasValue)
            query = query.Where(x => x.GroupType == request.Filter.GroupType.Value);

        var groups = await query.OrderByDescending(x => x.CreatedAt).Take(200).ToListAsync(cancellationToken);
        return groups.Select(x => x.ToDto()).ToList();
    }
}

public class CreatePickingGroupHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreatePickingGroupCommand, CreateInventoryControlResult>
{
    public async Task<CreateInventoryControlResult> Handle(CreatePickingGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = WarehouseExecutionHelpers.GetUserId(httpContextAccessor);
        var warehouse = await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext, sender, request.Item.CompanyId, request.Item.WarehouseId, cancellationToken);
        request.Item.BranchId ??= warehouse.BranchId;

        var operationIds = request.Item.InventoryOperationIds.Distinct().ToList();
        var operations = await dbContext.InventoryOperations.Include(x => x.Lines)
            .Where(x => operationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        WarehouseExecutionHelpers.EnsureCompatibleOperations(request.Item, operations);
        await WarehouseExecutionHelpers.EnsureOperationsNotAlreadyGroupedAsync(dbContext, operationIds, cancellationToken);

        var group = PickingGroup.Create(request.Item, WarehouseExecutionHelpers.GenerateGroupNumber(request.Item.GroupType), operations.OrderBy(x => x.CreatedAt), userId);
        await dbContext.PickingGroups.AddAsync(group, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryControlResult(group.Id);
    }
}

public class ProcessPickingGroupHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ProcessPickingGroupCommand>
{
    public async Task<Unit> Handle(ProcessPickingGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = WarehouseExecutionHelpers.GetUserId(httpContextAccessor);
        var group = await dbContext.PickingGroups.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Picking group", request.Id);

        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext, sender, group.CompanyId, group.WarehouseId, cancellationToken);

        var operationIds = group.Lines.Select(x => x.InventoryOperationId).ToList();
        var operations = await dbContext.InventoryOperations.Include(x => x.Lines).AsNoTracking()
            .Where(x => operationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var operation in operations)
        {
            await sender.Send(new ValidateInventoryOperationCommand(operation.Id, new ValidateInventoryOperationDto
            {
                Lines = operation.Lines.Select(line => new ValidateInventoryOperationLineDto
                {
                    LineId = line.Id,
                    DoneQuantity = line.PlannedQuantity
                }).ToList()
            }), cancellationToken);
        }

        var refreshed = await dbContext.InventoryOperations.Include(x => x.Lines).AsNoTracking()
            .Where(x => operationIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var line in group.Lines)
        {
            if (refreshed.TryGetValue(line.InventoryOperationId, out var operation))
                line.RefreshFromOperation(operation, userId);
        }

        group.MarkProcessed(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class WarehouseExecutionEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/execution-dashboard/company/{companyId:guid}", async (Guid companyId, Guid? branchId, ISender sender) =>
            Results.Ok(new { dashboard = await sender.Send(new GetInventoryExecutionDashboardQuery(companyId, branchId)) }))
            .RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapGet("/api/v1/inventory/picking-groups/company/{companyId:guid}", async (
            Guid companyId,
            Guid? branchId,
            Guid? warehouseId,
            PickingGroupStatus? status,
            PickingGroupType? groupType,
            ISender sender) =>
        {
            var groups = await sender.Send(new GetPickingGroupsQuery(new PickingGroupFilterDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                WarehouseId = warehouseId,
                Status = status,
                GroupType = groupType
            }));
            return Results.Ok(new { groups });
        }).RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost("/api/v1/inventory/picking-groups", async (CreatePickingGroupDto item, ISender sender) =>
        {
            var result = await sender.Send(new CreatePickingGroupCommand(item));
            return Results.Created($"/api/v1/inventory/picking-groups/{result.Id}", result);
        }).RequireAuthorization(PermissionList.InventoryPermissions.Create);

        app.MapPost("/api/v1/inventory/picking-groups/{id:guid}/process", async (Guid id, ISender sender) =>
        {
            await sender.Send(new ProcessPickingGroupCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);
    }
}

file static class WarehouseExecutionHelpers
{
    public static readonly InventoryOperationStatus[] OpenOperationStatuses =
    [
        InventoryOperationStatus.Ready,
        InventoryOperationStatus.InProgress,
        InventoryOperationStatus.PartiallyDone
    ];

    public static IQueryable<T> ApplyBranchScope<T>(IQueryable<T> query, GetCurrentUserBranchAccessResult access, Guid? branchId)
        where T : class
    {
        if (!access.CanViewAllBranches)
            return query.Where(x => EF.Property<Guid?>(x, "BranchId") == null || access.BranchIds.Contains(EF.Property<Guid?>(x, "BranchId")!.Value));
        return branchId.HasValue ? query.Where(x => EF.Property<Guid?>(x, "BranchId") == branchId.Value) : query;
    }

    public static IQueryable<T> ApplyWarehouseBranchScope<T>(IQueryable<T> query, InventoryDbContext dbContext, GetCurrentUserBranchAccessResult access, Guid? branchId)
        where T : class
    {
        if (access.CanViewAllBranches && !branchId.HasValue)
            return query;

        var warehouses = dbContext.Warehouses.AsNoTracking();
        if (!access.CanViewAllBranches)
            warehouses = warehouses.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
        else if (branchId.HasValue)
            warehouses = warehouses.Where(x => x.BranchId == branchId.Value);

        var ids = warehouses.Select(x => x.Id);
        return query.Where(x => ids.Contains(EF.Property<Guid>(x, "WarehouseId")));
    }

    public static IQueryable<BarcodeOperationSession> ApplyBarcodeScope(
        IQueryable<BarcodeOperationSession> query,
        InventoryDbContext dbContext,
        GetCurrentUserBranchAccessResult access,
        Guid? branchId)
    {
        if (access.CanViewAllBranches && !branchId.HasValue)
            return query;

        var warehouses = dbContext.Warehouses.AsNoTracking();
        if (!access.CanViewAllBranches)
            warehouses = warehouses.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
        else if (branchId.HasValue)
            warehouses = warehouses.Where(x => x.BranchId == branchId.Value);

        var ids = warehouses.Select(x => x.Id);
        return query.Where(x => !x.WarehouseId.HasValue || ids.Contains(x.WarehouseId.Value));
    }

    public static void EnsureCompatibleOperations(CreatePickingGroupDto dto, IReadOnlyCollection<InventoryOperation> operations)
    {
        if (operations.Count != dto.InventoryOperationIds.Distinct().Count())
            throw new BadRequestException("One or more selected inventory operations were not found.");
        if (operations.Any(x => x.CompanyId != dto.CompanyId || x.WarehouseId != dto.WarehouseId))
            throw new BadRequestException("Picking group operations must belong to the same company and warehouse.");
        if (operations.Any(x => !OpenOperationStatuses.Contains(x.Status)))
            throw new BadRequestException("Picking group operations must be open.");
        if (operations.Any(x => x.OperationKind is not InventoryOperationKind.Pick and not InventoryOperationKind.Delivery))
            throw new BadRequestException("Picking groups can only contain pick or delivery operations.");
        if (operations.Select(x => x.OperationKind).Distinct().Count() > 1)
            throw new BadRequestException("Picking group operations must have the same operation kind.");
        if (operations.Select(x => x.BranchId).Distinct().Count() > 1)
            throw new BadRequestException("Picking group operations must belong to the same branch scope.");
        if (dto.BranchId.HasValue && operations.Any(x => x.BranchId != dto.BranchId.Value))
            throw new BadRequestException("Picking group branch does not match the selected operations.");
    }

    public static async Task EnsureOperationsNotAlreadyGroupedAsync(InventoryDbContext dbContext, IReadOnlyCollection<Guid> operationIds, CancellationToken cancellationToken)
    {
        var alreadyGrouped = await dbContext.PickingGroupLines.AsNoTracking()
            .AnyAsync(line => operationIds.Contains(line.InventoryOperationId)
                && dbContext.PickingGroups.Any(group => group.Id == line.PickingGroupId && group.Status != PickingGroupStatus.Processed && group.Status != PickingGroupStatus.Cancelled),
                cancellationToken);

        if (alreadyGrouped)
            throw new BadRequestException("One or more selected operations already belong to an open picking group.");
    }

    public static string GenerateGroupNumber(PickingGroupType groupType)
    {
        var prefix = groupType == PickingGroupType.Wave ? "WV" : "BT";
        return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    public static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");
}
