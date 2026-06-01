namespace TaskManagement.Tasks.Models;

public class TaskAttachment : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public DateTime UploadedDate { get; private set; }
    public Guid UploadedByUserId { get; private set; }

    private TaskAttachment()
    {
    }

    public static TaskAttachment Create(Guid taskId, string fileName, string filePath, string contentType, long fileSize, Guid uploadedByUserId)
    {
        return new TaskAttachment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = fileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileSize,
            UploadedDate = DateTime.UtcNow,
            UploadedByUserId = uploadedByUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedByUserId.ToString()
        };
    }
}
