using SharedWithUI.Permissions;
using SharedWithUI.Procurement.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Procurement.Pages;

public static class ProcurementUiText
{
    public static ProcurementDocumentKind ParseKind(string? kind) =>
        (kind ?? string.Empty).ToLowerInvariant() switch
        {
            "purchase-requests" => ProcurementDocumentKind.PurchaseRequest,
            "requests-for-quotation" => ProcurementDocumentKind.RequestForQuotation,
            "supplier-quotations" => ProcurementDocumentKind.SupplierQuotation,
            "purchase-orders" => ProcurementDocumentKind.PurchaseOrder,
            "goods-receipts" => ProcurementDocumentKind.GoodsReceipt,
            "purchase-returns" => ProcurementDocumentKind.PurchaseReturn,
            "supplier-invoices" => ProcurementDocumentKind.SupplierInvoice,
            _ => ProcurementDocumentKind.PurchaseRequest
        };

    public static string Route(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => "purchase-requests",
            ProcurementDocumentKind.RequestForQuotation => "requests-for-quotation",
            ProcurementDocumentKind.SupplierQuotation => "supplier-quotations",
            ProcurementDocumentKind.PurchaseOrder => "purchase-orders",
            ProcurementDocumentKind.GoodsReceipt => "goods-receipts",
            ProcurementDocumentKind.PurchaseReturn => "purchase-returns",
            ProcurementDocumentKind.SupplierInvoice => "supplier-invoices",
            _ => "purchase-requests"
        };

    public static (string En, string Ar, string Icon, string Permission) Meta(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => ("Purchase Requests", "طلبات الشراء", "bi-card-checklist", PermissionList.PurchaseRequestPermissions.View),
            ProcurementDocumentKind.RequestForQuotation => ("Requests for Quotation", "طلبات عروض الأسعار", "bi-envelope-paper", PermissionList.RequestForQuotationPermissions.View),
            ProcurementDocumentKind.SupplierQuotation => ("Supplier Quotations", "عروض أسعار الموردين", "bi-file-earmark-text", PermissionList.SupplierQuotationPermissions.View),
            ProcurementDocumentKind.PurchaseOrder => ("Purchase Orders", "أوامر الشراء", "bi-bag-check", PermissionList.PurchaseOrderPermissions.View),
            ProcurementDocumentKind.GoodsReceipt => ("Goods Receipts", "استلام البضائع", "bi-box-arrow-in-down", PermissionList.GoodsReceiptPermissions.View),
            ProcurementDocumentKind.PurchaseReturn => ("Purchase Returns", "مرتجعات الشراء", "bi-arrow-return-left", PermissionList.PurchaseReturnPermissions.View),
            ProcurementDocumentKind.SupplierInvoice => ("Supplier Invoices", "فواتير الموردين", "bi-receipt", PermissionList.SupplierInvoicePermissions.View),
            _ => ("Procurement", "المشتريات", "bi-cart-check", PermissionList.PurchaseRequestPermissions.View)
        };

    public static string CreatePermission(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => PermissionList.PurchaseRequestPermissions.Create,
            ProcurementDocumentKind.RequestForQuotation => PermissionList.RequestForQuotationPermissions.Create,
            ProcurementDocumentKind.SupplierQuotation => PermissionList.SupplierQuotationPermissions.Create,
            ProcurementDocumentKind.PurchaseOrder => PermissionList.PurchaseOrderPermissions.Create,
            ProcurementDocumentKind.GoodsReceipt => PermissionList.GoodsReceiptPermissions.Create,
            ProcurementDocumentKind.PurchaseReturn => PermissionList.PurchaseReturnPermissions.Create,
            ProcurementDocumentKind.SupplierInvoice => PermissionList.SupplierInvoicePermissions.Create,
            _ => PermissionList.PurchaseRequestPermissions.Create
        };

    public static string EditPermission(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => PermissionList.PurchaseRequestPermissions.Edit,
            ProcurementDocumentKind.RequestForQuotation => PermissionList.RequestForQuotationPermissions.Edit,
            ProcurementDocumentKind.SupplierQuotation => PermissionList.SupplierQuotationPermissions.Edit,
            ProcurementDocumentKind.PurchaseOrder => PermissionList.PurchaseOrderPermissions.Edit,
            ProcurementDocumentKind.GoodsReceipt => PermissionList.GoodsReceiptPermissions.Edit,
            ProcurementDocumentKind.PurchaseReturn => PermissionList.PurchaseReturnPermissions.Edit,
            ProcurementDocumentKind.SupplierInvoice => PermissionList.SupplierInvoicePermissions.Edit,
            _ => PermissionList.PurchaseRequestPermissions.Edit
        };

    public static string DeletePermission(ProcurementDocumentKind kind) =>
        kind switch
        {
            ProcurementDocumentKind.PurchaseRequest => PermissionList.PurchaseRequestPermissions.Delete,
            ProcurementDocumentKind.RequestForQuotation => PermissionList.RequestForQuotationPermissions.Delete,
            ProcurementDocumentKind.SupplierQuotation => PermissionList.SupplierQuotationPermissions.Delete,
            ProcurementDocumentKind.PurchaseOrder => PermissionList.PurchaseOrderPermissions.Delete,
            ProcurementDocumentKind.GoodsReceipt => PermissionList.GoodsReceiptPermissions.Delete,
            ProcurementDocumentKind.PurchaseReturn => PermissionList.PurchaseReturnPermissions.Delete,
            ProcurementDocumentKind.SupplierInvoice => PermissionList.SupplierInvoicePermissions.Delete,
            _ => PermissionList.PurchaseRequestPermissions.Delete
        };

