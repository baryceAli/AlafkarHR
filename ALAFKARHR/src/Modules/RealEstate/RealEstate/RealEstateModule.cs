namespace RealEstate;

public static class RealEstateModule
{
    public static IServiceCollection AddRealEstateModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<RealEstateDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseRealEstateModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<RealEstateDbContext>("RealEstate");
        }

        return app;
    }
}
