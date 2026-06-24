using Shared.DDD;

namespace Recruitment.Recruitment.Models;

public class StaffingPlan : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public int Year { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? PositionId { get; private set; }
    public int PlannedHeadcount { get; private set; }
    public string? Notes { get; private set; }

    private StaffingPlan() { }

    public static StaffingPlan Create(Guid id, Guid companyId, int year, Guid? departmentId, Guid? positionId, int plannedHeadcount, string? notes, string createdBy) => new()
    {
        Id = id,
        CompanyId = companyId,
        Year = year,
        DepartmentId = departmentId,
        PositionId = positionId,
        PlannedHeadcount = plannedHeadcount,
        Notes = notes,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy
    };

    public void Update(int year, Guid? departmentId, Guid? positionId, int plannedHeadcount, string? notes, string modifiedBy)
    {
        Year = year;
        DepartmentId = departmentId;
        PositionId = positionId;
        PlannedHeadcount = plannedHeadcount;
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
