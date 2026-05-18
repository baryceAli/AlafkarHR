using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public class WarehouseTransferDto
{
    public Guid Id { get; set; }
    public Guid SourceWarehouseId { get; set; }

    public Guid DestinationWarehouseId { get;  set; }

    public TransferStatus Status { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    //public string TransferNumber { get; set; }
    //public Guid RequestedBy { get; set; }
    //public Guid ApprovedBy { get; set; }
    public string Reason { get; set; }
    //public string ReferenceNumber { get; set; }
    //public DateTime ExpectedDeliveryDate { get; set; }



    private List<TransferItemDto> Items = new();

}
