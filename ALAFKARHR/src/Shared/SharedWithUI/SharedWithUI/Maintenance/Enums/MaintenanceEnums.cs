namespace SharedWithUI.Maintenance.Enums;

public enum MaintenanceAssetType
{
    Building = 1,
    Apartment = 2,
    Office = 3,
    Vehicle = 4,
    Equipment = 5,
    Other = 99
}

public enum MaintenanceAssetStatus
{
    Active = 1,
    Inactive = 2,
    UnderMaintenance = 3,
    Retired = 4
}

public enum MaintenancePriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum MaintenanceWorkOrderStatus
{
    Draft = 1,
    Open = 2,
    Assigned = 3,
    InProgress = 4,
    OnHold = 5,
    PendingApproval = 6,
    Approved = 7,
    Rejected = 8,
    Completed = 9,
    Cancelled = 10
}

public enum MaintenanceCostApprovalStatus
{
    NotRequired = 1,
    Pending = 2,
    Approved = 3,
    Rejected = 4
}
