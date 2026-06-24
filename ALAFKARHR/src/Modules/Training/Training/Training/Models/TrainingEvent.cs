using Shared.DDD;

namespace Training.Training.Models;

public class TrainingEvent : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid ProgramId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public int Capacity { get; private set; }
    public TrainingEventStatus Status { get; private set; } = TrainingEventStatus.Planned;

    private TrainingEvent() { }

    public static TrainingEvent Create(Guid id, Guid companyId, Guid programId, string title, DateTime startAt, DateTime endAt, int capacity, string userId)
    {
        var item = new TrainingEvent { Id = id, CompanyId = companyId, ProgramId = programId, Status = TrainingEventStatus.Planned };
        item.Update(programId, title, startAt, endAt, capacity, userId);
        item.CreatedAt = DateTime.UtcNow;
        item.CreatedBy = userId;
        return item;
    }

    public void Update(Guid programId, string title, DateTime startAt, DateTime endAt, int capacity, string userId)
    {
        if (programId == Guid.Empty) throw new InvalidOperationException("Training program is required.");
        if (endAt < startAt) throw new InvalidOperationException("Training event end date must be after start date.");

        ProgramId = programId;
        Title = string.IsNullOrWhiteSpace(title) ? throw new InvalidOperationException("Training event title is required.") : title.Trim();
        StartAt = startAt;
        EndAt = endAt;
        Capacity = Math.Max(0, capacity);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Open(string userId) => SetStatus(TrainingEventStatus.Open, userId);
    public void Start(string userId) => SetStatus(TrainingEventStatus.InProgress, userId);
    public void Complete(string userId) => SetStatus(TrainingEventStatus.Completed, userId);
    public void Cancel(string userId) => SetStatus(TrainingEventStatus.Cancelled, userId);

    private void SetStatus(TrainingEventStatus status, string userId)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
