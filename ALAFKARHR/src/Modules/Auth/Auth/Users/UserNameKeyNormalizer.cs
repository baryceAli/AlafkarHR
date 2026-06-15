using System.Globalization;

namespace Auth.Users;

internal static class UserNameKeyNormalizer
{
    public static string Normalize(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return string.Empty;

        var normalized = userName.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category is UnicodeCategory.Control
                or UnicodeCategory.Format
                or UnicodeCategory.NonSpacingMark
                or UnicodeCategory.SpacingCombiningMark
                or UnicodeCategory.EnclosingMark)
            {
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
