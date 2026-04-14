namespace AlAfkarERP.Shared.Pages.Features.Company.Dtos;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public string Logo { get; private set; }
    public string HqLocation { get; private set; }
    public double HqLongitude { get; private set; }
    public double HqLatitude { get; private set; }
    public string VatNo { get; private set; }


    public string Code { get; private set; } // unique org code
    public string Currency { get; private set; } // critical for payroll
    public string TimeZone { get; private set; } // critical for attendance


    public string Phone { get; private set; }
    public string Email { get; private set; }



    //private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<BranchDto> Branches ;

}
