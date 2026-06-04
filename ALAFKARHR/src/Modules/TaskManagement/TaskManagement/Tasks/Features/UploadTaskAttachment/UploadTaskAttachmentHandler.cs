using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.UploadTaskAttachment;

public record UploadTaskAttachmentCommand(Guid TaskId, IFormFile File) : ICommand<UploadTaskAttachmentResult>;
public record UploadTaskAttachmentResult(Guid Id, string FilePath);

public class UploadTaskAttachmentHandler(
    TaskManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment environment)
    : ICommandHandler<UploadTaskAttachmentCommand, UploadTaskAttachmentResult>
{
    public async Task<UploadTaskAttachmentResult> Handle(UploadTaskAttachmentCommand command, CancellationToken cancellationToken)
    {
        if (command.File.Length == 0)
            throw new BadRequestException("Attachment file is empty.");

        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems.Include(x => x.Attachments).Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == command.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.TaskId}");

        var uploadRoot = Path.Combine(environment.WebRootPath ?? "wwwroot", "Images", "TaskManagement", command.TaskId.ToString());
        Directory.CreateDirectory(uploadRoot);

        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(command.File.FileName)}";
        var physicalPath = Path.Combine(uploadRoot, safeFileName);
        await using (var stream = File.Create(physicalPath))
        {
            await command.File.CopyToAsync(stream, cancellationToken);
        }

        var publicPath = $"/Images/TaskManagement/{command.TaskId}/{safeFileName}";
        var attachment = TaskAttachment.Create(task.Id, command.File.FileName, publicPath, command.File.ContentType, command.File.Length, userId);
        task.AddAttachment(attachment);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "AttachmentUploaded", null, command.File.FileName, task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UploadTaskAttachmentResult(attachment.Id, attachment.FilePath);
    }
}
