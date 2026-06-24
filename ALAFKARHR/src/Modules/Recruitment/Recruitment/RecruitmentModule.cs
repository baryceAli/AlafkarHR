using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Data;

namespace Recruitment;

public static class RecruitmentModule
{
    public static IServiceCollection AddRecruitmentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<RecruitmentDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    public static IApplicationBuilder UseRecruitmentModule(this IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<RecruitmentDbContext>("Recruitment");
        }

        return app;
    }
}
