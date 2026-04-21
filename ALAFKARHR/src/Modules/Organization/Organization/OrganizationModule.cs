using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Organization.Data.Seed;
using Shared.Data;
using Shared.Data.Seed;

namespace Organization;

public static class OrganizationModule
{

    public static IServiceCollection AddOrganizationModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<OrganizationDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IDataSeeder<OrganizationDbContext>, OrganizationDataSeeder>();


        return services;
    }

    public static IApplicationBuilder UseOrganizationModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<OrganizationDbContext>("Organization");
        }


        return app;
    }
}
