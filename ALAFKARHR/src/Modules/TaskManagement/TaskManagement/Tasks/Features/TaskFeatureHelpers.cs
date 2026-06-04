using Auth.Contracts.Features.GetByUserName;
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

    public static string GetCurrentUserName(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value
            ?? throw new UnauthorizedAccessException("User is not authorized.");

        return value;
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

        return query.Where(x => x.AssignedToUser == currentUserId.ToString() || x.CreatedBy == currentUserId.ToString() || x.AssignedByUserId== currentUserId);
    }

    public static IQueryable<TaskItem> ApplyFilters(IQueryable<TaskItem> query, TaskFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.TaskNumber))
            query = query.Where(x => x.TaskNumber.Contains(filter.TaskNumber));
        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(x => x.Title.Contains(filter.Title));
        if (!string.IsNullOrWhiteSpace(filter.AssignedToUser))
            query = query.Where(x => x.AssignedToUser == filter.AssignedToUser);
        if (filter.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == filter.DepartmentId.Value);
        if (filter.Priority.HasValue)
            query = query.Where(x => x.Priority == filter.Priority.Value);
        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);
        if (filter.FromDate.HasValue)
            query = query.Where(x => x.CreatedAt.Value.Date >= filter.FromDate.Value.Date);
        if (filter.ToDate.HasValue)
            query = query.Where(x => x.CreatedAt.Value.Date <= filter.ToDate.Value.Date);

        return query;
    }

    public static void AddHistoryAndNotification(TaskManagementDbContext dbContext, TaskItem task, Guid userId, string action, string? oldValue, string? newValue, string notifyUser)
    {
        task.AddHistory(TaskHistory.Create(task.Id, userId, action, oldValue, newValue));
        dbContext.TaskNotifications.Add(TaskNotification.Create(task.Id, notifyUser, action, $"{action}: {task.TaskNumber} - {task.Title}", userId.ToString()));
    }

    public static async Task EnsureAssignedUserExistsAsync(ISender sender, string userCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userCode ))
            throw new BadRequestException("Assigned user is required.");

        await sender.Send(new GetByUserNameQuery(userCode), cancellationToken);
    }

    public static void EnsureDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new BadRequestException("Department is required.");
    }
}
