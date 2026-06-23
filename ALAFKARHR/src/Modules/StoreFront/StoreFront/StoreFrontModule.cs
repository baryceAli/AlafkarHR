namespace StoreFront;

public static class StoreFrontModule
{
    public static IServiceCollection AddStoreFrontModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<StoreFrontDbContext>(options => options.UseSqlServer(connectionString));
        services.AddHttpContextAccessor();
        return services;
    }

    public static IApplicationBuilder UseStoreFrontModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<StoreFrontDbContext>("StoreFront");
        }

        return app;
    }
}
