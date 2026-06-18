using Shared.Data;

namespace Contracts;

public static class ContractsModule
{
    public static IServiceCollection AddContractsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<ContractsDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseContractsModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<ContractsDbContext>("Contracts");
        }

        return app;
    }
}
