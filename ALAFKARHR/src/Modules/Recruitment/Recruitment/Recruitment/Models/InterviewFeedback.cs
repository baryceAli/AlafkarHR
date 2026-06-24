using Shared.DDD;

namespace Recruitment.Recruitment.Models;

public class InterviewFeedback : Aggregate<Guid>
{
    public Guid ApplicantId { get; private set; }
    public Guid InterviewerEmployeeId { get; private set; }
    public DateTime InterviewAt { get; private set; }
    public int Rating { get; private set; }
    public string? Feedback { get; private set; }

    private InterviewFeedback() { }

    public static InterviewFeedback Create(Guid id, Guid applicantId, Guid interviewerEmployeeId, DateTime interviewAt, int rating, string? feedback, string createdBy) => new()
    {
        Id = id,
        ApplicantId = applicantId,
        InterviewerEmployeeId = interviewerEmployeeId,
        InterviewAt = interviewAt,
        Rating = rating,
        Feedback = feedback,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy
    };

    public void Update(Guid interviewerEmployeeId, DateTime interviewAt, int rating, string? feedback, string modifiedBy)
    {
        InterviewerEmployeeId = interviewerEmployeeId;
        InterviewAt = interviewAt;
        Rating = rating;
        Feedback = feedback;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
