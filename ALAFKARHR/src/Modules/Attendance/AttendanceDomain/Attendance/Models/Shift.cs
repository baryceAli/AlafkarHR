
using Shared.DDD;


namespace AttendanceDomain.Attendance.Models;

public class Shift : Aggregate<Guid>
{
    public string Name { get; private set; }

    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public int GracePeriodMinutes { get; private set; }
    public int LateAfterMinutes { get; private set; }
    public int ProhibitCheckInAfterMinutes { get; private set; }
    public int BreakMinutes { get; private set; }

    public bool IsFlexible { get; private set; }

    public Guid CompanyId { get; private set; }

    private Shift() { }

    public static Shift Create(
        Guid id,
        string name,
        TimeSpan start,
        TimeSpan end,
        int graceMinutes,
        int lateAfterMinutes,
        int prohibitCheckInAfterMinutes,
        int breakMinutes,
        Guid companyId)
    {
        return new Shift
        {
            Id = id,
            Name = name,
            StartTime = start,
            EndTime = end,
            GracePeriodMinutes = graceMinutes,
            LateAfterMinutes = lateAfterMinutes,
            ProhibitCheckInAfterMinutes = prohibitCheckInAfterMinutes,
            BreakMinutes = breakMinutes,
            CompanyId = companyId
        };
    }

    public DateTime BuildShiftStart(DateTime workDateUtc)
        => DateTime.SpecifyKind(workDateUtc.Date.Add(StartTime), DateTimeKind.Utc);

    public DateTime BuildShiftEnd(DateTime workDateUtc)
    {
        var shiftEnd = workDateUtc.Date.Add(EndTime);
        if (EndTime <= StartTime)
        {
            shiftEnd = shiftEnd.AddDays(1);
        }

        return DateTime.SpecifyKind(shiftEnd, DateTimeKind.Utc);
    }

    public DateTime LateAfter(DateTime shiftStartUtc)
        => DateTime.SpecifyKind(shiftStartUtc.AddMinutes(LateAfterMinutes), DateTimeKind.Utc);

    public DateTime ProhibitCheckInAfter(DateTime shiftStartUtc)
        => DateTime.SpecifyKind(shiftStartUtc.AddMinutes(ProhibitCheckInAfterMinutes), DateTimeKind.Utc);
}
