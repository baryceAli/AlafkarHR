namespace CustomersModule.Customers.Models;

public class Contact:Entity<Guid>
{
    public string FullName { get; private set; }

    public string? JobTitle { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }
    
    public bool IsPrimaryContact { get; private set; }
    protected Contact() { }
    internal Contact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact)
    {
        Id = Guid.Empty;
        FullName = fullName;
        JobTitle = jobTitle;
        Email = email;
        PhoneNumber = phoneNumber;
        IsPrimaryContact = isPrimaryContact;
    }
    public static Contact Create(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string createdBy)
    {
        return new Contact
        {
            Id=Guid.NewGuid(),
            FullName = fullName,
            JobTitle = jobTitle,
            Email = email,
            PhoneNumber = phoneNumber,
            IsPrimaryContact = isPrimaryContact,
            CreatedBy= createdBy,
            CreatedAt= DateTime.UtcNow
        };
    }
    public void Update(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact,string modifiedBy)
    {
        FullName=fullName;
        JobTitle=jobTitle;
        Email=email;
        PhoneNumber=phoneNumber;
        IsPrimaryContact = isPrimaryContact;
        ModifiedAt= DateTime.UtcNow;
        ModifiedBy= modifiedBy;

    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy= deletedBy;
    }
}