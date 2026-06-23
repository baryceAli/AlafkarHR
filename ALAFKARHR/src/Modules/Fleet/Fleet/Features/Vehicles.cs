namespace Fleet.Features;

public record CreateFleetVehicleRequest(CreateFleetVehicleDto Vehicle);
public record UpdateFleetVehicleRequest(UpdateFleetVehicleDto Vehicle);
public record UpdateFleetVehicleOdometerRequest(int Odometer);
public record CreateEmergencyFleetMaintenanceRequest(Guid VehicleId, string Title, string Description, MaintenancePriority Priority, decimal? EstimatedCost, Guid? CurrencyId, string? CurrencyCode, string? VendorName, Guid? SupplierId);
public record CreateFleetVehicleCommand(CreateFleetVehicleDto Vehicle) : ICommand<CreateFleetVehicleResult>;
public record UpdateFleetVehicleCommand(UpdateFleetVehicleDto Vehicle) : ICommand<FleetActionResult>;
public record DeleteFleetVehicleCommand(Guid Id) : ICommand<FleetActionResult>;
public record UpdateFleetVehicleOdometerCommand(Guid Id, int Odometer) : ICommand<FleetActionResult>;
public record CreateEmergencyFleetMaintenanceCommand(CreateEmergencyFleetMaintenanceRequest WorkOrder) : ICommand<CreateEmergencyFleetMaintenanceResult>;
public record RepairFleetVehicleMaintenanceLinksCommand(Guid? CompanyId) : ICommand<RepairFleetVehicleMaintenanceLinksResult>;
public record GetFleetVehiclesQuery(PaginationRequest PaginationRequest, FleetVehicleFilterDto Filter) : IQuery<GetFleetVehiclesResult>;
public record GetFleetVehicleByIdQuery(Guid Id) : IQuery<GetFleetVehicleByIdResult>;
public record CreateFleetVehicleResult(Guid Id);
public record CreateEmergencyFleetMaintenanceResult(Guid Id);
public record RepairFleetVehicleMaintenanceLinksResult(int RepairedCount);
public record GetFleetVehiclesResult(PaginatedResult<FleetVehicleDto> Vehicles);
public record GetFleetVehicleByIdResult(FleetVehicleDetailsDto VehicleDetails);
public record FleetActionResult(bool IsSuccess);

public class FleetVehicleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/vehicles", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            Guid? companyId,
            Guid? branchId,
            FleetVehicleOwnershipType? ownershipType,
            FleetVehicleStatus? status,
            FleetVehicleType? vehicleType,
            ISender sender) =>
        {
            var filter = new FleetVehicleFilterDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                OwnershipType = ownershipType,
                Status = status,
                VehicleType = vehicleType
            };
            var result = await sender.Send(new GetFleetVehiclesQuery(new PaginationRequest(PageIndex, PageSize, searchText), filter));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicles")
        .Produces<GetFleetVehiclesResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.View);

        app.MapGet("/api/v1/fleet/vehicles/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetVehicleByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicleById")
        .Produces<GetFleetVehicleByIdResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.View);

        app.MapPost("/api/v1/fleet/vehicles", async (CreateFleetVehicleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateFleetVehicleCommand(request.Vehicle));
            return Results.Created($"/api/v1/fleet/vehicles/{result.Id}", result);
        })
        .WithName("CreateFleetVehicle")
        .Produces<CreateFleetVehicleResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Create);

        app.MapPut("/api/v1/fleet/vehicles", async (UpdateFleetVehicleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateFleetVehicleCommand(request.Vehicle));
            return Results.Ok(result);
        })
        .WithName("UpdateFleetVehicle")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Edit);

        app.MapDelete("/api/v1/fleet/vehicles/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteFleetVehicleCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteFleetVehicle")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Delete);

        app.MapPut("/api/v1/fleet/vehicles/{id:guid}/odometer", async (Guid id, UpdateFleetVehicleOdometerRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateFleetVehicleOdometerCommand(id, request.Odometer));
            return Results.Ok(result);
        })
        .WithName("UpdateFleetVehicleOdometer")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Edit);

        app.MapPost("/api/v1/fleet/vehicles/maintenance/emergency", async (CreateEmergencyFleetMaintenanceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmergencyFleetMaintenanceCommand(request));
            return Results.Created($"/api/v1/maintenance/work-orders/{result.Id}", result);
        })
        .WithName("CreateEmergencyFleetMaintenance")
        .Produces<CreateEmergencyFleetMaintenanceResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Create);

        app.MapPost("/api/v1/fleet/vehicles/maintenance/repair-links", async (Guid? companyId, ISender sender) =>
        {
            var result = await sender.Send(new RepairFleetVehicleMaintenanceLinksCommand(companyId));
            return Results.Ok(result);
        })
        .WithName("RepairFleetVehicleMaintenanceLinks")
        .Produces<RepairFleetVehicleMaintenanceLinksResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehiclePermissions.Edit);
    }
}

