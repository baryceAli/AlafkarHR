namespace Maintenance.Services;

public interface IMaintenanceNumberGenerator
{
    Task<string> GenerateAssetCodeAsync(MaintenanceAssetType assetType, CancellationToken cancellationToken);
    Task<string> GenerateWorkOrderNumberAsync(CancellationToken cancellationToken);
}

public class MaintenanceNumberGenerator(MaintenanceDbContext dbContext) : IMaintenanceNumberGenerator
{
    public async Task<string> GenerateAssetCodeAsync(MaintenanceAssetType assetType, CancellationToken cancellationToken)
    {
        var prefix = assetType switch
        {
            MaintenanceAssetType.Building => "BLD",
            MaintenanceAssetType.Apartment => "APT",
            MaintenanceAssetType.Office => "OFF",
            MaintenanceAssetType.Vehicle => "VEH",
            MaintenanceAssetType.Equipment => "EQP",
            _ => "AST"
        };

        var count = await dbContext.MaintenanceAssets.LongCountAsync(cancellationToken) + 1;
        return $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }

    public async Task<string> GenerateWorkOrderNumberAsync(CancellationToken cancellationToken)
    {
        var count = await dbContext.MaintenanceWorkOrders.LongCountAsync(cancellationToken) + 1;
        return $"MWO-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }
}
