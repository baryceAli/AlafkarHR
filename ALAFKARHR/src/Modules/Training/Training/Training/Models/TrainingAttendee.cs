using Shared.DDD;

namespace Training.Training.Models;

public class TrainingAttendee : Entity<Guid>
{
    public Guid TrainingEventId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public bool Attended { get; private set; }
    public bool Passed { get; private set; }
    public TrainingAttendeeResultStatus Status { get; private set; } = TrainingAttendeeResultStatus.Assigned;
    public decimal? Score { get; private set; }
    public string? Feedback { get; private set; }
    public Guid? CertificationId { get; private set; }
    public string? CertificateName { get; private set; }
    public string? CertificateIssuer { get; private set; }
    public DateTime? CertificateIssuedAt { get; private set; }
    public DateTime? CertificateExpiresAt { get; private set; }

    private TrainingAttendee() { }

    public static TrainingAttendee Create(Guid id, Guid trainingEventId, Guid employeeId, string userId)
    {
        if (trainingEventId == Guid.Empty) throw new InvalidOperationException("Training event is required.");
        if (employeeId == Guid.Empty) throw new InvalidOperationException("Employee is required.");

        return new TrainingAttendee
        {
            Id = id,
            TrainingEventId = trainingEventId,
            EmployeeId = employeeId,
            Status = TrainingAttendeeResultStatus.Assigned,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = userId
        };
    }

    public void ChangeEmployee(Guid employeeId, string userId)
    {
        if (employeeId == Guid.Empty) throw new InvalidOperationException("Employee is required.");
        EmployeeId = employeeId;
        Touch(userId);
    }

    public void RecordResult(bool attended, bool passed, decimal? score, string? feedback, string userId)
    {
        Attended = attended;
        Passed = attended && passed;
        Score = score;
        Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
        Status = !attended ? TrainingAttendeeResultStatus.Assigned : Passed ? TrainingAttendeeResultStatus.Passed : TrainingAttendeeResultStatus.Failed;
        Touch(userId);
    }

    public void MarkAttended(bool attended, string userId)
    {
        Attended = attended;
        Status = attended ? TrainingAttendeeResultStatus.Attended : TrainingAttendeeResultStatus.Assigned;
        Touch(userId);
    }

    public void LinkCertification(Guid? certificationId, string? certificateName, string? issuer, DateTime? issuedAt, DateTime? expiresAt, string userId)
    {
        CertificationId = certificationId;
        CertificateName = string.IsNullOrWhiteSpace(certificateName) ? null : certificateName.Trim();
        CertificateIssuer = string.IsNullOrWhiteSpace(issuer) ? null : issuer.Trim();
        CertificateIssuedAt = issuedAt;
        CertificateExpiresAt = expiresAt;
        Touch(userId);
    }

    private void Touch(string userId)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
