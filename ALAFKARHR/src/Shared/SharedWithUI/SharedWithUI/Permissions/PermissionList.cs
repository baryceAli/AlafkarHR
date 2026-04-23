using SharedWithUI.Auth.Dtos;

namespace SharedWithUI.Permissions;

public static class PermissionList
{
    public static List<string> GetAll()
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
        ];



        return list;

    }
    

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

    //public static class ProductVariantPermissions
    //{

    //    //public static string GroupName { get; set; } = "ProductVariant";
    //    public static string Select { get; set; } = "ProductVariant.Select";
    //    public static string View { get; set; } = "ProductVariant.View";
    //    public static string Create { get; set; } = "ProductVariant.Create";
    //    public static string Edit { get; set; } = "ProductVariant.Edit";
    //    public static string Delete { get; set; } = "ProductVariant.Delete";

    //    public static List<string> Permissions =>
    //        new List<string>
    //        {
    //            $"{Select}",
    //            $"{View}",
    //            $"{Create}",
    //            $"{Edit}",
    //            $"{Delete}",
    //        };

    //}



}
