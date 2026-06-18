namespace Maintenance.Assets.Features;

public record DeleteMaintenanceAssetCommand(Guid Id) : ICommand<DeleteMaintenanceAssetResult>;
public record DeleteMaintenanceAssetResult(bool IsSuccess);

public class DeleteMaintenanceAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/maintenance/assets/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMaintenanceAssetCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteMaintenanceAsset")
        .Produces<DeleteMaintenanceAssetResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Maintenance Asset")
        .RequireAuthorization(PermissionList.MaintenanceAssetPermissions.Delete);
    }
}

public class DeleteMaintenanceAssetHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteMaintenanceAssetCommand, DeleteMaintenanceAssetResult>
{
    public async Task<DeleteMaintenanceAssetResult> Handle(DeleteMaintenanceAssetCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var asset = await dbContext.MaintenanceAssets.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance asset", request.Id);

        var hasChildren = await dbContext.MaintenanceAssets.AnyAsync(x => x.ParentAssetId == request.Id, cancellationToken);
        if (hasChildren)
            throw new BadRequestException("Cannot delete an asset that has child assets.");

        var hasWorkOrders = await dbContext.MaintenanceWorkOrders.AnyAsync(x => x.AssetId == request.Id, cancellationToken);
        if (hasWorkOrders)
            throw new BadRequestException("Cannot delete an asset that has work orders.");

        asset.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteMaintenanceAssetResult(true);
    }
}
