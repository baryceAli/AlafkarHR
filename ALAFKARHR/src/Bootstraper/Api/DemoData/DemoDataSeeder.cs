using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Accounting.Accounting.Features;
using Accounting.Contracts.Accounting.Features;
using Accounting.Data;
using Auth.Contracts.Features.ResetCompanyAdminPassword;
using Auth.Users.Dtos;
using Auth.Users.Models;
using Auth.Users.Roles;
using Catalog.Data;
using Catalog.Products.Models;
using CustomersModule.Customers.Models;
using CustomersModule.Data;
using EmployeeModule.Data;
using EmployeeModule.Employees.Models;
using Inventory.Data;
using Inventory.Warehouses.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Organization.Data;
using Organization.Organizations.Features.ParentCompanies;
using Organization.Organizations.Models;
using Shared.Contracts.Organization;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;
using SharedWithUI.Attendance.Enums;
using SharedWithUI.Auth.Dtos;
using SharedWithUI.Catalog.Enums;
using SharedWithUI.Customers.Enums;
using SharedWithUI.Employees.Dtos;
using SharedWithUI.Employees.Enums;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Inventory.Enums;
using SharedWithUI.Organization.Dtos;
using SharedWithUI.Organization.Enums;
using SharedWithUI.Permissions;
using SharedWithUI.Suppliers.Enums;
using SharedWithUI.TaskManagement.Enums;
using SuppliersModule.Data;
using SuppliersModule.Suppliers.Models;
using TaskManagement.Data;
using TaskManagement.Tasks.Models;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace Api.DemoData;

public interface IDemoDataManagementService
{
    Task<IReadOnlyList<DemoDataSummaryDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<DemoDataStatusDto> GetStatusAsync(string companyCode, CancellationToken cancellationToken = default);
    Task<DemoDataOperationResultDto> CreateAsync(CancellationToken cancellationToken = default);
    Task<DemoDataOperationResultDto> CreateAsync(DemoDataCreateRequestDto request, CancellationToken cancellationToken = default);
    Task<DemoDataOperationResultDto> ResetAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default);
    Task<DemoDataOperationResultDto> DeleteAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default);
    Task<DemoDataOperationResultDto> ResetAdminPasswordAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default);
}

