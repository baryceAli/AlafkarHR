namespace SharedWithUI.Customers.Dtos;

public class ContactDto
{
    public Guid Id { get; set; }
    public string FullName { get;  set; }

    public string? JobTitle { get;  set; }

    public string? Email { get;  set; }

    public string? PhoneNumber { get;  set; }

    public bool IsPrimaryContact { get;  set; }

}
