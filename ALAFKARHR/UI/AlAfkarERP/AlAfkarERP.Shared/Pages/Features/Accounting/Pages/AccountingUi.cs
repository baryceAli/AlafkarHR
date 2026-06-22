using SharedWithUI.Accounting.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Accounting.Pages;

public static class AccountingUi
{
    public static string DocumentTitle(AccountingDocumentType? type) => type switch
    {
        AccountingDocumentType.SalesInvoice => "Sales Invoices",
        AccountingDocumentType.SupplierInvoice => "Purchase Invoices",
        AccountingDocumentType.CustomerReceipt => "Receipts",
        AccountingDocumentType.SupplierPayment => "Payments",
        AccountingDocumentType.SalesCreditNote => "Credit Notes",
        AccountingDocumentType.SalesDebitNote => "Debit Notes",
        _ => "Accounting Documents"
    };

    public static string DocumentTitleAr(AccountingDocumentType? type) => type switch
    {
        AccountingDocumentType.SalesInvoice => "فواتير المبيعات",
        AccountingDocumentType.SupplierInvoice => "فواتير المشتريات",
        AccountingDocumentType.CustomerReceipt => "المقبوضات",
        AccountingDocumentType.SupplierPayment => "المدفوعات",
        AccountingDocumentType.SalesCreditNote => "الإشعارات الدائنة",
        AccountingDocumentType.SalesDebitNote => "الإشعارات المدينة",
        _ => "مستندات المحاسبة"
    };
}
