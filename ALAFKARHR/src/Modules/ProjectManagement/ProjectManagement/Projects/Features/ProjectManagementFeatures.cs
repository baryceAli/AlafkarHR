namespace ProjectManagement.Projects.Features;

public record GetProjectsQuery(Guid? CompanyId, string? SearchText, ProjectStatus? Status, PaginationRequest Pagination) : IQuery<GetProjectsResult>;
public record GetProjectsResult(PaginatedResult<ProjectDto> Projects);
public record GetProjectsResponse(PaginatedResult<ProjectDto> Projects);

public record GetProjectByIdQuery(Guid Id) : IQuery<GetProjectByIdResult>;
public record GetProjectByIdResult(ProjectDto Project);
public record GetProjectByIdResponse(ProjectDto Project);

public record CreateProjectRequest(ProjectDto Project);
public record CreateProjectCommand(ProjectDto Project) : ICommand<CreateProjectResult>;
public record CreateProjectResult(Guid Id, string ProjectNumber);

public record UpdateProjectRequest(ProjectDto Project);
public record UpdateProjectCommand(Guid Id, ProjectDto Project) : ICommand<UpdateProjectResult>;
public record UpdateProjectResult(bool IsSuccess);

public record DeleteProjectCommand(Guid Id) : ICommand<DeleteProjectResult>;
public record DeleteProjectResult(bool IsSuccess);

public record ChangeProjectStatusRequest(ProjectStatus Status);
public record ChangeProjectStatusCommand(Guid Id, ProjectStatus Status) : ICommand<UpdateProjectResult>;

public record CreateProjectCustomerRequest(ProjectCustomerDto Customer);
public record CreateProjectCustomerCommand(Guid ProjectId, ProjectCustomerDto Customer) : ICommand<CreateEntityResult>;

public record CreateProjectCustomerProductPlanRequest(ProjectCustomerProductPlanDto ProductPlan);
public record CreateProjectCustomerProductPlanCommand(Guid ProjectId, Guid ProjectCustomerId, ProjectCustomerProductPlanDto ProductPlan) : ICommand<CreateEntityResult>;

public record CreateProjectDeliverableRequest(ProjectDeliverableDto Deliverable);
public record CreateProjectDeliverableCommand(Guid ProjectId, ProjectDeliverableDto Deliverable) : ICommand<CreateEntityResult>;

public record GenerateMaterialRequirementsCommand(Guid ProjectId, Guid DeliverableId) : ICommand<GenerateMaterialRequirementsResult>;
public record GenerateMaterialRequirementsResult(List<ProjectMaterialRequirementDto> Requirements);
public record GetProjectMaterialRequirementsQuery(Guid ProjectId) : IQuery<GetProjectMaterialRequirementsResult>;
public record GetProjectMaterialRequirementsResult(List<ProjectMaterialRequirementDto> Requirements);
public record GetProjectMaterialRequirementsResponse(List<ProjectMaterialRequirementDto> Requirements);

public record GetDistributionPlacesQuery(Guid? CompanyId, string? SearchText, PaginationRequest Pagination) : IQuery<GetDistributionPlacesResult>;
public record GetDistributionPlacesResult(PaginatedResult<DistributionPlaceDto> Places);
public record GetDistributionPlacesResponse(PaginatedResult<DistributionPlaceDto> Places);

public record SaveDistributionPlaceRequest(DistributionPlaceDto Place);
public record CreateDistributionPlaceCommand(DistributionPlaceDto Place) : ICommand<CreateEntityResult>;
public record UpdateDistributionPlaceCommand(Guid Id, DistributionPlaceDto Place) : ICommand<UpdateProjectResult>;

public record CreateDistributionScheduleRequest(ProjectDistributionScheduleDto Schedule);
public record CreateDistributionScheduleCommand(Guid ProjectId, ProjectDistributionScheduleDto Schedule) : ICommand<CreateEntityResult>;

public record CreateDistributionAllocationRequest(ProjectDistributionAllocationDto Allocation);
public record CreateDistributionAllocationCommand(Guid ProjectId, Guid ScheduleId, ProjectDistributionAllocationDto Allocation) : ICommand<CreateEntityResult>;

public record RecordAllocationActualsRequest(decimal ShippedQuantity, decimal DeliveredQuantity, decimal ActualQuantity, string? Notes);
public record RecordAllocationActualsCommand(Guid AllocationId, decimal ShippedQuantity, decimal DeliveredQuantity, decimal ActualQuantity, string? Notes) : ICommand<UpdateProjectResult>;

public record CreateProjectResourceRequest(ProjectResourceDto Resource);
public record CreateProjectResourceCommand(Guid ProjectId, ProjectResourceDto Resource) : ICommand<CreateEntityResult>;

public record CreateProjectExpenseRequest(ProjectExpenseDto Expense);
public record CreateProjectExpenseCommand(Guid ProjectId, ProjectExpenseDto Expense) : ICommand<CreateEntityResult>;

public record PostProjectHandoffRequest(ProjectHandoffDto Handoff);
public record PostProjectHandoffCommand(Guid ProjectId, ProjectHandoffDto Handoff) : ICommand<CreateEntityResult>;
public record GetProjectHandoffsQuery(Guid ProjectId) : IQuery<GetProjectHandoffsResult>;
public record GetProjectHandoffsResult(List<ProjectHandoffDto> Handoffs);
public record GetProjectHandoffsResponse(List<ProjectHandoffDto> Handoffs);

public record CreateProjectTaskLinkRequest(ProjectTaskLinkDto TaskLink);
public record CreateProjectTaskLinkCommand(Guid ProjectId, ProjectTaskLinkDto TaskLink) : ICommand<CreateEntityResult>;
public record GetProjectTaskLinksQuery(Guid ProjectId) : IQuery<GetProjectTaskLinksResult>;
public record GetProjectTaskLinksResult(List<ProjectTaskLinkDto> TaskLinks);
public record GetProjectTaskLinksResponse(List<ProjectTaskLinkDto> TaskLinks);

