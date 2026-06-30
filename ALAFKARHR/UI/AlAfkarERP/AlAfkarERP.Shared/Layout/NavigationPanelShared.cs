using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace AlAfkarERP.Shared.Layout;

public static class NavigationPanelShared
{
    public static bool CanNavigate(MenuItem item, ClaimsPrincipal? user, IReadOnlySet<string>? licensedBusinessLineKeys)
        => !string.IsNullOrWhiteSpace(item.Url)
           && NavigationMenuResolver.HasOwnPermission(item, user, licensedBusinessLineKeys);

    public static IEnumerable<EnterpriseNavigationEntry> FilterEntries(
        EnterpriseNavigationGroup group,
        string? searchText,
        ClaimsPrincipal? user,
        IReadOnlySet<string>? licensedBusinessLineKeys)
        => group.Entries
            .Where(entry => CanNavigate(entry.Item, user, licensedBusinessLineKeys))
            .Where(entry => MatchesSearch(entry, searchText))
            .GroupBy(entry => NavigationMenuResolver.GetStorageKey(entry.Item), StringComparer.OrdinalIgnoreCase)
            .Select(entryGroup => entryGroup.First());

    public static bool MatchesSearch(EnterpriseNavigationEntry entry, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return true;
        }

        var term = NormalizeSearchText(searchText);
        var haystack = string.Join(" ", entry.Path.Select(item => $"{item.TextEn} {item.TextAr} {item.Url} {item.KeywordsEn} {item.KeywordsAr}"));
        return NormalizeSearchText(haystack).Contains(term);
    }

    public static bool MatchesCurrentWorkspace(
        MenuItem item,
        string activeWorkspace,
        IEnumerable<EnterpriseNavigationGroup> groups)
        => activeWorkspace == NavigationMenuResolver.WorkspaceMore
           || NavigationMenuResolver.GetWorkspaceKey(item) == activeWorkspace
           || groups.SelectMany(group => group.Entries)
               .Any(entry => NavigationMenuResolver.GetStorageKey(entry.Item) == NavigationMenuResolver.GetStorageKey(item));

    public static IEnumerable<MenuItem> ResolveKeys(IEnumerable<string> keys, IEnumerable<MenuItem> authorizedItems)
    {
        var items = authorizedItems.ToList();
        foreach (var key in keys)
        {
            var item = items.FirstOrDefault(candidate => NavigationMenuResolver.GetStorageKey(candidate) == key);
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    public static string GroupKindLabel(EnterpriseNavigationGroupKind kind, Func<string, string, string> text)
        => kind switch
        {
            EnterpriseNavigationGroupKind.Start => text("Start / Overview", "\u0627\u0644\u0628\u062f\u0621 / \u0646\u0638\u0631\u0629 \u0639\u0627\u0645\u0629"),
            EnterpriseNavigationGroupKind.Setup => text("Setup", "\u0627\u0644\u0625\u0639\u062f\u0627\u062f"),
            EnterpriseNavigationGroupKind.MasterData => text("Master Data", "\u0627\u0644\u0628\u064a\u0627\u0646\u0627\u062a \u0627\u0644\u0623\u0633\u0627\u0633\u064a\u0629"),
            EnterpriseNavigationGroupKind.DailyWork => text("Daily Work", "\u0627\u0644\u0639\u0645\u0644 \u0627\u0644\u064a\u0648\u0645\u064a"),
            EnterpriseNavigationGroupKind.Approvals => text("Approvals", "\u0627\u0644\u0627\u0639\u062a\u0645\u0627\u062f\u0627\u062a"),
            EnterpriseNavigationGroupKind.Adjustments => text("Adjustments / Exceptions", "\u0627\u0644\u062a\u0633\u0648\u064a\u0627\u062a / \u0627\u0644\u0627\u0633\u062a\u062b\u0646\u0627\u0621\u0627\u062a"),
            EnterpriseNavigationGroupKind.Reports => text("Reports", "\u0627\u0644\u062a\u0642\u0627\u0631\u064a\u0631"),
            EnterpriseNavigationGroupKind.Administration => text("Administration", "\u0627\u0644\u0625\u062f\u0627\u0631\u0629"),
            EnterpriseNavigationGroupKind.Related => text("Related", "\u0645\u0631\u062a\u0628\u0637"),
            _ => text("Pages", "\u0635\u0641\u062d\u0627\u062a")
        };

    public static string FavoriteTitle(MenuItem item, bool isFavorite, Func<string, string, string> text)
        => isFavorite
            ? text("Remove from favorites", "\u0625\u0632\u0627\u0644\u0629 \u0645\u0646 \u0627\u0644\u0645\u0641\u0636\u0644\u0629")
            : text("Add to favorites", "\u0625\u0636\u0627\u0641\u0629 \u0644\u0644\u0645\u0641\u0636\u0644\u0629");

    public static bool IsActionLike(MenuItem item)
    {
        var haystack = NormalizeSearchText(string.Join(" ", item.TextEn, item.TextAr, item.Url, item.KeywordsEn, item.KeywordsAr));
        return haystack.Contains("create")
               || haystack.Contains("new")
               || haystack.Contains("add")
               || haystack.Contains("assign")
               || haystack.Contains("approve")
               || haystack.Contains("generate")
               || haystack.Contains("upload")
               || haystack.Contains("setup")
               || haystack.Contains("configuration")
               || haystack.Contains("\u0625\u0646\u0634\u0627\u0621")
               || haystack.Contains("\u0625\u0636\u0627\u0641\u0629")
               || haystack.Contains("\u062a\u0639\u064a\u064a\u0646")
               || haystack.Contains("\u0627\u0639\u062a\u0645\u0627\u062f");
    }

    public static string NormalizeSearchText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark || character == '\u0640')
            {
                continue;
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
