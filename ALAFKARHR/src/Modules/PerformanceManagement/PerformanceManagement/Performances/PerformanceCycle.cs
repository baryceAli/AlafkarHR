namespace PerformanceManagement.Performances;


using Shared.DDD;


public class PerformanceCycle : Aggregate<Guid>
{
    public string Name { get; private set; } // e.g. "2026 Q1"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public bool IsActive { get; private set; }
    public bool IsClosed { get; private set; }
    public bool IsCancelled { get; private set; }

    public Guid CompanyId { get; private set; }

    private PerformanceCycle() { }

    public static PerformanceCycle Create(
        Guid id,
        string name,
        DateTime start,
        DateTime end,
        Guid companyId)
    {
        return new PerformanceCycle
        {
            Id = id,
            Name = name,
            StartDate = start,
            EndDate = end,
            IsActive = false,
            CompanyId = companyId
        };
    }

    public void Update(string name, DateTime start, DateTime end, string modifiedBy)
    {
        if (IsClosed || IsCancelled)
            throw new InvalidOperationException("Closed or cancelled cycles cannot be updated");

        Name = name;
        StartDate = start;
        EndDate = end;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Start(string modifiedBy)
    {
        if (IsClosed || IsCancelled)
            throw new InvalidOperationException("Closed or cancelled cycles cannot be started");

        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Close(string modifiedBy)
    {
        IsClosed = true;
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Cancel(string modifiedBy)
    {
        if (IsClosed)
            throw new InvalidOperationException("Closed cycles cannot be cancelled");

        IsCancelled = true;
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
