namespace Maintenance.WorkOrders.Features;

public record GetMaintenanceDashboardQuery() : IQuery<GetMaintenanceDashboardResult>;
public record GetMaintenanceDashboardResult(MaintenanceDashboardDto Dashboard);
public record GetMaintenanceSummaryReportQuery(DateTime? FromDate, DateTime? ToDate) : IQuery<GetMaintenanceSummaryReportResult>;
public record GetMaintenanceSummaryReportResult(MaintenanceSummaryReportDto Report);

public class MaintenanceDashboardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/maintenance/dashboard", async (ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceDashboardQuery());
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceDashboard")
        .Produces<GetMaintenanceDashboardResult>(StatusCodes.Status200OK)
        .WithSummary("Get Maintenance Dashboard")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.View);

        app.MapGet("/api/v1/maintenance/reports/summary", async (DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceSummaryReportQuery(fromDate, toDate));
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceSummaryReport")
        .Produces<GetMaintenanceSummaryReportResult>(StatusCodes.Status200OK)
        .WithSummary("Get Maintenance Summary Report")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.ViewReports);
    }
}

public class GetMaintenanceDashboardHandler(MaintenanceDbContext dbContext)
    : IQueryHandler<GetMaintenanceDashboardQuery, GetMaintenanceDashboardResult>
{
    public async Task<GetMaintenanceDashboardResult> Handle(GetMaintenanceDashboardQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await BuildDashboardAsync(dbContext, null, null, cancellationToken);
        return new GetMaintenanceDashboardResult(dashboard);
    }

    internal static async Task<MaintenanceDashboardDto> BuildDashboardAsync(
        MaintenanceDbContext dbContext,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var workOrders = dbContext.MaintenanceWorkOrders.AsNoTracking().AsQueryable();
        if (fromDate.HasValue)
            workOrders = workOrders.Where(x => x.RequestedDate.Date >= fromDate.Value.Date);
        if (toDate.HasValue)
            workOrders = workOrders.Where(x => x.RequestedDate.Date <= toDate.Value.Date);

        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return new MaintenanceDashboardDto
        {
            TotalAssets = await dbContext.MaintenanceAssets.CountAsync(cancellationToken),
            ActiveAssets = await dbContext.MaintenanceAssets.CountAsync(x => x.Status == MaintenanceAssetStatus.Active, cancellationToken),
            OpenWorkOrders = await workOrders.CountAsync(x => x.Status == MaintenanceWorkOrderStatus.Open || x.Status == MaintenanceWorkOrderStatus.Assigned, cancellationToken),
            InProgressWorkOrders = await workOrders.CountAsync(x => x.Status == MaintenanceWorkOrderStatus.InProgress, cancellationToken),
            PendingApprovals = await workOrders.CountAsync(x => x.CostApprovalStatus == MaintenanceCostApprovalStatus.Pending || x.Status == MaintenanceWorkOrderStatus.PendingApproval, cancellationToken),
            CompletedThisMonth = await dbContext.MaintenanceWorkOrders.CountAsync(x => x.Status == MaintenanceWorkOrderStatus.Completed && x.CompletedAt >= monthStart, cancellationToken),
            EstimatedCost = await workOrders.SumAsync(x => x.EstimatedCost ?? 0, cancellationToken),
            ActualCost = await workOrders.SumAsync(x => x.ActualCost ?? 0, cancellationToken)
        };
    }
}

public class GetMaintenanceSummaryReportHandler(MaintenanceDbContext dbContext)
    : IQueryHandler<GetMaintenanceSummaryReportQuery, GetMaintenanceSummaryReportResult>
{
    public async Task<GetMaintenanceSummaryReportResult> Handle(GetMaintenanceSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await GetMaintenanceDashboardHandler.BuildDashboardAsync(dbContext, request.FromDate, request.ToDate, cancellationToken);
        var query = dbContext.MaintenanceWorkOrders
            .Include(x => x.Asset)
            .AsNoTracking()
            .AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(x => x.RequestedDate.Date >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.RequestedDate.Date <= request.ToDate.Value.Date);

        var report = new MaintenanceSummaryReportDto
        {
            TotalAssets = dashboard.TotalAssets,
            ActiveAssets = dashboard.ActiveAssets,
            OpenWorkOrders = dashboard.OpenWorkOrders,
            InProgressWorkOrders = dashboard.InProgressWorkOrders,
            PendingApprovals = dashboard.PendingApprovals,
            CompletedThisMonth = dashboard.CompletedThisMonth,
            EstimatedCost = dashboard.EstimatedCost,
            ActualCost = dashboard.ActualCost,
            StatusSummary = await query
                .GroupBy(x => x.Status)
                .Select(x => new MaintenanceStatusSummaryDto { Status = x.Key, Count = x.Count() })
                .ToListAsync(cancellationToken),
            AssetTypeSummary = await query
                .GroupBy(x => x.Asset.AssetType)
                .Select(x => new MaintenanceAssetTypeSummaryDto { AssetType = x.Key, Count = x.Count() })
                .ToListAsync(cancellationToken)
        };

        return new GetMaintenanceSummaryReportResult(report);
    }
}
