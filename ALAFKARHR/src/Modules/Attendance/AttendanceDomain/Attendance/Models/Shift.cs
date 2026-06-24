
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
    public AttendanceBreakMode BreakMode { get; private set; }
    public TimeSpan? BreakStartTime { get; private set; }
    public TimeSpan? BreakEndTime { get; private set; }
    public bool IsBreakPaid { get; private set; } = true;

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
        AttendanceBreakMode breakMode,
        TimeSpan? breakStartTime,
        TimeSpan? breakEndTime,
        bool isBreakPaid,
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
            BreakMode = breakMode,
            BreakStartTime = breakStartTime,
            BreakEndTime = breakEndTime,
            IsBreakPaid = isBreakPaid,
            CompanyId = companyId
        };
    }

    public void Update(
        string name,
        TimeSpan start,
        TimeSpan end,
        int graceMinutes,
        int lateAfterMinutes,
        int prohibitCheckInAfterMinutes,
        int breakMinutes,
        AttendanceBreakMode breakMode,
        TimeSpan? breakStartTime,
        TimeSpan? breakEndTime,
        bool isBreakPaid,
        string? modifiedBy)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
        GracePeriodMinutes = graceMinutes;
        LateAfterMinutes = lateAfterMinutes;
        ProhibitCheckInAfterMinutes = prohibitCheckInAfterMinutes;
        BreakMinutes = breakMinutes;
        BreakMode = breakMode;
        BreakStartTime = breakStartTime;
        BreakEndTime = breakEndTime;
        IsBreakPaid = isBreakPaid;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Delete(string? deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
        ModifiedBy = deletedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public DateTime BuildShiftStart(DateTime workDateUtc)
        => DateTime.SpecifyKind(UtcDateTime.Normalize(workDateUtc).Date.Add(StartTime), DateTimeKind.Utc);

    public DateTime BuildShiftEnd(DateTime workDateUtc)
    {
        var shiftEnd = UtcDateTime.Normalize(workDateUtc).Date.Add(EndTime);
        if (EndTime <= StartTime)
        {
            shiftEnd = shiftEnd.AddDays(1);
        }

        return DateTime.SpecifyKind(shiftEnd, DateTimeKind.Utc);
    }

    public DateTime LateAfter(DateTime shiftStartUtc)
        => UtcDateTime.Normalize(shiftStartUtc).AddMinutes(LateAfterMinutes);

    public DateTime ProhibitCheckInAfter(DateTime shiftStartUtc)
        => UtcDateTime.Normalize(shiftStartUtc).AddMinutes(ProhibitCheckInAfterMinutes);
}
