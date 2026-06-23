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
    public int LeadTimeDays { get; private set; }
    public bool AutoCreatePurchaseRequest { get; private set; }
    public bool IsActive { get; private set; }

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

    public ReorderingRuleDto ToDto() => new()
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
        LeadTimeDays = LeadTimeDays,
        AutoCreatePurchaseRequest = AutoCreatePurchaseRequest,
        IsActive = IsActive
    };

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
        LeadTimeDays = dto.LeadTimeDays;
        AutoCreatePurchaseRequest = dto.AutoCreatePurchaseRequest;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
