namespace Fleet.Features;

public record CreateFleetVehicleExpenseRequest(CreateFleetVehicleExpenseDto Expense);
public record UpdateFleetVehicleExpenseRequest(UpdateFleetVehicleExpenseDto Expense);
public record ApproveFleetVehicleExpenseRequest(ApproveFleetVehicleExpenseDto Approval);
public record CreateFleetVehicleExpenseCommand(CreateFleetVehicleExpenseDto Expense) : ICommand<CreateFleetVehicleExpenseResult>;
public record UpdateFleetVehicleExpenseCommand(UpdateFleetVehicleExpenseDto Expense) : ICommand<FleetActionResult>;
public record DeleteFleetVehicleExpenseCommand(Guid Id) : ICommand<FleetActionResult>;
public record SubmitFleetVehicleExpenseCommand(Guid Id) : ICommand<FleetActionResult>;
public record ApproveFleetVehicleExpenseCommand(Guid Id, ApproveFleetVehicleExpenseDto Approval) : ICommand<FleetActionResult>;
public record GetFleetVehicleExpensesQuery(PaginationRequest PaginationRequest, Guid? VehicleId, FleetExpenseCategory? Category, FleetExpenseApprovalStatus? Status, DateTime? FromDate, DateTime? ToDate) : IQuery<GetFleetVehicleExpensesResult>;
public record CreateFleetVehicleExpenseResult(Guid Id);
public record GetFleetVehicleExpensesResult(PaginatedResult<FleetVehicleExpenseDto> Expenses);

public class FleetVehicleExpenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/expenses", async (int PageIndex, int PageSize, string? searchText, Guid? vehicleId, FleetExpenseCategory? category, FleetExpenseApprovalStatus? status, DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetVehicleExpensesQuery(new PaginationRequest(PageIndex, PageSize, searchText), vehicleId, category, status, fromDate, toDate));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicleExpenses")
        .Produces<GetFleetVehicleExpensesResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.View);

        app.MapPost("/api/v1/fleet/expenses", async (CreateFleetVehicleExpenseRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateFleetVehicleExpenseCommand(request.Expense));
            return Results.Created($"/api/v1/fleet/expenses/{result.Id}", result);
        })
        .WithName("CreateFleetVehicleExpense")
        .Produces<CreateFleetVehicleExpenseResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.Create);

        app.MapPut("/api/v1/fleet/expenses", async (UpdateFleetVehicleExpenseRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateFleetVehicleExpenseCommand(request.Expense));
            return Results.Ok(result);
        })
        .WithName("UpdateFleetVehicleExpense")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.Edit);

        app.MapPut("/api/v1/fleet/expenses/{id:guid}/submit", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new SubmitFleetVehicleExpenseCommand(id));
            return Results.Ok(result);
        })
        .WithName("SubmitFleetVehicleExpense")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.Edit);

        app.MapPut("/api/v1/fleet/expenses/{id:guid}/approval", async (Guid id, ApproveFleetVehicleExpenseRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ApproveFleetVehicleExpenseCommand(id, request.Approval));
            return Results.Ok(result);
        })
        .WithName("ApproveFleetVehicleExpense")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.Approve);

        app.MapDelete("/api/v1/fleet/expenses/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteFleetVehicleExpenseCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteFleetVehicleExpense")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleExpensePermissions.Delete);
    }
}

public class GetFleetVehicleExpensesHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetVehicleExpensesQuery, GetFleetVehicleExpensesResult>
{
    public async Task<GetFleetVehicleExpensesResult> Handle(GetFleetVehicleExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.VehicleExpenses.Include(x => x.Vehicle).AsNoTracking().AsQueryable();
        if (request.VehicleId.HasValue)
            query = query.Where(x => x.VehicleId == request.VehicleId.Value);
        if (request.Category.HasValue)
            query = query.Where(x => x.Category == request.Category.Value);
        if (request.Status.HasValue)
            query = query.Where(x => x.ApprovalStatus == request.Status.Value);
        if (request.FromDate.HasValue)
            query = query.Where(x => x.ExpenseDate.Date >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.ExpenseDate.Date <= request.ToDate.Value.Date);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x => x.Vehicle.Name.ToLower().Contains(search) || x.Vehicle.PlateNumber.ToLower().Contains(search) || (x.VendorName != null && x.VendorName.ToLower().Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var expenses = await query.OrderByDescending(x => x.ExpenseDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetFleetVehicleExpensesResult(new PaginatedResult<FleetVehicleExpenseDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            expenses.Select(FleetFeatureHelpers.ToDto).ToList()));
    }
}

public class CreateFleetVehicleExpenseHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateFleetVehicleExpenseCommand, CreateFleetVehicleExpenseResult>
{
    public async Task<CreateFleetVehicleExpenseResult> Handle(CreateFleetVehicleExpenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Expense.VehicleId, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle", request.Expense.VehicleId);

        var expense = FleetVehicleExpense.Create(request.Expense, currentUserId);
        dbContext.VehicleExpenses.Add(expense);
        if (request.Expense.Odometer.HasValue && request.Expense.Odometer.Value > vehicle.CurrentOdometer)
            vehicle.UpdateOdometer(request.Expense.Odometer.Value, currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateFleetVehicleExpenseResult(expense.Id);
    }
}

public class UpdateFleetVehicleExpenseHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateFleetVehicleExpenseCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(UpdateFleetVehicleExpenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var expense = await dbContext.VehicleExpenses.FirstOrDefaultAsync(x => x.Id == request.Expense.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle expense", request.Expense.Id);
        expense.Update(request.Expense, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class SubmitFleetVehicleExpenseHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<SubmitFleetVehicleExpenseCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(SubmitFleetVehicleExpenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var expense = await dbContext.VehicleExpenses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle expense", request.Id);
        expense.Submit(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class ApproveFleetVehicleExpenseHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApproveFleetVehicleExpenseCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(ApproveFleetVehicleExpenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var expense = await dbContext.VehicleExpenses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle expense", request.Id);
        expense.Approve(request.Approval.IsApproved, request.Approval.Notes, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class DeleteFleetVehicleExpenseHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteFleetVehicleExpenseCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(DeleteFleetVehicleExpenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var expense = await dbContext.VehicleExpenses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle expense", request.Id);
        expense.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}
