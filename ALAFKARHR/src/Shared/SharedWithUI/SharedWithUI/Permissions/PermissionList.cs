using SharedWithUI.Auth.Dtos;

namespace SharedWithUI.Permissions;

public static class PermissionList
{
    public static List<string> GetAll()
        => GetTenantPermissions();

    public static List<string> GetTenantPermissions()
    {
        List<string> list =
        [
            ..UsersPermissions.Permissions,
            ..RolesPermissions.Permissions,
            ..AcademicInistitutionPermissions.Permissions,
            ..SpecializationPermissions.Permissions,
            ..EmployeePermissions.Permissions,
            ..EmployeeLifecyclePermissions.Permissions,
            ..EmployeeDocumentPermissions.Permissions,
            ..EmployeeSkillPermissions.Permissions,
            ..TeamPermissions.Permissions,
            ..PositionPermissions.Permissions,
            ..CompanyPermissions.Permissions,
            ..BranchPermissions.Permissions,
            ..OrganizationBranchAccessPermissions.Permissions,
            ..AdministrationPermissions.Permissions,
            ..DepartmentPermissions.Permissions,
            .. CategoryPermissions.Permissions,
            .. BrandPermissions.Permissions,
            .. UnitPermissions.Permissions,
            .. VariantPermissions.Permissions,
            .. ProductPermissions.Permissions,
            .. ProductPackagePermissions.Permissions,
            .. WarehousePermissions.Permissions,
            .. InventoryItemPermissions.Permissions,
            .. StockTransactionPermissions.Permissions,
            .. InventoryPermissions.Permissions,
            .. WarehouseTransferPermissions.Permissions,
            .. BatchPermissions.Permissions,
            .. CustomerGroupPermissions.Permissions,
            .. CustomerPricingProfilePermissions.Permissions,
            .. CustomerPermissions.Permissions,
            .. SupplierGroupPermissions.Permissions,
            .. SupplierPermissions.Permissions,
            .. CartPermissions.Permissions,
            .. CateringContractPermissions.Permissions,
            .. CateringMealPermissions.Permissions,
            .. CateringLocationPermissions.Permissions,
            .. CateringSchedulePermissions.Permissions,
            .. CateringDeliveryPermissions.Permissions,
            .. CateringDistributionPermissions.Permissions,
            .. CateringAssignmentPermissions.Permissions,
            .. CateringReportsPermissions.Permissions,
            .. OrderIntakePermissions.Permissions,
            .. PaymentPermissions.Permissions,
            .. ContractPermissions.Permissions,
            .. ContractTemplatePermissions.Permissions,
            .. ContractRenewalPermissions.Permissions,
            .. DocumentManagementPermissions.Permissions,
            .. MediaCenterPermissions.Permissions,
            .. SalesOrderPermissions.Permissions,
            .. SalesQuotationPermissions.Permissions,
            .. SalesDeliveryNotePermissions.Permissions,
            .. SalesReturnPermissions.Permissions,
            .. SalesReportPermissions.Permissions,
            .. SalesPriceOverridePermissions.Permissions,
            .. StoreFrontStorePermissions.Permissions,
            .. StoreFrontDepartmentPermissions.Permissions,
            .. StoreFrontItemPermissions.Permissions,
            .. StoreFrontPosPermissions.Permissions,
            .. PurchaseRequestPermissions.Permissions,
            .. RequestForQuotationPermissions.Permissions,
            .. SupplierQuotationPermissions.Permissions,
            .. PurchaseOrderPermissions.Permissions,
            .. GoodsReceiptPermissions.Permissions,
            .. PurchaseReturnPermissions.Permissions,
            .. SupplierInvoicePermissions.Permissions,
            .. PricingPermissions.Permissions,
            .. PayrollContractPermissions.Permissions,
            .. PayrollLoanPermissions.Permissions,
            .. SalaryRunPermissions.Permissions,
            .. PayrollStructurePermissions.Permissions,
            .. PayrollPayslipPermissions.Permissions,
            .. PayrollWorkEntryPermissions.Permissions,
            .. AttendancePermissions.Permissions,
            .. AttendanceRosterPermissions.Permissions,
            .. AttendanceWorkEntryPermissions.Permissions,
            .. LeavePermissions.Permissions,
            .. LeavePolicyPermissions.Permissions,
            .. LeaveApplicationPermissions.Permissions,
            .. LeaveLedgerPermissions.Permissions,
            .. RecruitmentPermissions.Permissions,
            .. PerformancePermissions.Permissions,
            .. TrainingPermissions.Permissions,
            .. TaskManagementPermissions.Permissions,
            .. ProjectManagementPermissions.Permissions,
            .. MaintenanceAssetPermissions.Permissions,
            .. MaintenanceWorkOrderPermissions.Permissions,
            .. RealEstatePropertyPermissions.Permissions,
            .. RealEstateUnitPermissions.Permissions,
            .. RealEstateLeasePermissions.Permissions,
            .. RealEstateInstallmentPermissions.Permissions,
            .. RealEstateUtilityPermissions.Permissions,
            .. RealEstateExpensePermissions.Permissions,
            .. RealEstateReportsPermissions.Permissions,
            .. FleetVehiclePermissions.Permissions,
            .. FleetVehicleAssignmentPermissions.Permissions,
            .. FleetVehicleExpensePermissions.Permissions,
            .. FleetVehicleDocumentPermissions.Permissions,
            .. FleetReportsPermissions.Permissions,
            .. AccountingDashboardPermissions.Permissions,
            .. AccountingTemplatePermissions.Permissions,
            .. AccountPermissions.Permissions,
            .. FiscalPeriodPermissions.Permissions,
            .. TaxCodePermissions.Permissions,
            .. PostingProfilePermissions.Permissions,
            .. BankAccountPermissions.Permissions,
            .. CashAccountPermissions.Permissions,
            .. AccountingSettingsPermissions.Permissions,
            .. JournalEntryPermissions.Permissions,
            .. AccountingDocumentPermissions.Permissions,
            .. BankReconciliationPermissions.Permissions,
            .. AccountingReportPermissions.Permissions,
            .. AccountingBranchAccessPermissions.Permissions,
            .. ZatcaSettingsPermissions.Permissions,
            .. ZatcaEInvoicePermissions.Permissions,
            .. SystemSettingsPermissions.Permissions,
        ];



        return list;

    }

