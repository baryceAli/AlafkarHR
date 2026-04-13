using Shared.DDD;

namespace PerformanceManagement.Performances;


public class Competency : Aggregate<Guid>
{
    public string Name { get; private set; } // Communication, Leadership
    public decimal Weight { get; private set; }

    public Guid CompanyId { get; private set; }

    private Competency() { }
}