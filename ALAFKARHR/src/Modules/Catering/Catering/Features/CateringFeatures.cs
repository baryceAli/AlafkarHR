using Catalog.Contracts.Products.Features.GetProductSkuSelectionContext;

namespace Catering.Features;

public record EntityResult(Guid Id);
public record UpdateResult(bool IsSuccess);

public record GetMealsQuery(Guid? CompanyId, string? SearchText, bool ActiveOnly, PaginationRequest Pagination) : IQuery<GetMealsResult>;
public record GetMealsResult(PaginatedResult<MealDefinitionDto> Meals);
public record GetMealsResponse(PaginatedResult<MealDefinitionDto> Meals);
public record GetMealByIdQuery(Guid Id) : IQuery<GetMealByIdResult>;
public record GetMealByIdResult(MealDefinitionDto Meal);
public record SaveMealRequest(MealDefinitionDto Meal);
public record CreateMealCommand(MealDefinitionDto Meal) : ICommand<EntityResult>;
public record UpdateMealCommand(Guid Id, MealDefinitionDto Meal) : ICommand<UpdateResult>;
public record DeleteMealCommand(Guid Id) : ICommand<UpdateResult>;
public record SaveMealComponentRequest(MealComponentDto Component);
public record AddMealComponentCommand(Guid MealId, MealComponentDto Component) : ICommand<EntityResult>;
public record DeleteMealComponentCommand(Guid MealId, Guid ComponentId) : ICommand<UpdateResult>;

public record GetContractsQuery(Guid? CompanyId, Guid? CustomerId, CateringContractStatus? Status, string? SearchText, PaginationRequest Pagination) : IQuery<GetContractsResult>;
public record GetContractsResult(PaginatedResult<CateringContractDto> Contracts);
public record GetContractsResponse(PaginatedResult<CateringContractDto> Contracts);
public record GetContractByIdQuery(Guid Id) : IQuery<GetContractByIdResult>;
public record GetContractByIdResult(CateringContractDto Contract);
public record SaveContractRequest(CateringContractDto Contract);
public record CreateContractCommand(CateringContractDto Contract) : ICommand<EntityResult>;
public record UpdateContractCommand(Guid Id, CateringContractDto Contract) : ICommand<UpdateResult>;
public record DeleteContractCommand(Guid Id) : ICommand<UpdateResult>;
public record CloseContractCommand(Guid Id) : ICommand<UpdateResult>;
public record SaveAddendumRequest(CateringContractAddendumDto Addendum);
public record AddContractAddendumCommand(Guid ContractId, CateringContractAddendumDto Addendum) : ICommand<EntityResult>;

public record GetAreasQuery(Guid? CompanyId, string? SearchText, bool ActiveOnly, PaginationRequest Pagination) : IQuery<GetAreasResult>;
public record GetAreasResult(PaginatedResult<CateringAreaDto> Areas);
public record GetAreasResponse(PaginatedResult<CateringAreaDto> Areas);
public record SaveAreaRequest(CateringAreaDto Area);
public record CreateAreaCommand(CateringAreaDto Area) : ICommand<EntityResult>;
public record UpdateAreaCommand(Guid Id, CateringAreaDto Area) : ICommand<UpdateResult>;
public record GetSquaresQuery(Guid? CompanyId, Guid? AreaId, string? SearchText, bool ActiveOnly, PaginationRequest Pagination) : IQuery<GetSquaresResult>;
public record GetSquaresResult(PaginatedResult<CateringSquareDto> Squares);
public record GetSquaresResponse(PaginatedResult<CateringSquareDto> Squares);
public record SaveSquareRequest(CateringSquareDto Square);
public record CreateSquareCommand(CateringSquareDto Square) : ICommand<EntityResult>;
public record UpdateSquareCommand(Guid Id, CateringSquareDto Square) : ICommand<UpdateResult>;

public record GetSchedulesQuery(Guid? ContractId, DateTime? FromDate, DateTime? ToDate, PaginationRequest Pagination) : IQuery<GetSchedulesResult>;
public record GetSchedulesResult(PaginatedResult<CateringDailyScheduleDto> Schedules);
public record GetSchedulesResponse(PaginatedResult<CateringDailyScheduleDto> Schedules);
public record SaveScheduleRequest(CateringDailyScheduleDto Schedule);
public record CreateScheduleCommand(CateringDailyScheduleDto Schedule) : ICommand<EntityResult>;
public record UpdateScheduleCommand(Guid Id, CateringDailyScheduleDto Schedule) : ICommand<UpdateResult>;
public record SaveAllocationRequest(CateringSquareAllocationDto Allocation);
public record CreateAllocationCommand(Guid ScheduleId, CateringSquareAllocationDto Allocation) : ICommand<EntityResult>;
public record RecordAllocationActualsRequest(decimal ReceivedQuantity, decimal DistributedQuantity, string? VarianceNotes);
public record RecordAllocationActualsCommand(Guid AllocationId, decimal ReceivedQuantity, decimal DistributedQuantity, string? VarianceNotes) : ICommand<UpdateResult>;

public record GetDeliveriesQuery(Guid? ScheduleId, DateTime? FromDate, DateTime? ToDate, PaginationRequest Pagination) : IQuery<GetDeliveriesResult>;
public record GetDeliveriesResult(PaginatedResult<CateringVehicleDeliveryDto> Deliveries);
public record GetDeliveriesResponse(PaginatedResult<CateringVehicleDeliveryDto> Deliveries);
public record SaveDeliveryRequest(CateringVehicleDeliveryDto Delivery);
public record CreateDeliveryCommand(CateringVehicleDeliveryDto Delivery) : ICommand<EntityResult>;

