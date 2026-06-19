namespace Auth.Users.Models;

public class ApplicationRole:IdentityRole<Guid>
{
    public Guid? CompanyId { get;  set; }
    public string? DisplayName { get; set; }
    public string? TemplateKey { get; set; }

    public ApplicationRole()
    {
        
    }
}
