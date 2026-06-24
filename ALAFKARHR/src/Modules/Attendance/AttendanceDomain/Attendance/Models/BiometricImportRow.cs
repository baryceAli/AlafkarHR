using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class BiometricImportRow : Entity<Guid>
{
    public Guid BatchId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public string? DeviceEmployeeCode { get; private set; }
    public DateTime PunchTimeUtc { get; private set; }
    public bool IsCheckOut { get; private set; }
    public AttendanceImportRowStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }

    private BiometricImportRow() { }

    public static BiometricImportRow Create(Guid id, UpsertBiometricImportRowDto dto)
    {
        Validate(dto.BatchId, dto.CompanyId, dto.EmployeeId, dto.DeviceEmployeeCode);
        return new BiometricImportRow
        {
            Id = id,
            BatchId = dto.BatchId,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            DeviceEmployeeCode = dto.DeviceEmployeeCode,
            PunchTimeUtc = UtcDateTime.Normalize(dto.PunchTimeUtc),
            IsCheckOut = dto.IsCheckOut,
            Status = AttendanceImportRowStatus.Pending,
            ErrorMessage = dto.ErrorMessage,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(UpsertBiometricImportRowDto dto, string? modifiedBy)
    {
        if (Status == AttendanceImportRowStatus.Posted)
        {
            throw new BadRequestException("Posted import rows cannot be updated.");
        }

        Validate(dto.BatchId, dto.CompanyId, dto.EmployeeId, dto.DeviceEmployeeCode);
        BatchId = dto.BatchId;
        CompanyId = dto.CompanyId;
        EmployeeId = dto.EmployeeId;
        DeviceEmployeeCode = dto.DeviceEmployeeCode;
        PunchTimeUtc = UtcDateTime.Normalize(dto.PunchTimeUtc);
        IsCheckOut = dto.IsCheckOut;
        ErrorMessage = dto.ErrorMessage;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Review(bool isAccepted, string? errorMessage, string? reviewedBy)
    {
        if (Status == AttendanceImportRowStatus.Posted)
        {
            throw new BadRequestException("Posted import rows cannot be reviewed.");
        }

        Status = isAccepted ? AttendanceImportRowStatus.Accepted : AttendanceImportRowStatus.Rejected;
        ErrorMessage = errorMessage;
        ModifiedBy = reviewedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkPosted(string? userId)
    {
        if (Status != AttendanceImportRowStatus.Accepted)
        {
            throw new BadRequestException("Only accepted import rows can be posted.");
        }

        Status = AttendanceImportRowStatus.Posted;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid batchId, Guid companyId, Guid? employeeId, string? deviceEmployeeCode)
    {
        if (batchId == Guid.Empty || companyId == Guid.Empty)
        {
            throw new BadRequestException("Batch and company are required for import row.");
        }

        if (!employeeId.HasValue && string.IsNullOrWhiteSpace(deviceEmployeeCode))
        {
            throw new BadRequestException("Import row requires an employee or device employee code.");
        }
    }
}
