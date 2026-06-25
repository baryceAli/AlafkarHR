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
    public Guid? AdministrationId { get; set; }
    public string? AdministrationName { get; set; }
    public string? AdministrationNameEng { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? DepartmentNameEng { get; set; }
    public Guid StoreFrontTypeId { get; set; }
    public string? StoreFrontTypeName { get; set; }
    public string? StoreFrontTypeNameEng { get; set; }
    public Guid DefaultWarehouseId { get; set; }
    public Guid? StoreManagerEmployeeId { get; set; }
    public string? StoreManagerName { get; set; }
    public string? StoreManagerNameEng { get; set; }
    public Guid? DefaultCustomerId { get; set; }
    public Guid? PriceListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
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

public class StoreFrontDepartmentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid StoreFrontId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class StoreFrontCatalogItemDto
{
    public ProductSkuDto Sku { get; set; } = new();
    public StoreFrontSellableItemDto Settings { get; set; } = new();
}

public enum PosCashierSessionStatus
{
    Open = 0,
    Closed = 1
}

public class PosCashierSessionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BranchId { get; set; }
    public Guid StoreFrontId { get; set; }
    public string CashierUserId { get; set; } = string.Empty;
    public Guid? CashAccountId { get; set; }
    public decimal OpeningAmount { get; set; }
    public decimal ExpectedCashAmount { get; set; }
    public decimal CashSalesAmount { get; set; }
    public decimal CardSalesAmount { get; set; }
    public int PaymentCount { get; set; }
    public decimal? CountedCashAmount { get; set; }
    public decimal? VarianceAmount { get; set; }
    public PosCashierSessionStatus Status { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid? HandoverToSessionId { get; set; }
    public Guid? HandoverToCashAccountId { get; set; }
    public decimal? HandoverAmount { get; set; }
}

public class OpenPosCashierSessionDto
{
    public Guid StoreFrontId { get; set; }
    public Guid? CashAccountId { get; set; }
    public decimal OpeningAmount { get; set; }
}

public class ClosePosCashierSessionDto
{
    public decimal CountedCashAmount { get; set; }
    public Guid? HandoverToSessionId { get; set; }
    public Guid? HandoverToCashAccountId { get; set; }
}

public class PosCashierSessionSummaryDto
{
    public Guid StoreFrontId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal CashSalesAmount { get; set; }
    public decimal CardSalesAmount { get; set; }
    public decimal ExpectedCashAmount { get; set; }
    public decimal CountedCashAmount { get; set; }
    public decimal VarianceAmount { get; set; }
    public int SessionCount { get; set; }
    public int PaymentCount { get; set; }
}
