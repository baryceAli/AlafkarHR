using System.Security.Claims;

namespace AlAfkarERP.Shared.Layout;

public static partial class NavigationMenuResolver
{
    public static IReadOnlyList<EnterpriseNavigationGroup> GetEnterpriseNavigationGroups(
        ClaimsPrincipal? user,
        string workspaceKey,
        string? currentPath = null)
    {
        var resolvedWorkspaceKey = ResolveEnterpriseWorkspaceKey(workspaceKey);
        var normalizedCurrentPath = NormalizePath(currentPath);
        var entries = GetAuthorizedRows(user)
            .Where(row => !string.IsNullOrWhiteSpace(row.Item.Url) && HasOwnPermission(row.Item, user))
            .Select(row => GetEnterpriseNavigationEntry(row, resolvedWorkspaceKey, normalizedCurrentPath))
            .Where(entry => entry is not null)
            .Cast<EnterpriseNavigationEntry>()
            .GroupBy(entry => GetStorageKey(entry.Item), StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderBy(entry => entry.Priority)
                .ThenBy(entry => entry.Path.Count)
                .First())
            .ToList();

        return EnterpriseGroupDefinitions
            .Select(definition =>
            {
                var groupEntries = entries
                    .Where(entry => entry.Kind == definition.Kind)
                    .OrderBy(entry => entry.Priority)
                    .ThenBy(entry => GetMobilePriority(entry.Item))
                    .ThenBy(entry => entry.Item.TextEn)
                    .ToList();

                return new EnterpriseNavigationGroup(
                    $"{resolvedWorkspaceKey}:{definition.Kind}",
                    definition.Kind,
                    definition.TextEn,
                    definition.TextAr,
                    definition.Icon,
                    resolvedWorkspaceKey,
                    groupEntries);
            })
            .Where(group => group.Entries.Count > 0)
            .ToList();
    }

    public static EnterpriseNavigationEntry? GetEnterpriseNavigationEntry(
        NavigationMenuRow row,
        string workspaceKey,
        string? currentPath = null)
    {
        var itemWorkspaceKey = GetWorkspaceKey(row.Item);
        var resolvedWorkspaceKey = ResolveEnterpriseWorkspaceKey(workspaceKey);
        var isRelated = IsEnterpriseRelated(row.Item, resolvedWorkspaceKey, itemWorkspaceKey);
        if (!IsEnterpriseWorkspaceMatch(row.Item, resolvedWorkspaceKey, itemWorkspaceKey, isRelated))
        {
            return null;
        }

        var kind = isRelated
            ? EnterpriseNavigationGroupKind.Related
            : ResolveEnterpriseGroup(row.Item, row.Path);
        var priority = GetEnterprisePriority(row.Item, kind, resolvedWorkspaceKey, currentPath);
        return new EnterpriseNavigationEntry(
            row.Item,
            kind,
            priority,
            itemWorkspaceKey,
            isRelated,
            row.Depth,
            row.Path);
    }

