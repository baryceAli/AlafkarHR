using Accounting.Data.Seed;
using Shared.Data.Seed;

namespace Accounting;

public static class AccountingModule
{
    public static IServiceCollection AddAccountingModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<AccountingDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDataSeeder<AccountingDbContext>, AccountingDataSeeder>();
        return services;
    }

    public static IApplicationBuilder UseAccountingModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<AccountingDbContext>("Accounting");
        }

        return app;
    }
}
