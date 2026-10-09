using System.Security.Claims;
using System.Data;
using Carter;
using GeneralSettings.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Contracts.CQRS;
using Shared.Contracts.Organization;
using Shared.Exceptions;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.PublicWebsite;

public sealed record EffectiveWebsiteSettings(Guid OwnerCompanyId, string StorageKey, string StoragePath,
    string PublicOrigin, long ImageMaxBytes, long PdfMaxBytes, long MediaMaxBytes, bool Activated);

public sealed class PublicWebsiteSettingsResolver(GeneralSettingsDbContext db, IOptions<PublicWebsiteOptions> options, IWebHostEnvironment environment)
{
    public const long MiB = 1024 * 1024;
    public const int UploadCeilingMiB = 100;
    public List<WebsiteStorageLocation> Locations()
    {
        var legacy = new WebsiteStorageLocation { Key = "legacy", Name = "Existing website storage", Description = "Existing server location. Deployment must provide durable storage and backups.", Path = options.Value.StorageRoot };
        var locations = new List<WebsiteStorageLocation> { legacy };
        locations.AddRange(options.Value.StorageLocations);
        if (locations.Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Key.Length > 100 || string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.Path)) || locations.Select(x => x.Key).Distinct(StringComparer.Ordinal).Count() != locations.Count)
            throw new BadRequestException("Approved storage locations are invalid. Contact the deployment administrator.");
        return locations;
    }
    public static string ManagedKey(Guid id) => "managed-" + id.ToString("N");
    public List<WebsiteStorageLocation> ManagedRoots()
    {
        var roots = new List<WebsiteStorageLocation> { new() { Key = "default", Name = "Managed website storage", Description = "Server-managed folders. Deployment must provide durable storage and backups.", Path = "App_Data/PublicWebsiteStorage" } };
        roots.AddRange(options.Value.ManagedStorageRoots);
        if (roots.Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Key != x.Key.ToLowerInvariant() || x.Key.Length > 100 || string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.Path)) || roots.Select(x => x.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count() != roots.Count)
            throw new BadRequestException("Managed storage roots are invalid. Contact the deployment administrator.");
        return roots;
    }
    public string ManagedRootPath(string key)
    {
        var root = ManagedRoots().SingleOrDefault(x => x.Key == key) ?? throw new BadRequestException("Choose an approved managed storage root.");
        var path = OutsideWebRoot(root.Path); WebsiteManagedStorage.NoRedirects(path); return path;
    }
    private string OutsideWebRoot(string path)
    {
        var root = Path.GetFullPath(path, environment.ContentRootPath);
        var webRoot = Path.GetFullPath(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"));
        if ((root + Path.DirectorySeparatorChar).StartsWith(webRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Website storage must be outside wwwroot.");
        return root;
    }
    public async Task<List<WebsiteStorageChoice>> LocationsAsync(Guid parent, CancellationToken ct)
    {
        var locations = Locations().Select(x => new WebsiteStorageChoice(x.Key, x.Name, x.Description)).ToList();
        var managed = await db.Set<PublicWebsiteManagedLocation>().AsNoTracking().Where(x => x.ParentCompanyId == parent).OrderBy(x => x.DisplayName).ToListAsync(ct);
        locations.AddRange(managed.Select(x => new WebsiteStorageChoice(ManagedKey(x.Id), x.DisplayName, x.Description)));
        return locations;
    }
    public async Task<string> StoragePathAsync(string key, Guid parent, CancellationToken ct)
    {
        var deployed = Locations().SingleOrDefault(x => x.Key == key);
        if (deployed != null) return OutsideWebRoot(deployed.Path);
        if (key != null && key.StartsWith("managed-", StringComparison.Ordinal) && Guid.TryParseExact(key[8..], "N", out var id))
        {
            var location = await db.Set<PublicWebsiteManagedLocation>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.ParentCompanyId == parent, ct)
                ?? throw new BadRequestException("This media storage location is not authorized for this website.");
            return WebsiteManagedStorage.ChildPath(ManagedRootPath(location.RootKey), parent, location.FolderName);
        }
        throw new BadRequestException("Choose an approved media storage location.");
    }
    public async Task<EffectiveWebsiteSettings> GetAsync(CancellationToken ct)
    {
        var saved = await db.Set<PublicWebsiteConfiguration>().AsNoTracking().SingleOrDefaultAsync(ct);
        var owner = saved?.OwnerCompanyId ?? options.Value.OwnerCompanyId;
        if (saved == null && owner == Guid.Empty)
        {
            var owners = await db.PublicWebsiteSites.Select(x => x.CompanyId).Distinct().Take(2).ToListAsync(ct);
            if (owners.Count > 1) throw new BadRequestException("Multiple existing website owners require administrator review.");
            owner = owners.FirstOrDefault();
        }
        var key = saved?.StorageLocationKey ?? "legacy";
        return new(owner, key, await StoragePathAsync(key, saved?.AdministratorParentCompanyId ?? Guid.Empty, ct), saved?.PublicOrigin ?? options.Value.Origin,
            saved == null ? options.Value.ImageMaxBytes : saved.ImageLimitMiB * MiB,
            saved == null ? options.Value.PdfMaxBytes : saved.PdfLimitMiB * MiB,
            saved == null ? options.Value.MediaMaxBytes : saved.MediaLimitMiB * MiB,
            saved?.Activated ?? owner != Guid.Empty);
    }
    public static string ValidateOrigin(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 2048 || value.Any(char.IsControl) || value.Contains('\\') ||
            !Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https") || string.IsNullOrEmpty(uri.Host) ||
            uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0 || uri.AbsolutePath != "/")
            throw new BadRequestException("Enter a public HTTP(S) origin without a path, credentials, query or fragment.");
        return uri.GetLeftPart(UriPartial.Authority);
    }
}

