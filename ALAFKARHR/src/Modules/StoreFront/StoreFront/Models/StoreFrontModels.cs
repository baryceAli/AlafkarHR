namespace StoreFront.Models;

public class StoreFrontType : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private StoreFrontType()
    {
    }

    public static StoreFrontType Create(Guid companyId, string name, string nameEng, string code, string createdBy)
    {
        Validate(companyId, name, nameEng, code);
        return new StoreFrontType
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = name.Trim(),
            NameEng = nameEng.Trim(),
            Code = NormalizeCode(code),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string nameEng, string code, bool isActive, string modifiedBy)
    {
        Validate(CompanyId, name, nameEng, code);
        Name = name.Trim();
        NameEng = nameEng.Trim();
        Code = NormalizeCode(code);
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public static string NormalizeCode(string code) => code.Trim().ToLowerInvariant();

    private static void Validate(Guid companyId, string name, string nameEng, string code)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(nameEng)) throw new ArgumentException("English name is required", nameof(nameEng));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required", nameof(code));
    }
}

public class StoreFrontStore : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid StoreFrontTypeId { get; private set; }
    public StoreFrontType StoreFrontType { get; private set; } = default!;
    public Guid DefaultWarehouseId { get; private set; }
    public Guid? DefaultCustomerId { get; private set; }
    public Guid? PriceListId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? ReceiptHeader { get; private set; }
    public string? ReceiptFooter { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<StoreFrontSellableItem> _sellableItems = [];
    public IReadOnlyCollection<StoreFrontSellableItem> SellableItems => _sellableItems;

    private StoreFrontStore()
    {
    }

    public static StoreFrontStore Create(StoreFrontDto dto, string createdBy)
    {
        Validate(dto);
        return new StoreFrontStore
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            StoreFrontTypeId = dto.StoreFrontTypeId,
            DefaultWarehouseId = dto.DefaultWarehouseId,
            DefaultCustomerId = dto.DefaultCustomerId,
            PriceListId = dto.PriceListId,
            Name = dto.Name.Trim(),
            NameEng = dto.NameEng.Trim(),
            Code = NormalizeCode(dto.Code),
            ReceiptHeader = dto.ReceiptHeader?.Trim(),
            ReceiptFooter = dto.ReceiptFooter?.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(StoreFrontDto dto, string modifiedBy)
    {
        Validate(dto);
        BranchId = dto.BranchId;
        StoreFrontTypeId = dto.StoreFrontTypeId;
        DefaultWarehouseId = dto.DefaultWarehouseId;
        DefaultCustomerId = dto.DefaultCustomerId;
        PriceListId = dto.PriceListId;
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        Code = NormalizeCode(dto.Code);
        ReceiptHeader = dto.ReceiptHeader?.Trim();
        ReceiptFooter = dto.ReceiptFooter?.Trim();
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void SetActive(bool isActive, string modifiedBy)
    {
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public static string NormalizeCode(string code) => code.Trim().ToLowerInvariant();

    private static void Validate(StoreFrontDto dto)
    {
        if (dto.CompanyId == Guid.Empty) throw new ArgumentException("Company is required");
        if (dto.StoreFrontTypeId == Guid.Empty) throw new ArgumentException("Store type is required");
        if (dto.DefaultWarehouseId == Guid.Empty) throw new ArgumentException("Default warehouse is required");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(dto.NameEng)) throw new ArgumentException("English name is required");
        if (string.IsNullOrWhiteSpace(dto.Code)) throw new ArgumentException("Code is required");
    }
}

public class StoreFrontSellableItem : Entity<Guid>
{
    public Guid StoreFrontId { get; private set; }
    public StoreFrontStore StoreFront { get; private set; } = default!;
    public Guid ProductSkuId { get; private set; }
    public string? ProductName { get; private set; }
    public string? ProductNameEng { get; private set; }
    public string? SkuCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public bool AllowManualPrice { get; private set; }
    public bool RequireManualPriceNote { get; private set; }
    public decimal? MinimumManualPrice { get; private set; }
    public decimal? MaximumManualPrice { get; private set; }

    private StoreFrontSellableItem()
    {
    }

    public static StoreFrontSellableItem Create(Guid storeFrontId, StoreFrontSellableItemDto dto, string createdBy)
    {
        Validate(storeFrontId, dto);
        return new StoreFrontSellableItem
        {
            Id = Guid.NewGuid(),
            StoreFrontId = storeFrontId,
            ProductSkuId = dto.ProductSkuId,
            ProductName = dto.ProductName,
            ProductNameEng = dto.ProductNameEng,
            SkuCode = dto.SkuCode,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            AllowManualPrice = dto.AllowManualPrice,
            RequireManualPriceNote = dto.RequireManualPriceNote,
            MinimumManualPrice = dto.MinimumManualPrice,
            MaximumManualPrice = dto.MaximumManualPrice,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(StoreFrontSellableItemDto dto, string modifiedBy)
    {
        Validate(StoreFrontId, dto);
        ProductName = dto.ProductName;
        ProductNameEng = dto.ProductNameEng;
        SkuCode = dto.SkuCode;
        IsActive = dto.IsActive;
        DisplayOrder = dto.DisplayOrder;
        AllowManualPrice = dto.AllowManualPrice;
        RequireManualPriceNote = dto.RequireManualPriceNote;
        MinimumManualPrice = dto.MinimumManualPrice;
        MaximumManualPrice = dto.MaximumManualPrice;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static void Validate(Guid storeFrontId, StoreFrontSellableItemDto dto)
    {
        if (storeFrontId == Guid.Empty) throw new ArgumentException("Store front is required");
        if (dto.ProductSkuId == Guid.Empty) throw new ArgumentException("Product SKU is required");
        if (dto.MinimumManualPrice.HasValue && dto.MinimumManualPrice < 0) throw new ArgumentException("Minimum manual price cannot be negative");
        if (dto.MaximumManualPrice.HasValue && dto.MaximumManualPrice < 0) throw new ArgumentException("Maximum manual price cannot be negative");
        if (dto.MinimumManualPrice.HasValue && dto.MaximumManualPrice.HasValue && dto.MinimumManualPrice > dto.MaximumManualPrice)
            throw new ArgumentException("Minimum manual price cannot be greater than maximum manual price");
    }
}

public class PosCashierSession : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid StoreFrontId { get; private set; }
    public string CashierUserId { get; private set; } = string.Empty;
    public Guid? CashAccountId { get; private set; }
    public decimal OpeningAmount { get; private set; }
    public decimal ExpectedCashAmount { get; private set; }
    public decimal CashSalesAmount { get; private set; }
    public decimal CardSalesAmount { get; private set; }
    public int PaymentCount { get; private set; }
    public decimal? CountedCashAmount { get; private set; }
    public decimal? VarianceAmount { get; private set; }
    public PosCashierSessionStatus Status { get; private set; } = PosCashierSessionStatus.Open;
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private PosCashierSession()
    {
    }

    public static PosCashierSession Open(Guid companyId, Guid branchId, Guid storeFrontId, string cashierUserId, Guid? cashAccountId, decimal openingAmount)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required", nameof(companyId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch is required", nameof(branchId));
        if (storeFrontId == Guid.Empty) throw new ArgumentException("StoreFront is required", nameof(storeFrontId));
        if (string.IsNullOrWhiteSpace(cashierUserId)) throw new ArgumentException("Cashier is required", nameof(cashierUserId));
        if (openingAmount < 0) throw new ArgumentException("Opening amount cannot be negative", nameof(openingAmount));

        var now = DateTime.UtcNow;
        return new PosCashierSession
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            BranchId = branchId,
            StoreFrontId = storeFrontId,
            CashierUserId = cashierUserId,
            CashAccountId = cashAccountId,
            OpeningAmount = openingAmount,
            ExpectedCashAmount = openingAmount,
            Status = PosCashierSessionStatus.Open,
            OpenedAt = now,
            CreatedAt = now,
            CreatedBy = cashierUserId
        };
    }

    public void RecordPayment(Guid paymentId, PaymentMethodType paymentMethod, decimal amount, string userId)
    {
        if (Status != PosCashierSessionStatus.Open)
            throw new BadRequestException("Cashier session is closed.");
        if (amount <= 0)
            throw new BadRequestException("Payment amount must be greater than zero.");

        PaymentCount++;
        if (paymentMethod == PaymentMethodType.Cash)
        {
            CashSalesAmount += amount;
            ExpectedCashAmount += amount;
        }
        else if (paymentMethod == PaymentMethodType.CardRecorded)
        {
            CardSalesAmount += amount;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void ReceiveHandover(decimal amount, string userId)
    {
        if (Status != PosCashierSessionStatus.Open)
            throw new BadRequestException("Only open cashier sessions can receive handover cash.");
        if (amount <= 0)
            throw new BadRequestException("Handover amount must be greater than zero.");

        ExpectedCashAmount += amount;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Close(decimal countedCashAmount, string userId)
    {
        if (Status != PosCashierSessionStatus.Open)
            throw new BadRequestException("Cashier session is already closed.");
        if (countedCashAmount < 0)
            throw new BadRequestException("Counted cash cannot be negative.");

        CountedCashAmount = countedCashAmount;
        VarianceAmount = countedCashAmount - ExpectedCashAmount;
        Status = PosCashierSessionStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public PosCashierSessionDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        BranchId = BranchId,
        StoreFrontId = StoreFrontId,
        CashierUserId = CashierUserId,
        CashAccountId = CashAccountId,
        OpeningAmount = OpeningAmount,
        ExpectedCashAmount = ExpectedCashAmount,
        CashSalesAmount = CashSalesAmount,
        CardSalesAmount = CardSalesAmount,
        PaymentCount = PaymentCount,
        CountedCashAmount = CountedCashAmount,
        VarianceAmount = VarianceAmount,
        Status = Status,
        OpenedAt = OpenedAt,
        ClosedAt = ClosedAt
    };
}

public class PosCashierSessionTransfer : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid StoreFrontId { get; private set; }
    public Guid FromSessionId { get; private set; }
    public Guid? ToSessionId { get; private set; }
    public Guid? ToCashAccountId { get; private set; }
    public decimal Amount { get; private set; }

    private PosCashierSessionTransfer()
    {
    }

    public static PosCashierSessionTransfer Create(Guid companyId, Guid branchId, Guid storeFrontId, Guid fromSessionId, Guid? toSessionId, Guid? toCashAccountId, decimal amount, string userId)
    {
        if (!toSessionId.HasValue && !toCashAccountId.HasValue)
            throw new BadRequestException("Select a handover session or cash account.");

        return new PosCashierSessionTransfer
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            BranchId = branchId,
            StoreFrontId = storeFrontId,
            FromSessionId = fromSessionId,
            ToSessionId = toSessionId,
            ToCashAccountId = toCashAccountId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}
