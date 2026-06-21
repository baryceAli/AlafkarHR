using Shared.Data;

namespace DocumentManagement;

public static class DocumentManagementModule
{
    public static IServiceCollection AddDocumentManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<DocumentManagementDbContext>(options => options.UseSqlServer(connectionString));
        services.Configure<DocumentStorageOptions>(configuration.GetSection(DocumentStorageOptions.SectionName));
        services.AddScoped<IDocumentStorageProvider>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DocumentStorageOptions>>().Value;
            if (!string.Equals(options.Provider, DocumentStorageProviders.LocalFileSystem, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Document storage provider '{options.Provider}' is not supported by this build.");

            return serviceProvider.GetRequiredService<LocalDocumentStorageProvider>();
        });
        services.AddScoped<LocalDocumentStorageProvider>();
        services.AddScoped<IDocumentUploadPolicyService, DocumentUploadPolicyService>();
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
