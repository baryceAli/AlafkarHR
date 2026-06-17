using TaskManagement.Contracts.Features.GetTaskById;
using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.GetTaskById;

public class GetTaskByIdHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTaskByIdQuery, GetTaskByIdResult>
{
    public async Task<GetTaskByIdResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.TaskItems
            .Include(x => x.Comments)
            .Include(x => x.Attachments)
            .Include(x => x.History)
            .Include(x => x.Actions.Where(action => !action.IsDeleted))
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id);

        query = TaskFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId);

        var task = await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Task not found: {request.Id}");

        var dto = task.Adapt<TaskItemDto>();
        dto.CanAddAction = TaskFeatureHelpers.CanMutateTask(task, httpContextAccessor, currentUserId);
        dto.Actions = task.Actions
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.ExpectedCompletionAt ?? DateTime.MaxValue)
            .ThenBy(x => x.CreatedAt)
            .Select(x => TaskFeatureHelpers.MapAction(x, httpContextAccessor, currentUserId))
            .ToList();

        return new GetTaskByIdResult(dto);
    }
}
