
using Auth.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Shared.Contracts.Messaging;

namespace Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.

        services.Configure<JwtOptions>(
            configuration.GetSection("JwtOptions"));

        services.Configure<OTPOptions>(
            configuration.GetSection("OTPOptions"));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<Shared.Contracts.Messaging.IMessageSender, EmailSender>();
        //Api endpoint services


        //Application use case services


        //Data - Infrastructure services
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("Database");
        //var connectionString = configuration.GetConnectionString("AuthDatabase");
        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString);
        });



        // Minimal API-friendly Identity with roles
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddSignInManager()
        .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default")
        .AddTokenProvider<PhoneNumberTokenProvider<ApplicationUser>>("Phone")
        .AddTokenProvider<AuthenticatorTokenProvider<ApplicationUser>>("Authenticator");



        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IDataSeeder<AuthDbContext>, AuthDataSeeder>();

        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtOptions section is missing.");
        var key = jwtOptions.SecretKey;
        // ✅ THIS IS THE MISSING PART
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key))

            };
        });

        services.AddAuthorization(options =>
        {
            var authenticatedUserPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.DefaultPolicy = authenticatedUserPolicy;
            options.FallbackPolicy = authenticatedUserPolicy;

            foreach (var permissions in PermissionList.GetAuthorizationPolicyPermissions())
            {
                options.AddPolicy(permissions, policy => policy.AddRequirements(new PermissionRequirement(permissions)));
            }

            options.AddPolicy(PermissionList.AttendancePermissions.ViewReportsOrScopedReportsPolicy, policy =>
                policy.RequireAssertion(context =>
                {
                    var permissions = context.User.FindAll(CompanyRoleTemplates.ManagedClaimType)
                        .Select(x => x.Value)
                        .ToHashSet(StringComparer.Ordinal);

                    return permissions.Contains(PermissionList.AttendancePermissions.ViewReports)
                        || permissions.Contains(PermissionList.AttendancePermissions.ViewScopedReports);
                }));
        });

        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        return services;
    }

    public static IApplicationBuilder UseAuthModule(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        //application.use




        //Use Api endpoint services


        //Use Application use case services


        //Use Data - Infrastructure services
        if (env.IsDevelopment())
        {
            app.UseMigration<AuthDbContext>("Auth");
        }


        return app;
    }

}