public record GetBudgetSummaryQuery(Guid ProjectId) : IQuery<GetBudgetSummaryResult>;
public record GetBudgetSummaryResult(ProjectBudgetSummaryDto Budget);
public record GetBudgetSummaryResponse(ProjectBudgetSummaryDto Budget);

public record GetDashboardQuery(Guid? CompanyId) : IQuery<GetDashboardResult>;
public record ProjectDashboardDto(int ActiveProjects, int ScheduledDistributions, decimal PlannedMeals, decimal ActualMeals, decimal PlannedCost, decimal ActualCost);
public record GetDashboardResult(ProjectDashboardDto Dashboard);
public record GetDashboardResponse(ProjectDashboardDto Dashboard);

public record GetDistributionReportQuery(Guid? CompanyId, Guid? ProjectId, Guid? CustomerId, Guid? PlaceId, Guid? DeliverableId, DateTime? FromDate, DateTime? ToDate, ProjectReportGroupBy GroupBy)
    : IQuery<GetDistributionReportResult>;
public record GetDistributionReportResult(List<ProjectDistributionReportRowDto> Report);
public record GetDistributionReportResponse(List<ProjectDistributionReportRowDto> Report);

public record GetPlannedProductDemandQuery(Guid? CompanyId, Guid? ProjectId, Guid? CustomerId, Guid? ProductSkuId, DateTime? FromDate, DateTime? ToDate, ProjectReportGroupBy GroupBy)
    : IQuery<GetPlannedProductDemandResult>;
public record GetPlannedProductDemandResult(List<PlannedProductDemandRowDto> Report);
public record GetPlannedProductDemandResponse(List<PlannedProductDemandRowDto> Report);

public record CreateEntityResult(Guid Id);

