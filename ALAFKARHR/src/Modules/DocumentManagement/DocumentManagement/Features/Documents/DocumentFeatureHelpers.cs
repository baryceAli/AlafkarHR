namespace DocumentManagement.Documents.Features;

public static class DocumentFeatureHelpers
{
    public static Guid CurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

        return Guid.TryParse(value, out var userId) ? userId : Guid.Empty;
    }

    public static Guid CurrentCompanyId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirstValue("company_id");
        if (!Guid.TryParse(value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }

    public static bool HasManageAll(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == PermissionList.DocumentManagementPermissions.ManageAll) == true;

    public static IQueryable<DocumentItem> IncludeDetails(this IQueryable<DocumentItem> query) =>
        query.Include(x => x.Versions).Include(x => x.Collaborators);

    public static IQueryable<DocumentItem> ApplyVisibility(this IQueryable<DocumentItem> query, Guid companyId, Guid currentUserId, bool manageAll)
    {
        query = query.Where(x => x.CompanyId == companyId);
        if (manageAll)
            return query;

        return query.Where(x => x.OwnerUserId == currentUserId || x.Collaborators.Any(c => c.UserId == currentUserId));
    }

    public static DocumentCollaborator? ActiveCollaborator(DocumentItem document, Guid userId) =>
        document.Collaborators.FirstOrDefault(x => x.UserId == userId && !x.IsDeleted);

    public static bool CanView(DocumentItem document, Guid currentUserId, bool manageAll) =>
        manageAll || document.OwnerUserId == currentUserId || ActiveCollaborator(document, currentUserId) is not null;

    public static bool CanWrite(DocumentItem document, Guid currentUserId, bool manageAll) =>
        manageAll
        || document.OwnerUserId == currentUserId
        || ActiveCollaborator(document, currentUserId)?.AccessLevel == DocumentAccessLevel.ReadWrite;

    public static bool CanShare(DocumentItem document, Guid currentUserId, bool manageAll) =>
        manageAll || document.OwnerUserId == currentUserId;

    public static void EnsureCanView(DocumentItem document, Guid currentUserId, bool manageAll)
    {
        if (!CanView(document, currentUserId, manageAll))
            throw new ForbiddenException("You do not have access to this document.");
    }

    public static void EnsureCanWrite(DocumentItem document, Guid currentUserId, bool manageAll)
    {
        if (!CanWrite(document, currentUserId, manageAll))
            throw new ForbiddenException("You do not have write access to this document.");
    }

    public static void EnsureCanShare(DocumentItem document, Guid currentUserId, bool manageAll)
    {
        if (!CanShare(document, currentUserId, manageAll))
            throw new ForbiddenException("Only the owner can share this document.");
    }

    public static DocumentItemDto ToListDto(DocumentItem document, Guid currentUserId, bool manageAll)
    {
        var latest = document.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault();
        var collaborator = ActiveCollaborator(document, currentUserId);

        return new DocumentItemDto
        {
            Id = document.Id,
            CompanyId = document.CompanyId,
            OwnerUserId = document.OwnerUserId,
            Title = document.Title,
            Description = document.Description,
            SourceModule = document.SourceModule,
            SourceEntity = document.SourceEntity,
            SourceRecordId = document.SourceRecordId,
            LatestVersionNumber = latest?.VersionNumber ?? 0,
            LatestFileName = latest?.OriginalFileName,
            LatestContentType = latest?.ContentType,
            LatestFileSize = latest?.FileSize,
            LatestUploadedAt = latest?.UploadedAt,
            CreatedAt = document.CreatedAt,
            CollaboratorAccessLevel = collaborator?.AccessLevel,
            CanView = CanView(document, currentUserId, manageAll),
            CanWrite = CanWrite(document, currentUserId, manageAll),
            CanShare = CanShare(document, currentUserId, manageAll),
            CanDelete = CanShare(document, currentUserId, manageAll)
        };
    }

    public static DocumentDetailDto ToDetailDto(DocumentItem document, Guid currentUserId, bool manageAll)
    {
        var dto = ToListDto(document, currentUserId, manageAll).Adapt<DocumentDetailDto>();
        dto.Versions = document.Versions
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => new DocumentVersionDto
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                VersionNumber = x.VersionNumber,
                OriginalFileName = x.OriginalFileName,
                ContentType = x.ContentType,
                FileSize = x.FileSize,
                UploadedByUserId = x.UploadedByUserId,
                UploadedAt = x.UploadedAt
            }).ToList();
        dto.Collaborators = document.Collaborators
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.UserName ?? x.UserId.ToString())
            .Select(x => new DocumentCollaboratorDto
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                UserId = x.UserId,
                UserName = x.UserName,
                AccessLevel = x.AccessLevel,
                CreatedAt = x.CreatedAt
            }).ToList();

        return dto;
    }

    public static HashSet<string> AllowedContentTypes() =>
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp",
            "text/plain",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

    public static async Task<(string StoragePath, long FileSize)> SaveVersionFileAsync(
        IFormFile file,
        IWebHostEnvironment environment,
        Guid companyId,
        Guid documentId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            throw new BadRequestException("Document file is empty.");
        if (file.Length > 10 * 1024 * 1024)
            throw new BadRequestException("Document file size cannot exceed 10 MB.");
        if (!AllowedContentTypes().Contains(file.ContentType))
            throw new BadRequestException("Document file type is not allowed.");

        var root = Path.Combine(environment.ContentRootPath, "App_Data", "DocumentManagement", companyId.ToString(), documentId.ToString(), versionId.ToString());
        Directory.CreateDirectory(root);

        var storagePath = Path.Combine(root, Path.GetFileName(file.FileName));
        await using var stream = File.Create(storagePath);
        await file.CopyToAsync(stream, cancellationToken);
        return (storagePath, file.Length);
    }
}
