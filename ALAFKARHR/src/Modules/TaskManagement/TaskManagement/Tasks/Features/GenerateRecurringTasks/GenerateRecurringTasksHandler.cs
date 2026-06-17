using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Features.GenerateRecurringTasks;

public record GenerateRecurringTasksCommand : ICommand<GenerateRecurringTasksResult>;
public record GenerateRecurringTasksResult(int GeneratedCount);

public class GenerateRecurringTasksHandler(
    TaskManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ITaskNumberGenerator taskNumberGenerator)
    : ICommandHandler<GenerateRecurringTasksCommand, GenerateRecurringTasksResult>
{
    public async Task<GenerateRecurringTasksResult> Handle(GenerateRecurringTasksCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var today = DateTime.UtcNow.Date;
        var templates = await dbContext.TaskItems
            .Include(x => x.History)
            .Include(x => x.Actions)
            .Where(x => !x.IsDeleted
                && x.IsRecurring
                && x.NextOccurrenceDate.HasValue
                && x.NextOccurrenceDate.Value.Date <= today)
            .ToListAsync(cancellationToken);

        var generatedCount = 0;

        foreach (var template in templates.Where(x => x.CanGenerateOccurrence()))
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
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, template, userId, "RecurringTaskGenerated", null, occurrence.TaskNumber, template.AssignedToUser);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, occurrence, userId, "TaskCreatedFromRecurrence", null, template.TaskNumber, occurrence.AssignedToUser);
            generatedCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new GenerateRecurringTasksResult(generatedCount);
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
