using AlAfkarERP.Shared.Utilities;
using SharedWithUI.Contracts.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Contracts;

public static class ContractLocalization
{
    public static string Text(this SharedDataService sharedDataService, string en, string ar)
        => sharedDataService.SelectViewLang(en, ar);

    public static string ContractStatusLabel(this SharedDataService sharedDataService, ContractStatus status) => status switch
    {
        ContractStatus.Draft => sharedDataService.Text("Draft", "مسودة"),
        ContractStatus.UnderReview => sharedDataService.Text("Under Review", "قيد المراجعة"),
        ContractStatus.Signed => sharedDataService.Text("Signed", "موقع"),
        ContractStatus.Active => sharedDataService.Text("Active", "نشط"),
        ContractStatus.Expired => sharedDataService.Text("Expired", "منتهي"),
        ContractStatus.PendingRenewalPayment => sharedDataService.Text("Pending Payment", "بانتظار الدفع"),
        ContractStatus.Terminated => sharedDataService.Text("Terminated", "منهى"),
        ContractStatus.Renewed => sharedDataService.Text("Renewed", "مجدد"),
        _ => status.ToString()
    };

    public static string TermUnitLabel(this SharedDataService sharedDataService, ContractTermUnit unit) => unit switch
    {
        ContractTermUnit.Days => sharedDataService.Text("Days", "أيام"),
        ContractTermUnit.Months => sharedDataService.Text("Months", "أشهر"),
        ContractTermUnit.Years => sharedDataService.Text("Years", "سنوات"),
        _ => unit.ToString()
    };

    public static string FeeModeLabel(this SharedDataService sharedDataService, ContractRenewalFeeMode mode) => mode switch
    {
        ContractRenewalFeeMode.FixedAmount => sharedDataService.Text("Fixed amount", "مبلغ ثابت"),
        ContractRenewalFeeMode.PercentageOfContractValue => sharedDataService.Text("Percent of value", "نسبة من قيمة العقد"),
        _ => mode.ToString()
    };

    public static string RenewalStatusLabel(this SharedDataService sharedDataService, ContractRenewalStatus status) => status switch
    {
        ContractRenewalStatus.Pending => sharedDataService.Text("Pending", "قيد الانتظار"),
        ContractRenewalStatus.Activated => sharedDataService.Text("Activated", "مفعل"),
        ContractRenewalStatus.PendingPayment => sharedDataService.Text("Pending Payment", "بانتظار الدفع"),
        ContractRenewalStatus.Cancelled => sharedDataService.Text("Cancelled", "ملغى"),
        _ => status.ToString()
    };

    public static string PaymentStatusLabel(this SharedDataService sharedDataService, ContractRenewalPaymentStatus status) => status switch
    {
        ContractRenewalPaymentStatus.NotRequired => sharedDataService.Text("Not Required", "غير مطلوب"),
        ContractRenewalPaymentStatus.Pending => sharedDataService.Text("Pending", "قيد الانتظار"),
        ContractRenewalPaymentStatus.Paid => sharedDataService.Text("Paid", "مدفوع"),
        ContractRenewalPaymentStatus.Waived => sharedDataService.Text("Waived", "معفى"),
        _ => status.ToString()
    };

    public static string AttachmentKindLabel(this SharedDataService sharedDataService, ContractAttachmentKind kind) => kind switch
    {
        ContractAttachmentKind.SignedContract => sharedDataService.Text("Signed Contract", "العقد الموقع"),
        ContractAttachmentKind.SupportingDocument => sharedDataService.Text("Supporting Document", "مستند داعم"),
        ContractAttachmentKind.TemplateOutput => sharedDataService.Text("Template Output", "مخرج القالب"),
        _ => kind.ToString()
    };

    public static string WorkflowActionLabel(this SharedDataService sharedDataService, string action) => action switch
    {
        "submit-review" => sharedDataService.Text("Submit review", "إرسال للمراجعة"),
        "sign" => sharedDataService.Text("Sign", "توقيع"),
        "activate" => sharedDataService.Text("Activate", "تفعيل"),
        "terminate" => sharedDataService.Text("Terminate", "إنهاء"),
        "renew" => sharedDataService.Text("Renew", "تجديد"),
        _ => action
    };

    public static string PartyTypeLabel(this SharedDataService sharedDataService, string? partyType) => partyType?.Trim().ToLowerInvariant() switch
    {
        "customer" => sharedDataService.Text("Customer", "عميل"),
        "employee" => sharedDataService.Text("Employee", "موظف"),
        "supplier" => sharedDataService.Text("Supplier", "مورد"),
        "vendor" => sharedDataService.Text("Vendor", "مورد"),
        "tenant" => sharedDataService.Text("Tenant", "مستأجر"),
        "owner" => sharedDataService.Text("Owner", "مالك"),
        _ => partyType ?? string.Empty
    };
}