public class CreateFleetVehicleHandler(
    FleetDbContext dbContext,
    IFleetNumberGenerator fleetNumberGenerator,
    ISender sender,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateFleetVehicleCommand, CreateFleetVehicleResult>
{
    public async Task<CreateFleetVehicleResult> Handle(CreateFleetVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicleCode = string.IsNullOrWhiteSpace(request.Vehicle.VehicleCode)
            ? await fleetNumberGenerator.GenerateVehicleCodeAsync(cancellationToken)
            : request.Vehicle.VehicleCode.Trim();

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, request.Vehicle.BranchId))
            throw new ForbiddenException("You do not have permission to create a fleet vehicle in this branch scope.");

        var vehicle = FleetVehicle.Create(vehicleCode, request.Vehicle, currentUserId);
        dbContext.Vehicles.Add(vehicle);
        await dbContext.SaveChangesAsync(cancellationToken);

        var asset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Fleet",
            nameof(FleetVehicle),
            vehicle.Id,
            null,
            request.Vehicle.Name,
            request.Vehicle.NameEng,
            MaintenanceAssetType.Vehicle,
            request.Vehicle.Status == FleetVehicleStatus.UnderMaintenance ? MaintenanceAssetStatus.UnderMaintenance : MaintenanceAssetStatus.Active,
            request.Vehicle.CompanyId,
            request.Vehicle.BranchId,
            null,
            request.Vehicle.Notes,
            request.Vehicle.PlateNumber,
            request.Vehicle.Vin ?? request.Vehicle.EngineNumber,
            request.Vehicle.PurchaseDate,
            request.Vehicle.WarrantyEndDate), cancellationToken);

        vehicle.LinkMaintenanceAsset(asset.MaintenanceAssetId, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateFleetVehicleResult(vehicle.Id);
    }
}

public class UpdateFleetVehicleHandler(FleetDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateFleetVehicleCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(UpdateFleetVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Vehicle.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Vehicle.Id);

        if (request.Vehicle.CompanyId != vehicle.CompanyId)
            throw new BadRequestException("Fleet vehicle company cannot be changed.");

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, vehicle.BranchId) ||
            !BranchScopePolicy.CanMutate(branchAccess, request.Vehicle.BranchId))
        {
            throw new ForbiddenException("You do not have permission to update this fleet vehicle branch scope.");
        }

        vehicle.Update(request.Vehicle, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        var asset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Fleet",
            nameof(FleetVehicle),
            vehicle.Id,
            null,
            vehicle.Name,
            vehicle.NameEng,
            MaintenanceAssetType.Vehicle,
            vehicle.Status == FleetVehicleStatus.UnderMaintenance ? MaintenanceAssetStatus.UnderMaintenance : MaintenanceAssetStatus.Active,
            vehicle.CompanyId,
            vehicle.BranchId,
            null,
            vehicle.Notes,
            vehicle.PlateNumber,
            vehicle.Vin ?? vehicle.EngineNumber,
            vehicle.PurchaseDate,
            vehicle.WarrantyEndDate), cancellationToken);

        if (vehicle.MaintenanceAssetId != asset.MaintenanceAssetId)
        {
            vehicle.LinkMaintenanceAsset(asset.MaintenanceAssetId, currentUserId);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return new FleetActionResult(true);
    }
}

