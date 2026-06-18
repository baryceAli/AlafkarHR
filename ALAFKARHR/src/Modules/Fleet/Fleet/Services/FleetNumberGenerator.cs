namespace Fleet.Services;

public interface IFleetNumberGenerator
{
    Task<string> GenerateVehicleCodeAsync(CancellationToken cancellationToken);
}

public class FleetNumberGenerator(FleetDbContext dbContext) : IFleetNumberGenerator
{
    public async Task<string> GenerateVehicleCodeAsync(CancellationToken cancellationToken)
    {
        var count = await dbContext.Vehicles.LongCountAsync(cancellationToken) + 1;
        return $"FLT-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }
}
