namespace Maintenance.WorkOrders.Models;

public class MaintenanceAttachment : Entity<Guid>
{
    public Guid WorkOrderId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public Guid UploadedByUserId { get; private set; }

    private MaintenanceAttachment()
    {
    }

    public static MaintenanceAttachment Create(Guid workOrderId, string fileName, string contentType, string filePath, long fileSize, Guid uploadedByUserId)
    {
        return new MaintenanceAttachment
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrderId,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            FilePath = filePath.Trim(),
            FileSize = fileSize,
            UploadedByUserId = uploadedByUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedByUserId.ToString()
        };
    }
}
