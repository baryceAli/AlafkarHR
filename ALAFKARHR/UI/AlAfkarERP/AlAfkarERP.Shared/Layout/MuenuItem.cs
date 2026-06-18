using SharedWithUI.Permissions;

namespace AlAfkarERP.Shared.Layout;

public class MenuItem
{
    public string TextAr { get; set; } = default!;
    public string TextEn { get; set; } = default!;
    public string PermissionPolicy { get; set; }
    public string Icon { get; set; } = default!;
    public string? Url { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeCssClass { get; set; }
    public string? BadgeTitleEn { get; set; }
    public string? BadgeTitleAr { get; set; }
    public string? WorkspaceKey { get; set; }
    public int? MobilePriority { get; set; }
    public string? KeywordsEn { get; set; }
    public string? KeywordsAr { get; set; }
    public bool IsFavoriteCandidate { get; set; } = true;

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
            Url = "/Dashboard",
            PermissionPolicy = PermissionList.ProductPermissions.View
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
            PermissionPolicy = $"{PermissionList.CompanyPermissions.Select}",
            Children = new()
            {
                new MenuItem
                {
                    TextEn="Dashboard",
                    TextAr="لوحة التحكم",
                    Icon="bi-speedometer2",
                    Url="/Organization/Dashboard",
                    PermissionPolicy=PermissionList.CompanyPermissions.Select
                },
                new MenuItem
                {
                    TextEn = "Company",
                    TextAr = "الشركة",
                    Icon = "bi-building",
                    Url = "/Organization/Company/List",
                    PermissionPolicy = PermissionList.CompanyPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Child Companies",
                    TextAr = "الشركات التابعة",
                    Icon = "bi-buildings",
                    Url = "/Organization/ChildCompanies",
                    PermissionPolicy = PermissionList.CompanyPermissions.ViewChild
                },
                new MenuItem
                {
                    TextEn = "Branches",
                    TextAr = "الفروع",
                    Icon = "bi-diagram-2",
                    Url = "/Organization/Branch/List",
                    PermissionPolicy = PermissionList.BranchPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Administrations",
                    TextAr = "الإدارات",
                    Icon = "bi-kanban",
                    Url = "/Organization/Administration/List",
                    PermissionPolicy = PermissionList.AdministrationPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Departments",
                    TextAr = "الأقسام",
                    Icon = "bi-grid-1x2",
                    Url = "/Organization/Department/List",
                    PermissionPolicy = PermissionList.DepartmentPermissions.View
                }
            }
        },

