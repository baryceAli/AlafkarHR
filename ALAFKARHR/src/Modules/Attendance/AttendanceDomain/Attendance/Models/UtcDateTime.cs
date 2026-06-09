namespace AttendanceDomain.Attendance.Models;

internal static class UtcDateTime
{
    public static DateTime Normalize(DateTime value)
        => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    public static DateTime? Normalize(DateTime? value)
        => value.HasValue ? Normalize(value.Value) : null;
}
