using Shared.Data;

namespace DocumentManagement;

public static class DocumentManagementModule
{
    public static IServiceCollection AddDocumentManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<DocumentManagementDbContext>(options => options.UseSqlServer(connectionString));
        services.AddHttpContextAccessor();
        return services;
    }

    public static IApplicationBuilder UseDocumentManagementModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<DocumentManagementDbContext>("DocumentManagement");
        }

        return app;
    }
}
