namespace SharedWithUI.Recruitment.Dtos;

public enum RecruitmentRequestStatus
{
    Draft,
    Open,
    Interviewing,
    Offered,
    Hired,
    Cancelled
}

public class StaffingPlanDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
    public int PlannedHeadcount { get; set; }
    public string? Notes { get; set; }
}

public class JobRequisitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Openings { get; set; } = 1;
    public RecruitmentRequestStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.Today;
}

public class ApplicantDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? JobRequisitionId { get; set; }
    public string? JobRequisitionTitle { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public RecruitmentRequestStatus Status { get; set; }
    public string? StatusLabel { get; set; }
}

public class InterviewFeedbackDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid InterviewerEmployeeId { get; set; }
    public string? InterviewerEmployeeName { get; set; }
    public DateTime InterviewAt { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
}

public class JobOfferDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public string? ApplicantName { get; set; }
    public DateTime OfferDate { get; set; } = DateTime.Today;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public decimal? ProposedSalary { get; set; }
    public Guid? CreatedEmployeeId { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsRejected { get; set; }
    public string? StatusLabel { get; set; }
}

public class UpsertStaffingPlanDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public int PlannedHeadcount { get; set; } = 1;
    public string? Notes { get; set; }
}

public class UpsertJobRequisitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Openings { get; set; } = 1;
    public DateTime RequestedAt { get; set; } = DateTime.Today;
}

public class UpsertApplicantDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? JobRequisitionId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class UpsertInterviewFeedbackDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid InterviewerEmployeeId { get; set; }
    public DateTime InterviewAt { get; set; } = DateTime.Today;
    public int Rating { get; set; } = 3;
    public string? Feedback { get; set; }
}

public class UpsertJobOfferDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public DateTime OfferDate { get; set; } = DateTime.Today;
    public decimal? ProposedSalary { get; set; }
}

public class RecruitmentStatusActionDto
{
    public RecruitmentRequestStatus Status { get; set; }
}

public class RecruitmentHireActionDto
{
    public Guid EmployeeId { get; set; }
}

public class RecruitmentActionResultDto
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
