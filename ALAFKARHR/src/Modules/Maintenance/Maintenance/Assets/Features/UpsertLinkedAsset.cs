namespace Maintenance.Assets.Features;

public class UpsertLinkedMaintenanceAssetHandler(
    MaintenanceDbContext dbContext,
    IMaintenanceNumberGenerator numberGenerator,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertLinkedMaintenanceAssetCommand, UpsertLinkedMaintenanceAssetResult>
{
    public async Task<UpsertLinkedMaintenanceAssetResult> Handle(UpsertLinkedMaintenanceAssetCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceModule))
            throw new BadRequestException("Source module is required.");
        if (string.IsNullOrWhiteSpace(request.SourceEntityName))
            throw new BadRequestException("Source entity name is required.");
        if (request.SourceEntityId == Guid.Empty)
            throw new BadRequestException("Source entity id is required.");

        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await MaintenanceFeatureHelpers.EnsureParentAssetAsync(dbContext, request.ParentAssetId, cancellationToken);

        var asset = await dbContext.MaintenanceAssets.FirstOrDefaultAsync(
            x => x.SourceModule == request.SourceModule.Trim()
                && x.SourceEntityName == request.SourceEntityName.Trim()
                && x.SourceEntityId == request.SourceEntityId,
            cancellationToken);

        if (asset is null)
        {
            var assetCode = string.IsNullOrWhiteSpace(request.AssetCode)
                ? await numberGenerator.GenerateAssetCodeAsync(request.AssetType, cancellationToken)
                : request.AssetCode.Trim();

            asset = MaintenanceAsset.Create(
                assetCode,
                request.Name,
                request.NameEng,
                request.AssetType,
                request.Status,
                request.CompanyId,
                request.BranchId,
                request.ParentAssetId,
                request.SourceModule,
                request.SourceEntityName,
                request.SourceEntityId,
                request.Description,
                request.Location,
                request.SerialNumber,
                request.PurchaseDate,
                request.WarrantyEndDate,
                currentUserId);

            dbContext.MaintenanceAssets.Add(asset);
        }
        else
        {
            asset.Update(
                string.IsNullOrWhiteSpace(request.AssetCode) ? asset.AssetCode : request.AssetCode,
                request.Name,
                request.NameEng,
                request.AssetType,
                request.Status,
                request.CompanyId,
                request.BranchId,
                request.ParentAssetId,
                request.SourceModule,
                request.SourceEntityName,
                request.SourceEntityId,
                request.Description,
                request.Location,
                request.SerialNumber,
                request.PurchaseDate,
                request.WarrantyEndDate,
                currentUserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertLinkedMaintenanceAssetResult(asset.Id);
    }
}

public class GetLinkedMaintenanceAssetHandler(MaintenanceDbContext dbContext)
    : IQueryHandler<GetLinkedMaintenanceAssetQuery, GetLinkedMaintenanceAssetResult>
{
    public async Task<GetLinkedMaintenanceAssetResult> Handle(GetLinkedMaintenanceAssetQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceModule))
            throw new BadRequestException("Source module is required.");
        if (string.IsNullOrWhiteSpace(request.SourceEntityName))
            throw new BadRequestException("Source entity name is required.");
        if (request.SourceEntityId == Guid.Empty)
            throw new BadRequestException("Source entity id is required.");

        var assetId = await dbContext.MaintenanceAssets
            .AsNoTracking()
            .Where(x => x.SourceModule == request.SourceModule.Trim()
                && x.SourceEntityName == request.SourceEntityName.Trim()
                && x.SourceEntityId == request.SourceEntityId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetLinkedMaintenanceAssetResult(assetId);
    }
}
