using AlAfkarERP.Shared.Layout;
using SharedWithUI.Permissions;
using Shared.Setup;

var navigableItems = NavigationMenuResolver.Flatten(MenuItem.Menu)
    .Where(x => !string.IsNullOrWhiteSpace(x.Url))
    .ToList();

Assert(navigableItems.Count > 0, "Navigation must contain navigable pages.");
foreach (var item in navigableItems)
{
    Assert(!string.IsNullOrWhiteSpace(item.WorkspaceKey), $"{item.Url} has no workspace.");
    Assert(!string.IsNullOrWhiteSpace(item.NavigationFunctionalGroupKey), $"{item.Url} has no functional group.");
    Assert(!string.IsNullOrWhiteSpace(item.NavigationGroupKey), $"{item.Url} has no journey group.");
    Assert(item.NavigationOrder.HasValue, $"{item.Url} has no navigation order.");
    Assert(item.PermissionPolicy is not null, $"{item.Url} has no explicit permission value.");
    Assert(!string.IsNullOrWhiteSpace(item.Icon), $"{item.Url} has no icon.");
    Assert(!string.IsNullOrWhiteSpace(item.TextEn) && !string.IsNullOrWhiteSpace(item.TextAr), $"{item.Url} is not bilingual.");
    Assert(!string.IsNullOrWhiteSpace(item.KeywordsEn) && !string.IsNullOrWhiteSpace(item.KeywordsAr), $"{item.Url} has incomplete search keywords.");
}

var setupCenter = navigableItems.Single(x => string.Equals(x.Url, "/GeneralSettings/Setup", StringComparison.OrdinalIgnoreCase));
Assert(setupCenter.PermissionPolicy == PermissionList.SystemSetupPermissions.View, "Setup Center must use the setup view permission.");
Assert(setupCenter.WorkspaceKey == NavigationMenuResolver.WorkspaceAdmin, "Setup Center must be in the Admin workspace.");
Assert(setupCenter.NavigationFunctionalGroupKey == NavigationMenuResolver.AdminFunctionalGroupGeneralSettings, "Setup Center must be in General Settings.");
Assert(setupCenter.NavigationGroupKey == NavigationMenuResolver.NavigationGroupStart, "Setup Center must be a start page.");
Assert(setupCenter.NavigationAliases.Contains("/Configuration"), "Setup Center must be discoverable through its configuration alias.");
Assert(setupCenter.KeywordsEn!.Contains("wizard", StringComparison.OrdinalIgnoreCase), "Setup Center must be searchable as a wizard.");
Assert(setupCenter.KeywordsAr!.Contains("تهيئة", StringComparison.Ordinal), "Setup Center must have Arabic setup keywords.");

var setupTracks = new[]
{
    new SetupTrackDefinition("core", "Core", "أساسي", "", "", "bi-building", 10, false),
    new SetupTrackDefinition("supply-chain", "Supply chain", "سلسلة الإمداد", "", "", "bi-box", 20, true, null, ["core"]),
    new SetupTrackDefinition("store-front", "StoreFront", "المتجر", "", "", "bi-shop", 30, true, null, ["supply-chain"])
};
var expandedTracks = SetupProgressPolicy.ExpandTrackDependencies(["store-front"], setupTracks);
Assert(expandedTracks.SetEquals(["core", "supply-chain", "store-front"]), "Setup track dependencies must be selected transitively.");
Assert(SetupProgressPolicy.CompletionPercent([true, false, true, true]) == 75, "Setup completion must count only supplied blocking states.");

var journeyOrder = new[]
{
    NavigationMenuResolver.NavigationGroupStart,
    NavigationMenuResolver.NavigationGroupApprovals,
    NavigationMenuResolver.NavigationGroupDailyWork,
    NavigationMenuResolver.NavigationGroupMasterData,
    NavigationMenuResolver.NavigationGroupSetup,
    NavigationMenuResolver.NavigationGroupReports
};
Assert(journeyOrder.Distinct(StringComparer.OrdinalIgnoreCase).Count() == journeyOrder.Length, "Standard journey groups must remain distinct.");

Console.WriteLine($"Navigation pattern checks passed for {navigableItems.Count} pages.");

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
