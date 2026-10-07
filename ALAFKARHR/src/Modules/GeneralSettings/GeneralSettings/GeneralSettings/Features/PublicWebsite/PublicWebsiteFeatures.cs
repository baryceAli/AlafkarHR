using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Carter;
using FluentValidation;
using GeneralSettings.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.PublicWebsite;

public sealed record ReadWebsiteQuery(bool Draft) : IQuery<WebsiteDraftDto>;
public sealed record SaveWebsiteCommand(WebsiteSaveRequest Request) : ICommand<WebsiteDraftDto>;
public sealed record PublishWebsiteCommand(Guid ExpectedToken) : ICommand<WebsiteDraftDto>;
public sealed record RestoreWebsiteCommand(Guid RevisionId, Guid ExpectedToken) : ICommand<WebsiteDraftDto>;
public sealed record ListWebsiteRevisionsQuery : IQuery<List<WebsiteRevisionDto>>;
public sealed record UploadWebsiteMediaCommand(IFormFile File) : ICommand<WebsiteMediaDto>;
public sealed record ReadWebsiteMediaQuery(Guid Id, bool Draft) : IQuery<WebsiteMediaFile>;
public sealed record WebsiteMediaFile(Stream Stream, string ContentType, string Name);

public sealed class SaveWebsiteValidator : AbstractValidator<SaveWebsiteCommand>
{
    public SaveWebsiteValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.ExpectedToken).NotEmpty();
        RuleFor(x => x.Request.Content).NotNull();
    }
}

public sealed class PublicWebsiteEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/publicwebsite");
        group.AddEndpointFilter(async (context, next) =>
        {
            context.HttpContext.Response.Headers.CacheControl = "no-store";
            try { return await next(context); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (DbUpdateConcurrencyException) { return Results.Problem("Another editor saved changes. Reload the draft before saving again.", statusCode: 409); }
            catch (BadHttpRequestException e) { return Results.Problem(e.Message, statusCode: e.StatusCode); }
        });
        group.MapGet("/published", async (ISender sender) => (await sender.Send(new ReadWebsiteQuery(false))).Content).AllowAnonymous();
        group.MapGet("/draft", async (ISender sender) => await sender.Send(new ReadWebsiteQuery(true)))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.View);
        group.MapPut("/draft", async (WebsiteSaveRequest request, ISender sender) => await sender.Send(new SaveWebsiteCommand(request)))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.Edit);
        group.MapPost("/publish", async (WebsiteVersionRequest request, ISender sender) => await sender.Send(new PublishWebsiteCommand(request.ExpectedToken)))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.Publish);
        group.MapGet("/revisions", async (ISender sender) => await sender.Send(new ListWebsiteRevisionsQuery()))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.View);
        group.MapPost("/revisions/{id:guid}/restore", async (Guid id, WebsiteVersionRequest request, ISender sender) => await sender.Send(new RestoreWebsiteCommand(id, request.ExpectedToken)))
            .RequireAuthorization(PermissionList.PublicWebsitePermissions.Publish);
        group.MapPost("/media", async (IFormFile file, ISender sender) => await sender.Send(new UploadWebsiteMediaCommand(file)))
            .DisableAntiforgery().RequireAuthorization(PermissionList.PublicWebsitePermissions.Upload)
            .WithMetadata(new Microsoft.AspNetCore.Mvc.RequestSizeLimitAttribute(105 * 1024 * 1024));
        group.MapGet("/media/{id:guid}", async (Guid id, ISender sender) =>
        {
            var file = await sender.Send(new ReadWebsiteMediaQuery(id, false));
            return Results.File(file.Stream, file.ContentType, fileDownloadName: file.ContentType is "application/pdf" or "application/zip" ? file.Name : null, enableRangeProcessing: true);
        }).AllowAnonymous();
        group.MapGet("/media/{id:guid}/draft", async (Guid id, ISender sender) =>
        {
            var file = await sender.Send(new ReadWebsiteMediaQuery(id, true));
            return Results.File(file.Stream, file.ContentType, fileDownloadName: file.ContentType is "application/pdf" or "application/zip" ? file.Name : null, enableRangeProcessing: true);
        }).RequireAuthorization(PermissionList.PublicWebsitePermissions.View);
    }
}

