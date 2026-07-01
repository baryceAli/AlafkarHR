namespace Auth.Users.Roles;

public static class CompanyRoleTemplates
{
    public const string ManagedClaimType = "Permission";
    public static readonly Guid DefaultCompanyId = Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e");
    public const string PlatformSystemUserRoleName = "Platform-system-user";
    public const string PlatformSystemUserTemplateKey = "platform-system-user";

    public static IReadOnlyList<CompanyRoleTemplate> All { get; } =
    [
        new("admin", "Admin", PermissionList.GetTenantPermissions()),
        new("all-roles", "All Roles", PermissionList.GetTenantPermissions()),
        new("manager", "Manager", ManagerPermissions()),
        new("approver", "Approver", ApproverPermissions()),
        new("employee", "Employee", EmployeePermissions()),
        new("cashier", "Cashier", CashierPermissions()),
        new("sales-employee", "Sales Employee", SalesEmployeePermissions()),
        new("sales-manager", "Sales Manager", SalesManagerPermissions()),
        new("procurement-employee", "Procurement Employee", ProcurementEmployeePermissions()),
        new("procurement-manager", "Procurement Manager", ProcurementManagerPermissions()),
        new("hr-manager", "HR Manager", HrManagerPermissions()),
        new("hr-employee", "HR Employee", HrEmployeePermissions()),
        new("attendance-officer", "Attendance Officer", AttendanceOfficerPermissions()),
        new("payroll-employee", "Payroll Employee", PayrollEmployeePermissions()),
        new("accounting-manager", "Accounting Manager", AccountingManagerPermissions()),
        new("senior-accountant", "Senior Accountant", SeniorAccountantPermissions()),
        new("accountant", "Accountant", AccountantPermissions()),
        new("accounts-receivable-clerk", "Accounts Receivable Clerk", AccountingClerkPermissions()),
        new("accounts-payable-clerk", "Accounts Payable Clerk", AccountingClerkPermissions()),
        new("cash-bank-clerk", "Cash and Bank Clerk", CashBankClerkPermissions()),
        new("tax-zatca-officer", "Tax and ZATCA Officer", TaxZatcaOfficerPermissions()),
        new("accounting-auditor", "Accounting Auditor", AccountingAuditorPermissions()),
        new("attendance-and-leave-employee", "AttendanceAndLeaveEmployee", AttendanceAndLeaveEmployeePermissions()),
    ];

    public static string BuildCompanyRoleName(Guid companyId, string key)
        => $"CompanyRole-{companyId:N}-{NormalizeSlug(key)}";

    public static string BuildSystemAdminRoleName(Guid companyId)
        => $"SystemAdmin-{companyId:N}";

    public static string BuildPlatformRoleName(string key)
        => $"Platform-{NormalizeSlug(key)}";

