using SharedWithUI.Catalog.Dtos;

namespace CustomersModule.Customers.Models;

public class Customer : Aggregate<Guid>
{
    public string? CustomerCode { get; private set; }

    public string Name { get; private set; }

    public string? CommercialName { get; private set; }

    public string? VatNumber { get; private set; }

    public string? CommercialRegistrationNumber { get; private set; }

    public Guid? CustomerGroupId { get; private set; }

    public CustomerStatus Status { get; private set; }

    //public CustomerType Type { get; private set; }

    public decimal CreditLimit { get; private set; }

    public PaymentTermType PaymentTerm { get; private set; }

    public CreditStatus CreditStatus { get; private set; }

    public string? CreditHoldReason { get; private set; }

    public decimal AvailableCredit { get; private set; }

    public string? Notes { get; private set; }

    public bool IsTaxExempt { get; private set; }

    public Guid? ReceivableAccountId { get; private set; }

    public Guid? IncomeAccountId { get; private set; }

    public Guid? DefaultCurrencyId { get; private set; }

    public string? FiscalPosition { get; private set; }

    public string? CustomerPaymentReference { get; private set; }
    private readonly List<Address> _addresses = new();

    public IReadOnlyCollection<Address> Addresses => _addresses;

    private readonly List<Contact> _contacts= new();
    public IReadOnlyCollection<Contact> Contacts =>_contacts;
    public Guid CompanyId { get; set; }
    protected Customer() {}

    public static Customer Create(
        Guid id,
        string name,
        string? customerCode,
        string? commercialName,
        string? vatNumber,
        string? commercialRegistrationNumber,
        CustomerStatus status,
        decimal creditLimit,
        PaymentTermType paymentTerm,
        CreditStatus creditStatus,
        string? creditHoldReason,
        decimal availableCredit,
        string? notes,
        bool isTaxExempt,
        Guid companyId,
        Guid? customerGroupId,
        string createdBy)
    {
        return Create(
            id,
            name,
            customerCode,
            commercialName,
            vatNumber,
            commercialRegistrationNumber,
            status,
            creditLimit,
            paymentTerm,
            creditStatus,
            creditHoldReason,
            availableCredit,
            notes,
            isTaxExempt,
            null,
            null,
            null,
            null,
            null,
            companyId,
            customerGroupId,
            createdBy);
    }

