namespace SharedWithUI.Employees.Dtos;

public class PublicEmployeeViewDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string FirstNameEng { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string MiddleNameEng { get; set; } = "";
    public string LastName { get; set; } = "";
    public string LastNameEng { get; set; } = "";
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    public string FullNameEng => $"{FirstNameEng} {MiddleNameEng} {LastNameEng}".Trim();
    public string NationalId { get; set; } = "";
    public string Nationality { get; set; } = "";
    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string? PositionNameEng { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? BranchLocation { get; set; }
}
