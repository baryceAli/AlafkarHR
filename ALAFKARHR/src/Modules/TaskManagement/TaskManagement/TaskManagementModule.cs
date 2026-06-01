using Microsoft.Extensions.Hosting;
using TaskManagement.Data.Seed;
using TaskManagement.Tasks.Services;

namespace TaskManagement;

public static class TaskManagementModule
{
    public static IServiceCollection AddTaskManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ITaskNumberGenerator, TaskNumberGenerator>();
        services.AddScoped<IDataSeeder<TaskManagementDbContext>, TaskManagementDataSeeder>();

        return services;
    }

    public static IApplicationBuilder UseTaskManagementModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<TaskManagementDbContext>("TaskManagement");
        }

        return app;
    }
}