public sealed record ReadWebsiteConfigurationQuery : IQuery<WebsiteConfigurationDto>;
public sealed record SaveWebsiteConfigurationCommand(WebsiteConfigurationSaveRequest Request) : ICommand<WebsiteConfigurationDto>;
public sealed record CheckWebsiteReadinessCommand(WebsiteConfigurationSaveRequest Request) : ICommand<WebsiteReadinessDto>;
public sealed record RegisterWebsiteStorageFolderCommand(WebsiteStorageFolderRequest Request) : ICommand<WebsiteStorageChoice>;
public sealed record ActivateWebsiteCommand(Guid ExpectedToken) : ICommand<WebsiteConfigurationDto>;
public sealed record ReadWebsiteRuntimeQuery : IQuery<WebsiteRuntimeSettingsDto>;
public sealed record ReadWebsiteManagementStatusQuery : IQuery<WebsiteManagementStatusDto>;

public sealed class PublicWebsiteConfigurationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/publicwebsite");
        group.AddEndpointFilter(async (context, next) =>
        {
            context.HttpContext.Response.Headers.CacheControl = "no-store";
            try { return await next(context); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (DbUpdateConcurrencyException) { return Results.Problem("Configuration changed. Reload before saving.", statusCode: 409); }
            catch (DbUpdateException e) when (e.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 or 1205 })
            { return Results.Problem("Another administrator completed setup. Reload configuration.", statusCode: 409); }
            catch (Microsoft.Data.SqlClient.SqlException e) when (e.Number == 1205)
            { return Results.Problem("Another operation changed website settings. Reload configuration and retry.", statusCode: 409); }
            catch (BadHttpRequestException e) { return Results.Problem(e.Message, statusCode: e.StatusCode); }
            catch (Microsoft.Data.SqlClient.SqlException e) when (e.Number == 208)
            { return Results.Problem("Website database schema is unavailable. Apply the website migrations through deployment, then retry.", statusCode: 503); }
        });
        group.MapGet("/configuration", async (ISender sender) => await sender.Send(new ReadWebsiteConfigurationQuery()))
            .RequireAuthorization(PermissionList.PublicWebsiteConfigurationPermissions.View);
        group.MapPut("/configuration", async (WebsiteConfigurationSaveRequest request, ISender sender) => await sender.Send(new SaveWebsiteConfigurationCommand(request)))
            .RequireAuthorization(PermissionList.PublicWebsiteConfigurationPermissions.Edit);
        group.MapPost("/configuration/readiness", async (WebsiteConfigurationSaveRequest request, ISender sender) => await sender.Send(new CheckWebsiteReadinessCommand(request)))
            .RequireAuthorization(PermissionList.PublicWebsiteConfigurationPermissions.View);
        group.MapPost("/configuration/activate", async (WebsiteVersionRequest request, ISender sender) => await sender.Send(new ActivateWebsiteCommand(request.ExpectedToken)))
            .RequireAuthorization(PermissionList.PublicWebsiteConfigurationPermissions.Edit);
        group.MapPost("/configuration/storage-locations", async (WebsiteStorageFolderRequest request, ISender sender) => await sender.Send(new RegisterWebsiteStorageFolderCommand(request)))
            .RequireAuthorization(PermissionList.PublicWebsiteConfigurationPermissions.Edit)
            .Produces<WebsiteStorageChoice>();
        group.MapGet("/runtime-settings", async (ISender sender) => await sender.Send(new ReadWebsiteRuntimeQuery())).AllowAnonymous();
        group.MapGet("/management-status", async (ISender sender) => await sender.Send(new ReadWebsiteManagementStatusQuery()))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.View);
    }
}