    public static EnterpriseNavigationGroupKind ResolveEnterpriseGroup(MenuItem item, IReadOnlyList<MenuItem> path)
    {
        var text = GetEnterpriseHaystack(item, path);
        var normalizedPath = NormalizePath(item.Url);

        if (IsEnterpriseOverview(text, normalizedPath))
        {
            return EnterpriseNavigationGroupKind.Overview;
        }

        if (ContainsAny(text, " report ", " reports ", " analytics ", " analysis ", " distribution ", " cost report ")
            || normalizedPath.Contains("/reports", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("/report", StringComparison.OrdinalIgnoreCase))
        {
            return EnterpriseNavigationGroupKind.Reports;
        }

        if (ContainsAny(text,
                " setup ",
                " settings ",
                " setting ",
                " template ",
                " templates ",
                " fiscal ",
                " tax ",
                " posting ",
                " profile ",
                " profiles ",
                " role ",
                " roles ",
                " permission ",
                " default ",
                " defaults ",
                " rule ",
                " rules ",
                " currency ",
                " currencies",
                "\u0625\u0639\u062f\u0627\u062f",
                "\u0627\u0644\u0625\u0639\u062f\u0627\u062f\u0627\u062a",
                "\u0642\u0627\u0644\u0628",
                "\u0642\u0648\u0627\u0644\u0628"))
        {
            return EnterpriseNavigationGroupKind.Setup;
        }

        if (ContainsAny(text,
                " quotation ",
                " order ",
                " request ",
                " invoice ",
                " payment ",
                " receipt ",
                " delivery ",
                " return ",
                " credit ",
                " debit ",
                " stock in ",
                " stock out ",
                " attendance ",
                " leave ",
                " salary run ",
                " journal ",
                " reconciliation ",
                " work order ",
                " task ",
                " assignment ",
                " expense ",
                " lease ",
                " rent ",
                " utility ",
                " notification ",
                "\u0637\u0644\u0628",
                "\u0623\u0645\u0631",
                "\u0641\u0627\u062a\u0648\u0631",
                "\u0645\u062f\u0641\u0648\u0639",
                "\u0642\u064a\u062f",
                "\u0645\u0647\u0645",
                "\u062d\u0636\u0648\u0631",
                "\u0625\u062c\u0627\u0632"))
        {
            return EnterpriseNavigationGroupKind.Transactions;
        }

        return EnterpriseNavigationGroupKind.Documents;
    }

    public static int GetEnterprisePriority(
        MenuItem item,
        EnterpriseNavigationGroupKind group,
        string workspaceKey,
        string? currentPath = null)
    {
        var normalizedPath = NormalizePath(item.Url);
        var normalizedCurrentPath = NormalizePath(currentPath);
        if (normalizedCurrentPath != "/" && IsPathWithinItem(normalizedCurrentPath, item))
        {
            return 0;
        }

        if (group == EnterpriseNavigationGroupKind.Overview)
        {
            return 0;
        }

        var text = GetEnterpriseHaystack(item, []);
        if (ContainsAny(text,
                " pos ",
                " quotation ",
                " sales order ",
                " purchase request ",
                " purchase order ",
                " stock in ",
                " stock out ",
                " journal entries ",
                " salary runs ",
                " my tasks ",
                " my requests ")
            || normalizedPath is "/salesorder/pos")
        {
            return 10;
        }

        return group switch
        {
            EnterpriseNavigationGroupKind.Documents => 20,
            EnterpriseNavigationGroupKind.Transactions => 30,
            EnterpriseNavigationGroupKind.Reports => 40,
            EnterpriseNavigationGroupKind.Setup => 50,
            EnterpriseNavigationGroupKind.Related => 90,
            _ => 100
        };
    }

    public static string ResolveEnterpriseWorkspaceKey(string workspaceKey)
        => workspaceKey == WorkspacePos ? WorkspaceSales : workspaceKey;

    private static bool IsEnterpriseWorkspaceMatch(MenuItem item, string workspaceKey, string itemWorkspaceKey, bool isRelated)
        => workspaceKey == WorkspaceMore
           || itemWorkspaceKey == workspaceKey
           || workspaceKey == WorkspaceSales && itemWorkspaceKey == WorkspacePos
           || isRelated;

    private static bool IsEnterpriseRelated(MenuItem item, string workspaceKey, string itemWorkspaceKey)
    {
        if (workspaceKey == WorkspaceMore || itemWorkspaceKey == workspaceKey)
        {
            return false;
        }

        if (workspaceKey == WorkspaceSales && itemWorkspaceKey == WorkspacePos)
        {
            return false;
        }

        var aliases = item.NavigationAliases.Select(NormalizePath).ToList();
        return workspaceKey switch
        {
            WorkspaceSales => aliases.Any(alias =>
                alias.StartsWith("/sales", StringComparison.OrdinalIgnoreCase)
                || alias.StartsWith("/salesorder", StringComparison.OrdinalIgnoreCase)
                || alias.StartsWith("/orders", StringComparison.OrdinalIgnoreCase)
                || alias.StartsWith("/customers", StringComparison.OrdinalIgnoreCase)),
            WorkspacePurchasing => aliases.Any(alias =>
                alias.StartsWith("/procurement", StringComparison.OrdinalIgnoreCase)
                || alias.StartsWith("/suppliers", StringComparison.OrdinalIgnoreCase)),
            WorkspaceAccountingFinance => itemWorkspaceKey is WorkspaceSales or WorkspacePurchasing or WorkspacePos
                                          && ContainsAny(GetEnterpriseHaystack(item, []), " invoice ", " payment ", " receipt ", " credit ", " debit "),
            _ => false
        };
    }

    private static bool IsEnterpriseOverview(string text, string normalizedPath)
        => normalizedPath == "/dashboard"
           || normalizedPath.EndsWith("/dashboard", StringComparison.OrdinalIgnoreCase)
           || normalizedPath == "/salesorder/pos"
           || ContainsAny(text, " dashboard ", " overview ", " my tasks ", " my requests ", "\u0644\u0648\u062d\u0629");

    private static string GetEnterpriseHaystack(MenuItem item, IReadOnlyList<MenuItem> path)
    {
        var pathText = path.Count == 0
            ? ""
            : string.Join(" ", path.Select(candidate => $"{candidate.TextEn} {candidate.TextAr}"));
        return $" {item.TextEn} {item.TextAr} {item.Url} {item.KeywordsEn} {item.KeywordsAr} {item.PermissionPolicy} {pathText} "
            .ToLowerInvariant()
            .Replace('-', ' ')
            .Replace('/', ' ');
    }

    private static bool ContainsAny(string value, params string[] terms)
        => terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static readonly IReadOnlyList<EnterpriseGroupDefinition> EnterpriseGroupDefinitions =
    [
        new(EnterpriseNavigationGroupKind.Overview, "Overview", "\u0646\u0638\u0631\u0629 \u0639\u0627\u0645\u0629", "bi-speedometer2"),
        new(EnterpriseNavigationGroupKind.Documents, "Documents", "\u0645\u0633\u062a\u0646\u062f\u0627\u062a", "bi-folder2-open"),
        new(EnterpriseNavigationGroupKind.Transactions, "Transactions", "\u0645\u0639\u0627\u0645\u0644\u0627\u062a", "bi-arrow-left-right"),
        new(EnterpriseNavigationGroupKind.Reports, "Reports", "\u062a\u0642\u0627\u0631\u064a\u0631", "bi-bar-chart-line"),
        new(EnterpriseNavigationGroupKind.Setup, "Setup", "\u0625\u0639\u062f\u0627\u062f", "bi-sliders2"),
        new(EnterpriseNavigationGroupKind.Related, "Related", "\u0645\u0631\u062a\u0628\u0637", "bi-link-45deg")
    ];

    private sealed record EnterpriseGroupDefinition(
        EnterpriseNavigationGroupKind Kind,
        string TextEn,
        string TextAr,
        string Icon);
}

public enum EnterpriseNavigationGroupKind
{
    Overview,
    Documents,
    Transactions,
    Reports,
    Setup,
    Related
}

public sealed record EnterpriseNavigationGroup(
    string Key,
    EnterpriseNavigationGroupKind Kind,
    string TextEn,
    string TextAr,
    string Icon,
    string WorkspaceKey,
    IReadOnlyList<EnterpriseNavigationEntry> Entries);

public sealed record EnterpriseNavigationEntry(
    MenuItem Item,
    EnterpriseNavigationGroupKind Kind,
    int Priority,
    string WorkspaceKey,
    bool IsRelated,
    int Depth,
    IReadOnlyList<MenuItem> Path);
