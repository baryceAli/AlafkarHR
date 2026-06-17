namespace TaskManagement.Tasks.Models;

public class TaskItem : Aggregate<Guid>
{
    public string TaskNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TaskPriority Priority { get; private set; }
    public TaskWorkflowStatus Status { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public string AssignedToUser { get; private set; } = string.Empty;
    public Guid AssignedByUserId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public bool IsRecurring { get; private set; }
    public TaskRecurrenceFrequency RecurrenceFrequency { get; private set; }
    public int RecurrenceInterval { get; private set; } = 1;
    public TaskRecurrenceEndType RecurrenceEndType { get; private set; }
    public DateTime? RecurrenceEndDate { get; private set; }
    public int? RecurrenceMaxOccurrences { get; private set; }
    public int RecurrenceOccurrencesCreated { get; private set; }
    public DateTime? NextOccurrenceDate { get; private set; }
    public Guid? ParentTaskId { get; private set; }
    public DateTime? ReminderDate { get; private set; }
    public DateTime? ReminderNotificationSentAt { get; private set; }
    public bool IsArchived { get; private set; }

    public List<TaskComment> Comments { get; private set; } = [];
    public List<TaskAttachment> Attachments { get; private set; } = [];
    public List<TaskHistory> History { get; private set; } = [];
    public List<TaskActionItem> Actions { get; private set; } = [];

    private TaskItem()
    {
    }

    public static TaskItem Create(string taskNumber, string title, string description, TaskPriority priority, DateTime? startDate,
        DateTime dueDate, Guid createdByUserId, string assignedToUser, Guid assignedByUserId, Guid departmentId,
        bool isRecurring, DateTime? reminderDate, TaskRecurrenceFrequency recurrenceFrequency = TaskRecurrenceFrequency.None,
        int recurrenceInterval = 1, TaskRecurrenceEndType recurrenceEndType = TaskRecurrenceEndType.Never,
        DateTime? recurrenceEndDate = null, int? recurrenceMaxOccurrences = null, Guid? parentTaskId = null)
    {
        ValidateDates(startDate, dueDate);
        ValidateRecurrence(isRecurring, recurrenceFrequency, recurrenceInterval, recurrenceEndType, recurrenceEndDate, recurrenceMaxOccurrences);

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            TaskNumber = taskNumber,
            Title = title,
            Description = description,
            Priority = priority,
            Status = string.IsNullOrEmpty(assignedToUser) ? TaskWorkflowStatus.Draft : TaskWorkflowStatus.Assigned,
            StartDate = startDate,
            DueDate = dueDate,
            AssignedToUser = assignedToUser,
            AssignedByUserId = assignedByUserId,
            DepartmentId = departmentId,
            IsRecurring = isRecurring,
            RecurrenceFrequency = isRecurring ? recurrenceFrequency : TaskRecurrenceFrequency.None,
            RecurrenceInterval = isRecurring ? recurrenceInterval : 1,
            RecurrenceEndType = isRecurring ? recurrenceEndType : TaskRecurrenceEndType.Never,
            RecurrenceEndDate = isRecurring ? recurrenceEndDate : null,
            RecurrenceMaxOccurrences = isRecurring ? recurrenceMaxOccurrences : null,
            NextOccurrenceDate = isRecurring ? CalculateNextOccurrence(dueDate, recurrenceFrequency, recurrenceInterval) : null,
            ParentTaskId = parentTaskId,
            ReminderDate = reminderDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public void Update(string title, string description, TaskPriority priority, DateTime? startDate, DateTime dueDate,
        Guid departmentId, bool isRecurring, DateTime? reminderDate, Guid modifiedByUserId,
        TaskRecurrenceFrequency recurrenceFrequency = TaskRecurrenceFrequency.None, int recurrenceInterval = 1,
        TaskRecurrenceEndType recurrenceEndType = TaskRecurrenceEndType.Never, DateTime? recurrenceEndDate = null,
        int? recurrenceMaxOccurrences = null)
    {
        ValidateDates(startDate, dueDate);
        ValidateRecurrence(isRecurring, recurrenceFrequency, recurrenceInterval, recurrenceEndType, recurrenceEndDate, recurrenceMaxOccurrences);

        var oldReminderDate = ReminderDate;

        Title = title;
        Description = description;
        Priority = priority;
        StartDate = startDate;
        DueDate = dueDate;
        DepartmentId = departmentId;
        IsRecurring = isRecurring;
        RecurrenceFrequency = isRecurring ? recurrenceFrequency : TaskRecurrenceFrequency.None;
        RecurrenceInterval = isRecurring ? recurrenceInterval : 1;
        RecurrenceEndType = isRecurring ? recurrenceEndType : TaskRecurrenceEndType.Never;
        RecurrenceEndDate = isRecurring ? recurrenceEndDate : null;
        RecurrenceMaxOccurrences = isRecurring ? recurrenceMaxOccurrences : null;
        NextOccurrenceDate = isRecurring ? CalculateNextOccurrence(dueDate, recurrenceFrequency, recurrenceInterval) : null;
        ReminderDate = reminderDate;
        if (reminderDate != oldReminderDate)
            ReminderNotificationSentAt = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Assign(string assignedToUser, Guid assignedByUserId, Guid departmentId, DateTime? startDate, DateTime dueDate)
    {
        ValidateDates(startDate, dueDate);

        AssignedToUser = assignedToUser;
        AssignedByUserId = assignedByUserId;
        DepartmentId = departmentId;
        StartDate = startDate;
        DueDate = dueDate;
        Status = TaskWorkflowStatus.Assigned;
        CompletedDate = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = assignedByUserId.ToString();
    }

    public void ChangeStatus(TaskWorkflowStatus status, Guid modifiedByUserId)
    {
        Status = status;
        CompletedDate = status == TaskWorkflowStatus.Completed ? DateTime.UtcNow : null;
        ProgressPercentage = status == TaskWorkflowStatus.Completed ? 100 : ProgressPercentage;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void UpdateProgress(decimal progressPercentage, Guid modifiedByUserId)
    {
        ProgressPercentage = Math.Clamp(progressPercentage, 0, 100);
        if (ProgressPercentage > 0 && Status == TaskWorkflowStatus.Assigned)
            Status = TaskWorkflowStatus.InProgress;
        if (ProgressPercentage == 100)
            ChangeStatus(TaskWorkflowStatus.Completed, modifiedByUserId);

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void MarkOverdue(Guid userId)
    {
        if (Status is TaskWorkflowStatus.Completed or TaskWorkflowStatus.Cancelled)
            return;

        Status = TaskWorkflowStatus.Overdue;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId.ToString();
    }

    public void Archive(Guid userId)
    {
        IsArchived = true;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId.ToString();
    }

    public void AddComment(TaskComment comment) => Comments.Add(comment);
    public void AddAttachment(TaskAttachment attachment) => Attachments.Add(attachment);
    public void AddHistory(TaskHistory history) => History.Add(history);
    public void AddAction(TaskActionItem action) => Actions.Add(action);

    public void RecalculateProgressFromActions(Guid modifiedByUserId)
    {
        var activeActions = Actions.Where(x => !x.IsDeleted).ToList();
        if (activeActions.Count == 0)
        {
            if (Status != TaskWorkflowStatus.Completed)
                ProgressPercentage = 0;

            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedByUserId.ToString();
            return;
        }

        var completedCount = activeActions.Count(x => x.IsCompleted);
        ProgressPercentage = Math.Round(completedCount * 100m / activeActions.Count, 2);

        if (completedCount == activeActions.Count)
        {
            ChangeStatus(TaskWorkflowStatus.Completed, modifiedByUserId);
            return;
        }

        if (Status == TaskWorkflowStatus.Completed)
        {
            Status = TaskWorkflowStatus.InProgress;
            CompletedDate = null;
        }
        else if (ProgressPercentage > 0 && Status == TaskWorkflowStatus.Assigned)
        {
            Status = TaskWorkflowStatus.InProgress;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void MarkReminderSent(Guid userId)
    {
        ReminderNotificationSentAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId.ToString();
    }

    public void RegisterGeneratedOccurrence(DateTime? nextOccurrenceDate, Guid userId)
    {
        RecurrenceOccurrencesCreated++;
        NextOccurrenceDate = nextOccurrenceDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId.ToString();
    }

    private static void ValidateDates(DateTime? startDate, DateTime dueDate)
    {
        if (startDate.HasValue && dueDate.Date < startDate.Value.Date)
            throw new BadRequestException("Due date cannot be before start date.");
    }

    public static DateTime? CalculateNextOccurrence(DateTime fromDate, TaskRecurrenceFrequency frequency, int interval)
    {
        return frequency switch
        {
            TaskRecurrenceFrequency.Daily => fromDate.Date.AddDays(interval),
            TaskRecurrenceFrequency.Weekly => fromDate.Date.AddDays(interval * 7),
            TaskRecurrenceFrequency.Monthly => fromDate.Date.AddMonths(interval),
            _ => null
        };
    }

    public bool CanGenerateOccurrence()
    {
        if (!IsRecurring || RecurrenceFrequency == TaskRecurrenceFrequency.None || !NextOccurrenceDate.HasValue)
            return false;

        if (RecurrenceEndType == TaskRecurrenceEndType.OnDate && RecurrenceEndDate.HasValue && NextOccurrenceDate.Value.Date > RecurrenceEndDate.Value.Date)
            return false;

        if (RecurrenceEndType == TaskRecurrenceEndType.AfterOccurrences && RecurrenceMaxOccurrences.HasValue && RecurrenceOccurrencesCreated >= RecurrenceMaxOccurrences.Value)
            return false;

        return true;
    }

    private static void ValidateRecurrence(bool isRecurring, TaskRecurrenceFrequency frequency, int interval,
        TaskRecurrenceEndType endType, DateTime? endDate, int? maxOccurrences)
    {
        if (!isRecurring)
            return;

        if (frequency == TaskRecurrenceFrequency.None)
            throw new BadRequestException("Recurring tasks require a recurrence frequency.");
        if (interval < 1)
            throw new BadRequestException("Recurrence interval must be at least 1.");
        if (endType == TaskRecurrenceEndType.OnDate && !endDate.HasValue)
            throw new BadRequestException("Recurrence end date is required.");
        if (endType == TaskRecurrenceEndType.AfterOccurrences && (!maxOccurrences.HasValue || maxOccurrences.Value < 1))
            throw new BadRequestException("Recurrence occurrence count must be at least 1.");
    }
}