public record GetAssignmentsQuery(Guid? ContractId, CateringAssignmentRole? Role, Guid? SquareId, PaginationRequest Pagination) : IQuery<GetAssignmentsResult>;
public record GetAssignmentsResult(PaginatedResult<CateringAssignmentDto> Assignments);
public record GetAssignmentsResponse(PaginatedResult<CateringAssignmentDto> Assignments);
public record SaveAssignmentRequest(CateringAssignmentDto Assignment);
public record CreateAssignmentCommand(CateringAssignmentDto Assignment) : ICommand<EntityResult>;
public record UpdateAssignmentCommand(Guid Id, CateringAssignmentDto Assignment) : ICommand<UpdateResult>;
public record DeleteAssignmentCommand(Guid Id) : ICommand<UpdateResult>;

public record GetCateringDashboardQuery(Guid? CompanyId) : IQuery<GetCateringDashboardResult>;
public record GetCateringDashboardResult(CateringDashboardDto Dashboard);
public record GetCateringDashboardResponse(CateringDashboardDto Dashboard);
public record GetCateringReportQuery(Guid? CompanyId, Guid? ContractId, Guid? CustomerId, Guid? SquareId, Guid? VehicleId, DateTime? FromDate, DateTime? ToDate) : IQuery<GetCateringReportResult>;
public record GetCateringReportResult(List<CateringReportRowDto> Report);
public record GetCateringReportResponse(List<CateringReportRowDto> Report);

public class CateringEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/catering")
            .RequireBusinessLine(BusinessLineKeys.Catering);

        group.MapGet("/dashboard", async ([AsParameters] CompanyQuery query, ISender sender) => Results.Ok((await sender.Send(new GetCateringDashboardQuery(query.CompanyId))).Adapt<GetCateringDashboardResponse>()))
            .RequireAuthorization(PermissionList.CateringReportsPermissions.View);

        group.MapGet("/meals", async ([AsParameters] CompanySearchQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetMealsQuery(query.CompanyId, pagination.SearchText, query.ActiveOnly, pagination))).Adapt<GetMealsResponse>()))
            .RequireAuthorization(PermissionList.CateringMealPermissions.View);
        group.MapGet("/meals/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new GetMealByIdQuery(id))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.View);
        group.MapPost("/meals", async (SaveMealRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateMealCommand(request.Meal))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.Create);
        group.MapPut("/meals/{id:guid}", async (Guid id, SaveMealRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateMealCommand(id, request.Meal))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.Edit);
        group.MapDelete("/meals/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteMealCommand(id))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.Delete);
        group.MapPost("/meals/{id:guid}/components", async (Guid id, SaveMealComponentRequest request, ISender sender) => Results.Ok(await sender.Send(new AddMealComponentCommand(id, request.Component))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.Edit);
        group.MapDelete("/meals/{mealId:guid}/components/{componentId:guid}", async (Guid mealId, Guid componentId, ISender sender) => Results.Ok(await sender.Send(new DeleteMealComponentCommand(mealId, componentId))))
            .RequireAuthorization(PermissionList.CateringMealPermissions.Edit);

        group.MapGet("/contracts", async ([AsParameters] ContractListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetContractsQuery(query.CompanyId, query.CustomerId, query.Status, pagination.SearchText, pagination))).Adapt<GetContractsResponse>()))
            .RequireAuthorization(PermissionList.CateringContractPermissions.View);
        group.MapGet("/contracts/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new GetContractByIdQuery(id))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.View);
        group.MapPost("/contracts", async (SaveContractRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateContractCommand(request.Contract))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.Create);
        group.MapPut("/contracts/{id:guid}", async (Guid id, SaveContractRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateContractCommand(id, request.Contract))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.Edit);
        group.MapDelete("/contracts/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteContractCommand(id))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.Delete);
        group.MapPut("/contracts/{id:guid}/close", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new CloseContractCommand(id))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.Close);
        group.MapPost("/contracts/{id:guid}/addendums", async (Guid id, SaveAddendumRequest request, ISender sender) => Results.Ok(await sender.Send(new AddContractAddendumCommand(id, request.Addendum))))
            .RequireAuthorization(PermissionList.CateringContractPermissions.Addendum);

        group.MapGet("/areas", async ([AsParameters] CompanySearchQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetAreasQuery(query.CompanyId, pagination.SearchText, query.ActiveOnly, pagination))).Adapt<GetAreasResponse>()))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.View);
        group.MapPost("/areas", async (SaveAreaRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateAreaCommand(request.Area))))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.Create);
        group.MapPut("/areas/{id:guid}", async (Guid id, SaveAreaRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateAreaCommand(id, request.Area))))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.Edit);
        group.MapGet("/squares", async ([AsParameters] SquareListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetSquaresQuery(query.CompanyId, query.AreaId, pagination.SearchText, query.ActiveOnly, pagination))).Adapt<GetSquaresResponse>()))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.View);
        group.MapPost("/squares", async (SaveSquareRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateSquareCommand(request.Square))))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.Create);
        group.MapPut("/squares/{id:guid}", async (Guid id, SaveSquareRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateSquareCommand(id, request.Square))))
            .RequireAuthorization(PermissionList.CateringLocationPermissions.Edit);

        group.MapGet("/schedules", async ([AsParameters] ScheduleListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetSchedulesQuery(query.ContractId, query.FromDate, query.ToDate, pagination))).Adapt<GetSchedulesResponse>()))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.View);
        group.MapPost("/schedules", async (SaveScheduleRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateScheduleCommand(request.Schedule))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Create);
        group.MapPut("/schedules/{id:guid}", async (Guid id, SaveScheduleRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateScheduleCommand(id, request.Schedule))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Edit);
        group.MapPost("/schedules/{id:guid}/allocations", async (Guid id, SaveAllocationRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateAllocationCommand(id, request.Allocation))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Edit);
        group.MapPut("/allocations/{id:guid}/actuals", async (Guid id, RecordAllocationActualsRequest request, ISender sender) => Results.Ok(await sender.Send(new RecordAllocationActualsCommand(id, request.ReceivedQuantity, request.DistributedQuantity, request.VarianceNotes))))
            .RequireAuthorization(PermissionList.CateringDistributionPermissions.Edit);

        group.MapGet("/deliveries", async ([AsParameters] DeliveryListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetDeliveriesQuery(query.ScheduleId, query.FromDate, query.ToDate, pagination))).Adapt<GetDeliveriesResponse>()))
            .RequireAuthorization(PermissionList.CateringDeliveryPermissions.View);
        group.MapPost("/deliveries", async (SaveDeliveryRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateDeliveryCommand(request.Delivery))))
            .RequireAuthorization(PermissionList.CateringDeliveryPermissions.Create);

        group.MapGet("/assignments", async ([AsParameters] AssignmentListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetAssignmentsQuery(query.ContractId, query.Role, query.SquareId, pagination))).Adapt<GetAssignmentsResponse>()))
            .RequireAuthorization(PermissionList.CateringAssignmentPermissions.View);
        group.MapPost("/assignments", async (SaveAssignmentRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateAssignmentCommand(request.Assignment))))
            .RequireAuthorization(PermissionList.CateringAssignmentPermissions.Create);
        group.MapPut("/assignments/{id:guid}", async (Guid id, SaveAssignmentRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateAssignmentCommand(id, request.Assignment))))
            .RequireAuthorization(PermissionList.CateringAssignmentPermissions.Edit);
        group.MapDelete("/assignments/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteAssignmentCommand(id))))
            .RequireAuthorization(PermissionList.CateringAssignmentPermissions.Delete);

        group.MapGet("/reports/operations", async ([AsParameters] ReportQuery query, ISender sender) =>
            Results.Ok((await sender.Send(new GetCateringReportQuery(query.CompanyId, query.ContractId, query.CustomerId, query.SquareId, query.VehicleId, query.FromDate, query.ToDate))).Adapt<GetCateringReportResponse>()))
            .RequireAuthorization(PermissionList.CateringReportsPermissions.View);
    }
}