    public static string WorkflowPermission(ProcurementDocumentKind kind, string action) =>
        (kind, action.ToLowerInvariant()) switch
        {
            (ProcurementDocumentKind.PurchaseRequest, "submit") => PermissionList.PurchaseRequestPermissions.Submit,
            (ProcurementDocumentKind.PurchaseRequest, "approve") => PermissionList.PurchaseRequestPermissions.Approve,
            (ProcurementDocumentKind.PurchaseRequest, "reject") => PermissionList.PurchaseRequestPermissions.Reject,
            (ProcurementDocumentKind.PurchaseRequest, "cancel") => PermissionList.PurchaseRequestPermissions.Cancel,
            (ProcurementDocumentKind.PurchaseRequest, "convert") => PermissionList.PurchaseRequestPermissions.Close,
            (ProcurementDocumentKind.RequestForQuotation, "send") => PermissionList.RequestForQuotationPermissions.Submit,
            (ProcurementDocumentKind.RequestForQuotation, "close") => PermissionList.RequestForQuotationPermissions.Close,
            (ProcurementDocumentKind.RequestForQuotation, "cancel") => PermissionList.RequestForQuotationPermissions.Cancel,
            (ProcurementDocumentKind.SupplierQuotation, "accept") => PermissionList.SupplierQuotationPermissions.Approve,
            (ProcurementDocumentKind.SupplierQuotation, "reject") => PermissionList.SupplierQuotationPermissions.Reject,
            (ProcurementDocumentKind.PurchaseOrder, "approve") => PermissionList.PurchaseOrderPermissions.Approve,
            (ProcurementDocumentKind.PurchaseOrder, "send") => PermissionList.PurchaseOrderPermissions.Submit,
            (ProcurementDocumentKind.PurchaseOrder, "close") => PermissionList.PurchaseOrderPermissions.Close,
            (ProcurementDocumentKind.PurchaseOrder, "cancel") => PermissionList.PurchaseOrderPermissions.Cancel,
            (ProcurementDocumentKind.GoodsReceipt, "post") => PermissionList.GoodsReceiptPermissions.Receive,
            (ProcurementDocumentKind.GoodsReceipt, "cancel") => PermissionList.GoodsReceiptPermissions.Cancel,
            (ProcurementDocumentKind.PurchaseReturn, "post") => PermissionList.PurchaseReturnPermissions.Receive,
            (ProcurementDocumentKind.PurchaseReturn, "cancel") => PermissionList.PurchaseReturnPermissions.Cancel,
            (ProcurementDocumentKind.SupplierInvoice, "match") => PermissionList.SupplierInvoicePermissions.Approve,
            (ProcurementDocumentKind.SupplierInvoice, "post") => PermissionList.SupplierInvoicePermissions.Close,
            (ProcurementDocumentKind.SupplierInvoice, "cancel") => PermissionList.SupplierInvoicePermissions.Cancel,
            _ => Meta(kind).Permission
        };

    public static bool CanEditDocument(ProcurementDocumentKind kind, string? status) =>
        kind switch
        {
            ProcurementDocumentKind.SupplierQuotation => Is(status, SupplierQuotationStatus.Received.ToString()),
            ProcurementDocumentKind.PurchaseRequest => Is(status, PurchaseRequestStatus.Draft.ToString()),
            ProcurementDocumentKind.RequestForQuotation => Is(status, RequestForQuotationStatus.Draft.ToString()),
            ProcurementDocumentKind.PurchaseOrder => Is(status, PurchaseOrderStatus.Draft.ToString()),
            ProcurementDocumentKind.GoodsReceipt => Is(status, PostedDocumentStatus.Draft.ToString()),
            ProcurementDocumentKind.PurchaseReturn => Is(status, PostedDocumentStatus.Draft.ToString()),
            ProcurementDocumentKind.SupplierInvoice => Is(status, SupplierInvoiceStatus.Draft.ToString()),
            _ => false
        };

    public static bool CanDeleteDocument(ProcurementDocumentKind kind, string? status) =>
        CanEditDocument(kind, status);

    public static string LockedEditReason(ProcurementDocumentKind kind, string? status, SharedDataService sharedDataService) =>
        CanEditDocument(kind, status)
            ? sharedDataService.SelectViewLang("Edit", "تعديل")
            : sharedDataService.SelectViewLang("This document cannot be edited after it leaves its editable status.", "لا يمكن تعديل هذا المستند بعد مغادرة حالة التعديل.");

    public static string LockedDeleteReason(ProcurementDocumentKind kind, string? status, SharedDataService sharedDataService) =>
        CanDeleteDocument(kind, status)
            ? sharedDataService.SelectViewLang("Delete", "حذف")
            : sharedDataService.SelectViewLang("This document cannot be deleted after it leaves its editable status.", "لا يمكن حذف هذا المستند بعد مغادرة حالة التعديل.");

    private static bool Is(string? status, string expected) =>
        string.Equals(status, expected, StringComparison.OrdinalIgnoreCase);
}
