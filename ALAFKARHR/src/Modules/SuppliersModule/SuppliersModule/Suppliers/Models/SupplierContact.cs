namespace SuppliersModule.Suppliers.Models;

public class SupplierContact : Entity<Guid>
{
    public string FullName { get; private set; } = string.Empty;
    public string? JobTitle { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsPrimaryContact { get; private set; }

    protected SupplierContact() { }

    internal SupplierContact(Guid id, string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact)
    {
        Id = id;
        FullName = fullName;
        JobTitle = jobTitle;
        Email = email;
        PhoneNumber = phoneNumber;
        IsPrimaryContact = isPrimaryContact;
    }

    public static SupplierContact Create(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string createdBy)
    {
        return new SupplierContact
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            JobTitle = jobTitle,
            Email = email,
            PhoneNumber = phoneNumber,
            IsPrimaryContact = isPrimaryContact,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string modifiedBy)
    {
        FullName = fullName;
        JobTitle = jobTitle;
        Email = email;
        PhoneNumber = phoneNumber;
        IsPrimaryContact = isPrimaryContact;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
