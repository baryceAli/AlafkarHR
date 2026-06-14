using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceBreakPolicy : Entity<Guid>
{
    public ShiftAssignmentScope Scope { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? AdministrationId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public bool IsEnabled { get; private set; }
    public AttendanceBreakMode BreakMode { get; private set; }
    public TimeSpan? BreakStartTime { get; private set; }
    public TimeSpan? BreakEndTime { get; private set; }
    public int AllowedDurationMinutes { get; private set; }
    public bool IsPaid { get; private set; }

    private AttendanceBreakPolicy() { }

    public static AttendanceBreakPolicy Create(Guid id, UpsertAttendanceBreakPolicyDto dto)
    {
        Validate(dto);

        return new AttendanceBreakPolicy
        {
            Id = id,
            Scope = dto.Scope,
            CompanyId = dto.CompanyId,
            AdministrationId = dto.AdministrationId,
            DepartmentId = dto.DepartmentId,
            EmployeeId = dto.EmployeeId,
            IsEnabled = dto.IsEnabled,
            BreakMode = dto.BreakMode,
            BreakStartTime = dto.BreakStartTime,
            BreakEndTime = dto.BreakEndTime,
            AllowedDurationMinutes = dto.AllowedDurationMinutes,
            IsPaid = dto.IsPaid,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(UpsertAttendanceBreakPolicyDto dto, string? modifiedBy)
    {
        Validate(dto);

        Scope = dto.Scope;
        AdministrationId = dto.AdministrationId;
        DepartmentId = dto.DepartmentId;
        EmployeeId = dto.EmployeeId;
        IsEnabled = dto.IsEnabled;
        BreakMode = dto.BreakMode;
        BreakStartTime = dto.BreakStartTime;
        BreakEndTime = dto.BreakEndTime;
        AllowedDurationMinutes = dto.AllowedDurationMinutes;
        IsPaid = dto.IsPaid;
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

    private static void Validate(UpsertAttendanceBreakPolicyDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for break policy.");
        }

        if (dto.AllowedDurationMinutes < 0)
        {
            throw new BadRequestException("Allowed break duration cannot be negative.");
        }

        if (dto.BreakMode == AttendanceBreakMode.Strict)
        {
            if (!dto.BreakStartTime.HasValue || !dto.BreakEndTime.HasValue)
            {
                throw new BadRequestException("Strict break mode requires configured break start and end times.");
            }

            if (dto.BreakEndTime <= dto.BreakStartTime)
            {
                throw new BadRequestException("Break end time must be after break start time.");
            }
        }

        if (dto.Scope == ShiftAssignmentScope.Employee && !dto.EmployeeId.HasValue)
        {
            throw new BadRequestException("Employee break policy requires an employee.");
        }

        if (dto.Scope == ShiftAssignmentScope.Department && !dto.DepartmentId.HasValue)
        {
            throw new BadRequestException("Department break policy requires a department.");
        }

        if (dto.Scope == ShiftAssignmentScope.Administration && !dto.AdministrationId.HasValue)
        {
            throw new BadRequestException("Administration break policy requires an administration.");
        }
    }
}
