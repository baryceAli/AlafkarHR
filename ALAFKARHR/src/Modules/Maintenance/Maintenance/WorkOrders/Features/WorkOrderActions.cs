namespace Maintenance.WorkOrders.Features;

public record AssignMaintenanceWorkOrderRequest(AssignMaintenanceWorkOrderDto Assignment);
public record AssignMaintenanceWorkOrderCommand(Guid Id, AssignMaintenanceWorkOrderDto Assignment) : ICommand<MaintenanceActionResult>;
public record ChangeMaintenanceWorkOrderStatusRequest(ChangeMaintenanceWorkOrderStatusDto WorkOrderStatus);
public record ChangeMaintenanceWorkOrderStatusCommand(Guid Id, MaintenanceWorkOrderStatus Status) : ICommand<MaintenanceActionResult>;
public record ApproveMaintenanceCostRequest(ApproveMaintenanceCostDto CostApproval);
public record ApproveMaintenanceCostCommand(Guid Id, ApproveMaintenanceCostDto CostApproval) : ICommand<MaintenanceActionResult>;
public record AddMaintenanceCommentRequest(CreateMaintenanceCommentDto Comment);
public record AddMaintenanceCommentCommand(Guid Id, CreateMaintenanceCommentDto Comment) : ICommand<MaintenanceCreateResult>;
public record DeleteMaintenanceWorkOrderCommand(Guid Id) : ICommand<MaintenanceActionResult>;
public record MaintenanceActionResult(bool IsSuccess);
public record MaintenanceCreateResult(Guid Id);

public class MaintenanceWorkOrderActionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/maintenance/work-orders/{id:guid}/assign", async (Guid id, AssignMaintenanceWorkOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AssignMaintenanceWorkOrderCommand(id, request.Assignment));
            return Results.Ok(result);
        })
        .WithName("AssignMaintenanceWorkOrder")
        .Produces<MaintenanceActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Assign);

        app.MapPut("/api/v1/maintenance/work-orders/{id:guid}/status", async (Guid id, ChangeMaintenanceWorkOrderStatusRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ChangeMaintenanceWorkOrderStatusCommand(id, request.WorkOrderStatus.Status));
            return Results.Ok(result);
        })
        .WithName("ChangeMaintenanceWorkOrderStatus")
        .Produces<MaintenanceActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Edit);

        app.MapPut("/api/v1/maintenance/work-orders/{id:guid}/cost-approval", async (Guid id, ApproveMaintenanceCostRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ApproveMaintenanceCostCommand(id, request.CostApproval));
            return Results.Ok(result);
        })
        .WithName("ApproveMaintenanceCost")
        .Produces<MaintenanceActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.ApproveCost);

        app.MapPost("/api/v1/maintenance/work-orders/{id:guid}/comments", async (Guid id, AddMaintenanceCommentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AddMaintenanceCommentCommand(id, request.Comment));
            return Results.Created($"/api/v1/maintenance/work-orders/{id}", result);
        })
        .WithName("AddMaintenanceComment")
        .Produces<MaintenanceCreateResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.View);

        app.MapDelete("/api/v1/maintenance/work-orders/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMaintenanceWorkOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteMaintenanceWorkOrder")
        .Produces<MaintenanceActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Delete);
    }
}

public class AssignMaintenanceWorkOrderHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AssignMaintenanceWorkOrderCommand, MaintenanceActionResult>
{
    public async Task<MaintenanceActionResult> Handle(AssignMaintenanceWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        workOrder.Assign(request.Assignment.AssignedToUserId, request.Assignment.DueDate, currentUserId);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "Assigned", $"Assigned to {request.Assignment.AssignedToUserId}.", currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new MaintenanceActionResult(true);
    }
}

public class ChangeMaintenanceWorkOrderStatusHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ChangeMaintenanceWorkOrderStatusCommand, MaintenanceActionResult>
{
    public async Task<MaintenanceActionResult> Handle(ChangeMaintenanceWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        workOrder.ChangeStatus(request.Status, currentUserId);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "StatusChanged", $"Status changed to {request.Status}.", currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new MaintenanceActionResult(true);
    }
}

public class ApproveMaintenanceCostHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApproveMaintenanceCostCommand, MaintenanceActionResult>
{
    public async Task<MaintenanceActionResult> Handle(ApproveMaintenanceCostCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        workOrder.ApproveCost(request.CostApproval.IsApproved, request.CostApproval.ApprovedCost, request.CostApproval.ApprovalNotes, currentUserId);
        MaintenanceFeatureHelpers.AddHistory(workOrder, request.CostApproval.IsApproved ? "CostApproved" : "CostRejected", request.CostApproval.ApprovalNotes, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new MaintenanceActionResult(true);
    }
}

public class AddMaintenanceCommentHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AddMaintenanceCommentCommand, MaintenanceCreateResult>
{
    public async Task<MaintenanceCreateResult> Handle(AddMaintenanceCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        var comment = MaintenanceComment.Create(workOrder.Id, request.Comment.Comment, currentUserId);
        workOrder.AddComment(comment);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "CommentAdded", "Comment added.", currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new MaintenanceCreateResult(comment.Id);
    }
}

public class DeleteMaintenanceWorkOrderHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteMaintenanceWorkOrderCommand, MaintenanceActionResult>
{
    public async Task<MaintenanceActionResult> Handle(DeleteMaintenanceWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var workOrder = await dbContext.MaintenanceWorkOrders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        workOrder.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new MaintenanceActionResult(true);
    }
}
