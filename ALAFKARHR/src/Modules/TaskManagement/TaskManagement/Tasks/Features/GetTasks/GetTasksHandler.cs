using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.GetTasks;

public record GetTasksQuery(PaginationRequest PaginationRequest, TaskFilterDto Filter) : IQuery<GetTasksResult>;
public record GetTasksResult(PaginatedResult<TaskItemDto> TaskList);

public class GetTasksHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTasksQuery, GetTasksResult>
{
    public async Task<GetTasksResult> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.TaskItems
            .Include(x => x.Comments)
            .Include(x => x.Attachments)
            .AsNoTracking()
            .Where(x => !x.IsDeleted && !x.IsArchived)
            .AsQueryable();

        query = TaskFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId, request.Filter.DepartmentId);
        query = TaskFeatureHelpers.ApplyFilters(query, request.Filter);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x => x.TaskNumber.ToLower().Contains(search) || x.Title.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var tasks = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetTasksResult(new PaginatedResult<TaskItemDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            tasks.Adapt<List<TaskItemDto>>()));
    }
}
