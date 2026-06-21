namespace ProjectManagement;

public static class ProjectManagementModule
{
    public static IServiceCollection AddProjectManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<ProjectManagementDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseProjectManagementModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<ProjectManagementDbContext>("ProjectManagement");
        }

        return app;
    }
}
