namespace Fleet;

public static class FleetModule
{
    public static IServiceCollection AddFleetModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<FleetDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IFleetNumberGenerator, FleetNumberGenerator>();

        return services;
    }

    public static IApplicationBuilder UseFleetModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<FleetDbContext>("Fleet");
        }

        return app;
    }
}
