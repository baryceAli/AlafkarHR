using Inventory.Warehouses.Features.Inventories;

namespace Inventory.Warehouses.Features.InventoryOperations;

public record GetInventoryOperationsQuery(InventoryOperationFilterDto Filter) : IQuery<GetInventoryOperationsResult>;
public record GetInventoryOperationsResult(IReadOnlyCollection<InventoryOperationDto> Operations);
public record ValidateInventoryOperationCommand(Guid Id, ValidateInventoryOperationDto Validation) : ICommand;

public class GetWarehouseOperationFlowHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetWarehouseOperationFlowQuery, GetWarehouseOperationFlowResult>
{
    public async Task<GetWarehouseOperationFlowResult> Handle(GetWarehouseOperationFlowQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.WarehouseId && x.CompanyId == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException("Warehouse", request.WarehouseId);

        return new GetWarehouseOperationFlowResult((int)warehouse.InboundFlow, (int)warehouse.OutboundFlow);
    }
}

public class GetInventoryOperationsHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetInventoryOperationsQuery, GetInventoryOperationsResult>
{
    public async Task<GetInventoryOperationsResult> Handle(GetInventoryOperationsQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.Filter.BranchId))
            throw new ForbiddenException("You do not have permission to filter inventory operations by this branch.");

        var query = dbContext.InventoryOperations.Include(x => x.Lines).AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId);

        if (!access.CanViewAllBranches)
            query = query.Where(x => x.BranchId == null || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
        else if (request.Filter.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);

        if (request.Filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == request.Filter.WarehouseId.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (request.Filter.OperationKind.HasValue)
            query = query.Where(x => x.OperationKind == request.Filter.OperationKind.Value);
        if (request.Filter.FlowDirection.HasValue)
            query = query.Where(x => x.FlowDirection == request.Filter.FlowDirection.Value);
        if (!string.IsNullOrWhiteSpace(request.Filter.SourceDocumentType))
            query = query.Where(x => x.SourceDocumentType == request.Filter.SourceDocumentType);
        if (request.Filter.SourceDocumentId.HasValue)
            query = query.Where(x => x.SourceDocumentId == request.Filter.SourceDocumentId.Value);

        var operations = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Sequence)
            .Take(500)
            .ToListAsync(cancellationToken);

        return new GetInventoryOperationsResult(operations.Select(x => x.ToDto()).ToList());
    }
}

public class EnsureInventoryReceiptOperationChainHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<EnsureInventoryReceiptOperationChainCommand, EnsureInventoryOperationChainResult>
{
    public async Task<EnsureInventoryOperationChainResult> Handle(EnsureInventoryReceiptOperationChainCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryOperationHelpers.GetUserId(httpContextAccessor);
        var warehouse = await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, request.CompanyId, request.WarehouseId, cancellationToken);
        var operations = await InventoryOperationChainBuilder.EnsureReceiptChainAsync(dbContext, warehouse, request, userId, cancellationToken);

        if (request.MarkCompleted)
        {
            foreach (var operation in operations)
                operation.MarkCompletedWithoutPosting(userId);
        }
        else if (request.MarkFirstStepCompleted && operations.Count > 0)
        {
            operations[0].MarkCompletedWithoutPosting(userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureInventoryOperationChainResult(operations.Select(x => x.Id).ToList());
    }
}

public class EnsureInventoryDeliveryOperationChainHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<EnsureInventoryDeliveryOperationChainCommand, EnsureInventoryOperationChainResult>
{
    public async Task<EnsureInventoryOperationChainResult> Handle(EnsureInventoryDeliveryOperationChainCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryOperationHelpers.GetUserId(httpContextAccessor);
        var warehouse = await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, request.CompanyId, request.WarehouseId, cancellationToken);
        var operations = await InventoryOperationChainBuilder.EnsureDeliveryChainAsync(dbContext, warehouse, request, userId, cancellationToken);

