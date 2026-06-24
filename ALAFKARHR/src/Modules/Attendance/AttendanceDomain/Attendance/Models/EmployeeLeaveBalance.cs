using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class LegacyAttendanceEmployeeLeaveBalance : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public int Year { get; private set; }
    public decimal AnnualLeaveDays { get; private set; }
    public decimal TakenDays { get; private set; }
    public decimal CarriedForwardDays { get; private set; }
    public bool AllowCarryForward { get; private set; }
    public decimal MaxCarryForwardDays { get; private set; }
}
