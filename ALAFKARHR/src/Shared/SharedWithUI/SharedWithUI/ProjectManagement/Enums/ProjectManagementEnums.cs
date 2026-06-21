namespace SharedWithUI.ProjectManagement.Enums;

public enum ProjectStatus
{
    Draft = 0,
    Planned = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5
}

public enum ProjectType
{
    General = 0,
    FoodPreparationDistribution = 1
}

public enum MealHandlingType
{
    Other = 0,
    Hot = 1,
    Dry = 2,
    Chilled = 3
}

public enum DistributionStatus
{
    Draft = 0,
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

public enum ProjectResourceType
{
    Employee = 0,
    Team = 1,
    Equipment = 2,
    Vehicle = 3,
    Warehouse = 4,
    Supplier = 5,
    ExternalService = 6,
    Other = 7
}

public enum ProjectExpenseCategory
{
    Materials = 0,
    Labor = 1,
    Transport = 2,
    Packaging = 3,
    Cooling = 4,
    Fuel = 5,
    Subcontractor = 6,
    Other = 7
}

public enum ProjectHandoffType
{
    MaterialIssueToPreparation = 0,
    PreparedGoodsReceipt = 1,
    TransferToShipmentArea = 2,
    ShipmentToDistribution = 3,
    ReturnOrAdjustment = 4
}

public enum ProjectReportGroupBy
{
    Day = 0,
    Week = 1,
    Month = 2,
    Range = 3
}
