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

public class UpdateMaintenanceWorkOrderHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateMaintenanceWorkOrderCommand, UpdateMaintenanceWorkOrderResult>
{
    public async Task<UpdateMaintenanceWorkOrderResult> Handle(UpdateMaintenanceWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await MaintenanceFeatureHelpers.EnsureAssetAsync(dbContext, request.WorkOrder.AssetId, cancellationToken);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.WorkOrder.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.WorkOrder.Id);

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
            request.WorkOrder.CurrencyCode,
            request.WorkOrder.VendorName,
            request.WorkOrder.SupplierId,
            currentUserId);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "Updated", "Work order updated.", currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMaintenanceWorkOrderResult(true);
    }
}
