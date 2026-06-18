namespace Fleet.Features;

public record GetFleetDashboardQuery : IQuery<GetFleetDashboardResult>;
public record GetFleetSummaryReportQuery(DateTime? FromDate, DateTime? ToDate) : IQuery<GetFleetSummaryReportResult>;
public record GetFleetDashboardResult(FleetDashboardDto Dashboard);
public record GetFleetSummaryReportResult(FleetSummaryReportDto Report);

public class FleetDashboardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/dashboard", async (ISender sender) =>
        {
            var result = await sender.Send(new GetFleetDashboardQuery());
            return Results.Ok(result);
        })
        .WithName("GetFleetDashboard")
        .Produces<GetFleetDashboardResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetReportsPermissions.View);

        app.MapGet("/api/v1/fleet/reports/summary", async (DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetSummaryReportQuery(fromDate, toDate));
            return Results.Ok(result);
        })
        .WithName("GetFleetSummaryReport")
        .Produces<GetFleetSummaryReportResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetReportsPermissions.View);
    }
}

public class GetFleetDashboardHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetDashboardQuery, GetFleetDashboardResult>
{
    public async Task<GetFleetDashboardResult> Handle(GetFleetDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var vehicles = dbContext.Vehicles.AsNoTracking();
        var documents = dbContext.VehicleDocuments.Include(x => x.Vehicle).AsNoTracking();
        var serviceRules = dbContext.VehicleServiceRules.Include(x => x.Vehicle).AsNoTracking();

        var allDueRules = await serviceRules.Where(x => x.IsActive).ToListAsync(cancellationToken);
        var dueRules = allDueRules.Where(x => x.IsDue(x.Vehicle.CurrentOdometer, today)).ToList();
        var expiringDocs = await documents
            .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value.Date <= today.AddDays(30) && x.Status != FleetDocumentStatus.Renewed)
            .OrderBy(x => x.ExpiryDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        var dashboard = new FleetDashboardDto
        {
            TotalVehicles = await vehicles.CountAsync(cancellationToken),
            OwnedVehicles = await vehicles.CountAsync(x => x.OwnershipType == FleetVehicleOwnershipType.Owned, cancellationToken),
            RentedVehicles = await vehicles.CountAsync(x => x.OwnershipType == FleetVehicleOwnershipType.Rented, cancellationToken),
            ActiveAssignments = await dbContext.VehicleAssignments.CountAsync(x => x.Status == FleetAssignmentStatus.Active, cancellationToken),
            VehiclesUnderMaintenance = await vehicles.CountAsync(x => x.Status == FleetVehicleStatus.UnderMaintenance, cancellationToken),
            ExpiringDocuments = expiringDocs.Count,
            OverdueServices = dueRules.Count,
            MonthlyExpenses = await dbContext.VehicleExpenses
                .Where(x => x.ExpenseDate >= monthStart && x.ApprovalStatus != FleetExpenseApprovalStatus.Cancelled && x.ApprovalStatus != FleetExpenseApprovalStatus.Rejected)
                .SumAsync(x => x.Amount, cancellationToken),
            ExpiringDocumentList = expiringDocs.Select(FleetFeatureHelpers.ToDto).ToList(),
            DueServiceRules = dueRules.Take(10).Select(FleetFeatureHelpers.ToDto).ToList()
        };

        return new GetFleetDashboardResult(dashboard);
    }
}

public class GetFleetSummaryReportHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetSummaryReportQuery, GetFleetSummaryReportResult>
{
    public async Task<GetFleetSummaryReportResult> Handle(GetFleetSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var expenses = dbContext.VehicleExpenses.Include(x => x.Vehicle).AsNoTracking()
            .Where(x => x.ApprovalStatus != FleetExpenseApprovalStatus.Cancelled && x.ApprovalStatus != FleetExpenseApprovalStatus.Rejected);

        if (request.FromDate.HasValue)
            expenses = expenses.Where(x => x.ExpenseDate.Date >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            expenses = expenses.Where(x => x.ExpenseDate.Date <= request.ToDate.Value.Date);

        var expenseList = await expenses.ToListAsync(cancellationToken);
        var report = new FleetSummaryReportDto
        {
            TotalExpenses = expenseList.Sum(x => x.Amount),
            MaintenanceExpenses = expenseList.Where(x => x.Category == FleetExpenseCategory.Maintenance).Sum(x => x.Amount),
            RentedVehicleExpenses = expenseList.Where(x => x.Category == FleetExpenseCategory.RentalPayment || x.Vehicle.OwnershipType == FleetVehicleOwnershipType.Rented).Sum(x => x.Amount),
            ExpensesByCategory = expenseList.GroupBy(x => x.Category)
                .Select(x => new FleetExpenseCategorySummaryDto { Category = x.Key, Amount = x.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList(),
            ExpensesByVehicle = expenseList.GroupBy(x => new { x.VehicleId, x.Vehicle.Name })
                .Select(x => new FleetVehicleExpenseSummaryDto { VehicleId = x.Key.VehicleId, VehicleName = x.Key.Name, Amount = x.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList()
        };

        return new GetFleetSummaryReportResult(report);
    }
}
