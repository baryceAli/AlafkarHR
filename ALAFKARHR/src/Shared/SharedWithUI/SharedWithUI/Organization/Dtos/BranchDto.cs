using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Organization.Dtos;

public enum BranchSpecialization
{
    General = 0,
    StoreFront = 1
}

public class BranchDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage ="NameEng is required")]
    public string NameEng { get; set; }


    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; }


    [Required(ErrorMessage = "Longitude is required")]
    [Range(0.1,500,ErrorMessage = "Longitude Must be greator than 0")]
    public double Longitude { get; set; }
    
    
    [Required(ErrorMessage = "Latitude is required")]
    [Range(0.1, 500, ErrorMessage = "Latitude Must be greator than 0")]
    public double Latitude { get; set; }


    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; }
    
    
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }


    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    public bool IsMainBranch { get; set; }
    public BranchSpecialization Specialization { get; set; } = BranchSpecialization.General;
    public Guid CompanyId { get; set; } // 🔴 VERY IMPORTANT
    //public Company Company { get; set; }

    //private readonly List<Administration> _administrations = new();
    public IReadOnlyCollection<AdministrationDto> Administrations ;

}

public class UserBranchAssignmentsDto
{
    public List<Guid> BranchIds { get; set; } = [];
    public Guid? DefaultBranchId { get; set; }
}

public class CurrentUserBranchAccessDto
{
    public bool CanViewAllBranches { get; set; }
    public List<Guid> BranchIds { get; set; } = [];
}

public class AssignUserBranchesDto
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public List<Guid> BranchIds { get; set; } = [];
    public Guid? DefaultBranchId { get; set; }
}

public class AssignUserBranchesResultDto
{
    public int AssignedCount { get; set; }
}

public class BranchRoleProfileDto
{
    public string TemplateKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}

public class BranchRoleAssignmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BranchId { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string RoleNameAr { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}

public class AssignBranchRoleDto
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BranchId { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
}

public class CurrentUserBranchRoleAccessDto
{
    public Guid CompanyId { get; set; }
    public List<BranchRoleAssignmentDto> Assignments { get; set; } = [];
    public List<string> EffectivePermissions { get; set; } = [];
}
