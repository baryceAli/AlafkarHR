namespace SharedWithUI.Auth.Dtos;

public class AuthDashboardDto
{
    public Guid CompanyId { get; set; }
    public bool CanViewAllBranches { get; set; }
    public int VisibleBranchCount { get; set; }
    public int VisibleUserCount { get; set; }
    public int TotalRoleCount { get; set; }
    public int AssignedRoleLinkCount { get; set; }
    public int UsersWithoutRolesCount { get; set; }
    public List<AuthDashboardBranchDto> Branches { get; set; } = [];
    public List<AuthDashboardRoleDto> Roles { get; set; } = [];
    public List<AuthDashboardUserDto> UsersWithoutRoles { get; set; } = [];
    public List<AuthDashboardUserDto> ProtectedUsers { get; set; } = [];
    public List<AuthDashboardStoreBranchRoleDto> StoreBranchRoles { get; set; } = [];
}

public class AuthDashboardBranchDto
{
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public bool IsMainBranch { get; set; }
    public int UserCount { get; set; }
    public int StoreBranchRoleCount { get; set; }
}

public class AuthDashboardRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public bool IsSystemAdminRole { get; set; }
}

public class AuthDashboardUserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int AssignedRoleCount { get; set; }
    public int AssignedBranchCount { get; set; }
}

public class AuthDashboardStoreBranchRoleDto
{
    public Guid AssignmentId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string BranchNameEng { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
}
