using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesOrder.Data;
using Shared.Data;
using Shared.Data.Seed;

namespace SalesOrder;

public static class SalesOrderModule
{
    public static IServiceCollection AddSalesOrderModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<SalesOrderDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });

        //services.AddScoped<IDataSeeder<SalesOrderDbContext>, CatalogDataSeeder>();


        return services;
    }

    public static IApplicationBuilder UseSalesOrderModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<SalesOrderDbContext>("SalesOrder");
        }


        return app;
    }

}
