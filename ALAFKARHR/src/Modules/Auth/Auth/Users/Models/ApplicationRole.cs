namespace Auth.Users.Models;

public class ApplicationRole:IdentityRole<Guid>
{
    public Guid CompanyId { get;  set; }

    public ApplicationRole()
    {
        
    }
}
