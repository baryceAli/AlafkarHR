using SharedWithUI.Catalog.Dtos;

namespace SharedWithUI.StoreFront.Dtos;

public class StoreFrontTypeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class StoreFrontDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid StoreFrontTypeId { get; set; }
    public string? StoreFrontTypeName { get; set; }
    public string? StoreFrontTypeNameEng { get; set; }
    public Guid DefaultWarehouseId { get; set; }
    public Guid? DefaultCustomerId { get; set; }
    public Guid? PriceListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ReceiptHeader { get; set; }
    public string? ReceiptFooter { get; set; }
    public bool IsActive { get; set; } = true;
    public int ActiveItemsCount { get; set; }
}

public class StoreFrontSellableItemDto
{
    public Guid Id { get; set; }
    public Guid StoreFrontId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public string? SkuCode { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool AllowManualPrice { get; set; }
    public bool RequireManualPriceNote { get; set; }
    public decimal? MinimumManualPrice { get; set; }
    public decimal? MaximumManualPrice { get; set; }
}

public class StoreFrontCatalogItemDto
{
    public ProductSkuDto Sku { get; set; } = new();
    public StoreFrontSellableItemDto Settings { get; set; } = new();
}
