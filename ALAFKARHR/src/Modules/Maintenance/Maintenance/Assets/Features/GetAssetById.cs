namespace Maintenance.Assets.Features;

public record GetMaintenanceAssetByIdQuery(Guid Id) : IQuery<GetMaintenanceAssetByIdResult>;
public record GetMaintenanceAssetByIdResult(MaintenanceAssetDto Asset);

public class GetMaintenanceAssetByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/maintenance/assets/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceAssetByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetMaintenanceAssetById")
        .Produces<GetMaintenanceAssetByIdResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Maintenance Asset By Id")
        .RequireAuthorization(PermissionList.MaintenanceAssetPermissions.View);
    }
}

public class GetMaintenanceAssetByIdHandler(MaintenanceDbContext dbContext)
    : IQueryHandler<GetMaintenanceAssetByIdQuery, GetMaintenanceAssetByIdResult>
{
    public async Task<GetMaintenanceAssetByIdResult> Handle(GetMaintenanceAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await dbContext.MaintenanceAssets
            .Include(x => x.ParentAsset)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance asset", request.Id);

        return new GetMaintenanceAssetByIdResult(MaintenanceAssetMappings.ToDto(asset));
    }
}
