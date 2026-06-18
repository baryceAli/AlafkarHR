namespace Contracts.Contracts.Models;

public class ContractAttachment : Entity<Guid>
{
    private ContractAttachment()
    {
    }

    public Guid ContractId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public ContractAttachmentKind Kind { get; private set; }
    public DateTime UploadedDate { get; private set; }
    public Guid UploadedByUserId { get; private set; }

    public static ContractAttachment Create(Guid contractId, string fileName, string filePath, string contentType, long fileSize, ContractAttachmentKind kind, Guid uploadedByUserId) =>
        new()
        {
            Id = Guid.NewGuid(),
            ContractId = contractId,
            FileName = fileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileSize,
            Kind = kind,
            UploadedDate = DateTime.UtcNow,
            UploadedByUserId = uploadedByUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedByUserId.ToString()
        };

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ContractAttachmentDto ToDto() => new()
    {
        Id = Id,
        ContractId = ContractId,
        FileName = FileName,
        FilePath = FilePath,
        ContentType = ContentType,
        FileSize = FileSize,
        Kind = Kind,
        UploadedDate = UploadedDate,
        UploadedByUserId = UploadedByUserId
    };
}
