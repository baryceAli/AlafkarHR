using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public class WarehouseTransferDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public string? SourceWarehouseName { get; set; }
    public string? SourceWarehouseNameEng { get; set; }

    public Guid DestinationWarehouseId { get;  set; }
    public string? DestinationWarehouseName { get; set; }
    public string? DestinationWarehouseNameEng { get; set; }

    public TransferStatus Status { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public string TransferNumber { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? Reason { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public List<TransferItemDto> Items { get; set; } = new();
}

public class CreateWarehouseTransferDto
{
    public Guid CompanyId { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public Guid DestinationWarehouseId { get; set; }
    public string? TransferNumber { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
}

public class WarehouseTransferItemInputDto
{
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid BatchId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public Guid CurrencyId { get; set; }
}

public class ReceiveWarehouseTransferItemDto
{
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public Guid? DestinationLocationId { get; set; }
}
