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
    public string? BusinessLineKey { get; set; }
    public string? NavigationFunctionalGroupKey { get; set; }
    public string? NavigationGroupKey { get; set; }
    public int? NavigationOrder { get; set; }
    public string? ProcessKey { get; set; }
    public List<string> NavigationAliases { get; set; } = new();
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
            TextEn = "StoreFront",
            TextAr = "واجهات المتاجر",
            Icon = "bi-shop",
            PermissionPolicy = null,
            WorkspaceKey = NavigationMenuResolver.WorkspaceStoreFront,
            IsFavoriteCandidate = false,
            KeywordsEn = "storefront stores shops pos checkout cashier",
            KeywordsAr = "واجهات المتاجر متاجر نقطة بيع كاشير",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "POS",
                    TextAr = "نقطة بيع",
                    Icon = "bi-receipt",
                    Url = "/StoreFront/POS",
                    PermissionPolicy = null,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceStoreFront,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.StoreFrontFunctionalGroupCheckout,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                    NavigationOrder = 200,
                    MobilePriority = 4,
                    KeywordsEn = "storefront pos store front checkout cashier",
                    KeywordsAr = "واجهة متجر متجر نقطة بيع مبيعات كاشير"
                },
                new MenuItem
                {
                    TextEn = "Store Fronts",
                    TextAr = "واجهات المتاجر",
                    Icon = "bi-shop",
                    Url = "/StoreFront/Stores",
                    PermissionPolicy = PermissionList.StoreFrontStorePermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceStoreFront,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.StoreFrontFunctionalGroupStores,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                    NavigationOrder = 100,
                    NavigationAliases = new()
                    {
                        "/StoreFront/Stores/"
                    },
                    KeywordsEn = "store front shops grocery flowers car wash pos manage stores",
                    KeywordsAr = "متاجر واجهة متجر بقالة زهور غسيل سيارات نقطة بيع إدارة"
                },
                new MenuItem
                {
                    TextEn = "Operational Departments",
                    TextAr = "أقسام المتجر",
                    Icon = "bi-diagram-3",
                    Url = "/StoreFront/Departments",
                    PermissionPolicy = PermissionList.StoreFrontDepartmentPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceStoreFront,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.StoreFrontFunctionalGroupStores,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                    NavigationOrder = 110,
                    KeywordsEn = "storefront departments store departments operational teams",
                    KeywordsAr = "أقسام المتجر واجهة متجر فرق تشغيل"
                },
                new MenuItem
                {
                    TextEn = "Organization Structure",
                    TextAr = "الهيكل التنظيمي",
                    Icon = "bi-building-gear",
                    Url = "/StoreFront/Organization",
                    PermissionPolicy = PermissionList.StoreFrontStorePermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceStoreFront,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.StoreFrontFunctionalGroupStores,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                    NavigationOrder = 115,
                    KeywordsEn = "storefront organization structure administrations departments branch setup",
                    KeywordsAr = "هيكل تنظيمي واجهة متجر إدارات أقسام فرع إعداد"
                }
            }
        },
        new MenuItem
        {
            TextEn = "Sales Management",
            TextAr = "إدارة المبيعات",
            Icon = "bi-graph-up-arrow",
            PermissionPolicy = PermissionList.SalesOrderPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceSales,
            NavigationFunctionalGroupKey = NavigationMenuResolver.SalesFunctionalGroupSales,
            KeywordsEn = "sales orders order intakes dashboard pos",
            KeywordsAr = "مبيعات أوامر طلبات لوحة نقطة بيع",
            Children = new()
            {
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
                    TextEn = "Quotations",
                    TextAr = "عروض أسعار المبيعات",
                    Icon = "bi-file-earmark-text",
                    Url = "/Sales/Quotations",
                    PermissionPolicy = PermissionList.SalesQuotationPermissions.View
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
                    TextEn = "Delivery Notes",
                    TextAr = "إشعارات التسليم",
                    Icon = "bi-truck",
                    Url = "/Sales/DeliveryNotes",
                    PermissionPolicy = PermissionList.SalesDeliveryNotePermissions.View
                },
                new MenuItem
                {
                    TextEn = "Returns / Credit Notes",
                    TextAr = "المرتجعات / الإشعارات الدائنة",
                    Icon = "bi-arrow-counterclockwise",
                    Url = "/Sales/Returns",
                    PermissionPolicy = PermissionList.SalesReturnPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Sales Reports",
                    TextAr = "تقارير المبيعات",
                    Icon = "bi-bar-chart-line",
                    Url = "/Sales/Dashboard",
                    PermissionPolicy = PermissionList.SalesReportPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.SalesFunctionalGroupReports
                },
                new MenuItem
                {
                    TextEn = "Customers Management",
                    TextAr = "إدارة العملاء",
                    Icon = "bi-person-vcard-fill",
                    PermissionPolicy = PermissionList.CustomerPermissions.Select,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceSales,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.SalesFunctionalGroupCustomers,
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
        new MenuItem
        {
            TextEn = "Accounting",
            TextAr = "المحاسبة",
            Icon = "bi-calculator",
            PermissionPolicy = PermissionList.AccountingDashboardPermissions.View,
            WorkspaceKey = NavigationMenuResolver.WorkspaceAccountingFinance,
            NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupSetup,
            NavigationAliases = new()
            {
                "/Sales",
                "/SalesOrder",
                "/Orders",
                "/Customers",
                "/Procurement",
                "/Suppliers"
            },
            KeywordsEn = "finance accounting ledger journals vat zatca invoices receipts payments",
            KeywordsAr = "مالية محاسبة دفتر قيود ضريبة زاتكا فواتير مقبوضات مدفوعات",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة المحاسبة",
                    Icon = "bi-speedometer2",
                    Url = "/Accounting/Dashboard",
                    PermissionPolicy = PermissionList.AccountingDashboardPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Accounting Setup",
                    TextAr = "إعداد المحاسبة",
                    Icon = "bi-magic",
                    Url = "/Accounting/Setup",
                    PermissionPolicy = PermissionList.AccountingDashboardPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Templates",
                    TextAr = "القوالب",
                    Icon = "bi-journal-richtext",
                    Url = "/Accounting/Templates",
                    PermissionPolicy = PermissionList.AccountingTemplatePermissions.View
                },
                new MenuItem
                {
                    TextEn = "Chart of Accounts",
                    TextAr = "دليل الحسابات",
                    Icon = "bi-diagram-3",
                    Url = "/Accounting/Accounts",
                    PermissionPolicy = PermissionList.AccountPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupChartOfAccounts
                },
                new MenuItem
                {
                    TextEn = "Fiscal Periods",
                    TextAr = "الفترات المالية",
                    Icon = "bi-calendar2-week",
                    Url = "/Accounting/FiscalPeriods",
                    PermissionPolicy = PermissionList.FiscalPeriodPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Tax Codes",
                    TextAr = "أكواد الضريبة",
                    Icon = "bi-percent",
                    Url = "/Accounting/TaxCodes",
                    PermissionPolicy = PermissionList.TaxCodePermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupTaxZatca
                },
                new MenuItem
                {
                    TextEn = "Posting Profiles",
                    TextAr = "توجيهات الترحيل",
                    Icon = "bi-diagram-2",
                    Url = "/Accounting/PostingProfiles",
                    PermissionPolicy = PermissionList.PostingProfilePermissions.View
                },
                new MenuItem
                {
                    TextEn = "Bank & Cash Accounts",
                    TextAr = "الحسابات البنكية والنقدية",
                    Icon = "bi-bank",
                    Url = "/Accounting/BankCashAccounts",
                    PermissionPolicy = PermissionList.BankAccountPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupBankingCash
                },
                new MenuItem
                {
                    TextEn = "Cash Accounts",
                    TextAr = "الحسابات النقدية",
                    Icon = "bi-wallet2",
                    Url = "/Accounting/BankCashAccounts",
                    PermissionPolicy = PermissionList.CashAccountPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupBankingCash
                },
                new MenuItem
                {
                    TextEn = "Company Defaults",
                    TextAr = "إعدادات الشركة المحاسبية",
                    Icon = "bi-sliders",
                    Url = "/Accounting/Setup",
                    PermissionPolicy = PermissionList.AccountingSettingsPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Journal Entries",
                    TextAr = "القيود اليومية",
                    Icon = "bi-journal-check",
                    Url = "/Accounting/Journals",
                    PermissionPolicy = PermissionList.JournalEntryPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupJournalsDocuments
                },
                new MenuItem
                {
                    TextEn = "All Documents",
                    TextAr = "كل المستندات",
                    Icon = "bi-files",
                    Url = "/Accounting/Documents",
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupJournalsDocuments
                },
                new MenuItem
                {
                    TextEn = "Sales Invoices",
                    TextAr = "فواتير المبيعات",
                    Icon = "bi-receipt",
                    Url = "/Accounting/SalesInvoices",
                    NavigationAliases = new()
                    {
                        "/Sales/Orders",
                        "/Orders/Intakes"
                    },
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupInvoices
                },
                new MenuItem
                {
                    TextEn = "Purchase Invoices",
                    TextAr = "فواتير المشتريات",
                    Icon = "bi-receipt-cutoff",
                    Url = "/Accounting/PurchaseInvoices",
                    NavigationAliases = new()
                    {
                        "/Procurement/supplier-invoices"
                    },
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupInvoices
                },
                new MenuItem
                {
                    TextEn = "Receipts & Payments",
                    TextAr = "المقبوضات والمدفوعات",
                    Icon = "bi-cash-stack",
                    Url = "/Accounting/ReceiptsPayments",
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupReceiptsPayments
                },
                new MenuItem
                {
                    TextEn = "Credit Notes",
                    TextAr = "الإشعارات الدائنة",
                    Icon = "bi-arrow-counterclockwise",
                    Url = "/Accounting/CreditNotes",
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupAdjustments
                },
                new MenuItem
                {
                    TextEn = "Debit Notes",
                    TextAr = "الإشعارات المدينة",
                    Icon = "bi-arrow-clockwise",
                    Url = "/Accounting/DebitNotes",
                    PermissionPolicy = PermissionList.AccountingDocumentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupAdjustments
                },
                new MenuItem
                {
                    TextEn = "Bank Reconciliation",
                    TextAr = "تسوية البنك",
                    Icon = "bi-shuffle",
                    Url = "/Accounting/BankReconciliation",
                    PermissionPolicy = PermissionList.BankReconciliationPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupBankingCash
                },
                new MenuItem
                {
                    TextEn = "Reports",
                    TextAr = "التقارير",
                    Icon = "bi-bar-chart-line",
                    Url = "/Accounting/Reports",
                    PermissionPolicy = PermissionList.AccountingReportPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupReports
                },
                new MenuItem
                {
                    TextEn = "ZATCA Submissions",
                    TextAr = "إرسالات زاتكا",
                    Icon = "bi-cloud-arrow-up",
                    Url = "/Accounting/Zatca/Submissions",
                    PermissionPolicy = PermissionList.ZatcaEInvoicePermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupTaxZatca
                },
                new MenuItem
                {
                    TextEn = "ZATCA Settings",
                    TextAr = "إعدادات زاتكا",
                    Icon = "bi-shield-check",
                    Url = "/Accounting/Zatca/Settings",
                    PermissionPolicy = PermissionList.ZatcaSettingsPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AccountingFunctionalGroupTaxZatca
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupOrganization,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupContracts,
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
        new MenuItem
        {
            TextEn = "Catering",
            TextAr = "خدمات الإعاشة",
            Icon = "bi-cup-hot",
            PermissionPolicy = PermissionList.CateringContractPermissions.Select,
            WorkspaceKey = NavigationMenuResolver.WorkspaceCatering,
            NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupContracts,
            BusinessLineKey = SharedWithUI.Organization.BusinessLineKeys.Catering,
            KeywordsEn = "catering ramadan meals charity distribution haram squares refrigerated vehicles",
            KeywordsAr = "إعاشة رمضان وجبات جمعية توزيع الحرم مربعات برادات",
            Children = new()
            {
                new MenuItem
                {
                    TextEn = "Dashboard",
                    TextAr = "لوحة الإعاشة",
                    Icon = "bi-speedometer2",
                    Url = "/Catering/Dashboard",
                    PermissionPolicy = PermissionList.CateringReportsPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Contracts",
                    TextAr = "عقود الإعاشة",
                    Icon = "bi-file-earmark-check",
                    Url = "/Catering/Contracts",
                    PermissionPolicy = PermissionList.CateringContractPermissions.View
                },
                new MenuItem
                {
                    TextEn = "Meals",
                    TextAr = "الوجبات",
                    Icon = "bi-box-seam",
                    Url = "/Catering/Meals",
                    PermissionPolicy = PermissionList.CateringMealPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupMeals
                },
                new MenuItem
                {
                    TextEn = "Locations",
                    TextAr = "مواقع التوزيع",
                    Icon = "bi-geo-alt",
                    Url = "/Catering/Locations",
                    PermissionPolicy = PermissionList.CateringLocationPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupLocations
                },
                new MenuItem
                {
                    TextEn = "Schedules",
                    TextAr = "الجداول اليومية",
                    Icon = "bi-calendar-week",
                    Url = "/Catering/Schedules",
                    PermissionPolicy = PermissionList.CateringSchedulePermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupSchedules
                },
                new MenuItem
                {
                    TextEn = "Projects",
                    TextAr = "المشاريع",
                    Icon = "bi-diagram-3",
                    Url = "/Catering/Projects",
                    PermissionPolicy = PermissionList.CateringPlanPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupSchedules
                },
                new MenuItem
                {
                    TextEn = "Packaging",
                    TextAr = "التجهيز والتغليف",
                    Icon = "bi-box2-heart",
                    Url = "/Catering/Packaging",
                    PermissionPolicy = PermissionList.CateringPackagingPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupDeliveries
                },
                new MenuItem
                {
                    TextEn = "Deliveries",
                    TextAr = "التشغيل والرحلات",
                    Icon = "bi-truck",
                    Url = "/Catering/Deliveries",
                    PermissionPolicy = PermissionList.CateringDeliveryPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupDeliveries
                },
                new MenuItem
                {
                    TextEn = "Assignments",
                    TextAr = "التكليفات",
                    Icon = "bi-people",
                    Url = "/Catering/Assignments",
                    PermissionPolicy = PermissionList.CateringAssignmentPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupAssignments
                },
                new MenuItem
                {
                    TextEn = "Reports",
                    TextAr = "تقارير الإعاشة",
                    Icon = "bi-bar-chart-line",
                    Url = "/Catering/Reports",
                    PermissionPolicy = PermissionList.CateringReportsPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.CateringFunctionalGroupReports
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupOrganization,
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
                    TextEn = "Business Lines",
                    TextAr = "خطوط الأعمال",
                    Icon = "bi-grid-3x3-gap",
                    Url = "/Organization/BusinessLines",
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupEmployees,
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
                            TextEn = "HR Command Center",
                            TextAr = "مركز قيادة الموارد البشرية",
                            Icon = "bi-grid-1x2",
                            Url = "/HR/CommandCenter",
                            PermissionPolicy = PermissionList.EmployeeLifecyclePermissions.Select
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
                        },
                        new MenuItem
                        {
                            TextEn = "Lifecycle",
                            TextAr = "دورة حياة الموظف",
                            Icon = "bi-signpost-split",
                            Url = "/HR/EmployeeLifecycle",
                            PermissionPolicy = PermissionList.EmployeeLifecyclePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Documents",
                            TextAr = "مستندات الموظفين",
                            Icon = "bi-folder2-open",
                            Url = "/HR/EmployeeDocuments",
                            PermissionPolicy = PermissionList.EmployeeDocumentPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Emergency Contacts",
                            TextAr = "جهات اتصال الطوارئ",
                            Icon = "bi-telephone-plus",
                            Url = "/HR/EmployeeEmergencyContacts",
                            PermissionPolicy = PermissionList.EmployeeLifecyclePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Skills & Certifications",
                            TextAr = "المهارات والشهادات",
                            Icon = "bi-stars",
                            Url = "/HR/EmployeeSkills",
                            PermissionPolicy = PermissionList.EmployeeSkillPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Recruitment",
                            TextAr = "التوظيف",
                            Icon = "bi-person-plus",
                            Url = "/HR/Recruitment",
                            PermissionPolicy = PermissionList.RecruitmentPermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupRecruitment
                        },
                        new MenuItem
                        {
                            TextEn = "Performance",
                            TextAr = "الأداء",
                            Icon = "bi-graph-up-arrow",
                            Url = "/HR/Performance",
                            PermissionPolicy = PermissionList.PerformancePermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupPerformance
                        },
                        new MenuItem
                        {
                            TextEn = "Training",
                            TextAr = "التدريب",
                            Icon = "bi-award",
                            Url = "/HR/Training",
                            PermissionPolicy = PermissionList.TrainingPermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupTraining
                        },
                        new MenuItem
                        {
                            TextEn = "HR Reports",
                            TextAr = "تقارير الموارد البشرية",
                            Icon = "bi-bar-chart-line",
                            Url = "/HR/Reports",
                            PermissionPolicy = PermissionList.EmployeePermissions.View
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupAttendance,
                    KeywordsEn = "attendance setup configuration shifts shift assignments roster schedules sessions work entries holidays permission requests approvals reports",
                    KeywordsAr = "حضور انصراف إعداد تهيئة ورديات تعيين الورديات جداول جلسات مدخلات عمل عطلات أذونات اعتماد تقارير",
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Dashboard",
                            TextAr = "لوحة الحضور",
                            Icon = "bi-speedometer2",
                            Url = "/Attendance/Dashboard",
                            PermissionPolicy = PermissionList.AttendancePermissions.View,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupStart,
                            NavigationOrder = 2,
                            KeywordsEn = "attendance dashboard overview",
                            KeywordsAr = "لوحة الحضور نظرة عامة"
                        },
                        new MenuItem
                        {
                            TextEn = "My Attendance",
                            TextAr = "حضوري",
                            Icon = "bi-person-check",
                            Url = "/Attendance/MyAttendance",
                            PermissionPolicy = PermissionList.AttendancePermissions.Create,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 300,
                            KeywordsEn = "my attendance check in check out clock in clock out",
                            KeywordsAr = "حضوري تسجيل حضور انصراف"
                        },
                        new MenuItem
                        {
                            TextEn = "Sessions",
                            TextAr = "جلسات الحضور",
                            Icon = "bi-clock-history",
                            Url = "/Attendance/Sessions",
                            PermissionPolicy = PermissionList.AttendancePermissions.View,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 301,
                            KeywordsEn = "attendance sessions daily records check in check out",
                            KeywordsAr = "جلسات الحضور سجلات يومية حضور انصراف"
                        },
                        new MenuItem
                        {
                            TextEn = "Shifts",
                            TextAr = "الورديات",
                            Icon = "bi-calendar-range",
                            Url = "/Attendance/Shifts",
                            PermissionPolicy = PermissionList.AttendancePermissions.Edit,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                            NavigationOrder = 110,
                            KeywordsEn = "attendance setup shifts work shifts schedule rules",
                            KeywordsAr = "إعداد الحضور الورديات ورديات العمل قواعد الجدولة"
                        },
                        new MenuItem
                        {
                            TextEn = "Shift Assignments",
                            TextAr = "تعيين الورديات",
                            Icon = "bi-calendar2-week",
                            Url = "/Attendance/ShiftAssignments",
                            PermissionPolicy = PermissionList.AttendancePermissions.Edit,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                            NavigationOrder = 111,
                            KeywordsEn = "attendance setup shift assignments employee shifts",
                            KeywordsAr = "إعداد الحضور تعيين الورديات ورديات الموظفين"
                        },
                        new MenuItem
                        {
                            TextEn = "Roster & Shift Schedules",
                            TextAr = "الجداول وجدولة الورديات",
                            Icon = "bi-calendar3",
                            Url = "/HR/AttendanceRoster",
                            PermissionPolicy = PermissionList.AttendanceRosterPermissions.View,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                            NavigationOrder = 112,
                            KeywordsEn = "attendance setup roster shift schedules employee schedules",
                            KeywordsAr = "إعداد الحضور الجداول جدولة الورديات جداول الموظفين"
                        },
                        new MenuItem
                        {
                            TextEn = "Attendance Work Entries",
                            TextAr = "مدخلات عمل الحضور",
                            Icon = "bi-journal-check",
                            Url = "/HR/WorkEntries",
                            PermissionPolicy = PermissionList.AttendanceWorkEntryPermissions.View,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 303,
                            KeywordsEn = "attendance work entries work hours payroll attendance entries",
                            KeywordsAr = "مدخلات عمل الحضور ساعات العمل مدخلات الرواتب"
                        },
                        new MenuItem
                        {
                            TextEn = "Late Requests",
                            TextAr = "طلبات التأخير",
                            Icon = "bi-exclamation-triangle",
                            Url = "/Attendance/LateRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.ReviewRequests,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupApprovals,
                            NavigationOrder = 400,
                            KeywordsEn = "attendance approvals late requests exceptions",
                            KeywordsAr = "اعتمادات الحضور طلبات التأخير استثناءات"
                        },
                        new MenuItem
                        {
                            TextEn = "Calendar & Holidays",
                            TextAr = "التقويم والعطلات",
                            Icon = "bi-calendar-event",
                            Url = "/Attendance/Holidays",
                            PermissionPolicy = PermissionList.AttendancePermissions.ManageHolidays,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                            NavigationOrder = 113,
                            KeywordsEn = "attendance setup calendar holidays weekends public holidays",
                            KeywordsAr = "إعداد الحضور التقويم العطلات نهاية الأسبوع الإجازات الرسمية"
                        },
                        new MenuItem
                        {
                            TextEn = "Permission Requests",
                            TextAr = "طلبات الاذن",
                            Icon = "bi-door-open",
                            Url = "/Attendance/PermissionRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.RequestMidDayPermission,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 304,
                            KeywordsEn = "attendance permission requests mid day permission daily requests",
                            KeywordsAr = "طلبات إذن الحضور أذونات منتصف اليوم طلبات يومية"
                        },
                        new MenuItem
                        {
                            TextEn = "Approve Permission Requests",
                            TextAr = "اعتماد طلبات الاذن",
                            Icon = "bi-person-check",
                            Url = "/Attendance/ApprovePermissionRequests",
                            PermissionPolicy = PermissionList.AttendancePermissions.ApproveMidDayPermission,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupApprovals,
                            NavigationOrder = 401,
                            KeywordsEn = "attendance approvals approve permission requests mid day permission",
                            KeywordsAr = "اعتمادات الحضور اعتماد طلبات الإذن أذونات منتصف اليوم"
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "التقارير",
                            Icon = "bi-file-earmark-bar-graph",
                            PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                            NavigationOrder = 601,
                            KeywordsEn = "attendance reports daily summary late early leave break absence holidays permissions",
                            KeywordsAr = "تقارير الحضور يومي ملخص تأخير انصراف مبكر استراحة غياب عطلات أذونات",
                            Children = new()
                            {
                                new MenuItem
                                {
                                    TextEn = "Reports Overview",
                                    TextAr = "نظرة عامة على التقارير",
                                    Icon = "bi-file-earmark-bar-graph",
                                    Url = "/Attendance/Reports",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 601,
                                    KeywordsEn = "attendance reports overview",
                                    KeywordsAr = "نظرة عامة تقارير الحضور"
                                },
                                new MenuItem
                                {
                                    TextEn = "Daily Attendance Report",
                                    TextAr = "تقرير الحضور اليومي",
                                    Icon = "bi-calendar-day",
                                    Url = "/Attendance/Reports/Attendance",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 602,
                                    KeywordsEn = "daily attendance report attendance records",
                                    KeywordsAr = "تقرير الحضور اليومي سجلات الحضور"
                                },
                                new MenuItem
                                {
                                    TextEn = "Employee Attendance Summary",
                                    TextAr = "ملخص حضور الموظف",
                                    Icon = "bi-person-lines-fill",
                                    Url = "/Attendance/Reports/AttendanceSummary",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 603,
                                    KeywordsEn = "employee attendance summary report",
                                    KeywordsAr = "ملخص حضور الموظف تقرير"
                                },
                                new MenuItem
                                {
                                    TextEn = "Late Arrival Report",
                                    TextAr = "تقرير التأخير",
                                    Icon = "bi-clock",
                                    Url = "/Attendance/Reports/LateArrival",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 604,
                                    KeywordsEn = "late arrival report attendance delay",
                                    KeywordsAr = "تقرير التأخير تأخير الحضور"
                                },
                                new MenuItem
                                {
                                    TextEn = "Early Leave Report",
                                    TextAr = "تقرير الانصراف المبكر",
                                    Icon = "bi-box-arrow-right",
                                    Url = "/Attendance/Reports/EarlyLeave",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 605,
                                    KeywordsEn = "early leave report attendance early checkout",
                                    KeywordsAr = "تقرير الانصراف المبكر خروج مبكر"
                                },
                                new MenuItem
                                {
                                    TextEn = "Break Report",
                                    TextAr = "تقرير الاستراحة",
                                    Icon = "bi-cup-hot",
                                    Url = "/Attendance/Reports/Break",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 606,
                                    KeywordsEn = "break report attendance breaks",
                                    KeywordsAr = "تقرير الاستراحة استراحات الحضور"
                                },
                                new MenuItem
                                {
                                    TextEn = "Permission Requests Report",
                                    TextAr = "تقرير طلبات الاذن",
                                    Icon = "bi-door-open",
                                    Url = "/Attendance/Reports/MidDayPermission",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 607,
                                    KeywordsEn = "permission requests report mid day permission attendance",
                                    KeywordsAr = "تقرير طلبات الإذن أذونات منتصف اليوم"
                                },
                                new MenuItem
                                {
                                    TextEn = "Absence Report",
                                    TextAr = "تقرير الغياب",
                                    Icon = "bi-person-x",
                                    Url = "/Attendance/Reports/Absence",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 608,
                                    KeywordsEn = "absence report attendance absent employees",
                                    KeywordsAr = "تقرير الغياب غياب الموظفين"
                                },
                                new MenuItem
                                {
                                    TextEn = "Holiday / Weekend Report",
                                    TextAr = "تقرير العطلات ونهاية الأسبوع",
                                    Icon = "bi-calendar-event",
                                    Url = "/Attendance/Reports/HolidayWeekend",
                                    PermissionPolicy = PermissionList.AttendancePermissions.ViewScopedReports,
                                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                                    NavigationOrder = 609,
                                    KeywordsEn = "holiday weekend report attendance calendar holidays",
                                    KeywordsAr = "تقرير العطلات نهاية الأسبوع تقويم الحضور"
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
                    PermissionPolicy = null,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                    NavigationOrder = 300,
                    KeywordsEn = "leave emergency balances reports approvals policies applications ledger encashment allocations",
                    KeywordsAr = "إجازات طارئة أرصدة تقارير اعتماد سياسات طلبات سجل صرف مخصصات",
                    IsFavoriteCandidate = false,
                    Children = new()
                    {
                        new MenuItem
                        {
                            TextEn = "Emergency Leaves",
                            TextAr = "الإجازات الطارئة",
                            Icon = "bi-life-preserver",
                            Url = "/LeavesManagement/EmergencyLeaves",
                            PermissionPolicy = PermissionList.LeavePermissions.RequestEmergencyLeave,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 300,
                            KeywordsEn = "leave emergency request my leaves attachment",
                            KeywordsAr = "إجازات طارئة طلب إجازاتي مرفق"
                        },
                        new MenuItem
                        {
                            TextEn = "Approve Emergency Leave",
                            TextAr = "اعتماد الإجازة الطارئة",
                            Icon = "bi-patch-check",
                            Url = "/LeavesManagement/ApproveEmergencyLeaves",
                            PermissionPolicy = PermissionList.LeavePermissions.ApproveEmergencyLeave,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupApprovals,
                            NavigationOrder = 310,
                            KeywordsEn = "leave emergency approval approve reject queue",
                            KeywordsAr = "إجازات طارئة اعتماد موافقة رفض قائمة"
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Balances",
                            TextAr = "أرصدة الإجازات",
                            Icon = "bi-sliders",
                            Url = "/LeavesManagement/Balances",
                            PermissionPolicy = PermissionList.LeavePermissions.ViewLeaveBalances,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 320,
                            KeywordsEn = "leave balances entitlement annual carry forward remaining",
                            KeywordsAr = "أرصدة إجازات استحقاق سنوي ترحيل متبقي"
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Policies",
                            TextAr = "سياسات الإجازات",
                            Icon = "bi-ui-checks-grid",
                            Url = "/HR/LeavePolicies",
                            PermissionPolicy = PermissionList.LeavePolicyPermissions.View,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                            NavigationOrder = 330,
                            KeywordsEn = "leave policies types periods assignments allocations setup",
                            KeywordsAr = "سياسات إجازات أنواع فترات تعيينات مخصصات إعداد"
                        },
                        new MenuItem
                        {
                            TextEn = "My Leave Applications",
                            TextAr = "طلبات إجازاتي",
                            Icon = "bi-calendar-plus",
                            Url = "/LeavesManagement/MyLeaveApplications",
                            PermissionPolicy = PermissionList.LeaveApplicationPermissions.Request,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 335,
                            KeywordsEn = "my leave applications request submit cancel attachment self service",
                            KeywordsAr = "طلباتي إجازات إرسال إلغاء مرفق خدمة ذاتية"
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Applications",
                            TextAr = "طلبات الإجازات",
                            Icon = "bi-calendar-plus",
                            Url = "/HR/LeaveApplications",
                            PermissionPolicy = PermissionList.LeaveApplicationPermissions.View,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 340,
                            KeywordsEn = "leave applications request submit approve reject cancel attachment",
                            KeywordsAr = "طلبات إجازات إرسال اعتماد رفض إلغاء مرفق"
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Ledger",
                            TextAr = "سجل الإجازات",
                            Icon = "bi-journal-text",
                            Url = "/HR/LeaveLedger",
                            PermissionPolicy = PermissionList.LeaveLedgerPermissions.View,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupDailyWork,
                            NavigationOrder = 350,
                            KeywordsEn = "leave ledger adjustments encashment balance movements",
                            KeywordsAr = "سجل إجازات تسويات صرف رصيد حركات"
                        },
                        new MenuItem
                        {
                            TextEn = "Leave Reports",
                            TextAr = "تقارير الإجازات",
                            Icon = "bi-file-earmark-bar-graph",
                            Url = "/LeavesManagement/Reports",
                            PermissionPolicy = PermissionList.LeavePermissions.ViewLeaveReports,
                            WorkspaceKey = NavigationMenuResolver.WorkspaceHr,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupLeaves,
                            NavigationGroupKey = NavigationMenuResolver.NavigationGroupReports,
                            NavigationOrder = 360,
                            KeywordsEn = "leave reports entitlement taken remaining pending approved rejected",
                            KeywordsAr = "تقارير إجازات استحقاق مستخدم متبقي معلق معتمد مرفوض"
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.HrFunctionalGroupPayroll,
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
                            TextEn = "Salary Structures",
                            TextAr = "هياكل الرواتب",
                            Icon = "bi-diagram-3",
                            Url = "/HR/PayrollStructures",
                            PermissionPolicy = PermissionList.PayrollStructurePermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Payslips",
                            TextAr = "مسيرات الموظفين",
                            Icon = "bi-receipt",
                            Url = "/HR/Payslips",
                            PermissionPolicy = PermissionList.PayrollPayslipPermissions.View
                        },
                        new MenuItem
                        {
                            TextEn = "Saudi Payroll",
                            TextAr = "رواتب السعودية",
                            Icon = "bi-bank",
                            Url = "/HR/SaudiPayroll",
                            PermissionPolicy = PermissionList.PayrollPayslipPermissions.Generate
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.WarehouseFunctionalGroupProducts,
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.WarehouseFunctionalGroupInventory,
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
                            TextEn = "Controls",
                            TextAr = "ضوابط المخزون",
                            Icon = "bi-sliders",
                            Url = "/Inventory/Controls",
                            PermissionPolicy = PermissionList.InventoryPermissions.View,
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
                            NavigationFunctionalGroupKey = NavigationMenuResolver.WarehouseFunctionalGroupStockOperations,

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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.PurchasingFunctionalGroupSuppliers,
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.PurchasingFunctionalGroupProcurement,
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
                            TextEn = "Procurement Controls",
                            TextAr = "ضوابط المشتريات",
                            Icon = "bi-diagram-3",
                            Url = "/Procurement/Enhancements",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Supplier Items",
                            TextAr = "أصناف الموردين",
                            Icon = "bi-box-seam",
                            Url = "/Procurement/SupplierItems",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Vendor Pricelists",
                            TextAr = "قوائم أسعار الموردين",
                            Icon = "bi-tags",
                            Url = "/Procurement/VendorPricelists",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Reordering Rules",
                            TextAr = "قواعد إعادة الطلب",
                            Icon = "bi-arrow-repeat",
                            Url = "/Procurement/ReorderingRules",
                            PermissionPolicy = PermissionList.PurchaseRequestPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Replenishment",
                            TextAr = "تجديد المخزون",
                            Icon = "bi-basket",
                            Url = "/Procurement/Replenishment",
                            PermissionPolicy = PermissionList.PurchaseRequestPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Procurement Tracker",
                            TextAr = "متتبع المشتريات",
                            Icon = "bi-activity",
                            Url = "/Procurement/Tracker",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
                        },
                        new MenuItem
                        {
                            TextEn = "Supplier Scorecard",
                            TextAr = "تقييم الموردين",
                            Icon = "bi-clipboard2-data",
                            Url = "/Procurement/SupplierScorecard",
                            PermissionPolicy = PermissionList.PurchaseOrderPermissions.View,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupDocuments,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupDocuments,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupProjects,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupTasks,
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupFleet,
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupFleet,
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
                    WorkspaceKey = NavigationMenuResolver.WorkspaceRealEstate,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupProperties,
                    BusinessLineKey = SharedWithUI.Organization.BusinessLineKeys.RealEstate,
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
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupLeasing
                        },
                        new MenuItem
                        {
                            TextEn = "Tenant Leases",
                            TextAr = "Tenant Leases",
                            Icon = "bi-file-earmark-check",
                            Url = "/RealEstate/TenantLeases",
                            PermissionPolicy = PermissionList.RealEstateLeasePermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupLeasing
                        },
                        new MenuItem
                        {
                            TextEn = "Rent Collections",
                            TextAr = "Rent Collections",
                            Icon = "bi-cash-coin",
                            Url = "/RealEstate/Collections",
                            PermissionPolicy = PermissionList.RealEstateInstallmentPermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupCollections
                        },
                        new MenuItem
                        {
                            TextEn = "Utilities",
                            TextAr = "Utilities",
                            Icon = "bi-lightning-charge",
                            Url = "/RealEstate/Utilities",
                            PermissionPolicy = PermissionList.RealEstateUtilityPermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupUtilitiesExpenses
                        },
                        new MenuItem
                        {
                            TextEn = "Expenses",
                            TextAr = "Expenses",
                            Icon = "bi-receipt",
                            Url = "/RealEstate/Expenses",
                            PermissionPolicy = PermissionList.RealEstateExpensePermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupUtilitiesExpenses
                        },
                        new MenuItem
                        {
                            TextEn = "Reports",
                            TextAr = "Reports",
                            Icon = "bi-bar-chart-line",
                            Url = "/RealEstate/Reports",
                            PermissionPolicy = PermissionList.RealEstateReportsPermissions.View,
                            NavigationFunctionalGroupKey = NavigationMenuResolver.RealEstateFunctionalGroupReports
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
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupMaintenance,
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
            WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
            NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupGeneralSettings,
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
                },
                new MenuItem
                {
                    TextEn = "Demo Data",
                    TextAr = "البيانات التجريبية",
                    Icon = "bi-database-gear",
                    Url = "/GeneralSettings/DemoData",
                    PermissionPolicy = PermissionList.DemoDataPermissions.View,
                    WorkspaceKey = NavigationMenuResolver.WorkspaceAdmin,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.AdminFunctionalGroupGeneralSettings,
                    NavigationGroupKey = NavigationMenuResolver.NavigationGroupSetup,
                    NavigationOrder = 140,
                    KeywordsEn = "admin general settings demo data seed reset delete tenant",
                    KeywordsAr = "إدارة إعدادات عامة بيانات تجريبية إنشاء إعادة ضبط حذف شركة"
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
            NavigationFunctionalGroupKey = NavigationMenuResolver.SecurityFunctionalGroupSecurity,
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
                    PermissionPolicy = PermissionList.RolesPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.SecurityFunctionalGroupRoles
                },
                new MenuItem
                {
                    TextEn = "Assign User Roles",
                    TextAr = "تعيين صلاحيات المستخدمين",
                    Icon = "bi-person-gear",
                    Url = "/Auth/User/AssignRole",
                    PermissionPolicy = PermissionList.UsersPermissions.View,
                    NavigationFunctionalGroupKey = NavigationMenuResolver.SecurityFunctionalGroupUserAccess
                }
            }
        }
    };
}
