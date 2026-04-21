
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
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddSignInManager()
        .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default")
        .AddTokenProvider<PhoneNumberTokenProvider<ApplicationUser>>("Phone")
        .AddTokenProvider<AuthenticatorTokenProvider<ApplicationUser>>("Authenticator");



        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IDataSeeder<AuthDbContext>, AuthDataSeeder>();

        var key = configuration["JwtOptions:SecretKey"]!;
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
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key))

            };
        });

        services.AddAuthorization();
        services.AddAuthorization(options =>
        {
            foreach (var permissions in SharedWithUI.Permissions.PermissionList.GetAll())
            {
                options.AddPolicy(permissions, policy => policy.AddRequirements(new PermissionRequirement(permissions)));
            }
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
