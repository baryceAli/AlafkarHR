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

public record GetSchedulesQuery(Guid? ContractId, Guid? ProjectId, Guid? ProjectDailyPlanId, DateTime? FromDate, DateTime? ToDate, PaginationRequest Pagination) : IQuery<GetSchedulesResult>;
public record GetSchedulesResult(PaginatedResult<CateringDailyScheduleDto> Schedules);
public record GetSchedulesResponse(PaginatedResult<CateringDailyScheduleDto> Schedules);
public record SaveScheduleRequest(CateringDailyScheduleDto Schedule);
public record CreateScheduleCommand(CateringDailyScheduleDto Schedule) : ICommand<EntityResult>;
public record UpdateScheduleCommand(Guid Id, CateringDailyScheduleDto Schedule) : ICommand<UpdateResult>;
public record SaveAllocationRequest(CateringSquareAllocationDto Allocation);
public record CreateAllocationCommand(Guid ScheduleId, CateringSquareAllocationDto Allocation) : ICommand<EntityResult>;
public record RecordAllocationActualsRequest(decimal ReceivedQuantity, decimal DistributedQuantity, DateTime? ActualArrivalTime, Guid? ReceivingSupervisorEmployeeId, string? ReceivingSupervisorName, Guid? TeamLeaderEmployeeId, string? TeamLeaderName, string? VarianceNotes);
public record RecordAllocationActualsCommand(Guid AllocationId, decimal ReceivedQuantity, decimal DistributedQuantity, DateTime? ActualArrivalTime, Guid? ReceivingSupervisorEmployeeId, string? ReceivingSupervisorName, Guid? TeamLeaderEmployeeId, string? TeamLeaderName, string? VarianceNotes) : ICommand<UpdateResult>;
public record ActivateScheduleCommand(Guid ScheduleId) : ICommand<UpdateResult>;

public record GetPackagingPlansQuery(Guid? ScheduleId, DateTime? FromDate, DateTime? ToDate, PaginationRequest Pagination) : IQuery<GetPackagingPlansResult>;
public record GetPackagingPlansResult(PaginatedResult<CateringPackagingPlanDto> PackagingPlans);
public record GetPackagingPlansResponse(PaginatedResult<CateringPackagingPlanDto> PackagingPlans);
public record SavePackagingPlanRequest(CateringPackagingPlanDto PackagingPlan);
public record UpsertPackagingPlanCommand(CateringPackagingPlanDto PackagingPlan) : ICommand<EntityResult>;
public record ReleasePackagingStockCommand(Guid PackagingPlanId) : ICommand<UpdateResult>;
public record StartPackagingCommand(Guid PackagingPlanId) : ICommand<UpdateResult>;
public record CompletePackagingRequest(decimal PreparedMealCount, decimal RejectedMealCount, decimal DamagedMealCount, string? VarianceReason);
public record CompletePackagingCommand(Guid PackagingPlanId, decimal PreparedMealCount, decimal RejectedMealCount, decimal DamagedMealCount, string? VarianceReason) : ICommand<UpdateResult>;

public record GetDispatchPlansQuery(Guid? ScheduleId, DateTime? FromDate, DateTime? ToDate, PaginationRequest Pagination) : IQuery<GetDispatchPlansResult>;
public record GetDispatchPlansResult(PaginatedResult<CateringDispatchPlanDto> DispatchPlans);
public record GetDispatchPlansResponse(PaginatedResult<CateringDispatchPlanDto> DispatchPlans);
public record SaveDispatchPlanRequest(CateringDispatchPlanDto DispatchPlan);
public record UpsertDispatchPlanCommand(CateringDispatchPlanDto DispatchPlan) : ICommand<EntityResult>;
public record CreateDispatchFleetAssignmentCommand(Guid DispatchPlanId) : ICommand<UpdateResult>;
public record RecordExecutionEventRequest(CateringExecutionEventDto Event);
public record RecordExecutionEventCommand(CateringExecutionEventDto Event) : ICommand<UpdateResult>;

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

public record GetProjectsQuery(Guid? CompanyId, Guid? ContractId, PaginationRequest Pagination) : IQuery<GetProjectsResult>;
public record GetProjectsResult(PaginatedResult<CateringProjectDto> Projects);
public record GetProjectsResponse(PaginatedResult<CateringProjectDto> Projects);
public record GetProjectByIdQuery(Guid Id) : IQuery<GetProjectByIdResult>;
public record GetProjectByIdResult(CateringProjectDto Project);
public record SaveProjectRequest(CateringProjectDto Project);
public record CreateProjectCommand(CateringProjectDto Project) : ICommand<EntityResult>;
public record UpdateProjectCommand(Guid Id, CateringProjectDto Project) : ICommand<UpdateResult>;
public record DeleteProjectCommand(Guid Id) : ICommand<UpdateResult>;
public record SaveProjectDailyPlanRequest(CateringProjectDailyPlanDto DailyPlan);
public record UpsertProjectDailyPlanCommand(CateringProjectDailyPlanDto DailyPlan) : ICommand<EntityResult>;
public record GenerateProjectDailyPlansRequest(CateringGenerateProjectDailyPlansRequestDto Request);
public record GenerateProjectDailyPlansCommand(CateringGenerateProjectDailyPlansRequestDto Request) : ICommand<CateringGenerateDailyPlanResultDto>;

public record GetPlansQuery(Guid? CompanyId, Guid? ContractId, PaginationRequest Pagination) : IQuery<GetPlansResult>;
public record GetPlansResult(PaginatedResult<CateringOperationalPlanDto> Plans);
public record GetPlansResponse(PaginatedResult<CateringOperationalPlanDto> Plans);
public record SavePlanRequest(CateringOperationalPlanDto Plan);
public record CreatePlanCommand(CateringOperationalPlanDto Plan) : ICommand<EntityResult>;
public record UpdatePlanCommand(Guid Id, CateringOperationalPlanDto Plan) : ICommand<UpdateResult>;
public record DeletePlanCommand(Guid Id) : ICommand<UpdateResult>;
public record SavePlanResourceRequest(CateringPlanResourceAssignmentDto Resource);
public record AddPlanResourceCommand(Guid PlanId, CateringPlanResourceAssignmentDto Resource) : ICommand<EntityResult>;
public record DeletePlanResourceCommand(Guid PlanId, Guid ResourceId) : ICommand<UpdateResult>;
public record GenerateDailyPlansRequest(CateringGenerateDailyPlanRequestDto Request);
public record GenerateDailyPlansCommand(CateringGenerateDailyPlanRequestDto Request) : ICommand<CateringGenerateDailyPlanResultDto>;

