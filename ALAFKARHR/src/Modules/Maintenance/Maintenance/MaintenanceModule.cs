namespace Maintenance;

public static class MaintenanceModule
{
    public static IServiceCollection AddMaintenanceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<MaintenanceDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IMaintenanceNumberGenerator, MaintenanceNumberGenerator>();

        return services;
    }

    public static IApplicationBuilder UseMaintenanceModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<MaintenanceDbContext>("Maintenance");
        }

        return app;
    }
}
