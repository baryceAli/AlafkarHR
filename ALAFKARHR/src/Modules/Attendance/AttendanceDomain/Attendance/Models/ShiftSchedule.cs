using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class ShiftSchedule : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public AttendanceRosterStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }
    public string? PublishedBy { get; private set; }
    public DateTime? LockedAtUtc { get; private set; }
    public string? LockedBy { get; private set; }

    private ShiftSchedule() { }

    public static ShiftSchedule Create(Guid id, UpsertShiftScheduleDto dto)
    {
        Validate(dto.CompanyId, dto.Name, dto.StartDate, dto.EndDate);
        return new ShiftSchedule
        {
            Id = id,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            StartDate = UtcDateTime.Normalize(dto.StartDate).Date,
            EndDate = UtcDateTime.Normalize(dto.EndDate).Date,
            Notes = dto.Notes,
            Status = AttendanceRosterStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(UpsertShiftScheduleDto dto, string? modifiedBy)
    {
        if (Status is AttendanceRosterStatus.Locked or AttendanceRosterStatus.Cancelled)
        {
            throw new BadRequestException("Locked or cancelled rosters cannot be updated.");
        }

        Validate(dto.CompanyId, dto.Name, dto.StartDate, dto.EndDate);
        CompanyId = dto.CompanyId;
        Name = dto.Name.Trim();
        StartDate = UtcDateTime.Normalize(dto.StartDate).Date;
        EndDate = UtcDateTime.Normalize(dto.EndDate).Date;
        Notes = dto.Notes;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Publish(string? userId)
    {
        if (Status != AttendanceRosterStatus.Draft)
        {
            throw new BadRequestException("Only draft rosters can be published.");
        }

        Status = AttendanceRosterStatus.Published;
        PublishedBy = userId;
        PublishedAtUtc = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Lock(string? userId)
    {
        if (Status != AttendanceRosterStatus.Published)
        {
            throw new BadRequestException("Only published rosters can be locked.");
        }

        Status = AttendanceRosterStatus.Locked;
        LockedBy = userId;
        LockedAtUtc = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Cancel(string? userId)
    {
        if (Status == AttendanceRosterStatus.Locked)
        {
            throw new BadRequestException("Locked rosters cannot be cancelled.");
        }

        Status = AttendanceRosterStatus.Cancelled;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid companyId, string name, DateTime startDate, DateTime endDate)
    {
        if (companyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for shift schedule.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Schedule name is required.");
        }

        if (UtcDateTime.Normalize(endDate).Date < UtcDateTime.Normalize(startDate).Date)
        {
            throw new BadRequestException("Schedule end date must be on or after start date.");
        }
    }
}
