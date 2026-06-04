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
    public DateTime? ReminderDate { get; private set; }
    public bool IsArchived { get; private set; }

    public List<TaskComment> Comments { get; private set; } = [];
    public List<TaskAttachment> Attachments { get; private set; } = [];
    public List<TaskHistory> History { get; private set; } = [];

    private TaskItem()
    {
    }

    public static TaskItem Create(string taskNumber, string title, string description, TaskPriority priority, DateTime? startDate,
        DateTime dueDate, Guid createdByUserId, string assignedToUser, Guid assignedByUserId, Guid departmentId,
        bool isRecurring, DateTime? reminderDate)
    {
        ValidateDates(startDate, dueDate);

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
            ReminderDate = reminderDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public void Update(string title, string description, TaskPriority priority, DateTime? startDate, DateTime dueDate,
        Guid departmentId, bool isRecurring, DateTime? reminderDate, Guid modifiedByUserId)
    {
        ValidateDates(startDate, dueDate);

        Title = title;
        Description = description;
        Priority = priority;
        StartDate = startDate;
        DueDate = dueDate;
        DepartmentId = departmentId;
        IsRecurring = isRecurring;
        ReminderDate = reminderDate;
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

    private static void ValidateDates(DateTime? startDate, DateTime dueDate)
    {
        if (startDate.HasValue && dueDate.Date < startDate.Value.Date)
            throw new BadRequestException("Due date cannot be before start date.");
    }
}
