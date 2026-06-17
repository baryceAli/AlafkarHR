using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Features.DailyCheck;

public record RunMyTaskDailyCheckCommand : ICommand<RunMyTaskDailyCheckResult>;
public record RunMyTaskDailyCheckResult(bool IsSuccess, bool WasSkipped, int GeneratedCount, int ReminderCount, int OverdueCount);

public class RunMyTaskDailyCheckHandler(
    TaskManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ITaskNumberGenerator taskNumberGenerator)
    : ICommandHandler<RunMyTaskDailyCheckCommand, RunMyTaskDailyCheckResult>
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(30);

    public async Task<RunMyTaskDailyCheckResult> Handle(RunMyTaskDailyCheckCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var userName = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var now = DateTime.UtcNow;
        var today = now.Date;

        var run = await dbContext.TaskDailyCheckRuns
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CheckDate == today, cancellationToken);

        if (run is null)
        {
            run = TaskDailyCheckRun.Create(userId, userName, today);
            await dbContext.TaskDailyCheckRuns.AddAsync(run, cancellationToken);
        }

        if (run.ShouldSkip(now))
            return new RunMyTaskDailyCheckResult(true, true, 0, 0, 0);

        run.MarkAttempt();
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var generatedCount = await GenerateRecurringTasksAsync(userId, userName, today, cancellationToken);
            var reminderCount = await ProcessRemindersAsync(userId, userName, now, cancellationToken);
            var overdueCount = await MarkOverdueTasksAsync(userId, userName, today, cancellationToken);

            run.MarkCompleted();
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new RunMyTaskDailyCheckResult(true, false, generatedCount, reminderCount, overdueCount);
        }
        catch (Exception ex)
        {
            dbContext.ChangeTracker.Clear();
            await MarkRunFailedAsync(userId, today, ex, cancellationToken);
            return new RunMyTaskDailyCheckResult(true, true, 0, 0, 0);
        }
    }

    private async Task<int> GenerateRecurringTasksAsync(Guid userId, string userName, DateTime today, CancellationToken cancellationToken)
    {
        var templates = await dbContext.TaskItems
            .Include(x => x.History)
            .Include(x => x.Actions)
            .Where(x => !x.IsDeleted
                && x.IsRecurring
                && x.NextOccurrenceDate.HasValue
                && x.NextOccurrenceDate.Value.Date <= today)
            .ToListAsync(cancellationToken);

        var generatedCount = 0;

        foreach (var template in templates.Where(x => x.CanGenerateOccurrence() && TaskFeatureHelpers.IsTaskAssignedToUser(x, userId, userName)))
        {
            var nextDueDate = template.NextOccurrenceDate!.Value.Date;
            var nextStartDate = CalculateOccurrenceStartDate(template, nextDueDate);
            var nextReminderDate = CalculateOccurrenceReminderDate(template, nextDueDate);
            var taskNumber = await taskNumberGenerator.GenerateAsync(cancellationToken);

            var occurrence = TaskItem.Create(
                taskNumber,
                template.Title,
                template.Description,
                template.Priority,
                nextStartDate,
                nextDueDate,
                Guid.Empty,
                template.AssignedToUser,
                template.AssignedByUserId,
                template.DepartmentId,
                false,
                nextReminderDate,
                parentTaskId: template.Id);

            foreach (var templateAction in template.Actions.Where(x => !x.IsDeleted))
            {
                occurrence.AddAction(templateAction.CopyAsOpen(
                    occurrence.Id,
                    TaskFeatureHelpers.ShiftActionExpectedDate(template, templateAction, nextDueDate),
                    Guid.Empty,
                    TaskFeatureHelpers.SystemUserName));
            }

            await dbContext.TaskItems.AddAsync(occurrence, cancellationToken);

            var followingDate = TaskItem.CalculateNextOccurrence(nextDueDate, template.RecurrenceFrequency, template.RecurrenceInterval);
            if (ShouldStopAfterGeneration(template, followingDate))
                followingDate = null;

            template.RegisterGeneratedOccurrence(followingDate, userId);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, template, Guid.Empty, "RecurringTaskGenerated", null, occurrence.TaskNumber, template.AssignedToUser);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, occurrence, Guid.Empty, "TaskCreatedFromRecurrence", null, template.TaskNumber, occurrence.AssignedToUser);
            generatedCount++;
        }

        return generatedCount;
    }

    private async Task<int> ProcessRemindersAsync(Guid userId, string userName, DateTime now, CancellationToken cancellationToken)
    {
        var tasks = await dbContext.TaskItems.Include(x => x.History)
            .Where(x => !x.IsDeleted
                && !x.IsArchived
                && x.ReminderDate.HasValue
                && x.ReminderDate.Value <= now
                && x.ReminderNotificationSentAt == null
                && x.Status != TaskWorkflowStatus.Completed
                && x.Status != TaskWorkflowStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var count = 0;

        foreach (var task in tasks.Where(x => TaskFeatureHelpers.IsTaskAssignedToUser(x, userId, userName)))
        {
            task.MarkReminderSent(userId);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskReminder", null, task.ReminderDate?.ToString("O"), task.AssignedToUser);
            count++;
        }

        return count;
    }

    private async Task<int> MarkOverdueTasksAsync(Guid userId, string userName, DateTime today, CancellationToken cancellationToken)
    {
        var tasks = await dbContext.TaskItems.Include(x => x.History)
            .Where(x => !x.IsDeleted
                && x.DueDate.Date < today
                && x.Status != TaskWorkflowStatus.Completed
                && x.Status != TaskWorkflowStatus.Cancelled
                && x.Status != TaskWorkflowStatus.Overdue)
            .ToListAsync(cancellationToken);

        var count = 0;

        foreach (var task in tasks.Where(x => TaskFeatureHelpers.IsTaskAssignedToUser(x, userId, userName)))
        {
            var oldStatus = task.Status.ToString();
            task.MarkOverdue(userId);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskOverdue", oldStatus, TaskWorkflowStatus.Overdue.ToString(), task.AssignedToUser);
            count++;
        }

        return count;
    }

    private async Task MarkRunFailedAsync(Guid userId, DateTime today, Exception ex, CancellationToken cancellationToken)
    {
        var run = await dbContext.TaskDailyCheckRuns
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CheckDate == today, cancellationToken);

        if (run is null)
            return;

        run.MarkFailed(ex.Message, DateTime.UtcNow.Add(RetryDelay));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static DateTime? CalculateOccurrenceStartDate(TaskItem template, DateTime nextDueDate)
    {
        if (!template.StartDate.HasValue)
            return null;

        var duration = template.DueDate.Date - template.StartDate.Value.Date;
        return nextDueDate.Subtract(duration);
    }

    private static DateTime? CalculateOccurrenceReminderDate(TaskItem template, DateTime nextDueDate)
    {
        if (!template.ReminderDate.HasValue)
            return null;

        var offset = template.DueDate.Date - template.ReminderDate.Value.Date;
        return nextDueDate.Subtract(offset);
    }

    private static bool ShouldStopAfterGeneration(TaskItem template, DateTime? followingDate)
    {
        if (!followingDate.HasValue)
            return true;

        if (template.RecurrenceEndType == TaskRecurrenceEndType.OnDate
            && template.RecurrenceEndDate.HasValue
            && followingDate.Value.Date > template.RecurrenceEndDate.Value.Date)
            return true;

        return template.RecurrenceEndType == TaskRecurrenceEndType.AfterOccurrences
            && template.RecurrenceMaxOccurrences.HasValue
            && template.RecurrenceOccurrencesCreated + 1 >= template.RecurrenceMaxOccurrences.Value;
    }
}
