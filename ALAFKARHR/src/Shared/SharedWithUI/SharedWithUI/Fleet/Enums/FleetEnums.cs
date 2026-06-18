namespace SharedWithUI.Fleet.Enums;

public enum FleetVehicleOwnershipType
{
    Owned = 1,
    Rented = 2
}

public enum FleetVehicleType
{
    Sedan = 1,
    Suv = 2,
    Pickup = 3,
    Van = 4,
    Truck = 5,
    Bus = 6,
    Motorcycle = 7,
    Other = 99
}

public enum FleetVehicleStatus
{
    Active = 1,
    Assigned = 2,
    UnderMaintenance = 3,
    Inactive = 4,
    Retired = 5
}

public enum FleetFuelType
{
    Petrol = 1,
    Diesel = 2,
    Hybrid = 3,
    Electric = 4,
    Other = 99
}

public enum FleetAssignmentStatus
{
    Active = 1,
    Returned = 2,
    Cancelled = 3
}

public enum FleetDocumentType
{
    Registration = 1,
    Insurance = 2,
    License = 3,
    Inspection = 4,
    RentalContractAttachment = 5,
    Other = 99
}

public enum FleetDocumentStatus
{
    Active = 1,
    ExpiringSoon = 2,
    Expired = 3,
    Renewed = 4
}

public enum FleetExpenseCategory
{
    Fuel = 1,
    Oil = 2,
    Maintenance = 3,
    RegistrationRenewal = 4,
    InsuranceRenewal = 5,
    Inspection = 6,
    RentalPayment = 7,
    Fine = 8,
    Parking = 9,
    Washing = 10,
    Other = 99
}

public enum FleetExpenseApprovalStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}

public enum FleetServiceType
{
    OilChange = 1,
    Tires = 2,
    Inspection = 3,
    GeneralService = 4,
    Other = 99
}
