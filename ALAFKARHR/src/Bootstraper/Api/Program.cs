using Auth;
using Carter;
using EmployeeModule;
using Organization;
using Shared.Exceptions.Handler;
using Shared.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//var app = builder.Build();

// Add services to the container.
#region Common Services: Carter/ IMedtatR, FluentValidation
builder.Services.AddDataProtection();


var authAssembly = typeof(AuthModule).Assembly;
var organizationAssembly = typeof(OrganizationModule).Assembly;
var employeeAssembly = typeof(EmployeesModule).Assembly;


builder.Services.AddCarterWithAssemblies(
                        authAssembly,
                        organizationAssembly,
                        employeeAssembly);
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);

builder.Services.AddMediatRWithAssemblies(
                        authAssembly,
                        organizationAssembly,
                        employeeAssembly);
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);


#endregion Commin Services

#region Module Service: Auth, Catalog, ShoppingCart, Ordering
builder.Services
        .AddAuthModule(builder.Configuration)
        .AddOrganizationModule(builder.Configuration)
        .AddEmployeeModule(builder.Configuration);
#endregion






//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app
    .UseAuthModule(app.Environment)
    .UseOrganizationModule(app.Environment)
    .UseEmployeeModule(app.Environment);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();