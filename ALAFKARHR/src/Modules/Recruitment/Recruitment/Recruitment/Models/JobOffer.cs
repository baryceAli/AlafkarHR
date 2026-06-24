using Shared.DDD;

namespace Recruitment.Recruitment.Models;

public class JobOffer : Aggregate<Guid>
{
    public Guid ApplicantId { get; private set; }
    public DateTime OfferDate { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public decimal? ProposedSalary { get; private set; }
    public Guid? CreatedEmployeeId { get; private set; }

    private JobOffer() { }

    public static JobOffer Create(Guid id, Guid applicantId, DateTime offerDate, decimal? proposedSalary, string createdBy) => new()
    {
        Id = id,
        ApplicantId = applicantId,
        OfferDate = offerDate,
        ProposedSalary = proposedSalary,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy
    };

    public void Update(DateTime offerDate, decimal? proposedSalary, string modifiedBy)
    {
        if (AcceptedAt.HasValue || RejectedAt.HasValue)
            throw new InvalidOperationException("Accepted or rejected offers cannot be updated");
        OfferDate = offerDate;
        ProposedSalary = proposedSalary;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Accept(string modifiedBy)
    {
        AcceptedAt = DateTime.UtcNow;
        RejectedAt = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Reject(string modifiedBy)
    {
        RejectedAt = DateTime.UtcNow;
        AcceptedAt = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void MarkEmployeeCreated(Guid employeeId, string modifiedBy)
    {
        if (!AcceptedAt.HasValue)
            throw new InvalidOperationException("Only accepted offers can be marked as employee-created");
        CreatedEmployeeId = employeeId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
