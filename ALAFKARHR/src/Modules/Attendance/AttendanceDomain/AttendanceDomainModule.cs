using AttendanceDomain.Data.Seed;
using Shared.Data;
using Shared.Data.Seed;
using AttendanceDomain.Attendance.Features.SystemSetup;
using Shared.Setup;

namespace AttendanceDomain;

public static class AttendanceDomainModule
{
    public static IServiceCollection AddAttendanceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<AttendanceDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDataSeeder<AttendanceDbContext>, AttendanceDataSeeder>();
        services.AddScoped<ISetupReadinessContributor, AttendanceReadinessContributor>();

        return services;
    }

    public static IApplicationBuilder UseAttendanceModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseMigration<AttendanceDbContext>("Attendance");
        }

        return app;
    }
}
