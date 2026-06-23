namespace Maintenance.WorkOrders.Features;

public record UpdateMaintenanceWorkOrderRequest(UpdateMaintenanceWorkOrderDto WorkOrder);
public record UpdateMaintenanceWorkOrderCommand(UpdateMaintenanceWorkOrderDto WorkOrder) : ICommand<UpdateMaintenanceWorkOrderResult>;
public record UpdateMaintenanceWorkOrderResult(bool IsSuccess);

public class UpdateMaintenanceWorkOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/maintenance/work-orders", async (UpdateMaintenanceWorkOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateMaintenanceWorkOrderCommand(request.WorkOrder));
            return Results.Ok(result);
        })
        .WithName("UpdateMaintenanceWorkOrder")
        .Produces<UpdateMaintenanceWorkOrderResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Maintenance Work Order")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Edit);
    }
}

public class UpdateMaintenanceWorkOrderHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateMaintenanceWorkOrderCommand, UpdateMaintenanceWorkOrderResult>
{
    public async Task<UpdateMaintenanceWorkOrderResult> Handle(UpdateMaintenanceWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(x => x.Id == request.WorkOrder.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.WorkOrder.Id);

        if (workOrder.Asset is null || workOrder.Asset.IsDeleted)
            throw new NotFoundException("Maintenance asset", workOrder.AssetId);

        var newAsset = await dbContext.MaintenanceAssets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.WorkOrder.AssetId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Maintenance asset", request.WorkOrder.AssetId);

        if (workOrder.Asset.CompanyId != newAsset.CompanyId)
            throw new BadRequestException("Maintenance work order asset company cannot be changed.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(workOrder.Asset.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, workOrder.Asset.BranchId) ||
            !BranchScopePolicy.CanMutate(branchAccess, newAsset.BranchId))
        {
            throw new ForbiddenException("You do not have permission to update this work order branch scope.");
        }

        workOrder.Update(
            request.WorkOrder.Title,
            request.WorkOrder.Description,
            request.WorkOrder.AssetId,
            request.WorkOrder.Priority,
            request.WorkOrder.DueDate,
            request.WorkOrder.Category,
            request.WorkOrder.InternalNotes,
            request.WorkOrder.EstimatedCost,
            request.WorkOrder.ActualCost,
            request.WorkOrder.CurrencyId,
            request.WorkOrder.CurrencyCode,
            request.WorkOrder.VendorName,
            request.WorkOrder.SupplierId,
            currentUserId);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "Updated", "Work order updated.", currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMaintenanceWorkOrderResult(true);
    }
}
