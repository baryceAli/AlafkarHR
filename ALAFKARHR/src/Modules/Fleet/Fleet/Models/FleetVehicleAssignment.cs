namespace Fleet.Models;

public class FleetVehicleAssignment : Aggregate<Guid>
{
    public Guid VehicleId { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Purpose { get; private set; }
    public int? OdometerOut { get; private set; }
    public int? OdometerIn { get; private set; }
    public decimal? FuelLevelOut { get; private set; }
    public decimal? FuelLevelIn { get; private set; }
    public FleetAssignmentStatus Status { get; private set; }

    public FleetVehicle Vehicle { get; private set; } = default!;

    private FleetVehicleAssignment()
    {
    }

    public static FleetVehicleAssignment Create(CreateFleetVehicleAssignmentDto dto, Guid createdByUserId)
    {
        if (dto.VehicleId == Guid.Empty)
            throw new BadRequestException("Vehicle is required.");
        if (!dto.EmployeeId.HasValue && !dto.UserId.HasValue)
            throw new BadRequestException("Employee or user is required.");

        return new FleetVehicleAssignment
        {
            Id = Guid.NewGuid(),
            VehicleId = dto.VehicleId,
            EmployeeId = dto.EmployeeId,
            UserId = dto.UserId,
            BranchId = dto.BranchId,
            DepartmentId = dto.DepartmentId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Purpose = dto.Purpose?.Trim(),
            OdometerOut = dto.OdometerOut,
            FuelLevelOut = dto.FuelLevelOut,
            Status = FleetAssignmentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public void Return(ReturnFleetVehicleAssignmentDto dto, Guid modifiedByUserId)
    {
        if (Status != FleetAssignmentStatus.Active)
            throw new BadRequestException("Only active assignments can be returned.");

        EndDate = dto.ReturnDate;
        OdometerIn = dto.OdometerIn;
        FuelLevelIn = dto.FuelLevelIn;
        Status = FleetAssignmentStatus.Returned;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Cancel(Guid modifiedByUserId)
    {
        Status = FleetAssignmentStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }
}
