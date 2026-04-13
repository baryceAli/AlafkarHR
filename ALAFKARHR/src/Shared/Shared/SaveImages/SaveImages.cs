using System.Text.RegularExpressions;

namespace Shared.SaveImages;

public static class SaveImages
{

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

        // --- Determine save path ---
        string folderPath = Path.Combine(pathSegments);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = $"{fileNameWithoutExtension}.png";
        string filePath = Path.Combine(folderPath, fileName);

        File.WriteAllBytes(filePath, imageBytes);
        return fileName;
    }

}