public sealed class PublicWebsiteConfigurationHandlers(GeneralSettingsDbContext db, PublicWebsiteSettingsResolver resolver,
    ICompanyHierarchyReader hierarchy, IWebsiteCompanyReader companies, IHttpContextAccessor accessor) :
    IQueryHandler<ReadWebsiteConfigurationQuery, WebsiteConfigurationDto>,
    ICommandHandler<SaveWebsiteConfigurationCommand, WebsiteConfigurationDto>,
    ICommandHandler<CheckWebsiteReadinessCommand, WebsiteReadinessDto>,
    ICommandHandler<ActivateWebsiteCommand, WebsiteConfigurationDto>,
    ICommandHandler<RegisterWebsiteStorageFolderCommand, WebsiteStorageChoice>,
    IQueryHandler<ReadWebsiteRuntimeQuery, WebsiteRuntimeSettingsDto>,
    IQueryHandler<ReadWebsiteManagementStatusQuery, WebsiteManagementStatusDto>
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? throw new UnauthorizedAccessException();
    private bool Can(string permission) => User.HasClaim("Permission", permission);
    private Guid Company => Guid.TryParse(User.FindFirst("company_id")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
    private string Actor => (User.Identity?.Name ?? "")[..Math.Min(220, (User.Identity?.Name ?? "").Length)] + " (" + (User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown") + ")";
    private async Task<(PublicWebsiteConfiguration? Record, EffectiveWebsiteSettings Settings, Guid Parent, bool ParentAdmin, List<WebsiteCompanyChoice> Choices)> Access(CancellationToken ct)
    {
        var record = await db.Set<PublicWebsiteConfiguration>().SingleOrDefaultAsync(ct);
        var settings = await resolver.GetAsync(ct);
        var callerParent = await hierarchy.GetParentCompanyIdForCompanyAsync(Company, ct);
        var parentAdmin = callerParent == Company && Can(PermissionList.CompanyPermissions.EditChild);
        var parent = record?.AdministratorParentCompanyId ?? (settings.OwnerCompanyId == Guid.Empty ? callerParent : await hierarchy.GetParentCompanyIdForCompanyAsync(settings.OwnerCompanyId, ct));
        if (settings.OwnerCompanyId == Guid.Empty ? !parentAdmin : Company != settings.OwnerCompanyId && !(parentAdmin && Company == parent))
            throw new UnauthorizedAccessException();
        var allowed = await companies.GetActiveCompaniesAsync(parent, ct);
        var choices = allowed.Where(x => parentAdmin || x.Id == settings.OwnerCompanyId).Select(x => new WebsiteCompanyChoice(x.Id, x.Name, x.NameEng)).ToList();
        return (record, settings, parent, parentAdmin, choices);
    }
    private static WebsiteConfigurationValues Values(EffectiveWebsiteSettings s) => new()
    {
        OwnerCompanyId = s.OwnerCompanyId, StorageLocationKey = s.StorageKey, PublicOrigin = s.PublicOrigin,
        ImageLimitMiB = (int)(s.ImageMaxBytes / PublicWebsiteSettingsResolver.MiB), PdfLimitMiB = (int)(s.PdfMaxBytes / PublicWebsiteSettingsResolver.MiB), MediaLimitMiB = (int)(s.MediaMaxBytes / PublicWebsiteSettingsResolver.MiB)
    };
    private async Task<WebsiteConfigurationDto> Read(CancellationToken ct)
    {
        var a = await Access(ct);
        var locked = await db.PublicWebsiteMedia.AnyAsync(ct);
        var edit = Can(PermissionList.PublicWebsiteConfigurationPermissions.Edit);
        var locations = await resolver.LocationsAsync(a.Parent, ct);
        var roots = resolver.ManagedRoots().Select(x => new WebsiteManagedRootChoice(x.Key, x.Name, x.Description)).ToList();
        var reason = locked ? "media-exists" : !edit ? "missing-permission" : locations.Count < 2 ? "single-location" : null;
        return new(a.Record?.Token ?? Guid.Empty, Values(a.Settings), a.Settings.Activated,
            a.Settings.Activated || await db.PublicWebsiteSites.AnyAsync(ct) || !a.ParentAdmin,
            locked, edit, a.Choices, locations, a.Record?.UpdatedAt, a.Record?.UpdatedBy, edit && !locked && roots.Count > 0, reason, roots);
    }
    public Task<WebsiteConfigurationDto> Handle(ReadWebsiteConfigurationQuery q, CancellationToken ct) => Read(ct);
    public async Task<WebsiteStorageChoice> Handle(RegisterWebsiteStorageFolderCommand command, CancellationToken ct)
    {
        // Serialize registration with settings changes and uploads: a concurrent first upload locks storage.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var a = await Access(ct); Token(a.Record, command.Request.ExpectedToken);
        if (!Can(PermissionList.PublicWebsiteConfigurationPermissions.Edit)) throw new UnauthorizedAccessException();
        if (command.Request.OperationId == Guid.Empty) throw new BadRequestException("A storage registration identifier is required.");
        if (resolver.Locations().Any(x => x.Key == PublicWebsiteSettingsResolver.ManagedKey(command.Request.OperationId)))
            throw new BadHttpRequestException("This storage identifier is already reserved. Open a new folder dialog and retry.", 409);
        var v = command.Request.Values ?? throw new BadRequestException("Storage folder values are required.");
        var folder = WebsiteManagedStorage.FolderName(v.FolderName);
        var name = v.DisplayName?.Trim(); var description = v.Description?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(name) || name.Length > 150 || name.Any(char.IsControl) || description.Length > 500 || description.Any(char.IsControl))
            throw new BadRequestException("Enter a display name up to 150 characters and a description up to 500 characters, without control characters.");
        var root = resolver.ManagedRootPath(v.RootKey);
        var prior = await db.Set<PublicWebsiteManagedLocation>().SingleOrDefaultAsync(x => x.Id == command.Request.OperationId && x.ParentCompanyId == a.Parent, ct);
        if (prior != null)
        {
            if (prior.RootKey != v.RootKey || prior.FolderName != folder || prior.DisplayName != name || prior.Description != description)
                throw new BadHttpRequestException("This storage operation already completed with different values. Reload locations before continuing.", 409);
            return new(PublicWebsiteSettingsResolver.ManagedKey(prior.Id), prior.DisplayName, prior.Description);
        }
        if (await db.PublicWebsiteMedia.AnyAsync(ct)) throw new BadRequestException("Existing uploads lock media storage. Relocation requires a separate process.");
        if (await db.Set<PublicWebsiteManagedLocation>().AnyAsync(x => x.ParentCompanyId == a.Parent && x.RootKey == v.RootKey && x.FolderName == folder, ct))
            throw new BadHttpRequestException("A storage folder with this name already exists. Reload storage locations before selecting it.", 409);
        var path = WebsiteManagedStorage.ChildPath(root, a.Parent, folder);
        try
        {
            if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) throw new BadRequestException("Choose a new or empty folder. Existing nonempty folders cannot be registered.");
            await WebsiteManagedStorage.ProbeAsync(path, true, ct);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        { throw new BadRequestException("The storage folder could not be created or verified. Check managed root access and server permissions."); }
        var location = new PublicWebsiteManagedLocation { Id = command.Request.OperationId, ParentCompanyId = a.Parent, RootKey = v.RootKey, FolderName = folder, DisplayName = name, Description = description, CreatedAt = DateTime.UtcNow, CreatedBy = Actor };
        db.Add(location);
        db.PublicWebsiteAudit.Add(new() { Id = Guid.NewGuid(), CompanyId = a.Settings.OwnerCompanyId == Guid.Empty ? a.Parent : a.Settings.OwnerCompanyId, Action = "Register website storage folder", Token = location.Id, At = location.CreatedAt, By = Actor });
        await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
        return new(PublicWebsiteSettingsResolver.ManagedKey(location.Id), name, description);
    }
    private static void Token(PublicWebsiteConfiguration? record, Guid expected)
    {
        if ((record?.Token ?? Guid.Empty) != expected) throw new BadHttpRequestException("Configuration changed. Reload before continuing.", 409);
    }
    private async Task ValidateChanges(WebsiteConfigurationValues v, EffectiveWebsiteSettings current, bool parentAdmin, List<WebsiteCompanyChoice> choices, Guid parent, CancellationToken ct)
    {
        if (v == null) throw new BadRequestException("Configuration values are required.");
        if (v.OwnerCompanyId == Guid.Empty || !choices.Any(x => x.Id == v.OwnerCompanyId)) throw new BadRequestException("Select an active authorized owning company.");
        if (v.OwnerCompanyId != current.OwnerCompanyId && (!parentAdmin || current.Activated || await db.PublicWebsiteSites.AnyAsync(ct)))
            throw new BadRequestException("Website ownership is locked. Company transfers require a separate process.");
        if (v.StorageLocationKey != current.StorageKey && await db.PublicWebsiteMedia.AnyAsync(ct))
            throw new BadRequestException("Media already exists. Storage relocation requires a separate process.");
        await resolver.StoragePathAsync(v.StorageLocationKey, parent, ct);
        PublicWebsiteSettingsResolver.ValidateOrigin(v.PublicOrigin);
        if (new[] { v.ImageLimitMiB, v.PdfLimitMiB, v.MediaLimitMiB }.Any(x => x < 1 || x > PublicWebsiteSettingsResolver.UploadCeilingMiB))
            throw new BadRequestException("Upload limits must be between 1 and 100 MiB.");
    }
    private void Audit(PublicWebsiteConfiguration record, string action)
    {
        record.Token = Guid.NewGuid(); record.UpdatedAt = DateTime.UtcNow; record.UpdatedBy = Actor;
        db.PublicWebsiteAudit.Add(new() { Id = Guid.NewGuid(), CompanyId = record.OwnerCompanyId, Token = record.Token, Action = action, At = record.UpdatedAt, By = Actor });
    }
    public async Task<WebsiteConfigurationDto> Handle(SaveWebsiteConfigurationCommand command, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var a = await Access(ct); Token(a.Record, command.Request.ExpectedToken);
        await ValidateChanges(command.Request.Values, a.Settings, a.ParentAdmin, a.Choices, a.Parent, ct);
        var v = command.Request.Values;
        var record = a.Record ?? new PublicWebsiteConfiguration { AdministratorParentCompanyId = a.Parent, Activated = a.Settings.Activated };
        if (a.Record == null) db.Add(record);
        record.OwnerCompanyId = v.OwnerCompanyId; record.StorageLocationKey = v.StorageLocationKey;
        record.PublicOrigin = PublicWebsiteSettingsResolver.ValidateOrigin(v.PublicOrigin);
        record.ImageLimitMiB = v.ImageLimitMiB; record.PdfLimitMiB = v.PdfLimitMiB; record.MediaLimitMiB = v.MediaLimitMiB;
        Audit(record, "Save website configuration"); await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
        return await Read(ct);
    }
    private async Task<WebsiteReadinessDto> Readiness(WebsiteConfigurationValues v, Guid parent, CancellationToken ct)
    {
        var checks = new List<WebsiteReadinessCheck>();
        // Query each required table without changing the schema or initializing website content.
        try
        {
            await db.PublicWebsiteSites.AnyAsync(ct); await db.PublicWebsiteMedia.AnyAsync(ct);
            await db.PublicWebsiteRevisions.AnyAsync(ct); await db.PublicWebsiteAudit.AnyAsync(ct);
            await db.Set<PublicWebsiteConfiguration>().AnyAsync(ct);
            await db.Set<PublicWebsiteManagedLocation>().AnyAsync(ct);
            checks.Add(new("schema", true, "Website database tables are available."));
        }
        catch (Microsoft.Data.SqlClient.SqlException e) when (e.Number == 208)
        { checks.Add(new("schema", false, "Apply the website migrations through deployment.")); }
        checks.Add(new("owner", v.OwnerCompanyId != Guid.Empty, "An active authorized owning company must be selected."));
        try { PublicWebsiteSettingsResolver.ValidateOrigin(v.PublicOrigin); checks.Add(new("origin", true, "Public origin is valid.")); }
        catch (BadRequestException) { checks.Add(new("origin", false, "Use an HTTP(S) origin without a path, credentials, query or fragment.")); }
        checks.Add(new("limits", new[] { v.ImageLimitMiB, v.PdfLimitMiB, v.MediaLimitMiB }.All(x => x is >= 1 and <= 100), "Upload limits must be between 1 and 100 MiB."));
        try
        {
            var root = await resolver.StoragePathAsync(v.StorageLocationKey, parent, ct);
            await WebsiteManagedStorage.ProbeAsync(root, !resolver.Locations().Any(x => x.Key == v.StorageLocationKey), ct);
            checks.Add(new("storage", true, "Approved media storage supports reading, writing and deletion. Durability and backups are deployment responsibilities."));
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or BadRequestException)
        { checks.Add(new("storage", false, "Approved media storage is unavailable. Check server directory permissions and mounted storage.")); }
        return new(checks);
    }
    public async Task<WebsiteReadinessDto> Handle(CheckWebsiteReadinessCommand command, CancellationToken ct)
    {
        var a = await Access(ct); Token(a.Record, command.Request.ExpectedToken);
        // View-only administrators may inspect saved settings but cannot probe arbitrary pending selections.
        var v = Can(PermissionList.PublicWebsiteConfigurationPermissions.Edit) ? command.Request.Values : Values(a.Settings);
        await ValidateChangesForReadiness(v, a.Settings, a.ParentAdmin, a.Choices, a.Parent, ct);
        return await Readiness(v, a.Parent, ct);
    }
    private async Task ValidateChangesForReadiness(WebsiteConfigurationValues v, EffectiveWebsiteSettings current, bool parentAdmin, List<WebsiteCompanyChoice> choices, Guid parent, CancellationToken ct)
    {
        if (v == null || !choices.Any(x => x.Id == v.OwnerCompanyId)) throw new BadRequestException("Select an active authorized owning company.");
        if (v.OwnerCompanyId != current.OwnerCompanyId && (!parentAdmin || current.Activated || await db.PublicWebsiteSites.AnyAsync(ct))) throw new BadRequestException("Website ownership is locked.");
        if (v.StorageLocationKey != current.StorageKey && await db.PublicWebsiteMedia.AnyAsync(ct)) throw new BadRequestException("Media storage is locked.");
        if (!(await resolver.LocationsAsync(parent, ct)).Any(x => x.Key == v.StorageLocationKey)) throw new BadRequestException("Choose an approved storage location.");
    }
    public async Task<WebsiteConfigurationDto> Handle(ActivateWebsiteCommand command, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var a = await Access(ct); Token(a.Record, command.ExpectedToken);
        if (a.Record == null) throw new BadRequestException("Save configuration before activating the control panel.");
        await ValidateChanges(Values(a.Settings), a.Settings, a.ParentAdmin, a.Choices, a.Parent, ct);
        if (!(await Readiness(Values(a.Settings), a.Parent, ct)).Ready) throw new BadRequestException("Complete the website readiness checks before activation.");
        a.Record.Activated = true; Audit(a.Record, "Activate website control panel");
        await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); return await Read(ct);
    }
    public async Task<WebsiteRuntimeSettingsDto> Handle(ReadWebsiteRuntimeQuery q, CancellationToken ct)
    {
        // This contract intentionally excludes private configuration and storage paths.
        var origin = await db.Set<PublicWebsiteConfiguration>().AsNoTracking().Select(x => x.PublicOrigin).SingleOrDefaultAsync(ct);
        return new(origin ?? "");
    }
    public async Task<WebsiteManagementStatusDto> Handle(ReadWebsiteManagementStatusQuery q, CancellationToken ct)
    {
        var settings = await resolver.GetAsync(ct);
        if (settings.OwnerCompanyId != Guid.Empty && settings.OwnerCompanyId != Company) throw new UnauthorizedAccessException();
        return new(settings.Activated && settings.OwnerCompanyId != Guid.Empty);
    }
}
