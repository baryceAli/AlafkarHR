namespace Contracts.Contracts.Models;

public class ContractRenewalSettings
{
    private ContractRenewalSettings()
    {
    }

    public bool AutoRenew { get; private set; }
    public int RenewalTermValue { get; private set; }
    public ContractTermUnit RenewalTermUnit { get; private set; }
    public int RenewalNoticeDays { get; private set; }
    public bool RequiresRenewalFee { get; private set; }
    public ContractRenewalFeeMode FeeMode { get; private set; }
    public decimal? FeeAmount { get; private set; }
    public decimal? FeePercentage { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public int PaymentGraceDays { get; private set; }

    public static ContractRenewalSettings Default() => FromDto(new ContractRenewalSettingsDto());

    public static ContractRenewalSettings FromDto(ContractRenewalSettingsDto dto)
    {
        if (dto.RenewalTermValue <= 0)
            throw new BadRequestException("Renewal term must be greater than zero.");
        if (dto.RenewalNoticeDays < 0)
            throw new BadRequestException("Renewal notice days cannot be negative.");
        if (dto.PaymentGraceDays < 0)
            throw new BadRequestException("Payment grace days cannot be negative.");
        if (dto.RequiresRenewalFee && dto.FeeMode == ContractRenewalFeeMode.FixedAmount && (!dto.FeeAmount.HasValue || dto.FeeAmount <= 0))
            throw new BadRequestException("Fixed renewal fee amount must be greater than zero.");
        if (dto.RequiresRenewalFee && dto.FeeMode == ContractRenewalFeeMode.PercentageOfContractValue && (!dto.FeePercentage.HasValue || dto.FeePercentage <= 0))
            throw new BadRequestException("Renewal fee percentage must be greater than zero.");

        return new ContractRenewalSettings
        {
            AutoRenew = dto.AutoRenew,
            RenewalTermValue = dto.RenewalTermValue,
            RenewalTermUnit = dto.RenewalTermUnit,
            RenewalNoticeDays = dto.RenewalNoticeDays,
            RequiresRenewalFee = dto.RequiresRenewalFee,
            FeeMode = dto.FeeMode,
            FeeAmount = dto.RequiresRenewalFee && dto.FeeMode == ContractRenewalFeeMode.FixedAmount ? dto.FeeAmount : null,
            FeePercentage = dto.RequiresRenewalFee && dto.FeeMode == ContractRenewalFeeMode.PercentageOfContractValue ? dto.FeePercentage : null,
            CurrencyId = dto.CurrencyId,
            PaymentGraceDays = dto.PaymentGraceDays
        };
    }

    public DateTime AddTerm(DateTime date) =>
        RenewalTermUnit switch
        {
            ContractTermUnit.Days => date.AddDays(RenewalTermValue),
            ContractTermUnit.Months => date.AddMonths(RenewalTermValue),
            ContractTermUnit.Years => date.AddYears(RenewalTermValue),
            _ => date.AddYears(1)
        };

    public decimal CalculateFee(decimal contractValue) =>
        !RequiresRenewalFee
            ? 0
            : FeeMode == ContractRenewalFeeMode.FixedAmount
                ? FeeAmount.GetValueOrDefault()
                : Math.Round(contractValue * FeePercentage.GetValueOrDefault() / 100m, 2);

    public ContractRenewalSettingsDto ToDto() => new()
    {
        AutoRenew = AutoRenew,
        RenewalTermValue = RenewalTermValue,
        RenewalTermUnit = RenewalTermUnit,
        RenewalNoticeDays = RenewalNoticeDays,
        RequiresRenewalFee = RequiresRenewalFee,
        FeeMode = FeeMode,
        FeeAmount = FeeAmount,
        FeePercentage = FeePercentage,
        CurrencyId = CurrencyId,
        PaymentGraceDays = PaymentGraceDays
    };
}
