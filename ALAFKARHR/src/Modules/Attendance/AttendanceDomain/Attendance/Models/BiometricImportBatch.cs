using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class BiometricImportBatch : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string SourceName { get; private set; } = string.Empty;
    public DateTime ImportedAtUtc { get; private set; }
    public AttendanceImportBatchStatus Status { get; private set; }
    public int TotalRows { get; private set; }
    public int AcceptedRows { get; private set; }
    public int RejectedRows { get; private set; }
    public string? Notes { get; private set; }

    private BiometricImportBatch() { }

    public static BiometricImportBatch Create(Guid id, CreateBiometricImportBatchDto dto)
    {
        if (dto.CompanyId == Guid.Empty || string.IsNullOrWhiteSpace(dto.SourceName))
        {
            throw new BadRequestException("Company and source name are required for import batch.");
        }

        return new BiometricImportBatch
        {
            Id = id,
            CompanyId = dto.CompanyId,
            SourceName = dto.SourceName.Trim(),
            ImportedAtUtc = DateTime.UtcNow,
            Status = AttendanceImportBatchStatus.Draft,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetCounts(int totalRows, int acceptedRows, int rejectedRows)
    {
        TotalRows = totalRows;
        AcceptedRows = acceptedRows;
        RejectedRows = rejectedRows;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkReviewed(string? userId)
    {
        if (Status != AttendanceImportBatchStatus.Draft)
        {
            throw new BadRequestException("Only draft import batches can be reviewed.");
        }

        Status = AttendanceImportBatchStatus.Reviewed;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkPosted(string? userId)
    {
        if (Status is AttendanceImportBatchStatus.Cancelled or AttendanceImportBatchStatus.Posted)
        {
            throw new BadRequestException("Import batch cannot be posted.");
        }

        Status = AttendanceImportBatchStatus.Posted;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }
}