    public static List<string> GetPlatformPermissions()
        => ParentCompanyPermissions.Permissions
            .Concat(DemoDataPermissions.Permissions)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

    public static List<string> GetParentCompanyAdminPermissions()
    {
        List<string> list =
        [
            CompanyPermissions.Select,
            CompanyPermissions.View,
            CompanyPermissions.Edit,
            CompanyPermissions.ViewLicense,
            CompanyPermissions.ViewChild,
            CompanyPermissions.CreateChild,
            CompanyPermissions.EditChild,
            CompanyPermissions.DisableChild,
            CompanyPermissions.ResetChildAdminPassword,
            ..BranchPermissions.Permissions,
            ..UsersPermissions.Permissions,
            ..RolesPermissions.Permissions,
            SystemSettingsPermissions.Select,
            SystemSettingsPermissions.View,
            SystemSettingsPermissions.Edit,
        ];

        return list
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    public static List<string> GetAuthorizationPolicyPermissions()
        => GetTenantPermissions()
            .Concat(GetPlatformPermissions())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    

    public static List<PermissionGroupDto> GetGroupedPermissions(List<string> permissions)
    {
        //var permissions = GetAll();

        var result = permissions
            .Select(p =>
            {
                var parts = p.Split('.');
                return new
                {
                    Group = parts[0],
                    Entity = parts[1],
                    Action = parts[2]
                };
            })
            .GroupBy(x => x.Group)
            .Select(g => new PermissionGroupDto
            {
                Group = g.Key,
                Entities = g
                    .GroupBy(e => e.Entity)
                    .Select(e => new PermissionEntityDto
                    {
                        Entity = e.Key,
                        Actions = e.Select(a => a.Action).Distinct().ToList()
                    }).ToList()
            })
            .ToList();

        return result;
    }
    public static class UsersPermissions
    {

        public static string GroupName { get; set; } = "Authentication.Users";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string AssignUsers { get; set; } = $"{GroupName}.AssignUsers";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{AssignUsers}",
            };

    }
    public static class RolesPermissions
    {

        public static string GroupName { get; set; } = "Authentication.Roles";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class EmployeePermissions
    {

        public static string GroupName { get; set; } = "Employees.Employee";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }

    public static class EmployeeLifecyclePermissions
    {
        public static string GroupName { get; set; } = "Employees.Lifecycle";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Complete { get; set; } = $"{GroupName}.Complete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Approve, Complete];
    }

    public static class EmployeeDocumentPermissions
    {
        public static string GroupName { get; set; } = "Employees.Document";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Renew { get; set; } = $"{GroupName}.Renew";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Renew];
    }