        if (request.MarkCompleted)
        {
            foreach (var operation in operations)
                operation.MarkCompletedWithoutPosting(userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureInventoryOperationChainResult(operations.Select(x => x.Id).ToList());
    }
}

public class ValidateInventoryOperationHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ValidateInventoryOperationCommand>
{
    public async Task<Unit> Handle(ValidateInventoryOperationCommand request, CancellationToken cancellationToken)
    {
        var userId = InventoryOperationHelpers.GetUserId(httpContextAccessor);
        var operation = await dbContext.InventoryOperations.Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Inventory operation", request.Id);

        await InventoryBranchScope.EnsureCanMutateWarehouseAsync(dbContext, sender, operation.CompanyId, operation.WarehouseId, cancellationToken);
        var doneQuantities = request.Validation.Lines.ToDictionary(x => x.LineId, x => x.DoneQuantity);
        operation.Complete(doneQuantities, userId);

        if (operation.IsStockPostingStep && !operation.StockPosted && operation.DoneQuantity > 0)
        {
            foreach (var line in operation.Lines.Where(x => x.DoneQuantity > 0))
            {
                var result = operation.FlowDirection == InventoryOperationFlowDirection.Receipt
                    ? await sender.Send(new PostInventoryStockInCommand(
                        line.ProductId,
                        line.ProductSkuId,
                        line.ProductPackageId,
                        operation.WarehouseId,
                        line.BatchId,
                        line.DoneQuantity,
                        line.UnitCost,
                        InventoryOperationHelpers.ResolveDoneCost(line),
                        line.CurrencyId,
                        operation.CompanyId,
                        line.Notes ?? $"Inventory operation {operation.SourceDocumentNumber}",
                        operation.SourceDocumentNumber,
                        operation.SourceDocumentType,
                        line.UnitId,
                        operation.SourceDocumentId,
                        line.SourceDocumentLineId,
                        DestinationLocationId: line.DestinationLocationId),
                        cancellationToken)
                    : await sender.Send(new PostInventoryStockOutCommand(
                        line.ProductId,
                        line.ProductSkuId,
                        line.ProductPackageId,
                        operation.WarehouseId,
                        line.BatchId,
                        line.DoneQuantity,
                        line.UnitCost,
                        InventoryOperationHelpers.ResolveDoneCost(line),
                        line.CurrencyId,
                        operation.CompanyId,
                        line.Notes ?? $"Inventory operation {operation.SourceDocumentNumber}",
                        operation.SourceDocumentNumber,
                        operation.SourceDocumentType,
                        line.ConsumeReservedQuantity,
                        line.UnitId,
                        operation.SourceDocumentId,
                        line.SourceDocumentLineId,
                        SourceLocationId: line.SourceLocationId),
                        cancellationToken);

                line.AttachStockMovement(result.InventoryId, userId);
            }

            operation.MarkStockPosted(userId);
        }

        if (operation.Status == InventoryOperationStatus.PartiallyDone)
            await InventoryOperationChainBuilder.CreateBackorderAsync(dbContext, operation, userId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class InventoryOperationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/operations/company/{companyId:guid}", async (
            Guid companyId,
            Guid? branchId,
            Guid? warehouseId,
            InventoryOperationStatus? status,
            InventoryOperationKind? operationKind,
            InventoryOperationFlowDirection? flowDirection,
            string? sourceDocumentType,
            Guid? sourceDocumentId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetInventoryOperationsQuery(new InventoryOperationFilterDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                WarehouseId = warehouseId,
                Status = status,
                OperationKind = operationKind,
                FlowDirection = flowDirection,
                SourceDocumentType = sourceDocumentType,
                SourceDocumentId = sourceDocumentId
            }));

            return Results.Ok(new { operations = result.Operations });
        }).RequireAuthorization(PermissionList.InventoryPermissions.View);

        app.MapPost("/api/v1/inventory/operations/{id:guid}/validate", async (
            Guid id,
            ValidateInventoryOperationDto validation,
            ISender sender) =>
        {
            await sender.Send(new ValidateInventoryOperationCommand(id, validation));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.InventoryPermissions.Edit);
    }
}

