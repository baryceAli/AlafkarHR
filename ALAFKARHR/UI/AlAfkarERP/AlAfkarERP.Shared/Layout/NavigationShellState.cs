namespace AlAfkarERP.Shared.Layout;

public static class NavigationShellState
{
    public const string ActiveWorkspaceStorageKey = "alafkar.mobileNav.activeWorkspace";
    public const string FavoriteStorageKey = "alafkar.mobileNav.favoriteKeys";
    public const string RecentStorageKey = "alafkar.mobileNav.recentKeys";
    public const string SidebarCollapsedStorageKey = "alafkar.desktopNav.isCollapsed";
    public const string NavigationModeStorageKey = "alafkar.navigation.mode";
    public const string NavigationModeClassic = "classic";
    public const string NavigationModeFluentFlow = "fluent-flow";
    public const int MaxRecentItems = 6;

    public static IReadOnlyList<string> ParseKeys(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

    public static string SerializeKeys(IEnumerable<string> keys)
        => string.Join("|", keys.Where(key => !string.IsNullOrWhiteSpace(key)).Distinct(StringComparer.OrdinalIgnoreCase));

    public static IReadOnlyList<string> AddRecent(IEnumerable<string> currentKeys, string key)
        => new[] { key }
            .Concat(currentKeys.Where(currentKey => !string.Equals(currentKey, key, StringComparison.OrdinalIgnoreCase)))
            .Take(MaxRecentItems)
            .ToList();
}
