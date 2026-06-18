using System.Security.Claims;

namespace AlAfkarERP.Shared.Layout;

public static class NavigationMenuResolver
{
    public const string WorkspaceHome = "home";
    public const string WorkspacePeople = "people";
    public const string WorkspaceOperations = "operations";
    public const string WorkspaceFinancePayroll = "finance-payroll";
    public const string WorkspaceInventory = "inventory";
    public const string WorkspaceMore = "more";
    public const string WorkspacePos = "pos";
    public const string WorkspaceProcurement = WorkspaceOperations;

    public const string HubPeople = "people";
    public const string HubOperations = "operations";
    public const string HubTasks = "tasks";
    public const string HubSecurity = "security";
    public const string HubSettings = "settings";

    public static readonly IReadOnlyList<NavigationWorkspace> MobileWorkspaces =
    [
        new(WorkspaceHome, "Home", "\u0627\u0644\u0631\u0626\u064a\u0633\u064a\u0629", "bi-house-door", "/Dashboard"),
        new(WorkspacePeople, "People", "\u0627\u0644\u0623\u0641\u0631\u0627\u062f", "bi-people", "/Employee/Dashboard"),
        new(WorkspacePos, "POS", "\u0646\u0642\u0637\u0629 \u0628\u064a\u0639", "bi-receipt-cutoff", "/SalesOrder/POS"),
        new(WorkspaceOperations, "Operations", "\u0627\u0644\u0639\u0645\u0644\u064a\u0627\u062a", "bi-kanban", null),
        new(WorkspaceFinancePayroll, "Finance / Payroll", "\u0627\u0644\u0645\u0627\u0644\u064a\u0629 / \u0627\u0644\u0631\u0648\u0627\u062a\u0628", "bi-cash-stack", "/Payroll/SalaryRuns"),
        new(WorkspaceInventory, "Inventory", "\u0627\u0644\u0645\u062e\u0632\u0648\u0646", "bi-boxes", "/Inventory/Dashboard"),
        new(WorkspaceMore, "More", "\u0627\u0644\u0645\u0632\u064a\u062f", "bi-grid-3x3-gap", null)
    ];

