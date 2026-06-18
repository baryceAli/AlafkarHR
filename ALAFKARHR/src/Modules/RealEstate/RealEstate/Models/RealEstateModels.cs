namespace RealEstate.Models;

public class Property : Aggregate<Guid>
{
    private readonly List<PropertyUnit> _units = [];

    private Property() { }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? ResponsibleUserId { get; private set; }
    public PropertyStatus Status { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<PropertyUnit> Units => _units.Where(x => !x.IsDeleted).ToList();

    public static Property Create(PropertyDto dto, string userId)
    {
        Validate(dto);
        var property = new Property
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        property.Apply(dto, userId);
        return property;
    }

    public void Update(PropertyDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public PropertyDto ToDto() => new()
    {
        Id = Id,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        CompanyId = CompanyId,
        BranchId = BranchId,
        DepartmentId = DepartmentId,
        ResponsibleUserId = ResponsibleUserId,
        Status = Status,
        Address = Address,
        City = City,
        District = District,
        Notes = Notes,
        UnitsCount = Units.Count
    };

    private void Apply(PropertyDto dto, string userId)
    {
        Code = string.IsNullOrWhiteSpace(dto.Code) ? Code : dto.Code.Trim();
        Name = dto.Name.Trim();
        NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim();
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        DepartmentId = dto.DepartmentId;
        ResponsibleUserId = dto.ResponsibleUserId;
        Status = dto.Status;
        Address = dto.Address?.Trim();
        City = dto.City?.Trim();
        District = dto.District?.Trim();
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(PropertyDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Property name is required.");
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
    }
}

public class PropertyUnit : Aggregate<Guid>
{
    private PropertyUnit() { }

    public Guid PropertyId { get; private set; }
    public string UnitNumber { get; private set; } = string.Empty;
    public string? Name { get; private set; }
    public PropertyUnitType UnitType { get; private set; }
    public UnitStatus Status { get; private set; }
    public string? Floor { get; private set; }
    public decimal? Area { get; private set; }
    public int? Bedrooms { get; private set; }
    public int? Bathrooms { get; private set; }
    public string? Notes { get; private set; }
    public Property Property { get; private set; } = default!;

    public static PropertyUnit Create(PropertyUnitDto dto, string userId)
    {
        Validate(dto);
        var unit = new PropertyUnit
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        unit.Apply(dto, userId);
        return unit;
    }

    public void Update(PropertyUnitDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void ChangeStatus(UnitStatus status, string userId)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public PropertyUnitDto ToDto() => new()
    {
        Id = Id,
        PropertyId = PropertyId,
        PropertyName = Property?.Name ?? string.Empty,
        UnitNumber = UnitNumber,
        Name = Name,
        UnitType = UnitType,
        Status = Status,
        Floor = Floor,
        Area = Area,
        Bedrooms = Bedrooms,
        Bathrooms = Bathrooms,
        Notes = Notes
    };

    private void Apply(PropertyUnitDto dto, string userId)
    {
        PropertyId = dto.PropertyId;
        UnitNumber = dto.UnitNumber.Trim();
        Name = dto.Name?.Trim();
        UnitType = dto.UnitType;
        Status = dto.Status;
        Floor = dto.Floor?.Trim();
        Area = dto.Area;
        Bedrooms = dto.Bedrooms;
        Bathrooms = dto.Bathrooms;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(PropertyUnitDto dto)
    {
        if (dto.PropertyId == Guid.Empty)
            throw new BadRequestException("Property is required.");
        if (string.IsNullOrWhiteSpace(dto.UnitNumber))
            throw new BadRequestException("Unit number is required.");
    }
}

public class Lease : Aggregate<Guid>
{
    private readonly List<LeaseInstallment> _installments = [];

    private Lease() { }

    public string Number { get; private set; } = string.Empty;
    public LeaseDirection Direction { get; private set; }
    public LeaseStatus Status { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? UnitId { get; private set; }
    public Guid ContractId { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public string PartyType { get; private set; } = string.Empty;
    public Guid PartyId { get; private set; }
    public string PartyDisplayName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal RentAmount { get; private set; }
    public decimal DepositAmount { get; private set; }
    public BillingFrequency BillingFrequency { get; private set; }
    public int DueDay { get; private set; }
    public int CustomIntervalMonths { get; private set; }
    public int GraceDays { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public string? Notes { get; private set; }
    public Property Property { get; private set; } = default!;
    public PropertyUnit? Unit { get; private set; }
    public IReadOnlyCollection<LeaseInstallment> Installments => _installments.Where(x => !x.IsDeleted).OrderBy(x => x.Sequence).ToList();

    public static Lease Create(string number, LeaseDto dto, string userId)
    {
        Validate(dto);
        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            Number = string.IsNullOrWhiteSpace(number) ? $"LEA-{DateTime.UtcNow:yyyyMMddHHmmss}" : number,
            Status = LeaseStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        lease.Apply(dto, userId);
        return lease;
    }

    public void Update(LeaseDto dto, string userId)
    {
        if (Status != LeaseStatus.Draft)
            throw new BadRequestException("Only draft leases can be edited.");
        Validate(dto);
        Apply(dto, userId);
    }

    public void GenerateInstallments(string userId)
    {
        if (Status != LeaseStatus.Draft)
            throw new BadRequestException("Installments can only be regenerated for draft leases.");

        foreach (var installment in _installments.Where(x => !x.IsDeleted))
            installment.Cancel(userId);

        var sequence = 1;
        var periodStart = StartDate.Date;
        while (periodStart <= EndDate.Date)
        {
            var periodEnd = GetNextPeriodStart(periodStart).AddDays(-1);
            if (periodEnd > EndDate.Date)
                periodEnd = EndDate.Date;

            var dueDate = ResolveDueDate(periodStart);
            _installments.Add(LeaseInstallment.Create(Id, sequence++, dueDate, periodStart, periodEnd, RentAmount, userId));

            if (BillingFrequency == BillingFrequency.OneTime)
                break;

            periodStart = periodEnd.AddDays(1);
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Activate(string userId)
    {
        if (!Installments.Any())
            GenerateInstallments(userId);
        Status = LeaseStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Suspend(string userId)
    {
        Status = LeaseStatus.Suspended;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Terminate(string userId)
    {
        Status = LeaseStatus.Terminated;
        foreach (var installment in Installments.Where(x => x.Status is InstallmentStatus.Pending or InstallmentStatus.Overdue))
            installment.Cancel(userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public LeaseDto ToDto() => new()
    {
        Id = Id,
        Number = Number,
        Direction = Direction,
        Status = Status,
        CompanyId = CompanyId,
        BranchId = BranchId,
        DepartmentId = DepartmentId,
        PropertyId = PropertyId,
        PropertyName = Property?.Name ?? string.Empty,
        UnitId = UnitId,
        UnitNumber = Unit?.UnitNumber,
        ContractId = ContractId,
        ContractNumber = ContractNumber,
        PartyType = PartyType,
        PartyId = PartyId,
        PartyDisplayName = PartyDisplayName,
        StartDate = StartDate,
        EndDate = EndDate,
        RentAmount = RentAmount,
        DepositAmount = DepositAmount,
        BillingFrequency = BillingFrequency,
        DueDay = DueDay,
        CustomIntervalMonths = CustomIntervalMonths,
        GraceDays = GraceDays,
        CurrencyId = CurrencyId,
        Notes = Notes,
        Installments = Installments.Select(x => x.ToDto()).ToList()
    };

    private void Apply(LeaseDto dto, string userId)
    {
        Direction = dto.Direction;
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        DepartmentId = dto.DepartmentId;
        PropertyId = dto.PropertyId;
        UnitId = dto.UnitId;
        ContractId = dto.ContractId;
        ContractNumber = dto.ContractNumber?.Trim() ?? string.Empty;
        PartyType = dto.PartyType.Trim();
        PartyId = dto.PartyId;
        PartyDisplayName = dto.PartyDisplayName.Trim();
        StartDate = dto.StartDate.Date;
        EndDate = dto.EndDate.Date;
        RentAmount = dto.RentAmount;
        DepositAmount = dto.DepositAmount;
        BillingFrequency = dto.BillingFrequency;
        DueDay = Math.Clamp(dto.DueDay, 1, 28);
        CustomIntervalMonths = dto.CustomIntervalMonths < 1 ? 1 : dto.CustomIntervalMonths;
        GraceDays = Math.Max(0, dto.GraceDays);
        CurrencyId = dto.CurrencyId;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private DateTime GetNextPeriodStart(DateTime periodStart) =>
        BillingFrequency switch
        {
            BillingFrequency.Monthly => periodStart.AddMonths(1),
            BillingFrequency.Quarterly => periodStart.AddMonths(3),
            BillingFrequency.SemiAnnual => periodStart.AddMonths(6),
            BillingFrequency.Annual => periodStart.AddYears(1),
            BillingFrequency.Custom => periodStart.AddMonths(CustomIntervalMonths),
            _ => EndDate.AddDays(1)
        };

    private DateTime ResolveDueDate(DateTime periodStart)
    {
        var day = Math.Min(DueDay, DateTime.DaysInMonth(periodStart.Year, periodStart.Month));
        return new DateTime(periodStart.Year, periodStart.Month, day);
    }

    private static void Validate(LeaseDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (dto.PropertyId == Guid.Empty)
            throw new BadRequestException("Property is required.");
        if (dto.ContractId == Guid.Empty)
            throw new BadRequestException("Contract is required.");
        if (dto.Direction == LeaseDirection.CompanyToTenant && !dto.UnitId.HasValue)
            throw new BadRequestException("Tenant lease requires a unit.");
        if (dto.PartyId == Guid.Empty || string.IsNullOrWhiteSpace(dto.PartyType))
            throw new BadRequestException("Lease party is required.");
        if (dto.EndDate.Date < dto.StartDate.Date)
            throw new BadRequestException("Lease end date cannot be before start date.");
        if (dto.RentAmount <= 0)
            throw new BadRequestException("Rent amount must be greater than zero.");
    }
}

public class LeaseInstallment : Entity<Guid>
{
    private readonly List<LeasePaymentAllocation> _allocations = [];

    private LeaseInstallment() { }

    public Guid LeaseId { get; private set; }
    public int Sequence { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public InstallmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<LeasePaymentAllocation> Allocations => _allocations.Where(x => !x.IsDeleted).ToList();

    public static LeaseInstallment Create(Guid leaseId, int sequence, DateTime dueDate, DateTime periodStart, DateTime periodEnd, decimal amount, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            LeaseId = leaseId,
            Sequence = sequence,
            DueDate = dueDate,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Amount = amount,
            Status = InstallmentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void AllocatePayment(Guid? paymentReferenceId, DateTime paymentDate, string? reference, decimal amount, string? notes, string userId)
    {
        if (Status is InstallmentStatus.Cancelled or InstallmentStatus.Waived or InstallmentStatus.Paid)
            throw new BadRequestException("Installment cannot receive payment.");
        if (amount <= 0 || amount > RemainingAmount)
            throw new BadRequestException("Payment allocation amount is invalid.");

        _allocations.Add(LeasePaymentAllocation.Create(Id, paymentReferenceId, paymentDate, reference, amount, notes, userId));
        PaidAmount += amount;
        Status = PaidAmount >= Amount ? InstallmentStatus.Paid : InstallmentStatus.PartiallyPaid;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void MarkOverdue(string userId)
    {
        if (Status == InstallmentStatus.Pending && DueDate.Date < DateTime.UtcNow.Date)
        {
            Status = InstallmentStatus.Overdue;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = userId;
        }
    }

    public void Cancel(string userId)
    {
        if (Status is InstallmentStatus.Paid or InstallmentStatus.PartiallyPaid)
            return;
        Status = InstallmentStatus.Cancelled;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public decimal RemainingAmount => Amount - PaidAmount;

    public LeaseInstallmentDto ToDto() => new()
    {
        Id = Id,
        LeaseId = LeaseId,
        Sequence = Sequence,
        DueDate = DueDate,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd,
        Amount = Amount,
        PaidAmount = PaidAmount,
        RemainingAmount = RemainingAmount,
        Status = Status,
        Notes = Notes
    };
}

public class LeasePaymentAllocation : Entity<Guid>
{
    private LeasePaymentAllocation() { }

    public Guid LeaseInstallmentId { get; private set; }
    public Guid? PaymentReferenceId { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? Reference { get; private set; }
    public decimal Amount { get; private set; }
    public string? Notes { get; private set; }

    public static LeasePaymentAllocation Create(Guid installmentId, Guid? paymentReferenceId, DateTime paymentDate, string? reference, decimal amount, string? notes, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            LeaseInstallmentId = installmentId,
            PaymentReferenceId = paymentReferenceId,
            PaymentDate = paymentDate,
            Reference = reference?.Trim(),
            Amount = amount,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
}

public class PropertyExpense : Aggregate<Guid>
{
    private PropertyExpense() { }

    public Guid CompanyId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? UnitId { get; private set; }
    public Guid? LeaseId { get; private set; }
    public ExpenseCategory Category { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string? SupplierName { get; private set; }
    public bool IsRecoverableFromTenant { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public string? Notes { get; private set; }
    public Property Property { get; private set; } = default!;

    public decimal TotalAmount => Amount + TaxAmount;

    public static PropertyExpense Create(PropertyExpenseDto dto, string userId)
    {
        Validate(dto);

        var expense = new PropertyExpense
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        expense.Apply(dto, userId);
        return expense;
    }

    public void Update(PropertyExpenseDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public PropertyExpenseDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        PropertyId = PropertyId,
        PropertyName = Property?.Name ?? string.Empty,
        UnitId = UnitId,
        LeaseId = LeaseId,
        Category = Category,
        ExpenseDate = ExpenseDate,
        Amount = Amount,
        TaxAmount = TaxAmount,
        TotalAmount = TotalAmount,
        SupplierId = SupplierId,
        SupplierName = SupplierName,
        IsRecoverableFromTenant = IsRecoverableFromTenant,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        Notes = Notes
    };

    private void Apply(PropertyExpenseDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        PropertyId = dto.PropertyId;
        UnitId = dto.UnitId;
        LeaseId = dto.LeaseId;
        Category = dto.Category;
        ExpenseDate = dto.ExpenseDate == default ? DateTime.UtcNow : dto.ExpenseDate.Date;
        Amount = dto.Amount;
        TaxAmount = dto.TaxAmount;
        SupplierId = dto.SupplierId;
        SupplierName = dto.SupplierName?.Trim();
        IsRecoverableFromTenant = dto.IsRecoverableFromTenant;
        SourceDocumentId = dto.SourceDocumentId;
        SourceDocumentNumber = dto.SourceDocumentNumber?.Trim();
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(PropertyExpenseDto dto)
    {
        if (dto.CompanyId == Guid.Empty || dto.PropertyId == Guid.Empty)
            throw new BadRequestException("Company and property are required.");
        if (dto.Amount < 0 || dto.TaxAmount < 0)
            throw new BadRequestException("Expense amounts cannot be negative.");
    }
}

public class UtilityAccount : Aggregate<Guid>
{
    private UtilityAccount() { }

    public Guid CompanyId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? UnitId { get; private set; }
    public UtilityServiceType ServiceType { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public string? MeterNumber { get; private set; }
    public string? ProviderName { get; private set; }
    public Guid? SupplierId { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    public static UtilityAccount Create(UtilityAccountDto dto, string userId)
    {
        Validate(dto);

        var account = new UtilityAccount
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        account.Apply(dto, userId);
        return account;
    }

    public void Update(UtilityAccountDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public UtilityAccountDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        PropertyId = PropertyId,
        UnitId = UnitId,
        ServiceType = ServiceType,
        AccountNumber = AccountNumber,
        MeterNumber = MeterNumber,
        ProviderName = ProviderName,
        SupplierId = SupplierId,
        IsActive = IsActive,
        Notes = Notes
    };

    private void Apply(UtilityAccountDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        PropertyId = dto.PropertyId;
        UnitId = dto.UnitId;
        ServiceType = dto.ServiceType;
        AccountNumber = dto.AccountNumber.Trim();
        MeterNumber = dto.MeterNumber?.Trim();
        ProviderName = dto.ProviderName?.Trim();
        SupplierId = dto.SupplierId;
        IsActive = dto.IsActive;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(UtilityAccountDto dto)
    {
        if (dto.CompanyId == Guid.Empty || dto.PropertyId == Guid.Empty || string.IsNullOrWhiteSpace(dto.AccountNumber))
            throw new BadRequestException("Company, property, and account number are required.");
    }
}

public class UtilityBill : Aggregate<Guid>
{
    private UtilityBill() { }

    public Guid UtilityAccountId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? UnitId { get; private set; }
    public DateTime BillDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public bool IsPaid { get; private set; }
    public Guid? ExpenseId { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public decimal TotalAmount => Amount + TaxAmount;

    public static UtilityBill Create(UtilityBillDto dto, string userId)
    {
        Validate(dto);

        var bill = new UtilityBill
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        bill.Apply(dto, userId);
        return bill;
    }

    public void Update(UtilityBillDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void MarkPaid(string userId)
    {
        IsPaid = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public UtilityBillDto ToDto() => new()
    {
        Id = Id,
        UtilityAccountId = UtilityAccountId,
        PropertyId = PropertyId,
        UnitId = UnitId,
        BillDate = BillDate,
        DueDate = DueDate,
        Amount = Amount,
        TaxAmount = TaxAmount,
        TotalAmount = TotalAmount,
        IsPaid = IsPaid,
        ExpenseId = ExpenseId,
        Reference = Reference,
        Notes = Notes
    };

    private void Apply(UtilityBillDto dto, string userId)
    {
        UtilityAccountId = dto.UtilityAccountId;
        PropertyId = dto.PropertyId;
        UnitId = dto.UnitId;
        BillDate = dto.BillDate == default ? DateTime.UtcNow : dto.BillDate.Date;
        DueDate = dto.DueDate == default ? DateTime.UtcNow : dto.DueDate.Date;
        Amount = dto.Amount;
        TaxAmount = dto.TaxAmount;
        IsPaid = dto.IsPaid;
        ExpenseId = dto.ExpenseId;
        Reference = dto.Reference?.Trim();
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(UtilityBillDto dto)
    {
        if (dto.UtilityAccountId == Guid.Empty || dto.PropertyId == Guid.Empty)
            throw new BadRequestException("Utility account and property are required.");
        if (dto.Amount < 0 || dto.TaxAmount < 0)
            throw new BadRequestException("Utility bill amounts cannot be negative.");
    }
}

public class OccupancyHistory : Aggregate<Guid>
{
    private OccupancyHistory() { }

    public Guid PropertyId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid LeaseId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public static OccupancyHistory Create(Guid propertyId, Guid unitId, Guid leaseId, Guid customerId, DateTime startDate, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            UnitId = unitId,
            LeaseId = leaseId,
            CustomerId = customerId,
            StartDate = startDate.Date,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Close(DateTime endDate, string userId)
    {
        EndDate = endDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
