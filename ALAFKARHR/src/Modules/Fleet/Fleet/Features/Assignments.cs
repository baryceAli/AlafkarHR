namespace Fleet.Features;

public record CreateFleetVehicleAssignmentRequest(CreateFleetVehicleAssignmentDto Assignment);
public record ReturnFleetVehicleAssignmentRequest(ReturnFleetVehicleAssignmentDto Return);
public record CreateFleetVehicleAssignmentCommand(CreateFleetVehicleAssignmentDto Assignment) : ICommand<CreateFleetVehicleAssignmentResult>;
public record ReturnFleetVehicleAssignmentCommand(Guid Id, ReturnFleetVehicleAssignmentDto Return) : ICommand<FleetActionResult>;
public record CancelFleetVehicleAssignmentCommand(Guid Id) : ICommand<FleetActionResult>;
public record GetFleetVehicleAssignmentsQuery(PaginationRequest PaginationRequest, Guid? VehicleId, FleetAssignmentStatus? Status) : IQuery<GetFleetVehicleAssignmentsResult>;
public record CreateFleetVehicleAssignmentResult(Guid Id);
public record GetFleetVehicleAssignmentsResult(PaginatedResult<FleetVehicleAssignmentDto> Assignments);

public class FleetVehicleAssignmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/assignments", async (int PageIndex, int PageSize, string? searchText, Guid? vehicleId, FleetAssignmentStatus? status, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetVehicleAssignmentsQuery(new PaginationRequest(PageIndex, PageSize, searchText), vehicleId, status));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicleAssignments")
        .Produces<GetFleetVehicleAssignmentsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleAssignmentPermissions.View);

        app.MapPost("/api/v1/fleet/assignments", async (CreateFleetVehicleAssignmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateFleetVehicleAssignmentCommand(request.Assignment));
            return Results.Created($"/api/v1/fleet/assignments/{result.Id}", result);
        })
        .WithName("CreateFleetVehicleAssignment")
        .Produces<CreateFleetVehicleAssignmentResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.FleetVehicleAssignmentPermissions.Create);

        app.MapPut("/api/v1/fleet/assignments/{id:guid}/return", async (Guid id, ReturnFleetVehicleAssignmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ReturnFleetVehicleAssignmentCommand(id, request.Return));
            return Results.Ok(result);
        })
        .WithName("ReturnFleetVehicleAssignment")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleAssignmentPermissions.Close);

        app.MapPut("/api/v1/fleet/assignments/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelFleetVehicleAssignmentCommand(id));
            return Results.Ok(result);
        })
        .WithName("CancelFleetVehicleAssignment")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleAssignmentPermissions.Edit);
    }
}

public class GetFleetVehicleAssignmentsHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetVehicleAssignmentsQuery, GetFleetVehicleAssignmentsResult>
{
    public async Task<GetFleetVehicleAssignmentsResult> Handle(GetFleetVehicleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.VehicleAssignments.Include(x => x.Vehicle).AsNoTracking().AsQueryable();
        if (request.VehicleId.HasValue)
            query = query.Where(x => x.VehicleId == request.VehicleId.Value);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x => x.Vehicle.Name.ToLower().Contains(search) || x.Vehicle.PlateNumber.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var assignments = await query.OrderByDescending(x => x.StartDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetFleetVehicleAssignmentsResult(new PaginatedResult<FleetVehicleAssignmentDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            assignments.Select(FleetFeatureHelpers.ToDto).ToList()));
    }
}

public class CreateFleetVehicleAssignmentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateFleetVehicleAssignmentCommand, CreateFleetVehicleAssignmentResult>
{
    public async Task<CreateFleetVehicleAssignmentResult> Handle(CreateFleetVehicleAssignmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Assignment.VehicleId, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Assignment.VehicleId);

        var hasActiveAssignment = await dbContext.VehicleAssignments.AnyAsync(x => x.VehicleId == request.Assignment.VehicleId && x.Status == FleetAssignmentStatus.Active, cancellationToken);
        if (hasActiveAssignment)
            throw new BadRequestException("Vehicle already has an active assignment.");

        var assignment = FleetVehicleAssignment.Create(request.Assignment, currentUserId);
        dbContext.VehicleAssignments.Add(assignment);
        vehicle.SetStatus(FleetVehicleStatus.Assigned, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateFleetVehicleAssignmentResult(assignment.Id);
    }
}

public class ReturnFleetVehicleAssignmentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ReturnFleetVehicleAssignmentCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(ReturnFleetVehicleAssignmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var assignment = await dbContext.VehicleAssignments.Include(x => x.Vehicle).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle assignment", request.Id);

        assignment.Return(request.Return, currentUserId);
        if (request.Return.OdometerIn.HasValue)
            assignment.Vehicle.UpdateOdometer(request.Return.OdometerIn.Value, currentUserId);
        assignment.Vehicle.SetStatus(FleetVehicleStatus.Active, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class CancelFleetVehicleAssignmentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CancelFleetVehicleAssignmentCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(CancelFleetVehicleAssignmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var assignment = await dbContext.VehicleAssignments.Include(x => x.Vehicle).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle assignment", request.Id);

        assignment.Cancel(currentUserId);
        assignment.Vehicle.SetStatus(FleetVehicleStatus.Active, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}