    public static readonly IReadOnlyList<NavigationHubWorkspace> HubWorkspaces =
    [
        new(HubPeople, "People", "\u0627\u0644\u0623\u0641\u0631\u0627\u062f", "bi-people"),
        new(HubOperations, "Operations", "\u0627\u0644\u0639\u0645\u0644\u064a\u0627\u062a", "bi-box-seam"),
        new(HubTasks, "Tasks", "\u0627\u0644\u0645\u0647\u0627\u0645", "bi-check2-square"),
        new(HubSecurity, "Security", "\u0627\u0644\u0623\u0645\u0627\u0646", "bi-shield-lock"),
        new(HubSettings, "Settings", "\u0627\u0644\u0625\u0639\u062f\u0627\u062f\u0627\u062a", "bi-gear")
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

        if (path.StartsWith("/payroll", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/pricing", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/catalog/pricing", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Payroll", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Salary", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Finance", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Pricing", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceFinancePayroll;
        }

        if (path.StartsWith("/inventory", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/inventories", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/warehouse", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Inventory", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Stock", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Warehouse", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Batch", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceInventory;
        }

        if (path.StartsWith("/organization", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/employee", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/attendance", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/leavesmanagement", StringComparison.OrdinalIgnoreCase)
            || text.Contains("People", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Human Resource", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Employee", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Attendance", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Leave", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Company", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Branch", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Department", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Administration", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspacePeople;
        }

        if (path.StartsWith("/salesorder", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/procurement", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/suppliers", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/customers", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/catalog", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Operations", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Procurement", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Purchase", StringComparison.OrdinalIgnoreCase)
            || text.Contains("RFQ", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Supplier", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Customer", StringComparison.OrdinalIgnoreCase)
            || text.Contains("Product", StringComparison.OrdinalIgnoreCase))
        {
            return WorkspaceOperations;
        }

        return WorkspaceMore;
    }

    public static string ResolveActiveWorkspace(string currentUri, string baseUri)
    {
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
        if (workspaceKey == WorkspaceMore)
        {
            return null;
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
        return activePath.Count == 0 ? HubPeople : ResolveHubWorkspaceKey(activePath.Last());
    }

    public static string ResolveHubWorkspaceKey(MenuItem item)
    {
        var path = GetMenuPath(item);
        if (path.Count == 0)
        {
            path = [item];
        }

        if (path.Any(IsSettingsMenu))
        {
            return HubSettings;
        }

        if (path.Any(candidate => IsText(candidate, "Task Management")))
        {
            return HubTasks;
        }

        if (path.Any(candidate => IsText(candidate, "Security Management")))
        {
            return HubSecurity;
        }

        if (path.Any(candidate => IsText(candidate, "Operations")
                                  || IsText(candidate, "POS")
                                  || IsText(candidate, "Products Management")
                                  || IsText(candidate, "Inventory Management")
                                  || IsText(candidate, "Supplier Management")
                                  || IsText(candidate, "Procurement")))
        {
            return HubOperations;
        }

        return HubPeople;
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
        => !string.IsNullOrWhiteSpace(item.PermissionPolicy)
           && user?.Claims.Any(claim => claim.Value == item.PermissionPolicy) == true;

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
            "/salesorder/pos" => 1,
            "/inventories/list" => 2,
            "/inventory/operations/stockin" => 3,
            "/inventory/operations/stockout" => 4,
            "/inventory/operations/stockadjustment" => 5,
            "/inventory/operations/stockreservation" => 6,
            "/inventory/operations/stockrelease" => 7,
            "/inventory/warehouse/list" => 8,
            "/inventory/batch/list" => 9,
            "/suppliers/supplier/list" => 10,
            "/procurement/purchase-requests" => 11,
            "/procurement/purchase-orders" => 12,
            "/procurement/goods-receipts" => 13,
            "/procurement/supplier-invoices" => 14,
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
        if (IsText(root, "People") || IsText(root, "Operations"))
        {
            foreach (var child in root.Children)
            {
                AddHubSection(child, user, hubWorkspaceKey, sections);
            }

            return;
        }

        if (IsText(root, "Security Management"))
        {
            AddSecuritySections(root, user, hubWorkspaceKey, sections);
            return;
        }

        AddHubSection(root, user, hubWorkspaceKey, sections);
    }

    private static void AddHubSection(MenuItem section, ClaimsPrincipal? user, string hubWorkspaceKey, List<NavigationHubSection> sections)
    {
        var workspaceKey = ResolveHubWorkspaceKey(section);
        if (workspaceKey != hubWorkspaceKey)
        {
            return;
        }

        var source = !string.IsNullOrWhiteSpace(section.Url) ? [section] : section.Children;
        var rows = GetAuthorizedRows(user, source);
        if (rows.Count == 0)
        {
            return;
        }

        sections.Add(new NavigationHubSection(GetStorageKey(section), section.TextEn, section.TextAr, section.Icon, workspaceKey, rows));
    }

    private static void AddSecuritySections(MenuItem section, ClaimsPrincipal? user, string hubWorkspaceKey, List<NavigationHubSection> sections)
    {
        var securityRows = new List<NavigationMenuRow>();
        if (hubWorkspaceKey == HubSecurity && HasOwnPermission(section, user) && !string.IsNullOrWhiteSpace(section.Url))
        {
            securityRows.Add(new NavigationMenuRow(section, 0, [section]));
        }

        if (hubWorkspaceKey == HubSecurity)
        {
            securityRows.AddRange(GetAuthorizedRows(user, section.Children.Where(item => !IsSettingsMenu(item))));
            if (securityRows.Count > 0)
            {
                sections.Add(new NavigationHubSection(GetStorageKey(section), section.TextEn, section.TextAr, section.Icon, HubSecurity, securityRows));
            }
        }

        if (hubWorkspaceKey == HubSettings)
        {
            var settingsRows = GetAuthorizedRows(user, section.Children.Where(IsSettingsMenu));
            if (settingsRows.Count > 0)
            {
                sections.Add(new NavigationHubSection("settings", "Settings", "\u0627\u0644\u0625\u0639\u062f\u0627\u062f\u0627\u062a", "bi-gear", HubSettings, settingsRows));
            }
        }
    }

    private static bool IsSettingsMenu(MenuItem item)
        => IsText(item, "System Settings")
           || IsText(item, "Currencies")
           || NormalizePath(item.Url).StartsWith("/generalsettings", StringComparison.OrdinalIgnoreCase);

    private static bool IsWorkspaceAvailable(NavigationWorkspace workspace, ClaimsPrincipal? user)
    {
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

        if (GetWorkspaceKey(item) == workspaceKey)
        {
            return true;
        }

        return item.Children.Any(child => RowBelongsToWorkspace(child, workspaceKey, user));
    }

    private static bool IsText(MenuItem item, string text)
        => item.TextEn.Equals(text, StringComparison.OrdinalIgnoreCase);
}

public sealed record NavigationWorkspace(string Key, string TextEn, string TextAr, string Icon, string? Url);
public sealed record NavigationHubWorkspace(string Key, string TextEn, string TextAr, string Icon);
public sealed record NavigationHubSection(string Key, string TextEn, string TextAr, string Icon, string WorkspaceKey, IReadOnlyList<NavigationMenuRow> Rows);
public sealed record NavigationMenuRow(MenuItem Item, int Depth, IReadOnlyList<MenuItem> Path);
