namespace SharedWithUI.SalesOrder.Enums;

public enum SalesOrderStatus
{
    Draft = 1,
    PartiallyReserved = 2,
    Reserved = 3,
    PartiallyDelivered = 4,
    Delivered = 5,
    PartiallyInvoiced = 6,
    Invoiced = 7,
    Completed = 8,

    //Alternative:
    Cancelled = 9,
    Rejected = 10,
    Confirmed = 11,
    //PendingApproval
}
