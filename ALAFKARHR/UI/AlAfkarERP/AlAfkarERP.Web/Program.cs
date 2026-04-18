using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Services;
using AlAfkarERP.Shared.Pages.Features.Employees.Services;
using AlAfkarERP.Shared.Pages.Reuable2;
using AlAfkarERP.Shared.Services.Auth;
using AlAfkarERP.Shared.Utilities;
using AlAfkarERP.Web.Components;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

var apiConfig = new ApiConfig();
builder.Configuration.GetSection("ApiConfig").Bind(apiConfig);

builder.Services.AddSingleton(apiConfig);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<SharedDataService>();

builder.Services.AddTransient<AuthMessageHandler>();

builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<LoadingService>();
builder.Services.AddScoped<SearchModalService>();


builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient("AlAfkarERP", client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();

#region Organization Module Services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddHttpClient<ICompanyService, CompanyService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddHttpClient<IBranchService, BranchService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped<IAdministrationService, AdministrationService>();
builder.Services.AddHttpClient<IAdministrationService, AdministrationService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();
#endregion Organization Module Services


#region Employees Module Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();


builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddHttpClient<IPositionService, PositionService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
})
.AddHttpMessageHandler<AuthMessageHandler>();
#endregion Employees Module Services


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(AlAfkarERP.Shared._Imports).Assembly);

app.Run();
