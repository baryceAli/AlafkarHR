using SharedWithUI.TaskManagement.Enums;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace SharedWithUI.TaskManagement.Dtos;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public TaskWorkflowStatus Status { get; set; } = TaskWorkflowStatus.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? StartDate { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? CompletedDate { get; set; }
    public decimal ProgressPercentage { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string AssignedToUser { get; set; } = string.Empty;
    public Guid AssignedByUserId { get; set; }
    public Guid DepartmentId { get; set; }
    public bool IsRecurring { get; set; }
    public TaskRecurrenceFrequency RecurrenceFrequency { get; set; } = TaskRecurrenceFrequency.None;
    public int RecurrenceInterval { get; set; } = 1;
    public TaskRecurrenceEndType RecurrenceEndType { get; set; } = TaskRecurrenceEndType.Never;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? RecurrenceMaxOccurrences { get; set; }
    public int RecurrenceOccurrencesCreated { get; set; }
    public DateTime? NextOccurrenceDate { get; set; }
    public Guid? ParentTaskId { get; set; }
    public DateTime? ReminderDate { get; set; }
    public bool IsArchived { get; set; }
    public bool CanAddAction { get; set; }
    public List<TaskCommentDto> Comments { get; set; } = [];
    public List<TaskAttachmentDto> Attachments { get; set; } = [];
    public List<TaskHistoryDto> History { get; set; } = [];
    public List<TaskActionDto> Actions { get; set; } = [];
}

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public DateTime? StartDate { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date;
    public string AssignedToUser { get; set; }
    public Guid DepartmentId { get; set; }
    public bool IsRecurring { get; set; }
    public TaskRecurrenceFrequency RecurrenceFrequency { get; set; } = TaskRecurrenceFrequency.None;
    public int RecurrenceInterval { get; set; } = 1;
    public TaskRecurrenceEndType RecurrenceEndType { get; set; } = TaskRecurrenceEndType.Never;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? RecurrenceMaxOccurrences { get; set; }
    public DateTime? ReminderDate { get; set; }
    public List<CreateTaskActionDto> Actions { get; set; } = [];
}

public class UpdateTaskItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public DateTime? StartDate { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date;
    public Guid DepartmentId { get; set; }
    public bool IsRecurring { get; set; }
    public TaskRecurrenceFrequency RecurrenceFrequency { get; set; } = TaskRecurrenceFrequency.None;
    public int RecurrenceInterval { get; set; } = 1;
    public TaskRecurrenceEndType RecurrenceEndType { get; set; } = TaskRecurrenceEndType.Never;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? RecurrenceMaxOccurrences { get; set; }
    public DateTime? ReminderDate { get; set; }
}

public class AssignTaskDto
{
    public string AssignedToUser { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date;
}

public class ChangeTaskStatusDto
{
    public Guid Id { get; set; }
    public TaskWorkflowStatus Status { get; set; }
}

public class UpdateTaskProgressDto
{
    public Guid Id { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class TaskActionDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ExpectedCompletionAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public TaskActionStatus Status { get; set; } = TaskActionStatus.Open;
    public bool CanEdit { get; set; }
}

public class CreateTaskActionDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ExpectedCompletionAt { get; set; }
}

public class UpdateTaskActionDto
{
    public Guid TaskId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ExpectedCompletionAt { get; set; }
}

public class ToggleTaskActionCompletionDto
{
    public Guid TaskId { get; set; }
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; }
}

public class TaskCommentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class CreateTaskCommentDto
{
    public Guid TaskId { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class TaskAttachmentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedDate { get; set; }
    public Guid UploadedByUserId { get; set; }
}

public class TaskHistoryDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ActionDate { get; set; }
}

public class TaskNotificationDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class TaskNotificationUnreadCountDto
{
    public int Count { get; set; }
}

public class GenerateRecurringTasksResultDto
{
    public int GeneratedCount { get; set; }
}

public class ProcessTaskRemindersResultDto
{
    public int NotificationCount { get; set; }
}

public class MarkOverdueTasksResultDto
{
    public int UpdatedCount { get; set; }
}

public class RunMyTaskDailyCheckResultDto
{
    public bool IsSuccess { get; set; }
    public bool WasSkipped { get; set; }
    public int GeneratedCount { get; set; }
    public int ReminderCount { get; set; }
    public int OverdueCount { get; set; }
}

public class TaskFilterDto
{
    public string? TaskNumber { get; set; }
    public string? Title { get; set; }
    public string? AssignedToUser { get; set; }
    public Guid? DepartmentId { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskWorkflowStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class TaskDashboardDto
{
    public int TotalTasks { get; set; }
    public int OpenTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TasksDueToday { get; set; }
    public int TasksDueThisWeek { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageCompletionHours { get; set; }
    public List<EmployeeWorkloadDto> EmployeeWorkload { get; set; } = [];
    public List<DepartmentPerformanceDto> DepartmentPerformanceRanking { get; set; } = [];
}

public class EmployeeWorkloadDto
{
    public string UserCode { get; set; } = string.Empty;
    public int OpenTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
}

public class TaskSummaryReportDto
{
    public int TotalTasks { get; set; }
    public int OpenTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskStatusCountDto> ByStatus { get; set; } = [];
    public List<TaskPriorityCountDto> ByPriority { get; set; } = [];
}

public class TaskStatusCountDto
{
    public TaskWorkflowStatus Status { get; set; }
    public int Count { get; set; }
}

public class TaskPriorityCountDto
{
    public TaskPriority Priority { get; set; }
    public int Count { get; set; }
}

public class EmployeeProductivityReportDto
{
    public string UserCode { get; set; } = string.Empty;
    public int AssignedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageCompletionHours { get; set; }
}

public class DepartmentPerformanceDto
{
    public Guid DepartmentId { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal CompletionPercentage { get; set; }
}
