namespace SharedWithUI.HRCore.Dtos;

public enum HrLifecycleEventType
{
    Onboarding,
    Transfer,
    Promotion,
    Separation,
    ExitInterview,
    FinalSettlementReady
}

public enum HrLifecycleEventStatus
{
    Draft,
    Submitted,
    Approved,
    Completed,
    Cancelled
}

public class HrLifecycleEventDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNameEng { get; set; }
    public string? EmployeeNo { get; set; }
    public Guid CompanyId { get; set; }
    public HrLifecycleEventType EventType { get; set; }
    public HrLifecycleEventStatus Status { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromDepartmentId { get; set; }
    public Guid? ToDepartmentId { get; set; }
    public Guid? FromPositionId { get; set; }
    public Guid? ToPositionId { get; set; }
    public Guid? FromManagerEmployeeId { get; set; }
    public Guid? ToManagerEmployeeId { get; set; }
    public string? FromGrade { get; set; }
    public string? ToGrade { get; set; }
    public string? FromWorkLocation { get; set; }
    public string? ToWorkLocation { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class EmployeeEmergencyContactDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class EmployeeDocumentLinkDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? DocumentId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool RequiresRenewal => ExpiryDate.HasValue && ExpiryDate.Value.Date <= DateTime.Today.AddDays(60);
}

public class EmployeeSkillDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int ProficiencyLevel { get; set; }
    public string? Source { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class EmployeeCertificationDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? DocumentId { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value.Date < DateTime.Today;
}

public class HrCommandCenterDto
{
    public int EmployeeCount { get; set; }
    public int OpenLifecycleEvents { get; set; }
    public int ExpiringDocuments { get; set; }
    public int OpenRecruitmentRequests { get; set; }
    public int PendingLeaveApplications { get; set; }
    public int PayrollItemsNeedingReview { get; set; }
    public int UpcomingAppraisals { get; set; }
    public int ActiveTrainingEvents { get; set; }
}
