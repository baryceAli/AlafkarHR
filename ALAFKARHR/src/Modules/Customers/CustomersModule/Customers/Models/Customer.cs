using SharedWithUI.Catalog.Dtos;

namespace CustomersModule.Customers.Models;

public class Customer : Aggregate<Guid>
{
    //public string CustomerCode { get; private set; }

    public string Name { get; private set; }

    public string? CommercialName { get; private set; }

    public Guid? CustomerGroupId { get; private set; }

    public CustomerStatus Status { get; private set; }

    public CustomerType Type { get; private set; }

    public decimal CreditLimit { get; private set; }

    //public PaymentTermType PaymentTerm { get; private set; }

    public string? Notes { get; private set; }

    public bool IsTaxExempt { get; private set; }
    private readonly List<Address> _addresses = new();

    public IReadOnlyCollection<Address> Addresses => _addresses;

    private readonly List<Contact> _contacts= new();
    public IReadOnlyCollection<Contact> Contacts =>_contacts;
    public Guid CompanyId { get; set; }
    protected Customer() {}

    public static Customer Create(
        string name,
        string? commercialName,
        CustomerStatus status,
        CustomerType type,
        decimal creditLimit,
        //PaymentTermType paymentTerm,
        string notes,
        bool isTaxExempt,
        Guid companyId,
        string createdBy)
    {
        return new Customer
        {
            Name = name,
            CommercialName = commercialName,
            Status = status,
            Type = type,
            CreditLimit = creditLimit,
            //PaymentTerm = paymentTerm,
            Notes = notes,
            IsTaxExempt = isTaxExempt,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy

        };
    }
    public void Update(
        string name,
        string? commercialName,
        CustomerStatus status,
        CustomerType type,
        decimal creditLimit,
        //PaymentTermType paymentTerm,
        string notes,
        bool isTaxExempt,
        List<Address> addresses,
        List<Contact> contacts,
        string modifiedBy)
    {
        Name=name;
        CommercialName=commercialName;
        Status=status;
        Type=type;
        CreditLimit=creditLimit;
        //PaymentTerm=paymentTerm;
        Notes=notes;
        IsTaxExempt=isTaxExempt;
        ModifiedBy=modifiedBy;
        ModifiedAt = DateTime.UtcNow;


        var activeAddresses = _addresses.Where(a => !a.IsDeleted).ToList();
        var addressIds = activeAddresses.Select(a => a.Id).ToHashSet();
        // Add + Update
        foreach (var a in addresses)
        {
            if (a.Id == Guid.Empty)
            {
                AddAddress(
                    a.Title,
                    a.AddressLine1,
                    a.AddressLine2,
                    a.Longitude,
                    a.Latitude,
                    a.City,
                    a.State,
                    a.Country,
                    a.PostalCode,
                    a.IsDefaultShipping,
                    modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!addressIds.Contains(a.Id))
                throw new Exception($"Invalid or deleted Address Id: {a.Id}");


            var existingValue = activeAddresses.First(ev => ev.Id == a.Id);
            existingValue.Update(a.Title, a.AddressLine1, a.AddressLine2, a.Longitude, a.Latitude, a.City, a.State, a.Country, a.PostalCode, a.IsDefaultShipping);
        }

        // Remove
        var addressDtoIds = addresses
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var addressesToRemove = addressDtoIds.Any() ? activeAddresses
            .Where(ev => !addressDtoIds.Contains(ev.Id))
            .ToList() : [];

        foreach (var value in addressesToRemove)
        {
            value.Remove(modifiedBy);
        }



        var activeContacts = _contacts.Where(c => !c.IsDeleted).ToList();
        var contactIds=activeContacts.Select(c=> c.Id).ToHashSet();
        // Add + Update
        foreach (var c in contacts)
        {
            if (c.Id == Guid.Empty)
            {
                AddContact(c.FullName, c.JobTitle, c.Email, c.PhoneNumber, c.IsPrimaryContact, modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!contactIds.Contains(c.Id))
                throw new Exception($"Invalid or deleted Contact Id: {c.Id}");


            var existingValue = activeContacts.First(ev => ev.Id == c.Id);
            existingValue.Update(c.FullName, c.JobTitle, c.Email, c.PhoneNumber, c.IsPrimaryContact, modifiedBy);
        }

        // Remove
        var contactToRemoveIds = contacts
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var contactsToRemove = contactToRemoveIds.Any() ? activeContacts
            .Where(c => !contactToRemoveIds.Contains(c.Id))
            .ToList() : [];

        foreach (var value in contactsToRemove)
        {
            value.Remove(modifiedBy);
        }


    }

    public void AddAddress(string title, string addressLine1, string? addressLine2,double longitude, double latitude,
        string city, string state, string country,string postalCode,bool isDefaultShipping,string user)
    {
        
         _addresses.Add(Address.Create(title,addressLine1,addressLine2,longitude,latitude,city,state,country,postalCode,isDefaultShipping,user));
    }
    public void AddContact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string modifiedBy)
    {
        _contacts.Add(Contact.Create(fullName, jobTitle, email, phoneNumber, isPrimaryContact, modifiedBy));
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

}