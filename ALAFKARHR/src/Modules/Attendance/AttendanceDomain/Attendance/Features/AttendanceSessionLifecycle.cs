using AttendanceDomain.Attendance.Models;

namespace AttendanceDomain.Attendance.Features;

internal static class AttendanceSessionLifecycle
{
    public static bool IsCurrentWorkdaySession(AttendanceSessionDto session, DateTime workdayStartUtc, DateTime workdayEndUtc)
        => IsCurrentWorkdaySession(
            session.ShiftStart,
            session.ShiftEnd,
            session.ActualStartTime,
            session.ActualEndTime,
            workdayStartUtc,
            workdayEndUtc);

    public static bool IsCurrentWorkdaySession(
        AttendanceSession session,
        DateTime workdayStartUtc,
        DateTime workdayEndUtc)
        => IsCurrentWorkdaySession(
            session.ShiftStart,
            session.ShiftEnd,
            session.ActualStartTime,
            session.ActualEndTime,
            workdayStartUtc,
            workdayEndUtc);

    public static async Task<bool> AutoCloseStaleSessionsAsync(
        AttendanceDbContext dbContext,
        Guid employeeId,
        DateTime currentWorkdayStartUtc,
        CancellationToken cancellationToken)
    {
        var staleSessions = await dbContext.AttendanceSessions
            .Where(x => x.EmployeeId == employeeId
                && (x.Status == AttendanceSessionStatus.Active || x.Status == AttendanceSessionStatus.OnBreak)
                && x.ShiftStart.Date < currentWorkdayStartUtc.Date)
            .ToListAsync(cancellationToken);

        foreach (var session in staleSessions)
        {
            session.AutoCloseMissingCheckOut();
        }

        return staleSessions.Count > 0;
    }

    private static bool IsCurrentWorkdaySession(
        DateTime shiftStartUtc,
        DateTime shiftEndUtc,
        DateTime? actualStartUtc,
        DateTime? actualEndUtc,
        DateTime workdayStartUtc,
        DateTime workdayEndUtc)
        => (shiftStartUtc >= workdayStartUtc && shiftStartUtc < workdayEndUtc)
            || (shiftStartUtc < workdayEndUtc && shiftEndUtc > workdayStartUtc)
            || (actualStartUtc.HasValue && actualStartUtc.Value >= workdayStartUtc && actualStartUtc.Value < workdayEndUtc)
            || (actualEndUtc.HasValue && actualEndUtc.Value >= workdayStartUtc && actualEndUtc.Value < workdayEndUtc);
}
