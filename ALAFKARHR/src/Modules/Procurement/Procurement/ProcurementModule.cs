using Shared.Data;

namespace Procurement;

public static class ProcurementModule
{
    public static IServiceCollection AddProcurementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<ProcurementDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseProcurementModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<ProcurementDbContext>("Procurement");
        }

        return app;
    }
}
