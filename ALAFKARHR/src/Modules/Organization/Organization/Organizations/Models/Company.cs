
using Shared.DDD;
namespace Organization.Organizations.Models;

public class Company : Aggregate<Guid>
{
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



    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches;

    private Company() { }


    public void AddBranch(Branch branch)
    {
        //var createdBranch = Branch.Create(
        //    branch.Id,
        //    branch.Name,
        //    branch.NameEng,
        //    branch.Location,
        //    branch.Longitude,
        //    branch.Latitude,
        //    branch.TenantId,
        //    branch.CreatedBy
        //    );
        if (_branches.Any(b => b.Id == branch.Id))
            throw new Exception("Branch already exists");

        _branches.Add(branch);
    }
    public void RemoveBranch(Branch branch)
    {
        _branches.Remove(branch);
    }


    public static Company Create(
        Guid id,
        string name,
        string nameEng,
        string logo,
        string hqLocation,
        double hqLongitude,
        double hqLatitude,
        string vatNo,
        string code,
        string currency,
        string email,
        string phone,
        string timeZone,
        string createdBy)
    {

        ArgumentNullException.ThrowIfNullOrEmpty(name, "Name is required");
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng, "NameEng is required");
        ArgumentNullException.ThrowIfNullOrEmpty(vatNo, "VatNo is required");
        return new Company
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Logo = logo,
            HqLocation = hqLocation,
            HqLongitude = hqLongitude,
            HqLatitude = hqLatitude,
            VatNo = vatNo,
            Code=code,
            Currency=currency,
            Email=email,
            Phone=phone,
            TimeZone=timeZone,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(
        string name,
        string nameEng,
        string logo,
        string hqLocation,
        double hqLongitude,
        double hqLatitude,
        string vatNo,
        string modifiedBy)
    {
        Name = name;
        NameEng = nameEng;
        Logo = logo;
        HqLocation = hqLocation;
        HqLongitude = hqLongitude;
        HqLatitude = hqLatitude;
        VatNo = vatNo;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy.Trim();
        IsDeleted = true;
    }
}