    public static string NormalizeSlug(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();

        var slug = string.Join("-", new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(slug) ? "role" : slug;
    }

    public static async Task SeedDefaultRolesAsync(RoleManager<ApplicationRole> roleManager, Guid companyId)
    {
        foreach (var template in All)
        {
            await EnsureTemplateRoleAsync(roleManager, companyId, template);
        }
    }

    public static async Task<ApplicationRole> EnsurePlatformSystemUserRoleAsync(RoleManager<ApplicationRole> roleManager)
    {
        var role = await roleManager.FindByNameAsync(PlatformSystemUserRoleName)
            ?? await roleManager.FindByNameAsync("SystemUser")
            ?? await roleManager.Roles.FirstOrDefaultAsync(r => r.CompanyId == null && r.TemplateKey == PlatformSystemUserTemplateKey);

        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = PlatformSystemUserRoleName,
                DisplayName = "SystemUser",
                TemplateKey = PlatformSystemUserTemplateKey,
                CompanyId = null
            };

            var createResult = await roleManager.CreateAsync(role);
            if (!createResult.Succeeded)
                throw new BadRequestException(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            role = await roleManager.FindByNameAsync(PlatformSystemUserRoleName)
                ?? throw new Exception($"Couldn't find the role: {PlatformSystemUserRoleName}");
        }
        else
        {
            var changed = false;
            if (!string.Equals(role.Name, PlatformSystemUserRoleName, StringComparison.Ordinal))
            {
                role.Name = PlatformSystemUserRoleName;
                changed = true;
            }

            if (role.CompanyId is not null)
            {
                role.CompanyId = null;
                changed = true;
            }

            if (!string.Equals(role.DisplayName, "SystemUser", StringComparison.Ordinal))
            {
                role.DisplayName = "SystemUser";
                changed = true;
            }

            if (!string.Equals(role.TemplateKey, PlatformSystemUserTemplateKey, StringComparison.Ordinal))
            {
                role.TemplateKey = PlatformSystemUserTemplateKey;
                changed = true;
            }

            if (changed)
            {
                var updateResult = await roleManager.UpdateAsync(role);
                if (!updateResult.Succeeded)
                    throw new BadRequestException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
        }

        await SyncPermissionClaimsAsync(roleManager, role, GetPlatformSystemUserPermissions(), removeObsolete: true);
        return role;
    }

    public static async Task<ApplicationRole> EnsureTemplateRoleAsync(
        RoleManager<ApplicationRole> roleManager,
        Guid companyId,
        CompanyRoleTemplate template)
    {
        var role = await roleManager.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.TemplateKey == template.Key);

        var internalName = BuildCompanyRoleName(companyId, template.Key);
        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = internalName,
                DisplayName = template.DisplayName,
                TemplateKey = template.Key,
                CompanyId = companyId
            };

            var createResult = await roleManager.CreateAsync(role);
            if (!createResult.Succeeded)
                throw new BadRequestException(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            role = await roleManager.FindByNameAsync(internalName)
                ?? throw new Exception($"Couldn't find the role: {internalName}");
        }
        else
        {
            var changed = false;
            if (!string.Equals(role.Name, internalName, StringComparison.Ordinal))
            {
                role.Name = internalName;
                changed = true;
            }

            if (!string.Equals(role.DisplayName, template.DisplayName, StringComparison.Ordinal))
            {
                role.DisplayName = template.DisplayName;
                changed = true;
            }

            if (changed)
            {
                var updateResult = await roleManager.UpdateAsync(role);
                if (!updateResult.Succeeded)
                    throw new BadRequestException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
        }

        await SyncPermissionClaimsAsync(roleManager, role, template.Permissions, removeObsolete: true);
        return role;
    }

    public static async Task SyncPermissionClaimsAsync(
        RoleManager<ApplicationRole> roleManager,
        ApplicationRole role,
        IEnumerable<string> permissions,
        bool removeObsolete)
    {
        var requestedPermissions = permissions
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);

        var roleClaims = await roleManager.GetClaimsAsync(role);
        var permissionClaims = roleClaims
            .Where(c => c.Type == ManagedClaimType)
            .ToList();

        if (removeObsolete)
        {
            foreach (var claim in permissionClaims.Where(c => !requestedPermissions.Contains(c.Value)))
            {
                await roleManager.RemoveClaimAsync(role, claim);
            }
        }

