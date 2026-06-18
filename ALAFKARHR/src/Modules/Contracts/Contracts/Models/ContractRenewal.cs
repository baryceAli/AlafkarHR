namespace Contracts.Contracts.Models;

public class ContractRenewal : Entity<Guid>
{
    private ContractRenewal()
    {
    }

    public Guid ContractId { get; private set; }
    public DateTime PreviousStartDate { get; private set; }
    public DateTime PreviousEndDate { get; private set; }
    public DateTime RenewedStartDate { get; private set; }
    public DateTime RenewedEndDate { get; private set; }
    public ContractRenewalStatus Status { get; private set; }
    public bool FeeRequired { get; private set; }
    public decimal FeeAmount { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public ContractRenewalPaymentStatus PaymentStatus { get; private set; }
    public Guid? AccountingDocumentId { get; private set; }
    public string? AccountingDocumentNumber { get; private set; }
    public Guid? PaymentReferenceId { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public decimal? PaidAmount { get; private set; }

    public static ContractRenewal Create(Guid contractId, DateTime previousStartDate, DateTime previousEndDate, DateTime renewedStartDate, DateTime renewedEndDate, bool feeRequired, decimal feeAmount, Guid? currencyId, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            ContractId = contractId,
            PreviousStartDate = previousStartDate,
            PreviousEndDate = previousEndDate,
            RenewedStartDate = renewedStartDate,
            RenewedEndDate = renewedEndDate,
            Status = feeRequired ? ContractRenewalStatus.PendingPayment : ContractRenewalStatus.Pending,
            FeeRequired = feeRequired,
            FeeAmount = feeAmount,
            CurrencyId = currencyId,
            PaymentStatus = feeRequired ? ContractRenewalPaymentStatus.Pending : ContractRenewalPaymentStatus.NotRequired,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void RecordPayment(Guid? paymentReferenceId, decimal paidAmount, string userId)
    {
        if (!FeeRequired)
            throw new BadRequestException("This renewal does not require payment.");
        if (paidAmount <= 0)
            throw new BadRequestException("Paid amount must be greater than zero.");
        if (paidAmount < FeeAmount)
            throw new BadRequestException("Paid amount cannot be less than the renewal fee.");

        PaymentReferenceId = paymentReferenceId;
        PaidAmount = paidAmount;
        PaidAt = DateTime.UtcNow;
        PaymentStatus = ContractRenewalPaymentStatus.Paid;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Activate(string userId)
    {
        if (FeeRequired && PaymentStatus != ContractRenewalPaymentStatus.Paid)
            throw new BadRequestException("Renewal payment must be recorded before activation.");

        Status = ContractRenewalStatus.Activated;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public ContractRenewalDto ToDto() => new()
    {
        Id = Id,
        ContractId = ContractId,
        PreviousStartDate = PreviousStartDate,
        PreviousEndDate = PreviousEndDate,
        RenewedStartDate = RenewedStartDate,
        RenewedEndDate = RenewedEndDate,
        Status = Status,
        FeeRequired = FeeRequired,
        FeeAmount = FeeAmount,
        CurrencyId = CurrencyId,
        PaymentStatus = PaymentStatus,
        AccountingDocumentId = AccountingDocumentId,
        AccountingDocumentNumber = AccountingDocumentNumber,
        PaymentReferenceId = PaymentReferenceId,
        PaidAt = PaidAt,
        PaidAmount = PaidAmount
    };
}
