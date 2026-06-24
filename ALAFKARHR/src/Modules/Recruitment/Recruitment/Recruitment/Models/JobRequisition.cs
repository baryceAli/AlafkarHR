using Shared.DDD;

namespace Recruitment.Recruitment.Models;

public class JobRequisition : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? PositionId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Openings { get; private set; }
    public RecruitmentRequestStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }

    private JobRequisition() { }

    public static JobRequisition Create(Guid id, Guid companyId, Guid? departmentId, Guid? positionId, string title, int openings, DateTime requestedAt, string createdBy) => new()
    {
        Id = id,
        CompanyId = companyId,
        DepartmentId = departmentId,
        PositionId = positionId,
        Title = title,
        Openings = openings,
        RequestedAt = requestedAt,
        Status = RecruitmentRequestStatus.Draft,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy
    };

    public void Update(Guid? departmentId, Guid? positionId, string title, int openings, DateTime requestedAt, string modifiedBy)
    {
        if (Status is RecruitmentRequestStatus.Hired or RecruitmentRequestStatus.Cancelled)
            throw new InvalidOperationException("Closed requisitions cannot be updated");
        DepartmentId = departmentId;
        PositionId = positionId;
        Title = title;
        Openings = openings;
        RequestedAt = requestedAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Open(string modifiedBy) => MoveTo(RecruitmentRequestStatus.Open, modifiedBy);
    public void Cancel(string modifiedBy) => MoveTo(RecruitmentRequestStatus.Cancelled, modifiedBy);
    public void Close(string modifiedBy) => MoveTo(RecruitmentRequestStatus.Hired, modifiedBy);

    private void MoveTo(RecruitmentRequestStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
