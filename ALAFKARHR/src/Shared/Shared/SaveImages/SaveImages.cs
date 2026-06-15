using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Shared.SaveImages;

public static class SaveImages
{
    public record SavedUpload(string FileName, string PhysicalPath, string PublicPath);

    public static string SaveBase64Image(string fileNameWithoutExtension, string[] pathSegments, string base64String)
    {
        if (pathSegments == null || !pathSegments.Any())
            throw new ArgumentException("No folders supplied.", nameof(pathSegments));

        if (string.IsNullOrWhiteSpace(base64String))
            throw new ArgumentException("Base64 string cannot be null or empty.", nameof(base64String));

        // --- Clean the input ---
        base64String = base64String.Trim();

        // Strip "data:image/png;base64," if present
        int commaIndex = base64String.IndexOf(',');
        if (commaIndex >= 0)
            base64String = base64String.Substring(commaIndex + 1);

        // Replace URL-encoded characters and remove invalid whitespace
        base64String = base64String
            .Replace('-', '+')
            .Replace('_', '/')
            .Replace("%2F", "/")
            .Replace("%2B", "+")
            .Replace("%3D", "=")
            .Replace("\r", "")
            .Replace("\n", "")
            .Trim();

        // Pad with '=' if needed
        int mod4 = base64String.Length % 4;
        if (mod4 > 0)
            base64String = base64String.PadRight(base64String.Length + (4 - mod4), '=');

        // Optional: remove invalid characters before decoding
        base64String = Regex.Replace(base64String, @"[^A-Za-z0-9\+/=]", "");

        // --- Decode ---
        byte[] imageBytes = Convert.FromBase64String(base64String);

        return SaveBytes(fileNameWithoutExtension, pathSegments, ".png", imageBytes);
    }

    public static async Task<SavedUpload> SaveFormFileAsync(
        IFormFile file,
        string fileNameWithoutExtension,
        string[] physicalPathSegments,
        string publicPath,
        IReadOnlyCollection<string>? allowedExtensions = null,
        IReadOnlyCollection<string>? allowedContentTypes = null,
        CancellationToken cancellationToken = default)
    {
        if (physicalPathSegments == null || !physicalPathSegments.Any())
            throw new ArgumentException("No folders supplied.", nameof(physicalPathSegments));

        if (file.Length == 0)
            throw new ArgumentException("File cannot be empty.", nameof(file));

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File must include an extension.", nameof(file));

        if (allowedExtensions is not null
            && !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("File extension is not allowed.", nameof(file));
        }

        if (allowedContentTypes is not null
            && !allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("File type is not allowed.", nameof(file));
        }

        string folderPath = Path.Combine(physicalPathSegments);
        Directory.CreateDirectory(folderPath);

        var safeFileName = $"{SanitizeFileName(fileNameWithoutExtension)}{extension}";
        var physicalPath = Path.Combine(folderPath, safeFileName);
        await using (var stream = File.Create(physicalPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return new SavedUpload(
            safeFileName,
            physicalPath,
            $"{publicPath.TrimEnd('/')}/{safeFileName}");
    }

    public static bool IsBase64Image(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return true;

        Span<byte> buffer = new Span<byte>(new byte[input.Length]);

        return Convert.TryFromBase64String(input, buffer, out _);
    }

    private static string SaveBytes(string fileNameWithoutExtension, string[] pathSegments, string extension, byte[] bytes)
    {
        string folderPath = Path.Combine(pathSegments);
        Directory.CreateDirectory(folderPath);

        string fileName = $"{SanitizeFileName(fileNameWithoutExtension)}{extension}";
        string filePath = Path.Combine(folderPath, fileName);

        File.WriteAllBytes(filePath, bytes);
        return fileName;
    }

    private static string SanitizeFileName(string fileNameWithoutExtension)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(fileNameWithoutExtension
            .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
            .ToArray());

        return string.IsNullOrWhiteSpace(safeName) ? Guid.NewGuid().ToString("N") : safeName;
    }
}
