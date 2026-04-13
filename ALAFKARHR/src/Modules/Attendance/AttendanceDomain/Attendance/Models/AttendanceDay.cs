using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;


public class AttendanceDay : Aggregate<Guid>
{
    public Guid EmployeeId { get; private set; }

    public DateTime Date { get; private set; }

    public TimeSpan? CheckInTime { get; private set; }
    public TimeSpan? CheckOutTime { get; private set; }

    public int WorkedMinutes { get; private set; }

    public bool IsLate { get; private set; }
    public bool IsAbsent { get; private set; }

    public int LateMinutes { get; private set; }
    public int OvertimeMinutes { get; private set; }

    public Guid CompanyId { get; private set; }

    private AttendanceDay() { }

    public void Calculate(Shift shift)
    {
        if (CheckInTime == null || CheckOutTime == null)
        {
            IsAbsent = true;
            WorkedMinutes = 0;
            return;
        }

        var worked = (CheckOutTime.Value - CheckInTime.Value).TotalMinutes;

        WorkedMinutes = (int)worked;

        var expectedStart = shift.StartTime;
        var late = (CheckInTime.Value - expectedStart).TotalMinutes;

        if (late > shift.GracePeriodMinutes)
        {
            IsLate = true;
            LateMinutes = (int)late;
        }

        var expectedWork = (shift.EndTime - shift.StartTime).TotalMinutes - shift.BreakMinutes;

        if (WorkedMinutes > expectedWork)
        {
            OvertimeMinutes = WorkedMinutes - (int)expectedWork;
        }
    }
}