namespace Shared.Data.Seed;

public interface IDataSeeder<TContext>
{
    Task SeedAllAsync(TContext context);
}
