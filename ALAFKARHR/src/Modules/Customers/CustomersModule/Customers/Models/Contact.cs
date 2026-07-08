namespace CustomersModule.Customers.Models;

public class Contact:Entity<Guid>
{
    public string FullName { get; private set; }

    public string? JobTitle { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }
    
    public bool IsPrimaryContact { get; private set; }
    public PartnerContactType ContactType { get; private set; } = PartnerContactType.Contact;
    protected Contact() { }
    internal Contact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, PartnerContactType contactType)
    {
        Id = Guid.Empty;
        FullName = fullName;
        JobTitle = jobTitle;
        Email = email;
        PhoneNumber = phoneNumber;
        IsPrimaryContact = isPrimaryContact;
        ContactType = contactType;
    }
    public static Contact Create(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, PartnerContactType contactType, string createdBy)
    {
        return new Contact
        {
            Id=Guid.NewGuid(),
            FullName = fullName,
            JobTitle = jobTitle,
            Email = email,
            PhoneNumber = phoneNumber,
            IsPrimaryContact = isPrimaryContact,
            ContactType = contactType,
            CreatedBy= createdBy,
            CreatedAt= DateTime.UtcNow
        };
    }
    public void Update(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, PartnerContactType contactType,string modifiedBy)
    {
        FullName=fullName;
        JobTitle=jobTitle;
        Email=email;
        PhoneNumber=phoneNumber;
        IsPrimaryContact = isPrimaryContact;
        ContactType = contactType;
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
