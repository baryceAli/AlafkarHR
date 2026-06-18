namespace SharedWithUI.RealEstate.Enums;

public enum PropertyStatus
{
    Draft = 1,
    Active = 2,
    UnderPreparation = 3,
    Inactive = 4
}

public enum UnitStatus
{
    Available = 1,
    Reserved = 2,
    Occupied = 3,
    UnderMaintenance = 4,
    Inactive = 5
}

public enum PropertyUnitType
{
    Apartment = 1,
    Office = 2,
    Shop = 3,
    Room = 4,
    Floor = 5,
    Other = 99
}

public enum LeaseDirection
{
    OwnerToCompany = 1,
    CompanyToTenant = 2
}

public enum LeaseStatus
{
    Draft = 1,
    Active = 2,
    Suspended = 3,
    Expired = 4,
    Terminated = 5
}

public enum BillingFrequency
{
    Monthly = 1,
    Quarterly = 2,
    SemiAnnual = 3,
    Annual = 4,
    OneTime = 5,
    Custom = 6
}

public enum InstallmentStatus
{
    Pending = 1,
    PartiallyPaid = 2,
    Paid = 3,
    Overdue = 4,
    Waived = 5,
    Cancelled = 6
}

public enum ExpenseCategory
{
    OwnerRent = 1,
    Electricity = 2,
    Water = 3,
    Internet = 4,
    Cleaning = 5,
    Security = 6,
    Maintenance = 7,
    Furnishing = 8,
    Renovation = 9,
    BrokerFee = 10,
    Other = 99
}

public enum UtilityServiceType
{
    Electricity = 1,
    Water = 2,
    Internet = 3,
    Gas = 4,
    Other = 99
}
