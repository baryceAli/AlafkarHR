using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Auth.Services;
using AlAfkarERP.Shared.Pages.Features.Attendance.Services;
using AlAfkarERP.Shared.Pages.Features.Catalog.Services;
using AlAfkarERP.Shared.Pages.Features.Company.Services;
using AlAfkarERP.Shared.Pages.Features.Contracts.Services;
using AlAfkarERP.Shared.Pages.Features.Customers.Services;
using AlAfkarERP.Shared.Pages.Features.DocumentManagement.Services;
using AlAfkarERP.Shared.Pages.Features.Employees.Services;
using AlAfkarERP.Shared.Pages.Features.Fleet.Services;
using AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;
using AlAfkarERP.Shared.Pages.Features.Inventories.Services;
using AlAfkarERP.Shared.Pages.Features.Maintenance.Services;
using AlAfkarERP.Shared.Pages.Features.Payroll.Services;
using AlAfkarERP.Shared.Pages.Features.Procurement.Services;
using AlAfkarERP.Shared.Pages.Features.RealEstate.Services;
using AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;
using AlAfkarERP.Shared.Pages.Features.Suppliers.Services;
using AlAfkarERP.Shared.Pages.Features.TaskManagement.Services;
using AlAfkarERP.Shared.Pages.Reuable2;
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


builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<LoadingService>();
builder.Services.AddScoped<SearchModalService>();


builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient("AlAfkarERP", client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddHttpClient<IRoleService, RoleService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});


#region Organization Module Services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddHttpClient<ICompanyService, CompanyService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IParentCompanyService, ParentCompanyService>();
builder.Services.AddHttpClient<IParentCompanyService, ParentCompanyService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ILicenseCategoryService, LicenseCategoryService>();
builder.Services.AddHttpClient<ILicenseCategoryService, LicenseCategoryService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddHttpClient<IBranchService, BranchService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IAdministrationService, AdministrationService>();
builder.Services.AddHttpClient<IAdministrationService, AdministrationService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Organization Module Services


#region Employees Module Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});


builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddHttpClient<IPositionService, PositionService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IAcademicInistitutionService, AcademicInistitutionService>();
builder.Services.AddHttpClient<IAcademicInistitutionService, AcademicInistitutionService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ISpecializationService, SpecializationService>();
builder.Services.AddHttpClient<ISpecializationService, SpecializationService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});


#endregion Employees Module Services

#region Catalog Module Services
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddHttpClient<IBrandService, BrandService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});


builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddHttpClient<IUnitService, UnitService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IVariantService, VariantService>();
builder.Services.AddHttpClient<IVariantService, VariantService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddHttpClient<IPackageService, PackageService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IPriceListService, PriceListService>();
builder.Services.AddHttpClient<IPriceListService, PriceListService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

#endregion Catalog Module Services

#region Inventory
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddHttpClient<IWarehouseService, WarehouseService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddHttpClient<IBatchService, BatchService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddHttpClient<IInventoryService, InventoryService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

#endregion Inventory

#region Customers
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddHttpClient<ICustomerService, CustomerService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ICustomerGroupService, CustomerGroupService>();
builder.Services.AddHttpClient<ICustomerGroupService, CustomerGroupService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ICustomerPricingProfileService, CustomerPricingProfileService>();
builder.Services.AddHttpClient<ICustomerPricingProfileService, CustomerPricingProfileService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

#endregion Customers

#region Suppliers
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddHttpClient<ISupplierService, SupplierService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ISupplierGroupService, SupplierGroupService>();
builder.Services.AddHttpClient<ISupplierGroupService, SupplierGroupService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

#endregion Suppliers

#region GeneralSettings
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ICompanySettingService, CompanySettingService>();
builder.Services.AddHttpClient<ICompanySettingService, CompanySettingService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion GeneralSettings

#region Attendance
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddHttpClient<IAttendanceService, AttendanceService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Attendance

#region TaskManagement
builder.Services.AddScoped<ITaskManagementService, TaskManagementService>();
builder.Services.AddHttpClient<ITaskManagementService, TaskManagementService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion TaskManagement

#region Maintenance
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddHttpClient<IMaintenanceService, MaintenanceService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Maintenance

#region RealEstate
builder.Services.AddScoped<IRealEstateService, RealEstateService>();
builder.Services.AddHttpClient<IRealEstateService, RealEstateService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion RealEstate

#region Fleet
builder.Services.AddScoped<IFleetService, FleetService>();
builder.Services.AddHttpClient<IFleetService, FleetService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Fleet

#region Payroll
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddHttpClient<IPayrollService, PayrollService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Payroll

#region Procurement
builder.Services.AddScoped<IProcurementService, ProcurementService>();
builder.Services.AddHttpClient<IProcurementService, ProcurementService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Procurement

#region Contracts
builder.Services.AddScoped<IContractsService, ContractsService>();
builder.Services.AddHttpClient<IContractsService, ContractsService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Contracts

#region DocumentManagement
builder.Services.AddScoped<IDocumentManagementService, DocumentManagementService>();
builder.Services.AddHttpClient<IDocumentManagementService, DocumentManagementService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion DocumentManagement

#region Sales
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpClient<ICartService, CartService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<IOrderIntakeService, OrderIntakeService>();
builder.Services.AddHttpClient<IOrderIntakeService, OrderIntakeService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddHttpClient<ISalesService, SalesService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});

builder.Services.AddScoped<ISalesOrderWorkflowService, SalesOrderWorkflowService>();
builder.Services.AddHttpClient<ISalesOrderWorkflowService, SalesOrderWorkflowService>(client =>
{
    client.BaseAddress = new Uri($"{apiConfig.BaseURL}");
});
#endregion Sales
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
