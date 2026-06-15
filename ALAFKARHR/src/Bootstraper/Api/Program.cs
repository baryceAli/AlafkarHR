using AttendanceDomain;
using Auth;
using Carter;
using Catalog;
using CustomersModule;
using EmployeeModule;
using GeneralSettings;
using Inventory;
using Organization;
using Payroll;
using Pricing;
using SalesOrder;
using Shared.Exceptions.Handler;
using Shared.Extentions;
using SuppliersModule;
using TaskManagement;

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
var attendanceAssembly = typeof(AttendanceDomainModule).Assembly;
var organizationAssembly = typeof(OrganizationModule).Assembly;
var employeeAssembly = typeof(EmployeesModule).Assembly;
var catalogAssembly = typeof(CatalogModule).Assembly;
var inventoryAssembly = typeof(InventoryModule).Assembly;
var generalSettingsAssembly = typeof(GeneralSettingsModule).Assembly;
var customerAssembly = typeof(CustomerModule).Assembly;
var salesOrderAssembly=typeof(SalesOrderModule).Assembly;
var supplierAssembly =typeof(SupplierModule).Assembly;
var pricingAssembly = typeof(PricingModule).Assembly;
var payrollAssembly = typeof(PayrollModule).Assembly;
var taskManagementAssembly = typeof(TaskManagementModule).Assembly;

builder.Services.AddCarterWithAssemblies(
                        authAssembly,
                        attendanceAssembly,
                        organizationAssembly,
                        employeeAssembly,
                        catalogAssembly,
                        inventoryAssembly,
                        generalSettingsAssembly,
                        customerAssembly,
                        salesOrderAssembly,
                        supplierAssembly,
                        pricingAssembly,
                        payrollAssembly,
                        taskManagementAssembly
                        );
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);

builder.Services.AddMediatRWithAssemblies(
                        authAssembly,
                        attendanceAssembly,
                        organizationAssembly,
                        employeeAssembly,
                        catalogAssembly,
                        inventoryAssembly,
                        generalSettingsAssembly, 
                        customerAssembly,
                        salesOrderAssembly,
                        supplierAssembly,
                        pricingAssembly,
                        payrollAssembly,
                        taskManagementAssembly
                        );
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);


#endregion Commin Services

#region Module Service: Auth, Catalog, ShoppingCart, Ordering
builder.Services
        .AddAuthModule(builder.Configuration)
        .AddAttendanceModule(builder.Configuration)
        .AddOrganizationModule(builder.Configuration)
        .AddEmployeeModule(builder.Configuration)
        .AddCatalogModule(builder.Configuration)
        .AddInventoryModule(builder.Configuration)
        .AddGeneralSettingsModule(builder.Configuration)
        .AddCustomerModule(builder.Configuration)
        .AddSalesOrderModule(builder.Configuration)
        .AddSupplierModule(builder.Configuration)
        .AddPricingModule(builder.Configuration)
        .AddPayrollModule(builder.Configuration)
        .AddTaskManagementModule(builder.Configuration);
#endregion






//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
//builder.Services.AddAntiforgery();
var app = builder.Build();

//if(!app.Environment.IsDevelopment())
//{
//    app.UsePathBase("/backend");
//}
//app.UseAntiforgery();
app.UseExceptionHandler(options => { });
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app
    .UseAuthModule(app.Environment)
    .UseAttendanceModule(app.Environment)
    .UseOrganizationModule(app.Environment)
    .UseEmployeeModule(app.Environment)
    .UseCatalogModule(app.Environment)
    .UseInventoryModule(app.Environment)
    .UseGeneralSettingsModule(app.Environment)
    .UseCustomerModule(app.Environment)
    .UseSalesOrderModule(app.Environment)
    .UseSupplierModule(app.Environment)
    .UsePricingModule(app.Environment)
    .UsePayrollModule(app.Environment)
    .UseTaskManagementModule(app.Environment);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