public sealed class DemoDataSeeder(
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    ISender sender,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    OrganizationDbContext organizationDbContext,
    EmployeeDbContext employeeDbContext,
    TaskManagementDbContext taskDbContext,
    CatalogDbContext catalogDbContext,
    CustomerDbContext customerDbContext,
    SupplierDbContext supplierDbContext,
    InventoryDbContext inventoryDbContext,
    AccountingDbContext accountingDbContext) : IDemoDataManagementService
{
    private const string DemoActorName = "demo.seed";
    private const string DefaultCompanyCode = "DEMO-ERP";
    private const string DefaultPassword = "Admin@123";
    private const string DemoMarker = "Managed by DemoData";

    private sealed record DemoSeedContext(
        string CompanyCode,
        string CompanyName,
        string CompanyNameEng,
        string AdminUserName,
        string AdminEmail,
        string AdminPhoneNumber,
        string DisplayLabel,
        string CodeSlug,
        string Marker)
    {
        public string UserName(string roleKey) => $"demo.{roleKey}.{CodeSlug}";
        public string Email(string roleKey) => $"demo.{roleKey}.{CodeSlug}@alafkar.demo";
        public string TaskNumber(string number) => $"DEMO-{CompanyCode}-TASK-{number}";
        public string TaskNumberPrefix => $"DEMO-{CompanyCode}-TASK-";
        public string DocumentNumber(string number) => $"DEMO-{CompanyCode}-{number}";
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!configuration.GetValue<bool>("DemoData:Enabled"))
            return;

        await CreateAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DemoDataSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var configuredCode = ResolveCompanyCode();
        var candidates = await organizationDbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == null)
            .Where(x => x.Code == configuredCode || organizationDbContext.CompanyLicenses.Any(l => l.CompanyId == x.Id && l.Notes != null && l.Notes.Contains(DemoMarker)))
            .OrderBy(x => x.Code)
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var demos = new List<DemoDataSummaryDto>();
        foreach (var companyCode in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var status = await GetStatusAsync(companyCode, cancellationToken);
            if (status.Exists && status.IsRecognizedDemoTenant)
                demos.Add(ToSummary(status));
        }

        return demos;
    }

    public async Task<DemoDataStatusDto> GetStatusAsync(string companyCode, CancellationToken cancellationToken = default)
    {
        companyCode = NormalizeCompanyCode(companyCode);
        var company = await organizationDbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == companyCode && x.ParentCompanyId == null, cancellationToken);

        if (company is null)
        {
            return new DemoDataStatusDto
            {
                CompanyCode = companyCode,
                Exists = false,
                IsProduction = environment.IsProduction(),
                AllowProductionActions = configuration.GetValue<bool>("DemoData:AllowProduction"),
                DestructiveActionsAllowed = !environment.IsProduction() || configuration.GetValue<bool>("DemoData:AllowProduction")
            };
        }

        var isRecognizedDemoTenant = await IsRecognizedDemoTenantAsync(company.Id, companyCode, cancellationToken);
        var adminIdentity = await GetDemoAdminIdentityAsync(company.Id, companyCode, cancellationToken);
        var demoUserIds = await userManager.Users
            .Where(x => x.CompanyId == company.Id && x.UserName != null && x.UserName.StartsWith("demo."))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var taskNumberPrefix = TaskNumberPrefix(companyCode);

        return new DemoDataStatusDto
        {
            CompanyCode = companyCode,
            CompanyId = company.Id,
            CompanyName = company.Name,
            CompanyNameEng = company.NameEng,
            AdminUserName = adminIdentity.UserName,
            AdminEmail = adminIdentity.Email,
            Exists = true,
            IsRecognizedDemoTenant = isRecognizedDemoTenant,
            IsProduction = environment.IsProduction(),
            AllowProductionActions = configuration.GetValue<bool>("DemoData:AllowProduction"),
            DestructiveActionsAllowed = isRecognizedDemoTenant && (!environment.IsProduction() || configuration.GetValue<bool>("DemoData:AllowProduction")),
            BranchCount = await organizationDbContext.Branches.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            UserCount = demoUserIds.Count,
            EmployeeCount = await employeeDbContext.Employees.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            TaskCount = await taskDbContext.TaskItems.CountAsync(x => x.TaskNumber.StartsWith(taskNumberPrefix) || (companyCode == DefaultCompanyCode && x.TaskNumber.StartsWith("DEMO-TASK-")), cancellationToken),
            ProductSkuCount = await catalogDbContext.ProductSkus.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            CustomerCount = await customerDbContext.Customers.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            SupplierCount = await supplierDbContext.Suppliers.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            WarehouseCount = await inventoryDbContext.Warehouses.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            AccountingDocumentCount = await accountingDbContext.AccountingDocuments.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            JournalEntryCount = await accountingDbContext.JournalEntries.CountAsync(x => x.CompanyId == company.Id, cancellationToken),
            LastKnownMarker = isRecognizedDemoTenant ? DemoMarker : string.Empty
        };
    }

    public async Task<DemoDataOperationResultDto> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await CreateAsync(BuildDefaultCreateRequest(), cancellationToken);
    }

    public async Task<DemoDataOperationResultDto> CreateAsync(DemoDataCreateRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureEnvironmentAllowsAction();
        var context = BuildSeedContext(request);

        var platformUser = await EnsurePlatformSeedUserAsync(cancellationToken);
        using var _ = Impersonate(platformUser.Id, null, PermissionList.GetPlatformPermissions());

        Company? company = null;
        try
        {
            company = await EnsureParentCompanyAsync(context, cancellationToken);
            using var __ = Impersonate(platformUser.Id, company.Id, PermissionList.GetTenantPermissions());

            await EnsureRolesAndUsersAsync(company.Id, context);
            var branches = await EnsureOrganizationAsync(company.Id, cancellationToken);
            await EnsureAccountingAsync(company.Id, branches, cancellationToken);
            var employees = await EnsureEmployeesAsync(company.Id, branches, context, cancellationToken);
            await EnsureTasksAsync(employees, context, cancellationToken);
            var skus = await EnsureCatalogAsync(company.Id, cancellationToken);
            var customers = await EnsureCustomersAsync(company.Id, cancellationToken);
            await EnsureSuppliersAsync(company.Id, cancellationToken);
            await EnsureInventoryAsync(company.Id, branches, cancellationToken);
            await EnsureAccountingActivityAsync(company.Id, branches.First().Id, customers.FirstOrDefault()?.Id, skus.FirstOrDefault()?.Id, context, cancellationToken);
        }
        catch
        {
            if (company is not null)
                await TryPurgeDemoTenantAfterFailedCreateAsync(context.CompanyCode, cancellationToken);

            throw;
        }

        var seededCompany = company ?? throw new InvalidOperationException($"Demo company {context.CompanyCode} was not created.");
        Console.WriteLine($"Demo data ready for {seededCompany.NameEng} ({seededCompany.Code}). Admin: {context.AdminEmail} / {DefaultPassword}");
        return new DemoDataOperationResultDto
        {
            Success = true,
            Message = $"Demo data ready for {seededCompany.NameEng} ({seededCompany.Code}).",
            Status = await GetStatusAsync(context.CompanyCode, cancellationToken)
        };
    }

    public async Task<DemoDataOperationResultDto> ResetAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureEnvironmentAllowsAction();
        companyCode = NormalizeCompanyCode(companyCode);
        var existingStatus = await GetStatusAsync(companyCode, cancellationToken);
        await PurgeDemoTenantAsync(companyCode, request, cancellationToken);
        var result = await CreateAsync(new DemoDataCreateRequestDto
        {
            CompanyCode = companyCode,
            CompanyName = existingStatus.CompanyName ?? string.Empty,
            CompanyNameEng = existingStatus.CompanyNameEng ?? string.Empty
        }, cancellationToken);
        return result with { Message = "Demo data reset successfully." };
    }

    public async Task<DemoDataOperationResultDto> DeleteAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureEnvironmentAllowsAction();
        companyCode = NormalizeCompanyCode(companyCode);
        await PurgeDemoTenantAsync(companyCode, request, cancellationToken);
        return new DemoDataOperationResultDto
        {
            Success = true,
            Message = "Demo data deleted successfully.",
            Status = await GetStatusAsync(companyCode, cancellationToken)
        };
    }

    public async Task<DemoDataOperationResultDto> ResetAdminPasswordAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureEnvironmentAllowsAction();
        companyCode = NormalizeCompanyCode(companyCode);
        if (!string.Equals(NormalizeCompanyCode(request.CompanyCode), companyCode, StringComparison.Ordinal))
            throw new InvalidOperationException($"Confirmation code must exactly match {companyCode}.");

        var company = await organizationDbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == companyCode && x.ParentCompanyId == null, cancellationToken)
            ?? throw new InvalidOperationException($"Demo company {companyCode} was not found.");

        if (!await IsRecognizedDemoTenantAsync(company.Id, companyCode, cancellationToken))
            throw new InvalidOperationException("The selected company does not match the demo data safety markers.");

        await sender.Send(new ResetCompanyAdminPasswordCommand(company.Id, DefaultPassword), cancellationToken);

        return new DemoDataOperationResultDto
        {
            Success = true,
            Message = $"Demo admin password reset for {company.NameEng} ({company.Code}). Password: {DefaultPassword}",
            Status = await GetStatusAsync(companyCode, cancellationToken)
        };
    }

    private async Task<ApplicationUser> EnsurePlatformSeedUserAsync(CancellationToken cancellationToken)
    {
        var userName = DemoActorName;
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = ApplicationUser.Create(
                StableGuid("user", userName),
                userName,
                "demo.seed@alafkar.demo",
                "0500000001",
                UserType.SystemUser,
                "000000",
                OTPType.ConfirmEmail,
                DateTime.UtcNow.AddDays(7),
                null);

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        var role = await CompanyRoleTemplates.EnsurePlatformSystemUserRoleAsync(roleManager);
        if (!await userManager.IsInRoleAsync(user, role.Name!))
        {
            var result = await userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        return user;
    }

    private async Task<Company> EnsureParentCompanyAsync(DemoSeedContext context, CancellationToken cancellationToken)
    {
        var existing = await organizationDbContext.Companies
            .FirstOrDefaultAsync(x => x.Code == context.CompanyCode && x.ParentCompanyId == null, cancellationToken);
        if (existing is not null)
        {
            if (!await IsRecognizedDemoTenantAsync(existing.Id, context.CompanyCode, cancellationToken))
                throw new InvalidOperationException($"Company code {context.CompanyCode} already exists and is not recognized as a demo tenant.");

            await EnsureDemoLicenseMarkerAsync(existing.Id, context, cancellationToken);
            return existing;
        }

        var licenseCategory = await organizationDbContext.LicenseCategories
            .OrderByDescending(x => x.MaxUsers)
            .FirstAsync(cancellationToken);

        var businessLines = await organizationDbContext.BusinessLines
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new LicensedBusinessLineDto
            {
                BusinessLineId = x.Id,
                Key = x.Key,
                Name = x.Name,
                NameAr = x.NameAr,
                ActivationLimit = x.Key == "storefront" ? 3 : 1
            })
            .ToListAsync(cancellationToken);

        var result = await sender.Send(new CreateParentCompanyCommand(new ParentCompanyDto
        {
            Name = context.CompanyName,
            NameEng = context.CompanyNameEng,
            Code = context.CompanyCode,
            Logo = string.Empty,
            HqLocation = "Riyadh, Saudi Arabia",
            HqLatitude = 24.7136,
            HqLongitude = 46.6753,
            VatNo = "300000000000003",
            Phone = "0110000000",
            Email = "info@alafkar.demo",
            TimeZone = "Asia/Riyadh",
            AdminUserName = context.AdminUserName,
            AdminEmail = context.AdminEmail,
            AdminPhoneNumber = context.AdminPhoneNumber,
            AdminTemporaryPassword = DefaultPassword,
            License = new CompanyLicenseDto
            {
                LicenseCategoryId = licenseCategory.Id,
                Status = CompanyLicenseStatus.Active,
                StartDate = DateTime.UtcNow.Date.AddMonths(-1),
                EndDate = DateTime.UtcNow.Date.AddYears(2),
                Notes = context.Marker,
                BusinessLines = businessLines
            }
        }), cancellationToken);

        return await organizationDbContext.Companies
            .FirstAsync(x => x.Id == result.CreatedCompany.Id, cancellationToken);
    }

    private async Task EnsureRolesAndUsersAsync(Guid companyId, DemoSeedContext context)
    {
        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, companyId);

        var demoUsers = new[]
        {
            (context.UserName("hr"), context.Email("hr"), "0501000001", "hr-manager"),
            (context.UserName("accountant"), context.Email("accountant"), "0501000002", "accounting-manager"),
            (context.UserName("sales"), context.Email("sales"), "0501000003", "sales-manager"),
            (context.UserName("procurement"), context.Email("procurement"), "0501000004", "procurement-manager"),
            (context.UserName("cashier"), context.Email("cashier"), "0501000005", "cashier"),
            (context.UserName("employee"), context.Email("employee"), "0501000006", "employee"),
            (context.UserName("approver"), context.Email("approver"), "0501000007", "approver")
        };

        foreach (var (userName, email, phone, roleKey) in demoUsers)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                user = ApplicationUser.Create(
                    StableGuid("user", userName),
                    userName,
                    email,
                    phone,
                    UserType.SystemUser,
                    "000000",
                    OTPType.ConfirmEmail,
                    DateTime.UtcNow.AddDays(7),
                    companyId);

                var createResult = await userManager.CreateAsync(user, DefaultPassword);
                if (!createResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(x => x.Description)));

                user.UpdateOtp("000000", OTPType.ConfirmEmail, DateTime.UtcNow.AddDays(7), true);
                await userManager.UpdateAsync(user);
            }

            var roleName = CompanyRoleTemplates.BuildCompanyRoleName(companyId, roleKey);
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var addResult = await userManager.AddToRoleAsync(user, roleName);
                if (!addResult.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", addResult.Errors.Select(x => x.Description)));
            }
        }
    }

    private async Task<List<Branch>> EnsureOrganizationAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var mainBranch = await sender.Send(new EnsureMainBranchCommand(companyId, DemoActorName), cancellationToken);
        var company = await organizationDbContext.Companies.AsNoTracking().FirstAsync(x => x.Id == companyId, cancellationToken);

        var branchSpecs = new[]
        {
            ("MAIN", company.Name, company.NameEng, company.HqLocation, company.Phone, company.Email, true),
            ("RYD-SALES", "فرع الرياض للمبيعات", "Riyadh Sales Branch", "Riyadh Front", "0110000100", "riyadh.sales@alafkar.demo", false),
            ("JED-OPS", "فرع جدة للعمليات", "Jeddah Operations Branch", "Jeddah Industrial City", "0120000100", "jeddah.ops@alafkar.demo", false),
            ("DMM-SVC", "فرع الدمام للخدمات", "Dammam Service Branch", "Dammam Corniche", "0130000100", "dammam.service@alafkar.demo", false)
        };

        foreach (var (code, name, nameEng, location, phone, email, isMain) in branchSpecs)
        {
            var branch = await organizationDbContext.Branches.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Code == code, cancellationToken);
            if (branch is null)
            {
                branch = Branch.Create(
                    code == "MAIN" ? mainBranch.BranchId : StableGuid("branch", companyId, code),
                    name,
                    nameEng,
                    location,
                    46.6753,
                    24.7136,
                    code,
                    phone,
                    email,
                    isMain,
                    companyId,
                    DemoActorName);
                await organizationDbContext.Branches.AddAsync(branch, cancellationToken);
            }
        }

        await organizationDbContext.SaveChangesAsync(cancellationToken);

        var branches = await organizationDbContext.Branches
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.IsMainBranch)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);

        foreach (var branch in branches)
        {
            await sender.Send(new EnsureBranchAccountingCommand(branch.CompanyId, branch.Id, branch.Code, branch.Name, branch.NameEng), cancellationToken);
        }

        var adminSpecs = new[]
        {
            ("ADM-EXEC", "الإدارة التنفيذية", "Executive Office", branches[0].Id, true),
            ("ADM-HR", "إدارة الموارد البشرية", "Human Resources", branches[0].Id, false),
            ("ADM-FIN", "الإدارة المالية", "Finance", branches[0].Id, false),
            ("ADM-SALES", "إدارة المبيعات", "Sales", branches.First(x => x.Code == "RYD-SALES").Id, false),
            ("ADM-OPS", "إدارة العمليات", "Operations", branches.First(x => x.Code == "JED-OPS").Id, false)
        };

        foreach (var (code, name, nameEng, branchId, higher) in adminSpecs)
        {
            if (!await organizationDbContext.Administrations.AnyAsync(x => x.CompanyId == companyId && x.Code == code, cancellationToken))
            {
                await organizationDbContext.Administrations.AddAsync(Administration.Create(
                    StableGuid("admin", companyId, code),
                    name,
                    nameEng,
                    code,
                    branchId,
                    null,
                    null,
                    higher,
                    true,
                    companyId,
                    DemoActorName), cancellationToken);
            }
        }

        await organizationDbContext.SaveChangesAsync(cancellationToken);

        var administrations = await organizationDbContext.Administrations.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
        var departmentSpecs = new[]
        {
            ("DEP-HR", "قسم شؤون الموظفين", "People Operations", "ADM-HR"),
            ("DEP-PAY", "قسم الرواتب", "Payroll", "ADM-FIN"),
            ("DEP-AR", "قسم الحسابات المدينة", "Accounts Receivable", "ADM-FIN"),
            ("DEP-AP", "قسم الحسابات الدائنة", "Accounts Payable", "ADM-FIN"),
            ("DEP-SALES", "قسم المبيعات", "Sales Team", "ADM-SALES"),
            ("DEP-PROC", "قسم المشتريات", "Procurement", "ADM-OPS"),
            ("DEP-WH", "قسم المستودعات", "Warehouse Operations", "ADM-OPS")
        };

        foreach (var (code, name, nameEng, adminCode) in departmentSpecs)
        {
            if (!await organizationDbContext.Departments.AnyAsync(x => x.CompanyId == companyId && x.Code == code, cancellationToken))
            {
                var administration = administrations.First(x => x.Code == adminCode);
                await organizationDbContext.Departments.AddAsync(Department.Create(
                    StableGuid("department", companyId, code),
                    name,
                    nameEng,
                    code,
                    administration.Id,
                    null,
                    companyId,
                    true,
                    null,
                    administration.NameEng,
                    46.6753,
                    24.7136,
                    150,
                    DemoActorName), cancellationToken);
            }
        }

        await organizationDbContext.SaveChangesAsync(cancellationToken);
        return branches;
    }

    private async Task EnsureAccountingAsync(Guid companyId, List<Branch> branches, CancellationToken cancellationToken)
    {
        if (!await accountingDbContext.Accounts.AnyAsync(x => x.CompanyId == companyId, cancellationToken))
        {
            await sender.Send(new ApplyAccountingTemplateCommand(new ApplyAccountingTemplateDto
            {
                CompanyId = companyId,
                TemplateCode = "SA_SME",
                FiscalYearStart = new DateTime(DateTime.UtcNow.Year, 1, 1),
                CreateDefaultJournals = true
            }), cancellationToken);
        }

        foreach (var branch in branches)
        {
            await sender.Send(new EnsureBranchAccountingCommand(branch.CompanyId, branch.Id, branch.Code, branch.Name, branch.NameEng), cancellationToken);
        }
    }

    private async Task<List<Employee>> EnsureEmployeesAsync(Guid companyId, List<Branch> branches, DemoSeedContext context, CancellationToken cancellationToken)
    {
        var positionSpecs = new[]
        {
            ("POS-HR-MGR", "مدير الموارد البشرية", "HR Manager", 24000m),
            ("POS-ACC-MGR", "مدير الحسابات", "Accounting Manager", 26000m),
            ("POS-SALES-MGR", "مدير المبيعات", "Sales Manager", 23000m),
            ("POS-PROC-MGR", "مدير المشتريات", "Procurement Manager", 22000m),
            ("POS-CASHIER", "أمين صندوق", "Cashier", 8000m),
            ("POS-WH", "مشرف مستودع", "Warehouse Supervisor", 11000m),
            ("POS-EMP", "موظف عمليات", "Operations Employee", 9000m)
        };

        foreach (var (code, title, titleEng, salary) in positionSpecs)
        {
            if (!await employeeDbContext.Positions.AnyAsync(x => x.CompanyId == companyId && x.Code == code, cancellationToken))
            {
                await employeeDbContext.Positions.AddAsync(Position.Create(
                    StableGuid("position", companyId, code),
                    title,
                    titleEng,
                    code,
                    salary,
                    companyId,
                    DemoActorName), cancellationToken);
            }
        }

        var specialization = await employeeDbContext.Specializations.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.NameEng == "Business Administration", cancellationToken);
        if (specialization is null)
        {
            specialization = Specialization.Create(StableGuid("specialization", companyId, "business"), "إدارة الأعمال", "Business Administration", companyId, DemoActorName);
            await employeeDbContext.Specializations.AddAsync(specialization, cancellationToken);
        }

        var institution = await employeeDbContext.AcademicInstitutions.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.NameEng == "King Saud University", cancellationToken);
        if (institution is null)
        {
            institution = AcademicInstitution.Create(StableGuid("institution", companyId, "ksu"), "جامعة الملك سعود", "King Saud University", companyId, DemoActorName);
            await employeeDbContext.AcademicInstitutions.AddAsync(institution, cancellationToken);
        }

        await employeeDbContext.SaveChangesAsync(cancellationToken);

        var positions = await employeeDbContext.Positions.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
        var departments = await organizationDbContext.Departments.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
        var administrations = await organizationDbContext.Administrations.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
        var users = await userManager.Users.Where(x => x.CompanyId == companyId && x.UserName!.StartsWith("demo.")).ToListAsync(cancellationToken);

        var employeeSpecs = new[]
        {
            ("EMP-001", "سارة", "Sara", "عبدالله", "Abdullah", "العتيبي", "Alotaibi", context.UserName("hr"), "POS-HR-MGR", "DEP-HR"),
            ("EMP-002", "خالد", "Khalid", "محمد", "Mohammed", "القحطاني", "Alqahtani", context.UserName("accountant"), "POS-ACC-MGR", "DEP-AR"),
            ("EMP-003", "نورة", "Noura", "سالم", "Salem", "الحربي", "Alharbi", context.UserName("sales"), "POS-SALES-MGR", "DEP-SALES"),
            ("EMP-004", "فيصل", "Faisal", "ناصر", "Nasser", "الدوسري", "Aldosari", context.UserName("procurement"), "POS-PROC-MGR", "DEP-PROC"),
            ("EMP-005", "ريم", "Reem", "علي", "Ali", "الزهراني", "Alzahrani", context.UserName("cashier"), "POS-CASHIER", "DEP-SALES"),
            ("EMP-006", "ماجد", "Majed", "حسن", "Hassan", "الشمري", "Alshammari", context.UserName("employee"), "POS-WH", "DEP-WH"),
            ("EMP-007", "عبدالعزيز", "Abdulaziz", "فهد", "Fahad", "الغامدي", "Alghamdi", context.UserName("approver"), "POS-EMP", "DEP-PAY")
        };

        foreach (var spec in employeeSpecs)
        {
            if (await employeeDbContext.Employees.AnyAsync(x => x.CompanyId == companyId && x.EmployeeNo == spec.Item1, cancellationToken))
                continue;

            var department = departments.First(x => x.Code == spec.Item10);
            var administration = administrations.First(x => x.Id == department.AdministrationId);
            var branch = branches.First(x => x.Id == administration.BranchId);
            var position = positions.First(x => x.Code == spec.Item9);
            var user = users.FirstOrDefault(x => x.UserName == spec.Item8);

            await employeeDbContext.Employees.AddAsync(Employee.Create(
                StableGuid("employee", companyId, spec.Item1),
                spec.Item1,
                spec.Item2,
                spec.Item3,
                spec.Item4,
                spec.Item5,
                spec.Item6,
                spec.Item7,
                null,
                user?.Email ?? $"{spec.Item1.ToLowerInvariant()}@alafkar.demo",
                user?.PhoneNumber ?? "0501999999",
                DateTime.UtcNow.Date.AddYears(-32),
                $"10{Math.Abs(StableGuid("national", spec.Item1).GetHashCode()):000000000}",
                DateTime.UtcNow.Date.AddYears(-2),
                companyId,
                branch.Id,
                administration.Id,
                department.Id,
                position.Id,
                null,
                "G5",
                branch.NameEng,
                user?.Id,
                IdentityType.Iqama,
                spec.Item1 is "EMP-001" or "EMP-003" or "EMP-005" ? Gender.Female : Gender.Male,
                spec.Item1,
                "Saudi",
                "Riyadh, Saudi Arabia",
                MaritalStatus.Married,
                EmploymentType.Full,
                EmployeeAttendanceType.FixedLocation,
                150,
                Qualification.Bachelor,
                specialization.Id,
                institution.Id,
                2015,
                DemoActorName), cancellationToken);
        }

        await employeeDbContext.SaveChangesAsync(cancellationToken);
        return await employeeDbContext.Employees.Where(x => x.CompanyId == companyId).OrderBy(x => x.EmployeeNo).ToListAsync(cancellationToken);
    }

    private async Task EnsureTasksAsync(List<Employee> employees, DemoSeedContext context, CancellationToken cancellationToken)
    {
        if (employees.Count == 0)
            return;

        var assignedBy = employees[0].LinkedUserId ?? StableGuid("user", context.UserName("hr"));
        var assignedTo = employees.FirstOrDefault(x => x.LinkedUserId.HasValue)?.LinkedUserId?.ToString() ?? assignedBy.ToString();
        var departmentId = employees.FirstOrDefault(x => x.DepartmentId.HasValue)?.DepartmentId;
        var today = DateTime.UtcNow.Date;

        var taskSpecs = new[]
        {
            (context.TaskNumber("001"), "Prepare onboarding checklist", "New joiner onboarding tasks for HR demo.", TaskPriority.Normal, TaskWorkflowStatus.Assigned, 0m, today, today.AddDays(5)),
            (context.TaskNumber("002"), "Review overdue supplier invoice", "Finance task intentionally overdue for dashboard counters.", TaskPriority.High, TaskWorkflowStatus.Overdue, 25m, today.AddDays(-7), today.AddDays(-3)),
            (context.TaskNumber("003"), "Update sales price list", "Sales operations task in progress.", TaskPriority.Normal, TaskWorkflowStatus.InProgress, 60m, today.AddDays(-2), today.AddDays(2)),
            (context.TaskNumber("004"), "Complete monthly attendance audit", "Completed HR control task.", TaskPriority.Low, TaskWorkflowStatus.Completed, 100m, today.AddDays(-5), today.AddDays(-1)),
            (context.TaskNumber("005"), "Cancelled warehouse cycle count", "Cancelled task for status filtering.", TaskPriority.Normal, TaskWorkflowStatus.Cancelled, 0m, today.AddDays(-1), today.AddDays(4))
        };

        foreach (var (number, title, description, priority, status, progress, startDate, dueDate) in taskSpecs)
        {
            if (await taskDbContext.TaskItems.AnyAsync(x => x.TaskNumber == number, cancellationToken))
                continue;

            var task = TaskItem.Create(
                number,
                title,
                description,
                priority,
                startDate,
                dueDate,
                assignedBy,
                assignedTo,
                assignedBy,
                departmentId,
                false,
                dueDate.AddDays(-1));

            if (status == TaskWorkflowStatus.Overdue)
                task.MarkOverdue(assignedBy);
            else if (status is TaskWorkflowStatus.Completed or TaskWorkflowStatus.Cancelled)
                task.ChangeStatus(status, assignedBy);
            else if (progress > 0)
                task.UpdateProgress(progress, assignedBy);

            task.AddAction(TaskActionItem.Create(task.Id, "Initial review", dueDate.AddDays(-1), assignedBy, DemoActorName));
            await taskDbContext.TaskItems.AddAsync(task, cancellationToken);
        }

        await taskDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<ProductSku>> EnsureCatalogAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var category = await catalogDbContext.Categories.FirstAsync(cancellationToken);
        var brand = await catalogDbContext.Brands.FirstAsync(cancellationToken);
        var unit = await catalogDbContext.Units.FirstAsync(cancellationToken);

        var productSpecs = new[]
        {
            ("DEMO-PROD-COFFEE", "قهوة مختصة", "Specialty Coffee", "DEMO-SKU-COFFEE-1KG", 58m),
            ("DEMO-PROD-POS", "جهاز نقاط بيع", "POS Terminal", "DEMO-SKU-POS-DEVICE", 1450m),
            ("DEMO-PROD-SVC", "خدمة إعداد النظام", "Implementation Service", "DEMO-SKU-SETUP-SVC", 2500m)
        };

        foreach (var (productKey, name, nameEng, skuCode, price) in productSpecs)
        {
            var product = await catalogDbContext.Products.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.NameEng == nameEng, cancellationToken);
            if (product is null)
            {
                product = Product.Create(StableGuid("product", companyId, productKey), name, nameEng, category.Id, companyId, DemoActorName);
                await catalogDbContext.Products.AddAsync(product, cancellationToken);
            }

            if (!await catalogDbContext.ProductSkus.AnyAsync(x => x.CompanyId == companyId && x.SkuCode == skuCode, cancellationToken))
            {
                await catalogDbContext.ProductSkus.AddAsync(ProductSku.Create(
                    StableGuid("sku", companyId, skuCode),
                    product.Id,
                    brand.Id,
                    unit.Id,
                    null,
                    name,
                    nameEng,
                    skuCode,
                    skuCode,
                    skuCode,
                    null,
                    string.Empty,
                    price,
                    null,
                    SkuProductionType.PurchasedRawMaterial,
                    true,
                    true,
                    !productKey.EndsWith("SVC", StringComparison.Ordinal),
                    !productKey.EndsWith("SVC", StringComparison.Ordinal),
                    false,
                    companyId,
                    DemoActorName), cancellationToken);
            }
        }

        await catalogDbContext.SaveChangesAsync(cancellationToken);
        return await catalogDbContext.ProductSkus.Where(x => x.CompanyId == companyId).OrderBy(x => x.SkuCode).ToListAsync(cancellationToken);
    }

    private async Task<List<Customer>> EnsureCustomersAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var group = await customerDbContext.CustomerGroups.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.NameEng == "Demo Retail", cancellationToken);
        if (group is null)
        {
            group = CustomerGroup.Create(StableGuid("customer-group", companyId, "retail"), "عملاء التجزئة", "Demo Retail", "Retail customers for demo flows.", 5, null, companyId, DemoActorName);
            await customerDbContext.CustomerGroups.AddAsync(group, cancellationToken);
        }

        var customerSpecs = new[]
        {
            ("DEMO-CUST-001", "Riyadh Modern Trading", "300000000000011", 50000m),
            ("DEMO-CUST-002", "Jeddah Hospitality Group", "300000000000012", 75000m),
            ("DEMO-CUST-003", "Eastern Retail Co.", "300000000000013", 25000m)
        };

        foreach (var (code, name, vat, limit) in customerSpecs)
        {
            if (await customerDbContext.Customers.AnyAsync(x => x.CompanyId == companyId && x.CustomerCode == code, cancellationToken))
                continue;

            var customer = Customer.Create(
                StableGuid("customer", companyId, code),
                name,
                code,
                name,
                vat,
                $"CR-{code[^3..]}",
                CustomerStatus.Active,
                limit,
                PaymentTermType.Net30,
                CreditStatus.Good,
                null,
                limit,
                "Demo customer with active credit.",
                false,
                companyId,
                group.Id,
                DemoActorName);
            customer.AddContact("Demo Buyer", "Procurement Lead", $"buyer.{code[^3..]}@alafkar.demo", "0502000000", true, DemoActorName);
            customer.AddAddress("Main Office", "King Fahd Road", null, 46.6753, 24.7136, "Riyadh", "Riyadh", "Saudi Arabia", "12345", true, true, DemoActorName);
            await customerDbContext.Customers.AddAsync(customer, cancellationToken);
        }

        await customerDbContext.SaveChangesAsync(cancellationToken);
        return await customerDbContext.Customers.Where(x => x.CompanyId == companyId).OrderBy(x => x.CustomerCode).ToListAsync(cancellationToken);
    }

    private async Task EnsureSuppliersAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var group = await supplierDbContext.SupplierGroups.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Name == "Demo Local Suppliers", cancellationToken);
        if (group is null)
        {
            group = SupplierGroup.Create(StableGuid("supplier-group", companyId, "local"), "Demo Local Suppliers", "Demo vendors for procurement and AP.", null, SupplierPaymentTermType.Net30, companyId, DemoActorName);
            await supplierDbContext.SupplierGroups.AddAsync(group, cancellationToken);
        }

        var supplierSpecs = new[]
        {
            ("DEMO-SUP-001", "Saudi Office Supplies", SupplierType.Local),
            ("DEMO-SUP-002", "Gulf Equipment Trading", SupplierType.Distributor),
            ("DEMO-SUP-003", "Riyadh Logistics Partner", SupplierType.ServiceProvider)
        };

        foreach (var (code, name, type) in supplierSpecs)
        {
            if (await supplierDbContext.Suppliers.AnyAsync(x => x.CompanyId == companyId && x.SupplierCode == code, cancellationToken))
                continue;

            var supplier = Supplier.Create(name, name, code, group.Id, SupplierStatus.Active, type, SupplierPaymentTermType.Net30, "300000000000021", 40000, 0, "Demo supplier.", companyId, DemoActorName);
            supplier.AddContact("Demo Supplier Contact", "Account Manager", $"contact.{code[^3..]}@alafkar.demo", "0503000000", true, DemoActorName);
            supplier.AddAddress("Head Office", "Industrial Area", null, 46.6753, 24.7136, "Riyadh", "Riyadh", "Saudi Arabia", "12345", true, DemoActorName);
            await supplierDbContext.Suppliers.AddAsync(supplier, cancellationToken);
        }

        await supplierDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureInventoryAsync(Guid companyId, List<Branch> branches, CancellationToken cancellationToken)
    {
        foreach (var branch in branches.Take(3))
        {
            var code = $"DEMO-WH-{branch.Code}";
            if (await inventoryDbContext.Warehouses.AnyAsync(x => x.CompanyId == companyId && x.NameEng == code, cancellationToken))
                continue;

            await inventoryDbContext.Warehouses.AddAsync(Warehouse.Create(
                StableGuid("warehouse", companyId, code),
                $"مستودع {branch.Name}",
                code,
                branch.Location,
                branch.Location,
                branch.Longitude,
                branch.Latitude,
                companyId,
                branch.Id,
                WarehouseType.Commercial,
                DemoActorName), cancellationToken);
        }

        await inventoryDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAccountingActivityAsync(Guid companyId, Guid branchId, Guid? customerId, Guid? skuId, DemoSeedContext context, CancellationToken cancellationToken)
    {
        if (await accountingDbContext.AccountingDocuments.AnyAsync(x => x.CompanyId == companyId && x.SourceModule == "DemoData", cancellationToken))
            return;

        var salesDocument = new AccountingDocumentDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            Type = AccountingDocumentType.SalesInvoice,
            DocumentDate = DateTime.UtcNow.Date.AddDays(-5),
            PartyId = customerId,
            PartyName = "Riyadh Modern Trading",
            PartyVatNumber = "300000000000011",
            SourceModule = "DemoData",
            SourceDocumentId = StableGuid("demo-doc", companyId, "sales"),
            SourceDocumentNumber = context.DocumentNumber("SINV-001"),
            Lines =
            [
                new AccountingDocumentLineDto
                {
                    LineNumber = 1,
                    Description = "Demo specialty coffee sale",
                    ProductSkuId = skuId,
                    Quantity = 10,
                    UnitPrice = 58,
                    TaxRate = 15
                }
            ]
        };

        var salesResult = await sender.Send(new CreateAccountingDocumentCommand(salesDocument), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(salesResult.Id), cancellationToken);

        var branch = await organizationDbContext.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Id == branchId, cancellationToken)
            ?? throw new InvalidOperationException($"Demo branch {branchId} was not found for company {companyId}.");

        await sender.Send(new EnsureBranchAccountingCommand(branch.CompanyId, branch.Id, branch.Code, branch.Name, branch.NameEng), cancellationToken);

        var cashAccountId = await accountingDbContext.CashAccounts
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.DisplayName)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Demo branch accounting did not create an active cash account for branch {branch.Code}.");

        await sender.Send(new CreateBankTransactionCommand(new BankTransactionDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            CashAccountId = cashAccountId,
            TransactionDate = DateTime.UtcNow.Date.AddDays(-4),
            Description = "Demo customer receipt for SINV-001",
            ReferenceNumber = context.DocumentNumber("REC-001"),
            Amount = 667m
        }), cancellationToken);
    }

    private string ResolveCompanyCode()
    {
        var companyCode = configuration["DemoData:CompanyCode"]?.Trim();
        return string.IsNullOrWhiteSpace(companyCode) ? DefaultCompanyCode : NormalizeCompanyCode(companyCode);
    }

    private DemoDataCreateRequestDto BuildDefaultCreateRequest()
    {
        var companyCode = ResolveCompanyCode();
        return new DemoDataCreateRequestDto
        {
            CompanyCode = companyCode,
            CompanyName = "شركة العرض الشامل",
            CompanyNameEng = "Alafkar Demo Holding",
            DisplayLabel = "Default Demo"
        };
    }

    private DemoSeedContext BuildSeedContext(DemoDataCreateRequestDto request)
    {
        var companyCode = NormalizeCompanyCode(request.CompanyCode);
        var slug = CreateCodeSlug(companyCode);
        var companyNameEng = string.IsNullOrWhiteSpace(request.CompanyNameEng)
            ? $"Alafkar Demo Holding {companyCode}"
            : request.CompanyNameEng.Trim();
        var companyName = string.IsNullOrWhiteSpace(request.CompanyName)
            ? companyNameEng
            : request.CompanyName.Trim();
        var adminUserName = string.IsNullOrWhiteSpace(request.AdminUserName)
            ? $"demo.admin.{slug}"
            : request.AdminUserName.Trim();
        var adminEmail = string.IsNullOrWhiteSpace(request.AdminEmail)
            ? $"demo.admin.{slug}@alafkar.demo"
            : request.AdminEmail.Trim();
        var displayLabel = string.IsNullOrWhiteSpace(request.DisplayLabel)
            ? companyNameEng
            : request.DisplayLabel.Trim();

        return new DemoSeedContext(
            companyCode,
            companyName,
            companyNameEng,
            adminUserName,
            adminEmail,
            "0500000002",
            displayLabel,
            slug,
            $"Demo license with all available business lines. {DemoMarker}; CompanyCode={companyCode}; Label={displayLabel}.");
    }

    private static string NormalizeCompanyCode(string? companyCode)
    {
        if (string.IsNullOrWhiteSpace(companyCode))
            throw new InvalidOperationException("Company code is required.");

        var normalized = companyCode.Trim().ToUpperInvariant();
        if (normalized.Length > 64)
            throw new InvalidOperationException("Company code must be 64 characters or fewer.");

        if (normalized.Any(ch => !char.IsLetterOrDigit(ch) && ch != '-' && ch != '_'))
            throw new InvalidOperationException("Company code may contain only letters, numbers, hyphens, and underscores.");

        return normalized;
    }

    private static string CreateCodeSlug(string companyCode)
    {
        var builder = new StringBuilder(companyCode.Length);
        foreach (var ch in companyCode.ToLowerInvariant())
        {
            builder.Append(char.IsLetterOrDigit(ch) ? ch : '.');
        }

        return builder.ToString().Trim('.');
    }

    private static string TaskNumberPrefix(string companyCode) => $"DEMO-{NormalizeCompanyCode(companyCode)}-TASK-";

    private static DemoDataSummaryDto ToSummary(DemoDataStatusDto status) => new()
    {
        CompanyCode = status.CompanyCode,
        CompanyId = status.CompanyId,
        CompanyName = status.CompanyName,
        CompanyNameEng = status.CompanyNameEng,
        AdminUserName = status.AdminUserName,
        AdminEmail = status.AdminEmail,
        Exists = status.Exists,
        IsRecognizedDemoTenant = status.IsRecognizedDemoTenant,
        IsProduction = status.IsProduction,
        AllowProductionActions = status.AllowProductionActions,
        DestructiveActionsAllowed = status.DestructiveActionsAllowed,
        BranchCount = status.BranchCount,
        UserCount = status.UserCount,
        EmployeeCount = status.EmployeeCount,
        TaskCount = status.TaskCount,
        ProductSkuCount = status.ProductSkuCount,
        CustomerCount = status.CustomerCount,
        SupplierCount = status.SupplierCount,
        WarehouseCount = status.WarehouseCount,
        AccountingDocumentCount = status.AccountingDocumentCount,
        JournalEntryCount = status.JournalEntryCount,
        LastKnownMarker = status.LastKnownMarker
    };

    private async Task<(string? UserName, string? Email)> GetDemoAdminIdentityAsync(Guid companyId, string companyCode, CancellationToken cancellationToken)
    {
        companyCode = NormalizeCompanyCode(companyCode);
        var slug = CreateCodeSlug(companyCode);
        var expectedUserName = $"demo.admin.{slug}";
        var expectedEmail = $"demo.admin.{slug}@alafkar.demo";

        var expectedAdmin = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId
                && x.UserName != null
                && x.Email != null
                && ((x.UserName == expectedUserName && x.Email == expectedEmail)
                    || (companyCode == DefaultCompanyCode && x.UserName == "demo.admin" && x.Email == "demo.admin@alafkar.demo")), cancellationToken);

        if (expectedAdmin is not null)
            return (expectedAdmin.UserName, expectedAdmin.Email);

        var roleName = CompanyRoleTemplates.BuildSystemAdminRoleName(companyId);
        var roleAdmins = await userManager.GetUsersInRoleAsync(roleName);
        var admin = roleAdmins.FirstOrDefault(x => x.CompanyId == companyId && x.UserName != null && x.UserName.StartsWith("demo.admin"));
        return (admin?.UserName, admin?.Email);
    }

    private async Task EnsureDemoLicenseMarkerAsync(Guid companyId, DemoSeedContext context, CancellationToken cancellationToken)
    {
        var license = await organizationDbContext.CompanyLicenses
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        if (license is null || (license.Notes?.Contains(context.Marker, StringComparison.Ordinal) ?? false))
            return;

        var notes = string.IsNullOrWhiteSpace(license.Notes)
            ? context.Marker
            : $"{license.Notes} {context.Marker}";

        license.Update(
            license.Status,
            license.PlanKey,
            license.PlanName,
            license.StartDate,
            license.EndDate,
            license.MaxUsers,
            license.MaxChildCompanies,
            license.MaxBranches,
            notes,
            DemoActorName,
            license.LicenseCategoryId);

        await organizationDbContext.SaveChangesAsync(cancellationToken);
    }

    private void EnsureEnvironmentAllowsAction()
    {
        if (environment.IsProduction() && !configuration.GetValue<bool>("DemoData:AllowProduction"))
            throw new InvalidOperationException("Demo data management is disabled in production unless DemoData:AllowProduction is true.");
    }

    private async Task PurgeDemoTenantAsync(string companyCode, DemoDataConfirmationRequestDto request, CancellationToken cancellationToken)
    {
        companyCode = NormalizeCompanyCode(companyCode);
        if (!string.Equals(NormalizeCompanyCode(request.CompanyCode), companyCode, StringComparison.Ordinal))
            throw new InvalidOperationException($"Confirmation code must exactly match {companyCode}.");

        var company = await organizationDbContext.Companies
            .FirstOrDefaultAsync(x => x.Code == companyCode && x.ParentCompanyId == null, cancellationToken);

        if (company is null)
            return;

        if (!await IsRecognizedDemoTenantAsync(company.Id, companyCode, cancellationToken))
            throw new InvalidOperationException("The selected company does not match the demo data safety markers.");

        var companyId = company.Id;

        var eInvoiceIds = await accountingDbContext.EInvoices
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        await accountingDbContext.EInvoiceSubmissions.Where(x => eInvoiceIds.Contains(x.EInvoiceId)).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.EInvoices.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.BankTransactions.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.AccountingDocuments.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.JournalEntries.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.ZatcaDevices.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.ZatcaSettings.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.CashAccounts.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.BankAccounts.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.PostingProfiles.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.TaxCodes.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.FiscalPeriods.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.CompanyAccountingSettings.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.AccountCodingSettings.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await accountingDbContext.Accounts.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        var warehouseIds = await inventoryDbContext.Warehouses
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var batchIds = await inventoryDbContext.Batches
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var inventorySkuIds = await catalogDbContext.ProductSkus
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        await inventoryDbContext.InventoryValuationLayers.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.LandedCostVouchers.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.QualityInspections.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.PutawayRules.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.TransferItems.Where(x => batchIds.Contains(x.BatchId) || inventorySkuIds.Contains(x.ProductSkuId)).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.WarehouseTransfers.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.StockMovements.Where(x => warehouseIds.Contains(x.WarehouseId) || batchIds.Contains(x.BatchId) || inventorySkuIds.Contains(x.ProductSkuId)).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.InventorySnapshots.Where(x => warehouseIds.Contains(x.WarehouseId) || batchIds.Contains(x.BatchId) || inventorySkuIds.Contains(x.ProductSkuId)).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.Inventories.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.BatchStocks.Where(x => warehouseIds.Contains(x.WarehouseId) || batchIds.Contains(x.BatchId)).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.Batches.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.AssetInstances.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.WarehouseLocations.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await inventoryDbContext.Warehouses.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        var taskNumberPrefix = TaskNumberPrefix(companyCode);
        await taskDbContext.TaskItems
            .Where(x => x.TaskNumber.StartsWith(taskNumberPrefix) || (companyCode == DefaultCompanyCode && x.TaskNumber.StartsWith("DEMO-TASK-")))
            .ExecuteDeleteAsync(cancellationToken);

        await supplierDbContext.Suppliers.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await supplierDbContext.SupplierGroups.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        await customerDbContext.Customers.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await customerDbContext.CustomerPricingProfiles.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await customerDbContext.CustomerGroups.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        var productSkuIds = await catalogDbContext.ProductSkus
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var productPackageIds = await catalogDbContext.ProductPackages
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        await catalogDbContext.ProductSkuPackages.Where(x => productSkuIds.Contains(x.ProductSkuId) || productPackageIds.Contains(x.ProductPackageId)).ExecuteDeleteAsync(cancellationToken);
        await catalogDbContext.ProductSkuVariants.Where(x => productSkuIds.Contains(x.ProductSkuId)).ExecuteDeleteAsync(cancellationToken);
        await catalogDbContext.ProductSkuComponents.Where(x => productSkuIds.Contains(x.ParentProductSkuId) || productSkuIds.Contains(x.ComponentProductSkuId)).ExecuteDeleteAsync(cancellationToken);
        await catalogDbContext.ProductSkus.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await catalogDbContext.Products.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        var teamIds = await employeeDbContext.EmployeeTeams
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        await employeeDbContext.EmployeeCertifications.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.EmployeeSkills.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.EmployeeDocumentLinks.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.EmployeeEmergencyContacts.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.HrLifecycleEvents.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.EmployeeTeamMembers.Where(x => teamIds.Contains(x.TeamId)).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.EmployeeTeams.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.Employees.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.Positions.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.Specializations.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await employeeDbContext.AcademicInstitutions.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        await organizationDbContext.UserBranchRoleAssignments.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.UserBranchAssignments.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.Departments.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.Administrations.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.Branches.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);

        var licenseIds = await organizationDbContext.CompanyLicenses
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        await organizationDbContext.CompanyLicenseBusinessLines.Where(x => licenseIds.Contains(x.CompanyLicenseId)).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.CompanyLicenses.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.BusinessLineActivations.Where(x => x.CompanyId == companyId).ExecuteDeleteAsync(cancellationToken);
        await organizationDbContext.Companies.Where(x => x.Id == companyId).ExecuteDeleteAsync(cancellationToken);

        await DeleteDemoUsersAndRolesAsync(companyId, cancellationToken);
    }

    private async Task TryPurgeDemoTenantAfterFailedCreateAsync(string companyCode, CancellationToken cancellationToken)
    {
        try
        {
            await PurgeDemoTenantAsync(
                companyCode,
                new DemoDataConfirmationRequestDto { CompanyCode = companyCode },
                cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Demo data cleanup failed for {companyCode}: {ex.Message}");
        }
    }

    private async Task DeleteDemoUsersAndRolesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .Where(x => x.CompanyId == companyId)
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        var roles = await roleManager.Roles
            .Where(x => x.CompanyId == companyId)
            .ToListAsync(cancellationToken);

        foreach (var role in roles)
        {
            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    private async Task<bool> IsRecognizedDemoTenantAsync(Guid companyId, string companyCode, CancellationToken cancellationToken)
    {
        companyCode = NormalizeCompanyCode(companyCode);

        var hasMarker = await organizationDbContext.CompanyLicenses
            .AnyAsync(x => x.CompanyId == companyId
                && x.Notes != null
                && x.Notes.Contains(DemoMarker)
                && x.Notes.Contains($"CompanyCode={companyCode}"), cancellationToken);

        var hasLegacyMarker = companyCode == DefaultCompanyCode && await organizationDbContext.CompanyLicenses
            .AnyAsync(x => x.CompanyId == companyId && x.Notes != null && x.Notes.Contains(DemoMarker), cancellationToken);

        return hasMarker || hasLegacyMarker;
    }

    private IDisposable Impersonate(Guid userId, Guid? companyId, IEnumerable<string> permissions)
    {
        var previous = httpContextAccessor.HttpContext;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, DemoActorName)
        };

        if (companyId.HasValue)
            claims.Add(new Claim("company_id", companyId.Value.ToString()));

        claims.AddRange(permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .Distinct(StringComparer.Ordinal)
            .Select(permission => new Claim("Permission", permission)));

        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "DemoData"))
        };

        return new RestoreHttpContext(httpContextAccessor, previous);
    }

    private static Guid StableGuid(params object[] parts)
    {
        var input = string.Join(":", parts.Select(x => x?.ToString() ?? string.Empty));
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }

    private sealed class RestoreHttpContext(IHttpContextAccessor accessor, HttpContext? previous) : IDisposable
    {
        public void Dispose() => accessor.HttpContext = previous;
    }
}
