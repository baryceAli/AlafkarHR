using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceWorkEntry : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime WorkDate { get; private set; }
    public AttendanceWorkEntryType EntryType { get; private set; }
    public decimal Hours { get; private set; }
    public AttendanceWorkEntryStatus Status { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceModule { get; private set; }
    public string? Notes { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? LockedBy { get; private set; }
    public DateTime? LockedAtUtc { get; private set; }

    private AttendanceWorkEntry() { }

    public static AttendanceWorkEntry Create(Guid id, UpsertAttendanceWorkEntryDto dto)
    {
        Validate(dto.CompanyId, dto.EmployeeId, dto.Hours);
        return new AttendanceWorkEntry
        {
            Id = id,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date,
            EntryType = dto.EntryType,
            Hours = decimal.Round(dto.Hours, 2),
            SourceDocumentId = dto.SourceDocumentId,
            SourceModule = dto.SourceModule,
            Notes = dto.Notes,
            Status = AttendanceWorkEntryStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(UpsertAttendanceWorkEntryDto dto, string? modifiedBy)
    {
        if (Status == AttendanceWorkEntryStatus.Locked)
        {
            throw new BadRequestException("Locked work entries cannot be updated.");
        }

        Validate(dto.CompanyId, dto.EmployeeId, dto.Hours);
        CompanyId = dto.CompanyId;
        EmployeeId = dto.EmployeeId;
        WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date;
        EntryType = dto.EntryType;
        Hours = decimal.Round(dto.Hours, 2);
        SourceDocumentId = dto.SourceDocumentId;
        SourceModule = dto.SourceModule;
        Notes = dto.Notes;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Approve(string? userId)
    {
        if (Status != AttendanceWorkEntryStatus.Draft)
        {
            throw new BadRequestException("Only draft work entries can be approved.");
        }

        Status = AttendanceWorkEntryStatus.Approved;
        ApprovedBy = userId;
        ApprovedAtUtc = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Lock(string? userId)
    {
        if (Status != AttendanceWorkEntryStatus.Approved)
        {
            throw new BadRequestException("Only approved work entries can be locked.");
        }

        Status = AttendanceWorkEntryStatus.Locked;
        LockedBy = userId;
        LockedAtUtc = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid companyId, Guid employeeId, decimal hours)
    {
        if (companyId == Guid.Empty || employeeId == Guid.Empty)
        {
            throw new BadRequestException("Company and employee are required for work entry.");
        }

        if (hours < 0)
        {
            throw new BadRequestException("Work entry hours cannot be negative.");
        }
    }
}
