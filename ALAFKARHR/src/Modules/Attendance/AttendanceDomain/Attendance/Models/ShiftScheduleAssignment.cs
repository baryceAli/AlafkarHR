using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class ShiftScheduleAssignment : Entity<Guid>
{
    public Guid ScheduleId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid ShiftId { get; private set; }
    public DateTime WorkDate { get; private set; }
    public string? Notes { get; private set; }

    private ShiftScheduleAssignment() { }

    public static ShiftScheduleAssignment Create(Guid id, UpsertShiftScheduleAssignmentDto dto)
    {
        Validate(dto.ScheduleId, dto.CompanyId, dto.EmployeeId, dto.ShiftId);
        return new ShiftScheduleAssignment
        {
            Id = id,
            ScheduleId = dto.ScheduleId,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            ShiftId = dto.ShiftId,
            WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(UpsertShiftScheduleAssignmentDto dto, string? modifiedBy)
    {
        Validate(dto.ScheduleId, dto.CompanyId, dto.EmployeeId, dto.ShiftId);
        ScheduleId = dto.ScheduleId;
        CompanyId = dto.CompanyId;
        EmployeeId = dto.EmployeeId;
        ShiftId = dto.ShiftId;
        WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date;
        Notes = dto.Notes;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Delete(string? deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid scheduleId, Guid companyId, Guid employeeId, Guid shiftId)
    {
        if (scheduleId == Guid.Empty || companyId == Guid.Empty || employeeId == Guid.Empty || shiftId == Guid.Empty)
        {
            throw new BadRequestException("Schedule, company, employee, and shift are required for roster assignment.");
        }
    }
}
