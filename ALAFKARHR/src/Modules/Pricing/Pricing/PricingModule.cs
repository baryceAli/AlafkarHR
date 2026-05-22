using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Pricing.Data;
using Shared.Data.Seed;
using Pricing.Data.Seed;
using Microsoft.Extensions.Hosting;
using Shared.Data;

namespace Pricing;

public static class PricingModule
{

    public static IServiceCollection AddPricingModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<PricingDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IPriceResolver, PriceResolver>();
        services.AddScoped<IDataSeeder<PricingDbContext>, PricingDataSeeder>();


        return services;
    }

    public static IApplicationBuilder UsePricingModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<PricingDbContext>("Pricing");
        }


        return app;
    }


}
