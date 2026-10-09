namespace SharedWithUI.GeneralSettings.Dtos;

public sealed class WebsiteConfigurationValues
{
    public Guid OwnerCompanyId { get; set; }
    public string StorageLocationKey { get; set; } = "legacy";
    public string PublicOrigin { get; set; } = "";
    public int ImageLimitMiB { get; set; } = 10;
    public int PdfLimitMiB { get; set; } = 25;
    public int MediaLimitMiB { get; set; } = 100;
}
public sealed record WebsiteCompanyChoice(Guid Id, string Name, string NameEng);
public sealed record WebsiteStorageChoice(string Key, string Name, string Description);
public sealed record WebsiteManagedRootChoice(string Key, string Name, string Description);
public sealed class WebsiteStorageFolderValues
{
    public string RootKey { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string FolderName { get; set; } = "";
    public string Description { get; set; } = "";
}
public sealed record WebsiteStorageFolderRequest(Guid ExpectedToken, Guid OperationId, WebsiteStorageFolderValues Values);
public sealed record WebsiteConfigurationDto(Guid Token, WebsiteConfigurationValues Values, bool Activated,
    bool OwnerLocked, bool StorageLocked, bool CanEdit, List<WebsiteCompanyChoice> Companies,
    List<WebsiteStorageChoice> StorageLocations, DateTime? UpdatedAt, string? UpdatedBy,
    bool CanCreateStorageFolder, string? StorageChangeBlockedReason, List<WebsiteManagedRootChoice> ManagedRoots);
public sealed record WebsiteConfigurationSaveRequest(Guid ExpectedToken, WebsiteConfigurationValues Values);
public sealed record WebsiteReadinessCheck(string Key, bool Passed, string Message);
public sealed record WebsiteReadinessDto(List<WebsiteReadinessCheck> Checks)
{
    public bool Ready => Checks.Count > 0 && Checks.All(x => x.Passed);
}
public sealed record WebsiteRuntimeSettingsDto(string PublicOrigin);
public sealed record WebsiteManagementStatusDto(bool Activated);
