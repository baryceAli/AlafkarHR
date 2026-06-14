using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceHoliday : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid? AdministrationId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    private AttendanceHoliday() { }

    public static AttendanceHoliday Create(
        Guid id,
        Guid companyId,
        Guid? administrationId,
        Guid? departmentId,
        DateTime startDate,
        DateTime endDate,
        string? name,
        string? description)
    {
        ValidateDates(startDate, endDate);

        return new AttendanceHoliday
        {
            Id = id,
            CompanyId = companyId,
            AdministrationId = administrationId,
            DepartmentId = departmentId,
            StartDate = UtcDateTime.Normalize(startDate).Date,
            EndDate = UtcDateTime.Normalize(endDate).Date,
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        Guid? administrationId,
        Guid? departmentId,
        DateTime startDate,
        DateTime endDate,
        string? name,
        string? description,
        string? modifiedBy)
    {
        ValidateDates(startDate, endDate);

        AdministrationId = administrationId;
        DepartmentId = departmentId;
        StartDate = UtcDateTime.Normalize(startDate).Date;
        EndDate = UtcDateTime.Normalize(endDate).Date;
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

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (UtcDateTime.Normalize(endDate).Date < UtcDateTime.Normalize(startDate).Date)
        {
            throw new BadRequestException("Holiday end date must be on or after start date.");
        }
    }
}
