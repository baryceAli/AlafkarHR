namespace Maintenance.Assets.Features;

public record UpdateMaintenanceAssetRequest(UpdateMaintenanceAssetDto Asset);
public record UpdateMaintenanceAssetCommand(UpdateMaintenanceAssetDto Asset) : ICommand<UpdateMaintenanceAssetResult>;
public record UpdateMaintenanceAssetResult(bool IsSuccess);

public class UpdateMaintenanceAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/maintenance/assets", async (UpdateMaintenanceAssetRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateMaintenanceAssetCommand(request.Asset));
            return Results.Ok(result);
        })
        .WithName("UpdateMaintenanceAsset")
        .Produces<UpdateMaintenanceAssetResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Maintenance Asset")
        .RequireAuthorization(PermissionList.MaintenanceAssetPermissions.Edit);
    }
}

public class UpdateMaintenanceAssetHandler(MaintenanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateMaintenanceAssetCommand, UpdateMaintenanceAssetResult>
{
    public async Task<UpdateMaintenanceAssetResult> Handle(UpdateMaintenanceAssetCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var asset = await dbContext.MaintenanceAssets.FirstOrDefaultAsync(x => x.Id == request.Asset.Id, cancellationToken)
            ?? throw new NotFoundException("Maintenance asset", request.Asset.Id);

        await MaintenanceFeatureHelpers.EnsureParentAssetAsync(dbContext, request.Asset.ParentAssetId, cancellationToken);

        asset.Update(
            string.IsNullOrWhiteSpace(request.Asset.AssetCode) ? asset.AssetCode : request.Asset.AssetCode,
            request.Asset.Name,
            request.Asset.NameEng,
            request.Asset.AssetType,
            request.Asset.Status,
            request.Asset.CompanyId,
            request.Asset.BranchId,
            request.Asset.ParentAssetId,
            request.Asset.Description,
            request.Asset.Location,
            request.Asset.SerialNumber,
            request.Asset.PurchaseDate,
            request.Asset.WarrantyEndDate,
            currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMaintenanceAssetResult(true);
    }
}
