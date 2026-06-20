namespace Fleet.Features;

public record CreateFleetVehicleServiceRuleRequest(CreateFleetVehicleServiceRuleDto ServiceRule);
public record UpdateFleetVehicleServiceRuleRequest(UpdateFleetVehicleServiceRuleDto ServiceRule);
public record CompleteFleetVehicleServiceRuleRequest(int? Odometer, DateTime ServiceDate);
public record CreateFleetVehicleServiceRuleCommand(CreateFleetVehicleServiceRuleDto ServiceRule) : ICommand<CreateFleetVehicleServiceRuleResult>;
public record UpdateFleetVehicleServiceRuleCommand(UpdateFleetVehicleServiceRuleDto ServiceRule) : ICommand<FleetActionResult>;
public record DeleteFleetVehicleServiceRuleCommand(Guid Id) : ICommand<FleetActionResult>;
public record CompleteFleetVehicleServiceRuleCommand(Guid Id, CompleteFleetVehicleServiceRuleRequest ServiceCompletion) : ICommand<FleetActionResult>;
public record CreateMaintenanceFromFleetServiceRuleCommand(Guid Id) : ICommand<CreateEmergencyFleetMaintenanceResult>;
public record GetFleetVehicleServiceRulesQuery(PaginationRequest PaginationRequest, Guid? VehicleId, bool? DueOnly) : IQuery<GetFleetVehicleServiceRulesResult>;
public record CreateFleetVehicleServiceRuleResult(Guid Id);
public record GetFleetVehicleServiceRulesResult(PaginatedResult<FleetVehicleServiceRuleDto> ServiceRules);

public class FleetVehicleServiceRuleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/service-rules", async (int PageIndex, int PageSize, string? searchText, Guid? vehicleId, bool? dueOnly, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetVehicleServiceRulesQuery(new PaginationRequest(PageIndex, PageSize, searchText), vehicleId, dueOnly));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicleServiceRules")
        .Produces<GetFleetVehicleServiceRulesResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.View);

        app.MapPost("/api/v1/fleet/service-rules", async (CreateFleetVehicleServiceRuleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateFleetVehicleServiceRuleCommand(request.ServiceRule));
            return Results.Created($"/api/v1/fleet/service-rules/{result.Id}", result);
        })
        .WithName("CreateFleetVehicleServiceRule")
        .Produces<CreateFleetVehicleServiceRuleResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Create);

        app.MapPut("/api/v1/fleet/service-rules", async (UpdateFleetVehicleServiceRuleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateFleetVehicleServiceRuleCommand(request.ServiceRule));
            return Results.Ok(result);
        })
        .WithName("UpdateFleetVehicleServiceRule")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Edit);

        app.MapPut("/api/v1/fleet/service-rules/{id:guid}/complete", async (Guid id, CompleteFleetVehicleServiceRuleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CompleteFleetVehicleServiceRuleCommand(id, request));
            return Results.Ok(result);
        })
        .WithName("CompleteFleetVehicleServiceRule")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Edit);

        app.MapPost("/api/v1/fleet/service-rules/{id:guid}/maintenance-work-order", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CreateMaintenanceFromFleetServiceRuleCommand(id));
            return Results.Created($"/api/v1/maintenance/work-orders/{result.Id}", result);
        })
        .WithName("CreateMaintenanceFromFleetServiceRule")
        .Produces<CreateEmergencyFleetMaintenanceResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Create);

        app.MapDelete("/api/v1/fleet/service-rules/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteFleetVehicleServiceRuleCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteFleetVehicleServiceRule")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Delete);
    }
}

public class GetFleetVehicleServiceRulesHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetVehicleServiceRulesQuery, GetFleetVehicleServiceRulesResult>
{
    public async Task<GetFleetVehicleServiceRulesResult> Handle(GetFleetVehicleServiceRulesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.VehicleServiceRules.Include(x => x.Vehicle).AsNoTracking().AsQueryable();
        if (request.VehicleId.HasValue)
            query = query.Where(x => x.VehicleId == request.VehicleId.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x => x.Vehicle.Name.ToLower().Contains(search) || x.Vehicle.PlateNumber.ToLower().Contains(search));
        }

        var allRules = await query.OrderBy(x => x.NextDueDate).ThenBy(x => x.NextDueOdometer).ToListAsync(cancellationToken);
        if (request.DueOnly == true)
            allRules = allRules.Where(x => x.IsDue(x.Vehicle.CurrentOdometer, DateTime.UtcNow)).ToList();

