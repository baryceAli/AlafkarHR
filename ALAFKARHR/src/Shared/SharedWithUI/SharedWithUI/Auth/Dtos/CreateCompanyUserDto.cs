namespace SharedWithUI.Auth.Dtos;

public class CreateCompanyUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> RoleNames { get; set; } = [];
    public List<Guid> BranchIds { get; set; } = [];
    public Guid? DefaultBranchId { get; set; }
}

public class CreateCompanyUserResultDto
{
    public Guid UserId { get; set; }
    public int AssignedRolesCount { get; set; }
    public int BranchAssignmentsCount { get; set; }
}
