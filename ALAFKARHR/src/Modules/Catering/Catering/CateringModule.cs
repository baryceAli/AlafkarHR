namespace Catering;

public static class CateringModule
{
    public static IServiceCollection AddCateringModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<CateringDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseCateringModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<CateringDbContext>("Catering");
        }

        return app;
    }
}
