using AttendanceDomain;
using Auth;
using Cart;
using Carter;
using Catalog;
using Contracts;
using CustomersModule;
using DocumentManagement;
using EmployeeModule;
using Fleet;
using GeneralSettings;
using Inventory;
using LeaveManagement;
using Maintenance;
using Organization;
using Orders;
using Payroll;
using Payments;
using Pricing;
using Procurement;
using RealEstate;
using Sales;
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
var cartAssembly = typeof(CartModule).Assembly;
var contractsAssembly = typeof(ContractsModule).Assembly;
var documentManagementAssembly = typeof(DocumentManagementModule).Assembly;
var organizationAssembly = typeof(OrganizationModule).Assembly;
var ordersAssembly = typeof(OrdersModule).Assembly;
var employeeAssembly = typeof(EmployeesModule).Assembly;
var catalogAssembly = typeof(CatalogModule).Assembly;
var inventoryAssembly = typeof(InventoryModule).Assembly;
var leaveAssembly = typeof(LeaveModule).Assembly;
var generalSettingsAssembly = typeof(GeneralSettingsModule).Assembly;
var customerAssembly = typeof(CustomerModule).Assembly;
var salesOrderAssembly=typeof(SalesOrderModule).Assembly;
var supplierAssembly =typeof(SupplierModule).Assembly;
var paymentsAssembly = typeof(PaymentsModule).Assembly;
var pricingAssembly = typeof(PricingModule).Assembly;
var procurementAssembly = typeof(ProcurementModule).Assembly;
var realEstateAssembly = typeof(RealEstateModule).Assembly;
var salesAssembly = typeof(SalesModule).Assembly;
var payrollAssembly = typeof(PayrollModule).Assembly;
var taskManagementAssembly = typeof(TaskManagementModule).Assembly;
var maintenanceAssembly = typeof(MaintenanceModule).Assembly;
var fleetAssembly = typeof(FleetModule).Assembly;

builder.Services.AddCarterWithAssemblies(
                        authAssembly,
                        attendanceAssembly,
                        cartAssembly,
                        contractsAssembly,
                        documentManagementAssembly,
                        organizationAssembly,
                        ordersAssembly,
                        employeeAssembly,
                        catalogAssembly,
                        inventoryAssembly,
                        leaveAssembly,
                        generalSettingsAssembly,
                        customerAssembly,
                        salesOrderAssembly,
                        supplierAssembly,
                        paymentsAssembly,
                        pricingAssembly,
                        procurementAssembly,
                        realEstateAssembly,
                        salesAssembly,
                        payrollAssembly,
                        taskManagementAssembly,
                        maintenanceAssembly,
                        fleetAssembly
                        );
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);

builder.Services.AddMediatRWithAssemblies(
                        authAssembly,
                        attendanceAssembly,
                        cartAssembly,
                        contractsAssembly,
                        documentManagementAssembly,
                        organizationAssembly,
                        ordersAssembly,
                        employeeAssembly,
                        catalogAssembly,
                        inventoryAssembly,
                        leaveAssembly,
                        generalSettingsAssembly, 
                        customerAssembly,
                        salesOrderAssembly,
                        supplierAssembly,
                        paymentsAssembly,
                        pricingAssembly,
                        procurementAssembly,
                        realEstateAssembly,
                        salesAssembly,
                        payrollAssembly,
                        taskManagementAssembly,
                        maintenanceAssembly,
                        fleetAssembly
                        );
//catalogAssembly,
//basketAssembly,
//inventoryAssembly);


#endregion Commin Services

#region Module Service: Auth, Catalog, ShoppingCart, Ordering
builder.Services
        .AddAuthModule(builder.Configuration)
        .AddAttendanceModule(builder.Configuration)
        .AddCartModule(builder.Configuration)
        .AddContractsModule(builder.Configuration)
        .AddDocumentManagementModule(builder.Configuration)
        .AddOrganizationModule(builder.Configuration)
        .AddOrdersModule(builder.Configuration)
        .AddEmployeeModule(builder.Configuration)
        .AddCatalogModule(builder.Configuration)
        .AddInventoryModule(builder.Configuration)
        .AddLeaveModule(builder.Configuration)
        .AddGeneralSettingsModule(builder.Configuration)
        .AddCustomerModule(builder.Configuration)
        .AddSalesOrderModule(builder.Configuration)
        .AddSalesModule(builder.Configuration)
        .AddSupplierModule(builder.Configuration)
        .AddPaymentsModule(builder.Configuration)
        .AddPricingModule(builder.Configuration)
        .AddProcurementModule(builder.Configuration)
        .AddRealEstateModule(builder.Configuration)
        .AddPayrollModule(builder.Configuration)
        .AddTaskManagementModule(builder.Configuration)
        .AddMaintenanceModule(builder.Configuration)
        .AddFleetModule(builder.Configuration);
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
    .UseCartModule(app.Environment)
    .UseContractsModule(app.Environment)
    .UseDocumentManagementModule(app.Environment)
    .UseGeneralSettingsModule(app.Environment)
    .UseOrganizationModule(app.Environment)
    .UseOrdersModule(app.Environment)
    .UseEmployeeModule(app.Environment)
    .UseCatalogModule(app.Environment)
    .UseInventoryModule(app.Environment)
    .UseLeaveModule(app.Environment)
    .UseCustomerModule(app.Environment)
    .UseSalesOrderModule(app.Environment)
    .UseSalesModule(app.Environment)
    .UseSupplierModule(app.Environment)
    .UsePaymentsModule(app.Environment)
    .UsePricingModule(app.Environment)
    .UseProcurementModule(app.Environment)
    .UseRealEstateModule(app.Environment)
    .UsePayrollModule(app.Environment)
    .UseTaskManagementModule(app.Environment)
    .UseMaintenanceModule(app.Environment)
    .UseFleetModule(app.Environment);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
