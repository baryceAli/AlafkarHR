using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceHoliday : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public AttendanceHolidayType HolidayType { get; private set; } = AttendanceHolidayType.PublicHoliday;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsRecurringYearly { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    private AttendanceHoliday() { }

    public static AttendanceHoliday Create(
        Guid id,
        Guid companyId,
        AttendanceHolidayType holidayType,
        DateTime startDate,
        DateTime endDate,
        bool isRecurringYearly,
        bool isActive,
        string? name,
        string? description)
    {
        Validate(companyId, startDate, endDate);

        return new AttendanceHoliday
        {
            Id = id,
            CompanyId = companyId,
            HolidayType = holidayType,
            StartDate = UtcDateTime.Normalize(startDate).Date,
            EndDate = UtcDateTime.Normalize(endDate).Date,
            IsRecurringYearly = isRecurringYearly,
            IsActive = isActive,
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        AttendanceHolidayType holidayType,
        DateTime startDate,
        DateTime endDate,
        bool isRecurringYearly,
        bool isActive,
        string? name,
        string? description,
        string? modifiedBy)
    {
        Validate(CompanyId, startDate, endDate);

        HolidayType = holidayType;
        StartDate = UtcDateTime.Normalize(startDate).Date;
        EndDate = UtcDateTime.Normalize(endDate).Date;
        IsRecurringYearly = isRecurringYearly;
        IsActive = isActive;
        Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
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

    private static void Validate(Guid companyId, DateTime startDate, DateTime endDate)
    {
        if (companyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for attendance holidays.");
        }

        if (UtcDateTime.Normalize(endDate).Date < UtcDateTime.Normalize(startDate).Date)
        {
            throw new BadRequestException("Holiday end date must be on or after start date.");
        }
    }
}
