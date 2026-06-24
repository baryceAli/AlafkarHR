using System.Security.Claims;
using AlAfkarERP.Shared.Utilities;

namespace AlAfkarERP.Shared.Layout;

public static partial class NavigationMenuResolver
{
    public const string WorkspaceHome = "home";
    public const string WorkspaceHr = "hr";
    public const string WorkspaceSales = "sales";
    public const string WorkspacePurchasing = "purchasing";
    public const string WorkspaceCatering = "catering";
    public const string WorkspaceRealEstate = "real-estate";
    public const string WorkspaceWarehouse = "warehouse";
    public const string WorkspaceAccountingFinance = "accounting-finance";
    public const string WorkspaceAdmin = "admin";
    public const string WorkspaceSecurity = "it-security";
    public const string WorkspaceMore = "more";
    public const string WorkspacePeople = WorkspaceHr;
    public const string WorkspaceOperations = WorkspaceSales;
    public const string WorkspaceFinancePayroll = WorkspaceHr;
    public const string WorkspaceInventory = WorkspaceWarehouse;
    public const string WorkspacePos = "pos";
    public const string WorkspaceProcurement = WorkspacePurchasing;

    public const string NavigationGroupStart = "start";
    public const string NavigationGroupSetup = "setup";
    public const string NavigationGroupMasterData = "master-data";
    public const string NavigationGroupDailyWork = "daily-work";
    public const string NavigationGroupApprovals = "approvals";
    public const string NavigationGroupAdjustments = "adjustments";
    public const string NavigationGroupReports = "reports";
    public const string NavigationGroupAdministration = "administration";

    public const string HubHr = WorkspaceHr;
    public const string HubSales = WorkspaceSales;
    public const string HubPurchasing = WorkspacePurchasing;
    public const string HubCatering = WorkspaceCatering;
    public const string HubRealEstate = WorkspaceRealEstate;
    public const string HubWarehouse = WorkspaceWarehouse;
    public const string HubAccountingFinance = WorkspaceAccountingFinance;
    public const string HubAdmin = WorkspaceAdmin;
    public const string HubSecurity = WorkspaceSecurity;

