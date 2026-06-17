using Auth.Contracts.Features.GetByUserName;
using Auth.Contracts.Features.GetUserById;

namespace TaskManagement.Tasks.Features;

internal static class TaskFeatureHelpers
{
    public const string SystemUserName = "System";

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

    public static bool CanMutateTask(TaskItem task, IHttpContextAccessor httpContextAccessor, Guid currentUserId)
    {
        var currentUserName = GetCurrentUserName(httpContextAccessor);

        return HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ManageAllTasks)
            || string.Equals(task.AssignedToUser, currentUserId.ToString(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(task.AssignedToUser, currentUserName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(task.CreatedBy, currentUserId.ToString(), StringComparison.OrdinalIgnoreCase)
            || task.AssignedByUserId == currentUserId;
    }

    public static void EnsureCanMutateTask(TaskItem task, IHttpContextAccessor httpContextAccessor, Guid currentUserId)
    {
        if (!CanMutateTask(task, httpContextAccessor, currentUserId))
            throw new UnauthorizedAccessException("You are not allowed to update this task.");
    }

    public static IQueryable<TaskItem> ApplyVisibility(IQueryable<TaskItem> query, IHttpContextAccessor httpContextAccessor, Guid currentUserId, Guid? departmentId = null)
    {
        var currentUserName = GetCurrentUserName(httpContextAccessor);

        if (HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ManageAllTasks))
            return query;

        if (HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ViewReports) && departmentId.HasValue)
            return query.Where(x => x.DepartmentId == departmentId.Value);

        return query.Where(x =>
            x.AssignedToUser == currentUserId.ToString() ||
            x.AssignedToUser == currentUserName ||
            x.CreatedBy == currentUserId.ToString() ||
            x.AssignedByUserId == currentUserId);
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
            query = query.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Date >= filter.FromDate.Value.Date);
        if (filter.ToDate.HasValue)
            query = query.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Date <= filter.ToDate.Value.Date);

        return query;
    }

    public static void AddHistoryAndNotification(TaskManagementDbContext dbContext, TaskItem task, Guid userId, string action, string? oldValue, string? newValue, string notifyUser)
    {
        task.AddHistory(TaskHistory.Create(task.Id, userId, action, oldValue, newValue));
        AddNotification(dbContext, task, notifyUser, action, $"{action}: {task.TaskNumber} - {task.Title}", userId.ToString());
    }

    public static void AddNotification(TaskManagementDbContext dbContext, TaskItem task, string notifyUser, string notificationType, string message, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(notifyUser))
            return;

        dbContext.TaskNotifications.Add(TaskNotification.Create(task.Id, notifyUser, notificationType, message, createdBy));
    }

    public static bool CanMutateAction(TaskActionItem action, IHttpContextAccessor httpContextAccessor, Guid currentUserId)
    {
        return HasPermission(httpContextAccessor, PermissionList.TaskManagementPermissions.ManageAllTasks)
            || action.CreatedByUserId == currentUserId;
    }

    public static void EnsureCanMutateAction(TaskActionItem action, IHttpContextAccessor httpContextAccessor, Guid currentUserId)
    {
        if (!CanMutateAction(action, httpContextAccessor, currentUserId))
            throw new UnauthorizedAccessException("You are not allowed to update this task action.");
    }

    public static TaskActionDto MapAction(TaskActionItem action, IHttpContextAccessor httpContextAccessor, Guid currentUserId)
    {
        return new TaskActionDto
        {
            Id = action.Id,
            TaskId = action.TaskId,
            Title = action.Title,
            ExpectedCompletionAt = action.ExpectedCompletionAt,
            IsCompleted = action.IsCompleted,
            CompletedAt = action.CompletedAt,
            CreatedByUserId = action.CreatedByUserId,
            CreatedByUserName = action.CreatedByUserName,
            CreatedDate = action.CreatedAt ?? DateTime.UtcNow,
            Status = GetActionStatus(action),
            CanEdit = CanMutateAction(action, httpContextAccessor, currentUserId)
        };
    }

    public static TaskActionStatus GetActionStatus(TaskActionItem action)
    {
        if (action.IsCompleted)
            return TaskActionStatus.Completed;

        return action.ExpectedCompletionAt.HasValue && action.ExpectedCompletionAt.Value < DateTime.UtcNow
            ? TaskActionStatus.Overdue
            : TaskActionStatus.Open;
    }

    public static DateTime? ShiftActionExpectedDate(TaskItem template, TaskActionItem action, DateTime nextDueDate)
    {
        if (!action.ExpectedCompletionAt.HasValue)
            return null;

        var offset = template.DueDate - action.ExpectedCompletionAt.Value;
        return nextDueDate.Subtract(offset);
    }

    public static bool IsTaskAssignedToUser(TaskItem task, Guid userId, string userName)
    {
        return string.Equals(task.AssignedToUser, userId.ToString(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(task.AssignedToUser, userName, StringComparison.OrdinalIgnoreCase);
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
