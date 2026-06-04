using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.AddTaskComment;

public record AddTaskCommentCommand(CreateTaskCommentDto Comment) : ICommand<AddTaskCommentResult>;
public record AddTaskCommentResult(Guid Id);

public class AddTaskCommentCommandValidator : AbstractValidator<AddTaskCommentCommand>
{
    public AddTaskCommentCommandValidator()
    {
        RuleFor(x => x.Comment.TaskId).NotEmpty();
        RuleFor(x => x.Comment.Comment).NotEmpty().MaximumLength(2000);
    }
}

public class AddTaskCommentHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AddTaskCommentCommand, AddTaskCommentResult>
{
    public async Task<AddTaskCommentResult> Handle(AddTaskCommentCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems.Include(x => x.Comments).Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == command.Comment.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Comment.TaskId}");

        var comment = TaskComment.Create(task.Id, userId, command.Comment.Comment);
        task.AddComment(comment);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "CommentAdded", null, command.Comment.Comment, task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AddTaskCommentResult(comment.Id);
    }
}