    public static class EmployeeSkillPermissions
    {
        public static string GroupName { get; set; } = "Employees.Skill";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Verify { get; set; } = $"{GroupName}.Verify";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Verify];
    }

    public static class TeamPermissions
    {

        public static string GroupName { get; set; } = "Employees.Team";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class AcademicInistitutionPermissions
    {

        public static string GroupName { get; set; } = "Employees.AcademicInistitution";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }

    public static class SpecializationPermissions
    {

        public static string GroupName { get; set; } = "Employees.Specialization";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    
    public static class PositionPermissions
    {

        public static string GroupName { get; set; } = "Employees.Position";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class CompanyPermissions
    {

        public static string GroupName { get; set; } = "Organization.Company";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string ViewLicense { get; set; } = $"{GroupName}.ViewLicense";
        public static string ViewChild { get; set; } = $"{GroupName}.ViewChild";
        public static string CreateChild { get; set; } = $"{GroupName}.CreateChild";
        public static string EditChild { get; set; } = $"{GroupName}.EditChild";
        public static string DisableChild { get; set; } = $"{GroupName}.DisableChild";
        public static string ResetChildAdminPassword { get; set; } = $"{GroupName}.ResetChildAdminPassword";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{ViewLicense}",
                $"{ViewChild}",
                $"{CreateChild}",
                $"{EditChild}",
                $"{DisableChild}",
                $"{ResetChildAdminPassword}",
            };

    }
    public static class ParentCompanyPermissions
    {
        public static string GroupName { get; set; } = "Organization.ParentCompany";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string ManageLicense { get; set; } = $"{GroupName}.ManageLicense";
        public static string Suspend { get; set; } = $"{GroupName}.Suspend";
        public static string ResetAdminPassword { get; set; } = $"{GroupName}.ResetAdminPassword";

        public static List<string> Permissions =>
            new()
            {
                Select,
                View,
                Create,
                Edit,
                Delete,
                ManageLicense,
                Suspend,
                ResetAdminPassword,
            };
    }
    public static class BranchPermissions
    {

        public static string GroupName { get; set; } = "Organization.Branch";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string AssignUsers { get; set; } = $"{GroupName}.AssignUsers";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{AssignUsers}",
            };

    }

    public static class OrganizationBranchAccessPermissions
    {
        public static string GroupName { get; set; } = "Organization.BranchAccess";
        public static string ViewAll { get; set; } = $"{GroupName}.ViewAll";
        public static List<string> Permissions => [ViewAll];
    }

    public static class AdministrationPermissions
    {

        public static string GroupName { get; set; } = "Organization.Administration";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class DepartmentPermissions
    {

        public static string GroupName { get; set; } = "Organization.Department";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class CategoryPermissions
    {
        
        public static string GroupName { get; set; } = "Catalog.Category";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class BrandPermissions
    {

        public static string GroupName { get; set; } = "Catalog.Brand";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }
    public static class UnitPermissions
    {

        public static string GroupName { get; set; } = "Catalog.Unit";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class VariantPermissions
    {

        public static string GroupName { get; set; } = "Catalog.Variant";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class ProductPermissions
    {

        public static string GroupName { get; set; } = "Catalog.Product";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class ProductPackagePermissions
    {

        public static string GroupName { get; set; } = "Catalog.ProductPackage";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class WarehousePermissions
    {

        public static string GroupName { get; set; } = "Inventory.Warehouse";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class InventoryItemPermissions
    {

        public static string GroupName { get; set; } = "Inventory.InventoryItem";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class InventoryPermissions
    {

        public static string GroupName { get; set; } = "Inventory.Inventory";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class StockTransactionPermissions
    {

        public static string GroupName { get; set; } = "Inventory.StockTransaction";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }
    public static class WarehouseTransferPermissions
    {
        public static string GroupName { get; set; } = "Inventory.WarehouseTransfer";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Ship { get; set; } = $"{GroupName}.Ship";
        public static string Receive { get; set; } = $"{GroupName}.Receive";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";

        public static List<string> Permissions =>
        [
            Select,
            View,
            Create,
            Edit,
            Delete,
            Ship,
            Receive,
            Cancel
        ];
    }
    public static class BatchPermissions
    {

        public static string GroupName { get; set; } = "Inventory.Batch";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };

    }

    public static class CustomerGroupPermissions
    {

        public static string GroupName { get; set; } = "Customers.CustomerGroup";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class CustomerPricingProfilePermissions
    {

        public static string GroupName { get; set; } = "Customers.CustomerPricingProfile";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class CustomerPermissions
    {

        public static string GroupName { get; set; } = "Customers.Customer";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class SalesOrderPermissions
    {

        public static string GroupName { get; set; } = "SalesOrders.Order";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Confirm { get; set; } = $"{GroupName}.Confirm";
        public static string Deliver { get; set; } = $"{GroupName}.Deliver";
        public static string Invoice { get; set; } = $"{GroupName}.Invoice";
        public static string Complete { get; set; } = $"{GroupName}.Complete";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Return { get; set; } = $"{GroupName}.Return";
        public static string ViewReports { get; set; } = $"{GroupName}.ViewReports";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Confirm}",
                $"{Deliver}",
                $"{Invoice}",
                $"{Complete}",
                $"{Cancel}",
                $"{Return}",
                $"{ViewReports}",
            };
    }

    public static class SalesQuotationPermissions
    {
        public static string GroupName { get; set; } = "Sales.Quotation";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Send { get; set; } = $"{GroupName}.Send";
        public static string Accept { get; set; } = $"{GroupName}.Accept";
        public static string Reject { get; set; } = $"{GroupName}.Reject";
        public static string Convert { get; set; } = $"{GroupName}.Convert";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";

        public static List<string> Permissions =>
        [
            Select, View, Create, Edit, Delete, Send, Accept, Reject, Convert, Cancel
        ];
    }

    public static class SalesDeliveryNotePermissions
    {
        public static string GroupName { get; set; } = "Sales.DeliveryNote";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Post { get; set; } = $"{GroupName}.Post";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";

        public static List<string> Permissions =>
        [
            Select, View, Create, Edit, Post, Cancel
        ];
    }

    public static class SalesReturnPermissions
    {
        public static string GroupName { get; set; } = "Sales.Return";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Post { get; set; } = $"{GroupName}.Post";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Credit { get; set; } = $"{GroupName}.Credit";

        public static List<string> Permissions =>
        [
            Select, View, Create, Edit, Post, Cancel, Credit
        ];
    }

    public static class SalesReportPermissions
    {
        public static string GroupName { get; set; } = "Sales.Report";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Export { get; set; } = $"{GroupName}.Export";

        public static List<string> Permissions => [View, Export];
    }

    public static class SalesPriceOverridePermissions
    {
        public static string GroupName { get; set; } = "Sales.PriceOverride";
        public static string Apply { get; set; } = $"{GroupName}.Apply";
        public static string Manage { get; set; } = $"{GroupName}.Manage";

        public static List<string> Permissions => [Apply, Manage];
    }

    public static class CartPermissions
    {
        public static string GroupName { get; set; } = "Cart.Cart";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Checkout { get; set; } = $"{GroupName}.Checkout";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Checkout}",
            };
    }

    public static class OrderIntakePermissions
    {
        public static string GroupName { get; set; } = "Orders.Intake";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Accept { get; set; } = $"{GroupName}.Accept";
        public static string Reject { get; set; } = $"{GroupName}.Reject";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Accept}",
                $"{Reject}",
            };
    }

    public static class PaymentPermissions
    {
        public static string GroupName { get; set; } = "Payments.Payment";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Reject { get; set; } = $"{GroupName}.Reject";
        public static string Refund { get; set; } = $"{GroupName}.Refund";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Approve}",
                $"{Reject}",
                $"{Refund}",
            };
    }

    public static class ContractPermissions
    {
        public static string GroupName { get; set; } = "Contracts.Contract";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string SubmitReview { get; set; } = $"{GroupName}.SubmitReview";
        public static string Sign { get; set; } = $"{GroupName}.Sign";
        public static string Activate { get; set; } = $"{GroupName}.Activate";
        public static string Terminate { get; set; } = $"{GroupName}.Terminate";
        public static string Renew { get; set; } = $"{GroupName}.Renew";

        public static List<string> Permissions => [Select, View, Create, Edit, Delete, SubmitReview, Sign, Activate, Terminate, Renew];
    }

    public static class ContractTemplatePermissions
    {
        public static string GroupName { get; set; } = "Contracts.Template";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions => [View, Create, Edit, Delete];
    }

    public static class ContractRenewalPermissions
    {
        public static string GroupName { get; set; } = "Contracts.Renewal";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Configure { get; set; } = $"{GroupName}.Configure";
        public static string Process { get; set; } = $"{GroupName}.Process";
        public static string RecordPayment { get; set; } = $"{GroupName}.RecordPayment";

        public static List<string> Permissions => [View, Configure, Process, RecordPayment];
    }

    public static class DocumentManagementPermissions
    {
        public static string GroupName { get; set; } = "DocumentManagement.Document";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Share { get; set; } = $"{GroupName}.Share";
        public static string ManageAll { get; set; } = $"{GroupName}.ManageAll";
        public static string Configure { get; set; } = $"{GroupName}.Configure";

        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Share, ManageAll, Configure];
    }

    public static class MediaCenterPermissions
    {
        public static string GroupName { get; set; } = "MediaCenter.Activity";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Upload { get; set; } = $"{GroupName}.Upload";
        public static string ManageTypes { get; set; } = $"{GroupName}.ManageTypes";

        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Upload, ManageTypes];
    }

    public static class PricingPermissions
    {

        public static string GroupName { get; set; } = "Pricing.PriceList";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class PurchaseRequestPermissions
    {
        public static string GroupName { get; set; } = "Procurement.PurchaseRequest";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Submit { get; set; } = $"{GroupName}.Submit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Reject { get; set; } = $"{GroupName}.Reject";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Submit, Approve, Reject, Cancel, Close];
    }

    public static class RequestForQuotationPermissions
    {
        public static string GroupName { get; set; } = "Procurement.RequestForQuotation";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Submit { get; set; } = $"{GroupName}.Submit";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Submit, Cancel, Close];
    }

    public static class SupplierQuotationPermissions
    {
        public static string GroupName { get; set; } = "Procurement.SupplierQuotation";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Reject { get; set; } = $"{GroupName}.Reject";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Approve, Reject];
    }

    public static class PurchaseOrderPermissions
    {
        public static string GroupName { get; set; } = "Procurement.PurchaseOrder";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Submit { get; set; } = $"{GroupName}.Submit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Receive { get; set; } = $"{GroupName}.Receive";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Submit, Approve, Cancel, Receive, Close];
    }

    public static class GoodsReceiptPermissions
    {
        public static string GroupName { get; set; } = "Procurement.GoodsReceipt";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Receive { get; set; } = $"{GroupName}.Receive";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Receive, Cancel];
    }

    public static class PurchaseReturnPermissions
    {
        public static string GroupName { get; set; } = "Procurement.PurchaseReturn";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Receive { get; set; } = $"{GroupName}.Receive";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Receive, Cancel];
    }

    public static class SupplierInvoicePermissions
    {
        public static string GroupName { get; set; } = "Procurement.SupplierInvoice";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Approve, Cancel, Close];
    }

    public static class PayrollContractPermissions
    {

        public static string GroupName { get; set; } = "Payroll.Contract";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class SalaryRunPermissions
    {

        public static string GroupName { get; set; } = "Payroll.SalaryRun";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string AdminOverride { get; set; } = $"{GroupName}.AdminOverride";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Approve}",
                $"{AdminOverride}",
            };
    }

    public static class PayrollLoanPermissions
    {

        public static string GroupName { get; set; } = "Payroll.Loan";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Approve}",
                $"{Cancel}",
            };
    }

    public static class PayrollStructurePermissions
    {
        public static string GroupName { get; set; } = "Payroll.Structure";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Assign { get; set; } = $"{GroupName}.Assign";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Assign];
    }

    public static class PayrollPayslipPermissions
    {
        public static string GroupName { get; set; } = "Payroll.Payslip";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Generate { get; set; } = $"{GroupName}.Generate";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Pay { get; set; } = $"{GroupName}.Pay";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Generate, Approve, Pay, Close];
    }

    public static class PayrollWorkEntryPermissions
    {
        public static string GroupName { get; set; } = "Payroll.WorkEntry";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Import { get; set; } = $"{GroupName}.Import";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static List<string> Permissions => [Select, View, Import, Approve];
    }

    public static class SupplierGroupPermissions
    {

        public static string GroupName { get; set; } = "Suppliers.SupplierGroup";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class SupplierPermissions
    {

        public static string GroupName { get; set; } = "Suppliers.Supplier";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class TaskManagementPermissions
    {

        public static string GroupName { get; set; } = "TaskManagement.Task";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Assign { get; set; } = $"{GroupName}.Assign";
        public static string Reassign { get; set; } = $"{GroupName}.Reassign";
        public static string Comment { get; set; } = $"{GroupName}.Comment";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static string ViewReports { get; set; } = $"{GroupName}.ViewReports";
        public static string ManageAllTasks { get; set; } = $"{GroupName}.ManageAllTasks";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Assign}",
                $"{Reassign}",
                $"{Comment}",
                $"{Close}",
                $"{ViewReports}",
                $"{ManageAllTasks}",
            };
    }

    public static class ProjectManagementPermissions
    {

        public static string GroupName { get; set; } = "ProjectManagement.Project";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Plan { get; set; } = $"{GroupName}.Plan";
        public static string Inventory { get; set; } = $"{GroupName}.Inventory";
        public static string Distribution { get; set; } = $"{GroupName}.Distribution";
        public static string Tasks { get; set; } = $"{GroupName}.Tasks";
        public static string Budget { get; set; } = $"{GroupName}.Budget";
        public static string ViewReports { get; set; } = $"{GroupName}.ViewReports";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Plan}",
                $"{Inventory}",
                $"{Distribution}",
                $"{Tasks}",
                $"{Budget}",
                $"{ViewReports}",
            };
    }

    public static class MaintenanceAssetPermissions
    {
        public static string GroupName { get; set; } = "Maintenance.Asset";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
            };
    }

    public static class MaintenanceWorkOrderPermissions
    {
        public static string GroupName { get; set; } = "Maintenance.WorkOrder";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Assign { get; set; } = $"{GroupName}.Assign";
        public static string ApproveCost { get; set; } = $"{GroupName}.ApproveCost";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static string ViewReports { get; set; } = $"{GroupName}.ViewReports";
        public static string ManageAll { get; set; } = $"{GroupName}.ManageAll";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{Delete}",
                $"{Assign}",
                $"{ApproveCost}",
                $"{Close}",
                $"{ViewReports}",
                $"{ManageAll}",
            };
    }

    public static class RealEstatePropertyPermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Property";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class RealEstateUnitPermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Unit";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class RealEstateLeasePermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Lease";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Activate { get; set; } = $"{GroupName}.Activate";
        public static string Suspend { get; set; } = $"{GroupName}.Suspend";
        public static string Terminate { get; set; } = $"{GroupName}.Terminate";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Activate, Suspend, Terminate];
    }

    public static class RealEstateInstallmentPermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Installment";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Generate { get; set; } = $"{GroupName}.Generate";
        public static string RecordPayment { get; set; } = $"{GroupName}.RecordPayment";
        public static List<string> Permissions => [View, Generate, RecordPayment];
    }

    public static class RealEstateUtilityPermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Utility";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class RealEstateExpensePermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Expense";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class RealEstateReportsPermissions
    {
        public static string GroupName { get; set; } = "RealEstate.Reports";
        public static string View { get; set; } = $"{GroupName}.View";
        public static List<string> Permissions => [View];
    }

    public static class FleetVehiclePermissions
    {
        public static string GroupName { get; set; } = "Fleet.Vehicle";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";

        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class FleetVehicleAssignmentPermissions
    {
        public static string GroupName { get; set; } = "Fleet.VehicleAssignment";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Close { get; set; } = $"{GroupName}.Close";

        public static List<string> Permissions => [View, Create, Edit, Close];
    }

    public static class FleetVehicleExpensePermissions
    {
        public static string GroupName { get; set; } = "Fleet.VehicleExpense";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Approve { get; set; } = $"{GroupName}.Approve";

        public static List<string> Permissions => [View, Create, Edit, Delete, Approve];
    }

    public static class FleetVehicleDocumentPermissions
    {
        public static string GroupName { get; set; } = "Fleet.VehicleDocument";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Renew { get; set; } = $"{GroupName}.Renew";

        public static List<string> Permissions => [View, Create, Edit, Delete, Renew];
    }

    public static class FleetReportsPermissions
    {
        public static string GroupName { get; set; } = "Fleet.Reports";
        public static string View { get; set; } = $"{GroupName}.View";

        public static List<string> Permissions => [View];
    }

    public static class AttendancePermissions
    {
        public static string GroupName { get; set; } = "Attendance.Attendance";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string ReviewRequests { get; set; } = $"{GroupName}.ReviewRequests";
        public static string ViewReports { get; set; } = $"{GroupName}.ViewReports";
        public static string ViewConfiguration { get; set; } = $"{GroupName}.ViewConfiguration";
        public static string ManageConfiguration { get; set; } = $"{GroupName}.ManageConfiguration";
        public static string ManageHolidays { get; set; } = $"{GroupName}.ManageHolidays";
        public static string RequestMidDayPermission { get; set; } = $"{GroupName}.RequestMidDayPermission";
        public static string ApproveMidDayPermission { get; set; } = $"{GroupName}.ApproveMidDayPermission";
        public static string ViewAllReports { get; set; } = $"{GroupName}.ViewAllReports";
        public static string ViewScopedReports { get; set; } = $"{GroupName}.ViewScopedReports";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
                $"{ReviewRequests}",
                $"{ViewReports}",
                $"{ViewConfiguration}",
                $"{ManageConfiguration}",
                $"{ManageHolidays}",
                $"{RequestMidDayPermission}",
                $"{ApproveMidDayPermission}",
                $"{ViewAllReports}",
                $"{ViewScopedReports}",
            };
    }

    public static class AttendanceRosterPermissions
    {
        public static string GroupName { get; set; } = "Attendance.Roster";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Publish { get; set; } = $"{GroupName}.Publish";
        public static string ApproveSwap { get; set; } = $"{GroupName}.ApproveSwap";
        public static List<string> Permissions => [Select, View, Create, Edit, Publish, ApproveSwap];
    }

    public static class AttendanceWorkEntryPermissions
    {
        public static string GroupName { get; set; } = "Attendance.WorkEntry";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Generate { get; set; } = $"{GroupName}.Generate";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static List<string> Permissions => [Select, View, Generate, Edit, Approve];
    }

    public static class LeavePermissions
    {
        public static string GroupName { get; set; } = "Leave.Leave";
        public static string RequestEmergencyLeave { get; set; } = $"{GroupName}.RequestEmergencyLeave";
        public static string ApproveEmergencyLeave { get; set; } = $"{GroupName}.ApproveEmergencyLeave";
        public static string ViewLeaveBalances { get; set; } = $"{GroupName}.ViewLeaveBalances";
        public static string ManageLeaveBalances { get; set; } = $"{GroupName}.ManageLeaveBalances";
        public static string ViewLeaveReports { get; set; } = $"{GroupName}.ViewLeaveReports";

        public static List<string> Permissions =>
            [
                RequestEmergencyLeave,
                ApproveEmergencyLeave,
                ViewLeaveBalances,
                ManageLeaveBalances,
                ViewLeaveReports
            ];
    }

    public static class LeavePolicyPermissions
    {
        public static string GroupName { get; set; } = "Leave.Policy";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Assign { get; set; } = $"{GroupName}.Assign";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Assign];
    }

    public static class LeaveApplicationPermissions
    {
        public static string GroupName { get; set; } = "Leave.Application";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Request { get; set; } = $"{GroupName}.Request";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Cancel { get; set; } = $"{GroupName}.Cancel";
        public static List<string> Permissions => [Select, View, Create, Edit, Request, Approve, Cancel];
    }

    public static class LeaveLedgerPermissions
    {
        public static string GroupName { get; set; } = "Leave.Ledger";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Adjust { get; set; } = $"{GroupName}.Adjust";
        public static string Encash { get; set; } = $"{GroupName}.Encash";
        public static List<string> Permissions => [Select, View, Adjust, Encash];
    }

    public static class RecruitmentPermissions
    {
        public static string GroupName { get; set; } = "HR.Recruitment";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static string Hire { get; set; } = $"{GroupName}.Hire";
        public static List<string> Permissions => [Select, View, Create, Edit, Approve, Hire];
    }

    public static class PerformancePermissions
    {
        public static string GroupName { get; set; } = "HR.Performance";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Review { get; set; } = $"{GroupName}.Review";
        public static string Approve { get; set; } = $"{GroupName}.Approve";
        public static List<string> Permissions => [Select, View, Create, Edit, Review, Approve];
    }

    public static class TrainingPermissions
    {
        public static string GroupName { get; set; } = "HR.Training";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Complete { get; set; } = $"{GroupName}.Complete";
        public static List<string> Permissions => [Select, View, Create, Edit, Complete];
    }

    public static class SystemSettingsPermissions
    {
        public static string GroupName { get; set; } = "GeneralSettings.SystemSettings";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";

        public static List<string> Permissions =>
            new List<string>
            {
                $"{Select}",
                $"{View}",
                $"{Create}",
                $"{Edit}",
            };
    }

    public static class DemoDataPermissions
    {
        public static string GroupName { get; set; } = "GeneralSettings.DemoData";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Reset { get; set; } = $"{GroupName}.Reset";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string ResetAdminPassword { get; set; } = $"{GroupName}.ResetAdminPassword";

        public static List<string> Permissions => [View, Create, Reset, Delete, ResetAdminPassword];
    }

    public static class StoreFrontStorePermissions
    {
        public static string GroupName { get; set; } = "StoreFront.Store";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class StoreFrontDepartmentPermissions
    {
        public static string GroupName { get; set; } = "StoreFront.Department";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [View, Create, Edit, Delete];
    }

    public static class StoreFrontItemPermissions
    {
        public static string GroupName { get; set; } = "StoreFront.Item";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Edit];
    }

    public static class StoreFrontPosPermissions
    {
        public static string GroupName { get; set; } = "StoreFront.POS";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Checkout { get; set; } = $"{GroupName}.Checkout";
        public static string PriceOverride { get; set; } = $"{GroupName}.PriceOverride";
        public static string OpenSession { get; set; } = $"{GroupName}.OpenSession";
        public static string CloseSession { get; set; } = $"{GroupName}.CloseSession";
        public static string ViewOwnSummary { get; set; } = $"{GroupName}.ViewOwnSummary";
        public static string ViewBranchSummaries { get; set; } = $"{GroupName}.ViewBranchSummaries";
        public static string HandoverCash { get; set; } = $"{GroupName}.HandoverCash";
        public static string ManageCashAccounts { get; set; } = $"{GroupName}.ManageCashAccounts";
        public static List<string> Permissions => [View, Checkout, PriceOverride, OpenSession, CloseSession, ViewOwnSummary, ViewBranchSummaries, HandoverCash, ManageCashAccounts];
    }

    public static class CateringContractPermissions
    {
        public static string GroupName { get; set; } = "Catering.Contract";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Addendum { get; set; } = $"{GroupName}.Addendum";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Addendum, Close];
    }

    public static class CateringMealPermissions
    {
        public static string GroupName { get; set; } = "Catering.Meal";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class CateringLocationPermissions
    {
        public static string GroupName { get; set; } = "Catering.Location";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class CateringSchedulePermissions
    {
        public static string GroupName { get; set; } = "Catering.Schedule";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Create, Edit];
    }

    public static class CateringDeliveryPermissions
    {
        public static string GroupName { get; set; } = "Catering.Delivery";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Create, Edit];
    }

    public static class CateringDistributionPermissions
    {
        public static string GroupName { get; set; } = "Catering.Distribution";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Edit];
    }

    public static class CateringAssignmentPermissions
    {
        public static string GroupName { get; set; } = "Catering.Assignment";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [View, Create, Edit, Delete];
    }

    public static class CateringReportsPermissions
    {
        public static string GroupName { get; set; } = "Catering.Reports";
        public static string View { get; set; } = $"{GroupName}.View";
        public static List<string> Permissions => [View];
    }

    public static class AccountingDashboardPermissions
    {
        public static string GroupName { get; set; } = "Accounting.Dashboard";
        public static string View { get; set; } = $"{GroupName}.View";
        public static List<string> Permissions => [View];
    }

    public static class AccountingTemplatePermissions
    {
        public static string GroupName { get; set; } = "Accounting.Template";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static string Share { get; set; } = $"{GroupName}.Share";
        public static string Apply { get; set; } = $"{GroupName}.Apply";
        public static List<string> Permissions => [View, Create, Edit, Delete, Share, Apply];
    }

    public static class AccountPermissions
    {
        public static string GroupName { get; set; } = "Accounting.Account";
        public static string Select { get; set; } = $"{GroupName}.Select";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [Select, View, Create, Edit, Delete];
    }

    public static class FiscalPeriodPermissions
    {
        public static string GroupName { get; set; } = "Accounting.FiscalPeriod";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Close { get; set; } = $"{GroupName}.Close";
        public static string Lock { get; set; } = $"{GroupName}.Lock";
        public static string Reopen { get; set; } = $"{GroupName}.Reopen";
        public static string YearEndClose { get; set; } = $"{GroupName}.YearEndClose";
        public static List<string> Permissions => [View, Create, Close, Lock, Reopen, YearEndClose];
    }

    public static class TaxCodePermissions
    {
        public static string GroupName { get; set; } = "Accounting.TaxCode";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Create, Edit];
    }

    public static class PostingProfilePermissions
    {
        public static string GroupName { get; set; } = "Accounting.PostingProfile";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Create, Edit];
    }

    public static class BankAccountPermissions
    {
        public static string GroupName { get; set; } = "Accounting.BankAccount";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [View, Create, Edit, Delete];
    }

    public static class CashAccountPermissions
    {
        public static string GroupName { get; set; } = "Accounting.CashAccount";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static string Delete { get; set; } = $"{GroupName}.Delete";
        public static List<string> Permissions => [View, Create, Edit, Delete];
    }

    public static class AccountingSettingsPermissions
    {
        public static string GroupName { get; set; } = "Accounting.Settings";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Edit];
    }

    public static class JournalEntryPermissions
    {
        public static string GroupName { get; set; } = "Accounting.JournalEntry";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Post { get; set; } = $"{GroupName}.Post";
        public static string Reverse { get; set; } = $"{GroupName}.Reverse";
        public static List<string> Permissions => [View, Create, Post, Reverse];
    }

    public static class AccountingDocumentPermissions
    {
        public static string GroupName { get; set; } = "Accounting.Document";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Post { get; set; } = $"{GroupName}.Post";
        public static string Reverse { get; set; } = $"{GroupName}.Reverse";
        public static List<string> Permissions => [View, Create, Post, Reverse];
    }

    public static class BankReconciliationPermissions
    {
        public static string GroupName { get; set; } = "Accounting.BankReconciliation";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Create { get; set; } = $"{GroupName}.Create";
        public static string Reconcile { get; set; } = $"{GroupName}.Reconcile";
        public static List<string> Permissions => [View, Create, Reconcile];
    }

    public static class AccountingReportPermissions
    {
        public static string GroupName { get; set; } = "Accounting.Report";
        public static string View { get; set; } = $"{GroupName}.View";
        public static List<string> Permissions => [View];
    }

    public static class AccountingBranchAccessPermissions
    {
        public static string GroupName { get; set; } = "Accounting.BranchAccess";
        public static string ViewAll { get; set; } = $"{GroupName}.ViewAll";
        public static List<string> Permissions => [ViewAll];
    }

    public static class ZatcaSettingsPermissions
    {
        public static string GroupName { get; set; } = "Accounting.ZatcaSettings";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Edit { get; set; } = $"{GroupName}.Edit";
        public static List<string> Permissions => [View, Edit];
    }

    public static class ZatcaEInvoicePermissions
    {
        public static string GroupName { get; set; } = "Accounting.ZatcaEInvoice";
        public static string View { get; set; } = $"{GroupName}.View";
        public static string Generate { get; set; } = $"{GroupName}.Generate";
        public static string Submit { get; set; } = $"{GroupName}.Submit";
        public static string Retry { get; set; } = $"{GroupName}.Retry";
        public static List<string> Permissions => [View, Generate, Submit, Retry];
    }



}
