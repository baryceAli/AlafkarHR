namespace Maintenance.WorkOrders.Features;

public record GetMaintenanceWorkOrderByIdQuery(Guid Id) : IQuery<GetMaintenanceWorkOrderByIdResult>;
public record GetMaintenanceWorkOrderByIdResult(MaintenanceWorkOrderDto WorkOrder);

public class GetMaintenanceWorkOrderByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/maintenance/work-orders/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceWorkOrderByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceWorkOrderById")
        .Produces<GetMaintenanceWorkOrderByIdResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Maintenance Work Order By Id")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.View);
    }
}

public class GetMaintenanceWorkOrderByIdHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetMaintenanceWorkOrderByIdQuery, GetMaintenanceWorkOrderByIdResult>
{
    public async Task<GetMaintenanceWorkOrderByIdResult> Handle(GetMaintenanceWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.MaintenanceWorkOrders
            .Include(x => x.Asset)
            .Include(x => x.Comments)
            .Include(x => x.Attachments)
            .Include(x => x.History)
            .AsNoTracking()
            .AsQueryable();

        query = MaintenanceFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId);
        var workOrder = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.Id);

        return new GetMaintenanceWorkOrderByIdResult(MaintenanceFeatureHelpers.ToDto(workOrder));
    }
}
