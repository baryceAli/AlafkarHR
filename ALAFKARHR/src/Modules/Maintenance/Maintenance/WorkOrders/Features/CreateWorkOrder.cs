namespace Maintenance.WorkOrders.Features;

public record CreateMaintenanceWorkOrderRequest(CreateMaintenanceWorkOrderDto WorkOrder);
public record CreateMaintenanceWorkOrderCommand(CreateMaintenanceWorkOrderDto WorkOrder) : ICommand<CreateMaintenanceWorkOrderResult>;
public record CreateMaintenanceWorkOrderResult(Guid Id);

public class CreateMaintenanceWorkOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/maintenance/work-orders", async (CreateMaintenanceWorkOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateMaintenanceWorkOrderCommand(request.WorkOrder));
            return Results.Created($"/api/v1/maintenance/work-orders/{result.Id}", result);
        })
        .WithName("CreateMaintenanceWorkOrder")
        .Produces<CreateMaintenanceWorkOrderResult>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Maintenance Work Order")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Create);
    }
}

public class CreateMaintenanceWorkOrderHandler(
    MaintenanceDbContext dbContext,
    IMaintenanceNumberGenerator numberGenerator,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateMaintenanceWorkOrderCommand, CreateMaintenanceWorkOrderResult>
{
    public async Task<CreateMaintenanceWorkOrderResult> Handle(CreateMaintenanceWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await MaintenanceFeatureHelpers.EnsureAssetAsync(dbContext, request.WorkOrder.AssetId, cancellationToken);
        var workOrderNumber = await numberGenerator.GenerateWorkOrderNumberAsync(cancellationToken);

        var workOrder = MaintenanceWorkOrder.Create(
            workOrderNumber,
            request.WorkOrder.Title,
            request.WorkOrder.Description,
            request.WorkOrder.AssetId,
            currentUserId,
            request.WorkOrder.Priority,
            request.WorkOrder.DueDate,
            request.WorkOrder.Category,
            request.WorkOrder.InternalNotes,
            request.WorkOrder.EstimatedCost,
            request.WorkOrder.ActualCost,
            request.WorkOrder.CurrencyCode,
            request.WorkOrder.VendorName,
            request.WorkOrder.SupplierId);

        MaintenanceFeatureHelpers.AddHistory(workOrder, "Created", "Work order created.", currentUserId);
        dbContext.MaintenanceWorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateMaintenanceWorkOrderResult(workOrder.Id);
    }
}