public class ProjectManagementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/projectmanagement");

        group.MapGet("/dashboard", async ([AsParameters] GuidQuery query, ISender sender) =>
        {
            var result = await sender.Send(new GetDashboardQuery(query.CompanyId));
            return Results.Ok(result.Adapt<GetDashboardResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.View);

        group.MapGet("/projects", async ([AsParameters] ProjectListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectsQuery(query.CompanyId, pagination.SearchText, query.Status, pagination));
            return Results.Ok(result.Adapt<GetProjectsResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.View);

        group.MapGet("/projects/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectByIdQuery(id));
            return Results.Ok(result.Adapt<GetProjectByIdResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.View);

        group.MapPost("/projects", async (CreateProjectRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectCommand(request.Project));
            return Results.Created($"/api/v1/projectmanagement/projects/{result.Id}", result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Create);

        group.MapPut("/projects/{id:guid}", async (Guid id, UpdateProjectRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateProjectCommand(id, request.Project));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Edit);

        group.MapDelete("/projects/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProjectCommand(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Delete);

        group.MapPut("/projects/{id:guid}/status", async (Guid id, ChangeProjectStatusRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ChangeProjectStatusCommand(id, request.Status));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Plan);

        group.MapPost("/projects/{id:guid}/customers", async (Guid id, CreateProjectCustomerRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectCustomerCommand(id, request.Customer));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Edit);

        group.MapPost("/projects/{projectId:guid}/customers/{customerId:guid}/product-plans", async (Guid projectId, Guid customerId, CreateProjectCustomerProductPlanRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectCustomerProductPlanCommand(projectId, customerId, request.ProductPlan));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Edit);

        group.MapPost("/projects/{id:guid}/deliverables", async (Guid id, CreateProjectDeliverableRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectDeliverableCommand(id, request.Deliverable));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Plan);

        group.MapPost("/projects/{id:guid}/deliverables/{deliverableId:guid}/generate-material-requirements", async (Guid id, Guid deliverableId, ISender sender) =>
        {
            var result = await sender.Send(new GenerateMaterialRequirementsCommand(id, deliverableId));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Inventory);

        group.MapGet("/projects/{id:guid}/material-requirements", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectMaterialRequirementsQuery(id));
            return Results.Ok(result.Adapt<GetProjectMaterialRequirementsResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Inventory);

        group.MapGet("/distribution-places", async ([AsParameters] PlaceListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
        {
            var result = await sender.Send(new GetDistributionPlacesQuery(query.CompanyId, pagination.SearchText, pagination));
            return Results.Ok(result.Adapt<GetDistributionPlacesResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPost("/distribution-places", async (SaveDistributionPlaceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateDistributionPlaceCommand(request.Place));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPut("/distribution-places/{id:guid}", async (Guid id, SaveDistributionPlaceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateDistributionPlaceCommand(id, request.Place));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPost("/projects/{id:guid}/distribution-schedules", async (Guid id, CreateDistributionScheduleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateDistributionScheduleCommand(id, request.Schedule));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPost("/projects/{projectId:guid}/distribution-schedules/{scheduleId:guid}/allocations", async (Guid projectId, Guid scheduleId, CreateDistributionAllocationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateDistributionAllocationCommand(projectId, scheduleId, request.Allocation));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPut("/distribution-allocations/{allocationId:guid}/actuals", async (Guid allocationId, RecordAllocationActualsRequest request, ISender sender) =>
        {
            var result = await sender.Send(new RecordAllocationActualsCommand(allocationId, request.ShippedQuantity, request.DeliveredQuantity, request.ActualQuantity, request.Notes));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Distribution);

        group.MapPost("/projects/{id:guid}/resources", async (Guid id, CreateProjectResourceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectResourceCommand(id, request.Resource));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Budget);

        group.MapPost("/projects/{id:guid}/expenses", async (Guid id, CreateProjectExpenseRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectExpenseCommand(id, request.Expense));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Budget);

        group.MapPost("/projects/{id:guid}/handoffs", async (Guid id, PostProjectHandoffRequest request, ISender sender) =>
        {
            var result = await sender.Send(new PostProjectHandoffCommand(id, request.Handoff));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Inventory);

        group.MapGet("/projects/{id:guid}/handoffs", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectHandoffsQuery(id));
            return Results.Ok(result.Adapt<GetProjectHandoffsResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Inventory);

        group.MapPost("/projects/{id:guid}/task-links", async (Guid id, CreateProjectTaskLinkRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProjectTaskLinkCommand(id, request.TaskLink));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Tasks);

        group.MapGet("/projects/{id:guid}/task-links", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectTaskLinksQuery(id));
            return Results.Ok(result.Adapt<GetProjectTaskLinksResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Tasks);

        group.MapGet("/projects/{id:guid}/budget-summary", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBudgetSummaryQuery(id));
            return Results.Ok(result.Adapt<GetBudgetSummaryResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.Budget);

        group.MapGet("/reports/customer-distribution", async ([AsParameters] DistributionReportQuery query, ISender sender) =>
        {
            var result = await sender.Send(new GetDistributionReportQuery(query.CompanyId, query.ProjectId, query.CustomerId, query.PlaceId, query.DeliverableId, query.FromDate, query.ToDate, query.GroupBy));
            return Results.Ok(result.Adapt<GetDistributionReportResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.ViewReports);

        group.MapGet("/reports/planned-product-demand", async ([AsParameters] PlannedProductDemandQuery query, ISender sender) =>
        {
            var result = await sender.Send(new GetPlannedProductDemandQuery(query.CompanyId, query.ProjectId, query.CustomerId, query.ProductSkuId, query.FromDate, query.ToDate, query.GroupBy));
            return Results.Ok(result.Adapt<GetPlannedProductDemandResponse>());
        }).RequireAuthorization(PermissionList.ProjectManagementPermissions.ViewReports);
    }
}

public class ProjectManagementHandlers(ProjectManagementDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetProjectsQuery, GetProjectsResult>,
    IQueryHandler<GetProjectByIdQuery, GetProjectByIdResult>,
    ICommandHandler<CreateProjectCommand, CreateProjectResult>,
    ICommandHandler<UpdateProjectCommand, UpdateProjectResult>,
    ICommandHandler<DeleteProjectCommand, DeleteProjectResult>,
    ICommandHandler<ChangeProjectStatusCommand, UpdateProjectResult>,
    ICommandHandler<CreateProjectCustomerCommand, CreateEntityResult>,
    ICommandHandler<CreateProjectCustomerProductPlanCommand, CreateEntityResult>,
    ICommandHandler<CreateProjectDeliverableCommand, CreateEntityResult>,
    ICommandHandler<GenerateMaterialRequirementsCommand, GenerateMaterialRequirementsResult>,
    IQueryHandler<GetProjectMaterialRequirementsQuery, GetProjectMaterialRequirementsResult>,
    IQueryHandler<GetDistributionPlacesQuery, GetDistributionPlacesResult>,
    ICommandHandler<CreateDistributionPlaceCommand, CreateEntityResult>,
    ICommandHandler<UpdateDistributionPlaceCommand, UpdateProjectResult>,
    ICommandHandler<CreateDistributionScheduleCommand, CreateEntityResult>,
    ICommandHandler<CreateDistributionAllocationCommand, CreateEntityResult>,
    ICommandHandler<RecordAllocationActualsCommand, UpdateProjectResult>,
    ICommandHandler<CreateProjectResourceCommand, CreateEntityResult>,
    ICommandHandler<CreateProjectExpenseCommand, CreateEntityResult>,
    ICommandHandler<PostProjectHandoffCommand, CreateEntityResult>,
    IQueryHandler<GetProjectHandoffsQuery, GetProjectHandoffsResult>,
    ICommandHandler<CreateProjectTaskLinkCommand, CreateEntityResult>,
    IQueryHandler<GetProjectTaskLinksQuery, GetProjectTaskLinksResult>,
    IQueryHandler<GetBudgetSummaryQuery, GetBudgetSummaryResult>,
    IQueryHandler<GetDashboardQuery, GetDashboardResult>,
    IQueryHandler<GetDistributionReportQuery, GetDistributionReportResult>,
    IQueryHandler<GetPlannedProductDemandQuery, GetPlannedProductDemandResult>
{
    public async Task<GetProjectsResult> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Projects.AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.ProjectNumber.Contains(search) || x.Name.Contains(search) || x.NameEng.Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(request.Pagination.PageIndex * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new GetProjectsResult(new PaginatedResult<ProjectDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<GetProjectByIdResult> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await ProjectQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project not found: {request.Id}");

        return new GetProjectByIdResult(ToDto(project));
    }

    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var projectNumber = await NextProjectNumberAsync(request.Project.CompanyId, cancellationToken);
        var project = Project.Create(request.Project, projectNumber, UserId());
        await dbContext.Projects.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProjectResult(project.Id, project.ProjectNumber);
    }

    public async Task<UpdateProjectResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project not found: {request.Id}");
        project.Update(request.Project, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateProjectResult(true);
    }

    public async Task<DeleteProjectResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project not found: {request.Id}");
        project.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteProjectResult(true);
    }

    public async Task<UpdateProjectResult> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project not found: {request.Id}");
        project.ChangeStatus(request.Status, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateProjectResult(true);
    }

    public async Task<CreateEntityResult> Handle(CreateProjectCustomerCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var customer = ProjectCustomer.Create(request.ProjectId, request.Customer, UserId());
        await dbContext.ProjectCustomers.AddAsync(customer, cancellationToken);
        var productPlans = request.Customer.ProductPlans
            .Select(plan => ProjectCustomerProductPlan.Create(request.ProjectId, customer.Id, plan, UserId()))
            .ToList();
        await dbContext.ProjectCustomerProductPlans.AddRangeAsync(productPlans, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(customer.Id);
    }

    public async Task<CreateEntityResult> Handle(CreateProjectCustomerProductPlanCommand request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.ProjectCustomers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ProjectCustomerId && x.ProjectId == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Project customer not found: {request.ProjectCustomerId}");

        var productPlan = ProjectCustomerProductPlan.Create(customer.ProjectId, customer.Id, request.ProductPlan, UserId());
        await dbContext.ProjectCustomerProductPlans.AddAsync(productPlan, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(productPlan.Id);
    }

    public async Task<CreateEntityResult> Handle(CreateProjectDeliverableCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var deliverable = ProjectDeliverable.Create(request.ProjectId, request.Deliverable, UserId());
        await dbContext.ProjectDeliverables.AddAsync(deliverable, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(deliverable.Id);
    }

    public async Task<GenerateMaterialRequirementsResult> Handle(GenerateMaterialRequirementsCommand request, CancellationToken cancellationToken)
    {
        var deliverable = await dbContext.ProjectDeliverables.FirstOrDefaultAsync(x => x.Id == request.DeliverableId && x.ProjectId == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Deliverable not found: {request.DeliverableId}");

        var sku = await sender.Send(new GetProductSkuByIdQuery(deliverable.ProductSkuId), cancellationToken);
        var existing = await dbContext.ProjectMaterialRequirements
            .Where(x => x.ProjectId == request.ProjectId && x.DeliverableId == request.DeliverableId)
            .ToListAsync(cancellationToken);

        foreach (var item in existing)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = UserId();
        }

        var requirements = sku.ProductSku.Components
            .Select(component => ProjectMaterialRequirement.Create(
                request.ProjectId,
                request.DeliverableId,
                component.ComponentProductSkuId,
                component.ComponentSkuName ?? component.ComponentSkuCode ?? "Component",
                component.ComponentSkuNameEng,
                component.Quantity * deliverable.PlannedQuantity,
                UserId()))
            .ToList();

        await dbContext.ProjectMaterialRequirements.AddRangeAsync(requirements, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new GenerateMaterialRequirementsResult(requirements.Select(ToDto).ToList());
    }

    public async Task<GetProjectMaterialRequirementsResult> Handle(GetProjectMaterialRequirementsQuery request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var requirements = await dbContext.ProjectMaterialRequirements
            .AsNoTracking()
            .Where(x => x.ProjectId == request.ProjectId)
            .OrderBy(x => x.ComponentSkuName)
            .ToListAsync(cancellationToken);
        return new GetProjectMaterialRequirementsResult(requirements.Select(ToDto).ToList());
    }

    public async Task<GetDistributionPlacesResult> Handle(GetDistributionPlacesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.DistributionPlaces.AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.NameEng.Contains(search) || (x.City != null && x.City.Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query
            .OrderBy(x => x.Name)
            .Skip(request.Pagination.PageIndex * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new GetDistributionPlacesResult(new PaginatedResult<DistributionPlaceDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<CreateEntityResult> Handle(CreateDistributionPlaceCommand request, CancellationToken cancellationToken)
    {
        var place = DistributionPlace.Create(request.Place, UserId());
        await dbContext.DistributionPlaces.AddAsync(place, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(place.Id);
    }

    public async Task<UpdateProjectResult> Handle(UpdateDistributionPlaceCommand request, CancellationToken cancellationToken)
    {
        var place = await dbContext.DistributionPlaces.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Distribution place not found: {request.Id}");
        place.Update(request.Place, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateProjectResult(true);
    }

    public async Task<CreateEntityResult> Handle(CreateDistributionScheduleCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var schedule = ProjectDistributionSchedule.Create(request.ProjectId, request.Schedule, UserId());
        await dbContext.ProjectDistributionSchedules.AddAsync(schedule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(schedule.Id);
    }

    public async Task<CreateEntityResult> Handle(CreateDistributionAllocationCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.ProjectDistributionSchedules.FirstOrDefaultAsync(x => x.Id == request.ScheduleId && x.ProjectId == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Distribution schedule not found: {request.ScheduleId}");

        var customer = await dbContext.ProjectCustomers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Allocation.ProjectCustomerId, cancellationToken)
            ?? throw new NotFoundException($"Project customer not found: {request.Allocation.ProjectCustomerId}");
        var deliverable = await dbContext.ProjectDeliverables.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Allocation.DeliverableId, cancellationToken)
            ?? throw new NotFoundException($"Deliverable not found: {request.Allocation.DeliverableId}");
        var place = await dbContext.DistributionPlaces.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Allocation.DistributionPlaceId, cancellationToken)
            ?? throw new NotFoundException($"Distribution place not found: {request.Allocation.DistributionPlaceId}");

        request.Allocation.CustomerName = customer.CustomerName;
        request.Allocation.DeliverableName = deliverable.ProductSkuName;
        request.Allocation.PlaceName = place.Name;

        var allocation = ProjectDistributionAllocation.Create(request.ProjectId, request.ScheduleId, schedule.DistributionDate, request.Allocation, UserId());
        await dbContext.ProjectDistributionAllocations.AddAsync(allocation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(allocation.Id);
    }

    public async Task<UpdateProjectResult> Handle(RecordAllocationActualsCommand request, CancellationToken cancellationToken)
    {
        var allocation = await dbContext.ProjectDistributionAllocations.FirstOrDefaultAsync(x => x.Id == request.AllocationId, cancellationToken)
            ?? throw new NotFoundException($"Distribution allocation not found: {request.AllocationId}");
        allocation.RecordActuals(request.ShippedQuantity, request.DeliveredQuantity, request.ActualQuantity, request.Notes, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateProjectResult(true);
    }

    public async Task<CreateEntityResult> Handle(CreateProjectResourceCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var resource = ProjectResource.Create(request.ProjectId, request.Resource, UserId());
        await dbContext.ProjectResources.AddAsync(resource, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(resource.Id);
    }

    public async Task<CreateEntityResult> Handle(CreateProjectExpenseCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var expense = ProjectExpense.Create(request.ProjectId, request.Expense, UserId());
        await dbContext.ProjectExpenses.AddAsync(expense, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(expense.Id);
    }

    public async Task<CreateEntityResult> Handle(PostProjectHandoffCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var referenceNumber = string.IsNullOrWhiteSpace(request.Handoff.ReferenceNumber)
            ? $"PM-{request.Handoff.HandoffType}-{DateTime.UtcNow:yyyyMMddHHmmss}"
            : request.Handoff.ReferenceNumber;

        var handoff = ProjectHandoff.Create(request.ProjectId, request.Handoff.HandoffType, referenceNumber, request.Handoff.HandoffDate, request.Handoff.Notes, UserId());
        await dbContext.ProjectHandoffs.AddAsync(handoff, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var lineDto in request.Handoff.Lines)
        {
            var line = ProjectHandoffLine.Create(handoff.Id, lineDto, UserId());
            if (ShouldPostStockOut(request.Handoff.HandoffType) && CanPostInventory(lineDto))
            {
                var result = await sender.Send(new PostInventoryStockOutCommand(
                    lineDto.ProductId!.Value,
                    lineDto.ProductSkuId,
                    lineDto.ProductPackageId,
                    lineDto.WarehouseId!.Value,
                    lineDto.BatchId!.Value,
                    lineDto.Quantity,
                    lineDto.UnitCost,
                    lineDto.TotalCost == 0 ? lineDto.Quantity * lineDto.UnitCost : lineDto.TotalCost,
                    lineDto.CurrencyId!.Value,
                    (await dbContext.Projects.AsNoTracking().Where(x => x.Id == request.ProjectId).Select(x => x.CompanyId).FirstAsync(cancellationToken)),
                    request.Handoff.Notes,
                    referenceNumber,
                    "ProjectManagement",
                    request.Handoff.ConsumeReservedQuantity), cancellationToken);
                line.LinkInventoryMovement(result.InventoryId);
            }
            else if (ShouldPostStockIn(request.Handoff.HandoffType) && CanPostInventory(lineDto))
            {
                var result = await sender.Send(new PostInventoryStockInCommand(
                    lineDto.ProductId!.Value,
                    lineDto.ProductSkuId,
                    lineDto.ProductPackageId,
                    lineDto.WarehouseId!.Value,
                    lineDto.BatchId!.Value,
                    lineDto.Quantity,
                    lineDto.UnitCost,
                    lineDto.TotalCost == 0 ? lineDto.Quantity * lineDto.UnitCost : lineDto.TotalCost,
                    lineDto.CurrencyId!.Value,
                    (await dbContext.Projects.AsNoTracking().Where(x => x.Id == request.ProjectId).Select(x => x.CompanyId).FirstAsync(cancellationToken)),
                    request.Handoff.Notes,
                    referenceNumber,
                    "ProjectManagement"), cancellationToken);
                line.LinkInventoryMovement(result.InventoryId);
            }

            await dbContext.ProjectHandoffLines.AddAsync(line, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(handoff.Id);
    }

    public async Task<GetProjectHandoffsResult> Handle(GetProjectHandoffsQuery request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var handoffs = await dbContext.ProjectHandoffs
            .AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.ProjectId == request.ProjectId)
            .OrderByDescending(x => x.HandoffDate)
            .ToListAsync(cancellationToken);
        return new GetProjectHandoffsResult(handoffs.Select(ToDto).ToList());
    }

    public async Task<CreateEntityResult> Handle(CreateProjectTaskLinkCommand request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var link = ProjectTaskLink.Create(request.ProjectId, request.TaskLink, UserId());
        await dbContext.ProjectTaskLinks.AddAsync(link, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEntityResult(link.Id);
    }

    public async Task<GetProjectTaskLinksResult> Handle(GetProjectTaskLinksQuery request, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(request.ProjectId, cancellationToken);
        var links = await dbContext.ProjectTaskLinks
            .AsNoTracking()
            .Where(x => x.ProjectId == request.ProjectId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return new GetProjectTaskLinksResult(links.Select(ToDto).ToList());
    }

    public async Task<GetBudgetSummaryResult> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var budget = await BuildBudgetSummaryAsync(request.ProjectId, cancellationToken);
        return new GetBudgetSummaryResult(budget);
    }

    public async Task<GetDashboardResult> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var projects = dbContext.Projects.AsNoTracking();
        if (request.CompanyId.HasValue) projects = projects.Where(x => x.CompanyId == request.CompanyId);

        var projectIds = await projects.Select(x => x.Id).ToListAsync(cancellationToken);
        var activeProjects = await projects.CountAsync(x => x.Status == ProjectStatus.Active || x.Status == ProjectStatus.Planned, cancellationToken);
        var scheduledDistributions = await dbContext.ProjectDistributionSchedules.CountAsync(x => projectIds.Contains(x.ProjectId) && x.Status == DistributionStatus.Scheduled, cancellationToken);
        var plannedMeals = await dbContext.ProjectDistributionAllocations.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.PlannedQuantity, cancellationToken);
        var actualMeals = await dbContext.ProjectDistributionAllocations.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.ActualQuantity, cancellationToken);
        var plannedCost = await dbContext.ProjectResources.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.PlannedQuantity * x.PlannedRate, cancellationToken)
            + await dbContext.ProjectExpenses.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.PlannedAmount, cancellationToken);
        var actualCost = await dbContext.ProjectResources.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.ActualQuantity * x.ActualRate, cancellationToken)
            + await dbContext.ProjectExpenses.Where(x => projectIds.Contains(x.ProjectId)).SumAsync(x => x.ActualAmount, cancellationToken);

        return new GetDashboardResult(new ProjectDashboardDto(activeProjects, scheduledDistributions, plannedMeals, actualMeals, plannedCost, actualCost));
    }

    public async Task<GetDistributionReportResult> Handle(GetDistributionReportQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ProjectDistributionAllocations.AsNoTracking().AsQueryable();
        if (request.ProjectId.HasValue) query = query.Where(x => x.ProjectId == request.ProjectId);
        if (request.CustomerId.HasValue) query = query.Where(x => x.ProjectCustomerId == request.CustomerId);
        if (request.PlaceId.HasValue) query = query.Where(x => x.DistributionPlaceId == request.PlaceId);
        if (request.DeliverableId.HasValue) query = query.Where(x => x.DeliverableId == request.DeliverableId);
        if (request.FromDate.HasValue) query = query.Where(x => x.DistributionDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.DistributionDate <= request.ToDate.Value.Date);
        if (request.CompanyId.HasValue)
        {
            var projectIds = await dbContext.Projects.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).Select(x => x.Id).ToListAsync(cancellationToken);
            query = query.Where(x => projectIds.Contains(x.ProjectId));
        }

        var data = await query.ToListAsync(cancellationToken);
        var projectNames = await dbContext.Projects.AsNoTracking()
            .Where(x => data.Select(a => a.ProjectId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);
        var rows = data
            .GroupBy(x => new
            {
                Period = ReportKey(x.DistributionDate, request.GroupBy),
                x.ProjectId,
                x.ProjectCustomerId,
                x.CustomerName,
                x.DistributionPlaceId,
                x.PlaceName,
                x.DeliverableId,
                x.DeliverableName
            })
            .Select(g => new ProjectDistributionReportRowDto
            {
                PeriodKey = g.Key.Period.Key,
                PeriodStart = g.Key.Period.Start,
                PeriodEnd = g.Key.Period.End,
                ProjectId = g.Key.ProjectId,
                ProjectName = projectNames.TryGetValue(g.Key.ProjectId, out var projectName) ? projectName : null,
                ProjectCustomerId = g.Key.ProjectCustomerId,
                CustomerName = g.Key.CustomerName,
                DistributionPlaceId = g.Key.DistributionPlaceId,
                PlaceName = g.Key.PlaceName,
                DeliverableId = g.Key.DeliverableId,
                DeliverableName = g.Key.DeliverableName,
                PlannedQuantity = g.Sum(x => x.PlannedQuantity),
                ShippedQuantity = g.Sum(x => x.ShippedQuantity),
                DeliveredQuantity = g.Sum(x => x.DeliveredQuantity),
                ActualQuantity = g.Sum(x => x.ActualQuantity)
            })
            .OrderBy(x => x.PeriodStart)
            .ToList();

        return new GetDistributionReportResult(rows);
    }

    public async Task<GetPlannedProductDemandResult> Handle(GetPlannedProductDemandQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ProjectDistributionAllocations.AsNoTracking().AsQueryable();
        if (request.ProjectId.HasValue) query = query.Where(x => x.ProjectId == request.ProjectId);
        if (request.CustomerId.HasValue) query = query.Where(x => x.ProjectCustomerId == request.CustomerId);
        if (request.FromDate.HasValue) query = query.Where(x => x.DistributionDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.DistributionDate <= request.ToDate.Value.Date);
        if (request.CompanyId.HasValue)
        {
            var companyProjectIds = await dbContext.Projects.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).Select(x => x.Id).ToListAsync(cancellationToken);
            query = query.Where(x => companyProjectIds.Contains(x.ProjectId));
        }

        var allocations = await query.ToListAsync(cancellationToken);
        var deliverableIds = allocations.Select(x => x.DeliverableId).Distinct().ToList();
        var deliverables = await dbContext.ProjectDeliverables.AsNoTracking()
            .Where(x => deliverableIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (request.ProductSkuId.HasValue)
        {
            allocations = allocations
                .Where(x => deliverables.TryGetValue(x.DeliverableId, out var deliverable) && deliverable.ProductSkuId == request.ProductSkuId)
                .ToList();
        }

        var projectNames = await dbContext.Projects.AsNoTracking()
            .Where(x => allocations.Select(a => a.ProjectId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        var rows = allocations
            .Select(x => new { Allocation = x, Deliverable = deliverables.TryGetValue(x.DeliverableId, out var deliverable) ? deliverable : null })
            .Where(x => x.Deliverable is not null)
            .GroupBy(x => new
            {
                Period = ReportKey(x.Allocation.DistributionDate, request.GroupBy),
                x.Allocation.ProjectId,
                x.Allocation.ProjectCustomerId,
                x.Allocation.CustomerName,
                x.Deliverable!.ProductSkuId,
                x.Deliverable.ProductId,
                x.Deliverable.ProductSkuName,
                x.Deliverable.ProductSkuNameEng
            })
            .Select(g => new PlannedProductDemandRowDto
            {
                PeriodKey = g.Key.Period.Key,
                PeriodStart = g.Key.Period.Start,
                PeriodEnd = g.Key.Period.End,
                ProjectId = g.Key.ProjectId,
                ProjectName = projectNames.TryGetValue(g.Key.ProjectId, out var projectName) ? projectName : null,
                ProjectCustomerId = g.Key.ProjectCustomerId,
                CustomerName = g.Key.CustomerName,
                ProductSkuId = g.Key.ProductSkuId,
                ProductId = g.Key.ProductId,
                ProductSkuName = g.Key.ProductSkuName,
                ProductSkuNameEng = g.Key.ProductSkuNameEng,
                PlannedQuantity = g.Sum(x => x.Allocation.PlannedQuantity)
            })
            .OrderBy(x => x.PeriodStart)
            .ThenBy(x => x.ProductSkuName)
            .ToList();

        return new GetPlannedProductDemandResult(rows);
    }

    private IQueryable<Project> ProjectQuery() =>
        dbContext.Projects
            .Include(x => x.Customers)
                .ThenInclude(x => x.ProductPlans)
            .Include(x => x.Deliverables)
            .Include(x => x.DistributionSchedules)
                .ThenInclude(x => x.Allocations)
            .Include(x => x.Resources)
            .Include(x => x.Expenses)
            .Include(x => x.MaterialRequirements)
            .Include(x => x.Handoffs)
                .ThenInclude(x => x.Lines)
            .Include(x => x.TaskLinks);

    private async Task EnsureProjectExistsAsync(Guid projectId, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == projectId, cancellationToken))
            throw new NotFoundException($"Project not found: {projectId}");
    }

    private async Task<string> NextProjectNumberAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var count = await dbContext.Projects.IgnoreQueryFilters().CountAsync(x => x.CompanyId == companyId, cancellationToken) + 1;
        return $"PM-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }

    private string UserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? "system";

    private static bool CanPostInventory(ProjectHandoffLineDto line) =>
        line.ProductId.HasValue && line.WarehouseId.HasValue && line.BatchId.HasValue && line.CurrencyId.HasValue && line.Quantity > 0;

    private static bool ShouldPostStockOut(ProjectHandoffType type) =>
        type is ProjectHandoffType.MaterialIssueToPreparation or ProjectHandoffType.ShipmentToDistribution or ProjectHandoffType.TransferToShipmentArea;

    private static bool ShouldPostStockIn(ProjectHandoffType type) =>
        type is ProjectHandoffType.PreparedGoodsReceipt or ProjectHandoffType.ReturnOrAdjustment;

    private async Task<ProjectBudgetSummaryDto> BuildBudgetSummaryAsync(Guid projectId, CancellationToken cancellationToken)
    {
        await EnsureProjectExistsAsync(projectId, cancellationToken);
        var resources = await dbContext.ProjectResources.AsNoTracking().Where(x => x.ProjectId == projectId).ToListAsync(cancellationToken);
        var expenses = await dbContext.ProjectExpenses.AsNoTracking().Where(x => x.ProjectId == projectId).ToListAsync(cancellationToken);
        var handoffCost = await dbContext.ProjectHandoffLines.AsNoTracking()
            .Where(x => dbContext.ProjectHandoffs.Where(h => h.ProjectId == projectId).Select(h => h.Id).Contains(x.HandoffId))
            .SumAsync(x => x.TotalCost, cancellationToken);
        var actualQuantity = await dbContext.ProjectDistributionAllocations.AsNoTracking().Where(x => x.ProjectId == projectId).SumAsync(x => x.ActualQuantity, cancellationToken);

        return new ProjectBudgetSummaryDto
        {
            ProjectId = projectId,
            PlannedResourceCost = resources.Sum(x => x.PlannedQuantity * x.PlannedRate),
            ActualResourceCost = resources.Sum(x => x.ActualQuantity * x.ActualRate),
            PlannedExpenseCost = expenses.Sum(x => x.PlannedAmount),
            ActualExpenseCost = expenses.Sum(x => x.ActualAmount),
            InventoryMovementCost = handoffCost,
            TotalDistributedQuantity = actualQuantity
        };
    }

    private static (string Key, DateTime Start, DateTime End) ReportKey(DateTime date, ProjectReportGroupBy groupBy)
    {
        var day = date.Date;
        return groupBy switch
        {
            ProjectReportGroupBy.Week => ($"{day.Year}-W{System.Globalization.ISOWeek.GetWeekOfYear(day):00}", System.Globalization.ISOWeek.ToDateTime(day.Year, System.Globalization.ISOWeek.GetWeekOfYear(day), DayOfWeek.Monday), System.Globalization.ISOWeek.ToDateTime(day.Year, System.Globalization.ISOWeek.GetWeekOfYear(day), DayOfWeek.Sunday)),
            ProjectReportGroupBy.Month => ($"{day.Year}-{day.Month:00}", new DateTime(day.Year, day.Month, 1), new DateTime(day.Year, day.Month, DateTime.DaysInMonth(day.Year, day.Month))),
            ProjectReportGroupBy.Range => ("Range", DateTime.MinValue, DateTime.MaxValue),
            _ => (day.ToString("yyyy-MM-dd"), day, day)
        };
    }

    private static ProjectDto ToDto(Project project) => new()
    {
        Id = project.Id,
        ProjectNumber = project.ProjectNumber,
        Name = project.Name,
        NameEng = project.NameEng,
        CompanyId = project.CompanyId,
        BranchId = project.BranchId,
        Type = project.Type,
        Status = project.Status,
        ManagerUserId = project.ManagerUserId,
        ManagerName = project.ManagerName,
        SourceOrderId = project.SourceOrderId,
        SourceOrderNumber = project.SourceOrderNumber,
        SourceOrderType = project.SourceOrderType,
        PlannedStartDate = project.PlannedStartDate,
        PlannedEndDate = project.PlannedEndDate,
        ActualStartDate = project.ActualStartDate,
        ActualEndDate = project.ActualEndDate,
        Notes = project.Notes,
        Customers = project.Customers.Select(ToDto).ToList(),
        Deliverables = project.Deliverables.Select(ToDto).ToList(),
        DistributionSchedules = project.DistributionSchedules.Select(ToDto).ToList(),
        MaterialRequirements = project.MaterialRequirements.Select(ToDto).ToList(),
        Resources = project.Resources.Select(ToDto).ToList(),
        Expenses = project.Expenses.Select(ToDto).ToList(),
        Handoffs = project.Handoffs.Select(ToDto).ToList(),
        TaskLinks = project.TaskLinks.Select(ToDto).ToList(),
        TotalPlannedCost = project.Resources.Sum(x => x.PlannedQuantity * x.PlannedRate) + project.Expenses.Sum(x => x.PlannedAmount),
        TotalActualCost = project.Resources.Sum(x => x.ActualQuantity * x.ActualRate) + project.Expenses.Sum(x => x.ActualAmount)
    };

    private static ProjectCustomerDto ToDto(ProjectCustomer item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        CustomerId = item.CustomerId,
        CustomerName = item.CustomerName,
        CustomerNameEng = item.CustomerNameEng,
        SourceOrderId = item.SourceOrderId,
        SourceOrderNumber = item.SourceOrderNumber,
        ContractedQuantity = item.ContractedQuantity,
        ContractedAmount = item.ContractedAmount,
        ProductPlans = item.ProductPlans.Select(ToDto).ToList(),
        Notes = item.Notes
    };

    private static ProjectCustomerProductPlanDto ToDto(ProjectCustomerProductPlan item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        ProjectCustomerId = item.ProjectCustomerId,
        ProductSkuId = item.ProductSkuId,
        ProductId = item.ProductId,
        ProductSkuName = item.ProductSkuName,
        ProductSkuNameEng = item.ProductSkuNameEng,
        SkuCode = item.SkuCode,
        SkuCodeEng = item.SkuCodeEng,
        ProductPackageId = item.ProductPackageId,
        PackageName = item.PackageName,
        PackageNameEng = item.PackageNameEng,
        Quantity = item.Quantity,
        Notes = item.Notes
    };

    private static ProjectDeliverableDto ToDto(ProjectDeliverable item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        ProductSkuId = item.ProductSkuId,
        ProductId = item.ProductId,
        ProductSkuName = item.ProductSkuName,
        ProductSkuNameEng = item.ProductSkuNameEng,
        HandlingType = item.HandlingType,
        OrderedQuantity = item.OrderedQuantity,
        PlannedQuantity = item.PlannedQuantity,
        ProducedQuantity = item.ProducedQuantity,
        ShippedQuantity = item.ShippedQuantity,
        Notes = item.Notes
    };

    private static DistributionPlaceDto ToDto(DistributionPlace item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        Name = item.Name,
        NameEng = item.NameEng,
        Address = item.Address,
        City = item.City,
        Latitude = item.Latitude,
        Longitude = item.Longitude,
        ContactName = item.ContactName,
        ContactPhone = item.ContactPhone,
        IsActive = item.IsActive
    };

    private static ProjectDistributionScheduleDto ToDto(ProjectDistributionSchedule item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        DistributionDate = item.DistributionDate,
        WindowStart = item.WindowStart,
        WindowEnd = item.WindowEnd,
        Status = item.Status,
        Notes = item.Notes,
        Allocations = item.Allocations.Select(ToDto).ToList()
    };

    private static ProjectDistributionAllocationDto ToDto(ProjectDistributionAllocation item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        ScheduleId = item.ScheduleId,
        DistributionDate = item.DistributionDate,
        ProjectCustomerId = item.ProjectCustomerId,
        CustomerName = item.CustomerName,
        DeliverableId = item.DeliverableId,
        DeliverableName = item.DeliverableName,
        DistributionPlaceId = item.DistributionPlaceId,
        PlaceName = item.PlaceName,
        PlannedQuantity = item.PlannedQuantity,
        ShippedQuantity = item.ShippedQuantity,
        DeliveredQuantity = item.DeliveredQuantity,
        ActualQuantity = item.ActualQuantity,
        Notes = item.Notes
    };

    private static ProjectMaterialRequirementDto ToDto(ProjectMaterialRequirement item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        DeliverableId = item.DeliverableId,
        ComponentProductSkuId = item.ComponentProductSkuId,
        ComponentSkuName = item.ComponentSkuName,
        ComponentSkuNameEng = item.ComponentSkuNameEng,
        RequiredQuantity = item.RequiredQuantity,
        ReservedQuantity = item.ReservedQuantity,
        IssuedQuantity = item.IssuedQuantity,
        ConsumedQuantity = item.ConsumedQuantity,
        ReturnedQuantity = item.ReturnedQuantity,
        VarianceQuantity = item.VarianceQuantity
    };

    private static ProjectResourceDto ToDto(ProjectResource item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        ResourceType = item.ResourceType,
        ReferenceId = item.ReferenceId,
        Name = item.Name,
        PlannedQuantity = item.PlannedQuantity,
        PlannedRate = item.PlannedRate,
        ActualQuantity = item.ActualQuantity,
        ActualRate = item.ActualRate,
        CurrencyId = item.CurrencyId,
        Notes = item.Notes
    };

    private static ProjectExpenseDto ToDto(ProjectExpense item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        Category = item.Category,
        ExpenseDate = item.ExpenseDate,
        Description = item.Description,
        PlannedAmount = item.PlannedAmount,
        ActualAmount = item.ActualAmount,
        CurrencyId = item.CurrencyId,
        Notes = item.Notes
    };

    private static ProjectHandoffDto ToDto(ProjectHandoff item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        HandoffType = item.HandoffType,
        ReferenceNumber = item.ReferenceNumber,
        HandoffDate = item.HandoffDate,
        Notes = item.Notes,
        Lines = item.Lines.Select(ToDto).ToList()
    };

    private static ProjectHandoffLineDto ToDto(ProjectHandoffLine item) => new()
    {
        Id = item.Id,
        HandoffId = item.HandoffId,
        AllocationId = item.AllocationId,
        ProductId = item.ProductId,
        ProductSkuId = item.ProductSkuId,
        ProductPackageId = item.ProductPackageId,
        ItemName = item.ItemName,
        WarehouseId = item.WarehouseId,
        BatchId = item.BatchId,
        Quantity = item.Quantity,
        UnitCost = item.UnitCost,
        TotalCost = item.TotalCost,
        CurrencyId = item.CurrencyId,
        InventoryMovementId = item.InventoryMovementId
    };

    private static ProjectTaskLinkDto ToDto(ProjectTaskLink item) => new()
    {
        Id = item.Id,
        ProjectId = item.ProjectId,
        TaskId = item.TaskId,
        TaskNumber = item.TaskNumber,
        Title = item.Title
    };
}

public class GuidQuery
{
    public Guid? CompanyId { get; set; }
}

public class ProjectListQuery : GuidQuery
{
    public ProjectStatus? Status { get; set; }
}

public class PlaceListQuery : GuidQuery;

public class DistributionReportQuery : GuidQuery
{
    public Guid? ProjectId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? PlaceId { get; set; }
    public Guid? DeliverableId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public ProjectReportGroupBy GroupBy { get; set; }
}

public class PlannedProductDemandQuery : GuidQuery
{
    public Guid? ProjectId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public ProjectReportGroupBy GroupBy { get; set; }
}
