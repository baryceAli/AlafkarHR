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
