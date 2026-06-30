using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Accounting.Accounting.Features;
using Accounting.Contracts.Accounting.Features;
using Accounting.Data;
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
using SharedWithUI.Inventory.Enums;
using SharedWithUI.Organization.Dtos;
using SharedWithUI.Organization.Enums;
using SharedWithUI.Suppliers.Enums;
using SharedWithUI.TaskManagement.Enums;
using SuppliersModule.Data;
using SuppliersModule.Suppliers.Models;
using TaskManagement.Data;
using TaskManagement.Tasks.Models;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace Api.DemoData;

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
    AccountingDbContext accountingDbContext)
{
    private const string DemoActorName = "demo.seed";
    private const string DefaultCompanyCode = "DEMO-ERP";
    private const string DefaultPassword = "Demo@12345";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!configuration.GetValue<bool>("DemoData:Enabled"))
            return;

        if (environment.IsProduction() && !configuration.GetValue<bool>("DemoData:AllowProduction"))
            throw new InvalidOperationException("Demo data seeding is disabled in production unless DemoData:AllowProduction is true.");

        var companyCode = configuration["DemoData:CompanyCode"]?.Trim();
        if (string.IsNullOrWhiteSpace(companyCode))
            companyCode = DefaultCompanyCode;

        var platformUser = await EnsurePlatformSeedUserAsync(cancellationToken);
        using var _ = Impersonate(platformUser.Id, null);

        var company = await EnsureParentCompanyAsync(companyCode, cancellationToken);
        using var __ = Impersonate(platformUser.Id, company.Id);

        await EnsureRolesAndUsersAsync(company.Id);
        var branches = await EnsureOrganizationAsync(company.Id, cancellationToken);
        await EnsureAccountingAsync(company.Id, branches, cancellationToken);
        var employees = await EnsureEmployeesAsync(company.Id, branches, cancellationToken);
        await EnsureTasksAsync(employees, cancellationToken);
        var skus = await EnsureCatalogAsync(company.Id, cancellationToken);
        var customers = await EnsureCustomersAsync(company.Id, cancellationToken);
        await EnsureSuppliersAsync(company.Id, cancellationToken);
        await EnsureInventoryAsync(company.Id, branches, cancellationToken);
        await EnsureAccountingActivityAsync(company.Id, branches.First().Id, customers.FirstOrDefault()?.Id, skus.FirstOrDefault()?.Id, cancellationToken);

        Console.WriteLine($"Demo data ready for {company.NameEng} ({company.Code}). Admin: demo.admin@alafkar.demo / {DefaultPassword}");
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

    private async Task<Company> EnsureParentCompanyAsync(string companyCode, CancellationToken cancellationToken)
    {
        var existing = await organizationDbContext.Companies
            .FirstOrDefaultAsync(x => x.Code == companyCode && x.ParentCompanyId == null, cancellationToken);
        if (existing is not null)
            return existing;

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
            Name = "شركة العرض الشامل",
            NameEng = "Alafkar Demo Holding",
            Code = companyCode,
            Logo = string.Empty,
            HqLocation = "Riyadh, Saudi Arabia",
            HqLatitude = 24.7136,
            HqLongitude = 46.6753,
            VatNo = "300000000000003",
            Phone = "0110000000",
            Email = "info@alafkar.demo",
            TimeZone = "Asia/Riyadh",
            AdminUserName = "demo.admin",
            AdminEmail = "demo.admin@alafkar.demo",
            AdminPhoneNumber = "0500000002",
            AdminTemporaryPassword = DefaultPassword,
            License = new CompanyLicenseDto
            {
                LicenseCategoryId = licenseCategory.Id,
                Status = CompanyLicenseStatus.Active,
                StartDate = DateTime.UtcNow.Date.AddMonths(-1),
                EndDate = DateTime.UtcNow.Date.AddYears(2),
                Notes = "Demo license with all available business lines.",
                BusinessLines = businessLines
            }
        }), cancellationToken);

        return await organizationDbContext.Companies
            .FirstAsync(x => x.Id == result.CreatedCompany.Id, cancellationToken);
    }

    private async Task EnsureRolesAndUsersAsync(Guid companyId)
    {
        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, companyId);

        var demoUsers = new[]
        {
            ("demo.hr", "demo.hr@alafkar.demo", "0501000001", "hr-manager"),
            ("demo.accountant", "demo.accountant@alafkar.demo", "0501000002", "accounting-manager"),
            ("demo.sales", "demo.sales@alafkar.demo", "0501000003", "sales-manager"),
            ("demo.procurement", "demo.procurement@alafkar.demo", "0501000004", "procurement-manager"),
            ("demo.cashier", "demo.cashier@alafkar.demo", "0501000005", "cashier"),
            ("demo.employee", "demo.employee@alafkar.demo", "0501000006", "employee"),
            ("demo.approver", "demo.approver@alafkar.demo", "0501000007", "approver")
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

    private async Task<List<Employee>> EnsureEmployeesAsync(Guid companyId, List<Branch> branches, CancellationToken cancellationToken)
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
            ("EMP-001", "سارة", "Sara", "عبدالله", "Abdullah", "العتيبي", "Alotaibi", "demo.hr", "POS-HR-MGR", "DEP-HR"),
            ("EMP-002", "خالد", "Khalid", "محمد", "Mohammed", "القحطاني", "Alqahtani", "demo.accountant", "POS-ACC-MGR", "DEP-AR"),
            ("EMP-003", "نورة", "Noura", "سالم", "Salem", "الحربي", "Alharbi", "demo.sales", "POS-SALES-MGR", "DEP-SALES"),
            ("EMP-004", "فيصل", "Faisal", "ناصر", "Nasser", "الدوسري", "Aldosari", "demo.procurement", "POS-PROC-MGR", "DEP-PROC"),
            ("EMP-005", "ريم", "Reem", "علي", "Ali", "الزهراني", "Alzahrani", "demo.cashier", "POS-CASHIER", "DEP-SALES"),
            ("EMP-006", "ماجد", "Majed", "حسن", "Hassan", "الشمري", "Alshammari", "demo.employee", "POS-WH", "DEP-WH"),
            ("EMP-007", "عبدالعزيز", "Abdulaziz", "فهد", "Fahad", "الغامدي", "Alghamdi", "demo.approver", "POS-EMP", "DEP-PAY")
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

    private async Task EnsureTasksAsync(List<Employee> employees, CancellationToken cancellationToken)
    {
        if (employees.Count == 0)
            return;

        var assignedBy = employees[0].LinkedUserId ?? StableGuid("user", "demo.hr");
        var assignedTo = employees.FirstOrDefault(x => x.LinkedUserId.HasValue)?.LinkedUserId?.ToString() ?? assignedBy.ToString();
        var departmentId = employees.FirstOrDefault(x => x.DepartmentId.HasValue)?.DepartmentId;

        var taskSpecs = new[]
        {
            ("DEMO-TASK-001", "Prepare onboarding checklist", "New joiner onboarding tasks for HR demo.", TaskPriority.Normal, TaskWorkflowStatus.Assigned, 0m, DateTime.UtcNow.Date.AddDays(5)),
            ("DEMO-TASK-002", "Review overdue supplier invoice", "Finance task intentionally overdue for dashboard counters.", TaskPriority.High, TaskWorkflowStatus.Overdue, 25m, DateTime.UtcNow.Date.AddDays(-3)),
            ("DEMO-TASK-003", "Update sales price list", "Sales operations task in progress.", TaskPriority.Normal, TaskWorkflowStatus.InProgress, 60m, DateTime.UtcNow.Date.AddDays(2)),
            ("DEMO-TASK-004", "Complete monthly attendance audit", "Completed HR control task.", TaskPriority.Low, TaskWorkflowStatus.Completed, 100m, DateTime.UtcNow.Date.AddDays(-1)),
            ("DEMO-TASK-005", "Cancelled warehouse cycle count", "Cancelled task for status filtering.", TaskPriority.Normal, TaskWorkflowStatus.Cancelled, 0m, DateTime.UtcNow.Date.AddDays(4))
        };

        foreach (var (number, title, description, priority, status, progress, dueDate) in taskSpecs)
        {
            if (await taskDbContext.TaskItems.AnyAsync(x => x.TaskNumber == number, cancellationToken))
                continue;

            var task = TaskItem.Create(
                number,
                title,
                description,
                priority,
                DateTime.UtcNow.Date.AddDays(-2),
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

    private async Task EnsureAccountingActivityAsync(Guid companyId, Guid branchId, Guid? customerId, Guid? skuId, CancellationToken cancellationToken)
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
            SourceDocumentNumber = "DEMO-SINV-001",
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

        await sender.Send(new CreateBankTransactionCommand(new BankTransactionDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            TransactionDate = DateTime.UtcNow.Date.AddDays(-4),
            Description = "Demo customer receipt for SINV-001",
            ReferenceNumber = "DEMO-REC-001",
            Amount = 667m
        }), cancellationToken);
    }

    private IDisposable Impersonate(Guid userId, Guid? companyId)
    {
        var previous = httpContextAccessor.HttpContext;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, DemoActorName)
        };

        if (companyId.HasValue)
            claims.Add(new Claim("company_id", companyId.Value.ToString()));

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
