using System.Text;
using System.Text.RegularExpressions;
using Shared.Exceptions;

namespace GeneralSettings.GeneralSettings.Features.PublicWebsite;

internal static class WebsiteManagedStorage
{
    public static string FolderName(string? name)
    {
        if (name == null || !Regex.IsMatch(name, "\\A[A-Za-z0-9_-]{1,64}\\z", RegexOptions.CultureInvariant) ||
            Regex.IsMatch(name, "\\A(?:CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9])\\z", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            throw new BadRequestException("Use 1–64 letters, digits, hyphens or underscores for the folder name. Paths and reserved names are not allowed.");
        return name.ToLowerInvariant();
    }
    public static void NoRedirects(string path)
    {
        // Inspect existing ancestors too: an approved root can itself be reached through a junction.
        for (DirectoryInfo? directory = new(Path.GetFullPath(path)); directory != null; directory = directory.Parent)
        {
            if (directory.LinkTarget != null || (directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0))
                throw new BadRequestException("Managed storage cannot use symbolic links or redirected directories.");
            if (File.Exists(directory.FullName)) throw new BadRequestException("A storage directory path refers to a file.");
        }
    }
    public static string ChildPath(string root, Guid parentCompanyId, string folderName)
    {
        if (parentCompanyId == Guid.Empty) throw new BadRequestException("A parent-company storage scope is required.");
        var path = Path.GetFullPath(Path.Combine(root, parentCompanyId.ToString("N"), FolderName(folderName)));
        if (!(path + Path.DirectorySeparatorChar).StartsWith(Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("The media folder must remain inside its managed storage root.");
        NoRedirects(path);
        return path;
    }
    public static async Task ProbeAsync(string path, bool managed, CancellationToken ct)
    {
        if (managed) NoRedirects(path);
        Directory.CreateDirectory(path);
        if (managed) NoRedirects(path);
        var temporary = Path.Combine(path, "website-readiness-" + Guid.NewGuid().ToString("N") + ".tmp");
        var created = false;
        try
        {
            var expected = Encoding.UTF8.GetBytes("PublicWebsite storage check");
            await using (var output = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                created = true;
                await output.WriteAsync(expected, ct);
            }
            if (managed) NoRedirects(path);
            if (!(await File.ReadAllBytesAsync(temporary, ct)).SequenceEqual(expected)) throw new IOException("Storage verification failed.");
            File.Delete(temporary);
        }
        finally
        {
            // Delete only the generated probe, and never follow a redirected directory during cleanup.
            try { if (created) { if (managed) NoRedirects(path); if (File.Exists(temporary)) File.Delete(temporary); } }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or BadRequestException) { }
        }
    }
}
