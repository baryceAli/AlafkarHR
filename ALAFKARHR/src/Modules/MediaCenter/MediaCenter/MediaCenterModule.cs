namespace MediaCenter;

public static class MediaCenterModule
{
    public static IServiceCollection AddMediaCenterModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<MediaCenterDbContext>(options => options.UseSqlServer(connectionString));
        services.AddHttpContextAccessor();
        return services;
    }

    public static IApplicationBuilder UseMediaCenterModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<MediaCenterDbContext>("MediaCenter");
        }

        return app;
    }
}
