namespace Contracts.Contracts.Features.Attachments;

public record UploadContractAttachmentCommand(Guid ContractId, ContractAttachmentKind Kind, IFormFile File)
    : ICommand<UploadContractAttachmentResult>;

public record UploadContractAttachmentResult(Guid Id, string FilePath);

public record DeleteContractAttachmentCommand(Guid ContractId, Guid AttachmentId)
    : ICommand<DeleteContractAttachmentResult>;

public record DeleteContractAttachmentResult(bool IsSuccess);
