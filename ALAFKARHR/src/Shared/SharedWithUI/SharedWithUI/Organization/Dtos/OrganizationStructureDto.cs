namespace SharedWithUI.Organization.Dtos;

public class OrganizationStructureDto
{
    public List<OrganizationCompanyNodeDto> Companies { get; set; } = [];
    public int CompanyCount { get; set; }
    public int BranchCount { get; set; }
    public int AdministrationCount { get; set; }
    public int DepartmentCount { get; set; }
}

public class OrganizationCompanyNodeDto
{
    public Guid Id { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string HqLocation { get; set; } = string.Empty;
    public double HqLongitude { get; set; }
    public double HqLatitude { get; set; }
    public string VatNo { get; set; } = string.Empty;
    public Guid? CurrencyId { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int BranchCount { get; set; }
    public int AdministrationCount { get; set; }
    public int DepartmentCount { get; set; }
    public List<OrganizationCompanyNodeDto> ChildCompanies { get; set; } = [];
    public List<OrganizationAdministrationNodeDto> Administrations { get; set; } = [];
    public List<OrganizationBranchNodeDto> Branches { get; set; } = [];
}

public class OrganizationBranchNodeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsMainBranch { get; set; }
    public BranchSpecialization Specialization { get; set; }
    public int AdministrationCount { get; set; }
    public int DepartmentCount { get; set; }
    public List<OrganizationAdministrationNodeDto> Administrations { get; set; } = [];
}

public class OrganizationAdministrationNodeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? ParentAdministrationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public bool IsHigherManagement { get; set; }
    public bool IsActive { get; set; }
    public int DepartmentCount { get; set; }
    public List<OrganizationAdministrationNodeDto> ChildAdministrations { get; set; } = [];
    public List<OrganizationDepartmentNodeDto> Departments { get; set; } = [];
}

public class OrganizationDepartmentNodeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid AdministrationId { get; set; }
    public Guid? ParentDepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? HeadOfDepartment { get; set; }
    public bool IsActive { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int AllowedRadiusMeters { get; set; }
    public List<OrganizationDepartmentNodeDto> ChildDepartments { get; set; } = [];
}
