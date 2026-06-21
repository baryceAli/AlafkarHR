using System.Security.Claims;
using AlAfkarERP.Shared.Utilities;

namespace AlAfkarERP.Shared.Layout;

public static class NavigationMenuResolver
{
    public const string WorkspaceHome = "home";
    public const string WorkspaceHr = "hr";
    public const string WorkspaceSales = "sales";
    public const string WorkspacePurchasing = "purchasing";
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

    public const string HubHr = WorkspaceHr;
    public const string HubSales = WorkspaceSales;
    public const string HubPurchasing = WorkspacePurchasing;
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
        new(HubWarehouse, "Warehouse", "\u0627\u0644\u0645\u0633\u062a\u0648\u062f\u0639", "bi-boxes"),
        new(HubAccountingFinance, "Accounting / Finance", "\u0627\u0644\u0645\u062d\u0627\u0633\u0628\u0629 / \u0627\u0644\u0645\u0627\u0644\u064a\u0629", "bi-cash-stack"),
        new(HubAdmin, "Admin", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629", "bi-sliders2-vertical"),
        new(HubSecurity, "IT / Security", "\u062a\u0642\u0646\u064a\u0629 \u0648\u0623\u0645\u0627\u0646", "bi-shield-lock")
    ];

    public static IReadOnlyList<MenuItem> GetActivePath(string currentUri, string baseUri)
    {
        var currentPath = NormalizePath(ToRelativePath(currentUri, baseUri));
        var bestMatch = new List<MenuItem>();

        foreach (var item in MenuItem.Menu)
        {
            var path = FindBestPath(item, currentPath);
            if (path.Count > bestMatch.Count)
            {
                bestMatch = path;
            }
        }

        return bestMatch;
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

        var path = NormalizePath(item.Url);
        var text = item.TextEn;

        if (path == "/" || path == "/dashboard" || text.Equals("Home", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceHome;
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

        if (path.StartsWith("/generalsettings", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/contracts", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/documentmanagement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/fleet", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/realestate", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/maintenance", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/organization", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/projectmanagement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/taskmanagement", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Contract", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Document", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Fleet", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Real Estate", StringComparison.OrdinalIgnoreCase)
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

        if (TryResolveExactWorkspaceMatch(currentPath, preferredWorkspaceKey, out var preferredWorkspace))
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

        var activePath = GetActivePath(currentUri, baseUri);
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
        => MenuItem.Menu
            .Select(item => FilterAuthorized(item, user))
            .Where(item => item is not null)
            .Cast<MenuItem>()
            .ToList();

    public static IReadOnlyList<MenuItem> GetAuthorizedNavigableItems(ClaimsPrincipal? user)
        => Flatten(MenuItem.Menu)
            .Where(item => HasOwnPermission(item, user) && !string.IsNullOrWhiteSpace(item.Url))
            .OrderBy(item => GetMobilePriority(item))
            .ThenBy(item => item.TextEn)
            .ToList();

    public static IReadOnlyList<NavigationWorkspace> GetAuthorizedWorkspaces(ClaimsPrincipal? user)
        => MobileWorkspaces
            .Where(workspace => IsWorkspaceAvailable(workspace, user))
            .ToList();

    public static string? GetWorkspaceDefaultUrl(string workspaceKey, ClaimsPrincipal? user)
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

        var authorizedItems = GetAuthorizedNavigableItems(user).ToList();
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
    {
        var activePath = GetActivePath(currentUri, baseUri);
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

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedRows(ClaimsPrincipal? user, IEnumerable<MenuItem>? source = null)
    {
        var rows = new List<NavigationMenuRow>();
        foreach (var item in source ?? MenuItem.Menu)
        {
            AddAuthorizedRows(item, user, [], 0, rows);
        }

        return rows;
    }

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspaceRows(ClaimsPrincipal? user, string workspaceKey)
        => GetAuthorizedRows(user)
            .Where(row => workspaceKey == WorkspaceMore || RowBelongsToWorkspace(row.Item, workspaceKey, user))
            .ToList();

    public static IReadOnlyList<NavigationMenuRow> GetAuthorizedWorkspacePanelRows(ClaimsPrincipal? user, string workspaceKey)
    {
        if (workspaceKey == WorkspaceMore)
        {
            return GetAuthorizedWorkspaceRows(user, workspaceKey);
        }

        var rows = new List<NavigationMenuRow>();
        var roots = GetAuthorizedTree(user);
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
    {
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
        => CompanyContext.HasPermission(user, item.PermissionPolicy);

    public static bool IsAuthorized(MenuItem item, ClaimsPrincipal? user)
        => HasOwnPermission(item, user) || item.Children.Any(child => IsAuthorized(child, user));

    public static bool HasAuthorizedChildren(MenuItem item, ClaimsPrincipal? user)
        => item.Children.Any(child => IsAuthorized(child, user));

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

    private static List<MenuItem> FindBestPath(MenuItem item, string currentPath)
        => FindBestPathCandidate(item, currentPath).Path;

    private static (List<MenuItem> Path, int Score, int Specificity) FindBestPathCandidate(MenuItem item, string currentPath)
    {
        var ownMatch = GetPathMatch(currentPath, item.Url);

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

    private static MenuItem? FilterAuthorized(MenuItem item, ClaimsPrincipal? user)
    {
        var children = item.Children
            .Select(child => FilterAuthorized(child, user))
            .Where(child => child is not null)
            .Cast<MenuItem>()
            .ToList();

        if (!HasOwnPermission(item, user) && children.Count == 0)
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
            MobilePriority = item.MobilePriority,
            KeywordsEn = item.KeywordsEn,
            KeywordsAr = item.KeywordsAr,
            IsFavoriteCandidate = item.IsFavoriteCandidate,
            Children = children,
            IsOpen = item.IsOpen,
            IsActive = item.IsActive
        };
    }

    private static void AddAuthorizedRows(MenuItem item, ClaimsPrincipal? user, IReadOnlyList<MenuItem> ancestors, int depth, List<NavigationMenuRow> rows)
    {
        if (!IsAuthorized(item, user))
        {
            return;
        }

        var path = ancestors.Concat([item]).ToList();
        rows.Add(new NavigationMenuRow(item, depth, path));

        foreach (var child in item.Children)
        {
            AddAuthorizedRows(child, user, path, depth + 1, rows);
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

    private static IReadOnlyList<MenuItem> GetWorkspacePanelSections(IReadOnlyList<MenuItem> roots, string workspaceKey)
        => workspaceKey switch
        {
            WorkspaceHome => FindSections(roots, "Control Panel"),
            WorkspacePos => FindSections(roots, "POS"),
            WorkspaceSales => FindSections(roots, "Sales Management"),
            WorkspaceHr => FindChildSections(roots, "People", "Human Resource", "Attendance", "Leave Management", "Payroll"),
            WorkspacePurchasing => FindChildSections(roots, "Operations", "Supplier Management", "Procurement"),
            WorkspaceWarehouse => FindChildSections(roots, "Operations", "Products Management", "Inventory Management"),
            WorkspaceAccountingFinance => [],
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
    {
        if (workspace.Key == WorkspaceHome)
        {
            return true;
        }

        var authorizedItems = GetAuthorizedNavigableItems(user).ToList();
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
    {
        if (!IsAuthorized(item, user))
        {
            return false;
        }

        var itemWorkspaceKey = GetWorkspaceKey(item);
        if (itemWorkspaceKey == workspaceKey
            || workspaceKey == WorkspaceSales && itemWorkspaceKey == WorkspacePos)
        {
            return true;
        }

        return item.Children.Any(child => RowBelongsToWorkspace(child, workspaceKey, user));
    }

    private static bool IsText(MenuItem item, string text)
        => item.TextEn.Equals(text, StringComparison.OrdinalIgnoreCase);

    private static bool TryResolveExactWorkspaceMatch(string currentPath, string? preferredWorkspaceKey, out string workspaceKey)
    {
        workspaceKey = string.Empty;
        if (string.IsNullOrWhiteSpace(preferredWorkspaceKey))
        {
            return false;
        }

        var match = Flatten(MenuItem.Menu)
            .FirstOrDefault(item =>
                !string.IsNullOrWhiteSpace(item.Url)
                && NormalizePath(item.Url) == currentPath
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
}

public sealed record NavigationWorkspace(string Key, string TextEn, string TextAr, string Icon, string? Url);
public sealed record NavigationHubWorkspace(string Key, string TextEn, string TextAr, string Icon);
public sealed record NavigationHubSection(string Key, string TextEn, string TextAr, string Icon, string WorkspaceKey, IReadOnlyList<NavigationMenuRow> Rows);
public sealed record NavigationMenuRow(MenuItem Item, int Depth, IReadOnlyList<MenuItem> Path);
