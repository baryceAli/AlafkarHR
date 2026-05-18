using SharedWithUI.Permissions;

namespace AlAfkarERP.Shared.Layout;

public class MenuItem
{
    public string TextAr { get; set; } = default!;
    public string TextEn { get; set; } = default!;
    public string PermissionPolicy { get; set; }
    public string Icon { get; set; } = default!;
    public string? Url { get; set; }

    public List<MenuItem> Children { get; set; } = new();

    public bool IsOpen { get; set; } = false;

    // helper (not stored)
    public bool IsActive { get; set; } = false;


    public static List<MenuItem> Menu = new()
    {
        //home
        new MenuItem
        {
            TextEn = "Home",
            TextAr = "الرئيسية",
            Icon = "bi-house-door",
            Url = "/",
            PermissionPolicy = "View.Home"
        },

        //Control Panel
        new MenuItem
        {
            TextEn = "Control Panel",
            TextAr = "لوحة التحكم",
            Icon = "bi-speedometer2",
            Url = "/Dashboard",
            PermissionPolicy = "View.Dashboard"
        },
        
        //Organizational Structure
        new MenuItem
        {
            TextEn = "Organizational Structure",
            TextAr = "الهيكل التنظيمي",
            Icon = "bi-diagram-3",
            Url = "/Organization/Dashboard",
            PermissionPolicy = $"{PermissionList.CompanyPermissions.View}",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Company",
                    TextAr = "الشركة",
                    Icon = "bi-building",
                    Url = "/Organization/Company/List",
                    PermissionPolicy = "Organization.Company.View"
                },
                new MenuItem
                {
                    TextEn = "Branches",
                    TextAr = "الفروع",
                    Icon = "bi-diagram-2",
                    Url = "/Organization/Branch/List",
                    PermissionPolicy = "Organization.Branch.View"
                },
                new MenuItem
                {
                    TextEn = "Administrations",
                    TextAr = "الإدارات",
                    Icon = "bi-kanban",
                    Url = "/Organization/Administration/List",
                    PermissionPolicy = "Organization.Administration.View"
                },
                new MenuItem
                {
                    TextEn = "Departments",
                    TextAr = "الأقسام",
                    Icon = "bi-grid-1x2",
                    Url = "/Organization/Department/List",
                    PermissionPolicy = "Organization.Department.View"
                }
            }
        },

        //Human Resource
        new MenuItem
        {
            TextEn = "Human Resource",
            TextAr = "الموارد البشرية",
            Icon = "bi-people-fill",
            Url = "/Employee/Dashboard",
            PermissionPolicy = "View.HR",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Employees",
                    TextAr = "الموظفين",
                    Icon = "bi-person-badge",
                    Url = "/Employee/Employee/List",
                    PermissionPolicy = "HR.Employee.View"
                },
                new MenuItem
                {
                    TextEn = "Positions",
                    TextAr = "المسميات الوظيفية",
                    Icon = "bi-briefcase-fill",
                    Url = "/Employee/Position/List",
                    PermissionPolicy = "HR.Position.View"
                },
                new MenuItem
                {
                    TextEn = "Academic Institutions",
                    TextAr = "المؤسسات التعليمية",
                    Icon = "bi-mortarboard-fill",
                    Url = "/Employee/AcademicInistitution/List",
                    PermissionPolicy = "HR.Academic.View"
                },
                new MenuItem
                {
                    TextEn = "Specializations",
                    TextAr = "التخصصات",
                    Icon = "bi-journal-bookmark",
                    Url = "/Employee/Specialization/List",
                    PermissionPolicy = "HR.Specialization.View"
                }
            }
        },
        
        //Products Management
        new MenuItem
        {
            TextEn = "Products Management",
            TextAr = "إدارة المنتجات",
            Icon = "bi-tags-fill",
            Url = "/Warehouse/Product/Dashboard",
            PermissionPolicy = "Inventory.Products",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "SKU",
                    TextAr = "المنتج المخزني",
                    Icon = "bi-upc-scan",
                    Url = "/Catalog/Product/ProductSku/List",
                    PermissionPolicy = PermissionList.ProductPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Products",
                    TextAr = "المنتجات",
                    Icon = "bi-box",
                    Url = "/Catalog/Product/List",
                    PermissionPolicy = PermissionList.ProductPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Product Options",
                    TextAr = "خيارات المنتج",
                    Icon = "bi-sliders",
                    Url = "/Catalog/Variant/List",
                    PermissionPolicy = "Catalog.Variant.View"
                },

                new MenuItem
                {
                    TextEn = "Pakcages",
                    TextAr = "العبوات",
                    Icon = "bi-archive-fill",
                    Url = "/Warehouse/Product/Packages/List",
                    PermissionPolicy = PermissionList.ProductPackagePermissions.View
                },
                new MenuItem
                {
                    TextEn = "Brand",
                    TextAr = "العلامات التجارية",
                    Icon = "bi-award-fill",
                    Url = "/Warehouse/Product/Brand/List",
                    PermissionPolicy = "Catalog.Brand.View"
                },
                new MenuItem
                {
                    TextEn = "Categories",
                    TextAr = "الأصناف",
                    Icon = "bi-diagram-3-fill",
                    Url = "/Warehouse/Product/Category/List",
                    PermissionPolicy = "Catalog.Category.View"
                },
                new MenuItem
                {
                    TextEn = "Units",
                    TextAr = "الوحدات",
                    Icon = "bi-rulers",
                    Url = "/Warehouse/Product/Unit/List",
                    PermissionPolicy = "Catalog.Unit.View"
                }
            }
        },
        
        //Inventory
        new MenuItem
        {
            TextEn = "Inventory Management",
            TextAr = "إدارة المخزون",
            Icon = "bi-boxes",
            Url = "/Inventory/Dashboard",
            PermissionPolicy = "Inventory.Management",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Warehouses",
                    TextAr = "المستودعات",
                    Icon = "bi-building",
                    Url = "/Inventory/Warehouse/List",
                    PermissionPolicy = "View.Inventory",
                },

                new MenuItem
                {
                    TextEn = "Current Stock",
                    TextAr = "المخزون",
                    Icon = "bi-boxes",
                    Url = "/Inventories/List",
                    PermissionPolicy = "View.Inventory",
                },

                new MenuItem
                {
                    TextEn = "Batches",
                    TextAr = "الدفعات",
                    Icon = "bi-upc-scan",
                    Url = "/Inventory/Batch/List",
                    PermissionPolicy = "View.Inventory",
                },

                new MenuItem
                {
                    TextEn = "Stock Operations",
                    TextAr = "عمليات المخزون",
                    Icon = "bi-arrow-left-right",
                    Url = "/Inventory/Dashboard",
                    PermissionPolicy = "View.Inventory",

                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Stock In",
                            TextAr = "مخزون وارد",
                            Icon = "bi-box-arrow-in-down",
                            Url = "/Inventory/Operations/StockIn",
                            PermissionPolicy = "View.Inventory",
                        },

                        new MenuItem
                        {
                            TextEn = "Stock Out",
                            TextAr = "مخزون صادر",
                            Icon = "bi-box-arrow-up",
                            Url = "/Inventory/Operations/StockOut",
                            PermissionPolicy = "View.Inventory",
                        },

                        new MenuItem
                        {
                            TextEn = "Adjustments",
                            TextAr = "تسويات",
                            Icon = "bi-sliders",
                            Url = "/Inventory/Operations/StockAdjustment",
                            PermissionPolicy = "View.Inventory",
                        },

                        new MenuItem
                        {
                            TextEn = "Reserve",
                            TextAr = "حجز مخزون",
                            Icon = "bi-lock",
                            Url = "/Inventory/Operations/StockReservation",
                            PermissionPolicy = "View.Inventory",
                        },

                        new MenuItem
                        {
                            TextEn = "Release",
                            TextAr = "إطلاق مخزون",
                            Icon = "bi-unlock",
                            Url = "/Inventory/Operations/StockRelease",
                            PermissionPolicy = "View.Inventory",
                        },
                    }
                },

                //new MenuItem
                //{
                //    TextEn = "Transfers",
                //    TextAr = "تحويل المخزون",
                //    Icon = "bi-truck",
                //    Url = "/Inventory/WarehouseTransfer/Form",
                //    PermissionPolicy = "View.Inventory",
                //},

                //new MenuItem
                //{
                //    TextEn = "Movements",
                //    TextAr = "حركة المخزون",
                //    Icon = "bi-arrow-repeat",
                //    Url = "/Inventory/Dashboard",
                //    PermissionPolicy = "View.Inventory",
                //},

                //new MenuItem
                //{
                //    TextEn = "Expiry Tracking",
                //    TextAr = "تتبع تاريخ الانتهاء",
                //    Icon = "bi-calendar-x",
                //    Url = "/Inventory/Dashboard",
                //    PermissionPolicy = "View.Inventory",
                //},

                //new MenuItem
                //{
                //    TextEn = "Stock Count",
                //    TextAr = "جرد المخزون",
                //    Icon = "bi-clipboard-check",
                //    Url = "/Inventory/Dashboard",
                //    PermissionPolicy = "View.Inventory",
                //},

                //new MenuItem
                //{
                //    TextEn = "Reports",
                //    TextAr = "تقارير",
                //    Icon = "bi-bar-chart",
                //    Url = "/Inventory/Dashboard",
                //    PermissionPolicy = "View.Inventory",
                //},
            }
        },

        //Security Management
        new MenuItem
        {
            TextEn = "Security Management",
            TextAr = "إدارة الأمان",
            Icon = "bi-shield-lock-fill",
            Url = "/Auth/Dashboard",
            PermissionPolicy = "View.Security",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Roles Management",
                    TextAr = "إدارة الصلاحيات",
                    Icon = "bi-shield-check",
                    Url = "/Auth/Role/List",
                    PermissionPolicy = "Security.Role.View"
                },
                new MenuItem
                {
                    TextEn = "Assign User Roles",
                    TextAr = "تعيين صلاحيات المستخدمين",
                    Icon = "bi-person-gear",
                    Url = "/Auth/User/AssignRole",
                    PermissionPolicy = "Security.UserRole.Assign"
                }
            }
        }
    };
}