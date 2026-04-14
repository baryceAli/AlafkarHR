using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Seed;

namespace Shared.Data;

public static class Extentions
{
    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app, string schema)
        where TContext : DbContext
    {
        bool isFirstModule = true;
        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(2);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();

                if (!isFirstModule)
                {
                    context.Database.MigrateAsync().GetAwaiter().GetResult();
                }

                isFirstModule = false;
                //context.Database.Migrate();
                // ✅ Only runs AFTER migration succeeds
                var seeders = scope.ServiceProvider.GetServices<IDataSeeder<TContext>>();
                foreach (var seeder in seeders)
                {
                    seeder.SeedAllAsync(context).GetAwaiter().GetResult();
                }

                Console.WriteLine($"Migration + Seeding success for {typeof(TContext).Name}");
                break;
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                    throw;

                Console.WriteLine($"Retry {attempt} failed for {typeof(TContext).Name}");

                Thread.Sleep(delay);
            }
        }

        return app;


        //MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();
        //SeedDataAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();
        //return app;
    }


    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }
    private static async Task SeedDataAsync<TContext>(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder<TContext>>();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync(context);
        }

    }


}
