using Shared.DDD;

namespace PerformanceManagement.Performances;


public class Competency : Aggregate<Guid>
{
    public string Name { get; private set; } // Communication, Leadership
    public decimal Weight { get; private set; }

    public Guid CompanyId { get; private set; }

    private Competency() { }

    public static Competency Create(Guid id, Guid companyId, string name, decimal weight, string createdBy)
    {
        return new Competency
        {
            Id = id,
            CompanyId = companyId,
            Name = name,
            Weight = weight,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, decimal weight, string modifiedBy)
    {
        Name = name;
        Weight = weight;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