        //People
        new MenuItem
        {
            TextEn = "People",
            TextAr = "الأفراد",
            Icon = "bi-people",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Human Resource",
                    TextAr = "الموارد البشرية",
                    Icon = "bi-person-workspace",
                    PermissionPolicy = PermissionList.EmployeePermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة الموارد البشرية",
                            Icon = "bi-speedometer2",
                            Url = "/Employee/Dashboard",
                            PermissionPolicy = PermissionList.EmployeePermissions.Select
                        },
                        new MenuItem
                        {
                            TextEn = "Employees",
                            TextAr = "الموظفين",
                            Icon = "bi-person-badge",
                            Url = "/Employee/Employee/List",
                            PermissionPolicy = PermissionList.EmployeePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Positions",
                            TextAr = "المسميات الوظيفية",
                            Icon = "bi-briefcase-fill",
                            Url = "/Employee/Position/List",
                            PermissionPolicy = PermissionList.PositionPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Academic Institutions",
                            TextAr = "المؤسسات التعليمية",
                            Icon = "bi-mortarboard-fill",
                            Url = "/Employee/AcademicInistitution/List",
                            PermissionPolicy = PermissionList.AcademicInistitutionPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Specializations",
                            TextAr = "التخصصات",
                            Icon = "bi-journal-bookmark",
                            Url = "/Employee/Specialization/List",
                            PermissionPolicy = PermissionList.SpecializationPermissions.View
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Attendance",
                    TextAr = "الحضور والانصراف",
                    Icon = "bi-calendar-check",
                    PermissionPolicy = PermissionList.AttendancePermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة الحضور",
                            Icon = "bi-speedometer2",
                            Url = "/Attendance/Dashboard",
                            PermissionPolicy = PermissionList.AttendancePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "My Attendance",
                            TextAr = "حضوري",
                            Icon = "bi-person-check",
                            Url = "/Attendance/MyAttendance",
                            PermissionPolicy = PermissionList.AttendancePermissions.Create
                        },
                        new MenuItem
                        {
                            TextEn = "Sessions",
                            TextAr = "جلسات الحضور",
                            Icon = "bi-clock-history",
                            Url = "/Attendance/Sessions",
                            PermissionPolicy = PermissionList.AttendancePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Shifts",
                            TextAr = "الورديات",
                            Icon = "bi-calendar-range",
                            Url = "/Attendance/Shifts",
                            PermissionPolicy = PermissionList.AttendancePermissions.Edit
                        },
                        new MenuItem
                        {
                            TextEn = "Shift Assignments",
                            TextAr = "تعيين الورديات",
                            Icon = "bi-calendar2-week",
                            Url = "/Attendance/ShiftAssignments",
                            PermissionPolicy = PermissionList.AttendancePermissions.Edit
                        },
                        new MenuItem
                        {
                            TextEn = "Late Requests",
                            TextAr = "طلبات التأخير",
                            Icon = "bi-exclamation-triangle",
                            Url = "/Attendance/LateRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.ReviewRequests
                        },
                        new MenuItem
                        {
                            TextEn = "Configuration",
                            TextAr = "اعدادات الحضور",
                            Icon = "bi-gear",
                            Url = "/Attendance/Configuration",
                            PermissionPolicy = PermissionList.AttendancePermissions.ViewConfiguration
                        },
                        new MenuItem
                        {
                            TextEn = "Holidays",
                            TextAr = "العطلات",
                            Icon = "bi-calendar-event",
                            Url = "/Attendance/Holidays",
                            PermissionPolicy = PermissionList.AttendancePermissions.ManageHolidays
                        },
                        new MenuItem
                        {
                            TextEn = "Permission Requests",
                            TextAr = "طلبات الاذن",
                            Icon = "bi-door-open",
                            Url = "/Attendance/PermissionRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.RequestMidDayPermission
                        },
                        new MenuItem
                        {
                            TextEn = "Approve Permission Requests",
                            TextAr = "اعتماد طلبات الاذن",
                            Icon = "bi-person-check",
                            Url = "/Attendance/ApprovePermissionRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.ApproveMidDayPermission
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "التقارير",
                            Icon = "bi-file-earmark-bar-graph",
                            PermissionPolicy = PermissionList.AttendancePermissions.ViewReports,
                            Children = new()
                            {
                                new MenuItem
                                {
                                    TextEn = "Reports Overview",
                                    TextAr = "نظرة عامة على التقارير",
                                    Icon = "bi-file-earmark-bar-graph",
                                    Url = "/Attendance/Reports",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Daily Attendance Report",
                                    TextAr = "تقرير الحضور اليومي",
                                    Icon = "bi-calendar-day",
                                    Url = "/Attendance/Reports/Attendance",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Employee Attendance Summary",
                                    TextAr = "ملخص حضور الموظف",
                                    Icon = "bi-person-lines-fill",
                                    Url = "/Attendance/Reports/AttendanceSummary",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Late Arrival Report",
                                    TextAr = "تقرير التأخير",
                                    Icon = "bi-clock",
                                    Url = "/Attendance/Reports/LateArrival",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Early Leave Report",
                                    TextAr = "تقرير الانصراف المبكر",
                                    Icon = "bi-box-arrow-right",
                                    Url = "/Attendance/Reports/EarlyLeave",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Break Report",
                                    TextAr = "تقرير الاستراحة",
                                    Icon = "bi-cup-hot",
                                    Url = "/Attendance/Reports/Break",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Permission Requests Report",
                                    TextAr = "تقرير طلبات الاذن",
                                    Icon = "bi-door-open",
                                    Url = "/Attendance/Reports/MidDayPermission",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Absence Report",
                                    TextAr = "تقرير الغياب",
                                    Icon = "bi-person-x",
                                    Url = "/Attendance/Reports/Absence",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                },
                                new MenuItem
                                {
                                    TextEn = "Holiday / Weekend Report",
                                    TextAr = "تقرير العطلات ونهاية الأسبوع",
                                    Icon = "bi-calendar-event",
                                    Url = "/Attendance/Reports/HolidayWeekend",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewReports
                                }
                            }
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Leave Management",
                    TextAr = "إدارة الإجازات",
                    Icon = "bi-calendar-heart",
                    PermissionPolicy = PermissionList.AttendancePermissions.RequestEmergencyLeave,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Emergency Leaves",
                            TextAr = "الإجازات الطارئة",
                            Icon = "bi-life-preserver",
                            Url = "/LeavesManagement/EmergencyLeaves",
                            PermissionPolicy = PermissionList.AttendancePermissions.RequestEmergencyLeave
                        },
                        new MenuItem
                        {
                            TextEn = "Approve Emergency Leave",
                            TextAr = "اعتماد الإجازة الطارئة",
                            Icon = "bi-patch-check",
                            Url = "/LeavesManagement/ApproveEmergencyLeaves",
                            PermissionPolicy = PermissionList.AttendancePermissions.ApproveEmergencyLeave
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Balances",
                            TextAr = "أرصدة الإجازات",
                            Icon = "bi-sliders",
                            Url = "/LeavesManagement/Balances",
                            PermissionPolicy = PermissionList.AttendancePermissions.ViewLeaveBalances
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Reports",
                            TextAr = "تقارير الإجازات",
                            Icon = "bi-file-earmark-bar-graph",
                            Url = "/LeavesManagement/Reports",
                            PermissionPolicy = PermissionList.AttendancePermissions.ViewLeaveReports
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Payroll",
                    TextAr = "الرواتب",
                    Icon = "bi-cash-stack",
                    PermissionPolicy = PermissionList.SalaryRunPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Generate Salaries",
                            TextAr = "توليد الرواتب",
                            Icon = "bi-calculator",
                            Url = "/Payroll/SalaryRuns",
                            PermissionPolicy = PermissionList.SalaryRunPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Salary Contracts",
                            TextAr = "عقود الرواتب",
                            Icon = "bi-file-earmark-text",
                            Url = "/Payroll/Contracts",
                            PermissionPolicy = PermissionList.PayrollContractPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Assign Contract",
                            TextAr = "تعيين عقد",
                            Icon = "bi-person-check",
                            Url = "/Payroll/AssignContract",
                            PermissionPolicy = PermissionList.PayrollContractPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Payroll Components",
                            TextAr = "مكونات الرواتب",
                            Icon = "bi-sliders",
                            Url = "/Payroll/Components",
                            PermissionPolicy = PermissionList.PayrollContractPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Loans & Deductions",
                            TextAr = "السلف والخصومات",
                            Icon = "bi-wallet2",
                            Url = "/Payroll/Loans",
                            PermissionPolicy = PermissionList.PayrollLoanPermissions.View
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Customers Management",
                    TextAr = "إدارة العملاء",
                    Icon = "bi-person-vcard-fill",
                    PermissionPolicy = PermissionList.CustomerPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة العملاء",
                            Icon = "bi-speedometer2",
                            Url = "/Customers/Customer/Dashboard",
                            PermissionPolicy = PermissionList.CustomerPermissions.Select
                        },
                        new MenuItem
                        {
                            TextEn = "Customers List",
                            TextAr = "قائمة العملاء",
                            Icon = "bi-people-fill",
                            Url = "/Customers/Customer/List",
                            PermissionPolicy = PermissionList.CustomerPermissions.View,
                        }, 
                        new MenuItem
                        {
                            TextEn = "Customer Groups",
                            TextAr = "مجموعات العملاء",
                            Icon = "bi-collection-fill",
                            Url = "/Customers/CustomerGroup/List",
                            PermissionPolicy = PermissionList.CustomerGroupPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Special Customer Pricing",
                            TextAr = "تخصيص تسعير العملاء",
                            Icon = "bi-percent",
                            Url = "/Customers/CustomerPricingProfile/List",
                            PermissionPolicy = PermissionList.CustomerPricingProfilePermissions.View
                        }
                    }
                }
            }
        },

        //Operations
        new MenuItem
        {
            TextEn = "Operations",
            TextAr = "العمليات",
            Icon = "bi-box-seam",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Sales",
                    TextAr = "المبيعات",
                    Icon = "bi-receipt-cutoff",
                    PermissionPolicy = PermissionList.SalesOrderPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "POS",
                            TextAr = "نقطة بيع",
                            Icon = "bi-receipt-cutoff",
                            Url = "/SalesOrder/POS",
                            PermissionPolicy = PermissionList.SalesOrderPermissions.View
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Products Management",
                    TextAr = "إدارة المنتجات",
                    Icon = "bi-tags-fill",
                    PermissionPolicy = PermissionList.ProductPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة المنتجات",
                            Icon = "bi-speedometer2",
                            Url = "/Warehouse/Product/Dashboard",
                            PermissionPolicy = PermissionList.ProductPermissions.Select
                        },
                        new MenuItem
                        {
                            TextEn = "Pricing List",
                            TextAr = "قائمة التسعير",
                            Icon = "bi-currency-dollar",
                            Url = "/Catalog/Pricing/List",
                            PermissionPolicy = PermissionList.PricingPermissions.View,
                        },
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
                            PermissionPolicy = PermissionList.VariantPermissions.View
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
                            PermissionPolicy = PermissionList.BrandPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Categories",
                            TextAr = "الأصناف",
                            Icon = "bi-diagram-3-fill",
                            Url = "/Warehouse/Product/Category/List",
                            PermissionPolicy = PermissionList.CategoryPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Units",
                            TextAr = "الوحدات",
                            Icon = "bi-rulers",
                            Url = "/Warehouse/Product/Unit/List",
                            PermissionPolicy = PermissionList.UnitPermissions.View
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Inventory Management",
                    TextAr = "إدارة المخزون",
                    Icon = "bi-boxes",
                    PermissionPolicy = PermissionList.InventoryPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة المخزون",
                            Icon = "bi-speedometer2",
                            Url = "/Inventory/Dashboard",
                            PermissionPolicy = PermissionList.InventoryPermissions.Select
                        },
                        new MenuItem
                        {
                            TextEn = "Warehouses",
                            TextAr = "المستودعات",
                            Icon = "bi-building",
                            Url = "/Inventory/Warehouse/List",
                            PermissionPolicy = PermissionList.WarehousePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Current Stock",
                            TextAr = "المخزون",
                            Icon = "bi-boxes",
                            Url = "/Inventories/List",
                            PermissionPolicy = PermissionList.InventoryPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Batches",
                            TextAr = "الدفعات",
                            Icon = "bi-upc-scan",
                            Url = "/Inventory/Batch/List",
                            PermissionPolicy = PermissionList.BatchPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Stock Operations",
                            TextAr = "عمليات المخزون",
                            Icon = "bi-arrow-left-right",
                            PermissionPolicy = PermissionList.InventoryPermissions.View,

                            Children = new()
                            {
                                new MenuItem
                                {
                                    TextEn = "Stock In",
                                    TextAr = "مخزون وارد",
                                    Icon = "bi-box-arrow-in-down",
                                    Url = "/Inventory/Operations/StockIn",
                                    PermissionPolicy = PermissionList.InventoryPermissions.View,
                                },
                                new MenuItem
                                {
                                    TextEn = "Stock Out",
                                    TextAr = "مخزون صادر",
                                    Icon = "bi-box-arrow-up",
                                    Url = "/Inventory/Operations/StockOut",
                                    PermissionPolicy = PermissionList.InventoryPermissions.View,
                                },
                                new MenuItem
                                {
                                    TextEn = "Adjustments",
                                    TextAr = "تسويات",
                                    Icon = "bi-sliders",
                                    Url = "/Inventory/Operations/StockAdjustment",
                                    PermissionPolicy = PermissionList.InventoryPermissions.View,
                                },
                                new MenuItem
                                {
                                    TextEn = "Reserve",
                                    TextAr = "حجز مخزون",
                                    Icon = "bi-lock",
                                    Url = "/Inventory/Operations/StockReservation",
                                    PermissionPolicy = PermissionList.InventoryPermissions.View,
                                },
                                new MenuItem
                                {
                                    TextEn = "Release",
                                    TextAr = "إطلاق مخزون",
                                    Icon = "bi-unlock",
                                    Url = "/Inventory/Operations/StockRelease",
                                    PermissionPolicy = PermissionList.InventoryPermissions.View,
                                },
                            }
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Supplier Management",
                    TextAr = "إدارة الموردين",
                    Icon = "bi-truck",
                    PermissionPolicy = PermissionList.SupplierPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Suppliers",
                            TextAr = "الموردون",
                            Icon = "bi-truck",
                            Url = "/Suppliers/Supplier/List",
                            PermissionPolicy = PermissionList.SupplierPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Supplier Groups",
                            TextAr = "مجموعات الموردين",
                            Icon = "bi-collection",
                            Url = "/Suppliers/SupplierGroup/List",
                            PermissionPolicy = PermissionList.SupplierGroupPermissions.View,
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Procurement",
                    TextAr = "المشتريات",
                    Icon = "bi-cart-check",
                    PermissionPolicy = PermissionList.PurchaseOrderPermissions.Select,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة المشتريات",
                            Icon = "bi-speedometer2",
                            Url = "/Procurement/Dashboard",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.Select,
                        },
                        new MenuItem
                        {
                            TextEn = "Purchase Requests",
                            TextAr = "طلبات الشراء",
                            Icon = "bi-card-checklist",
                            Url = "/Procurement/purchase-requests",
                            PermissionPolicy = PermissionList.PurchaseRequestPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "RFQs",
                            TextAr = "طلبات عروض الأسعار",
                            Icon = "bi-envelope-paper",
                            Url = "/Procurement/requests-for-quotation",
                            PermissionPolicy = PermissionList.RequestForQuotationPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Supplier Quotations",
                            TextAr = "عروض أسعار الموردين",
                            Icon = "bi-file-earmark-text",
                            Url = "/Procurement/supplier-quotations",
                            PermissionPolicy = PermissionList.SupplierQuotationPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Purchase Orders",
                            TextAr = "أوامر الشراء",
                            Icon = "bi-bag-check",
                            Url = "/Procurement/purchase-orders",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Goods Receipts",
                            TextAr = "استلام البضائع",
                            Icon = "bi-box-arrow-in-down",
                            Url = "/Procurement/goods-receipts",
                            PermissionPolicy = PermissionList.GoodsReceiptPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Purchase Returns",
                            TextAr = "مرتجعات الشراء",
                            Icon = "bi-arrow-return-left",
                            Url = "/Procurement/purchase-returns",
                            PermissionPolicy = PermissionList.PurchaseReturnPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Supplier Invoices",
                            TextAr = "فواتير الموردين",
                            Icon = "bi-receipt",
                            Url = "/Procurement/supplier-invoices",
                            PermissionPolicy = PermissionList.SupplierInvoicePermissions.View,
                        }
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

        //Task Management
        new MenuItem
        {
            TextEn = "Task Management",
            TextAr = "إدارة المهام",
            Icon = "bi-kanban-fill",
            PermissionPolicy = PermissionList.TaskManagementPermissions.Select,
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة المهام",
                    Icon = "bi-speedometer2",
                    Url = "/TaskManagement/Dashboard",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "My Tasks",
                    TextAr = "مهامي",
                    Icon = "bi-person-check",
                    Url = "/TaskManagement/MyTasks",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Notifications",
                    TextAr = "التنبيهات",
                    Icon = "bi-bell",
                    Url = "/TaskManagement/Notifications",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Task List",
                    TextAr = "قائمة المهام",
                    Icon = "bi-list-task",
                    Url = "/TaskManagement/List",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Kanban Board",
                    TextAr = "لوحة كانبان",
                    Icon = "bi-kanban",
                    Url = "/TaskManagement/Kanban",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Reports",
                    TextAr = "التقارير",
                    Icon = "bi-bar-chart-line",
                    Url = "/TaskManagement/Reports",
                    PermissionPolicy = PermissionList.TaskManagementPermissions.ViewReports,
                }
            }
        },

        //Security Management
        new MenuItem
        {
            TextEn = "Security Management",
            TextAr = "إدارة الأمان",
            Icon = "bi-shield-lock-fill",
            PermissionPolicy = PermissionList.UsersPermissions.Select,
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة الأمان",
                    Icon = "bi-speedometer2",
                    Url = "/Auth/Dashboard",
                    PermissionPolicy = PermissionList.UsersPermissions.Select
                },
                new MenuItem
                {
                    TextEn = "Roles Management",
                    TextAr = "إدارة الصلاحيات",
                    Icon = "bi-shield-check",
                    Url = "/Auth/Role/List",
                    PermissionPolicy = PermissionList.RolesPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Assign User Roles",
                    TextAr = "تعيين صلاحيات المستخدمين",
                    Icon = "bi-person-gear",
                    Url = "/Auth/User/AssignRole",
                    PermissionPolicy = PermissionList.UsersPermissions.View
                },
                new MenuItem
                {
                    TextEn = "System Settings",
                    TextAr = "إعدادات النظام",
                    Icon = "bi-gear-wide-connected",
                    Url = "/GeneralSettings/SystemSettings",
                    PermissionPolicy = PermissionList.SystemSettingsPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Currencies",
                    TextAr = "العملات",
                    Icon = "bi-currency-exchange",
                    Url = "/GeneralSettings/Currencies",
                    PermissionPolicy = PermissionList.SystemSettingsPermissions.View
                }
            }
        }
    };
}
