using System.ComponentModel.DataAnnotations;
using SharedWithUI.Contracts.Enums;

namespace SharedWithUI.Contracts.Dtos;

public class ContractDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? TitleEng { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }

    [Required]
    public string PartyType { get; set; } = string.Empty;

    public Guid PartyId { get; set; }
    public string PartyDisplayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddYears(1);
    public decimal ContractValue { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string? Notes { get; set; }
    public Guid? TemplateId { get; set; }
    public ContractRenewalSettingsDto RenewalSettings { get; set; } = new();
    public List<ContractRenewalDto> Renewals { get; set; } = [];
    public List<ContractAttachmentDto> Attachments { get; set; } = [];
    public List<ContractStatusHistoryDto> StatusHistory { get; set; } = [];
}

public class ContractRenewalSettingsDto
{
    public bool AutoRenew { get; set; }
    public int RenewalTermValue { get; set; } = 1;
    public ContractTermUnit RenewalTermUnit { get; set; } = ContractTermUnit.Years;
    public int RenewalNoticeDays { get; set; } = 30;
    public bool RequiresRenewalFee { get; set; }
    public ContractRenewalFeeMode FeeMode { get; set; } = ContractRenewalFeeMode.FixedAmount;
    public decimal? FeeAmount { get; set; }
    public decimal? FeePercentage { get; set; }
    public Guid? CurrencyId { get; set; }
    public int PaymentGraceDays { get; set; } = 0;
}

public class ContractRenewalDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public DateTime PreviousStartDate { get; set; }
    public DateTime PreviousEndDate { get; set; }
    public DateTime RenewedStartDate { get; set; }
    public DateTime RenewedEndDate { get; set; }
    public ContractRenewalStatus Status { get; set; }
    public bool FeeRequired { get; set; }
    public decimal FeeAmount { get; set; }
    public Guid? CurrencyId { get; set; }
    public ContractRenewalPaymentStatus PaymentStatus { get; set; }
    public Guid? AccountingDocumentId { get; set; }
    public string? AccountingDocumentNumber { get; set; }
    public Guid? PaymentReferenceId { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal? PaidAmount { get; set; }
}

public class ContractTemplateDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ContractAttachmentDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public ContractAttachmentKind Kind { get; set; }
    public DateTime UploadedDate { get; set; }
    public Guid UploadedByUserId { get; set; }
}

public class ContractStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public ContractStatus? OldStatus { get; set; }
    public ContractStatus NewStatus { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
}

public class ContractRenewalObligationDto
{
    public Guid ContractId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PartyType { get; set; } = string.Empty;
    public Guid PartyId { get; set; }
    public string PartyDisplayName { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public bool RequiresPayment { get; set; }
    public decimal FeeAmount { get; set; }
    public Guid? CurrencyId { get; set; }
}

public class CreateContractResponseDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
}
