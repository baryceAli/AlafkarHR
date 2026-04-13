using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;


public class EmployeeShift : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid ShiftId { get; private set; }

    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    public bool IsActive { get; private set; }

    public Guid CompanyId { get; private set; }

    private EmployeeShift() { }

    public static EmployeeShift Assign(
        Guid id,
        Guid employeeId,
        Guid shiftId,
        DateTime from,
        Guid companyId)
    {
        return new EmployeeShift
        {
            Id = id,
            EmployeeId = employeeId,
            ShiftId = shiftId,
            EffectiveFrom = from,
            IsActive = true,
            CompanyId = companyId
        };
    }
}