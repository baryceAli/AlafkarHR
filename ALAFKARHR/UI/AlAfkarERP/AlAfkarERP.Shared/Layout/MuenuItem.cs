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
        new MenuItem
        {
            TextEn = "Sales Management",
            TextAr = "إدارة المبيعات",
            Icon = "bi-graph-up-arrow",
            PermissionPolicy = PermissionList.SalesOrderPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceSales,
            KeywordsEn = "sales orders order intakes dashboard pos",
            KeywordsAr = "مبيعات أوامر طلبات لوحة نقطة بيع",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "POS",
                    TextAr = "نقطة بيع",
                    Icon = "bi-receipt",
                    Url = "/SalesOrder/POS",
                    PermissionPolicy = PermissionList.SalesOrderPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspacePos,
                    MobilePriority = 4,
                    KeywordsEn = "pos sales checkout cashier",
                    KeywordsAr = "نقطة بيع مبيعات كاشير"
                },
                new MenuItem
                {
                    TextEn = "Sales Dashboard",
                    TextAr = "لوحة المبيعات",
                    Icon = "bi-speedometer2",
                    Url = "/Sales/Dashboard",
                    PermissionPolicy = PermissionList.SalesOrderPermissions.ViewReports
                },
                new MenuItem
                {
                    TextEn = "Order Intakes",
                    TextAr = "طلبات البيع",
                    Icon = "bi-card-checklist",
                    Url = "/Orders/Intakes",
                    PermissionPolicy = PermissionList.OrderIntakePermissions.View
                },
                new MenuItem
                {
                    TextEn = "Sales Orders",
                    TextAr = "أوامر البيع",
                    Icon = "bi-list-check",
                    Url = "/Sales/Orders",
                    PermissionPolicy = PermissionList.SalesOrderPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Customers Management",
                    TextAr = "إدارة العملاء",
                    Icon = "bi-person-vcard-fill",
                    PermissionPolicy = PermissionList.CustomerPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceSales,
                    KeywordsEn = "sales customers customer groups customer pricing",
                    KeywordsAr = "مبيعات عملاء مجموعات العملاء تسعير العملاء",
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
        //Control Panel
        new MenuItem
        {
            TextEn = "Control Panel",
            TextAr = "لوحة التحكم",
            Icon = "bi-speedometer2",
            Url = "/Dashboard",
            PermissionPolicy = "View.Dashboard",
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            MobilePriority = 1,
            KeywordsEn = "control panel admin dashboard overview",
            KeywordsAr = "لوحة التحكم الإدارة نظرة عامة"
        },
        new MenuItem
        {
            TextEn = "Contracts",
            TextAr = "العقود",
            Icon = "bi-file-earmark-check",
            PermissionPolicy = PermissionList.ContractPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin contracts agreements renewals templates fees",
            KeywordsAr = "إدارة عقود اتفاقيات تجديدات قوالب رسوم",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة العقود",
                    Icon = "bi-speedometer2",
                    Url = "/Contracts/Dashboard",
                    PermissionPolicy = PermissionList.ContractPermissions.Select
                },
                new MenuItem
                {
                    TextEn = "Contracts List",
                    TextAr = "قائمة العقود",
                    Icon = "bi-list-check",
                    Url = "/Contracts/List",
                    PermissionPolicy = PermissionList.ContractPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Renewals",
                    TextAr = "تجديدات العقود",
                    Icon = "bi-arrow-repeat",
                    Url = "/Contracts/Renewals",
                    PermissionPolicy = PermissionList.ContractRenewalPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Templates",
                    TextAr = "القوالب",
                    Icon = "bi-file-earmark-text",
                    Url = "/Contracts/Templates",
                    PermissionPolicy = PermissionList.ContractTemplatePermissions.View
                }
            }
        },
        
        //Organizational Structure
        new MenuItem
        {
            TextEn = "Organizational Structure",
            TextAr = "الهيكل التنظيمي",
            Icon = "bi-diagram-3",
            PermissionPolicy = $"{PermissionList.CompanyPermissions.Select}",
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin organization company branch department structure",
            KeywordsAr = "إدارة هيكل تنظيمي شركة فروع أقسام",
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
                    TextEn = "Tenant Companies",
                    TextAr = "الشركات الرئيسية",
                    Icon = "bi-building-lock",
                    Url = "/Organization/ParentCompanies",
                    PermissionPolicy = PermissionList.ParentCompanyPermissions.View
                },
                new MenuItem
                {
                    TextEn = "License Categories",
                    TextAr = "فئات الترخيص",
                    Icon = "bi-patch-check",
                    Url = "/Organization/LicenseCategories",
                    PermissionPolicy = PermissionList.ParentCompanyPermissions.View
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
            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
            KeywordsEn = "hr people employees attendance leave payroll customers",
            KeywordsAr = "الموارد الأفراد الموظفين الحضور الإجازات الرواتب العملاء",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Human Resource",
                    TextAr = "الموارد البشرية",
                    Icon = "bi-person-workspace",
                    PermissionPolicy = PermissionList.EmployeePermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                    KeywordsEn = "hr employees positions specializations",
                    KeywordsAr = "موارد بشرية موظفين مسميات تخصصات",
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
                            TextEn = "Teams",
                            TextAr = "الفرق",
                            Icon = "bi-people",
                            Url = "/Employee/Teams",
                            PermissionPolicy = PermissionList.TeamPermissions.View
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
                    WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                    KeywordsEn = "attendance shifts sessions late holidays permissions reports",
                    KeywordsAr = "حضور انصراف ورديات جلسات تأخير عطلات أذونات تقارير",
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
                    PermissionPolicy = PermissionList.LeavePermissions.RequestEmergencyLeave,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                    KeywordsEn = "leave emergency balances reports approvals",
                    KeywordsAr = "إجازات طارئة أرصدة تقارير اعتماد",
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Emergency Leaves",
                            TextAr = "الإجازات الطارئة",
                            Icon = "bi-life-preserver",
                            Url = "/LeavesManagement/EmergencyLeaves",
                            PermissionPolicy = PermissionList.LeavePermissions.RequestEmergencyLeave
                        },
                        new MenuItem
                        {
                            TextEn = "Approve Emergency Leave",
                            TextAr = "اعتماد الإجازة الطارئة",
                            Icon = "bi-patch-check",
                            Url = "/LeavesManagement/ApproveEmergencyLeaves",
                            PermissionPolicy = PermissionList.LeavePermissions.ApproveEmergencyLeave
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Balances",
                            TextAr = "أرصدة الإجازات",
                            Icon = "bi-sliders",
                            Url = "/LeavesManagement/Balances",
                            PermissionPolicy = PermissionList.LeavePermissions.ViewLeaveBalances
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Reports",
                            TextAr = "تقارير الإجازات",
                            Icon = "bi-file-earmark-bar-graph",
                            Url = "/LeavesManagement/Reports",
                            PermissionPolicy = PermissionList.LeavePermissions.ViewLeaveReports
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Payroll",
                    TextAr = "الرواتب",
                    Icon = "bi-cash-stack",
                    PermissionPolicy = PermissionList.SalaryRunPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                    KeywordsEn = "hr payroll salaries contracts loans deductions",
                    KeywordsAr = "الموارد رواتب عقود سلف خصومات",
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
                }
            }
        },

        //Operations
        new MenuItem
        {
            TextEn = "Operations",
            TextAr = "العمليات",
            Icon = "bi-box-seam",
            WorkspaceKey = NavigationMenuResolver.WorkspaceWarehouse,
            KeywordsEn = "warehouse purchasing products inventory procurement suppliers",
            KeywordsAr = "مستودع مشتريات منتجات مخزون موردين",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Products Management",
                    TextAr = "إدارة المنتجات",
                    Icon = "bi-tags-fill",
                    PermissionPolicy = PermissionList.ProductPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceWarehouse,
                    KeywordsEn = "warehouse products sku variants packages brand categories units",
                    KeywordsAr = "مستودع منتجات أصناف خيارات عبوات علامات وحدات",
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
                    WorkspaceKey = NavigationMenuResolver.WorkspaceWarehouse,
                    KeywordsEn = "warehouse inventory stock batches operations reserve release",
                    KeywordsAr = "مستودع مخزون دفعات عمليات حجز إطلاق",
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
                            TextEn = "Transfers",
                            TextAr = "التحويلات",
                            Icon = "bi-truck",
                            Url = "/Inventory/Transfers",
                            PermissionPolicy = PermissionList.WarehouseTransferPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Movements",
                            TextAr = "حركات المخزون",
                            Icon = "bi-clock-history",
                            Url = "/Inventory/Movements",
                            PermissionPolicy = PermissionList.StockTransactionPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Asset Instances",
                            TextAr = "الأصول المخزنية",
                            Icon = "bi-hdd-rack",
                            Url = "/Inventory/AssetInstances",
                            PermissionPolicy = PermissionList.InventoryItemPermissions.View,
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
                    WorkspaceKey = NavigationMenuResolver.WorkspacePurchasing,
                    KeywordsEn = "purchasing suppliers supplier groups",
                    KeywordsAr = "مشتريات موردين مجموعات الموردين",
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
                    WorkspaceKey = NavigationMenuResolver.WorkspacePurchasing,
                    KeywordsEn = "purchasing procurement purchase requests rfq quotations orders receipts invoices",
                    KeywordsAr = "مشتريات طلبات شراء عروض أسعار أوامر استلام فواتير",
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

        //Document Management
        new MenuItem
        {
            TextEn = "Document Management",
            TextAr = "إدارة المستندات",
            Icon = "bi-files",
            PermissionPolicy = PermissionList.DocumentManagementPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin documents attachments files versions collaboration",
            KeywordsAr = "إدارة مستندات مرفقات ملفات إصدارات تعاون",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Document Library",
                    TextAr = "مكتبة المستندات",
                    Icon = "bi-folder2-open",
                    Url = "/DocumentManagement/List",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "My Documents",
                    TextAr = "مستنداتي",
                    Icon = "bi-person-lines-fill",
                    Url = "/DocumentManagement/MyDocuments",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Shared With Me",
                    TextAr = "مشاركة معي",
                    Icon = "bi-people",
                    Url = "/DocumentManagement/SharedWithMe",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "New Document",
                    TextAr = "مستند جديد",
                    Icon = "bi-cloud-upload",
                    Url = "/DocumentManagement/Create",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.Create,
                },
                new MenuItem
                {
                    TextEn = "Source Documents",
                    TextAr = "مستندات المصدر",
                    Icon = "bi-link-45deg",
                    Url = "/DocumentManagement/SourceDocuments",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.View,
                },
                new MenuItem
                {
                    TextEn = "Upload Rules",
                    TextAr = "قواعد الرفع",
                    Icon = "bi-shield-check",
                    Url = "/DocumentManagement/UploadPolicy",
                    PermissionPolicy = PermissionList.DocumentManagementPermissions.Configure,
                }
            }
        },

        //Media Center
        new MenuItem
        {
            TextEn = "Media Center",
            TextAr = "المركز الإعلامي",
            Icon = "bi-images",
            PermissionPolicy = PermissionList.MediaCenterPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin media library activities photos videos audio documents gallery",
            KeywordsAr = "إدارة مركز إعلامي مكتبة أنشطة صور فيديو صوت مستندات معرض",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Library",
                    TextAr = "المكتبة",
                    Icon = "bi-images",
                    Url = "/MediaCenter/Activities",
                    PermissionPolicy = PermissionList.MediaCenterPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Activity Types",
                    TextAr = "أنواع الأنشطة",
                    Icon = "bi-tags",
                    Url = "/MediaCenter/ActivityTypes",
                    PermissionPolicy = PermissionList.MediaCenterPermissions.ManageTypes,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                }
            }
        },

        //Project Management
        new MenuItem
        {
            TextEn = "Project Management",
            TextAr = "إدارة المشاريع",
            Icon = "bi-diagram-3",
            PermissionPolicy = PermissionList.ProjectManagementPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "projects meals distribution customers locations budget resources",
            KeywordsAr = "مشاريع وجبات توزيع عملاء مواقع ميزانية موارد",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة المشاريع",
                    Icon = "bi-speedometer2",
                    Url = "/ProjectManagement/Dashboard",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Projects",
                    TextAr = "المشاريع",
                    Icon = "bi-kanban",
                    Url = "/ProjectManagement/Projects",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Distribution Places",
                    TextAr = "مواقع التوزيع",
                    Icon = "bi-geo-alt",
                    Url = "/ProjectManagement/DistributionPlaces",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.Distribution,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Distribution Schedule",
                    TextAr = "جدول التوزيع",
                    Icon = "bi-calendar-event",
                    Url = "/ProjectManagement/DistributionSchedule",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.Distribution,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Customer Distribution Report",
                    TextAr = "تقرير توزيع العملاء",
                    Icon = "bi-bar-chart",
                    Url = "/ProjectManagement/Reports/CustomerDistribution",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.ViewReports,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Place Distribution Report",
                    TextAr = "تقرير توزيع المواقع",
                    Icon = "bi-geo-alt",
                    Url = "/ProjectManagement/Reports/PlaceDistribution",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.ViewReports,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Daily Distribution Report",
                    TextAr = "تقرير التوزيع اليومي",
                    Icon = "bi-calendar-day",
                    Url = "/ProjectManagement/Reports/DailyDistribution",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.ViewReports,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Planned Product Demand",
                    TextAr = "الطلب المخطط للمنتجات",
                    Icon = "bi-graph-up-arrow",
                    Url = "/ProjectManagement/Reports/PlannedProductDemand",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.ViewReports,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                },
                new MenuItem
                {
                    TextEn = "Project Cost Report",
                    TextAr = "تقرير تكلفة المشاريع",
                    Icon = "bi-cash-stack",
                    Url = "/ProjectManagement/Reports/Costs",
                    PermissionPolicy = PermissionList.ProjectManagementPermissions.ViewReports,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                }
            }
        },

        //Task Management
        new MenuItem
        {
            TextEn = "Task Management",
            TextAr = "إدارة المهام",
            Icon = "bi-kanban-fill",
            PermissionPolicy = PermissionList.TaskManagementPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin tasks notifications kanban reports",
            KeywordsAr = "إدارة مهام تنبيهات كانبان تقارير",
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

        //Platform Operations
        new MenuItem
        {
            TextEn = "Platform Operations",
            TextAr = "عمليات المنصة",
            Icon = "bi-building-gear",
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin platform operations fleet vehicles real estate properties maintenance assets repairs",
            KeywordsAr = "إدارة عمليات المنصة أسطول مركبات عقارات صيانة أصول إصلاحات",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Fleet",
                    TextAr = "الأسطول",
                    Icon = "bi-truck-front",
                    PermissionPolicy = PermissionList.FleetVehiclePermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                    KeywordsEn = "admin fleet vehicles cars rented owned assignments expenses fuel oil documents renewals maintenance",
                    KeywordsAr = "إدارة أسطول مركبات سيارات مؤجرة مملوكة عهد مصروفات وقود زيت مستندات تجديد صيانة",
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة الأسطول",
                            Icon = "bi-speedometer2",
                            Url = "/Fleet/Dashboard",
                            PermissionPolicy = PermissionList.FleetReportsPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Vehicles",
                            TextAr = "المركبات",
                            Icon = "bi-truck-front",
                            Url = "/Fleet/Vehicles",
                            PermissionPolicy = PermissionList.FleetVehiclePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Assignments",
                            TextAr = "العهد",
                            Icon = "bi-person-vcard",
                            Url = "/Fleet/Assignments",
                            PermissionPolicy = PermissionList.FleetVehicleAssignmentPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Expenses",
                            TextAr = "المصروفات",
                            Icon = "bi-receipt",
                            Url = "/Fleet/Expenses",
                            PermissionPolicy = PermissionList.FleetVehicleExpensePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Documents",
                            TextAr = "المستندات والتجديدات",
                            Icon = "bi-file-earmark-text",
                            Url = "/Fleet/Documents",
                            PermissionPolicy = PermissionList.FleetVehicleDocumentPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Service Rules",
                            TextAr = "قواعد الصيانة",
                            Icon = "bi-wrench-adjustable",
                            Url = "/Fleet/ServiceRules",
                            PermissionPolicy = PermissionList.FleetVehiclePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "التقارير",
                            Icon = "bi-bar-chart-line",
                            Url = "/Fleet/Reports",
                            PermissionPolicy = PermissionList.FleetReportsPermissions.View,
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Real Estate",
                    TextAr = "Real Estate",
                    Icon = "bi-buildings",
                    PermissionPolicy = PermissionList.RealEstatePropertyPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                    KeywordsEn = "admin real estate properties units leases rent utilities expenses",
                    KeywordsAr = "real estate properties units leases rent utilities expenses",
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "Dashboard",
                            Icon = "bi-speedometer2",
                            Url = "/RealEstate/Dashboard",
                            PermissionPolicy = PermissionList.RealEstateReportsPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Properties",
                            TextAr = "Properties",
                            Icon = "bi-buildings",
                            Url = "/RealEstate/Properties",
                            PermissionPolicy = PermissionList.RealEstatePropertyPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Units",
                            TextAr = "Units",
                            Icon = "bi-door-open",
                            Url = "/RealEstate/Units",
                            PermissionPolicy = PermissionList.RealEstateUnitPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Owner Leases",
                            TextAr = "Owner Leases",
                            Icon = "bi-file-earmark-minus",
                            Url = "/RealEstate/OwnerLeases",
                            PermissionPolicy = PermissionList.RealEstateLeasePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Tenant Leases",
                            TextAr = "Tenant Leases",
                            Icon = "bi-file-earmark-check",
                            Url = "/RealEstate/TenantLeases",
                            PermissionPolicy = PermissionList.RealEstateLeasePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Rent Collections",
                            TextAr = "Rent Collections",
                            Icon = "bi-cash-coin",
                            Url = "/RealEstate/Collections",
                            PermissionPolicy = PermissionList.RealEstateInstallmentPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Utilities",
                            TextAr = "Utilities",
                            Icon = "bi-lightning-charge",
                            Url = "/RealEstate/Utilities",
                            PermissionPolicy = PermissionList.RealEstateUtilityPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Expenses",
                            TextAr = "Expenses",
                            Icon = "bi-receipt",
                            Url = "/RealEstate/Expenses",
                            PermissionPolicy = PermissionList.RealEstateExpensePermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "Reports",
                            Icon = "bi-bar-chart-line",
                            Url = "/RealEstate/Reports",
                            PermissionPolicy = PermissionList.RealEstateReportsPermissions.View,
                        }
                    }
                },
                new MenuItem
                {
                    TextEn = "Maintenance",
                    TextAr = "الصيانة",
                    Icon = "bi-tools",
                    PermissionPolicy = PermissionList.MaintenanceWorkOrderPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                    KeywordsEn = "admin maintenance assets buildings apartments offices vehicles work orders repairs",
                    KeywordsAr = "إدارة صيانة أصول مباني شقق مكاتب مركبات أوامر عمل إصلاحات",
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة الصيانة",
                            Icon = "bi-speedometer2",
                            Url = "/Maintenance/Dashboard",
                            PermissionPolicy = PermissionList.MaintenanceWorkOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Assets",
                            TextAr = "الأصول",
                            Icon = "bi-buildings",
                            Url = "/Maintenance/Assets",
                            PermissionPolicy = PermissionList.MaintenanceAssetPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Work Orders",
                            TextAr = "أوامر العمل",
                            Icon = "bi-clipboard2-check",
                            Url = "/Maintenance/WorkOrders",
                            PermissionPolicy = PermissionList.MaintenanceWorkOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "My Requests",
                            TextAr = "طلباتي",
                            Icon = "bi-person-check",
                            Url = "/Maintenance/WorkOrders/MyRequests",
                            PermissionPolicy = PermissionList.MaintenanceWorkOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "التقارير",
                            Icon = "bi-bar-chart-line",
                            Url = "/Maintenance/Reports",
                            PermissionPolicy = PermissionList.MaintenanceWorkOrderPermissions.ViewReports,
                        }
                    }
                }
            }
        },

        //General Settings
        new MenuItem
        {
            TextEn = "General Settings",
            TextAr = "الإعدادات العامة",
            Icon = "bi-gear-wide-connected",
            PermissionPolicy = PermissionList.SystemSettingsPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            KeywordsEn = "admin general settings system settings currencies configuration",
            KeywordsAr = "إدارة إعدادات عامة إعدادات النظام عملات تهيئة",
            Children = new()
            {
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
                    TextEn = "Home Page Templates",
                    TextAr = "قوالب الصفحة الرئيسية",
                    Icon = "bi-window",
                    Url = "/GeneralSettings/HomePageTemplates",
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
        },

        //Security Management
        new MenuItem
        {
            TextEn = "Security Management",
            TextAr = "إدارة الأمان",
            Icon = "bi-shield-lock-fill",
            PermissionPolicy = PermissionList.UsersPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceSecurity,
            KeywordsEn = "it security roles users permissions",
            KeywordsAr = "تقنية أمن أدوار مستخدمين صلاحيات",
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
                }
            }
        }
    };
}
