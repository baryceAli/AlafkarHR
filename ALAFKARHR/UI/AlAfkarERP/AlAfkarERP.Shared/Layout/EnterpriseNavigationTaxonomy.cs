using System.Security.Claims;

namespace AlAfkarERP.Shared.Layout;

public static partial class NavigationMenuResolver
{
    public static IReadOnlyList<EnterpriseNavigationGroup> GetEnterpriseNavigationGroups(
        ClaimsPrincipal? user,
        string workspaceKey,
        string? currentPath = null)
        => GetEnterpriseNavigationGroups(user, workspaceKey, currentPath, null);

    public static IReadOnlyList<EnterpriseNavigationGroup> GetEnterpriseNavigationGroups(
        ClaimsPrincipal? user,
        string workspaceKey,
        string? currentPath,
        IReadOnlySet<string>? licensedBusinessLineKeys)
        => GetEnterpriseNavigationGroups(user, workspaceKey, currentPath, licensedBusinessLineKeys, null);

    public static IReadOnlyList<EnterpriseNavigationGroup> GetEnterpriseNavigationGroups(
        ClaimsPrincipal? user,
        string workspaceKey,
        string? currentPath,
        IReadOnlySet<string>? licensedBusinessLineKeys,
        string? functionalGroupKey)
    {
        var resolvedWorkspaceKey = ResolveEnterpriseWorkspaceKey(workspaceKey);
        var normalizedCurrentPath = NormalizePath(currentPath);
        var entries = GetAuthorizedRows(user, null, licensedBusinessLineKeys)
            .Where(row => !string.IsNullOrWhiteSpace(row.Item.Url) && HasOwnPermission(row.Item, user, licensedBusinessLineKeys))
            .Where(row => FunctionalGroupMatches(row.Item, functionalGroupKey))
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
        return ToEnterpriseGroupKind(ResolveNavigationGroupKey(item));
    }

    public static int GetEnterprisePriority(
        MenuItem item,
        EnterpriseNavigationGroupKind group,
        string workspaceKey,
        string? currentPath = null)
    {
        var normalizedCurrentPath = NormalizePath(currentPath);
        if (normalizedCurrentPath != "/" && IsPathWithinItem(normalizedCurrentPath, item))
        {
            return 0;
        }

        return group == EnterpriseNavigationGroupKind.Related
            ? 9000 + GetMobilePriority(item)
            : GetNavigationJourneyOrder(item);
    }

    public static string ResolveEnterpriseWorkspaceKey(string workspaceKey)
        => workspaceKey;

    private static bool IsEnterpriseWorkspaceMatch(MenuItem item, string workspaceKey, string itemWorkspaceKey, bool isRelated)
        => workspaceKey == WorkspaceMore
           || itemWorkspaceKey == workspaceKey
           || isRelated;

    private static bool IsEnterpriseRelated(MenuItem item, string workspaceKey, string itemWorkspaceKey)
    {
        if (workspaceKey == WorkspaceMore || itemWorkspaceKey == workspaceKey)
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

    private static EnterpriseNavigationGroupKind ToEnterpriseGroupKind(string navigationGroupKey)
        => navigationGroupKey switch
        {
            NavigationGroupStart => EnterpriseNavigationGroupKind.Start,
            NavigationGroupSetup => EnterpriseNavigationGroupKind.Setup,
            NavigationGroupMasterData => EnterpriseNavigationGroupKind.MasterData,
            NavigationGroupDailyWork => EnterpriseNavigationGroupKind.DailyWork,
            NavigationGroupApprovals => EnterpriseNavigationGroupKind.Approvals,
            NavigationGroupAdjustments => EnterpriseNavigationGroupKind.Adjustments,
            NavigationGroupReports => EnterpriseNavigationGroupKind.Reports,
            NavigationGroupAdministration => EnterpriseNavigationGroupKind.Administration,
            _ => EnterpriseNavigationGroupKind.DailyWork
        };

    private static readonly IReadOnlyList<EnterpriseGroupDefinition> EnterpriseGroupDefinitions =
    [
        new(EnterpriseNavigationGroupKind.Start, "Start / Overview", "\u0627\u0644\u0628\u062f\u0621 / \u0646\u0638\u0631\u0629 \u0639\u0627\u0645\u0629", "bi-speedometer2"),
        new(EnterpriseNavigationGroupKind.Approvals, "Approvals", "\u0627\u0644\u0627\u0639\u062a\u0645\u0627\u062f\u0627\u062a", "bi-patch-check"),
        new(EnterpriseNavigationGroupKind.DailyWork, "Daily Work", "\u0627\u0644\u0639\u0645\u0644 \u0627\u0644\u064a\u0648\u0645\u064a", "bi-arrow-left-right"),
        new(EnterpriseNavigationGroupKind.MasterData, "Master Data", "\u0627\u0644\u0628\u064a\u0627\u0646\u0627\u062a \u0627\u0644\u0623\u0633\u0627\u0633\u064a\u0629", "bi-collection"),
        new(EnterpriseNavigationGroupKind.Setup, "Setup", "\u0627\u0644\u0625\u0639\u062f\u0627\u062f", "bi-sliders2"),
        new(EnterpriseNavigationGroupKind.Adjustments, "Adjustments / Exceptions", "\u0627\u0644\u062a\u0633\u0648\u064a\u0627\u062a / \u0627\u0644\u0627\u0633\u062a\u062b\u0646\u0627\u0621\u0627\u062a", "bi-sliders"),
        new(EnterpriseNavigationGroupKind.Reports, "Reports", "\u0627\u0644\u062a\u0642\u0627\u0631\u064a\u0631", "bi-bar-chart-line"),
        new(EnterpriseNavigationGroupKind.Administration, "Administration", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629", "bi-shield-lock"),
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
    Start,
    Setup,
    MasterData,
    DailyWork,
    Approvals,
    Adjustments,
    Reports,
    Administration,
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
