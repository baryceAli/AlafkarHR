namespace Maintenance.Assets.Features;

public record GetMaintenanceAssetsQuery(PaginationRequest PaginationRequest, MaintenanceAssetFilterDto Filter) : IQuery<GetMaintenanceAssetsResult>;
public record GetMaintenanceAssetsResult(PaginatedResult<MaintenanceAssetDto> Assets);

public class GetMaintenanceAssetsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/maintenance/assets", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            MaintenanceAssetType? assetType,
            MaintenanceAssetStatus? status,
            Guid? companyId,
            Guid? branchId,
            Guid? parentAssetId,
            string? sourceModule,
            string? sourceEntityName,
            Guid? sourceEntityId,
            ISender sender) =>
        {
            var filter = new MaintenanceAssetFilterDto
            {
                AssetType = assetType,
                Status = status,
                CompanyId = companyId,
                BranchId = branchId,
                ParentAssetId = parentAssetId,
                SourceModule = sourceModule,
                SourceEntityName = sourceEntityName,
                SourceEntityId = sourceEntityId
            };

            var result = await sender.Send(new GetMaintenanceAssetsQuery(new PaginationRequest(PageIndex, PageSize, searchText), filter));
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceAssets")
        .Produces<GetMaintenanceAssetsResult>(StatusCodes.Status200OK)
        .WithSummary("Get Maintenance Assets")
        .RequireAuthorization(PermissionList.MaintenanceAssetPermissions.View);
    }
}

public class GetMaintenanceAssetsHandler(MaintenanceDbContext dbContext)
    : IQueryHandler<GetMaintenanceAssetsQuery, GetMaintenanceAssetsResult>
{
    public async Task<GetMaintenanceAssetsResult> Handle(GetMaintenanceAssetsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.MaintenanceAssets
            .Include(x => x.ParentAsset)
            .AsNoTracking()
            .AsQueryable();

        if (request.Filter.AssetType.HasValue)
            query = query.Where(x => x.AssetType == request.Filter.AssetType.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (request.Filter.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.Filter.CompanyId.Value);
        if (request.Filter.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);
        if (request.Filter.ParentAssetId.HasValue)
            query = query.Where(x => x.ParentAssetId == request.Filter.ParentAssetId.Value);
        if (!string.IsNullOrWhiteSpace(request.Filter.SourceModule))
            query = query.Where(x => x.SourceModule == request.Filter.SourceModule.Trim());
        if (!string.IsNullOrWhiteSpace(request.Filter.SourceEntityName))
            query = query.Where(x => x.SourceEntityName == request.Filter.SourceEntityName.Trim());
        if (request.Filter.SourceEntityId.HasValue)
            query = query.Where(x => x.SourceEntityId == request.Filter.SourceEntityId.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x =>
                x.AssetCode.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search) ||
                x.NameEng.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var assets = await query
            .OrderBy(x => x.AssetType)
            .ThenBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetMaintenanceAssetsResult(new PaginatedResult<MaintenanceAssetDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            assets.Select(MaintenanceAssetMappings.ToDto).ToList()));
    }
}
