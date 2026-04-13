
using Shared.DDD;


namespace AttendanceDomain.Attendance.Models;

public class Shift : Aggregate<Guid>
{
    public string Name { get; private set; }

    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public int GracePeriodMinutes { get; private set; }
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
            BreakMinutes = breakMinutes,
            CompanyId = companyId
        };
    }
}