file static class InventoryOperationChainBuilder
{
    public static async Task<List<InventoryOperation>> EnsureReceiptChainAsync(
        InventoryDbContext dbContext,
        Warehouse warehouse,
        EnsureInventoryReceiptOperationChainCommand request,
        string userId,
        CancellationToken cancellationToken)
    {
        var existing = await LoadExistingAsync(dbContext, request.CompanyId, request.SourceDocumentType, request.SourceDocumentId, cancellationToken);
        if (existing.Count > 0)
            return existing;

        var steps = warehouse.InboundFlow switch
        {
            WarehouseOperationFlow.OneStep => new[]
            {
                Step(InventoryOperationKind.Receipt, null, Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for one-step receipts."), true)
            },
            WarehouseOperationFlow.TwoStep => new[]
            {
                Step(InventoryOperationKind.Receipt, null, Required(warehouse.DefaultSourceLocationId, "Default input/source location is required for two-step receipts."), false),
                Step(InventoryOperationKind.InternalTransfer, Required(warehouse.DefaultSourceLocationId, "Default input/source location is required for two-step receipts."), Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for two-step receipts."), true)
            },
            WarehouseOperationFlow.ThreeStep => new[]
            {
                Step(InventoryOperationKind.Receipt, null, Required(warehouse.DefaultSourceLocationId, "Default input/source location is required for three-step receipts."), false),
                Step(InventoryOperationKind.QualityControl, Required(warehouse.DefaultSourceLocationId, "Default input/source location is required for three-step receipts."), Required(warehouse.DefaultQualityLocationId, "Default quality location is required for three-step receipts."), false),
                Step(InventoryOperationKind.InternalTransfer, Required(warehouse.DefaultQualityLocationId, "Default quality location is required for three-step receipts."), Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for three-step receipts."), true)
            },
            _ => throw new BadRequestException("Unsupported inbound warehouse flow.")
        };

        return await CreateOperationsAsync(dbContext, warehouse, request.CompanyId, request.BranchId, InventoryOperationFlowDirection.Receipt, request.SourceDocumentType, request.SourceDocumentId, request.SourceDocumentNumber, request.Lines, steps, userId, cancellationToken);
    }

    public static async Task<List<InventoryOperation>> EnsureDeliveryChainAsync(
        InventoryDbContext dbContext,
        Warehouse warehouse,
        EnsureInventoryDeliveryOperationChainCommand request,
        string userId,
        CancellationToken cancellationToken)
    {
        var existing = await LoadExistingAsync(dbContext, request.CompanyId, request.SourceDocumentType, request.SourceDocumentId, cancellationToken);
        if (existing.Count > 0)
            return existing;

        var steps = warehouse.OutboundFlow switch
        {
            WarehouseOperationFlow.OneStep => new[]
            {
                Step(InventoryOperationKind.Delivery, Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for one-step deliveries."), null, true)
            },
            WarehouseOperationFlow.TwoStep => new[]
            {
                Step(InventoryOperationKind.Pick, Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for two-step deliveries."), Required(warehouse.DefaultOutputLocationId, "Default output location is required for two-step deliveries."), false),
                Step(InventoryOperationKind.Delivery, Required(warehouse.DefaultOutputLocationId, "Default output location is required for two-step deliveries."), null, true)
            },
            WarehouseOperationFlow.ThreeStep => new[]
            {
                Step(InventoryOperationKind.Pick, Required(warehouse.DefaultDestinationLocationId, "Default destination/stock location is required for three-step deliveries."), Required(warehouse.DefaultPackingLocationId, "Default packing location is required for three-step deliveries."), false),
                Step(InventoryOperationKind.Pack, Required(warehouse.DefaultPackingLocationId, "Default packing location is required for three-step deliveries."), Required(warehouse.DefaultOutputLocationId, "Default output location is required for three-step deliveries."), false),
                Step(InventoryOperationKind.Delivery, Required(warehouse.DefaultOutputLocationId, "Default output location is required for three-step deliveries."), null, true)
            },
            _ => throw new BadRequestException("Unsupported outbound warehouse flow.")
        };

        return await CreateOperationsAsync(dbContext, warehouse, request.CompanyId, request.BranchId, InventoryOperationFlowDirection.Delivery, request.SourceDocumentType, request.SourceDocumentId, request.SourceDocumentNumber, request.Lines, steps, userId, cancellationToken);
    }

    public static async Task CreateBackorderAsync(InventoryDbContext dbContext, InventoryOperation operation, string userId, CancellationToken cancellationToken)
    {
        if (await dbContext.InventoryOperations.AnyAsync(x => x.BackorderOfOperationId == operation.Id, cancellationToken))
            return;

        var remainingLines = operation.Lines
            .Where(x => x.PlannedQuantity > x.DoneQuantity)
            .Select((line, index) => InventoryOperationLine.Create(
                index + 1,
                new InventoryOperationChainLine(line.ProductId, line.ProductSkuId, line.ProductPackageId, line.UnitId, line.BatchId, line.PlannedQuantity - line.DoneQuantity, line.UnitCost, line.TotalCost, line.CurrencyId, line.Notes, line.SourceDocumentLineId, line.ConsumeReservedQuantity),
                line.SourceLocationId,
                line.DestinationLocationId,
                userId))
            .ToList();

        if (remainingLines.Count == 0)
            return;

        var backorder = InventoryOperation.Create(operation.CompanyId, operation.BranchId, operation.WarehouseId, operation.FlowDirection, operation.OperationKind, operation.Sequence, operation.IsStockPostingStep, operation.SourceDocumentType, operation.SourceDocumentId, $"{operation.SourceDocumentNumber}-BO", remainingLines, userId, operation.Id);
        await dbContext.InventoryOperations.AddAsync(backorder, cancellationToken);
    }

    private static async Task<List<InventoryOperation>> LoadExistingAsync(InventoryDbContext dbContext, Guid companyId, string sourceDocumentType, Guid sourceDocumentId, CancellationToken cancellationToken) =>
        await dbContext.InventoryOperations.Include(x => x.Lines)
            .Where(x => x.CompanyId == companyId && x.SourceDocumentType == sourceDocumentType && x.SourceDocumentId == sourceDocumentId)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);

    private static async Task<List<InventoryOperation>> CreateOperationsAsync(
        InventoryDbContext dbContext,
        Warehouse warehouse,
        Guid companyId,
        Guid? branchId,
        InventoryOperationFlowDirection flowDirection,
        string sourceDocumentType,
        Guid sourceDocumentId,
        string sourceDocumentNumber,
        IReadOnlyList<InventoryOperationChainLine> sourceLines,
        IReadOnlyList<OperationStep> steps,
        string userId,
        CancellationToken cancellationToken)
    {
        var operations = new List<InventoryOperation>();
        foreach (var step in steps.Select((value, index) => new { value, index }))
        {
            var lines = sourceLines.Select((line, lineIndex) => InventoryOperationLine.Create(lineIndex + 1, line, step.value.SourceLocationId, step.value.DestinationLocationId, userId)).ToList();
            var operation = InventoryOperation.Create(companyId, branchId, warehouse.Id, flowDirection, step.value.OperationKind, step.index + 1, step.value.IsStockPostingStep, sourceDocumentType, sourceDocumentId, sourceDocumentNumber, lines, userId);
            operations.Add(operation);
            await dbContext.InventoryOperations.AddAsync(operation, cancellationToken);
        }

        return operations;
    }

    private static Guid Required(Guid? value, string message) => value ?? throw new BadRequestException(message);
    private static OperationStep Step(InventoryOperationKind kind, Guid? sourceLocationId, Guid? destinationLocationId, bool isStockPostingStep) => new(kind, sourceLocationId, destinationLocationId, isStockPostingStep);
    private readonly record struct OperationStep(InventoryOperationKind OperationKind, Guid? SourceLocationId, Guid? DestinationLocationId, bool IsStockPostingStep);
}

file static class InventoryOperationHelpers
{
    public static string GetUserId(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static decimal ResolveDoneCost(InventoryOperationLine line) =>
        line.PlannedQuantity <= 0 ? 0m : Math.Round(line.TotalCost * line.DoneQuantity / line.PlannedQuantity, 2);
}
