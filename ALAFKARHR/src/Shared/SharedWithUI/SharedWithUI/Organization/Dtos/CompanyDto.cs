namespace SharedWithUI.Organization.Dtos;

public class CompanyDto
{
    public Guid Id { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public string? ParentCompanyName { get; set; }
    public int ChildCompaniesCount { get; set; }
    public string Name { get;  set; }
    public string NameEng { get; set; }
    public string Logo { get; set; }
    public string HqLocation { get; set; }
    public double HqLongitude { get; set; }
    public double HqLatitude { get; set; }
    public string VatNo { get; set; }


    public string Code { get; set; } // unique org code
    public string Currency { get; set; } // critical for payroll
    public string TimeZone { get; set; } // critical for attendance


    public string Phone { get; set; }
    public string Email { get; set; }

    public string? AdminUserName { get; set; }
    public string? AdminEmail { get; set; }
    public string? AdminPhoneNumber { get; set; }
    public string? AdminTemporaryPassword { get; set; }



    //private readonly List<Branch> _branches = new();
    public List<BranchDto> Branches ;

}
