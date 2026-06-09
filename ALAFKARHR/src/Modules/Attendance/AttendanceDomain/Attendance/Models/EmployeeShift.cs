using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;


public class EmployeeShift : Entity<Guid>
{
    public Guid? EmployeeId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? AdministrationId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid ShiftId { get; private set; }
    public ShiftAssignmentScope Scope { get; private set; }

    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    public bool IsActive { get; private set; }

    private EmployeeShift() { }

    public static EmployeeShift Assign(
        Guid id,
        Guid shiftId,
        ShiftAssignmentScope scope,
        Guid companyId,
        Guid? administrationId,
        Guid? departmentId,
        Guid? employeeId,
        DateTime effectiveFrom,
        DateTime? effectiveTo)
    {
        ValidateScope(scope, companyId, administrationId, departmentId, employeeId);

        return new EmployeeShift
        {
            Id = id,
            ShiftId = shiftId,
            Scope = scope,
            CompanyId = companyId,
            AdministrationId = administrationId,
            DepartmentId = departmentId,
            EmployeeId = employeeId,
            EffectiveFrom = UtcDateTime.Normalize(effectiveFrom),
            EffectiveTo = UtcDateTime.Normalize(effectiveTo),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool AppliesOn(DateTime workDateUtc)
        => IsActive
            && !IsDeleted
            && EffectiveFrom <= workDateUtc
            && (!EffectiveTo.HasValue || EffectiveTo.Value >= workDateUtc);

    public void Close(DateTime effectiveTo)
    {
        EffectiveTo = UtcDateTime.Normalize(effectiveTo);
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
    }

    private static void ValidateScope(
        ShiftAssignmentScope scope,
        Guid companyId,
        Guid? administrationId,
        Guid? departmentId,
        Guid? employeeId)
    {
        if (companyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for shift assignment.");
        }

        switch (scope)
        {
            case ShiftAssignmentScope.Employee when !employeeId.HasValue:
                throw new BadRequestException("Employee is required for employee shift assignment.");
            case ShiftAssignmentScope.Department when !departmentId.HasValue:
                throw new BadRequestException("Department is required for department shift assignment.");
            case ShiftAssignmentScope.Administration when !administrationId.HasValue:
                throw new BadRequestException("Administration is required for administration shift assignment.");
        }
    }
}
