using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Data;
using Shared.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Payroll.Data.Seed;
using Microsoft.Extensions.Hosting;
using Shared.Data;
namespace Payroll;

public static class PayrollModule
{
    public static IServiceCollection AddPayrollModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.


        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<PayrollDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            //options.UseSqlServer(connectionString);
        });

        services.AddScoped<IDataSeeder<PayrollDbContext>, PayrollDataSeeder>();


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
            app.UseMigration<PayrollDbContext>("Payroll");
        }


        return app;
    }
}