public record GetInventoryRequestsQuery(Guid? CompanyId, Guid? PlanId, Guid? ScheduleId, CateringInventoryRequestStatus? Status, PaginationRequest Pagination) : IQuery<GetInventoryRequestsResult>;
public record GetInventoryRequestsResult(PaginatedResult<CateringInventoryRequestDto> Requests);
public record GetInventoryRequestsResponse(PaginatedResult<CateringInventoryRequestDto> Requests);
public record SaveInventoryRequestRequest(CateringInventoryRequestDto Request);
public record CreateInventoryRequestCommand(CateringInventoryRequestDto Request) : ICommand<EntityResult>;
public record SubmitInventoryRequestCommand(Guid Id) : ICommand<UpdateResult>;
public record ApproveInventoryRequestCommand(Guid Id) : ICommand<UpdateResult>;
public record FulfillInventoryRequestCommand(Guid Id) : ICommand<UpdateResult>;

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
            Results.Ok((await sender.Send(new GetSchedulesQuery(query.ContractId, query.ProjectId, query.ProjectDailyPlanId, query.FromDate, query.ToDate, pagination))).Adapt<GetSchedulesResponse>()))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.View);
        group.MapPost("/schedules", async (SaveScheduleRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateScheduleCommand(request.Schedule))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Create);
        group.MapPut("/schedules/{id:guid}", async (Guid id, SaveScheduleRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateScheduleCommand(id, request.Schedule))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Edit);
        group.MapPut("/schedules/{id:guid}/activate", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new ActivateScheduleCommand(id))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Edit);
        group.MapPost("/schedules/{id:guid}/allocations", async (Guid id, SaveAllocationRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateAllocationCommand(id, request.Allocation))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Edit);
        group.MapPut("/allocations/{id:guid}/actuals", async (Guid id, RecordAllocationActualsRequest request, ISender sender) => Results.Ok(await sender.Send(new RecordAllocationActualsCommand(id, request.ReceivedQuantity, request.DistributedQuantity, request.ActualArrivalTime, request.ReceivingSupervisorEmployeeId, request.ReceivingSupervisorName, request.TeamLeaderEmployeeId, request.TeamLeaderName, request.VarianceNotes))))
            .RequireAuthorization(PermissionList.CateringDistributionPermissions.Edit);

        group.MapGet("/packaging", async ([AsParameters] DeliveryListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetPackagingPlansQuery(query.ScheduleId, query.FromDate, query.ToDate, pagination))).Adapt<GetPackagingPlansResponse>()))
            .RequireAuthorization(PermissionList.CateringPackagingPermissions.View);
        group.MapPost("/packaging", async (SavePackagingPlanRequest request, ISender sender) => Results.Ok(await sender.Send(new UpsertPackagingPlanCommand(request.PackagingPlan))))
            .RequireAuthorization(PermissionList.CateringPackagingPermissions.Create);
        group.MapPut("/packaging/{id:guid}", async (Guid id, SavePackagingPlanRequest request, ISender sender) =>
        {
            request.PackagingPlan.Id = id;
            return Results.Ok(await sender.Send(new UpsertPackagingPlanCommand(request.PackagingPlan)));
        }).RequireAuthorization(PermissionList.CateringPackagingPermissions.Edit);
        group.MapPut("/packaging/{id:guid}/release-stock", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new ReleasePackagingStockCommand(id))))
            .RequireAuthorization(PermissionList.CateringPackagingPermissions.Post);
        group.MapPut("/packaging/{id:guid}/start", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new StartPackagingCommand(id))))
            .RequireAuthorization(PermissionList.CateringPackagingPermissions.Edit);
        group.MapPut("/packaging/{id:guid}/complete", async (Guid id, CompletePackagingRequest request, ISender sender) => Results.Ok(await sender.Send(new CompletePackagingCommand(id, request.PreparedMealCount, request.RejectedMealCount, request.DamagedMealCount, request.VarianceReason))))
            .RequireAuthorization(PermissionList.CateringPackagingPermissions.Edit);

        group.MapGet("/dispatches", async ([AsParameters] DeliveryListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetDispatchPlansQuery(query.ScheduleId, query.FromDate, query.ToDate, pagination))).Adapt<GetDispatchPlansResponse>()))
            .RequireAuthorization(PermissionList.CateringDeliveryPermissions.View);
        group.MapPost("/dispatches", async (SaveDispatchPlanRequest request, ISender sender) => Results.Ok(await sender.Send(new UpsertDispatchPlanCommand(request.DispatchPlan))))
            .RequireAuthorization(PermissionList.CateringDeliveryPermissions.Create);
        group.MapPut("/dispatches/{id:guid}", async (Guid id, SaveDispatchPlanRequest request, ISender sender) =>
        {
            request.DispatchPlan.Id = id;
            return Results.Ok(await sender.Send(new UpsertDispatchPlanCommand(request.DispatchPlan)));
        }).RequireAuthorization(PermissionList.CateringDeliveryPermissions.Edit);
        group.MapPut("/dispatches/{id:guid}/fleet-assignment", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new CreateDispatchFleetAssignmentCommand(id))))
            .RequireAuthorization(PermissionList.CateringDeliveryPermissions.Edit);
        group.MapPost("/execution-events", async (RecordExecutionEventRequest request, ISender sender) => Results.Ok(await sender.Send(new RecordExecutionEventCommand(request.Event))))
            .RequireAuthorization(PermissionList.CateringExecutionPermissions.Edit);

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

        group.MapGet("/projects", async ([AsParameters] PlanListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetProjectsQuery(query.CompanyId, query.ContractId, pagination))).Adapt<GetProjectsResponse>()))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.View);
        group.MapGet("/projects/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new GetProjectByIdQuery(id))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.View);
        group.MapPost("/projects", async (SaveProjectRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateProjectCommand(request.Project))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Create);
        group.MapPut("/projects/{id:guid}", async (Guid id, SaveProjectRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdateProjectCommand(id, request.Project))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Edit);
        group.MapDelete("/projects/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteProjectCommand(id))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Delete);
        group.MapPost("/projects/daily-plans", async (SaveProjectDailyPlanRequest request, ISender sender) => Results.Ok(await sender.Send(new UpsertProjectDailyPlanCommand(request.DailyPlan))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Create);
        group.MapPut("/projects/daily-plans/{id:guid}", async (Guid id, SaveProjectDailyPlanRequest request, ISender sender) =>
        {
            request.DailyPlan.Id = id;
            return Results.Ok(await sender.Send(new UpsertProjectDailyPlanCommand(request.DailyPlan)));
        }).RequireAuthorization(PermissionList.CateringPlanPermissions.Edit);
        group.MapPost("/projects/generate-daily", async (GenerateProjectDailyPlansRequest request, ISender sender) => Results.Ok(await sender.Send(new GenerateProjectDailyPlansCommand(request.Request))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Create);

        group.MapGet("/plans", async ([AsParameters] PlanListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetPlansQuery(query.CompanyId, query.ContractId, pagination))).Adapt<GetPlansResponse>()))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.View);
        group.MapPost("/plans", async (SavePlanRequest request, ISender sender) => Results.Ok(await sender.Send(new CreatePlanCommand(request.Plan))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Create);
        group.MapPut("/plans/{id:guid}", async (Guid id, SavePlanRequest request, ISender sender) => Results.Ok(await sender.Send(new UpdatePlanCommand(id, request.Plan))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Edit);
        group.MapDelete("/plans/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeletePlanCommand(id))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Delete);
        group.MapPost("/plans/{id:guid}/resources", async (Guid id, SavePlanResourceRequest request, ISender sender) => Results.Ok(await sender.Send(new AddPlanResourceCommand(id, request.Resource))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Edit);
        group.MapDelete("/plans/{planId:guid}/resources/{resourceId:guid}", async (Guid planId, Guid resourceId, ISender sender) => Results.Ok(await sender.Send(new DeletePlanResourceCommand(planId, resourceId))))
            .RequireAuthorization(PermissionList.CateringPlanPermissions.Edit);
        group.MapPost("/plans/generate-daily", async (GenerateDailyPlansRequest request, ISender sender) => Results.Ok(await sender.Send(new GenerateDailyPlansCommand(request.Request))))
            .RequireAuthorization(PermissionList.CateringSchedulePermissions.Create);

        group.MapGet("/inventory-requests", async ([AsParameters] InventoryRequestListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
            Results.Ok((await sender.Send(new GetInventoryRequestsQuery(query.CompanyId, query.PlanId, query.ScheduleId, query.Status, pagination))).Adapt<GetInventoryRequestsResponse>()))
            .RequireAuthorization(PermissionList.CateringInventoryRequestPermissions.View);
        group.MapPost("/inventory-requests", async (SaveInventoryRequestRequest request, ISender sender) => Results.Ok(await sender.Send(new CreateInventoryRequestCommand(request.Request))))
            .RequireAuthorization(PermissionList.CateringInventoryRequestPermissions.Create);
        group.MapPut("/inventory-requests/{id:guid}/submit", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new SubmitInventoryRequestCommand(id))))
            .RequireAuthorization(PermissionList.CateringInventoryRequestPermissions.Create);
        group.MapPut("/inventory-requests/{id:guid}/approve", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new ApproveInventoryRequestCommand(id))))
            .RequireAuthorization(PermissionList.CateringInventoryRequestPermissions.Approve);
        group.MapPut("/inventory-requests/{id:guid}/fulfill", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new FulfillInventoryRequestCommand(id))))
            .RequireAuthorization(PermissionList.CateringInventoryRequestPermissions.Fulfill);

        group.MapGet("/reports/operations", async ([AsParameters] ReportQuery query, ISender sender) =>
            Results.Ok((await sender.Send(new GetCateringReportQuery(query.CompanyId, query.ContractId, query.CustomerId, query.SquareId, query.VehicleId, query.FromDate, query.ToDate))).Adapt<GetCateringReportResponse>()))
            .RequireAuthorization(PermissionList.CateringReportsPermissions.View);
    }
}

