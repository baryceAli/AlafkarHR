namespace SharedWithUI.Catering.Enums;

public enum CateringServiceType
{
    Ramadan = 1,
    General = 2
}

public enum CateringContractStatus
{
    Draft = 0,
    Active = 1,
    Closed = 2,
    Cancelled = 3
}

public enum CateringMealType
{
    Dry = 1,
    Hot = 2,
    Cold = 3,
    Other = 99
}

public enum CateringMealStructureType
{
    Product = 1,
    Combo = 2
}

public enum CateringAssignmentRole
{
    DistributionTeam = 1,
    Supervisor = 2,
    Photographer = 3
}

public enum CateringPlanStatus
{
    Draft = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3
}

public enum CateringProjectStatus
{
    Draft = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3
}

public enum CateringProjectDailyPlanStatus
{
    Draft = 0,
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

public enum CateringPlanResourceType
{
    Truck = 1,
    Driver = 2,
    Supervisor = 3,
    DistributionTeamMember = 4,
    DistributionTeamLeader = 5,
    PackagingResponsible = 6,
    DispatchResponsible = 7
}

public enum CateringInventoryRequestStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Fulfilled = 3,
    Rejected = 4,
    Cancelled = 5
}

public enum CateringScheduleStatus
{
    Draft = 0,
    Planned = 1,
    Packaging = 2,
    ReadyForDispatch = 3,
    InTransit = 4,
    AtDistributionArea = 5,
    Completed = 6,
    Cancelled = 7
}

public enum CateringPackagingStatus
{
    NotRequired = 0,
    Planned = 1,
    StockReleased = 2,
    InProgress = 3,
    Completed = 4,
    Exception = 5
}

public enum CateringDispatchStatus
{
    Planned = 0,
    VehicleAssigned = 1,
    ArrivedForLoading = 2,
    Loaded = 3,
    Departed = 4,
    ArrivedAtDistribution = 5,
    Completed = 6,
    Cancelled = 7
}

public enum CateringExecutionEventType
{
    StockReleased = 1,
    PackagingStarted = 2,
    PackagingCompleted = 3,
    TruckArrivedForLoading = 4,
    TruckLoaded = 5,
    TruckDeparted = 6,
    TruckArrivedAtDistribution = 7,
    SupervisorReceived = 8,
    BlockDelivered = 9,
    ExceptionRecorded = 99
}
