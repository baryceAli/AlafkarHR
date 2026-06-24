namespace SharedWithUI.Training.Dtos;

public enum TrainingEventStatus
{
    Planned,
    Open,
    InProgress,
    Completed,
    Cancelled
}

public enum TrainingAttendeeResultStatus
{
    Assigned,
    Attended,
    Passed,
    Failed,
    Cancelled
}

public class TrainingProgramDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Provider { get; set; }
    public string? Objective { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class TrainingEventDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public Guid CompanyId { get; set; }
    public string? ProgramName { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int Capacity { get; set; }
    public TrainingEventStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public int AttendeeCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class TrainingAttendeeDto
{
    public Guid Id { get; set; }
    public Guid TrainingEventId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public bool Attended { get; set; }
    public bool Passed { get; set; }
    public TrainingAttendeeResultStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public string? CertificateName { get; set; }
    public string? CertificateIssuer { get; set; }
    public DateTime? CertificateIssuedAt { get; set; }
    public DateTime? CertificateExpiresAt { get; set; }
    public Guid? CertificationId { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class UpsertTrainingProgramDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Provider { get; set; }
    public string? Objective { get; set; }
    public string? Description { get; set; }
}

public class UpsertTrainingEventDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int Capacity { get; set; }
}

public class UpsertTrainingAttendeeDto
{
    public Guid Id { get; set; }
    public Guid TrainingEventId { get; set; }
    public Guid EmployeeId { get; set; }
}

public class TrainingAttendeeResultDto
{
    public bool Attended { get; set; }
    public bool Passed { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
}

public class TrainingCertificateLinkDto
{
    public Guid? CertificationId { get; set; }
    public string? CertificateName { get; set; }
    public string? CertificateIssuer { get; set; }
    public DateTime? CertificateIssuedAt { get; set; }
    public DateTime? CertificateExpiresAt { get; set; }
}

public class TrainingActionResultDto
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