public class DeleteFleetVehicleHandler(FleetDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteFleetVehicleCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(DeleteFleetVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Id);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, vehicle.BranchId))
            throw new ForbiddenException("You do not have permission to delete this fleet vehicle branch scope.");

        await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Fleet",
            nameof(FleetVehicle),
            vehicle.Id,
            null,
            vehicle.Name,
            vehicle.NameEng,
            MaintenanceAssetType.Vehicle,
            MaintenanceAssetStatus.Inactive,
            vehicle.CompanyId,
            vehicle.BranchId,
            null,
            vehicle.Notes,
            vehicle.PlateNumber,
            vehicle.Vin ?? vehicle.EngineNumber,
            vehicle.PurchaseDate,
            vehicle.WarrantyEndDate), cancellationToken);

        vehicle.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class UpdateFleetVehicleOdometerHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateFleetVehicleOdometerCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(UpdateFleetVehicleOdometerCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Id);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, vehicle.BranchId))
            throw new ForbiddenException("You do not have permission to update this fleet vehicle branch scope.");

        vehicle.UpdateOdometer(request.Odometer, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class RepairFleetVehicleMaintenanceLinksHandler(FleetDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RepairFleetVehicleMaintenanceLinksCommand, RepairFleetVehicleMaintenanceLinksResult>
{
    public async Task<RepairFleetVehicleMaintenanceLinksResult> Handle(RepairFleetVehicleMaintenanceLinksCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.Vehicles.Where(x => !x.MaintenanceAssetId.HasValue);

        if (request.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.CompanyId.Value);

        var vehicles = await query.ToListAsync(cancellationToken);
        foreach (var vehicle in vehicles)
        {
            var asset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
                "Fleet",
                nameof(FleetVehicle),
                vehicle.Id,
                null,
                vehicle.Name,
                vehicle.NameEng,
                MaintenanceAssetType.Vehicle,
                vehicle.Status == FleetVehicleStatus.UnderMaintenance ? MaintenanceAssetStatus.UnderMaintenance : MaintenanceAssetStatus.Active,
                vehicle.CompanyId,
                vehicle.BranchId,
                null,
                vehicle.Notes,
                vehicle.PlateNumber,
                vehicle.Vin ?? vehicle.EngineNumber,
                vehicle.PurchaseDate,
                vehicle.WarrantyEndDate), cancellationToken);

            vehicle.LinkMaintenanceAsset(asset.MaintenanceAssetId, currentUserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RepairFleetVehicleMaintenanceLinksResult(vehicles.Count);
    }
}

public class GetFleetVehiclesHandler(FleetDbContext dbContext, ISender sender)
    : IQueryHandler<GetFleetVehiclesQuery, GetFleetVehiclesResult>
{
    public async Task<GetFleetVehiclesResult> Handle(GetFleetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Vehicles.AsNoTracking().AsQueryable();

        if (request.Filter.CompanyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == request.Filter.CompanyId.Value);
            var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId.Value), cancellationToken);
            if (!BranchScopePolicy.CanFilter(branchAccess, request.Filter.BranchId))
                throw new ForbiddenException("You do not have permission to view this fleet vehicle branch scope.");

            if (branchAccess.CanViewAllBranches)
            {
                if (request.Filter.BranchId.HasValue)
                    query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);
            }
            else
            {
                query = request.Filter.BranchId.HasValue
                    ? query.Where(x => x.BranchId == null || x.BranchId == request.Filter.BranchId.Value)
                    : query.Where(x => x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)));
            }
        }
        else if (request.Filter.BranchId.HasValue)
        {
            query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);
        }
        if (request.Filter.OwnershipType.HasValue)
            query = query.Where(x => x.OwnershipType == request.Filter.OwnershipType.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (request.Filter.VehicleType.HasValue)
            query = query.Where(x => x.VehicleType == request.Filter.VehicleType.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x =>
                x.VehicleCode.ToLower().Contains(search) ||
                x.PlateNumber.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search) ||
                x.NameEng.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var vehicles = await query
            .OrderBy(x => x.PlateNumber)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetFleetVehiclesResult(new PaginatedResult<FleetVehicleDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            vehicles.Select(FleetFeatureHelpers.ToDto).ToList()));
    }
}