    public static Customer Create(
        Guid id,
        string name,
        string? customerCode,
        string? commercialName,
        string? vatNumber,
        string? commercialRegistrationNumber,
        CustomerStatus status,
        //CustomerType type,
        decimal creditLimit,
        PaymentTermType paymentTerm,
        CreditStatus creditStatus,
        string? creditHoldReason,
        decimal availableCredit,
        string? notes,
        bool isTaxExempt,
        Guid? receivableAccountId,
        Guid? incomeAccountId,
        Guid? defaultCurrencyId,
        string? fiscalPosition,
        string? customerPaymentReference,
        Guid companyId,
        Guid? customerGroupId,
        string createdBy)
    {
        return new Customer
        {
            Id =id,
            Name = name,
            CustomerCode = customerCode,
            CommercialName = commercialName,
            VatNumber = vatNumber,
            CommercialRegistrationNumber = commercialRegistrationNumber,
            Status = status,
            //Type = type,
            CreditLimit = creditLimit,
            PaymentTerm = paymentTerm,
            CreditStatus = creditStatus,
            CreditHoldReason = creditHoldReason,
            AvailableCredit = availableCredit,
            Notes = notes,
            IsTaxExempt = isTaxExempt,
            ReceivableAccountId = receivableAccountId,
            IncomeAccountId = incomeAccountId,
            DefaultCurrencyId = defaultCurrencyId,
            FiscalPosition = fiscalPosition,
            CustomerPaymentReference = customerPaymentReference,
            CompanyId = companyId,
            CustomerGroupId = customerGroupId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy

        };
    }
    public void Update(
        string name,
        string? customerCode,
        string? commercialName,
        string? vatNumber,
        string? commercialRegistrationNumber,
        CustomerStatus status,
        Guid? customerGroupId,
        decimal creditLimit,
        PaymentTermType paymentTerm,
        CreditStatus creditStatus,
        string? creditHoldReason,
        decimal availableCredit,
        string? notes,
        bool isTaxExempt,
        Guid? receivableAccountId,
        Guid? incomeAccountId,
        Guid? defaultCurrencyId,
        string? fiscalPosition,
        string? customerPaymentReference,
        List<AddressDto> addresses,
        List<ContactDto> contacts,
        string modifiedBy)
    {
        Name=name;
        CustomerCode=customerCode;
        CommercialName=commercialName;
        VatNumber=vatNumber;
        CommercialRegistrationNumber=commercialRegistrationNumber;
        Status=status;
        //Type=type;
        CustomerGroupId=customerGroupId;
        CreditLimit=creditLimit;
        PaymentTerm=paymentTerm;
        CreditStatus=creditStatus;
        CreditHoldReason=creditHoldReason;
        AvailableCredit=availableCredit;
        Notes=notes;
        IsTaxExempt=isTaxExempt;
        ReceivableAccountId = receivableAccountId;
        IncomeAccountId = incomeAccountId;
        DefaultCurrencyId = defaultCurrencyId;
        FiscalPosition = fiscalPosition;
        CustomerPaymentReference = customerPaymentReference;
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
                    a.IsDefaultBilling,
                    a.IsDefaultShipping,
                    a.AddressType,
                    modifiedBy);
                continue;
            }

            // ?? ONLY validate against ACTIVE values
            if (!addressIds.Contains(a.Id))
                throw new Exception($"Invalid or deleted Address Id: {a.Id}");


            var existingValue = activeAddresses.First(ev => ev.Id == a.Id);
            existingValue.Update(a.Title, a.AddressLine1, a.AddressLine2, a.Longitude, a.Latitude, a.City, a.State, a.Country, a.PostalCode, a.IsDefaultBilling, a.IsDefaultShipping, a.AddressType);
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
                AddContact(c.FullName, c.JobTitle, c.Email, c.PhoneNumber, c.IsPrimaryContact, c.ContactType, modifiedBy);
                continue;
            }

            // ?? ONLY validate against ACTIVE values
            if (!contactIds.Contains(c.Id))
                throw new Exception($"Invalid or deleted Contact Id: {c.Id}");


            var existingValue = activeContacts.First(ev => ev.Id == c.Id);
            existingValue.Update(c.FullName, c.JobTitle, c.Email, c.PhoneNumber, c.IsPrimaryContact, c.ContactType, modifiedBy);
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
        string city, string state, string country,string postalCode,bool isDefaultBilling,bool isDefaultShipping,string user)
    {
        AddAddress(title, addressLine1, addressLine2, longitude, latitude, city, state, country, postalCode, isDefaultBilling, isDefaultShipping, PartnerAddressType.Contact, user);
    }

    public void AddAddress(string title, string addressLine1, string? addressLine2,double longitude, double latitude,
        string city, string state, string country,string postalCode,bool isDefaultBilling,bool isDefaultShipping, PartnerAddressType addressType,string user)
    {
        
         _addresses.Add(Address.Create(title,addressLine1,addressLine2,longitude,latitude,city,state,country,postalCode,isDefaultBilling,isDefaultShipping,addressType,user));
    }
    public void AddContact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, string modifiedBy)
    {
        AddContact(fullName, jobTitle, email, phoneNumber, isPrimaryContact, PartnerContactType.Contact, modifiedBy);
    }

    public void AddContact(string fullName, string? jobTitle, string? email, string? phoneNumber, bool isPrimaryContact, PartnerContactType contactType, string modifiedBy)
    {
        _contacts.Add(Contact.Create(fullName, jobTitle, email, phoneNumber, isPrimaryContact, contactType, modifiedBy));
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

}

