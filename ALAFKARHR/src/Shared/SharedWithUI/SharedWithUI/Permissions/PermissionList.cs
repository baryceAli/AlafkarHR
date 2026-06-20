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
            ..PositionPermissions.Permissions,
            ..CompanyPermissions.Permissions,
            ..BranchPermissions.Permissions,
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
            .. BatchPermissions.Permissions,
            .. CustomerGroupPermissions.Permissions,
            .. CustomerPricingProfilePermissions.Permissions,
            .. CustomerPermissions.Permissions,
            .. SupplierGroupPermissions.Permissions,
            .. SupplierPermissions.Permissions,
            .. CartPermissions.Permissions,
            .. OrderIntakePermissions.Permissions,
            .. PaymentPermissions.Permissions,
            .. ContractPermissions.Permissions,
            .. ContractTemplatePermissions.Permissions,
            .. ContractRenewalPermissions.Permissions,
            .. DocumentManagementPermissions.Permissions,
            .. SalesOrderPermissions.Permissions,
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
            .. AttendancePermissions.Permissions,
            .. TaskManagementPermissions.Permissions,
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
            .. SystemSettingsPermissions.Permissions,
        ];



        return list;

    }

    public static List<string> GetPlatformPermissions()
        => ParentCompanyPermissions.Permissions
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
            CompanyPermissions.ViewChild,
            CompanyPermissions.CreateChild,
            CompanyPermissions.EditChild,
            CompanyPermissions.DisableChild,
            CompanyPermissions.ResetChildAdminPassword,
            ..UsersPermissions.Permissions,
            ..RolesPermissions.Permissions,
            SystemSettingsPermissions.Select,
            SystemSettingsPermissions.View,
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

        public static List<string> Permissions => [Select, View, Create, Edit, Delete, Share, ManageAll];
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
        public static string RequestEmergencyLeave { get; set; } = $"{GroupName}.RequestEmergencyLeave";
        public static string ApproveEmergencyLeave { get; set; } = $"{GroupName}.ApproveEmergencyLeave";
        public static string ViewLeaveBalances { get; set; } = $"{GroupName}.ViewLeaveBalances";
        public static string ManageLeaveBalances { get; set; } = $"{GroupName}.ManageLeaveBalances";
        public static string ViewLeaveReports { get; set; } = $"{GroupName}.ViewLeaveReports";
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
                $"{RequestEmergencyLeave}",
                $"{ApproveEmergencyLeave}",
                $"{ViewLeaveBalances}",
                $"{ManageLeaveBalances}",
                $"{ViewLeaveReports}",
                $"{RequestMidDayPermission}",
                $"{ApproveMidDayPermission}",
                $"{ViewAllReports}",
                $"{ViewScopedReports}",
            };
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



}
