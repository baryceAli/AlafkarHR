namespace Contracts.Contracts.Features.Attachments;

public class AttachmentHandlers(ContractsDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment) :
    ICommandHandler<UploadContractAttachmentCommand, UploadContractAttachmentResult>,
    ICommandHandler<DeleteContractAttachmentCommand, DeleteContractAttachmentResult>
{
    public async Task<UploadContractAttachmentResult> Handle(UploadContractAttachmentCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.ContractId, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.ContractId}");

        var filePath = await ContractFeatureHelpers.SaveFileAsync(request.File, environment, "Attachments", contract.Id, ContractFeatureHelpers.AllowedDocumentContentTypes(), cancellationToken);
        var attachment = ContractAttachment.Create(contract.Id, request.File.FileName, filePath, request.File.ContentType, request.File.Length, request.Kind, ContractFeatureHelpers.CurrentUserGuid(httpContextAccessor));
        contract.AddAttachment(attachment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UploadContractAttachmentResult(attachment.Id, attachment.FilePath);
    }

    public async Task<DeleteContractAttachmentResult> Handle(DeleteContractAttachmentCommand request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == request.ContractId, cancellationToken)
            ?? throw new NotFoundException($"Contract not found: {request.ContractId}");
        contract.RemoveAttachment(request.AttachmentId, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteContractAttachmentResult(true);
    }
}
