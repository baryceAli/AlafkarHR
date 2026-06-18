namespace Contracts.Contracts.Models;

public class Contract : Aggregate<Guid>
{
    private readonly List<ContractRenewal> _renewals = [];
    private readonly List<ContractAttachment> _attachments = [];
    private readonly List<ContractStatusHistory> _statusHistory = [];

    private Contract()
    {
    }

    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? TitleEng { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public ContractStatus Status { get; private set; } = ContractStatus.Draft;
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public string PartyType { get; private set; } = string.Empty;
    public Guid PartyId { get; private set; }
    public string PartyDisplayName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal ContractValue { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public string? Notes { get; private set; }
    public Guid? TemplateId { get; private set; }
    public ContractRenewalSettings RenewalSettings { get; private set; } = ContractRenewalSettings.Default();
    public IReadOnlyCollection<ContractRenewal> Renewals => _renewals.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAt).ToList();
    public IReadOnlyCollection<ContractAttachment> Attachments => _attachments.Where(x => !x.IsDeleted).OrderByDescending(x => x.UploadedDate).ToList();
    public IReadOnlyCollection<ContractStatusHistory> StatusHistory => _statusHistory.OrderByDescending(x => x.ChangedAt).ToList();

    public static Contract Create(string number, ContractDto dto, string userId)
    {
        Validate(dto);
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            Number = number,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        contract.Apply(dto, userId);
        contract.AddHistory(null, ContractStatus.Draft, "create", null, userId);
        return contract;
    }

    public void Update(ContractDto dto, string userId)
    {
        if (Status is ContractStatus.Terminated or ContractStatus.Renewed)
            throw new BadRequestException("Closed contracts cannot be edited.");

        Apply(dto, userId);
    }

    public void ConfigureRenewal(ContractRenewalSettingsDto dto, string userId)
    {
        RenewalSettings = ContractRenewalSettings.FromDto(dto);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void ChangeStatus(ContractStatus newStatus, string action, string? notes, string userId)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        AddHistory(oldStatus, newStatus, action, notes, userId);
    }

    public ContractRenewal ProcessRenewal(string userId)
    {
        if (!RenewalSettings.AutoRenew)
            throw new BadRequestException("Auto renewal is not enabled for this contract.");

        var existingPending = Renewals.FirstOrDefault(x => x.Status is ContractRenewalStatus.PendingPayment or ContractRenewalStatus.Pending);
        if (existingPending is not null)
            return existingPending;

        var renewedStart = EndDate.Date.AddDays(1);
        var renewedEnd = RenewalSettings.AddTerm(renewedStart).AddDays(-1);
        var fee = RenewalSettings.CalculateFee(ContractValue);
        var renewal = ContractRenewal.Create(Id, StartDate, EndDate, renewedStart, renewedEnd, RenewalSettings.RequiresRenewalFee, fee, RenewalSettings.CurrencyId ?? CurrencyId, userId);
        _renewals.Add(renewal);

        if (RenewalSettings.RequiresRenewalFee)
        {
            ChangeStatus(ContractStatus.PendingRenewalPayment, "renewal-payment-required", "Renewal fee is required.", userId);
            return renewal;
        }

        ActivateRenewal(renewal, userId);
        return renewal;
    }

    public void ActivatePaidRenewal(Guid renewalId, Guid? paymentReferenceId, decimal paidAmount, string userId)
    {
        var renewal = Renewals.FirstOrDefault(x => x.Id == renewalId)
            ?? throw new NotFoundException($"Renewal not found: {renewalId}");
        renewal.RecordPayment(paymentReferenceId, paidAmount, userId);
        ActivateRenewal(renewal, userId);
    }

    public void AddAttachment(ContractAttachment attachment)
    {
        _attachments.Add(attachment);
    }

    public void RemoveAttachment(Guid attachmentId, string userId)
    {
        var attachment = Attachments.FirstOrDefault(x => x.Id == attachmentId)
            ?? throw new NotFoundException($"Attachment not found: {attachmentId}");
        attachment.Remove(userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ContractDto ToDto() => new()
    {
        Id = Id,
        Number = Number,
        Title = Title,
        TitleEng = TitleEng,
        Type = Type,
        Status = Status,
        CompanyId = CompanyId,
        BranchId = BranchId,
        DepartmentId = DepartmentId,
        PartyType = PartyType,
        PartyId = PartyId,
        PartyDisplayName = PartyDisplayName,
        StartDate = StartDate,
        EndDate = EndDate,
        ContractValue = ContractValue,
        CurrencyId = CurrencyId,
        OwnerUserId = OwnerUserId,
        Notes = Notes,
        TemplateId = TemplateId,
        RenewalSettings = RenewalSettings.ToDto(),
        Renewals = Renewals.Select(x => x.ToDto()).ToList(),
        Attachments = Attachments.Select(x => x.ToDto()).ToList(),
        StatusHistory = StatusHistory.Select(x => x.ToDto()).ToList()
    };

    private void Apply(ContractDto dto, string userId)
    {
        Validate(dto);
        Title = dto.Title.Trim();
        TitleEng = string.IsNullOrWhiteSpace(dto.TitleEng) ? null : dto.TitleEng.Trim();
        Type = dto.Type.Trim();
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        DepartmentId = dto.DepartmentId;
        PartyType = dto.PartyType.Trim();
        PartyId = dto.PartyId;
        PartyDisplayName = dto.PartyDisplayName.Trim();
        StartDate = dto.StartDate.Date;
        EndDate = dto.EndDate.Date;
        ContractValue = dto.ContractValue;
        CurrencyId = dto.CurrencyId;
        OwnerUserId = dto.OwnerUserId;
        Notes = dto.Notes;
        TemplateId = dto.TemplateId;
        RenewalSettings = ContractRenewalSettings.FromDto(dto.RenewalSettings);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void ActivateRenewal(ContractRenewal renewal, string userId)
    {
        StartDate = renewal.RenewedStartDate;
        EndDate = renewal.RenewedEndDate;
        renewal.Activate(userId);
        ChangeStatus(ContractStatus.Active, "renewal-activated", "Contract renewed.", userId);
    }

    private void AddHistory(ContractStatus? oldStatus, ContractStatus newStatus, string action, string? notes, string userId)
    {
        _statusHistory.Add(ContractStatusHistory.Create(Id, oldStatus, newStatus, action, notes, userId));
    }

    private static void Validate(ContractDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new BadRequestException("Contract title is required.");
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new BadRequestException("Contract type is required.");
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.PartyType))
            throw new BadRequestException("Party type is required.");
        if (dto.PartyId == Guid.Empty)
            throw new BadRequestException("Party is required.");
        if (string.IsNullOrWhiteSpace(dto.PartyDisplayName))
            throw new BadRequestException("Party display name is required.");
        if (dto.EndDate.Date < dto.StartDate.Date)
            throw new BadRequestException("Contract end date cannot be before start date.");
        if (dto.ContractValue < 0)
            throw new BadRequestException("Contract value cannot be negative.");
    }
}
