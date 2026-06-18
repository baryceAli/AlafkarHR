namespace Fleet.Models;

public class FleetVehicle : Aggregate<Guid>
{
    public string VehicleCode { get; private set; } = string.Empty;
    public string PlateNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string? Make { get; private set; }
    public string? Model { get; private set; }
    public int? Year { get; private set; }
    public string? Color { get; private set; }
    public string? Vin { get; private set; }
    public string? EngineNumber { get; private set; }
    public FleetVehicleType VehicleType { get; private set; }
    public FleetVehicleStatus Status { get; private set; }
    public FleetVehicleOwnershipType OwnershipType { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? MaintenanceAssetId { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public decimal? PurchaseCost { get; private set; }
    public DateTime? WarrantyEndDate { get; private set; }
    public Guid? SupplierId { get; private set; }
    public Guid? RentalContractId { get; private set; }
    public DateTime? RentalStartDate { get; private set; }
    public DateTime? RentalEndDate { get; private set; }
    public decimal? MonthlyRent { get; private set; }
    public decimal? DailyRent { get; private set; }
    public decimal? DepositAmount { get; private set; }
    public int? AllowedKilometers { get; private set; }
    public decimal? ExcessKilometerRate { get; private set; }
    public int CurrentOdometer { get; private set; }
    public FleetFuelType FuelType { get; private set; }
    public decimal? FuelCapacity { get; private set; }
    public Guid? DefaultDriverEmployeeId { get; private set; }
    public string? Notes { get; private set; }

    public List<FleetVehicleAssignment> Assignments { get; private set; } = [];
    public List<FleetVehicleDocument> Documents { get; private set; } = [];
    public List<FleetVehicleExpense> Expenses { get; private set; } = [];
    public List<FleetVehicleServiceRule> ServiceRules { get; private set; } = [];

    private FleetVehicle()
    {
    }

    public static FleetVehicle Create(string vehicleCode, CreateFleetVehicleDto dto, Guid createdByUserId)
    {
        EnsureValid(dto);
        return new FleetVehicle
        {
            Id = Guid.NewGuid(),
            VehicleCode = vehicleCode.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        }.Apply(dto);
    }

    public void Update(UpdateFleetVehicleDto dto, Guid modifiedByUserId)
    {
        EnsureValid(dto);
        Apply(dto);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void LinkMaintenanceAsset(Guid maintenanceAssetId, Guid modifiedByUserId)
    {
        MaintenanceAssetId = maintenanceAssetId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void UpdateOdometer(int odometer, Guid modifiedByUserId)
    {
        if (odometer < CurrentOdometer)
            throw new BadRequestException("Odometer cannot be lower than the current value.");

        CurrentOdometer = odometer;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void SetStatus(FleetVehicleStatus status, Guid modifiedByUserId)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private FleetVehicle Apply(CreateFleetVehicleDto dto)
    {
        PlateNumber = dto.PlateNumber.Trim();
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        Make = dto.Make?.Trim();
        Model = dto.Model?.Trim();
        Year = dto.Year;
        Color = dto.Color?.Trim();
        Vin = dto.Vin?.Trim();
        EngineNumber = dto.EngineNumber?.Trim();
        VehicleType = dto.VehicleType;
        Status = dto.Status;
        OwnershipType = dto.OwnershipType;
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        PurchaseDate = dto.OwnershipType == FleetVehicleOwnershipType.Owned ? dto.PurchaseDate : null;
        PurchaseCost = dto.OwnershipType == FleetVehicleOwnershipType.Owned ? dto.PurchaseCost : null;
        WarrantyEndDate = dto.OwnershipType == FleetVehicleOwnershipType.Owned ? dto.WarrantyEndDate : null;
        SupplierId = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.SupplierId : null;
        RentalContractId = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.RentalContractId : null;
        RentalStartDate = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.RentalStartDate : null;
        RentalEndDate = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.RentalEndDate : null;
        MonthlyRent = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.MonthlyRent : null;
        DailyRent = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.DailyRent : null;
        DepositAmount = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.DepositAmount : null;
        AllowedKilometers = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.AllowedKilometers : null;
        ExcessKilometerRate = dto.OwnershipType == FleetVehicleOwnershipType.Rented ? dto.ExcessKilometerRate : null;
        CurrentOdometer = dto.CurrentOdometer;
        FuelType = dto.FuelType;
        FuelCapacity = dto.FuelCapacity;
        DefaultDriverEmployeeId = dto.DefaultDriverEmployeeId;
        Notes = dto.Notes?.Trim();
        return this;
    }

    private static void EnsureValid(CreateFleetVehicleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PlateNumber))
            throw new BadRequestException("Plate number is required.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Vehicle name is required.");
        if (string.IsNullOrWhiteSpace(dto.NameEng))
            throw new BadRequestException("Vehicle English name is required.");
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (dto.CurrentOdometer < 0)
            throw new BadRequestException("Current odometer cannot be negative.");
        if (dto.OwnershipType == FleetVehicleOwnershipType.Rented)
        {
            if (!dto.SupplierId.HasValue)
                throw new BadRequestException("Supplier is required for rented vehicles.");
            if (!dto.RentalStartDate.HasValue || !dto.RentalEndDate.HasValue)
                throw new BadRequestException("Rental start and end dates are required for rented vehicles.");
            if (dto.RentalEndDate.Value.Date < dto.RentalStartDate.Value.Date)
                throw new BadRequestException("Rental end date cannot be before rental start date.");
        }
    }
}
