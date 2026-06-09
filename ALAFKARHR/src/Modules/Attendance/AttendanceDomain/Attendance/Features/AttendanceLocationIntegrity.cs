namespace AttendanceDomain.Attendance.Features;

internal static class AttendanceLocationIntegrity
{
    public static void EnsureTrusted(bool isMockedLocation, string? note)
    {
        if (!isMockedLocation)
        {
            return;
        }

        var details = string.IsNullOrWhiteSpace(note) ? "Mocked or spoofed location was detected." : note.Trim();
        throw new BadRequestException($"Attendance location rejected. {details}");
    }

    public static string SuspiciousReason(string? note)
    {
        return string.IsNullOrWhiteSpace(note)
            ? "Mocked or spoofed location was detected."
            : note.Trim();
    }
}
