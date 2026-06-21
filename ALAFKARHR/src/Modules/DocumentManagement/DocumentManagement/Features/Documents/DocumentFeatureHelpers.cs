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

    public static bool HasDelete(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == PermissionList.DocumentManagementPermissions.Delete) == true;

    public static bool HasView(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User.Claims.Any(c =>
            c.Value == PermissionList.DocumentManagementPermissions.View
            || c.Value == PermissionList.DocumentManagementPermissions.Select
            || c.Value == PermissionList.DocumentManagementPermissions.ManageAll) == true;

    public static bool HasConfigure(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == PermissionList.DocumentManagementPermissions.Configure) == true;

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

    public static DocumentItemDto ToListDto(DocumentItem document, Guid currentUserId, bool manageAll, bool hasDelete)
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
            CanDelete = CanShare(document, currentUserId, manageAll),
            CanDeleteStorage = hasDelete && CanShare(document, currentUserId, manageAll)
        };
    }

    public static DocumentDetailDto ToDetailDto(DocumentItem document, Guid currentUserId, bool manageAll, bool hasDelete)
    {
        var dto = ToListDto(document, currentUserId, manageAll, hasDelete).Adapt<DocumentDetailDto>();
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

    public static void ValidateVersionFile(IFormFile file, DocumentUploadOptionsDto options)
    {
        if (file.Length == 0)
            throw new BadRequestException("Document file is empty.");

        if (file.Length > options.MaxFileSizeBytes)
            throw new BadRequestException($"Document file size cannot exceed {FormatFileSize(options.MaxFileSizeBytes)}.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            throw new BadRequestException("Document file must include an extension.");

        var allowedExtensions = new HashSet<string>(options.AllowedExtensions, StringComparer.OrdinalIgnoreCase);
        if (!allowedExtensions.Contains(extension))
            throw new BadRequestException("Document file extension is not allowed.");

        var allowedContentTypes = new HashSet<string>(options.AllowedContentTypes, StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(file.ContentType) || !allowedContentTypes.Contains(file.ContentType))
            throw new BadRequestException("Document file type is not allowed.");
    }

    public static string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024d:0.#} KB";
        return $"{bytes / 1024d / 1024d:0.#} MB";
    }

}