public class CateringHandlers(CateringDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender) :
    IQueryHandler<GetMealsQuery, GetMealsResult>, IQueryHandler<GetMealByIdQuery, GetMealByIdResult>, ICommandHandler<CreateMealCommand, EntityResult>, ICommandHandler<UpdateMealCommand, UpdateResult>, ICommandHandler<DeleteMealCommand, UpdateResult>, ICommandHandler<AddMealComponentCommand, EntityResult>, ICommandHandler<DeleteMealComponentCommand, UpdateResult>,
    IQueryHandler<GetContractsQuery, GetContractsResult>, IQueryHandler<GetContractByIdQuery, GetContractByIdResult>, ICommandHandler<CreateContractCommand, EntityResult>, ICommandHandler<UpdateContractCommand, UpdateResult>, ICommandHandler<DeleteContractCommand, UpdateResult>, ICommandHandler<CloseContractCommand, UpdateResult>, ICommandHandler<AddContractAddendumCommand, EntityResult>,
    IQueryHandler<GetAreasQuery, GetAreasResult>, ICommandHandler<CreateAreaCommand, EntityResult>, ICommandHandler<UpdateAreaCommand, UpdateResult>, IQueryHandler<GetSquaresQuery, GetSquaresResult>, ICommandHandler<CreateSquareCommand, EntityResult>, ICommandHandler<UpdateSquareCommand, UpdateResult>,
    IQueryHandler<GetSchedulesQuery, GetSchedulesResult>, ICommandHandler<CreateScheduleCommand, EntityResult>, ICommandHandler<UpdateScheduleCommand, UpdateResult>, ICommandHandler<CreateAllocationCommand, EntityResult>, ICommandHandler<RecordAllocationActualsCommand, UpdateResult>, ICommandHandler<ActivateScheduleCommand, UpdateResult>,
    IQueryHandler<GetPackagingPlansQuery, GetPackagingPlansResult>, ICommandHandler<UpsertPackagingPlanCommand, EntityResult>, ICommandHandler<ReleasePackagingStockCommand, UpdateResult>, ICommandHandler<StartPackagingCommand, UpdateResult>, ICommandHandler<CompletePackagingCommand, UpdateResult>,
    IQueryHandler<GetDispatchPlansQuery, GetDispatchPlansResult>, ICommandHandler<UpsertDispatchPlanCommand, EntityResult>, ICommandHandler<CreateDispatchFleetAssignmentCommand, UpdateResult>, ICommandHandler<RecordExecutionEventCommand, UpdateResult>,
    IQueryHandler<GetDeliveriesQuery, GetDeliveriesResult>, ICommandHandler<CreateDeliveryCommand, EntityResult>,
    IQueryHandler<GetAssignmentsQuery, GetAssignmentsResult>, ICommandHandler<CreateAssignmentCommand, EntityResult>, ICommandHandler<UpdateAssignmentCommand, UpdateResult>, ICommandHandler<DeleteAssignmentCommand, UpdateResult>,
    IQueryHandler<GetProjectsQuery, GetProjectsResult>, IQueryHandler<GetProjectByIdQuery, GetProjectByIdResult>, ICommandHandler<CreateProjectCommand, EntityResult>, ICommandHandler<UpdateProjectCommand, UpdateResult>, ICommandHandler<DeleteProjectCommand, UpdateResult>, ICommandHandler<UpsertProjectDailyPlanCommand, EntityResult>, ICommandHandler<GenerateProjectDailyPlansCommand, CateringGenerateDailyPlanResultDto>,
    IQueryHandler<GetPlansQuery, GetPlansResult>, ICommandHandler<CreatePlanCommand, EntityResult>, ICommandHandler<UpdatePlanCommand, UpdateResult>, ICommandHandler<DeletePlanCommand, UpdateResult>, ICommandHandler<AddPlanResourceCommand, EntityResult>, ICommandHandler<DeletePlanResourceCommand, UpdateResult>, ICommandHandler<GenerateDailyPlansCommand, CateringGenerateDailyPlanResultDto>,
    IQueryHandler<GetInventoryRequestsQuery, GetInventoryRequestsResult>, ICommandHandler<CreateInventoryRequestCommand, EntityResult>, ICommandHandler<SubmitInventoryRequestCommand, UpdateResult>, ICommandHandler<ApproveInventoryRequestCommand, UpdateResult>, ICommandHandler<FulfillInventoryRequestCommand, UpdateResult>,
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
        await ValidateGenericContractLinkAsync(request.Contract, cancellationToken);
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
        await ValidateGenericContractLinkAsync(request.Contract, cancellationToken);
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
        if (request.ProjectId.HasValue) query = query.Where(x => x.CateringProjectId == request.ProjectId);
        if (request.ProjectDailyPlanId.HasValue) query = query.Where(x => x.CateringProjectDailyPlanId == request.ProjectDailyPlanId);
        if (request.FromDate.HasValue) query = query.Where(x => x.ServiceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.ServiceDate <= request.ToDate.Value.Date);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.ServiceDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var squares = await SquareNamesAsync(data.SelectMany(x => x.Allocations).Select(x => x.SquareId), cancellationToken);
        var scheduleIds = data.Select(x => x.Id).ToList();
        var packaging = await dbContext.CateringPackagingPlans.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId)).ToDictionaryAsync(x => x.DailyScheduleId, cancellationToken);
        var dispatches = await dbContext.CateringDispatchPlans.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId)).ToDictionaryAsync(x => x.DailyScheduleId, cancellationToken);
        var events = await dbContext.CateringExecutionEvents.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId)).OrderByDescending(x => x.OccurredAt).ToListAsync(cancellationToken);
        return new GetSchedulesResult(new PaginatedResult<CateringDailyScheduleDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, squares, packaging.GetValueOrDefault(x.Id), dispatches.GetValueOrDefault(x.Id), events.Where(e => e.DailyScheduleId == x.Id)))));
    }

    public async Task<EntityResult> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        await ResolveScheduleProjectAsync(request.Schedule, cancellationToken);
        await ValidateScheduleAgainstContractAsync(request.Schedule, null, cancellationToken);
        var schedule = CateringDailySchedule.Create(request.Schedule, UserId());
        await dbContext.CateringDailySchedules.AddAsync(schedule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(schedule.Id);
    }

    public async Task<UpdateResult> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.Id}");
        await ResolveScheduleProjectAsync(request.Schedule, cancellationToken);
        await ValidateScheduleAgainstContractAsync(request.Schedule, request.Id, cancellationToken);
        schedule.Update(request.Schedule, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(ActivateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.Include(x => x.Allocations).FirstOrDefaultAsync(x => x.Id == request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.ScheduleId}");
        var allocated = schedule.Allocations.Sum(x => x.PlannedQuantity);
        if (allocated != schedule.PlannedQuantity)
            throw new BadRequestException("Block allocation total must equal the schedule planned quantity before activation.");
        schedule.SetStatus(CateringScheduleStatus.Planned, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(CreateAllocationCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.Include(x => x.Allocations).FirstOrDefaultAsync(x => x.Id == request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.ScheduleId}");
        await EnsureSquareAsync(request.Allocation.SquareId, cancellationToken);
        if (schedule.CateringProjectId.HasValue)
        {
            var inScope = await dbContext.CateringProjectSquareScopes.AnyAsync(x => x.CateringProjectId == schedule.CateringProjectId.Value && x.SquareId == request.Allocation.SquareId, cancellationToken);
            if (!inScope) throw new BadRequestException("Square allocation must be inside the catering project square scope.");
        }
        var dto = request.Allocation;
        dto.DailyScheduleId = request.ScheduleId;
        var existingAllocated = schedule.Allocations.Sum(x => x.PlannedQuantity);
        if (existingAllocated + dto.PlannedQuantity > schedule.PlannedQuantity)
            throw new BadRequestException("Block allocations cannot exceed the schedule planned quantity.");
        var allocation = CateringSquareAllocation.Create(dto, UserId());
        await dbContext.CateringSquareAllocations.AddAsync(allocation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(allocation.Id);
    }

    public async Task<UpdateResult> Handle(RecordAllocationActualsCommand request, CancellationToken cancellationToken)
    {
        var allocation = await dbContext.CateringSquareAllocations.FirstOrDefaultAsync(x => x.Id == request.AllocationId, cancellationToken)
            ?? throw new NotFoundException($"Catering allocation not found: {request.AllocationId}");
        var dispatch = await dbContext.CateringDispatchPlans.AsNoTracking().FirstOrDefaultAsync(x => x.DailyScheduleId == allocation.DailyScheduleId, cancellationToken);
        if (dispatch is not null && dispatch.LoadedMealCount > 0)
        {
            var otherDistributed = await dbContext.CateringSquareAllocations.AsNoTracking()
                .Where(x => x.DailyScheduleId == allocation.DailyScheduleId && x.Id != allocation.Id)
                .SumAsync(x => x.DistributedQuantity, cancellationToken);
            if (otherDistributed + request.DistributedQuantity > dispatch.LoadedMealCount && string.IsNullOrWhiteSpace(request.VarianceNotes))
                throw new BadRequestException("Distributed quantity cannot exceed loaded meals without variance notes.");
        }
        allocation.RecordActuals(request.ReceivedQuantity, request.DistributedQuantity, request.ActualArrivalTime, request.ReceivingSupervisorEmployeeId, request.ReceivingSupervisorName, request.TeamLeaderEmployeeId, request.TeamLeaderName, request.VarianceNotes, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetPackagingPlansResult> Handle(GetPackagingPlansQuery request, CancellationToken cancellationToken)
    {
        var query = from plan in dbContext.CateringPackagingPlans.AsNoTracking()
                    join schedule in dbContext.CateringDailySchedules.AsNoTracking() on plan.DailyScheduleId equals schedule.Id
                    select new { plan, schedule };
        if (request.ScheduleId.HasValue) query = query.Where(x => x.plan.DailyScheduleId == request.ScheduleId);
        if (request.FromDate.HasValue) query = query.Where(x => x.schedule.ServiceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.schedule.ServiceDate <= request.ToDate.Value.Date);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.schedule.ServiceDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).Select(x => x.plan).ToListAsync(cancellationToken);
        return new GetPackagingPlansResult(new PaginatedResult<CateringPackagingPlanDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<EntityResult> Handle(UpsertPackagingPlanCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.Include(x => x.Allocations).FirstOrDefaultAsync(x => x.Id == request.PackagingPlan.DailyScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.PackagingPlan.DailyScheduleId}");
        if (request.PackagingPlan.RequiredMealCount != schedule.PlannedQuantity)
            request.PackagingPlan.RequiredMealCount = schedule.PlannedQuantity;

        var existing = request.PackagingPlan.Id == Guid.Empty
            ? await dbContext.CateringPackagingPlans.FirstOrDefaultAsync(x => x.DailyScheduleId == request.PackagingPlan.DailyScheduleId, cancellationToken)
            : await dbContext.CateringPackagingPlans.FirstOrDefaultAsync(x => x.Id == request.PackagingPlan.Id, cancellationToken);
        if (existing is null)
        {
            var plan = CateringPackagingPlan.Create(request.PackagingPlan, UserId());
            await dbContext.CateringPackagingPlans.AddAsync(plan, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new EntityResult(plan.Id);
        }
        existing.UpdatePlan(request.PackagingPlan, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(existing.Id);
    }

    public async Task<UpdateResult> Handle(ReleasePackagingStockCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringPackagingPlans.FirstOrDefaultAsync(x => x.Id == request.PackagingPlanId, cancellationToken)
            ?? throw new NotFoundException($"Packaging plan not found: {request.PackagingPlanId}");
        var schedule = await dbContext.CateringDailySchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == plan.DailyScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {plan.DailyScheduleId}");
        var contract = await dbContext.CateringContracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringContractId, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {schedule.CateringContractId}");
        var meal = await dbContext.MealDefinitions.Include(x => x.Components).AsNoTracking().FirstOrDefaultAsync(x => x.Id == contract.MealDefinitionId, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {contract.MealDefinitionId}");
        var lines = await BuildInventoryRequestLinesAsync(meal.Id, plan.RequiredMealCount, cancellationToken);
        var inventoryIds = new List<Guid>();
        foreach (var line in lines)
        {
            if (!line.ProductId.HasValue) throw new BadRequestException($"Meal item {line.ProductSkuName} must be linked to a product.");
            var result = await sender.Send(new PostInventoryStockOutBySkuCommand(
                line.ProductId.Value,
                line.ProductSkuId,
                line.ProductPackageId,
                plan.SourceWarehouseId,
                line.RequiredQuantity,
                0m,
                0m,
                null,
                contract.CompanyId,
                $"Catering stock release for schedule {schedule.ServiceDate:yyyy-MM-dd}",
                plan.Id.ToString(),
                "CateringPackagingPlan"), cancellationToken);
            inventoryIds.Add(result.InventoryId);
        }
        plan.MarkStockReleased(plan.RequiredMealCount, inventoryIds, UserId());
        await AddExecutionEventAsync(new CateringExecutionEventDto { DailyScheduleId = plan.DailyScheduleId, EventType = CateringExecutionEventType.StockReleased, Quantity = plan.RequiredMealCount, Notes = "Stock released to packaging." }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(StartPackagingCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringPackagingPlans.FirstOrDefaultAsync(x => x.Id == request.PackagingPlanId, cancellationToken)
            ?? throw new NotFoundException($"Packaging plan not found: {request.PackagingPlanId}");
        plan.StartPreparation(UserId());
        await AddExecutionEventAsync(new CateringExecutionEventDto { DailyScheduleId = plan.DailyScheduleId, EventType = CateringExecutionEventType.PackagingStarted }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(CompletePackagingCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringPackagingPlans.FirstOrDefaultAsync(x => x.Id == request.PackagingPlanId, cancellationToken)
            ?? throw new NotFoundException($"Packaging plan not found: {request.PackagingPlanId}");
        plan.Complete(request.PreparedMealCount, request.RejectedMealCount, request.DamagedMealCount, request.VarianceReason, UserId());
        var schedule = await dbContext.CateringDailySchedules.FirstOrDefaultAsync(x => x.Id == plan.DailyScheduleId, cancellationToken);
        schedule?.SetStatus(plan.Status == CateringPackagingStatus.Completed ? CateringScheduleStatus.ReadyForDispatch : CateringScheduleStatus.Packaging, UserId());
        await AddExecutionEventAsync(new CateringExecutionEventDto { DailyScheduleId = plan.DailyScheduleId, EventType = CateringExecutionEventType.PackagingCompleted, Quantity = request.PreparedMealCount, Notes = request.VarianceReason }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<GetDispatchPlansResult> Handle(GetDispatchPlansQuery request, CancellationToken cancellationToken)
    {
        var query = from plan in dbContext.CateringDispatchPlans.AsNoTracking()
                    join schedule in dbContext.CateringDailySchedules.AsNoTracking() on plan.DailyScheduleId equals schedule.Id
                    select new { plan, schedule };
        if (request.ScheduleId.HasValue) query = query.Where(x => x.plan.DailyScheduleId == request.ScheduleId);
        if (request.FromDate.HasValue) query = query.Where(x => x.schedule.ServiceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.schedule.ServiceDate <= request.ToDate.Value.Date);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.schedule.ServiceDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).Select(x => x.plan).ToListAsync(cancellationToken);
        return new GetDispatchPlansResult(new PaginatedResult<CateringDispatchPlanDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<EntityResult> Handle(UpsertDispatchPlanCommand request, CancellationToken cancellationToken)
    {
        await EnsureScheduleReadyForDispatchAsync(request.DispatchPlan.DailyScheduleId, cancellationToken);
        var existing = request.DispatchPlan.Id == Guid.Empty
            ? await dbContext.CateringDispatchPlans.FirstOrDefaultAsync(x => x.DailyScheduleId == request.DispatchPlan.DailyScheduleId, cancellationToken)
            : await dbContext.CateringDispatchPlans.FirstOrDefaultAsync(x => x.Id == request.DispatchPlan.Id, cancellationToken);
        if (existing is null)
        {
            var plan = CateringDispatchPlan.Create(request.DispatchPlan, UserId());
            await dbContext.CateringDispatchPlans.AddAsync(plan, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new EntityResult(plan.Id);
        }
        existing.UpdatePlan(request.DispatchPlan, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(existing.Id);
    }

    public async Task<UpdateResult> Handle(CreateDispatchFleetAssignmentCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringDispatchPlans.FirstOrDefaultAsync(x => x.Id == request.DispatchPlanId, cancellationToken)
            ?? throw new NotFoundException($"Dispatch plan not found: {request.DispatchPlanId}");
        if (plan.FleetAssignmentId.HasValue) return new UpdateResult(true);
        var schedule = await dbContext.CateringDailySchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == plan.DailyScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {plan.DailyScheduleId}");
        var result = await sender.Send(new CreateFleetVehicleAssignmentCommand(new SharedWithUI.Fleet.Dtos.CreateFleetVehicleAssignmentDto
        {
            VehicleId = plan.VehicleId,
            EmployeeId = plan.DriverEmployeeId,
            StartDate = schedule.ServiceDate,
            EndDate = schedule.ServiceDate,
            Purpose = $"Catering dispatch {schedule.ServiceDate:yyyy-MM-dd}"
        }), cancellationToken);
        plan.AttachFleetAssignment(result.Id, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(RecordExecutionEventCommand request, CancellationToken cancellationToken)
    {
        await ApplyExecutionEventAsync(request.Event, cancellationToken);
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

    public async Task<GetProjectsResult> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringProjects
            .Include(x => x.Contracts)
            .Include(x => x.Squares)
            .Include(x => x.DailyPlans)
            .AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ContractId.HasValue) query = query.Where(x => x.Contracts.Any(c => c.CateringContractId == request.ContractId.Value));
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.StartDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var dtos = await ProjectDtosAsync(data, cancellationToken);
        return new GetProjectsResult(new PaginatedResult<CateringProjectDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, dtos));
    }

    public async Task<GetProjectByIdResult> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await dbContext.CateringProjects
            .Include(x => x.Contracts)
            .Include(x => x.Squares)
            .Include(x => x.DailyPlans)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering project not found: {request.Id}");
        return new GetProjectByIdResult((await ProjectDtosAsync([project], cancellationToken)).Single());
    }

    public async Task<EntityResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        await ValidateProjectAsync(request.Project, cancellationToken);
        var project = CateringProject.Create(request.Project, UserId());
        await dbContext.CateringProjects.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await SyncProjectChildrenAsync(project.Id, request.Project, cancellationToken);
        return new EntityResult(project.Id);
    }

    public async Task<UpdateResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.CateringProjects.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering project not found: {request.Id}");
        request.Project.Id = request.Id;
        await ValidateProjectAsync(request.Project, cancellationToken);
        project.Update(request.Project, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        await SyncProjectChildrenAsync(project.Id, request.Project, cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.CateringProjects.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering project not found: {request.Id}");
        project.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(UpsertProjectDailyPlanCommand request, CancellationToken cancellationToken)
    {
        var project = await EnsureProjectAsync(request.DailyPlan.CateringProjectId, cancellationToken);
        ValidateProjectDailyPlan(project, request.DailyPlan, allowExtensionDays: request.DailyPlan.ServiceDate.Date > project.EndDate.Date);
        CateringProjectDailyPlan dailyPlan;
        if (request.DailyPlan.Id == Guid.Empty)
        {
            if (await dbContext.CateringProjectDailyPlans.AnyAsync(x => x.CateringProjectId == request.DailyPlan.CateringProjectId && x.ServiceDate == request.DailyPlan.ServiceDate.Date, cancellationToken))
                throw new BadRequestException("A project daily plan already exists for this date.");
            dailyPlan = CateringProjectDailyPlan.Create(request.DailyPlan, UserId());
            await dbContext.CateringProjectDailyPlans.AddAsync(dailyPlan, cancellationToken);
        }
        else
        {
            dailyPlan = await dbContext.CateringProjectDailyPlans.FirstOrDefaultAsync(x => x.Id == request.DailyPlan.Id, cancellationToken)
                ?? throw new NotFoundException($"Project daily plan not found: {request.DailyPlan.Id}");
            dailyPlan.Update(request.DailyPlan, UserId());
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(dailyPlan.Id);
    }

    public async Task<CateringGenerateDailyPlanResultDto> Handle(GenerateProjectDailyPlansCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.CateringProjects.Include(x => x.Contracts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Request.CateringProjectId, cancellationToken)
            ?? throw new NotFoundException($"Catering project not found: {request.Request.CateringProjectId}");
        if (!project.Contracts.Any()) throw new BadRequestException("Link at least one contract before generating project daily plans.");
        if (request.Request.PlannedQuantity <= 0) throw new BadRequestException("Planned quantity must be greater than zero.");
        var from = request.Request.UseProjectDuration ? project.StartDate.Date : request.Request.FromDate.Date;
        var to = request.Request.UseProjectDuration ? project.EndDate.Date : request.Request.ToDate.Date;
        if (to < from) throw new BadRequestException("Daily plan end date cannot be before start date.");
        if (!request.Request.AllowExtensionDays && (from < project.StartDate.Date || to > project.EndDate.Date))
            throw new BadRequestException("Project daily plan dates must be inside the project duration unless extension days are allowed.");

        var result = new CateringGenerateDailyPlanResultDto();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var exists = await dbContext.CateringProjectDailyPlans.AnyAsync(x => x.CateringProjectId == project.Id && x.ServiceDate == date, cancellationToken);
            if (exists)
            {
                result.SkippedCount++;
                result.SkippedDates.Add(date);
                continue;
            }
            var dto = new CateringProjectDailyPlanDto
            {
                CateringProjectId = project.Id,
                ServiceDate = date,
                PlannedQuantity = request.Request.PlannedQuantity,
                Status = CateringProjectDailyPlanStatus.Draft,
                Notes = date > project.EndDate.Date ? "Extension day" : null
            };
            ValidateProjectDailyPlan(project, dto, request.Request.AllowExtensionDays);
            await dbContext.CateringProjectDailyPlans.AddAsync(CateringProjectDailyPlan.Create(dto, UserId()), cancellationToken);
            result.CreatedCount++;
            result.CreatedDates.Add(date);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<GetPlansResult> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringOperationalPlans.Include(x => x.Resources).AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ContractId.HasValue) query = query.Where(x => x.CateringContractId == request.ContractId);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.StartDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        var contracts = await ContractLookupAsync(data.Select(x => x.CateringContractId), cancellationToken);
        var squares = await SquareNamesAsync(data.SelectMany(x => x.Resources).Where(x => x.SquareId.HasValue).Select(x => x.SquareId!.Value), cancellationToken);
        return new GetPlansResult(new PaginatedResult<CateringOperationalPlanDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, contracts, squares))));
    }

    public async Task<EntityResult> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var contract = await EnsureContractAsync(request.Plan.CateringContractId, cancellationToken);
        request.Plan.CompanyId = request.Plan.CompanyId == Guid.Empty ? contract.CompanyId : request.Plan.CompanyId;
        if (request.Plan.StartDate.Date < contract.StartDate || request.Plan.EndDate.Date > contract.EndDate)
            throw new BadRequestException("Plan dates must stay inside the catering contract period.");
        var plan = CateringOperationalPlan.Create(request.Plan, UserId());
        await dbContext.CateringOperationalPlans.AddAsync(plan, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(plan.Id);
    }

    public async Task<UpdateResult> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringOperationalPlans.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering plan not found: {request.Id}");
        var contract = await EnsureContractAsync(request.Plan.CateringContractId, cancellationToken);
        request.Plan.CompanyId = request.Plan.CompanyId == Guid.Empty ? contract.CompanyId : request.Plan.CompanyId;
        if (request.Plan.StartDate.Date < contract.StartDate || request.Plan.EndDate.Date > contract.EndDate)
            throw new BadRequestException("Plan dates must stay inside the catering contract period.");
        plan.Update(request.Plan, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringOperationalPlans.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering plan not found: {request.Id}");
        plan.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<EntityResult> Handle(AddPlanResourceCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.CateringOperationalPlans.AnyAsync(x => x.Id == request.PlanId, cancellationToken))
            throw new NotFoundException($"Catering plan not found: {request.PlanId}");
        if (request.Resource.SquareId.HasValue) await EnsureSquareAsync(request.Resource.SquareId.Value, cancellationToken);
        var resource = CateringPlanResourceAssignment.Create(request.PlanId, request.Resource, UserId());
        await dbContext.CateringPlanResourceAssignments.AddAsync(resource, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(resource.Id);
    }

    public async Task<UpdateResult> Handle(DeletePlanResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await dbContext.CateringPlanResourceAssignments.FirstOrDefaultAsync(x => x.Id == request.ResourceId && x.CateringOperationalPlanId == request.PlanId, cancellationToken)
            ?? throw new NotFoundException($"Catering plan resource not found: {request.ResourceId}");
        resource.IsDeleted = true;
        resource.DeletedAt = DateTime.UtcNow;
        resource.DeletedBy = UserId();
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<CateringGenerateDailyPlanResultDto> Handle(GenerateDailyPlansCommand request, CancellationToken cancellationToken)
    {
        var plan = await dbContext.CateringOperationalPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Request.CateringOperationalPlanId, cancellationToken)
            ?? throw new NotFoundException($"Catering plan not found: {request.Request.CateringOperationalPlanId}");
        var contract = await EnsureContractAsync(plan.CateringContractId, cancellationToken);
        var from = request.Request.UseFullContractDuration ? contract.StartDate.Date : request.Request.FromDate.Date;
        var to = request.Request.UseFullContractDuration ? contract.EndDate.Date : request.Request.ToDate.Date;
        if (from < contract.StartDate.Date || to > contract.EndDate.Date || to < from)
            throw new BadRequestException("Generated schedule dates must be inside the contract period.");
        if (request.Request.PlannedQuantity <= 0) throw new BadRequestException("Planned quantity must be greater than zero.");

        var result = new CateringGenerateDailyPlanResultDto();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var exists = await dbContext.CateringDailySchedules.AnyAsync(x => x.CateringContractId == plan.CateringContractId && x.ServiceDate == date, cancellationToken);
            if (exists)
            {
                result.SkippedCount++;
                result.SkippedDates.Add(date);
                continue;
            }

            var scheduleDto = new CateringDailyScheduleDto
            {
                CateringOperationalPlanId = plan.Id,
                CateringContractId = plan.CateringContractId,
                ServiceDate = date,
                PlannedQuantity = request.Request.PlannedQuantity,
                Status = CateringScheduleStatus.Draft,
                Notes = $"Generated from catering plan {plan.Id}"
            };
            await ValidateScheduleAgainstContractAsync(scheduleDto, null, cancellationToken);
            await dbContext.CateringDailySchedules.AddAsync(CateringDailySchedule.Create(scheduleDto, UserId()), cancellationToken);
            result.CreatedCount++;
            result.CreatedDates.Add(date);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<GetInventoryRequestsResult> Handle(GetInventoryRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CateringInventoryRequests.Include(x => x.Lines).AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.PlanId.HasValue) query = query.Where(x => x.CateringOperationalPlanId == request.PlanId);
        if (request.ScheduleId.HasValue) query = query.Where(x => x.DailyScheduleId == request.ScheduleId);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        var count = await query.LongCountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.RequestDate).Skip(request.Pagination.PageIndex * request.Pagination.PageSize).Take(request.Pagination.PageSize).ToListAsync(cancellationToken);
        return new GetInventoryRequestsResult(new PaginatedResult<CateringInventoryRequestDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<EntityResult> Handle(CreateInventoryRequestCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Request.DailyScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {request.Request.DailyScheduleId}");
        var contract = await dbContext.CateringContracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringContractId, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {schedule.CateringContractId}");
        request.Request.CompanyId = contract.CompanyId;
        request.Request.BranchId = contract.BranchId;
        request.Request.CateringOperationalPlanId ??= schedule.CateringOperationalPlanId;
        request.Request.PlannedMealCount = request.Request.PlannedMealCount <= 0 ? schedule.PlannedQuantity : request.Request.PlannedMealCount;
        var lines = request.Request.Lines.Any() ? request.Request.Lines : await BuildInventoryRequestLinesAsync(contract.MealDefinitionId, request.Request.PlannedMealCount, cancellationToken);
        var entity = CateringInventoryRequest.Create(request.Request, lines, UserId());
        await dbContext.CateringInventoryRequests.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EntityResult(entity.Id);
    }

    public async Task<UpdateResult> Handle(SubmitInventoryRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CateringInventoryRequests.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering inventory request not found: {request.Id}");
        entity.Submit(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(ApproveInventoryRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CateringInventoryRequests.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering inventory request not found: {request.Id}");
        entity.Approve(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateResult(true);
    }

    public async Task<UpdateResult> Handle(FulfillInventoryRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CateringInventoryRequests.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Catering inventory request not found: {request.Id}");
        var movementIds = new List<Guid>();
        foreach (var line in entity.Lines)
        {
            var movement = await sender.Send(new PostInventoryStockOutBySkuCommand(
                line.ProductId ?? throw new BadRequestException($"Inventory request line {line.ProductSkuName} must be linked to a product."),
                line.ProductSkuId,
                line.ProductPackageId,
                entity.SourceWarehouseId,
                line.ApprovedQuantity,
                0m,
                0m,
                null,
                entity.CompanyId,
                $"Catering inventory request {entity.Id}",
                entity.Id.ToString(),
                "CateringInventoryRequest"), cancellationToken);
            movementIds.Add(movement.InventoryId);
        }
        entity.Fulfill(movementIds, UserId());
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
        var packagingPlans = dbContext.CateringPackagingPlans.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId));
        var dispatchPlans = dbContext.CateringDispatchPlans.AsNoTracking().Where(x => scheduleIds.Contains(x.DailyScheduleId));
        var today = DateTime.UtcNow.Date;
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
            DistributedQuantity = await allocations.SumAsync(x => x.DistributedQuantity, cancellationToken),
            PreparedQuantity = await packagingPlans.SumAsync(x => x.PreparedMealCount, cancellationToken),
            LoadedQuantity = await dispatchPlans.SumAsync(x => x.LoadedMealCount, cancellationToken),
            TodaySchedules = await schedules.CountAsync(x => x.ServiceDate == today, cancellationToken),
            PackagingInProgress = await packagingPlans.CountAsync(x => x.Status == CateringPackagingStatus.InProgress || x.Status == CateringPackagingStatus.StockReleased, cancellationToken),
            TrucksPendingLoad = await dispatchPlans.CountAsync(x => x.Status == CateringDispatchStatus.VehicleAssigned || x.Status == CateringDispatchStatus.ArrivedForLoading, cancellationToken),
            TrucksInTransit = await dispatchPlans.CountAsync(x => x.Status == CateringDispatchStatus.Departed, cancellationToken),
            LateDispatches = await dispatchPlans.CountAsync(x => x.PlannedDepartureTime.HasValue && x.PlannedDepartureTime.Value < DateTime.UtcNow && x.DepartedAt == null && x.Status != CateringDispatchStatus.Completed && x.Status != CateringDispatchStatus.Cancelled, cancellationToken),
            BlockVariances = await allocations.CountAsync(x => x.DistributedQuantity != 0 && x.DistributedQuantity != x.PlannedQuantity, cancellationToken),
            OperationalExceptions = await packagingPlans.CountAsync(x => x.Status == CateringPackagingStatus.Exception, cancellationToken)
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
        var reportScheduleIds = data.Select(x => x.schedule.Id).Distinct().ToList();
        var reportContractIds = data.Select(x => x.contract.Id).Distinct().ToList();
        var addendumTotals = await dbContext.CateringContractAddendums.AsNoTracking()
            .Where(x => reportContractIds.Contains(x.CateringContractId))
            .GroupBy(x => x.CateringContractId)
            .ToDictionaryAsync(x => x.Key, x => x.Sum(a => a.AddedQuantity), cancellationToken);
        var packagingBySchedule = await dbContext.CateringPackagingPlans.AsNoTracking()
            .Where(x => reportScheduleIds.Contains(x.DailyScheduleId))
            .ToDictionaryAsync(x => x.DailyScheduleId, cancellationToken);
        var dispatchBySchedule = await dbContext.CateringDispatchPlans.AsNoTracking()
            .Where(x => reportScheduleIds.Contains(x.DailyScheduleId))
            .ToDictionaryAsync(x => x.DailyScheduleId, cancellationToken);
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
            VehicleId = dispatchBySchedule.GetValueOrDefault(x.schedule.Id)?.VehicleId,
            VehicleName = dispatchBySchedule.GetValueOrDefault(x.schedule.Id)?.VehicleName,
            ScheduleStatus = x.schedule.Status,
            PackagingStatus = packagingBySchedule.GetValueOrDefault(x.schedule.Id)?.Status,
            DispatchStatus = dispatchBySchedule.GetValueOrDefault(x.schedule.Id)?.Status,
            ContractedQuantity = x.contract.ContractedMealQuantity + addendumTotals.GetValueOrDefault(x.contract.Id),
            ScheduledQuantity = x.allocation?.PlannedQuantity ?? x.schedule.PlannedQuantity,
            PreparedQuantity = packagingBySchedule.GetValueOrDefault(x.schedule.Id)?.PreparedMealCount ?? 0,
            LoadedQuantity = dispatchBySchedule.GetValueOrDefault(x.schedule.Id)?.LoadedMealCount ?? 0,
            ReceivedQuantity = x.allocation?.ReceivedQuantity ?? 0,
            DistributedQuantity = x.allocation?.DistributedQuantity ?? 0,
            VarianceQuantity = (x.allocation?.DistributedQuantity ?? 0) - (x.allocation?.PlannedQuantity ?? x.schedule.PlannedQuantity)
        }).ToList();

        if (request.VehicleId.HasValue)
        {
            rows = rows.Where(row => row.VehicleId == request.VehicleId).ToList();
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

    private async Task ValidateGenericContractLinkAsync(CateringContractDto contract, CancellationToken cancellationToken)
    {
        if (!contract.GenericContractId.HasValue)
            return;

        var linked = await sender.Send(new GetPartyContractsQuery("Customer", contract.CustomerId, contract.CompanyId, null), cancellationToken);
        if (!linked.Contracts.Any(x => x.Id == contract.GenericContractId.Value))
            throw new BadRequestException("Linked generic contract must belong to the same company and customer.");
    }

    private async Task ValidateProjectAsync(CateringProjectDto project, CancellationToken cancellationToken)
    {
        if (project.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(project.ProjectName)) throw new BadRequestException("Project name is required.");
        if (project.EndDate.Date < project.StartDate.Date) throw new BadRequestException("Project end date cannot be before start date.");
        if (!project.Contracts.Any()) throw new BadRequestException("Link at least one catering contract to the project.");

        var contractIds = project.Contracts.Select(x => x.CateringContractId).Where(x => x != Guid.Empty).Distinct().ToList();
        if (contractIds.Count != project.Contracts.Count) throw new BadRequestException("Project contracts must be unique and valid.");
        var contracts = await dbContext.CateringContracts.AsNoTracking().Where(x => contractIds.Contains(x.Id)).ToListAsync(cancellationToken);
        if (contracts.Count != contractIds.Count) throw new BadRequestException("One or more linked catering contracts were not found.");
        if (contracts.Any(x => x.CompanyId != project.CompanyId)) throw new BadRequestException("Linked contracts must belong to the project company.");
        if (project.BranchId.HasValue && contracts.Any(x => x.BranchId.HasValue && x.BranchId.Value != project.BranchId.Value))
            throw new BadRequestException("Linked contracts must match the project branch when both are branch-scoped.");

        var squareIds = project.Squares.Select(x => x.SquareId).Where(x => x != Guid.Empty).Distinct().ToList();
        if (squareIds.Count != project.Squares.Count) throw new BadRequestException("Project squares must be unique and valid.");
        if (squareIds.Any())
        {
            var found = await dbContext.CateringSquares.CountAsync(x => squareIds.Contains(x.Id), cancellationToken);
            if (found != squareIds.Count) throw new BadRequestException("One or more project squares were not found.");
        }
    }

    private async Task SyncProjectChildrenAsync(Guid projectId, CateringProjectDto dto, CancellationToken cancellationToken)
    {
        var userId = UserId();
        var contractIds = dto.Contracts.Select(x => x.CateringContractId).Where(x => x != Guid.Empty).Distinct().ToHashSet();
        var links = await dbContext.CateringProjectContractLinks.Where(x => x.CateringProjectId == projectId).ToListAsync(cancellationToken);
        foreach (var removed in links.Where(x => !contractIds.Contains(x.CateringContractId)))
        {
            removed.IsDeleted = true;
            removed.DeletedAt = DateTime.UtcNow;
            removed.DeletedBy = userId;
        }
        foreach (var contractId in contractIds.Where(id => links.All(x => x.CateringContractId != id)))
        {
            await dbContext.CateringProjectContractLinks.AddAsync(CateringProjectContractLink.Create(projectId, contractId, userId), cancellationToken);
        }

        var squareIds = dto.Squares.Select(x => x.SquareId).Where(x => x != Guid.Empty).Distinct().ToHashSet();
        var scopes = await dbContext.CateringProjectSquareScopes.Where(x => x.CateringProjectId == projectId).ToListAsync(cancellationToken);
        foreach (var removed in scopes.Where(x => !squareIds.Contains(x.SquareId)))
        {
            removed.IsDeleted = true;
            removed.DeletedAt = DateTime.UtcNow;
            removed.DeletedBy = userId;
        }
        foreach (var squareId in squareIds.Where(id => scopes.All(x => x.SquareId != id)))
        {
            await dbContext.CateringProjectSquareScopes.AddAsync(CateringProjectSquareScope.Create(projectId, squareId, userId), cancellationToken);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateProjectDailyPlan(CateringProject project, CateringProjectDailyPlanDto dailyPlan, bool allowExtensionDays)
    {
        if (dailyPlan.ServiceDate.Date < project.StartDate.Date)
            throw new BadRequestException("Project daily plan date cannot be before the project start date.");
        if (dailyPlan.ServiceDate.Date > project.EndDate.Date && !allowExtensionDays)
            throw new BadRequestException("Project daily plan date is beyond project duration. Enable extension days to continue.");
    }

    private async Task<CateringProject> EnsureProjectAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.CateringProjects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Catering project not found: {id}");
    }

    private async Task ResolveScheduleProjectAsync(CateringDailyScheduleDto schedule, CancellationToken cancellationToken)
    {
        if (!schedule.CateringProjectDailyPlanId.HasValue)
            return;

        var projectDay = await dbContext.CateringProjectDailyPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringProjectDailyPlanId.Value, cancellationToken)
            ?? throw new NotFoundException($"Project daily plan not found: {schedule.CateringProjectDailyPlanId.Value}");
        schedule.CateringProjectId = projectDay.CateringProjectId;
        schedule.ServiceDate = projectDay.ServiceDate;
    }

    private async Task ValidateScheduleAgainstContractAsync(CateringDailyScheduleDto schedule, Guid? existingScheduleId, CancellationToken cancellationToken)
    {
        var contract = await dbContext.CateringContracts.Include(x => x.Addendums).AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringContractId, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {schedule.CateringContractId}");
        if (schedule.ServiceDate.Date < contract.StartDate || schedule.ServiceDate.Date > contract.EndDate)
            throw new BadRequestException("Schedule date must be inside the catering contract period.");

        if (schedule.CateringProjectId.HasValue)
        {
            var project = await dbContext.CateringProjects.Include(x => x.Contracts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringProjectId.Value, cancellationToken)
                ?? throw new NotFoundException($"Catering project not found: {schedule.CateringProjectId.Value}");
            if (project.CompanyId != contract.CompanyId)
                throw new BadRequestException("Schedule contract must belong to the same company as the catering project.");
            if (project.BranchId.HasValue && contract.BranchId.HasValue && project.BranchId.Value != contract.BranchId.Value)
                throw new BadRequestException("Schedule contract must belong to the same branch as the catering project.");
            if (!project.Contracts.Any(x => x.CateringContractId == schedule.CateringContractId))
                throw new BadRequestException("Schedule contract must be linked to the catering project.");

            if (schedule.CateringProjectDailyPlanId.HasValue)
            {
                var projectDay = await dbContext.CateringProjectDailyPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringProjectDailyPlanId.Value && x.CateringProjectId == project.Id, cancellationToken)
                    ?? throw new NotFoundException($"Project daily plan not found: {schedule.CateringProjectDailyPlanId.Value}");
                if (projectDay.ServiceDate.Date != schedule.ServiceDate.Date)
                    throw new BadRequestException("Contract schedule date must match the selected project daily plan date.");

                var projectDayScheduledTotal = await dbContext.CateringDailySchedules.AsNoTracking()
                    .Where(x => x.CateringProjectDailyPlanId == projectDay.Id && (!existingScheduleId.HasValue || x.Id != existingScheduleId.Value))
                    .SumAsync(x => x.PlannedQuantity, cancellationToken);
                if (projectDayScheduledTotal + schedule.PlannedQuantity > projectDay.PlannedQuantity)
                    throw new BadRequestException("Contract schedules cannot exceed the project daily planned quantity.");
            }
        }

        var scheduledTotal = await dbContext.CateringDailySchedules.AsNoTracking()
            .Where(x => x.CateringContractId == schedule.CateringContractId && (!existingScheduleId.HasValue || x.Id != existingScheduleId.Value))
            .SumAsync(x => x.PlannedQuantity, cancellationToken);
        var contractedTotal = contract.ContractedMealQuantity + contract.Addendums
            .Where(x => x.EffectiveFrom.Date <= schedule.ServiceDate.Date && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= schedule.ServiceDate.Date))
            .Sum(x => x.AddedQuantity);
        if (scheduledTotal + schedule.PlannedQuantity > contractedTotal)
            throw new BadRequestException("Scheduled meal quantity cannot exceed contract quantity plus effective addendums.");
    }

    private async Task EnsureScheduleReadyForDispatchAsync(Guid scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.Include(x => x.Allocations).AsNoTracking().FirstOrDefaultAsync(x => x.Id == scheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {scheduleId}");
        if (schedule.Allocations.Sum(x => x.PlannedQuantity) != schedule.PlannedQuantity)
            throw new BadRequestException("Block allocation total must equal the schedule planned quantity before dispatch.");

        var contract = await dbContext.CateringContracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == schedule.CateringContractId, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {schedule.CateringContractId}");
        if (!contract.IsPackagingRequired)
            return;

        var packaging = await dbContext.CateringPackagingPlans.AsNoTracking().FirstOrDefaultAsync(x => x.DailyScheduleId == scheduleId, cancellationToken)
            ?? throw new BadRequestException("Packaging must be planned before dispatch.");
        if (packaging.Status != CateringPackagingStatus.Completed)
            throw new BadRequestException("Packaging must be completed before dispatch.");
        if (packaging.PreparedMealCount < schedule.PlannedQuantity)
            throw new BadRequestException("Prepared meals cannot be below planned dispatch quantity without resolving the packaging exception.");
    }

    private async Task AddExecutionEventAsync(CateringExecutionEventDto dto, CancellationToken cancellationToken)
    {
        var executionEvent = CateringExecutionEvent.Create(dto, UserId());
        await dbContext.CateringExecutionEvents.AddAsync(executionEvent, cancellationToken);
    }

    private async Task ApplyExecutionEventAsync(CateringExecutionEventDto dto, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.CateringDailySchedules.FirstOrDefaultAsync(x => x.Id == dto.DailyScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Catering schedule not found: {dto.DailyScheduleId}");
        var dispatch = dto.DispatchPlanId.HasValue
            ? await dbContext.CateringDispatchPlans.FirstOrDefaultAsync(x => x.Id == dto.DispatchPlanId.Value, cancellationToken)
            : await dbContext.CateringDispatchPlans.FirstOrDefaultAsync(x => x.DailyScheduleId == dto.DailyScheduleId, cancellationToken);

        if (dto.EventType is CateringExecutionEventType.TruckArrivedForLoading or CateringExecutionEventType.TruckLoaded or CateringExecutionEventType.TruckDeparted or CateringExecutionEventType.TruckArrivedAtDistribution)
        {
            if (dispatch is null) throw new BadRequestException("Dispatch plan is required for truck execution events.");
            if (dto.EventType == CateringExecutionEventType.TruckLoaded)
            {
                var packaging = await dbContext.CateringPackagingPlans.AsNoTracking().FirstOrDefaultAsync(x => x.DailyScheduleId == dto.DailyScheduleId, cancellationToken);
                if (packaging is not null && dto.Quantity.GetValueOrDefault() > packaging.PreparedMealCount)
                    throw new BadRequestException("Loaded meals cannot exceed prepared meals.");
            }
            dispatch.Record(dto.EventType, dto.Quantity, UserId());
            dto.DispatchPlanId = dispatch.Id;
            schedule.SetStatus(dto.EventType switch
            {
                CateringExecutionEventType.TruckDeparted => CateringScheduleStatus.InTransit,
                CateringExecutionEventType.TruckArrivedAtDistribution => CateringScheduleStatus.AtDistributionArea,
                _ => schedule.Status
            }, UserId());
        }

        if (dto.EventType == CateringExecutionEventType.BlockDelivered)
        {
            if (!dto.AllocationId.HasValue) throw new BadRequestException("Allocation is required for block delivery.");
            var allocation = await dbContext.CateringSquareAllocations.FirstOrDefaultAsync(x => x.Id == dto.AllocationId.Value, cancellationToken)
                ?? throw new NotFoundException($"Catering allocation not found: {dto.AllocationId.Value}");
            allocation.RecordActuals(dto.Quantity ?? allocation.ReceivedQuantity, dto.Quantity ?? allocation.DistributedQuantity, DateTime.UtcNow, dto.EmployeeId, dto.EmployeeName, null, null, dto.Notes, UserId());
        }

        if (dto.EventType == CateringExecutionEventType.SupervisorReceived && dispatch is not null)
        {
            dispatch.Complete(UserId());
            schedule.SetStatus(CateringScheduleStatus.Completed, UserId());
            if (dispatch.FleetAssignmentId.HasValue && dispatch.IsFleetAssignmentManagedByCatering)
            {
                await sender.Send(new ReturnFleetVehicleAssignmentCommand(dispatch.FleetAssignmentId.Value, new SharedWithUI.Fleet.Dtos.ReturnFleetVehicleAssignmentDto
                {
                    ReturnDate = DateTime.UtcNow.Date,
                    Notes = $"Returned from catering schedule {schedule.ServiceDate:yyyy-MM-dd}"
                }), cancellationToken);
            }
        }

        await AddExecutionEventAsync(dto, cancellationToken);
    }

    private async Task<CateringContract> EnsureContractAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.CateringContracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Catering contract not found: {id}");
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

    private async Task<Dictionary<Guid, CateringContractDto>> ContractLookupAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var contractIds = ids.Distinct().ToList();
        var contracts = await dbContext.CateringContracts.Include(x => x.Addendums).AsNoTracking().Where(x => contractIds.Contains(x.Id)).ToListAsync(cancellationToken);
        var meals = await MealNamesAsync(contracts.Select(x => x.MealDefinitionId), cancellationToken);
        return contracts.ToDictionary(x => x.Id, x => ToDto(x, meals));
    }

    private async Task<List<CateringProjectDto>> ProjectDtosAsync(IReadOnlyCollection<CateringProject> projects, CancellationToken cancellationToken)
    {
        var projectIds = projects.Select(x => x.Id).ToList();
        var contractIds = projects.SelectMany(x => x.Contracts).Select(x => x.CateringContractId).Distinct().ToList();
        var squareIds = projects.SelectMany(x => x.Squares).Select(x => x.SquareId).Distinct().ToList();
        var contracts = await ContractLookupAsync(contractIds, cancellationToken);
        var squares = await SquareNamesAsync(squareIds, cancellationToken);
        var resources = await dbContext.CateringPlanResourceAssignments.AsNoTracking().Where(x => projectIds.Contains(x.CateringOperationalPlanId)).ToListAsync(cancellationToken);
        var resourceSquares = await SquareNamesAsync(resources.Where(x => x.SquareId.HasValue).Select(x => x.SquareId!.Value), cancellationToken);
        var schedules = await dbContext.CateringDailySchedules.Include(x => x.Allocations).AsNoTracking().Where(x => x.CateringProjectId.HasValue && projectIds.Contains(x.CateringProjectId.Value)).ToListAsync(cancellationToken);
        var scheduleSquares = await SquareNamesAsync(schedules.SelectMany(x => x.Allocations).Select(x => x.SquareId), cancellationToken);

        return projects.Select(project => ToDto(
            project,
            contracts,
            squares,
            resources.Where(x => x.CateringOperationalPlanId == project.Id),
            resourceSquares,
            schedules.Where(x => x.CateringProjectId == project.Id),
            scheduleSquares)).ToList();
    }

    private async Task<List<CateringInventoryRequestLineDto>> BuildInventoryRequestLinesAsync(Guid mealId, decimal mealCount, CancellationToken cancellationToken)
    {
        var meal = await dbContext.MealDefinitions.Include(x => x.Components).AsNoTracking().FirstOrDefaultAsync(x => x.Id == mealId, cancellationToken)
            ?? throw new NotFoundException($"Meal not found: {mealId}");
        if (meal.StructureType == CateringMealStructureType.Product)
        {
            if (!meal.ProductSkuId.HasValue)
                throw new BadRequestException("Product meal requires a catalog SKU before inventory can be requested.");
            return
            [
                new CateringInventoryRequestLineDto
                {
                    ProductId = meal.ProductId,
                    ProductSkuId = meal.ProductSkuId.Value,
                    ProductPackageId = meal.ProductPackageId,
                    ProductSkuName = meal.ProductSkuName ?? meal.Name,
                    ProductSkuNameEng = meal.ProductSkuNameEng ?? meal.NameEng,
                    QuantityPerMeal = 1m,
                    RequiredQuantity = mealCount,
                    ApprovedQuantity = mealCount
                }
            ];
        }

        if (!meal.Components.Any())
            throw new BadRequestException("Combo meal components are required before inventory can be requested.");
        return meal.Components.Select(x => new CateringInventoryRequestLineDto
        {
            ProductId = x.ProductId,
            ProductSkuId = x.ProductSkuId,
            ProductPackageId = x.ProductPackageId,
            ProductSkuName = x.ComponentName,
            ProductSkuNameEng = x.ComponentNameEng,
            QuantityPerMeal = x.QuantityPerMeal,
            RequiredQuantity = x.QuantityPerMeal * mealCount,
            ApprovedQuantity = x.QuantityPerMeal * mealCount,
            UnitName = x.UnitName,
            Notes = x.Notes
        }).ToList();
    }

    private static MealDefinitionDto ToDto(MealDefinition item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        MealType = item.MealType,
        StructureType = item.StructureType,
        ProductId = item.ProductId,
        ProductSkuId = item.ProductSkuId,
        ProductPackageId = item.ProductPackageId,
        ProductSkuName = item.ProductSkuName,
        ProductSkuNameEng = item.ProductSkuNameEng,
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
            IsPackagingRequired = item.IsPackagingRequired,
            DefaultSourceWarehouseId = item.DefaultSourceWarehouseId,
            DefaultPackagingWarehouseId = item.DefaultPackagingWarehouseId,
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

    private static CateringDailyScheduleDto ToDto(CateringDailySchedule item, IReadOnlyDictionary<Guid, CateringSquareDto> squares, CateringPackagingPlan? packagingPlan = null, CateringDispatchPlan? dispatchPlan = null, IEnumerable<CateringExecutionEvent>? executionEvents = null) => new()
    {
        Id = item.Id,
        CateringOperationalPlanId = item.CateringOperationalPlanId,
        CateringProjectId = item.CateringProjectId,
        CateringProjectDailyPlanId = item.CateringProjectDailyPlanId,
        CateringContractId = item.CateringContractId,
        ServiceDate = item.ServiceDate,
        PlannedQuantity = item.PlannedQuantity,
        ReceivedQuantity = item.Allocations.Sum(x => x.ReceivedQuantity),
        DistributedQuantity = item.Allocations.Sum(x => x.DistributedQuantity),
        Status = item.Status,
        PlannedPackagingStartTime = item.PlannedPackagingStartTime,
        PlannedPackagingEndTime = item.PlannedPackagingEndTime,
        PlannedLoadTime = item.PlannedLoadTime,
        PlannedDepartureTime = item.PlannedDepartureTime,
        PlannedArrivalTime = item.PlannedArrivalTime,
        AllocationPlannedQuantity = item.Allocations.Sum(x => x.PlannedQuantity),
        Notes = item.Notes,
        Allocations = item.Allocations.Select(x => ToDto(x, squares)).ToList(),
        PackagingPlan = packagingPlan is null ? null : ToDto(packagingPlan),
        DispatchPlan = dispatchPlan is null ? null : ToDto(dispatchPlan),
        ExecutionEvents = executionEvents?.Select(ToDto).ToList() ?? []
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
            PlannedArrivalTime = item.PlannedArrivalTime,
            ActualArrivalTime = item.ActualArrivalTime,
            ReceivingSupervisorEmployeeId = item.ReceivingSupervisorEmployeeId,
            ReceivingSupervisorName = item.ReceivingSupervisorName,
            TeamLeaderEmployeeId = item.TeamLeaderEmployeeId,
            TeamLeaderName = item.TeamLeaderName,
            VarianceQuantity = item.DistributedQuantity - item.PlannedQuantity,
            VarianceNotes = item.VarianceNotes
        };
    }

    private static CateringPackagingPlanDto ToDto(CateringPackagingPlan item) => new()
    {
        Id = item.Id,
        DailyScheduleId = item.DailyScheduleId,
        IsPackagingRequired = item.IsPackagingRequired,
        SourceWarehouseId = item.SourceWarehouseId,
        PackagingWarehouseId = item.PackagingWarehouseId,
        RequiredMealCount = item.RequiredMealCount,
        StockReleasedMealCount = item.StockReleasedMealCount,
        PreparedMealCount = item.PreparedMealCount,
        RejectedMealCount = item.RejectedMealCount,
        DamagedMealCount = item.DamagedMealCount,
        Status = item.Status,
        StockReleasedAt = item.StockReleasedAt,
        PreparationStartedAt = item.PreparationStartedAt,
        PreparationCompletedAt = item.PreparationCompletedAt,
        InventoryReferenceIdsCsv = item.InventoryReferenceIdsCsv,
        VarianceReason = item.VarianceReason,
        Notes = item.Notes
    };

    private static CateringDispatchPlanDto ToDto(CateringDispatchPlan item) => new()
    {
        Id = item.Id,
        DailyScheduleId = item.DailyScheduleId,
        VehicleId = item.VehicleId,
        VehicleName = item.VehicleName,
        PlateNumber = item.PlateNumber,
        DriverEmployeeId = item.DriverEmployeeId,
        DriverName = item.DriverName,
        FleetAssignmentId = item.FleetAssignmentId,
        IsFleetAssignmentManagedByCatering = item.IsFleetAssignmentManagedByCatering,
        LoadedMealCount = item.LoadedMealCount,
        Status = item.Status,
        PlannedLoadTime = item.PlannedLoadTime,
        PlannedDepartureTime = item.PlannedDepartureTime,
        PlannedArrivalTime = item.PlannedArrivalTime,
        TruckArrivedForLoadingAt = item.TruckArrivedForLoadingAt,
        LoadedAt = item.LoadedAt,
        DepartedAt = item.DepartedAt,
        ArrivedAtDistributionAt = item.ArrivedAtDistributionAt,
        CompletedAt = item.CompletedAt,
        Notes = item.Notes
    };

    private static CateringExecutionEventDto ToDto(CateringExecutionEvent item) => new()
    {
        Id = item.Id,
        DailyScheduleId = item.DailyScheduleId,
        AllocationId = item.AllocationId,
        DispatchPlanId = item.DispatchPlanId,
        EventType = item.EventType,
        OccurredAt = item.OccurredAt,
        Quantity = item.Quantity,
        EmployeeId = item.EmployeeId,
        EmployeeName = item.EmployeeName,
        LocationText = item.LocationText,
        Notes = item.Notes
    };

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

    private static CateringOperationalPlanDto ToDto(CateringOperationalPlan item, IReadOnlyDictionary<Guid, CateringContractDto> contracts, IReadOnlyDictionary<Guid, CateringSquareDto> squares)
    {
        contracts.TryGetValue(item.CateringContractId, out var contract);
        return new CateringOperationalPlanDto
        {
            Id = item.Id,
            CompanyId = item.CompanyId,
            BranchId = item.BranchId,
            CateringContractId = item.CateringContractId,
            ContractNumber = contract?.Number,
            CustomerName = contract?.CustomerName,
            CustomerNameEng = contract?.CustomerNameEng,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Status = item.Status,
            Notes = item.Notes,
            Resources = item.Resources.Select(x => ToDto(x, squares)).ToList()
        };
    }

    private static CateringProjectDto ToDto(
        CateringProject item,
        IReadOnlyDictionary<Guid, CateringContractDto> contracts,
        IReadOnlyDictionary<Guid, CateringSquareDto> squares,
        IEnumerable<CateringPlanResourceAssignment> resources,
        IReadOnlyDictionary<Guid, CateringSquareDto> resourceSquares,
        IEnumerable<CateringDailySchedule> schedules,
        IReadOnlyDictionary<Guid, CateringSquareDto> scheduleSquares)
    {
        var scheduleList = schedules.ToList();
        var dailyPlans = item.DailyPlans.OrderBy(x => x.ServiceDate).Select(day =>
        {
            var childSchedules = scheduleList.Where(x => x.CateringProjectDailyPlanId == day.Id).ToList();
            return new CateringProjectDailyPlanDto
            {
                Id = day.Id,
                CateringProjectId = day.CateringProjectId,
                ServiceDate = day.ServiceDate,
                PlannedQuantity = day.PlannedQuantity,
                ScheduledQuantity = childSchedules.Sum(x => x.PlannedQuantity),
                IsExtensionDay = day.ServiceDate.Date > item.EndDate.Date,
                Status = day.Status,
                Notes = day.Notes,
                ContractSchedules = childSchedules.Select(x => ToDto(x, scheduleSquares)).ToList()
            };
        }).ToList();

        return new CateringProjectDto
        {
            Id = item.Id,
            CompanyId = item.CompanyId,
            BranchId = item.BranchId,
            ProjectName = item.ProjectName,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Status = item.Status,
            Notes = item.Notes,
            ContractCount = item.Contracts.Count,
            PlannedDays = dailyPlans.Count,
            ScheduledDays = scheduleList.Select(x => x.ServiceDate.Date).Distinct().Count(),
            TotalPlannedMeals = dailyPlans.Sum(x => x.PlannedQuantity),
            IsExtended = dailyPlans.Any(x => x.IsExtensionDay) || scheduleList.Any(x => x.ServiceDate.Date > item.EndDate.Date),
            Contracts = item.Contracts.Select(link =>
            {
                contracts.TryGetValue(link.CateringContractId, out var contract);
                return new CateringProjectContractLinkDto
                {
                    Id = link.Id,
                    CateringProjectId = link.CateringProjectId,
                    CateringContractId = link.CateringContractId,
                    ContractNumber = contract?.Number,
                    CustomerName = contract?.CustomerName,
                    CustomerNameEng = contract?.CustomerNameEng,
                    ContractStartDate = contract?.StartDate ?? default,
                    ContractEndDate = contract?.EndDate ?? default,
                    TotalContractedMealQuantity = contract?.TotalContractedMealQuantity ?? 0m
                };
            }).ToList(),
            Squares = item.Squares.Select(scope =>
            {
                squares.TryGetValue(scope.SquareId, out var square);
                return new CateringProjectSquareScopeDto
                {
                    Id = scope.Id,
                    CateringProjectId = scope.CateringProjectId,
                    SquareId = scope.SquareId,
                    SquareName = square?.Name,
                    SquareNameEng = square?.NameEng
                };
            }).ToList(),
            DailyPlans = dailyPlans,
            Resources = resources.Select(x => ToDto(x, resourceSquares)).ToList()
        };
    }

    private static CateringPlanResourceAssignmentDto ToDto(CateringPlanResourceAssignment item, IReadOnlyDictionary<Guid, CateringSquareDto> squares)
    {
        CateringSquareDto? square = null;
        if (item.SquareId.HasValue) squares.TryGetValue(item.SquareId.Value, out square);
        return new CateringPlanResourceAssignmentDto
        {
            Id = item.Id,
            CateringOperationalPlanId = item.CateringOperationalPlanId,
            ResourceType = item.ResourceType,
            EmployeeId = item.EmployeeId,
            EmployeeName = item.EmployeeName,
            VehicleId = item.VehicleId,
            VehicleName = item.VehicleName,
            PlateNumber = item.PlateNumber,
            SquareId = item.SquareId,
            SquareName = square?.Name,
            EffectiveFrom = item.EffectiveFrom,
            EffectiveTo = item.EffectiveTo,
            Notes = item.Notes
        };
    }

    private static CateringInventoryRequestDto ToDto(CateringInventoryRequest item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        BranchId = item.BranchId,
        CateringOperationalPlanId = item.CateringOperationalPlanId,
        DailyScheduleId = item.DailyScheduleId,
        PackagingPlanId = item.PackagingPlanId,
        SourceWarehouseId = item.SourceWarehouseId,
        RequestedByEmployeeId = item.RequestedByEmployeeId,
        RequestedByEmployeeName = item.RequestedByEmployeeName,
        RequestDate = item.RequestDate,
        PlannedMealCount = item.PlannedMealCount,
        Status = item.Status,
        InventoryReferenceIdsCsv = item.InventoryReferenceIdsCsv,
        Notes = item.Notes,
        Lines = item.Lines.Select(ToDto).ToList()
    };

    private static CateringInventoryRequestLineDto ToDto(CateringInventoryRequestLine item) => new()
    {
        Id = item.Id,
        CateringInventoryRequestId = item.CateringInventoryRequestId,
        ProductId = item.ProductId,
        ProductSkuId = item.ProductSkuId,
        ProductPackageId = item.ProductPackageId,
        ProductSkuName = item.ProductSkuName,
        ProductSkuNameEng = item.ProductSkuNameEng,
        QuantityPerMeal = item.QuantityPerMeal,
        RequiredQuantity = item.RequiredQuantity,
        ApprovedQuantity = item.ApprovedQuantity,
        UnitName = item.UnitName,
        Notes = item.Notes
    };

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
    public Guid? ProjectId { get; set; }
    public Guid? ProjectDailyPlanId { get; set; }
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

public class PlanListQuery : CompanyQuery
{
    public Guid? ContractId { get; set; }
}

public class InventoryRequestListQuery : CompanyQuery
{
    public Guid? PlanId { get; set; }
    public Guid? ScheduleId { get; set; }
    public CateringInventoryRequestStatus? Status { get; set; }
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
