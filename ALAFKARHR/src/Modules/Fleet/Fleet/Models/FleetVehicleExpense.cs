namespace Fleet.Models;

public class FleetVehicleExpense : Aggregate<Guid>
{
    public Guid VehicleId { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public FleetExpenseCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public string? CurrencyCode { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string? VendorName { get; private set; }
    public int? Odometer { get; private set; }
    public decimal? Quantity { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public string? Notes { get; private set; }
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public Guid? MaintenanceWorkOrderId { get; private set; }
    public Guid? ContractId { get; private set; }
    public Guid? ProcurementDocumentId { get; private set; }
    public Guid? PaymentReferenceId { get; private set; }
    public FleetExpenseApprovalStatus ApprovalStatus { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovalNotes { get; private set; }

    public FleetVehicle Vehicle { get; private set; } = default!;

    private FleetVehicleExpense()
    {
    }

    public static FleetVehicleExpense Create(CreateFleetVehicleExpenseDto dto, Guid createdByUserId)
    {
        EnsureValid(dto);
        return new FleetVehicleExpense
        {
            Id = Guid.NewGuid(),
            ApprovalStatus = FleetExpenseApprovalStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        }.Apply(dto);
    }

    public void Update(UpdateFleetVehicleExpenseDto dto, Guid modifiedByUserId)
    {
        if (ApprovalStatus == FleetExpenseApprovalStatus.Approved)
            throw new BadRequestException("Approved expenses cannot be edited.");

        EnsureValid(dto);
        Apply(dto);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Submit(Guid modifiedByUserId)
    {
        ApprovalStatus = FleetExpenseApprovalStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Approve(bool isApproved, string? notes, Guid approvedByUserId)
    {
        ApprovalStatus = isApproved ? FleetExpenseApprovalStatus.Approved : FleetExpenseApprovalStatus.Rejected;
        ApprovalNotes = notes?.Trim();
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedByUserId.ToString();
    }

    public void Cancel(Guid modifiedByUserId)
    {
        ApprovalStatus = FleetExpenseApprovalStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private FleetVehicleExpense Apply(CreateFleetVehicleExpenseDto dto)
    {
        VehicleId = dto.VehicleId;
        ExpenseDate = dto.ExpenseDate;
        Category = dto.Category;
        Amount = dto.Amount;
        CurrencyCode = dto.CurrencyCode?.Trim();
        SupplierId = dto.SupplierId;
        VendorName = dto.VendorName?.Trim();
        Odometer = dto.Odometer;
        Quantity = dto.Quantity;
        UnitPrice = dto.UnitPrice;
        Notes = dto.Notes?.Trim();
        FileName = dto.FileName?.Trim();
        FilePath = dto.FilePath?.Trim();
        MaintenanceWorkOrderId = dto.MaintenanceWorkOrderId;
        ContractId = dto.ContractId;
        ProcurementDocumentId = dto.ProcurementDocumentId;
        PaymentReferenceId = dto.PaymentReferenceId;
        return this;
    }

    private static void EnsureValid(CreateFleetVehicleExpenseDto dto)
    {
        if (dto.VehicleId == Guid.Empty)
            throw new BadRequestException("Vehicle is required.");
        if (dto.Amount <= 0)
            throw new BadRequestException("Expense amount must be greater than zero.");
    }
}