public class GetFleetVehicleByIdHandler(FleetDbContext dbContext, MaintenanceDbContext maintenanceDbContext, ISender sender)
    : IQueryHandler<GetFleetVehicleByIdQuery, GetFleetVehicleByIdResult>
{
    public async Task<GetFleetVehicleByIdResult> Handle(GetFleetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Id);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, vehicle.BranchId))
            throw new ForbiddenException("You do not have permission to view this fleet vehicle branch scope.");

        var assignments = await dbContext.VehicleAssignments.Include(x => x.Vehicle).AsNoTracking().Where(x => x.VehicleId == request.Id).OrderByDescending(x => x.StartDate).ToListAsync(cancellationToken);
        var documents = await dbContext.VehicleDocuments.Include(x => x.Vehicle).AsNoTracking().Where(x => x.VehicleId == request.Id).OrderBy(x => x.ExpiryDate).ToListAsync(cancellationToken);
        var expenses = await dbContext.VehicleExpenses.Include(x => x.Vehicle).AsNoTracking().Where(x => x.VehicleId == request.Id).OrderByDescending(x => x.ExpenseDate).Take(100).ToListAsync(cancellationToken);
        var rules = await dbContext.VehicleServiceRules.Include(x => x.Vehicle).AsNoTracking().Where(x => x.VehicleId == request.Id).OrderBy(x => x.ServiceType).ToListAsync(cancellationToken);

        var workOrders = vehicle.MaintenanceAssetId.HasValue
            ? await maintenanceDbContext.MaintenanceWorkOrders
                .Include(x => x.Asset)
                .Include(x => x.Comments)
                .Include(x => x.Attachments)
                .Include(x => x.History)
                .AsNoTracking()
                .Where(x => x.AssetId == vehicle.MaintenanceAssetId.Value)
                .OrderByDescending(x => x.RequestedDate)
                .ToListAsync(cancellationToken)
            : [];

        return new GetFleetVehicleByIdResult(new FleetVehicleDetailsDto
        {
            Vehicle = FleetFeatureHelpers.ToDto(vehicle),
            Assignments = assignments.Select(FleetFeatureHelpers.ToDto).ToList(),
            Documents = documents.Select(FleetFeatureHelpers.ToDto).ToList(),
            Expenses = expenses.Select(FleetFeatureHelpers.ToDto).ToList(),
            ServiceRules = rules.Select(FleetFeatureHelpers.ToDto).ToList(),
            MaintenanceWorkOrders = workOrders.Select(ToMaintenanceDto).ToList()
        });
    }

    private static SharedWithUI.Maintenance.Dtos.MaintenanceWorkOrderDto ToMaintenanceDto(MaintenanceWorkOrder workOrder)
    {
        return new SharedWithUI.Maintenance.Dtos.MaintenanceWorkOrderDto
        {
            Id = workOrder.Id,
            WorkOrderNumber = workOrder.WorkOrderNumber,
            Title = workOrder.Title,
            Description = workOrder.Description,
            AssetId = workOrder.AssetId,
            AssetName = workOrder.Asset?.Name ?? string.Empty,
            AssetType = workOrder.Asset?.AssetType ?? MaintenanceAssetType.Vehicle,
            RequestedByUserId = workOrder.RequestedByUserId,
            AssignedToUserId = workOrder.AssignedToUserId,
            Priority = workOrder.Priority,
            Status = workOrder.Status,
            RequestedDate = workOrder.RequestedDate,
            DueDate = workOrder.DueDate,
            StartedAt = workOrder.StartedAt,
            CompletedAt = workOrder.CompletedAt,
            Category = workOrder.Category,
            InternalNotes = workOrder.InternalNotes,
            EstimatedCost = workOrder.EstimatedCost,
            ApprovedCost = workOrder.ApprovedCost,
            ActualCost = workOrder.ActualCost,
            CurrencyId = workOrder.CurrencyId,
            CurrencyCode = workOrder.CurrencyCode,
            VendorName = workOrder.VendorName,
            SupplierId = workOrder.SupplierId,
            CostApprovalStatus = workOrder.CostApprovalStatus,
            BranchId = workOrder.Asset?.BranchId
        };
    }
}