public class CateringHandlers(CateringDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender) :
    IQueryHandler<GetMealsQuery, GetMealsResult>, IQueryHandler<GetMealByIdQuery, GetMealByIdResult>, ICommandHandler<CreateMealCommand, EntityResult>, ICommandHandler<UpdateMealCommand, UpdateResult>, ICommandHandler<DeleteMealCommand, UpdateResult>, ICommandHandler<AddMealComponentCommand, EntityResult>, ICommandHandler<DeleteMealComponentCommand, UpdateResult>,
    IQueryHandler<GetContractsQuery, GetContractsResult>, IQueryHandler<GetContractByIdQuery, GetContractByIdResult>, ICommandHandler<CreateContractCommand, EntityResult>, ICommandHandler<UpdateContractCommand, UpdateResult>, ICommandHandler<DeleteContractCommand, UpdateResult>, ICommandHandler<CloseContractCommand, UpdateResult>, ICommandHandler<AddContractAddendumCommand, EntityResult>,
    IQueryHandler<GetAreasQuery, GetAreasResult>, ICommandHandler<CreateAreaCommand, EntityResult>, ICommandHandler<UpdateAreaCommand, UpdateResult>, IQueryHandler<GetSquaresQuery, GetSquaresResult>, ICommandHandler<CreateSquareCommand, EntityResult>, ICommandHandler<UpdateSquareCommand, UpdateResult>,
    IQueryHandler<GetSchedulesQuery, GetSchedulesResult>, ICommandHandler<CreateScheduleCommand, EntityResult>, ICommandHandler<UpdateScheduleCommand, UpdateResult>, ICommandHandler<CreateAllocationCommand, EntityResult>, ICommandHandler<RecordAllocationActualsCommand, UpdateResult>,
    IQueryHandler<GetDeliveriesQuery, GetDeliveriesResult>, ICommandHandler<CreateDeliveryCommand, EntityResult>,
    IQueryHandler<GetAssignmentsQuery, GetAssignmentsResult>, ICommandHandler<CreateAssignmentCommand, EntityResult>, ICommandHandler<UpdateAssignmentCommand, UpdateResult>, ICommandHandler<DeleteAssignmentCommand, UpdateResult>,
    IQueryHandler<GetCateringDashboardQuery, GetCateringDashboardResult>, IQueryHandler<GetCateringReportQuery, GetCateringReportResult>
{
    public async Task<GetMealsResult> Handle(GetMealsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.MealDefinitions.Include(x => x.Components).AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ActiveOnly) query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || (x.NameEng != null && x.NameEng.Contains(search)));
        }
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.Name).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        return new GetMealsResult(new PaginatedResult<MealDefinitionDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<GetMealByIdResult> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
    {
        var meal = await dbContext.MealDefinitions.Include(x => x.Components).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {request.Id}");
        return new GetMealByIdResult(ToDto(meal));
    }

    public async Task<EntityResult> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        var meal = MealDefinition.Create(request.Meal, UserId());
        await dbContext.MealDefinitions.AddAsync(meal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        foreach (var component in request.Meal.Components)
        {
            await dbContext.MealComponents.AddAsync(MealComponent.Create(meal.Id, component, UserId()), cancellationToken);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(meal.Id);
    }

    public async Task<UpdateResult> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        var meal = await dbContext.MealDefinitions.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {request.Id}");
        meal.Update(request.Meal, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(DeleteMealCommand request, CancellationToken cancellationToken)
    {
        var meal = await dbContext.MealDefinitions.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {request.Id}");
        meal.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(AddMealComponentCommand request, CancellationToken cancellationToken)
    {
        var meal = await GetMealAsync(request.MealId, cancellationToken);
        var sku = await sender.Send(new GetProductSkuSelectionContextQuery(request.Component.ProductSkuId, meal.CompanyId), cancellationToken);
        if (string.IsNullOrWhiteSpace(sku.UnitName))
            throw new BadRequestException("Selected SKU must have a configured unit.");
        if (!sku.Calories.HasValue)
            throw new BadRequestException("Selected SKU must have a configured numeric calorie value.");

        var componentDto = new MealComponentDto
        {
            ProductId = sku.ProductId,
            ProductSkuId = sku.ProductSkuId,
            ProductPackageId = sku.ProductPackageId,
            ComponentName = $"{sku.SkuCode} - {sku.Name}",
            ComponentNameEng = string.IsNullOrWhiteSpace(sku.NameEng)
                ? $"{sku.SkuCode} - {sku.Name}"
                : $"{sku.SkuCodeEng} - {sku.NameEng}",
            QuantityPerMeal = request.Component.QuantityPerMeal,
            UnitName = sku.UnitName,
            CaloriesPerUnit = sku.Calories,
            Notes = request.Component.Notes
        };

        var component = MealComponent.Create(request.MealId, componentDto, UserId());
        await dbContext.MealComponents.AddAsync(component, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await RecalculateMealCaloriesAsync(request.MealId, cancellationToken);
        return new EntityResult(component.Id);
    }

    public async Task<UpdateResult> Handle(DeleteMealComponentCommand request, CancellationToken cancellationToken)
    {
        var component = await dbContext.MealComponents.FirstOrDefaultAsync(x => x.Id == request.ComponentId && x.MealDefinitionId == request.MealId, cancellationToken)
            ?? throw new NotFoundException($"Meal component not found: {request.ComponentId}");
        component.IsDeleted = true;
        component.DeletedAt = DateTime.UtcNow;
        component.DeletedBy = UserId();
        await dbContext.SaveChangesAsync(cancellationToken);
        await RecalculateMealCaloriesAsync(request.MealId, cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetContractsResult> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringContracts.Include(x => x.Addendums).AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.CustomerId.HasValue) query = query.Where(x => x.CustomerId == request.CustomerId);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Number.Contains(search) || x.CustomerName.Contains(search) || (x.CustomerNameEng != null && x.CustomerNameEng.Contains(search)) || x.SeasonLabel.Contains(search));
        }
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.StartDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var meals = await MealNamesAsync(data.Select(x => x.MealDefinitionId), cancellationToken);
        return new GetContractsResult(new PaginatedResult<CateringContractDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, meals))));
    }

    public async Task<GetContractByIdResult> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.CateringContracts.Include(x => x.Addendums).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {request.Id}");
        var meals = await MealNamesAsync([contract.MealDefinitionId], cancellationToken);
        return new GetContractByIdResult(ToDto(contract, meals));
    }

    public async Task<EntityResult> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        await EnsureMealAsync(request.Contract.MealDefinitionId, cancellationToken);
        await ValidateContractMealCaloriesAsync(request.Contract, cancellationToken);
        var contract = CateringContract.Create(await NextContractNumberAsync(cancellationToken), request.Contract, UserId());
        await dbContext.CateringContracts.AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(contract.Id);
    }

    public async Task<UpdateResult> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.CateringContracts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {request.Id}");
        await EnsureMealAsync(request.Contract.MealDefinitionId, cancellationToken);
        await ValidateContractMealCaloriesAsync(request.Contract, cancellationToken);
        contract.Update(request.Contract, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.CateringContracts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {request.Id}");
        contract.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(CloseContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.CateringContracts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {request.Id}");
        contract.Close(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(AddContractAddendumCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringContracts.AnyAsync(x => x.Id == request.ContractId, cancellationToken)) throw new NotFoundException($"Catering contract not found: {request.ContractId}");
        var addendum = CateringContractAddendum.Create(request.ContractId, request.Addendum, UserId());
        await dbContext.CateringContractAddendums.AddAsync(addendum, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(addendum.Id);
    }

    public async Task<GetAreasResult> Handle(GetAreasQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringAreas.AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ActiveOnly) query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || (x.NameEng != null && x.NameEng.Contains(search)) || (x.LocationText != null && x.LocationText.Contains(search)));
        }
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.Name).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        return new GetAreasResult(new PaginatedResult<CateringAreaDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<EntityResult> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var area = CateringArea.Create(request.Area, UserId());
        await dbContext.CateringAreas.AddAsync(area, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(area.Id);
    }

    public async Task<UpdateResult> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await dbContext.CateringAreas.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering area not found: {request.Id}");
        area.Update(request.Area, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetSquaresResult> Handle(GetSquaresQuery request, CancellationToken cancellationToken)
    {
        var query = from square in dbContext.CateringSquares.AsNoTracking()
                    join area in dbContext.CateringAreas.AsNoTracking() on square.AreaId equals area.Id
                    select new { square, area };
        if (request.CompanyId.HasValue) query = query.Where(x => x.area.CompanyId == request.CompanyId);
        if (request.AreaId.HasValue) query = query.Where(x => x.square.AreaId == request.AreaId);
        if (request.ActiveOnly) query = query.Where(x => x.square.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.square.Code.Contains(search) || x.square.Name.Contains(search) || (x.square.NameEng != null && x.square.NameEng.Contains(search)));
        }
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.area.Name).ThenBy(x => x.square.Code).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        return new GetSquaresResult(new PaginatedResult<CateringSquareDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x.square, x.area))));
    }

    public async Task<EntityResult> Handle(CreateSquareCommand request, CancellationToken cancellationToken)
    {
        await EnsureAreaAsync(request.Square.AreaId, cancellationToken);
        var square = CateringSquare.Create(request.Square, UserId());
        await dbContext.CateringSquares.AddAsync(square, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(square.Id);
    }

    public async Task<UpdateResult> Handle(UpdateSquareCommand request, CancellationToken cancellationToken)
    {
        var square = await dbContext.CateringSquares.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering square not found: {request.Id}");
        await EnsureAreaAsync(request.Square.AreaId, cancellationToken);
        square.Update(request.Square, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetSchedulesResult> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringDailySchedules.Include(x => x.Allocations).AsNoTracking();
        if (request.ContractId.HasValue) query = query.Where(x => x.CateringContractId == request.ContractId);
        if (request.FromDate.HasValue) query = query.Where(x => x.ServiceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.ServiceDate <= request.ToDate.Value.Date);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.ServiceDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var squares = await SquareNamesAsync(data.SelectMany(x => x.Allocations).Select(x => x.SquareId), cancellationToken);
        return new GetSchedulesResult(new PaginatedResult<CateringDailyScheduleDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, squares))));
    }

    public async Task<EntityResult> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        await EnsureContractAsync(request.Schedule.CateringContractId, cancellationToken);
        var schedule = CateringDailySchedule.Create(request.Schedule, UserId());
        await dbContext.CateringDailySchedules.AddAsync(schedule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(schedule.Id);
    }

    public async Task<UpdateResult> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.Id}");
        await EnsureContractAsync(request.Schedule.CateringContractId, cancellationToken);
        schedule.Update(request.Schedule, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(CreateAllocationCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringDailySchedules.AnyAsync(x => x.Id == request.ScheduleId, cancellationToken)) throw new NotFoundException($"Catering schedule not found: {request.ScheduleId}");
        await EnsureSquareAsync(request.Allocation.SquareId, cancellationToken);
        var dto = request.Allocation;
        dto.DailyScheduleId = request.ScheduleId;
        var allocation = CateringSquareAllocation.Create(dto, UserId());
        await dbContext.CateringSquareAllocations.AddAsync(allocation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(allocation.Id);
    }

    public async Task<UpdateResult> Handle(RecordAllocationActualsCommand request, CancellationToken cancellationToken)
    {
        var allocation = await dbContext.CateringSquareAllocations.FirstOrDefaultAsync(x => x.Id == request.AllocationId, cancellationToken)
            ?? throw new NotFoundException($"Catering allocation not found: {request.AllocationId}");
        allocation.RecordActuals(request.ReceivedQuantity, request.DistributedQuantity, request.VarianceNotes, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetDeliveriesResult> Handle(GetDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringVehicleDeliveries.AsNoTracking();
        if (request.ScheduleId.HasValue) query = query.Where(x => x.DailyScheduleId == request.ScheduleId);
        if (request.FromDate.HasValue) query = query.Where(x => x.ArrivalTime.Date >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.ArrivalTime.Date <= request.ToDate.Value.Date);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.ArrivalTime).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        return new GetDeliveriesResult(new PaginatedResult<CateringVehicleDeliveryDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<EntityResult> Handle(CreateDeliveryCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringDailySchedules.AnyAsync(x => x.Id == request.Delivery.DailyScheduleId, cancellationToken)) throw new NotFoundException($"Catering schedule not found: {request.Delivery.DailyScheduleId}");
        var delivery = CateringVehicleDelivery.Create(request.Delivery, UserId());
        await dbContext.CateringVehicleDeliveries.AddAsync(delivery, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(delivery.Id);
    }

    public async Task<GetAssignmentsResult> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringAssignments.AsNoTracking();
        if (request.ContractId.HasValue) query = query.Where(x => x.CateringContractId == request.ContractId);
        if (request.Role.HasValue) query = query.Where(x => x.Role == request.Role);
        if (request.SquareId.HasValue) query = query.Where(x => x.SquareId == request.SquareId || x.CoveredSquareIdsCsv.Contains(request.SquareId.Value.ToString()));
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.Role).ThenBy(x => x.EmployeeName).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var squares = await SquareNamesAsync(data.Where(x => x.SquareId.HasValue).Select(x => x.SquareId!.Value), cancellationToken);
        return new GetAssignmentsResult(new PaginatedResult<CateringAssignmentDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, squares))));
    }

    public async Task<EntityResult> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        await EnsureContractAsync(request.Assignment.CateringContractId, cancellationToken);
        if (request.Assignment.SquareId.HasValue) await EnsureSquareAsync(request.Assignment.SquareId.Value, cancellationToken);
        var assignment = CateringAssignment.Create(request.Assignment, UserId());
        await dbContext.CateringAssignments.AddAsync(assignment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(assignment.Id);
    }

    public async Task<UpdateResult> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.CateringAssignments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering assignment not found: {request.Id}");
        assignment.Update(request.Assignment, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.CateringAssignments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering assignment not found: {request.Id}");
        assignment.IsDeleted = true;
        assignment.DeletedAt = DateTime.UtcNow;
        assignment.DeletedBy = UserId();
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetCateringDashboardResult> Handle(GetCateringDashboardQuery request, CancellationToken cancellationToken)
    {
        var contracts = dbContext.CateringContracts.AsNoTracking();
        var meals = dbContext.MealDefinitions.AsNoTracking();
        var areas = dbContext.CateringAreas.AsNoTracking();
        var schedules = dbContext.CateringDailySchedules.AsNoTracking();
        if (request.CompanyId.HasValue)
        {
            contracts = contracts.Where(x => x.CompanyId == request.CompanyId);
            meals = meals.Where(x => x.CompanyId == request.CompanyId);
            areas = areas.Where(x => x.CompanyId == request.CompanyId);
            var contractIds = await contracts.Select(x => x.Id).ToListAsync(cancellationToken);
            schedules = schedules.Where(x => contractIds.Contains(x.CateringContractId));
        }
        var scheduleIds = await schedules.Select(x => x.Id).ToListAsync(cancellationToken);
        var allocations = dbContext.CateringSquareAllocations.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId));
        var contractList = await contracts.Include(x => x.Addendums).ToListAsync(cancellationToken);
        var dashboard = new CateringDashboardDto
        {
            ActiveContracts = contractList.Count(x => x.Status == CateringContractStatus.Active || x.Status == CateringContractStatus.Draft),
            ActiveMeals = await meals.CountAsync(x => x.IsActive, cancellationToken),
            ActiveSquares = await (from square in dbContext.CateringSquares.AsNoTracking()
                                   join area in areas on square.AreaId equals area.Id
                                   where square.IsActive
                                   select square.Id).CountAsync(cancellationToken),
            ContractedQuantity = contractList.Sum(x => x.ContractedMealQuantity + x.Addendums.Sum(a => a.AddedQuantity)),
            ScheduledQuantity = await schedules.SumAsync(x => x.PlannedQuantity, cancellationToken),
            ReceivedQuantity = await allocations.SumAsync(x => x.ReceivedQuantity, cancellationToken),
            DistributedQuantity = await allocations.SumAsync(x => x.DistributedQuantity, cancellationToken)
        };
        return new GetCateringDashboardResult(dashboard);
    }

    public async Task<GetCateringReportResult> Handle(GetCateringReportQuery request, CancellationToken cancellationToken)
    {
        var query = from schedule in dbContext.CateringDailySchedules.AsNoTracking()
                    join contract in dbContext.CateringContracts.AsNoTracking() on schedule.CateringContractId equals contract.Id
                    join allocation in dbContext.CateringSquareAllocations.AsNoTracking() on schedule.Id equals allocation.DailyScheduleId into allocations
                    from allocation in allocations.DefaultIfEmpty()
                    join square in dbContext.CateringSquares.AsNoTracking() on allocation.SquareId equals square.Id into squares
                    from square in squares.DefaultIfEmpty()
                    select new { schedule, contract, allocation, square };
        if (request.CompanyId.HasValue) query = query.Where(x => x.contract.CompanyId == request.CompanyId);
        if (request.ContractId.HasValue) query = query.Where(x => x.contract.Id == request.ContractId);
        if (request.CustomerId.HasValue) query = query.Where(x => x.contract.CustomerId == request.CustomerId);
        if (request.SquareId.HasValue) query = query.Where(x => x.allocation != null && x.allocation.SquareId == request.SquareId);
        if (request.FromDate.HasValue) query = query.Where(x => x.schedule.ServiceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.schedule.ServiceDate <= request.ToDate.Value.Date);

        var data = await query.ToListAsync(cancellationToken);
        var rows = data.Select(x => new CateringReportRowDto
        {
            PeriodKey = x.schedule.ServiceDate.ToString("yyyy-MM-dd"),
            ServiceDate = x.schedule.ServiceDate,
            ContractId = x.contract.Id,
            ContractNumber = x.contract.Number,
            CustomerId = x.contract.CustomerId,
            CustomerName = x.contract.CustomerName,
            SquareId = x.allocation?.SquareId,
            SquareName = x.square?.Name,
            ContractedQuantity = x.contract.ContractedMealQuantity + x.contract.Addendums.Sum(a => a.AddedQuantity),
            ScheduledQuantity = x.allocation?.PlannedQuantity ?? x.schedule.PlannedQuantity,
            ReceivedQuantity = x.allocation?.ReceivedQuantity ?? 0,
            DistributedQuantity = x.allocation?.DistributedQuantity ?? 0,
            VarianceQuantity = (x.allocation?.DistributedQuantity ?? 0) - (x.allocation?.PlannedQuantity ?? x.schedule.PlannedQuantity)
        }).ToList();

        if (request.VehicleId.HasValue)
        {
            var scheduleIds = rows.Select(x => x.ServiceDate).ToList();
            var deliveries = await dbContext.CateringVehicleDeliveries.AsNoTracking().Where(x => x.VehicleId == request.VehicleId).ToListAsync(cancellationToken);
            rows = rows.Where(row => deliveries.Any(d => scheduleIds.Contains(row.ServiceDate) && row.ServiceDate.HasValue)).ToList();
        }

        return new GetCateringReportResult(rows);
    }

    private async Task EnsureMealAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await dbContext.MealDefinitions.AnyAsync(x => x.Id == id, cancellationToken)) throw new NotFoundException($"Meal not found: {id}");
    }

    private async Task<MealDefinition> GetMealAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.MealDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {id}");
    }

    private async Task RecalculateMealCaloriesAsync(Guid id, CancellationToken cancellationToken)
    {
        var meal = await dbContext.MealDefinitions.Include(x => x.Components).FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {id}");

        meal.RecalculateCalories(meal.Components, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateContractMealCaloriesAsync(CateringContractDto contract, CancellationToken cancellationToken)
    {
        if (!contract.IsMealCaloriesRequired)
            return;

        var mealCalories = await dbContext.MealDefinitions.AsNoTracking()
            .Where(x => x.Id == contract.MealDefinitionId)
            .Select(x => x.Calories)
            .FirstOrDefaultAsync(cancellationToken);

        if (!mealCalories.HasValue)
            throw new BadRequestException("Selected meal must have calculated calories before it can be used for a calorie-required contract.");

        if (contract.MinMealCalories.HasValue && mealCalories.Value < contract.MinMealCalories.Value)
            throw new BadRequestException("Selected meal calories are below the contract minimum.");

        if (contract.MaxMealCalories.HasValue && mealCalories.Value > contract.MaxMealCalories.Value)
            throw new BadRequestException("Selected meal calories are above the contract maximum.");
    }

    private async Task EnsureContractAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringContracts.AnyAsync(x => x.Id == id, cancellationToken)) throw new NotFoundException($"Catering contract not found: {id}");
    }

    private async Task EnsureAreaAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringAreas.AnyAsync(x => x.Id == id, cancellationToken)) throw new NotFoundException($"Catering area not found: {id}");
    }

    private async Task EnsureSquareAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringSquares.AnyAsync(x => x.Id == id, cancellationToken)) throw new NotFoundException($"Catering square not found: {id}");
    }

    private async Task<string> NextContractNumberAsync(CancellationToken cancellationToken)
    {
        var count = await dbContext.CateringContracts.IgnoreQueryFilters().CountAsync(cancellationToken) + 1;
        return $"CAT-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }

    private string UserId() => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

    private async Task<Dictionary<Guid, MealDefinitionDto>> MealNamesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var mealIds = ids.Distinct().ToList();
        return await dbContext.MealDefinitions.AsNoTracking().Where(x => mealIds.Contains(x.Id)).Select(x => ToDto(x)).ToDictionaryAsync(x => x.Id, cancellationToken);
    }

    private async Task<Dictionary<Guid, CateringSquareDto>> SquareNamesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var squareIds = ids.Distinct().ToList();
        return await (from square in dbContext.CateringSquares.AsNoTracking()
                      join area in dbContext.CateringAreas.AsNoTracking() on square.AreaId equals area.Id
                      where squareIds.Contains(square.Id)
                      select new { square, area }).ToDictionaryAsync(x => x.square.Id, x => ToDto(x.square, x.area), cancellationToken);
    }

    private static MealDefinitionDto ToDto(MealDefinition item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        MealType = item.MealType,
        Name = item.Name,
        NameEng = item.NameEng,
        Calories = item.Calories,
        IsActive = item.IsActive,
        Notes = item.Notes,
        Components = item.Components.Select(ToDto).ToList()
    };

    private static MealComponentDto ToDto(MealComponent item) => new()
    {
        Id = item.Id,
        MealDefinitionId = item.MealDefinitionId,
        ProductId = item.ProductId,
        ProductSkuId = item.ProductSkuId,
        ProductPackageId = item.ProductPackageId,
        ComponentName = item.ComponentName,
        ComponentNameEng = item.ComponentNameEng,
        QuantityPerMeal = item.QuantityPerMeal,
        UnitName = item.UnitName,
        CaloriesPerUnit = item.CaloriesPerUnit,
        TotalCalories = item.TotalCalories,
        Notes = item.Notes
    };

    private static CateringContractDto ToDto(CateringContract item, IReadOnlyDictionary<Guid, MealDefinitionDto>? meals = null)
    {
        MealDefinitionDto? meal = null;
        meals?.TryGetValue(item.MealDefinitionId, out meal);
        var addendumQuantity = item.Addendums.Sum(x => x.AddedQuantity);
        return new CateringContractDto
        {
            Id = item.Id,
            Number = item.Number,
            CompanyId = item.CompanyId,
            BranchId = item.BranchId,
            CustomerId = item.CustomerId,
            CustomerName = item.CustomerName,
            CustomerNameEng = item.CustomerNameEng,
            GenericContractId = item.GenericContractId,
            ServiceType = item.ServiceType,
            SeasonLabel = item.SeasonLabel,
            RamadanYear = item.RamadanYear,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            ContractedMealQuantity = item.ContractedMealQuantity,
            AddendumMealQuantity = addendumQuantity,
            TotalContractedMealQuantity = item.ContractedMealQuantity + addendumQuantity,
            MealDefinitionId = item.MealDefinitionId,
            MealName = meal?.Name,
            MealNameEng = meal?.NameEng,
            IsMealCaloriesRequired = item.IsMealCaloriesRequired,
            MinMealCalories = item.MinMealCalories,
            MaxMealCalories = item.MaxMealCalories,
            Status = item.Status,
            Notes = item.Notes,
            Addendums = item.Addendums.Select(ToDto).ToList()
        };
    }

    private static CateringContractAddendumDto ToDto(CateringContractAddendum item) => new()
    {
        Id = item.Id,
        CateringContractId = item.CateringContractId,
        AddedQuantity = item.AddedQuantity,
        EffectiveFrom = item.EffectiveFrom,
        EffectiveTo = item.EffectiveTo,
        Reason = item.Reason,
        AttachmentDocumentId = item.AttachmentDocumentId
    };

    private static CateringAreaDto ToDto(CateringArea item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        Name = item.Name,
        NameEng = item.NameEng,
        GenderGroup = item.GenderGroup,
        LocationText = item.LocationText,
        IsActive = item.IsActive
    };

    private static CateringSquareDto ToDto(CateringSquare square, CateringArea? area = null) => new()
    {
        Id = square.Id,
        AreaId = square.AreaId,
        AreaName = area?.Name ?? string.Empty,
        AreaNameEng = area?.NameEng,
        Code = square.Code,
        Name = square.Name,
        NameEng = square.NameEng,
        LocationText = square.LocationText,
        Latitude = square.Latitude,
        Longitude = square.Longitude,
        IsActive = square.IsActive,
        Notes = square.Notes
    };

    private static CateringDailyScheduleDto ToDto(CateringDailySchedule item, IReadOnlyDictionary<Guid, CateringSquareDto> squares) => new()
    {
        Id = item.Id,
        CateringContractId = item.CateringContractId,
        ServiceDate = item.ServiceDate,
        PlannedQuantity = item.PlannedQuantity,
        ReceivedQuantity = item.Allocations.Sum(x => x.ReceivedQuantity),
        DistributedQuantity = item.Allocations.Sum(x => x.DistributedQuantity),
        Notes = item.Notes,
        Allocations = item.Allocations.Select(x => ToDto(x, squares)).ToList()
    };

    private static CateringSquareAllocationDto ToDto(CateringSquareAllocation item, IReadOnlyDictionary<Guid, CateringSquareDto> squares)
    {
        squares.TryGetValue(item.SquareId, out var square);
        return new CateringSquareAllocationDto
        {
            Id = item.Id,
            DailyScheduleId = item.DailyScheduleId,
            SquareId = item.SquareId,
            SquareName = square?.Name ?? string.Empty,
            SquareNameEng = square?.NameEng,
            PlannedQuantity = item.PlannedQuantity,
            ReceivedQuantity = item.ReceivedQuantity,
            DistributedQuantity = item.DistributedQuantity,
            VarianceQuantity = item.DistributedQuantity - item.PlannedQuantity,
            VarianceNotes = item.VarianceNotes
        };
    }

    private static CateringVehicleDeliveryDto ToDto(CateringVehicleDelivery item) => new()
    {
        Id = item.Id,
        DailyScheduleId = item.DailyScheduleId,
        VehicleId = item.VehicleId,
        VehicleName = item.VehicleName,
        PlateNumber = item.PlateNumber,
        DriverEmployeeId = item.DriverEmployeeId,
        DriverName = item.DriverName,
        ReceivingSupervisorEmployeeId = item.ReceivingSupervisorEmployeeId,
        ReceivingSupervisorName = item.ReceivingSupervisorName,
        ArrivalTime = item.ArrivalTime,
        ReceivedQuantity = item.ReceivedQuantity,
        Notes = item.Notes
    };

    private static CateringAssignmentDto ToDto(CateringAssignment item, IReadOnlyDictionary<Guid, CateringSquareDto> squares)
    {
        CateringSquareDto? square = null;
        if (item.SquareId.HasValue) squares.TryGetValue(item.SquareId.Value, out square);
        return new CateringAssignmentDto
        {
            Id = item.Id,
            CateringContractId = item.CateringContractId,
            Role = item.Role,
            SquareId = item.SquareId,
            SquareName = square?.Name,
            EmployeeId = item.EmployeeId,
            EmployeeName = item.EmployeeName,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Notes = item.Notes,
            CoveredSquareIds = ParseIds(item.CoveredSquareIdsCsv),
            DistributorEmployeeIds = ParseIds(item.DistributorEmployeeIdsCsv)
        };
    }

    private static List<Guid> ParseIds(string csv) => string.IsNullOrWhiteSpace(csv)
        ? []
        : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => Guid.TryParse(x, out var id) ? id : Guid.Empty).Where(x => x != Guid.Empty).ToList();
}

public class CompanyQuery
{
    public Guid? CompanyId { get; set; }
}

public class CompanySearchQuery : CompanyQuery
{
    public bool ActiveOnly { get; set; } = true;
}

public class ContractListQuery : CompanyQuery
{
    public Guid? CustomerId { get; set; }
    public CateringContractStatus? Status { get; set; }
}

public class SquareListQuery : CompanySearchQuery
{
    public Guid? AreaId { get; set; }
}

public class ScheduleListQuery
{
    public Guid? ContractId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class DeliveryListQuery
{
    public Guid? ScheduleId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class AssignmentListQuery
{
    public Guid? ContractId { get; set; }
    public CateringAssignmentRole? Role { get; set; }
    public Guid? SquareId { get; set; }
}

public class ReportQuery : CompanyQuery
{
    public Guid? ContractId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? SquareId { get; set; }
    public Guid? VehicleId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
