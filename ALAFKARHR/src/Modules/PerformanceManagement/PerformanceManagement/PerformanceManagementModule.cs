using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Data;

namespace PerformanceManagement;

public static class PerformanceManagementModule
{
    public static IServiceCollection AddPerformanceManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<PerformanceDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UsePerformanceManagementModule(this IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<PerformanceDbContext>("PerformanceManagement");
        }

        return app;
    }
}