public class CreateEmergencyFleetMaintenanceHandler(
    FleetDbContext dbContext,
    MaintenanceDbContext maintenanceDbContext,
    IMaintenanceNumberGenerator numberGenerator,
    IHttpContextAccessor httpContextAccessor,
    ISender sender)
    : ICommandHandler<CreateEmergencyFleetMaintenanceCommand, CreateEmergencyFleetMaintenanceResult>
{
    public async Task<CreateEmergencyFleetMaintenanceResult> Handle(CreateEmergencyFleetMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.WorkOrder.VehicleId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.WorkOrder.VehicleId);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(vehicle.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, vehicle.BranchId))
            throw new ForbiddenException("You do not have permission to create maintenance for this fleet vehicle branch scope.");

        if (!vehicle.MaintenanceAssetId.HasValue)
            throw new BadRequestException("Vehicle is not linked to a maintenance asset.");

        var number = await numberGenerator.GenerateWorkOrderNumberAsync(cancellationToken);
        var workOrder = MaintenanceWorkOrder.Create(
            number,
            request.WorkOrder.Title,
            request.WorkOrder.Description,
            vehicle.MaintenanceAssetId.Value,
            currentUserId,
            request.WorkOrder.Priority,
            null,
            "Fleet Emergency",
            $"Vehicle {vehicle.PlateNumber}",
            request.WorkOrder.EstimatedCost,
            null,
            request.WorkOrder.CurrencyId,
            request.WorkOrder.CurrencyCode,
            request.WorkOrder.VendorName,
            request.WorkOrder.SupplierId);

        workOrder.AddHistory(MaintenanceHistory.Create(workOrder.Id, "CreatedFromFleet", "Emergency fleet maintenance request created.", currentUserId));
        maintenanceDbContext.MaintenanceWorkOrders.Add(workOrder);
        await FleetMaintenanceAssetSync.MarkUnderMaintenanceAsync(maintenanceDbContext, vehicle.MaintenanceAssetId.Value, currentUserId, cancellationToken);
        vehicle.SetStatus(FleetVehicleStatus.UnderMaintenance, currentUserId);

        await maintenanceDbContext.SaveChangesAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEmergencyFleetMaintenanceResult(workOrder.Id);
    }
}

internal static class FleetMaintenanceAssetSync
{
    public static async Task MarkUnderMaintenanceAsync(MaintenanceDbContext maintenanceDbContext, Guid maintenanceAssetId, Guid currentUserId, CancellationToken cancellationToken)
    {
        var asset = await maintenanceDbContext.MaintenanceAssets.FirstOrDefaultAsync(x => x.Id == maintenanceAssetId, cancellationToken)
            ?? throw new NotFoundException("Maintenance asset", maintenanceAssetId);

        asset.Update(
            asset.AssetCode,
            asset.Name,
            asset.NameEng,
            asset.AssetType,
            MaintenanceAssetStatus.UnderMaintenance,
            asset.CompanyId,
            asset.BranchId,
            asset.ParentAssetId,
            asset.SourceModule,
            asset.SourceEntityName,
            asset.SourceEntityId,
            asset.Description,
            asset.Location,
            asset.SerialNumber,
            asset.PurchaseDate,
            asset.WarrantyEndDate,
            currentUserId);
    }
}
