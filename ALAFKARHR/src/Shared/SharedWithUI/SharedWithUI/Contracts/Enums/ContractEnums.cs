namespace SharedWithUI.Contracts.Enums;

public enum ContractStatus
{
    Draft = 0,
    UnderReview = 1,
    Signed = 2,
    Active = 3,
    Expired = 4,
    PendingRenewalPayment = 5,
    Terminated = 6,
    Renewed = 7
}

public enum ContractTermUnit
{
    Days = 0,
    Months = 1,
    Years = 2
}

public enum ContractRenewalFeeMode
{
    FixedAmount = 0,
    PercentageOfContractValue = 1
}

public enum ContractRenewalStatus
{
    Pending = 0,
    Activated = 1,
    PendingPayment = 2,
    Cancelled = 3
}

public enum ContractRenewalPaymentStatus
{
    NotRequired = 0,
    Pending = 1,
    Paid = 2,
    Waived = 3
}

public enum ContractAttachmentKind
{
    SignedContract = 0,
    SupportingDocument = 1,
    TemplateOutput = 2
}
