namespace Fleet.Models;

public class FleetVehicleDocument : Aggregate<Guid>
{
    public Guid VehicleId { get; private set; }
    public FleetDocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = string.Empty;
    public DateTime? IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public decimal? RenewalCost { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public string? ContentType { get; private set; }
    public long? FileSize { get; private set; }
    public FleetDocumentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public FleetVehicle Vehicle { get; private set; } = default!;

    private FleetVehicleDocument()
    {
    }

    public static FleetVehicleDocument Create(CreateFleetVehicleDocumentDto dto, Guid createdByUserId)
    {
        if (dto.VehicleId == Guid.Empty)
            throw new BadRequestException("Vehicle is required.");

        return new FleetVehicleDocument
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        }.Apply(dto);
    }

    public void Update(UpdateFleetVehicleDocumentDto dto, Guid modifiedByUserId)
    {
        Apply(dto);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Renew(RenewFleetVehicleDocumentDto dto, Guid modifiedByUserId)
    {
        IssueDate = dto.IssueDate;
        ExpiryDate = dto.ExpiryDate;
        RenewalCost = dto.RenewalCost;
        SupplierId = dto.SupplierId;
        Notes = dto.Notes?.Trim();
        Status = FleetDocumentStatus.Renewed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void RefreshStatus(DateTime today)
    {
        if (Status == FleetDocumentStatus.Renewed)
            return;
        Status = ExpiryDate switch
        {
            null => FleetDocumentStatus.Active,
            var date when date.Value.Date < today.Date => FleetDocumentStatus.Expired,
            var date when date.Value.Date <= today.Date.AddDays(30) => FleetDocumentStatus.ExpiringSoon,
            _ => FleetDocumentStatus.Active
        };
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private FleetVehicleDocument Apply(CreateFleetVehicleDocumentDto dto)
    {
        VehicleId = dto.VehicleId;
        DocumentType = dto.DocumentType;
        DocumentNumber = dto.DocumentNumber.Trim();
        IssueDate = dto.IssueDate;
        ExpiryDate = dto.ExpiryDate;
        RenewalCost = dto.RenewalCost;
        SupplierId = dto.SupplierId;
        FileName = dto.FileName?.Trim();
        FilePath = dto.FilePath?.Trim();
        ContentType = dto.ContentType?.Trim();
        FileSize = dto.FileSize;
        Notes = dto.Notes?.Trim();
        RefreshStatus(DateTime.UtcNow);
        return this;
    }
}
