namespace Maintenance.WorkOrders.Features;

public record GetMaintenanceWorkOrdersQuery(PaginationRequest PaginationRequest, MaintenanceWorkOrderFilterDto Filter, bool MineOnly) : IQuery<GetMaintenanceWorkOrdersResult>;
public record GetMaintenanceWorkOrdersResult(PaginatedResult<MaintenanceWorkOrderDto> WorkOrders);

public class GetMaintenanceWorkOrdersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/maintenance/work-orders", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            Guid? companyId,
            Guid? assetId,
            Guid? branchId,
            MaintenanceAssetType? assetType,
            MaintenancePriority? priority,
            MaintenanceWorkOrderStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            ISender sender) =>
        {
            var filter = new MaintenanceWorkOrderFilterDto
            {
                CompanyId = companyId,
                AssetId = assetId,
                BranchId = branchId,
                AssetType = assetType,
                Priority = priority,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await sender.Send(new GetMaintenanceWorkOrdersQuery(new PaginationRequest(PageIndex, PageSize, searchText), filter, false));
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceWorkOrders")
        .Produces<GetMaintenanceWorkOrdersResult>(StatusCodes.Status200OK)
        .WithSummary("Get Maintenance Work Orders")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.View);

        app.MapGet("/api/v1/maintenance/work-orders/my", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            Guid? companyId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceWorkOrdersQuery(
                new PaginationRequest(PageIndex, PageSize, searchText),
                new MaintenanceWorkOrderFilterDto { CompanyId = companyId },
                true));
            return Results.Ok(result);
        })
        .WithName("GetMyMaintenanceWorkOrders")
        .Produces<GetMaintenanceWorkOrdersResult>(StatusCodes.Status200OK)
        .WithSummary("Get My Maintenance Work Orders")
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.View);
    }
}

public class GetMaintenanceWorkOrdersHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : IQueryHandler<GetMaintenanceWorkOrdersQuery, GetMaintenanceWorkOrdersResult>
{
    public async Task<GetMaintenanceWorkOrdersResult> Handle(GetMaintenanceWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.MaintenanceWorkOrders
            .Include(x => x.Asset)
            .Include(x => x.Comments)
            .Include(x => x.Attachments)
            .Include(x => x.History)
            .AsNoTracking()
            .AsQueryable();

        query = request.MineOnly
            ? query.Where(x => x.RequestedByUserId == currentUserId || x.AssignedToUserId == currentUserId.ToString())
            : MaintenanceFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId);

        if (!request.Filter.CompanyId.HasValue)
            throw new BadRequestException("CompanyId is required for maintenance work order branch scope.");

        query = query.Where(x => x.Asset.CompanyId == request.Filter.CompanyId.Value);
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Filter.CompanyId.Value), cancellationToken);
        if (!BranchScopePolicy.CanFilter(branchAccess, request.Filter.BranchId))
            throw new ForbiddenException("You do not have permission to view this branch's maintenance work orders.");

        if (branchAccess.CanViewAllBranches)
        {
            if (request.Filter.BranchId.HasValue)
                query = query.Where(x => x.Asset.BranchId == request.Filter.BranchId.Value);
        }
        else
        {
            query = request.Filter.BranchId.HasValue
                ? query.Where(x => x.Asset.BranchId == null || x.Asset.BranchId == request.Filter.BranchId.Value)
                : query.Where(x => x.Asset.BranchId == null || (x.Asset.BranchId.HasValue && branchAccess.BranchIds.Contains(x.Asset.BranchId.Value)));
        }

        if (request.Filter.AssetId.HasValue)
            query = query.Where(x => x.AssetId == request.Filter.AssetId.Value);
        if (request.Filter.AssetType.HasValue)
            query = query.Where(x => x.Asset.AssetType == request.Filter.AssetType.Value);
        if (request.Filter.Priority.HasValue)
            query = query.Where(x => x.Priority == request.Filter.Priority.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (request.Filter.FromDate.HasValue)
            query = query.Where(x => x.RequestedDate.Date >= request.Filter.FromDate.Value.Date);
        if (request.Filter.ToDate.HasValue)
            query = query.Where(x => x.RequestedDate.Date <= request.Filter.ToDate.Value.Date);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x =>
                x.WorkOrderNumber.ToLower().Contains(search) ||
                x.Title.ToLower().Contains(search) ||
                x.Asset.Name.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var workOrders = await query
            .OrderByDescending(x => x.RequestedDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetMaintenanceWorkOrdersResult(new PaginatedResult<MaintenanceWorkOrderDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            workOrders.Select(MaintenanceFeatureHelpers.ToDto).ToList()));
    }
}
