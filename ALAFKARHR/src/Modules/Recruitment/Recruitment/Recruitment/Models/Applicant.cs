using Shared.DDD;

namespace Recruitment.Recruitment.Models;

public class Applicant : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid? JobRequisitionId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public RecruitmentRequestStatus Status { get; private set; }

    private Applicant() { }

    public static Applicant Create(Guid id, Guid companyId, Guid? jobRequisitionId, string fullName, string email, string phone, string createdBy) => new()
    {
        Id = id,
        CompanyId = companyId,
        JobRequisitionId = jobRequisitionId,
        FullName = fullName,
        Email = email,
        Phone = phone,
        Status = RecruitmentRequestStatus.Open,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy
    };

    public void Update(Guid? jobRequisitionId, string fullName, string email, string phone, string modifiedBy)
    {
        if (Status is RecruitmentRequestStatus.Hired or RecruitmentRequestStatus.Cancelled)
            throw new InvalidOperationException("Closed applicants cannot be updated");
        JobRequisitionId = jobRequisitionId;
        FullName = fullName;
        Email = email;
        Phone = phone;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void MoveTo(RecruitmentRequestStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
