namespace SuppliersModule.Suppliers.Models;

public class Supplier : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? CommercialName { get; private set; }
    public string SupplierCode { get; private set; } = string.Empty;
    public Guid? SupplierGroupId { get; private set; }
    public SupplierStatus Status { get; private set; }
    public SupplierType Type { get; private set; }
    public SupplierPaymentTermType PaymentTerm { get; private set; }
    public string? TaxNumber { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public string? Notes { get; private set; }
    public Guid CompanyId { get; private set; }

    private readonly List<SupplierAddress> _addresses = new();
    public IReadOnlyCollection<SupplierAddress> Addresses => _addresses;

    private readonly List<SupplierContact> _contacts = new();
    public IReadOnlyCollection<SupplierContact> Contacts => _contacts;

    protected Supplier() { }

    public static Supplier Create(string name, string? commercialName, string supplierCode, Guid? supplierGroupId, SupplierStatus status, SupplierType type, SupplierPaymentTermType paymentTerm, string? taxNumber, decimal creditLimit, decimal openingBalance, string? notes, Guid companyId, string createdBy)
    {
        return new Supplier
        {
            Id = Guid.NewGuid(),
            Name = name,
            CommercialName = commercialName,
            SupplierCode = supplierCode,
            SupplierGroupId = supplierGroupId,
            Status = status,
            Type = type,
            PaymentTerm = paymentTerm,
            TaxNumber = taxNumber,
            CreditLimit = creditLimit,
            OpeningBalance = openingBalance,
            Notes = notes,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string? commercialName, string supplierCode, Guid? supplierGroupId, SupplierStatus status, SupplierType type, SupplierPaymentTermType paymentTerm, string? taxNumber, decimal creditLimit, decimal openingBalance, string? notes, List<SupplierAddress> addresses, List<SupplierContact> contacts, string modifiedBy)
    {
        Name = name;
        CommercialName = commercialName;
        SupplierCode = supplierCode;
        SupplierGroupId = supplierGroupId;
        Status = status;
        Type = type;
        PaymentTerm = paymentTerm;
        TaxNumber = taxNumber;
        CreditLimit = creditLimit;
        OpeningBalance = openingBalance;
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

        SyncAddresses(addresses, modifiedBy);
        SyncContacts(contacts, modifiedBy);
    }

    public void AddAddress(string title, string addressLine1, string? addressLine2, double longitude, double latitude, string city, string state, string country, string postalCode, bool isDefaultBilling, string createdBy)
    {
        _addresses.Add(SupplierAddress.Create(title, addressLine1, addressLine2, longitude, latitude, city, state, country, postalCode, isDefaultBilling, createdBy));
    }

    public void AddContact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string createdBy)
    {
        _contacts.Add(SupplierContact.Create(fullName, jobTitle, email, phoneNumber, isPrimaryContact, createdBy));
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private void SyncAddresses(List<SupplierAddress> addresses, string modifiedBy)
    {
        var activeAddresses = _addresses.Where(a => !a.IsDeleted).ToList();
        var addressIds = activeAddresses.Select(a => a.Id).ToHashSet();

        foreach (var address in addresses)
        {
            if (address.Id == Guid.Empty)
            {
                AddAddress(address.Title, address.AddressLine1, address.AddressLine2, address.Longitude, address.Latitude, address.City, address.State, address.Country, address.PostalCode, address.IsDefaultBilling, modifiedBy);
                continue;
            }

            if (!addressIds.Contains(address.Id))
                throw new BadRequestException($"Invalid or deleted supplier address id: {address.Id}");

            var existing = activeAddresses.First(a => a.Id == address.Id);
            existing.Update(address.Title, address.AddressLine1, address.AddressLine2, address.Longitude, address.Latitude, address.City, address.State, address.Country, address.PostalCode, address.IsDefaultBilling, modifiedBy);
        }

        var incomingIds = addresses.Where(a => a.Id != Guid.Empty).Select(a => a.Id).ToHashSet();
        foreach (var address in activeAddresses.Where(a => !incomingIds.Contains(a.Id)))
        {
            address.Remove(modifiedBy);
        }
    }

    private void SyncContacts(List<SupplierContact> contacts, string modifiedBy)
    {
        var activeContacts = _contacts.Where(c => !c.IsDeleted).ToList();
        var contactIds = activeContacts.Select(c => c.Id).ToHashSet();

        foreach (var contact in contacts)
        {
            if (contact.Id == Guid.Empty)
            {
                AddContact(contact.FullName, contact.JobTitle, contact.Email, contact.PhoneNumber, contact.IsPrimaryContact, modifiedBy);
                continue;
            }

            if (!contactIds.Contains(contact.Id))
                throw new BadRequestException($"Invalid or deleted supplier contact id: {contact.Id}");

            var existing = activeContacts.First(c => c.Id == contact.Id);
            existing.Update(contact.FullName, contact.JobTitle, contact.Email, contact.PhoneNumber, contact.IsPrimaryContact, modifiedBy);
        }

        var incomingIds = contacts.Where(c => c.Id != Guid.Empty).Select(c => c.Id).ToHashSet();
        foreach (var contact in activeContacts.Where(c => !incomingIds.Contains(c.Id)))
        {
            contact.Remove(modifiedBy);
        }
    }
}
