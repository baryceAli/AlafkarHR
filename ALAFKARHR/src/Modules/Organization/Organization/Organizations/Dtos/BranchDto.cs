namespace AlAfkarERP.Shared.Pages.Features.Company.Dtos;

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string Location { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public string Code { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsMainBranch { get; set; }
    public Guid CompanyId { get; set; } // 🔴 VERY IMPORTANT
    //public Company Company { get; set; }

    //private readonly List<Administration> _administrations = new();
    public IReadOnlyCollection<AdministrationDto> Administrations ;

}
