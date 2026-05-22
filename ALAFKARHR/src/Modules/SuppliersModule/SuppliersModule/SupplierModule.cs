using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Data;
using Shared.Data.Seed;
using SuppliersModule.Data;

namespace SuppliersModule;

public static class SupplierModule
{
    public static IServiceCollection AddSupplierModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<SupplierDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });

        //services.AddScoped<IDataSeeder<SupplierDbContext>, CatalogDataSeeder>();


        return services;
    }

    public static IApplicationBuilder UseSupplierModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<SupplierDbContext>("Supplier");
        }


        return app;
    }

}