public sealed class PublicWebsiteHandlers(GeneralSettingsDbContext db, IHttpContextAccessor accessor,
    IOptions<PublicWebsiteOptions> options, IWebHostEnvironment environment) :
    IQueryHandler<ReadWebsiteQuery, WebsiteDraftDto>, ICommandHandler<SaveWebsiteCommand, WebsiteDraftDto>,
    ICommandHandler<PublishWebsiteCommand, WebsiteDraftDto>, ICommandHandler<RestoreWebsiteCommand, WebsiteDraftDto>,
    IQueryHandler<ListWebsiteRevisionsQuery, List<WebsiteRevisionDto>>, ICommandHandler<UploadWebsiteMediaCommand, WebsiteMediaDto>,
    IQueryHandler<ReadWebsiteMediaQuery, WebsiteMediaFile>
{
    private Guid Owner => options.Value.OwnerCompanyId;
    private string User
    {
        get
        {
            var principal = accessor.HttpContext?.User;
            var id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var name = principal?.Identity?.Name;
            return string.IsNullOrWhiteSpace(name) ? id : name[..Math.Min(220, name.Length)] + " (" + id + ")";
        }
    }
    private void Authorize()
    {
        if (Owner == Guid.Empty) throw new BadRequestException("Configure PublicWebsite:OwnerCompanyId before using the control panel.");
        if (!Guid.TryParse(accessor.HttpContext?.User.FindFirst("company_id")?.Value, out var company) || company != Owner)
            throw new UnauthorizedAccessException();
    }
    private static WebsiteSnapshot Parse(string json) => JsonSerializer.Deserialize<WebsiteSnapshot>(json)!;
    private static WebsiteDraftDto Dto(PublicWebsiteSite site) => new(site.Token, Parse(site.DraftJson), site.UpdatedAt, site.UpdatedBy);
    private void Audit(string action, Guid token) => db.Set<PublicWebsiteAudit>().Add(new()
    { Id = Guid.NewGuid(), CompanyId = Owner, Action = action, Token = token, At = DateTime.UtcNow, By = User });
    private async Task<PublicWebsiteSite> Site(CancellationToken ct)
    {
        Authorize();
        var site = await db.Set<PublicWebsiteSite>().SingleOrDefaultAsync(x => x.CompanyId == Owner, ct);
        if (site != null) return site;
        var baseline = new PublicWebsiteRevision { Id = Guid.NewGuid(), CompanyId = Owner, ContentJson = JsonSerializer.Serialize(WebsiteContentCatalog.Manifest.CreateSnapshot()), PublishedAt = DateTime.UtcNow, PublishedBy = "system: initial website" };
        site = new() { Id = Guid.NewGuid(), CompanyId = Owner, Token = Guid.NewGuid(), PublishedRevisionId = baseline.Id, DraftJson = baseline.ContentJson, UpdatedAt = DateTime.UtcNow, UpdatedBy = User };
        db.Add(baseline);
        db.Add(site);
        Audit("Initialize", site.Token);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException e) when (e.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
        {
            // Two first-time editors can initialize together; use the winning baseline.
            db.ChangeTracker.Clear();
            site = await db.Set<PublicWebsiteSite>().SingleAsync(x => x.CompanyId == Owner, ct);
        }
        return site;
    }
    private static void CheckToken(PublicWebsiteSite site, Guid expected)
    {
        if (expected == Guid.Empty || site.Token != expected)
            throw new BadHttpRequestException("Another editor changed this draft. Reload before continuing.", 409);
    }
    public async Task<WebsiteDraftDto> Handle(ReadWebsiteQuery q, CancellationToken ct)
    {
        if (q.Draft) return Dto(await Site(ct));
        var json = Owner == Guid.Empty ? null : await db.Set<PublicWebsiteSite>().AsNoTracking()
            .Where(x => x.CompanyId == Owner).Join(db.Set<PublicWebsiteRevision>(), s => s.PublishedRevisionId, r => (Guid?)r.Id, (s, r) => r.ContentJson).SingleOrDefaultAsync(ct);
        return new(Guid.Empty, json == null ? WebsiteContentCatalog.Manifest.CreateSnapshot() : Parse(json), default, "");
    }
    public async Task<WebsiteDraftDto> Handle(SaveWebsiteCommand command, CancellationToken ct)
    {
        var site = await Site(ct); CheckToken(site, command.Request.ExpectedToken);
        await Validate(command.Request.Content, false, ct);
        site.DraftJson = JsonSerializer.Serialize(command.Request.Content);
        Touch(site, "Save draft"); await db.SaveChangesAsync(ct); return Dto(site);
    }
    private void Touch(PublicWebsiteSite site, string action)
    {
        site.Token = Guid.NewGuid(); site.UpdatedAt = DateTime.UtcNow; site.UpdatedBy = User; Audit(action, site.Token);
    }
    public async Task<WebsiteDraftDto> Handle(PublishWebsiteCommand command, CancellationToken ct)
    {
        var site = await Site(ct); CheckToken(site, command.ExpectedToken);
        var snapshot = Parse(site.DraftJson); await Validate(snapshot, true, ct);
        var revision = new PublicWebsiteRevision { Id = Guid.NewGuid(), CompanyId = Owner, ContentJson = site.DraftJson, PublishedAt = DateTime.UtcNow, PublishedBy = User };
        db.Add(revision); site.PublishedRevisionId = revision.Id;
        var ids = Values(snapshot).Where(x => x.MediaId.HasValue).Select(x => x.MediaId!.Value).ToHashSet();
        foreach (var media in await db.Set<PublicWebsiteMedia>().Where(x => x.CompanyId == Owner && ids.Contains(x.Id)).ToListAsync(ct)) media.WasPublished = true;
        Touch(site, "Publish"); await db.SaveChangesAsync(ct); return Dto(site);
    }
    public async Task<WebsiteDraftDto> Handle(RestoreWebsiteCommand command, CancellationToken ct)
    {
        var site = await Site(ct); CheckToken(site, command.ExpectedToken);
        var revision = await db.Set<PublicWebsiteRevision>().SingleOrDefaultAsync(x => x.Id == command.RevisionId && x.CompanyId == Owner, ct)
            ?? throw new NotFoundException("Website revision not found.");
        site.DraftJson = revision.ContentJson; Touch(site, "Restore " + revision.Id);
        await db.SaveChangesAsync(ct); return Dto(site);
    }
    public async Task<List<WebsiteRevisionDto>> Handle(ListWebsiteRevisionsQuery query, CancellationToken ct)
    {
        Authorize(); return await db.Set<PublicWebsiteRevision>().AsNoTracking().Where(x => x.CompanyId == Owner)
            .OrderByDescending(x => x.PublishedAt).Select(x => new WebsiteRevisionDto(x.Id, x.PublishedAt, x.PublishedBy)).ToListAsync(ct);
    }
    private static IEnumerable<WebsiteValue> Values(WebsiteSnapshot s) => s.Fields.Values.Concat(s.Collections.Values.SelectMany(x => x).SelectMany(x => x.Fields.Values));
    private async Task Validate(WebsiteSnapshot snapshot, bool publishing, CancellationToken ct)
    {
        var manifest = WebsiteContentCatalog.Manifest;
        if (snapshot.SchemaVersion != 1 || snapshot.Fields == null || snapshot.Pages == null || snapshot.Collections == null)
            throw new BadRequestException("Unsupported website content schema.");
        if (JsonSerializer.Serialize(snapshot).Length > 2 * 1024 * 1024) throw new BadRequestException("Website content exceeds the 2 MiB snapshot limit.");
        var definitions = manifest.Fields.ToDictionary(x => x.Key);
        if (!snapshot.Fields.Keys.ToHashSet().SetEquals(definitions.Keys) || !snapshot.Pages.Keys.ToHashSet().SetEquals(manifest.Pages.Keys) || !snapshot.Collections.Keys.ToHashSet().SetEquals(manifest.Collections.Keys))
            throw new BadRequestException("Website fields, pages and collections must match the approved design.");
        foreach (var (page, sections) in snapshot.Pages)
            if (sections == null || sections.Any(x => x == null) || !sections.Select(x => x.Key).ToHashSet().SetEquals(manifest.Pages[page]) || sections.Count != manifest.Pages[page].Count)
                throw new BadRequestException("Invalid page sections.");
        foreach (var (collection, items) in snapshot.Collections)
        {
            if (items == null || items.Count > 100 || items.Any(x => x == null || x.Fields == null || x.Id == Guid.Empty || !manifest.Collections[collection].Contains(x.TemplateKey)) || items.Select(x => x.Id).Distinct().Count() != items.Count)
                throw new BadRequestException("Invalid collection items.");
            if (items.Where(x => x.Original).Select(x => x.TemplateKey).Distinct().Count() != items.Count(x => x.Original))
                throw new BadRequestException("Duplicate original item anchors.");
            foreach (var item in items)
                foreach (var (key, value) in item.Fields)
                {
                    if (!definitions.TryGetValue(key, out var d) || !key.StartsWith(item.TemplateKey + ".", StringComparison.Ordinal)) throw new BadRequestException("Invalid collection field.");
                    ValidateValue(d, value, publishing);
                }
        }
        var itemIds = snapshot.Collections.Values.SelectMany(x => x).Select(x => x.Id).ToList();
        if (itemIds.Distinct().Count() != itemIds.Count) throw new BadRequestException("Repeated items must have unique identifiers.");
        foreach (var (key, value) in snapshot.Fields) ValidateValue(definitions[key], value, publishing);
        var ids = Values(snapshot).Where(x => x.MediaId.HasValue).Select(x => x.MediaId!.Value).Distinct().ToList();
        var media = await db.Set<PublicWebsiteMedia>().Where(x => x.CompanyId == Owner && ids.Contains(x.Id)).ToListAsync(ct);
        if (ids.Count != media.Count) throw new BadRequestException("A media file does not belong to this website.");
        foreach (var (key, value) in snapshot.Fields.Concat(snapshot.Collections.Values.SelectMany(x => x).SelectMany(x => x.Fields)))
            if (value.MediaId is Guid id && !Accepts(definitions[key].Kind, media.Single(x => x.Id == id).ContentType)) throw new BadRequestException("The uploaded file does not match this field.");
        if (publishing && media.Any(x => !File.Exists(PhysicalPath(x.StorageKey)))) throw new BadRequestException("A referenced media file is missing from storage.");
    }
    private static bool Accepts(string kind, string type) => kind switch
    {
        "image" => type.StartsWith("image/"), "pdf" => type == "application/pdf", "video" => type.StartsWith("video/"),
        "audio" => type.StartsWith("audio/"), "download" => type == "application/zip", _ => false
    };
    private static void ValidateValue(WebsiteFieldDefinition d, WebsiteValue value, bool publishing)
    {
        if (value == null || value.Ar == null || value.En == null || value.Ar.Length > 10000 || value.En.Length > 10000) throw new BadRequestException("Invalid website field value.");
        if (publishing && d.Required && (string.IsNullOrWhiteSpace(value.Ar) || string.IsNullOrWhiteSpace(value.En)))
            throw new BadRequestException("Complete both languages for: " + d.En[..Math.Min(80, d.En.Length)]);
        if (d.Kind == "link") foreach (var link in new[] { value.Ar, value.En })
            if (link.Length > 0 && !SafeLink(link)) throw new BadRequestException("Use a local path, anchor, HTTPS, mailto or tel link.");
        if (d.Kind is "image" or "pdf" or "video" or "audio" or "download")
            if (value.Ar != d.Ar || value.En != d.En) throw new BadRequestException("Replace media through the upload control.");
        if (value.MediaId.HasValue && d.Kind is not ("image" or "pdf" or "video" or "audio" or "download")) throw new BadRequestException("This field cannot contain media.");
    }
    private static bool SafeLink(string value) => !value.Any(char.IsControl) && !value.Contains('\\') &&
        ((value.StartsWith('/') && !value.StartsWith("//")) || value.StartsWith('#') ||
        (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "https" or "mailto" or "tel"));
    private string PhysicalPath(string key)
    {
        if (Path.GetFileName(key) != key || key.Contains('\\') || key.Contains('/')) throw new BadRequestException("Invalid storage key.");
        var root = Path.GetFullPath(options.Value.StorageRoot, environment.ContentRootPath);
        if (environment.WebRootPath is { } webRoot && (root + Path.DirectorySeparatorChar).StartsWith(Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Website uploads must be stored outside wwwroot.");
        return Path.Combine(root, key);
    }
    public async Task<WebsiteMediaDto> Handle(UploadWebsiteMediaCommand command, CancellationToken ct)
    {
        Authorize(); var file = command.File;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var type = extension switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".gif" => "image/gif", ".webp" => "image/webp", ".pdf" => "application/pdf", ".mp4" => "video/mp4", ".webm" => "video/webm", ".mp3" => "audio/mpeg", ".zip" => "application/zip", _ => "" };
        var limit = type.StartsWith("image/") ? options.Value.ImageMaxBytes : type == "application/pdf" ? options.Value.PdfMaxBytes : options.Value.MediaMaxBytes;
        if (type == "" || !string.Equals(file.ContentType, type, StringComparison.OrdinalIgnoreCase) || file.Length <= 0 || file.Length > limit) throw new BadRequestException("Unsupported file type or file exceeds the upload size limit.");
        await using var input = file.OpenReadStream(); var header = new byte[32]; var count = await input.ReadAtLeastAsync(header, 32, throwOnEndOfStream: false, ct);
        if (!Signature(type, header.AsSpan(0, count))) throw new BadRequestException("File signature does not match the selected file type.");
        var id = Guid.NewGuid(); var key = id.ToString("N") + extension; var path = PhysicalPath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        try
        {
            await using (var output = File.Create(path))
            {
                await output.WriteAsync(header.AsMemory(0, count), ct); await input.CopyToAsync(output, ct);
                if (output.Length != file.Length || output.Length > limit) throw new BadRequestException("Uploaded file length is invalid.");
            }
            var name = string.Concat(Path.GetFileName(file.FileName).Where(c => !char.IsControl(c))); if (name.Length > 255) name = name[^255..];
            db.Add(new PublicWebsiteMedia { Id = id, CompanyId = Owner, Name = name, ContentType = type, Size = file.Length, StorageKey = key, UploadedAt = DateTime.UtcNow, UploadedBy = User });
            Audit("Upload " + id, id); await db.SaveChangesAsync(ct);
            return new(id, name, type, file.Length);
        }
        catch { if (File.Exists(path)) File.Delete(path); throw; }
    }
    private static bool Signature(string type, ReadOnlySpan<byte> b) => type switch
    {
        "image/jpeg" => b.StartsWith(new byte[] { 255, 216, 255 }),
        "image/png" => b.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
        "image/gif" => b.StartsWith("GIF87a"u8) || b.StartsWith("GIF89a"u8),
        "image/webp" => b.Length >= 12 && b[..4].SequenceEqual("RIFF"u8) && b.Slice(8, 4).SequenceEqual("WEBP"u8),
        "application/pdf" => b.StartsWith("%PDF-"u8),
        "video/mp4" => b.Length >= 12 && b.Slice(4, 4).SequenceEqual("ftyp"u8),
        "video/webm" => b.StartsWith(new byte[] { 26, 69, 223, 163 }),
        "audio/mpeg" => b.StartsWith("ID3"u8) || (b.Length >= 2 && b[0] == 255 && (b[1] & 224) == 224),
        "application/zip" => b.StartsWith(new byte[] { 80, 75, 3, 4 }), _ => false
    };
    public async Task<WebsiteMediaFile> Handle(ReadWebsiteMediaQuery query, CancellationToken ct)
    {
        if (query.Draft) Authorize();
        var media = await db.Set<PublicWebsiteMedia>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == query.Id && x.CompanyId == Owner && (query.Draft || x.WasPublished), ct)
            ?? throw new NotFoundException("Website media not found.");
        var path = PhysicalPath(media.StorageKey); if (!File.Exists(path)) throw new NotFoundException("Website media file is missing.");
        accessor.HttpContext!.Response.Headers["X-Content-Type-Options"] = "nosniff";
        accessor.HttpContext.Response.Headers.CacheControl = query.Draft ? "private, no-store" : "public, max-age=31536000, immutable";
        if (media.ContentType is "application/pdf" or "application/zip") accessor.HttpContext.Response.Headers.ContentDisposition = "attachment";
        return new(File.OpenRead(path), media.ContentType, media.Name);
    }
}
