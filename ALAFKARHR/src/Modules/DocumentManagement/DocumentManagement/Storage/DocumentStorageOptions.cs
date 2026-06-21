namespace DocumentManagement.Storage;

public class DocumentStorageOptions
{
    public const string SectionName = "DocumentManagement:Storage";

    public string Provider { get; set; } = DocumentStorageProviders.LocalFileSystem;
    public string? LocalRootPath { get; set; }
    public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024;
    public List<string> AllowedContentTypes { get; set; } = DefaultAllowedContentTypes();
    public List<string> AllowedExtensions { get; set; } = DefaultAllowedExtensions();

    public static List<string> DefaultAllowedContentTypes() =>
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/svg+xml",
        "text/plain",
        "text/csv",
        "application/rtf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "video/mp4",
        "video/mpeg",
        "video/quicktime",
        "video/webm",
        "audio/mpeg",
        "audio/mp4",
        "audio/wav",
        "audio/webm",
        "application/zip",
        "application/octet-stream"
    ];

    public static List<string> DefaultAllowedExtensions() =>
    [
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp",
        ".svg",
        ".txt",
        ".csv",
        ".rtf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".mp4",
        ".mpeg",
        ".mov",
        ".webm",
        ".mp3",
        ".m4a",
        ".wav",
        ".zip",
        ".bin"
    ];
}

public static class DocumentStorageProviders
{
    public const string LocalFileSystem = "LocalFileSystem";
}
