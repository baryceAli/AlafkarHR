using GeneralSettings.Data;
using GeneralSettings.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Data;
using Shared.Data.Seed;
using Microsoft.EntityFrameworkCore;
using GeneralSettings.GeneralSettings.Features.SystemSetup;
using Shared.Setup;
namespace GeneralSettings;

public static class GeneralSettingsModule
{
    public static IServiceCollection AddGeneralSettingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.Configure<GeneralSettings.Features.PublicWebsite.PublicWebsiteOptions>(configuration.GetSection("PublicWebsite"));
        services.AddHttpContextAccessor();
        services.AddScoped<GeneralSettings.Features.PublicWebsite.PublicWebsiteSettingsResolver>();
        services.AddDbContext<GeneralSettingsDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IDataSeeder<GeneralSettingsDbContext>, GeneralSettingsDataSeeder>();
        services.AddScoped<ISetupReadinessContributor, GeneralSettingsReadinessContributor>();
        services.AddScoped<ISetupReadinessContributor, PosDefaultsReadinessContributor>();


        return services;
    }

    public static IApplicationBuilder UseGeneralSettingsModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<GeneralSettingsDbContext>("GeneralSettings");
        }


        return app;
    }

}
