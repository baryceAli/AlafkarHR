namespace Procurement.Procurement.Models;

public class SupplierItem : Aggregate<Guid>
{
    private SupplierItem() { }

    public Guid CompanyId { get; private set; }
    public Guid SupplierId { get; private set; }
    public string? SupplierName { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string? ProductName { get; private set; }
    public string? ProductNameEng { get; private set; }
    public string? SkuCode { get; private set; }
    public string SupplierSku { get; private set; } = string.Empty;
    public int LeadTimeDays { get; private set; }
    public decimal MinimumOrderQuantity { get; private set; }
    public bool IsPreferred { get; private set; }
    public string? Notes { get; private set; }

    public static SupplierItem Create(SupplierItemDto dto, string userId)
    {
        var item = new SupplierItem { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        item.Apply(dto, userId);
        return item;
    }

    public void Update(SupplierItemDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public SupplierItemDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        SupplierId = SupplierId,
        SupplierName = SupplierName,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductName = ProductName,
        ProductNameEng = ProductNameEng,
        SkuCode = SkuCode,
        SupplierSku = SupplierSku,
        LeadTimeDays = LeadTimeDays,
        MinimumOrderQuantity = MinimumOrderQuantity,
        IsPreferred = IsPreferred,
        Notes = Notes
    };

    private void Apply(SupplierItemDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        SupplierId = dto.SupplierId;
        SupplierName = dto.SupplierName;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        ProductName = dto.ProductName;
        ProductNameEng = dto.ProductNameEng;
        SkuCode = dto.SkuCode;
        SupplierSku = dto.SupplierSku;
        LeadTimeDays = dto.LeadTimeDays;
        MinimumOrderQuantity = dto.MinimumOrderQuantity;
        IsPreferred = dto.IsPreferred;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class VendorPricelist : Aggregate<Guid>
{
    private VendorPricelist() { }

    public Guid CompanyId { get; private set; }
    public Guid SupplierId { get; private set; }
    public string? SupplierName { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal DiscountRate { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsPreferred { get; private set; }

    public static VendorPricelist Create(VendorPricelistDto dto, string userId)
    {
        var list = new VendorPricelist { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        list.Apply(dto, userId);
        return list;
    }

    public void Update(VendorPricelistDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public VendorPricelistDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        SupplierId = SupplierId,
        SupplierName = SupplierName,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        CurrencyId = CurrencyId,
        MinimumQuantity = MinimumQuantity,
        UnitCost = UnitCost,
        DiscountRate = DiscountRate,
        ValidFrom = ValidFrom,
        ValidTo = ValidTo,
        IsPreferred = IsPreferred
    };

    private void Apply(VendorPricelistDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        SupplierId = dto.SupplierId;
        SupplierName = dto.SupplierName;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        CurrencyId = dto.CurrencyId;
        MinimumQuantity = dto.MinimumQuantity;
        UnitCost = dto.UnitCost;
        DiscountRate = dto.DiscountRate;
        ValidFrom = dto.ValidFrom == default ? DateTime.UtcNow.Date : dto.ValidFrom;
        ValidTo = dto.ValidTo;
        IsPreferred = dto.IsPreferred;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class ReorderingRule : Aggregate<Guid>
{
    private ReorderingRule() { }

    public Guid CompanyId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal MaximumQuantity { get; private set; }
    public decimal ReorderQuantity { get; private set; }
    public decimal MultipleQuantity { get; private set; }
    public int LeadTimeDays { get; private set; }
    public int HorizonDays { get; private set; }
    public ReplenishmentTriggerMode TriggerMode { get; private set; } = ReplenishmentTriggerMode.Manual;
    public bool AutoCreatePurchaseRequest { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastRunAt { get; private set; }
    public Guid? LastGeneratedDocumentId { get; private set; }
    public string? LastGeneratedDocumentNumber { get; private set; }
    public DateTime? LastGeneratedAt { get; private set; }

    public static ReorderingRule Create(ReorderingRuleDto dto, string userId)
    {
        var rule = new ReorderingRule { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        rule.Apply(dto, userId);
        return rule;
    }

    public void Update(ReorderingRuleDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ReorderingRuleDto ToDto()
    {
        var triggerMode = ResolveTriggerMode();
        return new ReorderingRuleDto
        {
            Id = Id,
            CompanyId = CompanyId,
            ProductId = ProductId,
            ProductSkuId = ProductSkuId,
            WarehouseId = WarehouseId,
            SupplierId = SupplierId,
            MinimumQuantity = MinimumQuantity,
            MaximumQuantity = MaximumQuantity,
            ReorderQuantity = ReorderQuantity,
            MultipleQuantity = MultipleQuantity,
            LeadTimeDays = LeadTimeDays,
            HorizonDays = HorizonDays,
            TriggerMode = triggerMode,
            AutoCreatePurchaseRequest = triggerMode == ReplenishmentTriggerMode.Automatic,
            IsActive = IsActive,
            LastRunAt = LastRunAt,
            LastGeneratedDocumentId = LastGeneratedDocumentId,
            LastGeneratedDocumentNumber = LastGeneratedDocumentNumber,
            LastGeneratedAt = LastGeneratedAt
        };
    }

    public ReplenishmentTriggerMode ResolveTriggerMode() =>
        AutoCreatePurchaseRequest
            ? ReplenishmentTriggerMode.Automatic
            : TriggerMode == default ? ReplenishmentTriggerMode.Manual : TriggerMode;

    public void MarkReplenishmentRun(Guid? documentId, string? documentNumber, string userId)
    {
        LastRunAt = DateTime.UtcNow;
        if (documentId.HasValue)
        {
            LastGeneratedDocumentId = documentId;
            LastGeneratedDocumentNumber = documentNumber;
            LastGeneratedAt = DateTime.UtcNow;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void Apply(ReorderingRuleDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        WarehouseId = dto.WarehouseId;
        SupplierId = dto.SupplierId;
        MinimumQuantity = dto.MinimumQuantity;
        MaximumQuantity = dto.MaximumQuantity;
        ReorderQuantity = dto.ReorderQuantity;
        MultipleQuantity = dto.MultipleQuantity;
        LeadTimeDays = dto.LeadTimeDays;
        HorizonDays = dto.HorizonDays;
        TriggerMode = dto.AutoCreatePurchaseRequest
            ? ReplenishmentTriggerMode.Automatic
            : dto.TriggerMode == default ? ReplenishmentTriggerMode.Manual : dto.TriggerMode;
        AutoCreatePurchaseRequest = TriggerMode == ReplenishmentTriggerMode.Automatic;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class ProcurementAgreement : Aggregate<Guid>
{
    private readonly List<ProcurementAgreementLine> _lines = [];

    private ProcurementAgreement() { }

    public ProcurementAgreementType Type { get; private set; }
    public ProcurementAgreementStatus Status { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string? SupplierName { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public DateTime AgreementDate { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ProcurementAgreementLine> Lines => _lines;

    public static ProcurementAgreement Create(ProcurementAgreementDto dto, string userId)
    {
        var agreement = new ProcurementAgreement
        {
            Id = Guid.NewGuid(),
            Status = ProcurementAgreementStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        agreement.Apply(dto, userId);
        return agreement;
    }

    public void Update(ProcurementAgreementDto dto, string userId)
    {
        if (Status is ProcurementAgreementStatus.Closed or ProcurementAgreementStatus.Cancelled)
            throw new BadRequestException("Closed or cancelled purchase agreements cannot be edited.");

        Apply(dto, userId);
    }

    public void Confirm(string userId) => ChangeStatus(ProcurementAgreementStatus.Confirmed, userId);
    public void Close(string userId) => ChangeStatus(ProcurementAgreementStatus.Closed, userId);
    public void Cancel(string userId) => ChangeStatus(ProcurementAgreementStatus.Cancelled, userId);

    public void Remove(string userId)
    {
        if (Status is ProcurementAgreementStatus.Confirmed)
            throw new BadRequestException("Confirmed purchase agreements must be closed or cancelled before deletion.");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ProcurementAgreementDto ToDto() => new()
    {
        Id = Id,
        Type = Type,
        Status = Status,
        CompanyId = CompanyId,
        BranchId = BranchId,
        SupplierId = SupplierId,
        SupplierName = SupplierName,
        Name = Name,
        Reference = Reference,
        CurrencyId = CurrencyId,
        AgreementDate = AgreementDate,
        ValidUntil = ValidUntil,
        Notes = Notes,
        Lines = _lines.OrderBy(x => x.LineNumber).Select(x => x.ToDto()).ToList()
    };

    private void Apply(ProcurementAgreementDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        Type = dto.Type;
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        SupplierId = dto.SupplierId;
        SupplierName = dto.SupplierName;
        Name = dto.Name.Trim();
        Reference = dto.Reference;
        CurrencyId = dto.CurrencyId;
        AgreementDate = dto.AgreementDate == default ? DateTime.UtcNow.Date : dto.AgreementDate;
        ValidUntil = dto.ValidUntil;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;

        ReplaceLines(dto.Lines, userId);
    }

    private void ReplaceLines(List<ProcurementAgreementLineDto> lines, string userId)
    {
        _lines.Clear();

        var lineNumber = 1;
        foreach (var line in lines)
            _lines.Add(ProcurementAgreementLine.Create(lineNumber++, line, userId));
    }

    private void ChangeStatus(ProcurementAgreementStatus status, string userId)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class ProcurementAgreementLine : Entity<Guid>
{
    private ProcurementAgreementLine() { }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid? UnitOfMeasureId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }

    public static ProcurementAgreementLine Create(int lineNumber, ProcurementAgreementLineDto dto, string userId) => new()
    {
        Id = Guid.NewGuid(),
        LineNumber = lineNumber,
        ProductId = dto.ProductId,
        ProductSkuId = dto.ProductSkuId,
        ProductName = dto.ProductName,
        ProductNameEng = dto.ProductNameEng,
        SkuCode = dto.SkuCode,
        UnitOfMeasureId = dto.UnitOfMeasureId,
        Quantity = dto.Quantity,
        UnitCost = dto.UnitCost,
        DiscountRate = dto.DiscountRate,
        TaxRate = dto.TaxRate,
        Notes = dto.Notes,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    public ProcurementAgreementLineDto ToDto() => new()
    {
        Id = Id,
        LineNumber = LineNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductName = ProductName,
        ProductNameEng = ProductNameEng,
        SkuCode = SkuCode,
        UnitOfMeasureId = UnitOfMeasureId,
        Quantity = Quantity,
        UnitCost = UnitCost,
        DiscountRate = DiscountRate,
        TaxRate = TaxRate,
        Notes = Notes
    };
}
