namespace Fleet.Models;

public class FleetVehicleServiceRule : Aggregate<Guid>
{
    public Guid VehicleId { get; private set; }
    public FleetServiceType ServiceType { get; private set; }
    public int? IntervalKilometers { get; private set; }
    public int? IntervalDays { get; private set; }
    public int? LastServiceOdometer { get; private set; }
    public DateTime? LastServiceDate { get; private set; }
    public int? NextDueOdometer { get; private set; }
    public DateTime? NextDueDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    public FleetVehicle Vehicle { get; private set; } = default!;

    private FleetVehicleServiceRule()
    {
    }

    public static FleetVehicleServiceRule Create(CreateFleetVehicleServiceRuleDto dto, Guid createdByUserId)
    {
        EnsureValid(dto);
        return new FleetVehicleServiceRule
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        }.Apply(dto);
    }

    public void Update(UpdateFleetVehicleServiceRuleDto dto, Guid modifiedByUserId)
    {
        EnsureValid(dto);
        Apply(dto);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void CompleteService(int? odometer, DateTime serviceDate, Guid modifiedByUserId)
    {
        LastServiceOdometer = odometer;
        LastServiceDate = serviceDate;
        RecalculateNextDue();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    public bool IsDue(int currentOdometer, DateTime today)
    {
        return IsActive &&
            ((NextDueOdometer.HasValue && currentOdometer >= NextDueOdometer.Value) ||
             (NextDueDate.HasValue && today.Date >= NextDueDate.Value.Date));
    }

    private FleetVehicleServiceRule Apply(CreateFleetVehicleServiceRuleDto dto)
    {
        VehicleId = dto.VehicleId;
        ServiceType = dto.ServiceType;
        IntervalKilometers = dto.IntervalKilometers;
        IntervalDays = dto.IntervalDays;
        LastServiceOdometer = dto.LastServiceOdometer;
        LastServiceDate = dto.LastServiceDate;
        IsActive = dto.IsActive;
        Notes = dto.Notes?.Trim();
        RecalculateNextDue();
        return this;
    }

    private void RecalculateNextDue()
    {
        NextDueOdometer = LastServiceOdometer.HasValue && IntervalKilometers.HasValue
            ? LastServiceOdometer.Value + IntervalKilometers.Value
            : null;
        NextDueDate = LastServiceDate.HasValue && IntervalDays.HasValue
            ? LastServiceDate.Value.Date.AddDays(IntervalDays.Value)
            : null;
    }

    private static void EnsureValid(CreateFleetVehicleServiceRuleDto dto)
    {
        if (dto.VehicleId == Guid.Empty)
            throw new BadRequestException("Vehicle is required.");
        if (!dto.IntervalKilometers.HasValue && !dto.IntervalDays.HasValue)
            throw new BadRequestException("Service rule requires a kilometer or day interval.");
    }
}
