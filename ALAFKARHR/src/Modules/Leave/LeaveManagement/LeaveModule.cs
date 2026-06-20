using LeaveManagement.Data;
using Shared.Data;

namespace LeaveManagement;

public static class LeaveModule
{
    public static IServiceCollection AddLeaveModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<LeaveDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    public static IApplicationBuilder UseLeaveModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<LeaveDbContext>("Leave");
        }

        return app;
    }
}
