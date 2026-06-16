namespace SharedWithUI.Procurement.Enums;

public enum ProcurementDocumentKind
{
    PurchaseRequest = 1,
    RequestForQuotation = 2,
    SupplierQuotation = 3,
    PurchaseOrder = 4,
    GoodsReceipt = 5,
    PurchaseReturn = 6,
    SupplierInvoice = 7
}

public enum PurchaseRequestStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5,
    Converted = 6
}

public enum RequestForQuotationStatus
{
    Draft = 1,
    Sent = 2,
    Closed = 3,
    Cancelled = 4
}

public enum SupplierQuotationStatus
{
    Received = 1,
    Accepted = 2,
    Rejected = 3
}

public enum PurchaseOrderStatus
{
    Draft = 1,
    Approved = 2,
    Sent = 3,
    PartiallyReceived = 4,
    Received = 5,
    Closed = 6,
    Cancelled = 7
}

public enum PostedDocumentStatus
{
    Draft = 1,
    Posted = 2,
    Cancelled = 3
}

public enum SupplierInvoiceStatus
{
    Draft = 1,
    Matched = 2,
    Posted = 3,
    Cancelled = 4
}
