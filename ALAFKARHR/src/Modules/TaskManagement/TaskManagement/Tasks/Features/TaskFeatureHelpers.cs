using Auth.Contracts.Features.GetUserById;

namespace TaskManagement.Tasks.Features;

internal static class TaskFeatureHelpers
{
    public static Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("User is not authorized.");

        return Guid.Parse(value);
    }

    public static bool HasPermission(IHttpContextAccessor httpContextAccessor, string permission)
    {
        return httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == permission) == true;
    }

    public static IQueryable<TaskItem> ApplyVisibility(IQueryable<TaskItem> query, IHttpContextAccessor httpContextAccessor, Guid currentUserId, Guid? departmentId = null)
    {
        if (HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ManageAllTasks))
            return query;

        if (HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ViewReports) && departmentId.HasValue)
            return query.Where(x => x.DepartmentId == departmentId.Value);

        return query.Where(x => x.AssignedToUserId == currentUserId || x.CreatedByUserId == currentUserId || x.AssignedByUserId == currentUserId);
    }

    public static IQueryable<TaskItem> ApplyFilters(IQueryable<TaskItem> query, TaskFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.TaskNumber))
            query = query.Where(x => x.TaskNumber.Contains(filter.TaskNumber));
        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(x => x.Title.Contains(filter.Title));
        if (filter.AssignedToUserId.HasValue)
            query = query.Where(x => x.AssignedToUserId == filter.AssignedToUserId.Value);
        if (filter.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == filter.DepartmentId.Value);
        if (filter.Priority.HasValue)
            query = query.Where(x => x.Priority == filter.Priority.Value);
        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);
        if (filter.FromDate.HasValue)
            query = query.Where(x => x.CreatedDate.Date >= filter.FromDate.Value.Date);
        if (filter.ToDate.HasValue)
            query = query.Where(x => x.CreatedDate.Date <= filter.ToDate.Value.Date);

        return query;
    }

    public static void AddHistoryAndNotification(TaskManagementDbContext dbContext, TaskItem task, Guid userId, string action, string? oldValue, string? newValue, Guid notifyUserId)
    {
        task.AddHistory(TaskHistory.Create(task.Id, userId, action, oldValue, newValue));
        dbContext.TaskNotifications.Add(TaskNotification.Create(task.Id, notifyUserId, action, $"{action}: {task.TaskNumber} - {task.Title}"));
    }

    public static async Task EnsureAssignedUserExistsAsync(ISender sender, Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            throw new BadRequestException("Assigned user is required.");

        await sender.Send(new GetUserByIdQuery(userId), cancellationToken);
    }

    public static void EnsureDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new BadRequestException("Department is required.");
    }
}