    public static readonly IReadOnlyList<NavigationWorkspace> MobileWorkspaces =
    [
        new(WorkspaceHome, "Home", "\u0627\u0644\u0631\u0626\u064a\u0633\u064a\u0629", "bi-house-door", "/Dashboard"),
        new(WorkspacePos, "POS", "\u0646\u0642\u0637\u0629 \u0628\u064a\u0639", "bi-receipt", "/SalesOrder/POS"),
        new(WorkspaceHr, "HR", "\u0627\u0644\u0645\u0648\u0627\u0631\u062f", "bi-people", "/Employee/Dashboard"),
        new(WorkspaceSales, "Sales", "\u0627\u0644\u0645\u0628\u064a\u0639\u0627\u062a", "bi-graph-up-arrow", "/Sales/Dashboard"),
        new(WorkspacePurchasing, "Purchasing", "\u0627\u0644\u0645\u0634\u062a\u0631\u064a\u0627\u062a", "bi-cart-check", "/Procurement/Dashboard"),
        new(WorkspaceCatering, "Catering", "\u062e\u062f\u0645\u0627\u062a \u0627\u0644\u0625\u0639\u0627\u0634\u0629", "bi-cup-hot", "/Catering/Dashboard"),
        new(WorkspaceRealEstate, "Real Estate", "\u0627\u0644\u0639\u0642\u0627\u0631\u0627\u062a", "bi-buildings", "/RealEstate/Dashboard"),
        new(WorkspaceWarehouse, "Warehouse", "\u0627\u0644\u0645\u0633\u062a\u0648\u062f\u0639", "bi-boxes", "/Inventory/Dashboard"),
        new(WorkspaceAccountingFinance, "Accounting / Finance", "\u0627\u0644\u0645\u062d\u0627\u0633\u0628\u0629 / \u0627\u0644\u0645\u0627\u0644\u064a\u0629", "bi-cash-stack", null),
        new(WorkspaceAdmin, "Admin", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629", "bi-sliders2-vertical", "/Dashboard"),
        new(WorkspaceSecurity, "IT / Security", "\u062a\u0642\u0646\u064a\u0629 \u0648\u0623\u0645\u0627\u0646", "bi-shield-lock", "/Auth/Dashboard"),
        new(WorkspaceMore, "More", "\u0627\u0644\u0645\u0632\u064a\u062f", "bi-grid-3x3-gap", null)
    ];

    public static readonly IReadOnlyList<NavigationHubWorkspace> HubWorkspaces =
    [
        new(HubHr, "HR", "\u0627\u0644\u0645\u0648\u0627\u0631\u062f", "bi-people"),
        new(HubSales, "Sales", "\u0627\u0644\u0645\u0628\u064a\u0639\u0627\u062a", "bi-graph-up-arrow"),
        new(HubPurchasing, "Purchasing", "\u0627\u0644\u0645\u0634\u062a\u0631\u064a\u0627\u062a", "bi-cart-check"),
        new(HubCatering, "Catering", "\u062e\u062f\u0645\u0627\u062a \u0627\u0644\u0625\u0639\u0627\u0634\u0629", "bi-cup-hot"),
        new(HubRealEstate, "Real Estate", "\u0627\u0644\u0639\u0642\u0627\u0631\u0627\u062a", "bi-buildings"),
        new(HubWarehouse, "Warehouse", "\u0627\u0644\u0645\u0633\u062a\u0648\u062f\u0639", "bi-boxes"),
        new(HubAccountingFinance, "Accounting / Finance", "\u0627\u0644\u0645\u062d\u0627\u0633\u0628\u0629 / \u0627\u0644\u0645\u0627\u0644\u064a\u0629", "bi-cash-stack"),
        new(HubAdmin, "Admin", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629", "bi-sliders2-vertical"),
        new(HubSecurity, "IT / Security", "\u062a\u0642\u0646\u064a\u0629 \u0648\u0623\u0645\u0627\u0646", "bi-shield-lock")
    ];

    public static IReadOnlyList<MenuItem> GetActivePath(string currentUri, string baseUri)
        => GetActivePath(currentUri, baseUri, null);

    public static IReadOnlyList<MenuItem> GetActivePath(string currentUri, string baseUri, string? preferredWorkspaceKey)
    {
        var currentPath = NormalizePath(ToRelativePath(currentUri, baseUri));
        var bestMatch = (Path: new List<MenuItem>(), Score: 0, Specificity: 0);

        if (!string.IsNullOrWhiteSpace(preferredWorkspaceKey))
        {
            foreach (var item in MenuItem.Menu)
            {
                var candidate = FindBestPathCandidate(item, currentPath);
                if (candidate.Path.Count == 0 || !PathBelongsToWorkspace(candidate.Path, preferredWorkspaceKey))
                {
                    continue;
                }

                if (IsBetterPathCandidate(candidate, bestMatch))
                {
                    bestMatch = candidate;
                }
            }

            if (bestMatch.Path.Count > 0)
            {
                return bestMatch.Path;
            }
        }

        foreach (var item in MenuItem.Menu)
        {
            var candidate = FindBestPathCandidate(item, currentPath);
            if (IsBetterPathCandidate(candidate, bestMatch))
            {
                bestMatch = candidate;
            }
        }

        return bestMatch.Path;
    }

    public static string NormalizePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return "/";
        }

        var path = url.Split('?', '#')[0].Trim();

        if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri))
        {
            path = absoluteUri.AbsolutePath;
        }

        path = "/" + path.Trim('/').ToLowerInvariant();
        return path == "/" ? "/" : path.TrimEnd('/');
    }

    public static bool IsExactPathMatch(string? currentPath, string? targetPath)
        => NormalizePath(currentPath) == NormalizePath(targetPath);

    public static bool IsPathWithinUrl(string? currentPath, string? targetUrl)
    {
        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            return false;
        }

        var normalizedCurrentPath = NormalizePath(currentPath);
        var normalizedTargetPath = NormalizePath(targetUrl);

        return normalizedCurrentPath == normalizedTargetPath
               || normalizedTargetPath != "/"
               && normalizedCurrentPath.StartsWith($"{normalizedTargetPath}/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPathWithinItem(string? currentPath, MenuItem item)
        => IsPathWithinUrl(currentPath, item.Url)
           || item.NavigationAliases.Any(alias => IsPathWithinUrl(currentPath, alias));

    public static string GetStorageKey(MenuItem item)
        => !string.IsNullOrWhiteSpace(item.Url)
            ? NormalizePath(item.Url)
            : $"{item.TextEn}|{item.TextAr}";

    public static string GetWorkspaceKey(MenuItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.WorkspaceKey))
        {
            return item.WorkspaceKey;
        }

        if (TryFindInheritedWorkspaceKey(MenuItem.Menu, item, null, out var inheritedWorkspaceKey))
        {
            return inheritedWorkspaceKey;
        }

        var path = NormalizePath(item.Url);
        var text = item.TextEn;

        if (path == "/" || path == "/dashboard" || text.Equals("Home", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceHome;
        }

        if (path.StartsWith("/accounting", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Accounting", StringComparison.OrdinalIgnoreCase)
            || text.Contains("ZATCA", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Journal", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Ledger", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceAccountingFinance;
        }

        if (path == "/salesorder/pos" || text.Equals("POS", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspacePos;
        }

        if (text.Contains("Finance", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceAccountingFinance;
        }

        if (path.StartsWith("/auth", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Security", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Role", StringComparison.OrdinalIgnoreCase)
            || text.Contains("User", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceSecurity;
        }

        if (path.StartsWith("/catering", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Catering", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Ramadan", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceCatering;
        }

        if (path.StartsWith("/realestate", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Real Estate", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceRealEstate;
        }

        if (path.StartsWith("/generalsettings", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/contracts", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/documentmanagement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/fleet", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/maintenance", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/organization", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/projectmanagement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/taskmanagement", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Contract", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Document", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Fleet", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Maintenance", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Organization", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Company", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Branch", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Department", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Administration", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Task", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Settings", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Currency", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceAdmin;
        }

        if (path.StartsWith("/employee", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/attendance", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/leavesmanagement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/payroll", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Human Resource", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Employee", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Attendance", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Payroll", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Salary", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Loan", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Deduction", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Leave", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceHr;
        }

        if (path == "/salesorder/pos"
            || text.Equals("POS", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspacePos;
        }

        if (path.StartsWith("/salesorder", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/sales", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/orders", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/customers", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Sales", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Customer", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceSales;
        }

        if (path.StartsWith("/procurement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/suppliers", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Procurement", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Purchase", StringComparison.OrdinalIgnoreCase)
            || text.Contains("RFQ", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Supplier", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspacePurchasing;
        }

        if (path.StartsWith("/inventory", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/inventories", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/warehouse", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/catalog/pricing", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/catalog/product", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/catalog/variant", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Inventory", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Stock", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Warehouse", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Batch", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Product", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceWarehouse;
        }

        return WorkspaceMore;
    }

    public static string ResolveActiveWorkspace(string currentUri, string baseUri)
        => ResolveActiveWorkspace(currentUri, baseUri, null);

    public static string ResolveActiveWorkspace(string currentUri, string baseUri, string? preferredWorkspaceKey)
    {
        var currentPath = NormalizePath(ToRelativePath(currentUri, baseUri));
        if (currentPath == "/" || currentPath == "/dashboard")
        {
            return WorkspaceHome;
        }

        if (TryResolveWorkspaceMatch(currentPath, preferredWorkspaceKey, out var preferredWorkspace))
        {
            return preferredWorkspace;
        }

        if (currentPath.StartsWith("/projectmanagement", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceAdmin;
        }

        if (TryResolveExplicitExactWorkspaceMatch(currentPath, out var exactWorkspace))
        {
            return exactWorkspace;
        }

        var activePath = GetActivePath(currentUri, baseUri, preferredWorkspaceKey);
        return activePath.Count == 0 ? WorkspaceHome : GetWorkspaceKey(activePath.Last());
    }

    public static IReadOnlyList<MenuItem> GetWorkspaceItems(string workspaceKey)
        => MenuItem.Menu
            .Where(item => GetWorkspaceKey(item) == workspaceKey || Flatten(item.Children).Any(child => GetWorkspaceKey(child) == workspaceKey))
            .ToList();

    public static IReadOnlyList<MenuItem> GetNavigableItems()
        => Flatten(MenuItem.Menu)
            .Where(item => !string.IsNullOrWhiteSpace(item.Url))
            .OrderBy(item => GetMobilePriority(item))
            .ThenBy(item => item.TextEn)
            .ToList();

    public static IReadOnlyList<MenuItem> GetAuthorizedTree(ClaimsPrincipal? user)
        => GetAuthorizedTree(user, null);

    public static IReadOnlyList<MenuItem> GetAuthorizedTree(ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => MenuItem.Menu
            .Select(item => FilterAuthorized(item, user, licensedBusinessLineKeys))
            .Where(item => item is not null)
            .Cast<MenuItem>()
            .ToList();

    public static IReadOnlyList<MenuItem> GetAuthorizedNavigableItems(ClaimsPrincipal? user)
        => GetAuthorizedNavigableItems(user, null);

    public static IReadOnlyList<MenuItem> GetAuthorizedNavigableItems(ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => Flatten(MenuItem.Menu)
            .Where(item => HasOwnPermission(item, user, licensedBusinessLineKeys) && !string.IsNullOrWhiteSpace(item.Url))
            .OrderBy(item => GetMobilePriority(item))
            .ThenBy(item => item.TextEn)
            .ToList();

    public static IReadOnlyList<NavigationWorkspace> GetAuthorizedWorkspaces(ClaimsPrincipal? user)
        => GetAuthorizedWorkspaces(user, null);

    public static IReadOnlyList<NavigationWorkspace> GetAuthorizedWorkspaces(ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => MobileWorkspaces
            .Where(workspace => IsWorkspaceAvailable(workspace, user, licensedBusinessLineKeys))
            .ToList();

    public static string? GetWorkspaceDefaultUrl(string workspaceKey, ClaimsPrincipal? user)
        => GetWorkspaceDefaultUrl(workspaceKey, user, null);

    public static string? GetWorkspaceDefaultUrl(string workspaceKey, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        if (workspaceKey == WorkspaceHome)
        {
            return "/Dashboard";
        }

        if (workspaceKey == WorkspaceMore)
        {
            return null;
        }

        if (workspaceKey == WorkspaceAdmin
            && CompanyContext.IsPlatformScoped(user)
            && CompanyContext.HasPermission(user, SharedWithUI.Permissions.PermissionList.ParentCompanyPermissions.View))
        {
            return "/Organization/ParentCompanies";
        }

        var authorizedItems = GetAuthorizedNavigableItems(user, licensedBusinessLineKeys).ToList();
        if (workspaceKey == WorkspaceAccountingFinance
            && authorizedItems.Any(item => NormalizePath(item.Url) == "/accounting/dashboard"))
        {
            return "/Accounting/Dashboard";
        }

        var workspace = MobileWorkspaces.FirstOrDefault(item => item.Key == workspaceKey);
        if (!string.IsNullOrWhiteSpace(workspace?.Url)
            && authorizedItems.Any(item => NormalizePath(item.Url) == NormalizePath(workspace.Url)))
        {
            return workspace.Url;
        }

        return authorizedItems.FirstOrDefault(item => GetWorkspaceKey(item) == workspaceKey)?.Url;
    }

    public static IReadOnlyList<NavigationHubSection> GetAuthorizedHubSections(ClaimsPrincipal? user, string hubWorkspaceKey)
    {
        var sections = new List<NavigationHubSection>();
        foreach (var root in GetAuthorizedTree(user))
        {
            AddHubSections(root, user, hubWorkspaceKey, sections);
        }

        return sections;
    }

    public static string ResolveHubWorkspaceKey(string currentUri, string baseUri)
        => ResolveHubWorkspaceKey(currentUri, baseUri, null);

    public static string ResolveHubWorkspaceKey(string currentUri, string baseUri, string? preferredWorkspaceKey)
    {
        var activePath = GetActivePath(currentUri, baseUri, preferredWorkspaceKey);
        return activePath.Count == 0 ? HubAdmin : ResolveHubWorkspaceKey(activePath.Last());
    }

    public static string ResolveHubWorkspaceKey(MenuItem item)
    {
        var workspaceKey = GetWorkspaceKey(item);
        if (workspaceKey == WorkspacePos)
        {
            return HubSales;
        }

        return workspaceKey == WorkspaceMore ? HubAdmin : workspaceKey;
    }

    public static string ResolveNavigationWorkspace(MenuItem item, string? currentWorkspaceKey)
    {
        if (!string.IsNullOrWhiteSpace(currentWorkspaceKey)
            && ItemCanNavigateWithinWorkspace(item, currentWorkspaceKey))
        {
            return currentWorkspaceKey;
        }

        return GetWorkspaceKey(item);
    }

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedRows(ClaimsPrincipal? user, IEnumerable<MenuItem>? source = null)
        => GetAuthorizedRows(user, source, null);

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedRows(ClaimsPrincipal? user, IEnumerable<MenuItem>? source, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        var rows = new List<NavigationMenuRow>();
        foreach (var item in source ?? MenuItem.Menu)
        {
            AddAuthorizedRows(item, user, licensedBusinessLineKeys, [], 0, rows);
        }

        return rows;
    }

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspaceRows(ClaimsPrincipal? user, string workspaceKey)
        => GetAuthorizedWorkspaceRows(user, workspaceKey, null);

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspaceRows(ClaimsPrincipal? user, string workspaceKey, IReadOnlySet<string>? licensedBusinessLineKeys)
        => GetAuthorizedRows(user, null, licensedBusinessLineKeys)
            .Where(row => workspaceKey == WorkspaceMore || RowBelongsToWorkspace(row.Item, workspaceKey, user, licensedBusinessLineKeys))
            .ToList();

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspacePanelRows(ClaimsPrincipal? user, string workspaceKey)
        => GetAuthorizedWorkspacePanelRows(user, workspaceKey, null);

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspacePanelRows(ClaimsPrincipal? user, string workspaceKey, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        if (workspaceKey == WorkspaceMore)
        {
            return GetAuthorizedWorkspaceRows(user, workspaceKey, licensedBusinessLineKeys);
        }

        var journeyRows = new List<NavigationMenuRow>();
        foreach (var section in BuildJourneyPanelSections(user, workspaceKey, licensedBusinessLineKeys))
        {
            AddPanelRows(section, [], 0, journeyRows);
        }

        if (journeyRows.Count > 0)
        {
            return journeyRows;
        }

        var rows = new List<NavigationMenuRow>();
        var roots = GetAuthorizedTree(user, licensedBusinessLineKeys);
        foreach (var section in GetWorkspacePanelSections(roots, workspaceKey))
        {
            var shapedSection = FilterWorkspacePanelSection(section, workspaceKey);
            if (shapedSection is not null)
            {
                AddPanelRows(shapedSection, [], 0, rows);
            }
        }

        return rows;
    }

    public static IReadOnlyList<MenuItem> GetMenuPath(MenuItem target)
        => GetMenuPath(target, null);

    public static IReadOnlyList<MenuItem> GetMenuPath(MenuItem target, string? preferredWorkspaceKey)
    {
        if (!string.IsNullOrWhiteSpace(preferredWorkspaceKey))
        {
            foreach (var item in MenuItem.Menu)
            {
                var path = FindPath(item, target);
                if (path.Count > 0 && PathBelongsToWorkspace(path, preferredWorkspaceKey))
                {
                    return path;
                }
            }
        }

        foreach (var item in MenuItem.Menu)
        {
            var path = FindPath(item, target);
            if (path.Count > 0)
            {
                return path;
            }
        }

        return [];
    }

    public static bool HasOwnPermission(MenuItem item, ClaimsPrincipal? user)
        => HasOwnPermission(item, user, null);

    public static bool HasOwnPermission(MenuItem item, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => IsBusinessLineAllowed(item, licensedBusinessLineKeys)
           && CompanyContext.HasPermission(user, item.PermissionPolicy);

    public static bool IsAuthorized(MenuItem item, ClaimsPrincipal? user)
        => IsAuthorized(item, user, null);

    public static bool IsAuthorized(MenuItem item, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => HasOwnPermission(item, user, licensedBusinessLineKeys)
           || item.Children.Any(child => IsAuthorized(child, user, licensedBusinessLineKeys));

    public static bool HasAuthorizedChildren(MenuItem item, ClaimsPrincipal? user)
        => item.Children.Any(child => IsAuthorized(child, user));

    private static bool IsBusinessLineAllowed(MenuItem item, IReadOnlySet<string>? licensedBusinessLineKeys)
        => string.IsNullOrWhiteSpace(item.BusinessLineKey)
           || licensedBusinessLineKeys is null
           || licensedBusinessLineKeys.Contains(item.BusinessLineKey);

    public static int GetMobilePriority(MenuItem item)
    {
        if (item.MobilePriority.HasValue)
        {
            return item.MobilePriority.Value;
        }

        var path = NormalizePath(item.Url);
        return path switch
        {
            "/" => 0,
            "/dashboard" => 1,
            "/employee/dashboard" => 2,
            "/employee/employee/list" => 3,
            "/salesorder/pos" => 4,
            "/customers/customer/list" => 5,
            "/procurement/dashboard" => 6,
            "/procurement/purchase-requests" => 7,
            "/procurement/purchase-orders" => 8,
            "/suppliers/supplier/list" => 9,
            "/inventory/dashboard" => 10,
            "/inventories/list" => 11,
            "/inventory/operations/stockin" => 12,
            "/inventory/operations/stockout" => 13,
            "/inventory/warehouse/list" => 14,
            "/payroll/salaryruns" => 15,
            "/taskmanagement/mytasks" => 16,
            "/taskmanagement/dashboard" => 17,
            "/generalsettings/systemsettings" => 18,
            "/auth/dashboard" => 19,
            "/auth/role/list" => 20,
            "/auth/user/assignrole" => 21,
            _ => 100
        };
    }

    public static NavigationJourneyGroup GetNavigationJourneyGroup(MenuItem item)
    {
        var key = ResolveNavigationGroupKey(item);
        return NavigationJourneyGroups.FirstOrDefault(group => group.Key == key)
               ?? NavigationJourneyGroups.First(group => group.Key == NavigationGroupDailyWork);
    }

    public static string ResolveNavigationGroupKey(MenuItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.NavigationGroupKey))
        {
            return item.NavigationGroupKey;
        }

        var path = NormalizePath(item.Url);
        var text = GetNavigationHaystack(item);

        if (IsJourneyStart(path, text))
        {
            return NavigationGroupStart;
        }

        if (IsJourneyReport(path, text))
        {
            return NavigationGroupReports;
        }

        if (IsJourneyApproval(path, text))
        {
            return NavigationGroupApprovals;
        }

        if (IsJourneySetup(path, text))
        {
            return NavigationGroupSetup;
        }

        if (IsJourneyAdministration(path, text))
        {
            return NavigationGroupAdministration;
        }

        if (IsJourneyAdjustment(path, text))
        {
            return NavigationGroupAdjustments;
        }

        if (IsJourneyMasterData(path, text))
        {
            return NavigationGroupMasterData;
        }

        return NavigationGroupDailyWork;
    }

    public static int GetNavigationJourneyOrder(MenuItem item)
    {
        if (item.NavigationOrder.HasValue)
        {
            return item.NavigationOrder.Value;
        }

        var path = NormalizePath(item.Url);
        if (JourneyPathPriorities.TryGetValue(path, out var pathPriority))
        {
            return pathPriority;
        }

        var group = GetNavigationJourneyGroup(item);
        return group.Order * 1000 + GetMobilePriority(item);
    }

    public static IEnumerable<MenuItem> Flatten(IEnumerable<MenuItem> items)
    {
        foreach (var item in items)
        {
            yield return item;

            foreach (var child in Flatten(item.Children))
            {
                yield return child;
            }
        }
    }

    private static (List<MenuItem> Path, int Score, int Specificity) FindBestPathCandidate(MenuItem item, string currentPath)
    {
        var ownMatch = GetPathMatch(currentPath, item);

        var bestChildPath = (Path: new List<MenuItem>(), Score: 0, Specificity: 0);
        foreach (var child in item.Children)
        {
            var childPath = FindBestPathCandidate(child, currentPath);
            if (IsBetterPathCandidate(childPath, bestChildPath))
            {
                bestChildPath = childPath;
            }
        }

        if (ownMatch.Score > 0 || bestChildPath.Path.Count > 0)
        {
            var result = new List<MenuItem> { item };
            result.AddRange(bestChildPath.Path);
            return ownMatch.Score > bestChildPath.Score
                ? (result, ownMatch.Score, ownMatch.Specificity)
                : (result, bestChildPath.Score, bestChildPath.Specificity);
        }

        return (new List<MenuItem>(), 0, 0);
    }

    private static (int Score, int Specificity) GetPathMatch(string currentPath, MenuItem item)
    {
        var bestMatch = GetPathMatch(currentPath, item.Url);
        foreach (var alias in item.NavigationAliases)
        {
            var aliasMatch = GetPathMatch(currentPath, alias);
            if (aliasMatch.Score > bestMatch.Score
                || aliasMatch.Score == bestMatch.Score && aliasMatch.Specificity > bestMatch.Specificity)
            {
                bestMatch = aliasMatch;
            }
        }

        return bestMatch;
    }

    private static (int Score, int Specificity) GetPathMatch(string currentPath, string? targetUrl)
    {
        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            return (0, 0);
        }

        var targetPath = NormalizePath(targetUrl);
        if (currentPath == targetPath)
        {
            return (2, targetPath.Length);
        }

        return targetPath != "/"
               && currentPath.StartsWith($"{targetPath}/", StringComparison.OrdinalIgnoreCase)
            ? (1, targetPath.Length)
            : (0, 0);
    }

    private static bool IsBetterPathCandidate(
        (List<MenuItem> Path, int Score, int Specificity) candidate,
        (List<MenuItem> Path, int Score, int Specificity) currentBest)
    {
        if (candidate.Path.Count != currentBest.Path.Count)
        {
            return candidate.Path.Count > currentBest.Path.Count;
        }

        if (candidate.Score != currentBest.Score)
        {
            return candidate.Score > currentBest.Score;
        }

        return candidate.Specificity > currentBest.Specificity;
    }

    private static string ToRelativePath(string currentUri, string baseUri)
    {
        if (Uri.TryCreate(currentUri, UriKind.Absolute, out var absoluteCurrent)
            && Uri.TryCreate(baseUri, UriKind.Absolute, out var absoluteBase)
            && absoluteBase.IsBaseOf(absoluteCurrent))
        {
            var relative = absoluteBase.MakeRelativeUri(absoluteCurrent).ToString();
            return Uri.UnescapeDataString(relative);
        }

        return currentUri;
    }

    private static MenuItem? FilterAuthorized(MenuItem item, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        var children = item.Children
            .Select(child => FilterAuthorized(child, user, licensedBusinessLineKeys))
            .Where(child => child is not null)
            .Cast<MenuItem>()
            .ToList();

        if (!HasOwnPermission(item, user, licensedBusinessLineKeys) && children.Count == 0)
        {
            return null;
        }

        return new MenuItem
        {
            TextAr = item.TextAr,
            TextEn = item.TextEn,
            PermissionPolicy = item.PermissionPolicy,
            Icon = item.Icon,
            Url = item.Url,
            BadgeText = item.BadgeText,
            BadgeCssClass = item.BadgeCssClass,
            BadgeTitleEn = item.BadgeTitleEn,
            BadgeTitleAr = item.BadgeTitleAr,
            WorkspaceKey = item.WorkspaceKey,
            BusinessLineKey = item.BusinessLineKey,
            NavigationGroupKey = item.NavigationGroupKey,
            NavigationOrder = item.NavigationOrder,
            ProcessKey = item.ProcessKey,
            NavigationAliases = item.NavigationAliases.ToList(),
            MobilePriority = item.MobilePriority,
            KeywordsEn = item.KeywordsEn,
            KeywordsAr = item.KeywordsAr,
            IsFavoriteCandidate = item.IsFavoriteCandidate,
            Children = children,
            IsOpen = item.IsOpen,
            IsActive = item.IsActive
        };
    }

    private static void AddAuthorizedRows(MenuItem item, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys, IReadOnlyList<MenuItem> ancestors, int depth, List<NavigationMenuRow> rows)
    {
        if (!IsAuthorized(item, user, licensedBusinessLineKeys))
        {
            return;
        }

        var path = ancestors.Concat([item]).ToList();
        rows.Add(new NavigationMenuRow(item, depth, path));

        foreach (var child in item.Children)
        {
            AddAuthorizedRows(child, user, licensedBusinessLineKeys, path, depth + 1, rows);
        }
    }

    private static void AddPanelRows(MenuItem item, IReadOnlyList<MenuItem> ancestors, int depth, List<NavigationMenuRow> rows)
    {
        var path = ancestors.Concat([item]).ToList();
        rows.Add(new NavigationMenuRow(item, depth, path));

        foreach (var child in item.Children)
        {
            AddPanelRows(child, path, depth + 1, rows);
        }
    }

    private static IReadOnlyList<MenuItem> BuildJourneyPanelSections(
        ClaimsPrincipal? user,
        string workspaceKey,
        IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        var items = GetAuthorizedRows(user, null, licensedBusinessLineKeys)
            .Where(row => !string.IsNullOrWhiteSpace(row.Item.Url)
                          && HasOwnPermission(row.Item, user, licensedBusinessLineKeys)
                          && RowBelongsToWorkspace(row.Item, workspaceKey, user, licensedBusinessLineKeys))
            .Select(row => row.Item)
            .GroupBy(GetStorageKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderBy(GetNavigationJourneyOrder)
                .ThenBy(item => GetMenuPath(item, workspaceKey).Count)
                .First())
            .ToList();

        if (items.Count == 0)
        {
            return [];
        }

        return NavigationJourneyGroups
            .Select(group =>
            {
                var children = items
                    .Where(item => ResolveNavigationGroupKey(item) == group.Key)
                    .OrderBy(GetNavigationJourneyOrder)
                    .ThenBy(item => item.TextEn)
                    .Select(item => ClonePanelItem(item, []))
                    .ToList();

                return children.Count == 0 ? null : CreateJourneySection(group, workspaceKey, children);
            })
            .Where(section => section is not null)
            .Cast<MenuItem>()
            .ToList();
    }

    private static MenuItem CreateJourneySection(NavigationJourneyGroup group, string workspaceKey, List<MenuItem> children)
        => new()
        {
            TextEn = group.TextEn,
            TextAr = group.TextAr,
            Icon = group.Icon,
            PermissionPolicy = string.Empty,
            WorkspaceKey = workspaceKey,
            NavigationGroupKey = group.Key,
            NavigationOrder = group.Order,
            IsFavoriteCandidate = false,
            Children = children
        };

    private static IReadOnlyList<MenuItem> GetWorkspacePanelSections(IReadOnlyList<MenuItem> roots, string workspaceKey)
        => workspaceKey switch
        {
            WorkspaceHome => FindSections(roots, "Control Panel"),
            WorkspacePos => FindSections(roots, "POS"),
            WorkspaceSales => FindSections(roots, "Sales Management"),
            WorkspaceHr => FindChildSections(roots, "People", "Human Resource", "Attendance", "Leave Management", "Payroll"),
            WorkspacePurchasing => FindChildSections(roots, "Operations", "Supplier Management", "Procurement"),
            WorkspaceRealEstate => FindSections(roots, "Real Estate"),
            WorkspaceCatering => FindSections(roots, "Catering"),
            WorkspaceWarehouse => FindChildSections(roots, "Operations", "Products Management", "Inventory Management"),
            WorkspaceAccountingFinance => FindSections(roots, "Accounting"),
            WorkspaceAdmin => FindSections(roots,
                "Organizational Structure",
                "Contracts",
                "Document Management",
                "Project Management",
                "Task Management",
                "Platform Operations",
                "General Settings"),
            WorkspaceSecurity => FindSections(roots, "Security Management"),
            _ => []
        };

    private static IReadOnlyList<MenuItem> FindSections(IEnumerable<MenuItem> roots, params string[] texts)
    {
        var sections = new List<MenuItem>();
        foreach (var text in texts)
        {
            var section = FindByText(roots, text);
            if (section is not null)
            {
                sections.Add(section);
            }
        }

        return sections;
    }

    private static IReadOnlyList<MenuItem> FindChildSections(IEnumerable<MenuItem> roots, string parentText, params string[] childTexts)
    {
        var parent = FindByText(roots, parentText);
        return parent is null ? [] : FindSections(parent.Children, childTexts);
    }

    private static MenuItem? FindByText(IEnumerable<MenuItem> items, string text)
    {
        foreach (var item in items)
        {
            if (IsText(item, text))
            {
                return item;
            }

            var child = FindByText(item.Children, text);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private static MenuItem? FilterWorkspacePanelSection(MenuItem item, string workspaceKey)
    {
        var children = item.Children
            .Select(child => FilterWorkspacePanelChild(child, workspaceKey))
            .Where(child => child is not null)
            .Cast<MenuItem>()
            .ToList();

        return ClonePanelItem(item, children);
    }

    private static MenuItem? FilterWorkspacePanelChild(MenuItem item, string workspaceKey)
    {
        var children = item.Children
            .Select(child => FilterWorkspacePanelChild(child, workspaceKey))
            .Where(child => child is not null)
            .Cast<MenuItem>()
            .ToList();

        if (GetWorkspaceKey(item) != workspaceKey && children.Count == 0)
        {
            return null;
        }

        return ClonePanelItem(item, children);
    }

    private static MenuItem ClonePanelItem(MenuItem item, List<MenuItem> children)
        => new()
        {
            TextAr = item.TextAr,
            TextEn = item.TextEn,
            PermissionPolicy = item.PermissionPolicy,
            Icon = item.Icon,
            Url = item.Url,
            BadgeText = item.BadgeText,
            BadgeCssClass = item.BadgeCssClass,
            BadgeTitleEn = item.BadgeTitleEn,
            BadgeTitleAr = item.BadgeTitleAr,
            WorkspaceKey = item.WorkspaceKey,
            BusinessLineKey = item.BusinessLineKey,
            NavigationGroupKey = item.NavigationGroupKey,
            NavigationOrder = item.NavigationOrder,
            ProcessKey = item.ProcessKey,
            NavigationAliases = item.NavigationAliases.ToList(),
            MobilePriority = item.MobilePriority,
            KeywordsEn = item.KeywordsEn,
            KeywordsAr = item.KeywordsAr,
            IsFavoriteCandidate = item.IsFavoriteCandidate,
            Children = children,
            IsOpen = item.IsOpen,
            IsActive = item.IsActive
        };

    private static List<MenuItem> FindPath(MenuItem current, MenuItem target)
    {
        if (ReferenceEquals(current, target) || GetStorageKey(current) == GetStorageKey(target))
        {
            return [current];
        }

        foreach (var child in current.Children)
        {
            var childPath = FindPath(child, target);
            if (childPath.Count > 0)
            {
                var result = new List<MenuItem> { current };
                result.AddRange(childPath);
                return result;
            }
        }

        return [];
    }

    private static void AddHubSections(MenuItem root, ClaimsPrincipal? user, string hubWorkspaceKey, List<NavigationHubSection> sections)
    {
        if (IsText(root, "People") || IsText(root, "Operations") || IsText(root, "Platform Operations"))
        {
            foreach (var child in root.Children)
            {
                AddHubSection(child, user, hubWorkspaceKey, sections);
            }

            return;
        }

        if (IsText(root, "Security Management"))
        {
            if (hubWorkspaceKey == HubSecurity)
            {
                AddHubSection(root, user, hubWorkspaceKey, sections);
            }

            return;
        }

        AddHubSection(root, user, hubWorkspaceKey, sections);
    }

    private static void AddHubSection(MenuItem section, ClaimsPrincipal? user, string hubWorkspaceKey, List<NavigationHubSection> sections)
    {
        var source = !string.IsNullOrWhiteSpace(section.Url) ? [section] : section.Children;
        var rows = GetAuthorizedRows(user, source)
            .Where(row => RowBelongsToWorkspace(row.Item, hubWorkspaceKey, user))
            .ToList();
        if (rows.Count == 0)
        {
            return;
        }

        sections.Add(new NavigationHubSection(GetStorageKey(section), section.TextEn, section.TextAr, section.Icon, hubWorkspaceKey, rows));
    }

    private static bool IsWorkspaceAvailable(NavigationWorkspace workspace, ClaimsPrincipal? user)
        => IsWorkspaceAvailable(workspace, user, null);

    private static bool IsWorkspaceAvailable(NavigationWorkspace workspace, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        if (workspace.Key == WorkspaceHome)
        {
            return true;
        }

        var authorizedItems = GetAuthorizedNavigableItems(user, licensedBusinessLineKeys).ToList();
        if (workspace.Key == WorkspaceMore)
        {
            return authorizedItems.Count > 0;
        }

        if (!string.IsNullOrWhiteSpace(workspace.Url)
            && authorizedItems.Any(item => NormalizePath(item.Url) == NormalizePath(workspace.Url)))
        {
            return true;
        }

        return authorizedItems.Any(item => GetWorkspaceKey(item) == workspace.Key);
    }

    private static bool RowBelongsToWorkspace(MenuItem item, string workspaceKey, ClaimsPrincipal? user)
        => RowBelongsToWorkspace(item, workspaceKey, user, null);

    private static bool RowBelongsToWorkspace(MenuItem item, string workspaceKey, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
    {
        if (!IsAuthorized(item, user, licensedBusinessLineKeys))
        {
            return false;
        }

        var itemWorkspaceKey = GetWorkspaceKey(item);
        if (itemWorkspaceKey == workspaceKey
            || workspaceKey == WorkspaceSales && itemWorkspaceKey == WorkspacePos)
        {
            return true;
        }

        return item.Children.Any(child => RowBelongsToWorkspace(child, workspaceKey, user, licensedBusinessLineKeys));
    }

    private static bool PathBelongsToWorkspace(IReadOnlyList<MenuItem> path, string workspaceKey)
        => path.Any(item => string.Equals(GetWorkspaceKey(item), workspaceKey, StringComparison.OrdinalIgnoreCase));

    private static bool ItemCanNavigateWithinWorkspace(MenuItem item, string workspaceKey)
    {
        var itemWorkspaceKey = GetWorkspaceKey(item);
        if (string.Equals(itemWorkspaceKey, workspaceKey, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(item.Url))
        {
            return false;
        }

        var itemPath = NormalizePath(item.Url);
        return Flatten(MenuItem.Menu)
            .Any(candidate =>
                string.Equals(GetWorkspaceKey(candidate), workspaceKey, StringComparison.OrdinalIgnoreCase)
                && IsPathWithinItem(itemPath, candidate));
    }

    private static bool TryFindInheritedWorkspaceKey(IEnumerable<MenuItem> items, MenuItem target, string? inheritedWorkspaceKey, out string workspaceKey)
    {
        foreach (var item in items)
        {
            var currentWorkspaceKey = !string.IsNullOrWhiteSpace(item.WorkspaceKey)
                ? item.WorkspaceKey
                : inheritedWorkspaceKey;

            if (IsSameMenuIdentity(item, target) && !string.IsNullOrWhiteSpace(currentWorkspaceKey))
            {
                workspaceKey = currentWorkspaceKey;
                return true;
            }

            if (TryFindInheritedWorkspaceKey(item.Children, target, currentWorkspaceKey, out workspaceKey))
            {
                return true;
            }
        }

        workspaceKey = string.Empty;
        return false;
    }

    private static bool IsSameMenuIdentity(MenuItem first, MenuItem second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(first.Url) && !string.IsNullOrWhiteSpace(second.Url))
        {
            return NormalizePath(first.Url) == NormalizePath(second.Url);
        }

        return string.Equals(first.TextEn, second.TextEn, StringComparison.OrdinalIgnoreCase)
               && string.Equals(first.TextAr, second.TextAr, StringComparison.OrdinalIgnoreCase)
               && string.Equals(first.Icon, second.Icon, StringComparison.OrdinalIgnoreCase)
               && string.Equals(first.PermissionPolicy, second.PermissionPolicy, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsText(MenuItem item, string text)
        => item.TextEn.Equals(text, StringComparison.OrdinalIgnoreCase);

    private static bool TryResolveWorkspaceMatch(string currentPath, string? preferredWorkspaceKey, out string workspaceKey)
    {
        workspaceKey = string.Empty;
        if (string.IsNullOrWhiteSpace(preferredWorkspaceKey))
        {
            return false;
        }

        var match = Flatten(MenuItem.Menu)
            .FirstOrDefault(item =>
                IsPathWithinItem(currentPath, item)
                && string.Equals(GetWorkspaceKey(item), preferredWorkspaceKey, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            return false;
        }

        workspaceKey = GetWorkspaceKey(match);
        return true;
    }

    private static bool TryResolveExplicitExactWorkspaceMatch(string currentPath, out string workspaceKey)
    {
        workspaceKey = string.Empty;
        var match = Flatten(MenuItem.Menu)
            .FirstOrDefault(item =>
                !string.IsNullOrWhiteSpace(item.Url)
                && !string.IsNullOrWhiteSpace(item.WorkspaceKey)
                && NormalizePath(item.Url) == currentPath);

        if (match is null)
        {
            return false;
        }

        workspaceKey = GetWorkspaceKey(match);
        return true;
    }

    private static bool IsJourneyStart(string path, string text)
        => path == "/dashboard"
           || path.EndsWith("/dashboard", StringComparison.OrdinalIgnoreCase)
           || ContainsNavigationAny(text, " dashboard ", " command center ", " overview ", " pos ", "\u0644\u0648\u062d\u0629");

    private static bool IsJourneyReport(string path, string text)
        => path.Contains("/reports", StringComparison.OrdinalIgnoreCase)
           || ContainsNavigationAny(text, " report ", " reports ", " analytics ", " scorecard ", "\u062a\u0642\u0631\u064a\u0631", "\u062a\u0642\u0627\u0631\u064a\u0631");

    private static bool IsJourneyApproval(string path, string text)
        => path.Contains("/approve", StringComparison.OrdinalIgnoreCase)
           || ContainsNavigationAny(text, " approve ", " approval ", " review ", " late requests ", "\u0627\u0639\u062a\u0645\u0627\u062f", "\u0645\u0631\u0627\u062c\u0639\u0629");

    private static bool IsJourneySetup(string path, string text)
        => ContainsNavigationAny(text,
            " setup ",
            " setting ",
            " settings ",
            " configuration ",
            " configure ",
            " template ",
            " templates ",
            " policy ",
            " policies ",
            " structure ",
            " structures ",
            " component ",
            " components ",
            " position ",
            " positions ",
            " specialization ",
            " specializations ",
            " academic institution ",
            " academic institutions ",
            " shift ",
            " shifts ",
            " holiday ",
            " holidays ",
            " pricing list ",
            " fiscal ",
            " tax ",
            " posting ",
            " profile ",
            " profiles ",
            " chart of accounts ",
            " controls ",
            " rule ",
            " rules ",
            "\u0625\u0639\u062f\u0627\u062f",
            "\u0627\u0644\u0625\u0639\u062f\u0627\u062f\u0627\u062a",
            "\u0642\u0627\u0644\u0628",
            "\u0642\u0648\u0627\u0644\u0628",
            "\u0633\u064a\u0627\u0633\u0627\u062a",
            "\u0636\u0648\u0627\u0628\u0637");

    private static bool IsJourneyAdministration(string path, string text)
        => path.StartsWith("/auth", StringComparison.OrdinalIgnoreCase)
           || path.StartsWith("/generalsettings", StringComparison.OrdinalIgnoreCase)
           || ContainsNavigationAny(text, " role ", " roles ", " permission ", " permissions ", " assign user roles ", " system settings ", " administration ");

    private static bool IsJourneyAdjustment(string path, string text)
        => ContainsNavigationAny(text,
            " adjustment ",
            " adjustments ",
            " return ",
            " returns ",
            " credit note ",
            " credit notes ",
            " debit note ",
            " debit notes ",
            " reconciliation ",
            " exception ",
            " exceptions ",
            "\u062a\u0633\u0648\u064a\u0627\u062a",
            "\u0645\u0631\u062a\u062c\u0639",
            "\u0625\u0634\u0639\u0627\u0631");

    private static bool IsJourneyMasterData(string path, string text)
        => ContainsNavigationAny(text,
            " employee ",
            " employees ",
            " team ",
            " teams ",
            " document ",
            " documents ",
            " emergency contacts ",
            " skills ",
            " certifications ",
            " contract ",
            " contracts ",
            " customer ",
            " customers ",
            " supplier ",
            " suppliers ",
            " product ",
            " products ",
            " sku ",
            " brand ",
            " category ",
            " categories ",
            " unit ",
            " units ",
            " warehouse ",
            " warehouses ",
            " current stock ",
            " batch ",
            " batches ",
            " account ",
            " accounts ",
            " bank cash accounts ",
            " vehicle ",
            " vehicles ",
            " asset ",
            " assets ",
            " property ",
            " properties ",
            "\u0645\u0648\u0638\u0641",
            "\u0645\u0648\u0638\u0641\u064a\u0646",
            "\u0639\u0645\u064a\u0644",
            "\u0639\u0645\u0644\u0627\u0621",
            "\u0645\u0648\u0631\u062f",
            "\u0645\u0648\u0631\u062f\u0648\u0646",
            "\u0645\u0646\u062a\u062c",
            "\u0645\u0646\u062a\u062c\u0627\u062a",
            "\u0645\u0633\u062a\u0646\u062f\u0627\u062a",
            "\u0645\u062e\u0632\u0648\u0646");

    private static string GetNavigationHaystack(MenuItem item)
        => $" {item.TextEn} {item.TextAr} {item.Url} {item.KeywordsEn} {item.KeywordsAr} {item.PermissionPolicy} "
            .ToLowerInvariant()
            .Replace('-', ' ')
            .Replace('/', ' ');

    private static bool ContainsNavigationAny(string value, params string[] terms)
        => terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static readonly IReadOnlyList<NavigationJourneyGroup> NavigationJourneyGroups =
    [
        new(NavigationGroupStart, "Start / Overview", "\u0627\u0644\u0628\u062f\u0621 / \u0646\u0638\u0631\u0629 \u0639\u0627\u0645\u0629", "bi-speedometer2", 0),
        new(NavigationGroupSetup, "Setup", "\u0627\u0644\u0625\u0639\u062f\u0627\u062f", "bi-sliders2", 1),
        new(NavigationGroupMasterData, "Master Data", "\u0627\u0644\u0628\u064a\u0627\u0646\u0627\u062a \u0627\u0644\u0623\u0633\u0627\u0633\u064a\u0629", "bi-collection", 2),
        new(NavigationGroupDailyWork, "Daily Work", "\u0627\u0644\u0639\u0645\u0644 \u0627\u0644\u064a\u0648\u0645\u064a", "bi-arrow-left-right", 3),
        new(NavigationGroupApprovals, "Approvals", "\u0627\u0644\u0627\u0639\u062a\u0645\u0627\u062f\u0627\u062a", "bi-patch-check", 4),
        new(NavigationGroupAdjustments, "Adjustments / Exceptions", "\u0627\u0644\u062a\u0633\u0648\u064a\u0627\u062a / \u0627\u0644\u0627\u0633\u062a\u062b\u0646\u0627\u0621\u0627\u062a", "bi-sliders", 5),
        new(NavigationGroupReports, "Reports", "\u0627\u0644\u062a\u0642\u0627\u0631\u064a\u0631", "bi-bar-chart-line", 6),
        new(NavigationGroupAdministration, "Administration", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629", "bi-shield-lock", 7)
    ];

    private static readonly IReadOnlyDictionary<string, int> JourneyPathPriorities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["/employee/dashboard"] = 0,
        ["/hr/commandcenter"] = 1,
        ["/attendance/dashboard"] = 2,
        ["/sales/dashboard"] = 3,
        ["/procurement/dashboard"] = 4,
        ["/inventory/dashboard"] = 5,
        ["/accounting/dashboard"] = 6,
        ["/auth/dashboard"] = 7,

        ["/employee/position/list"] = 100,
        ["/employee/teams"] = 101,
        ["/employee/academicinistitution/list"] = 102,
        ["/employee/specialization/list"] = 103,
        ["/attendance/configuration"] = 110,
        ["/attendance/shifts"] = 111,
        ["/attendance/shiftassignments"] = 112,
        ["/attendance/holidays"] = 113,
        ["/hr/leavepolicies"] = 120,
        ["/payroll/components"] = 130,
        ["/hr/payrollstructures"] = 131,

        ["/employee/employee/list"] = 200,
        ["/hr/employeelifecycle"] = 201,
        ["/hr/employeedocuments"] = 202,
        ["/hr/employeeemergencycontacts"] = 203,
        ["/hr/employeeskills"] = 204,
        ["/payroll/contracts"] = 230,
        ["/payroll/assigncontract"] = 231,

        ["/attendance/myattendance"] = 300,
        ["/attendance/sessions"] = 301,
        ["/hr/attendanceroster"] = 302,
        ["/hr/workentries"] = 303,
        ["/hr/leaveapplications"] = 320,
        ["/leavesmanagement/emergencyleaves"] = 321,
        ["/leavesmanagement/balances"] = 322,
        ["/leavesmanagement/ledger"] = 323,
        ["/hr/leaveledger"] = 324,
        ["/payroll/salaryruns"] = 330,
        ["/hr/payslips"] = 331,
        ["/hr/saudipayroll"] = 332,
        ["/payroll/loans"] = 333,

        ["/attendance/laterequests"] = 400,
        ["/attendance/approvepermissionrequests"] = 401,
        ["/leavesmanagement/approveemergencyleaves"] = 402,

        ["/hr/reports"] = 600,
        ["/attendance/reports"] = 601,
        ["/leavesmanagement/reports"] = 602
    };
}

public sealed record NavigationWorkspace(string Key, string TextEn, string TextAr, string Icon, string? Url);
public sealed record NavigationHubWorkspace(string Key, string TextEn, string TextAr, string Icon);
public sealed record NavigationHubSection(string Key, string TextEn, string TextAr, string Icon, string WorkspaceKey, IReadOnlyList<NavigationMenuRow> Rows);
public sealed record NavigationMenuRow(MenuItem Item, int Depth, IReadOnlyList<MenuItem> Path);
public sealed record NavigationJourneyGroup(string Key, string TextEn, string TextAr, string Icon, int Order);
