namespace Maintenance.Assets.Features;

public record CreateMaintenanceAssetRequest(CreateMaintenanceAssetDto Asset);
public record CreateMaintenanceAssetCommand(CreateMaintenanceAssetDto Asset) : ICommand<CreateMaintenanceAssetResult>;
public record CreateMaintenanceAssetResult(Guid Id);

public class CreateMaintenanceAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/maintenance/assets", async (CreateMaintenanceAssetRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateMaintenanceAssetCommand(request.Asset));
            return Results.Created($"/api/v1/maintenance/assets/{result.Id}", result);
        })
        .WithName("CreateMaintenanceAsset")
        .Produces<CreateMaintenanceAssetResult>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Maintenance Asset")
        .RequireAuthorization(PermissionList.MaintenanceAssetPermissions.Create);
    }
}

public class CreateMaintenanceAssetHandler(
    MaintenanceDbContext dbContext,
    IMaintenanceNumberGenerator numberGenerator,
    IHttpContextAccessor httpContextAccessor,
    ISender sender)
    : ICommandHandler<CreateMaintenanceAssetCommand, CreateMaintenanceAssetResult>
{
    public async Task<CreateMaintenanceAssetResult> Handle(CreateMaintenanceAssetCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var assetCode = string.IsNullOrWhiteSpace(request.Asset.AssetCode)
            ? await numberGenerator.GenerateAssetCodeAsync(request.Asset.AssetType, cancellationToken)
            : request.Asset.AssetCode.Trim();

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.Asset.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, request.Asset.BranchId))
            throw new ForbiddenException("You do not have permission to create a maintenance asset in this branch scope.");

        await MaintenanceFeatureHelpers.EnsureParentAssetScopeAsync(
            dbContext,
            sender,
            request.Asset.CompanyId,
            request.Asset.BranchId,
            request.Asset.ParentAssetId,
            null,
            cancellationToken);

        var asset = MaintenanceAsset.Create(
            assetCode,
            request.Asset.Name,
            request.Asset.NameEng,
            request.Asset.AssetType,
            request.Asset.Status,
            request.Asset.CompanyId,
            request.Asset.BranchId,
            request.Asset.ParentAssetId,
            null,
            null,
            null,
            request.Asset.Description,
            request.Asset.Location,
            request.Asset.SerialNumber,
            request.Asset.PurchaseDate,
            request.Asset.WarrantyEndDate,
            currentUserId);

        dbContext.MaintenanceAssets.Add(asset);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateMaintenanceAssetResult(asset.Id);
    }
}