        var count = allRules.Count;
        var rules = allRules
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToList();

        return new GetFleetVehicleServiceRulesResult(new PaginatedResult<FleetVehicleServiceRuleDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            rules.Select(FleetFeatureHelpers.ToDto).ToList()));
    }
}

public class CreateFleetVehicleServiceRuleHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateFleetVehicleServiceRuleCommand, CreateFleetVehicleServiceRuleResult>
{
    public async Task<CreateFleetVehicleServiceRuleResult> Handle(CreateFleetVehicleServiceRuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await FleetFeatureHelpers.EnsureVehicleAsync(dbContext, request.ServiceRule.VehicleId, cancellationToken);
        var rule = FleetVehicleServiceRule.Create(request.ServiceRule, currentUserId);
        dbContext.VehicleServiceRules.Add(rule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateFleetVehicleServiceRuleResult(rule.Id);
    }
}

public class UpdateFleetVehicleServiceRuleHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateFleetVehicleServiceRuleCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(UpdateFleetVehicleServiceRuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var rule = await dbContext.VehicleServiceRules.FirstOrDefaultAsync(x => x.Id == request.ServiceRule.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet service rule", request.ServiceRule.Id);
        rule.Update(request.ServiceRule, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class CompleteFleetVehicleServiceRuleHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CompleteFleetVehicleServiceRuleCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(CompleteFleetVehicleServiceRuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var rule = await dbContext.VehicleServiceRules.Include(x => x.Vehicle).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet service rule", request.Id);
        rule.CompleteService(request.ServiceCompletion.Odometer, request.ServiceCompletion.ServiceDate, currentUserId);
        if (request.ServiceCompletion.Odometer.HasValue && request.ServiceCompletion.Odometer.Value > rule.Vehicle.CurrentOdometer)
            rule.Vehicle.UpdateOdometer(request.ServiceCompletion.Odometer.Value, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class DeleteFleetVehicleServiceRuleHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteFleetVehicleServiceRuleCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(DeleteFleetVehicleServiceRuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var rule = await dbContext.VehicleServiceRules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet service rule", request.Id);
        rule.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class CreateMaintenanceFromFleetServiceRuleHandler(
    FleetDbContext dbContext,
    MaintenanceDbContext maintenanceDbContext,
    GeneralSettingsDbContext generalSettingsDbContext,
    IMaintenanceNumberGenerator numberGenerator,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateMaintenanceFromFleetServiceRuleCommand, CreateEmergencyFleetMaintenanceResult>
{
    public async Task<CreateEmergencyFleetMaintenanceResult> Handle(CreateMaintenanceFromFleetServiceRuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var rule = await dbContext.VehicleServiceRules.Include(x => x.Vehicle).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet service rule", request.Id);

        if (!rule.Vehicle.MaintenanceAssetId.HasValue)
            throw new BadRequestException("Vehicle is not linked to a maintenance asset.");

        var currency = await generalSettingsDbContext.Currencies
            .AsNoTracking()
            .Where(x => x.CompanyId == rule.Vehicle.CompanyId && !x.IsDeleted)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException("No currency is configured for the vehicle company.");

        var number = await numberGenerator.GenerateWorkOrderNumberAsync(cancellationToken);
        var workOrder = MaintenanceWorkOrder.Create(
            number,
            $"{rule.ServiceType} - {rule.Vehicle.PlateNumber}",
            rule.Notes ?? "Scheduled fleet service.",
            rule.Vehicle.MaintenanceAssetId.Value,
            currentUserId,
            MaintenancePriority.Medium,
            rule.NextDueDate,
            "Fleet Regular",
            null,
            null,
            null,
            currency.Id,
            currency.Code,
            null,
            null);

        workOrder.AddHistory(MaintenanceHistory.Create(workOrder.Id, "CreatedFromFleetServiceRule", "Regular fleet service work order created.", currentUserId));
        maintenanceDbContext.MaintenanceWorkOrders.Add(workOrder);
        await FleetMaintenanceAssetSync.MarkUnderMaintenanceAsync(maintenanceDbContext, rule.Vehicle.MaintenanceAssetId.Value, currentUserId, cancellationToken);
        rule.Vehicle.SetStatus(FleetVehicleStatus.UnderMaintenance, currentUserId);
        await maintenanceDbContext.SaveChangesAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEmergencyFleetMaintenanceResult(workOrder.Id);
    }
}