        foreach (var permission in requestedPermissions)
        {
            if (!permissionClaims.Any(c => c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim(ManagedClaimType, permission));
            }
        }
    }

    private static List<string> SelectView(params IEnumerable<string>[] permissionSets)
        => permissionSets
            .SelectMany(p => p)
            .Where(p => p.EndsWith(".Select", StringComparison.Ordinal) || p.EndsWith(".View", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

    private static List<string> ManagerPermissions()
    {
        List<string> permissions =
        [
            ..SelectView(PermissionList.GetTenantPermissions()),
            PermissionList.CompanyPermissions.ViewLicense,
            PermissionList.SalesOrderPermissions.ViewReports,
            PermissionList.AttendancePermissions.ViewReports,
            PermissionList.LeavePermissions.ViewLeaveReports,
            PermissionList.AttendancePermissions.ViewAllReports,
            PermissionList.AttendancePermissions.ViewScopedReports,
            PermissionList.TaskManagementPermissions.Assign,
            PermissionList.TaskManagementPermissions.Reassign,
            PermissionList.TaskManagementPermissions.ViewReports,
            PermissionList.TaskManagementPermissions.ManageAllTasks,
        ];

        return permissions
            .Where(p => !p.StartsWith("Authentication.", StringComparison.Ordinal)
                && !p.StartsWith("GeneralSettings.", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static List<string> ApproverPermissions() =>
    [
        ..SelectView(PermissionList.GetTenantPermissions()),
        PermissionList.AttendancePermissions.ReviewRequests,
        PermissionList.AttendancePermissions.ApproveMidDayPermission,
        PermissionList.LeavePermissions.ApproveEmergencyLeave,
        PermissionList.LeaveApplicationPermissions.Approve,
        PermissionList.PayrollLoanPermissions.Approve,
        PermissionList.PayrollLoanPermissions.Cancel,
        PermissionList.SalaryRunPermissions.Approve,
        PermissionList.PurchaseRequestPermissions.Approve,
        PermissionList.PurchaseRequestPermissions.Reject,
        PermissionList.PurchaseRequestPermissions.Cancel,
        PermissionList.PurchaseRequestPermissions.Close,
        PermissionList.RequestForQuotationPermissions.Cancel,
        PermissionList.RequestForQuotationPermissions.Close,
        PermissionList.SupplierQuotationPermissions.Approve,
        PermissionList.SupplierQuotationPermissions.Reject,
        PermissionList.PurchaseOrderPermissions.Approve,
        PermissionList.PurchaseOrderPermissions.Cancel,
        PermissionList.PurchaseOrderPermissions.Close,
        PermissionList.PurchaseOrderPermissions.Receive,
        PermissionList.GoodsReceiptPermissions.Receive,
        PermissionList.GoodsReceiptPermissions.Cancel,
        PermissionList.PurchaseReturnPermissions.Receive,
        PermissionList.PurchaseReturnPermissions.Cancel,
        PermissionList.SupplierInvoicePermissions.Approve,
        PermissionList.SupplierInvoicePermissions.Cancel,
        PermissionList.SupplierInvoicePermissions.Close,
        PermissionList.SalesOrderPermissions.Confirm,
        PermissionList.SalesOrderPermissions.Deliver,
        PermissionList.SalesOrderPermissions.Invoice,
        PermissionList.SalesOrderPermissions.Complete,
        PermissionList.SalesOrderPermissions.Cancel,
        PermissionList.SalesOrderPermissions.Return,
        PermissionList.OrderIntakePermissions.Accept,
        PermissionList.OrderIntakePermissions.Reject,
        PermissionList.PaymentPermissions.Approve,
        PermissionList.PaymentPermissions.Reject,
        PermissionList.PaymentPermissions.Refund,
        PermissionList.TaskManagementPermissions.Close,
        PermissionList.TaskManagementPermissions.Assign,
        PermissionList.TaskManagementPermissions.Reassign,
    ];

    private static List<string> EmployeePermissions() =>
    [
        PermissionList.AttendancePermissions.Select,
        PermissionList.AttendancePermissions.View,
        PermissionList.AttendancePermissions.Create,
        PermissionList.AttendancePermissions.RequestMidDayPermission,
        PermissionList.AttendancePermissions.ViewScopedReports,
        PermissionList.LeavePermissions.RequestEmergencyLeave,
        PermissionList.LeavePermissions.ViewLeaveBalances,
        PermissionList.LeaveApplicationPermissions.Request,
        PermissionList.TaskManagementPermissions.Select,
        PermissionList.TaskManagementPermissions.View,
        PermissionList.TaskManagementPermissions.Comment,
    ];

    private static List<string> CashierPermissions() =>
    [
        PermissionList.SalesOrderPermissions.Select,
        PermissionList.SalesOrderPermissions.View,
        PermissionList.SalesOrderPermissions.Create,
        PermissionList.CartPermissions.Select,
        PermissionList.CartPermissions.View,
        PermissionList.CartPermissions.Create,
        PermissionList.CartPermissions.Edit,
        PermissionList.CartPermissions.Checkout,
        PermissionList.ProductPermissions.Select,
        PermissionList.ProductPermissions.View,
        PermissionList.PricingPermissions.Select,
        PermissionList.PricingPermissions.View,
        PermissionList.CustomerPermissions.Select,
        PermissionList.CustomerPermissions.View,
        PermissionList.StoreFrontStorePermissions.View,
        PermissionList.StoreFrontPosPermissions.View,
        PermissionList.StoreFrontPosPermissions.Checkout,
    ];

    private static List<string> SalesEmployeePermissions() =>
    [
        ..CashierPermissions(),
        PermissionList.CustomerPermissions.Create,
        PermissionList.CustomerPermissions.Edit,
        PermissionList.CustomerGroupPermissions.Select,
        PermissionList.CustomerGroupPermissions.View,
        PermissionList.CustomerPricingProfilePermissions.Select,
        PermissionList.CustomerPricingProfilePermissions.View,
        PermissionList.OrderIntakePermissions.Select,
        PermissionList.OrderIntakePermissions.View,
        PermissionList.OrderIntakePermissions.Create,
        PermissionList.OrderIntakePermissions.Edit,
        PermissionList.SalesOrderPermissions.Edit,
        PermissionList.StoreFrontItemPermissions.View,
    ];

    private static List<string> SalesManagerPermissions() =>
    [
        ..SalesEmployeePermissions(),
        PermissionList.CustomerPermissions.Delete,
        PermissionList.CustomerGroupPermissions.Create,
        PermissionList.CustomerGroupPermissions.Edit,
        PermissionList.CustomerGroupPermissions.Delete,
        PermissionList.CustomerPricingProfilePermissions.Create,
        PermissionList.CustomerPricingProfilePermissions.Edit,
        PermissionList.CustomerPricingProfilePermissions.Delete,
        PermissionList.OrderIntakePermissions.Accept,
        PermissionList.OrderIntakePermissions.Reject,
        PermissionList.SalesOrderPermissions.Confirm,
        PermissionList.SalesOrderPermissions.Deliver,
        PermissionList.SalesOrderPermissions.Invoice,
        PermissionList.SalesOrderPermissions.Complete,
        PermissionList.SalesOrderPermissions.Cancel,
        PermissionList.SalesOrderPermissions.Return,
        PermissionList.SalesOrderPermissions.ViewReports,
        PermissionList.PaymentPermissions.Select,
        PermissionList.PaymentPermissions.View,
        PermissionList.PaymentPermissions.Create,
        PermissionList.PaymentPermissions.Approve,
        PermissionList.PaymentPermissions.Reject,
        PermissionList.PaymentPermissions.Refund,
    ];

    private static List<string> ProcurementEmployeePermissions() =>
    [
        PermissionList.PurchaseRequestPermissions.Select,
        PermissionList.PurchaseRequestPermissions.View,
        PermissionList.PurchaseRequestPermissions.Create,
        PermissionList.PurchaseRequestPermissions.Edit,
        PermissionList.PurchaseRequestPermissions.Submit,
        PermissionList.RequestForQuotationPermissions.Select,
        PermissionList.RequestForQuotationPermissions.View,
        PermissionList.RequestForQuotationPermissions.Create,
        PermissionList.RequestForQuotationPermissions.Edit,
        PermissionList.RequestForQuotationPermissions.Submit,
        PermissionList.SupplierQuotationPermissions.Select,
        PermissionList.SupplierQuotationPermissions.View,
        PermissionList.SupplierQuotationPermissions.Create,
        PermissionList.SupplierQuotationPermissions.Edit,
        PermissionList.PurchaseOrderPermissions.Select,
        PermissionList.PurchaseOrderPermissions.View,
        PermissionList.PurchaseOrderPermissions.Create,
        PermissionList.PurchaseOrderPermissions.Edit,
        PermissionList.PurchaseOrderPermissions.Submit,
        PermissionList.GoodsReceiptPermissions.Select,
        PermissionList.GoodsReceiptPermissions.View,
        PermissionList.GoodsReceiptPermissions.Create,
        PermissionList.GoodsReceiptPermissions.Edit,
        PermissionList.PurchaseReturnPermissions.Select,
        PermissionList.PurchaseReturnPermissions.View,
        PermissionList.PurchaseReturnPermissions.Create,
        PermissionList.PurchaseReturnPermissions.Edit,
        PermissionList.SupplierInvoicePermissions.Select,
        PermissionList.SupplierInvoicePermissions.View,
        PermissionList.SupplierInvoicePermissions.Create,
        PermissionList.SupplierInvoicePermissions.Edit,
        PermissionList.SupplierPermissions.Select,
        PermissionList.SupplierPermissions.View,
        PermissionList.SupplierGroupPermissions.Select,
        PermissionList.SupplierGroupPermissions.View,
        PermissionList.ProductPermissions.Select,
        PermissionList.ProductPermissions.View,
        PermissionList.InventoryPermissions.Select,
        PermissionList.InventoryPermissions.View,
        PermissionList.WarehousePermissions.Select,
        PermissionList.WarehousePermissions.View,
    ];

    private static List<string> ProcurementManagerPermissions() =>
    [
        ..ProcurementEmployeePermissions(),
        PermissionList.SupplierPermissions.Create,
        PermissionList.SupplierPermissions.Edit,
        PermissionList.SupplierPermissions.Delete,
        PermissionList.SupplierGroupPermissions.Create,
        PermissionList.SupplierGroupPermissions.Edit,
        PermissionList.SupplierGroupPermissions.Delete,
        PermissionList.PurchaseRequestPermissions.Approve,
        PermissionList.PurchaseRequestPermissions.Reject,
        PermissionList.PurchaseRequestPermissions.Cancel,
        PermissionList.PurchaseRequestPermissions.Close,
        PermissionList.RequestForQuotationPermissions.Cancel,
        PermissionList.RequestForQuotationPermissions.Close,
        PermissionList.SupplierQuotationPermissions.Approve,
        PermissionList.SupplierQuotationPermissions.Reject,
        PermissionList.PurchaseOrderPermissions.Approve,
        PermissionList.PurchaseOrderPermissions.Cancel,
        PermissionList.PurchaseOrderPermissions.Receive,
        PermissionList.PurchaseOrderPermissions.Close,
        PermissionList.GoodsReceiptPermissions.Receive,
        PermissionList.GoodsReceiptPermissions.Cancel,
        PermissionList.PurchaseReturnPermissions.Receive,
        PermissionList.PurchaseReturnPermissions.Cancel,
        PermissionList.SupplierInvoicePermissions.Approve,
        PermissionList.SupplierInvoicePermissions.Cancel,
        PermissionList.SupplierInvoicePermissions.Close,
    ];

    private static List<string> HrManagerPermissions() =>
    [
        ..PermissionList.EmployeePermissions.Permissions,
        ..PermissionList.PositionPermissions.Permissions,
        ..PermissionList.AcademicInistitutionPermissions.Permissions,
        ..PermissionList.SpecializationPermissions.Permissions,
        ..SelectView(
            PermissionList.CompanyPermissions.Permissions,
            PermissionList.BranchPermissions.Permissions,
            PermissionList.AdministrationPermissions.Permissions,
            PermissionList.DepartmentPermissions.Permissions),
        PermissionList.AttendancePermissions.Select,
        PermissionList.AttendancePermissions.View,
        PermissionList.AttendancePermissions.Create,
        PermissionList.AttendancePermissions.Edit,
        PermissionList.AttendancePermissions.ReviewRequests,
        PermissionList.AttendancePermissions.ViewReports,
        PermissionList.AttendancePermissions.ViewAllReports,
        PermissionList.AttendancePermissions.ViewScopedReports,
        PermissionList.AttendancePermissions.ApproveMidDayPermission,
        ..PermissionList.AttendanceWorkEntryPermissions.Permissions,
        PermissionList.LeavePermissions.ViewLeaveReports,
        PermissionList.LeavePermissions.ViewLeaveBalances,
        PermissionList.LeavePermissions.ManageLeaveBalances,
        PermissionList.LeavePermissions.ApproveEmergencyLeave,
        ..PermissionList.LeavePolicyPermissions.Permissions,
        PermissionList.LeaveApplicationPermissions.Select,
        PermissionList.LeaveApplicationPermissions.View,
        PermissionList.LeaveApplicationPermissions.Create,
        PermissionList.LeaveApplicationPermissions.Edit,
        PermissionList.LeaveApplicationPermissions.Approve,
        PermissionList.LeaveApplicationPermissions.Cancel,
        PermissionList.LeaveLedgerPermissions.View,
        PermissionList.LeaveLedgerPermissions.Adjust,
        PermissionList.LeaveLedgerPermissions.Encash,
        PermissionList.TaskManagementPermissions.Select,
        PermissionList.TaskManagementPermissions.View,
        PermissionList.TaskManagementPermissions.Create,
        PermissionList.TaskManagementPermissions.Edit,
        PermissionList.TaskManagementPermissions.Assign,
        PermissionList.TaskManagementPermissions.Reassign,
        PermissionList.TaskManagementPermissions.Comment,
        PermissionList.TaskManagementPermissions.Close,
        PermissionList.TaskManagementPermissions.ViewReports,
    ];

    private static List<string> HrEmployeePermissions() =>
    [
        PermissionList.EmployeePermissions.Select,
        PermissionList.EmployeePermissions.View,
        PermissionList.EmployeePermissions.Create,
        PermissionList.EmployeePermissions.Edit,
        PermissionList.PositionPermissions.Select,
        PermissionList.PositionPermissions.View,
        PermissionList.PositionPermissions.Create,
        PermissionList.PositionPermissions.Edit,
        PermissionList.AcademicInistitutionPermissions.Select,
        PermissionList.AcademicInistitutionPermissions.View,
        PermissionList.AcademicInistitutionPermissions.Create,
        PermissionList.AcademicInistitutionPermissions.Edit,
        PermissionList.SpecializationPermissions.Select,
        PermissionList.SpecializationPermissions.View,
        PermissionList.SpecializationPermissions.Create,
        PermissionList.SpecializationPermissions.Edit,
        ..SelectView(
            PermissionList.CompanyPermissions.Permissions,
            PermissionList.BranchPermissions.Permissions,
            PermissionList.AdministrationPermissions.Permissions,
            PermissionList.DepartmentPermissions.Permissions),
        PermissionList.AttendancePermissions.Select,
        PermissionList.AttendancePermissions.View,
        PermissionList.AttendancePermissions.ViewReports,
        PermissionList.AttendancePermissions.ViewScopedReports,
        PermissionList.LeavePermissions.ViewLeaveBalances,
        PermissionList.LeavePermissions.ViewLeaveReports,
        PermissionList.LeavePolicyPermissions.Select,
        PermissionList.LeavePolicyPermissions.View,
        PermissionList.LeaveApplicationPermissions.Select,
        PermissionList.LeaveApplicationPermissions.View,
        PermissionList.LeaveApplicationPermissions.Create,
        PermissionList.LeaveApplicationPermissions.Edit,
        PermissionList.LeaveLedgerPermissions.View,
    ];

    private static List<string> AttendanceOfficerPermissions() =>
    [
        PermissionList.AttendancePermissions.Select,
        PermissionList.AttendancePermissions.View,
        PermissionList.AttendancePermissions.ViewReports,
        PermissionList.AttendancePermissions.ViewScopedReports,
        PermissionList.AttendancePermissions.ReviewRequests,
        PermissionList.AttendancePermissions.ApproveMidDayPermission,
        PermissionList.AttendanceRosterPermissions.View,
        ..PermissionList.AttendanceWorkEntryPermissions.Permissions,
    ];

    private static List<string> PayrollEmployeePermissions() =>
    [
        PermissionList.PayrollContractPermissions.Select,
        PermissionList.PayrollContractPermissions.View,
        PermissionList.PayrollContractPermissions.Create,
        PermissionList.PayrollContractPermissions.Edit,
        PermissionList.PayrollLoanPermissions.Select,
        PermissionList.PayrollLoanPermissions.View,
        PermissionList.PayrollLoanPermissions.Create,
        PermissionList.PayrollLoanPermissions.Edit,
        PermissionList.SalaryRunPermissions.Select,
        PermissionList.SalaryRunPermissions.View,
        PermissionList.SalaryRunPermissions.Create,
        PermissionList.SalaryRunPermissions.Edit,
        ..SelectView(
            PermissionList.EmployeePermissions.Permissions,
            PermissionList.CompanyPermissions.Permissions,
            PermissionList.BranchPermissions.Permissions,
            PermissionList.AdministrationPermissions.Permissions,
            PermissionList.DepartmentPermissions.Permissions),
    ];

    private static List<string> AccountingManagerPermissions() =>
    [
        ..AllAccountingPermissions(),
        ..OrganizationReadPermissions(),
    ];

    private static List<string> SeniorAccountantPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        PermissionList.AccountingTemplatePermissions.View,
        PermissionList.AccountingTemplatePermissions.Apply,
        ..PermissionList.AccountPermissions.Permissions,
        ..PermissionList.FiscalPeriodPermissions.Permissions,
        ..PermissionList.TaxCodePermissions.Permissions,
        ..PermissionList.PostingProfilePermissions.Permissions,
        ..PermissionList.BankAccountPermissions.Permissions,
        ..PermissionList.CashAccountPermissions.Permissions,
        PermissionList.AccountingSettingsPermissions.View,
        PermissionList.AccountingSettingsPermissions.Edit,
        ..PermissionList.JournalEntryPermissions.Permissions,
        ..PermissionList.AccountingDocumentPermissions.Permissions,
        PermissionList.ZatcaSettingsPermissions.View,
        PermissionList.ZatcaEInvoicePermissions.View,
    ];

    private static List<string> AccountantPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        PermissionList.AccountingTemplatePermissions.View,
        PermissionList.AccountPermissions.Select,
        PermissionList.AccountPermissions.View,
        PermissionList.FiscalPeriodPermissions.View,
        PermissionList.TaxCodePermissions.View,
        PermissionList.PostingProfilePermissions.View,
        PermissionList.BankAccountPermissions.View,
        PermissionList.CashAccountPermissions.View,
        PermissionList.AccountingSettingsPermissions.View,
        PermissionList.JournalEntryPermissions.View,
        PermissionList.JournalEntryPermissions.Create,
        PermissionList.JournalEntryPermissions.Post,
        PermissionList.AccountingDocumentPermissions.View,
        PermissionList.AccountingDocumentPermissions.Create,
        PermissionList.AccountingDocumentPermissions.Post,
    ];

    private static List<string> AccountingClerkPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        PermissionList.AccountPermissions.Select,
        PermissionList.AccountPermissions.View,
        PermissionList.JournalEntryPermissions.View,
        PermissionList.JournalEntryPermissions.Create,
        PermissionList.AccountingDocumentPermissions.View,
        PermissionList.AccountingDocumentPermissions.Create,
        PermissionList.AccountingDocumentPermissions.Post,
    ];

    private static List<string> CashBankClerkPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        PermissionList.AccountPermissions.Select,
        PermissionList.AccountPermissions.View,
        PermissionList.BankAccountPermissions.View,
        PermissionList.BankAccountPermissions.Create,
        PermissionList.BankAccountPermissions.Edit,
        PermissionList.CashAccountPermissions.View,
        PermissionList.CashAccountPermissions.Create,
        PermissionList.CashAccountPermissions.Edit,
        PermissionList.JournalEntryPermissions.View,
        PermissionList.JournalEntryPermissions.Create,
        PermissionList.JournalEntryPermissions.Post,
        PermissionList.AccountingDocumentPermissions.View,
        PermissionList.AccountingDocumentPermissions.Create,
        PermissionList.AccountingDocumentPermissions.Post,
    ];

    private static List<string> TaxZatcaOfficerPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        PermissionList.TaxCodePermissions.View,
        PermissionList.TaxCodePermissions.Create,
        PermissionList.TaxCodePermissions.Edit,
        PermissionList.ZatcaSettingsPermissions.View,
        PermissionList.ZatcaSettingsPermissions.Edit,
        PermissionList.ZatcaEInvoicePermissions.View,
        PermissionList.ZatcaEInvoicePermissions.Generate,
        PermissionList.ZatcaEInvoicePermissions.Submit,
        PermissionList.AccountingDocumentPermissions.View,
    ];

    private static List<string> AccountingAuditorPermissions() =>
    [
        PermissionList.AccountingDashboardPermissions.View,
        ..SelectView(
            PermissionList.AccountingTemplatePermissions.Permissions,
            PermissionList.AccountPermissions.Permissions,
            PermissionList.FiscalPeriodPermissions.Permissions,
            PermissionList.TaxCodePermissions.Permissions,
            PermissionList.PostingProfilePermissions.Permissions,
            PermissionList.BankAccountPermissions.Permissions,
            PermissionList.CashAccountPermissions.Permissions,
            PermissionList.AccountingSettingsPermissions.Permissions,
            PermissionList.JournalEntryPermissions.Permissions,
            PermissionList.AccountingDocumentPermissions.Permissions,
            PermissionList.ZatcaSettingsPermissions.Permissions,
            PermissionList.ZatcaEInvoicePermissions.Permissions),
    ];

    private static List<string> AllAccountingPermissions() =>
    [
        ..PermissionList.AccountingDashboardPermissions.Permissions,
        ..PermissionList.AccountingTemplatePermissions.Permissions,
        ..PermissionList.AccountPermissions.Permissions,
        ..PermissionList.FiscalPeriodPermissions.Permissions,
        ..PermissionList.TaxCodePermissions.Permissions,
        ..PermissionList.PostingProfilePermissions.Permissions,
        ..PermissionList.BankAccountPermissions.Permissions,
        ..PermissionList.CashAccountPermissions.Permissions,
        ..PermissionList.AccountingSettingsPermissions.Permissions,
        ..PermissionList.JournalEntryPermissions.Permissions,
        ..PermissionList.AccountingDocumentPermissions.Permissions,
        ..PermissionList.ZatcaSettingsPermissions.Permissions,
        ..PermissionList.ZatcaEInvoicePermissions.Permissions,
    ];

    private static List<string> OrganizationReadPermissions() =>
    [
        ..SelectView(
            PermissionList.CompanyPermissions.Permissions,
            PermissionList.BranchPermissions.Permissions,
            PermissionList.AdministrationPermissions.Permissions,
            PermissionList.DepartmentPermissions.Permissions),
        PermissionList.CompanyPermissions.ViewLicense,
    ];

    private static List<string> AttendanceAndLeaveEmployeePermissions() =>
    [
        PermissionList.AttendancePermissions.Select,
        PermissionList.AttendancePermissions.View,
        PermissionList.AttendancePermissions.Create,
        PermissionList.AttendancePermissions.RequestMidDayPermission,
        PermissionList.AttendancePermissions.ViewScopedReports,
        PermissionList.LeavePermissions.RequestEmergencyLeave,
        PermissionList.LeavePermissions.ViewLeaveBalances,
        PermissionList.LeaveApplicationPermissions.Request,
        PermissionList.TaskManagementPermissions.Select,
        PermissionList.TaskManagementPermissions.View,
        PermissionList.TaskManagementPermissions.Comment,
    ];

    public static List<string> GetPlatformSystemUserPermissions()
        => PermissionList.GetPlatformPermissions()
            .Distinct(StringComparer.Ordinal)
            .ToList();
}

public sealed record CompanyRoleTemplate(string Key, string DisplayName, IReadOnlyCollection<string> Permissions);
