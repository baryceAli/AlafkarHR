namespace PerformanceManagement.Performances;


using Shared.DDD;


public class PerformanceCycle : Aggregate<Guid>
{
    public string Name { get; private set; } // e.g. "2026 Q1"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public bool IsActive { get; private set; }
    public bool IsClosed { get; private set; }

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
            IsActive = true,
            CompanyId = companyId
        };
    }

    public void Close()
    {
        IsClosed = true;
        IsActive = false;
    }
}