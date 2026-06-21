using SharedWithUI.Employees.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class EmployeeTeamDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? NameEng { get; set; }
    public EmployeeTeamCategory Category { get; set; } = EmployeeTeamCategory.Projects;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public Guid? CreatedForProjectId { get; set; }
    public List<EmployeeTeamMemberDto> Members { get; set; } = [];
    public int MemberCount => Members.Count;
}

public class EmployeeTeamMemberDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }

    [Required]
    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeNameEng { get; set; }
    public string? EmployeeNo { get; set; }
}
