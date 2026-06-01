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
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id);

        query = TaskFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId);

        var task = await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Task not found: {request.Id}");

        return new GetTaskByIdResult(task.Adapt<TaskItemDto>());
    }
}
