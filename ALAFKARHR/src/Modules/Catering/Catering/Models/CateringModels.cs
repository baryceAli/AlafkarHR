namespace Catering.Models;

public class MealDefinition : Aggregate<Guid>
{
    private readonly List<MealComponent> _components = [];

    public Guid CompanyId { get; private set; }
    public CateringMealType MealType { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameEng { get; private set; }
    public int? Calories { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }
    public IReadOnlyCollection<MealComponent> Components => _components.Where(x => !x.IsDeleted).ToList();

    private MealDefinition() { }

    public static MealDefinition Create(MealDefinitionDto dto, string userId)
    {
        Validate(dto);
        var meal = new MealDefinition { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        meal.Apply(dto, userId);
        return meal;
    }

    public void Update(MealDefinitionDto dto, string userId)
    {
        Validate(dto);
        Apply(dto, userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private void Apply(MealDefinitionDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        MealType = dto.MealType;
        Name = dto.Name.Trim();
        NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? null : dto.NameEng.Trim();
        Calories = dto.Calories;
        IsActive = dto.IsActive;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(MealDefinitionDto dto)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new BadRequestException("Meal name is required.");
        if (dto.Calories.HasValue && dto.Calories.Value < 0) throw new BadRequestException("Calories cannot be negative.");
    }
}

public class MealComponent : Entity<Guid>
{
    public Guid MealDefinitionId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string ComponentName { get; private set; } = string.Empty;
    public string? ComponentNameEng { get; private set; }
    public decimal QuantityPerMeal { get; private set; }
    public string? UnitName { get; private set; }
    public string? Notes { get; private set; }

    private MealComponent() { }

    public static MealComponent Create(Guid mealDefinitionId, MealComponentDto dto, string userId)
    {
        if (mealDefinitionId == Guid.Empty) throw new BadRequestException("Meal is required.");
        if (dto.ProductSkuId == Guid.Empty) throw new BadRequestException("Component SKU is required.");
        if (string.IsNullOrWhiteSpace(dto.ComponentName)) throw new BadRequestException("Component name is required.");
        if (dto.QuantityPerMeal <= 0) throw new BadRequestException("Component quantity must be greater than zero.");

        return new MealComponent
        {
            Id = Guid.NewGuid(),
            MealDefinitionId = mealDefinitionId,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            ComponentName = dto.ComponentName.Trim(),
            ComponentNameEng = dto.ComponentNameEng?.Trim(),
            QuantityPerMeal = dto.QuantityPerMeal,
            UnitName = dto.UnitName?.Trim(),
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class CateringContract : Aggregate<Guid>
{
    private readonly List<CateringContractAddendum> _addendums = [];

    public string Number { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string? CustomerNameEng { get; private set; }
    public Guid? GenericContractId { get; private set; }
    public CateringServiceType ServiceType { get; private set; }
    public string SeasonLabel { get; private set; } = string.Empty;
    public int? RamadanYear { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal ContractedMealQuantity { get; private set; }
    public Guid MealDefinitionId { get; private set; }
    public CateringContractStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<CateringContractAddendum> Addendums => _addendums.Where(x => !x.IsDeleted).ToList();

    private CateringContract() { }

    public static CateringContract Create(string number, CateringContractDto dto, string userId)
    {
        Validate(dto);
        var contract = new CateringContract
        {
            Id = Guid.NewGuid(),
            Number = number,
            Status = CateringContractStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        contract.Apply(dto, userId);
        return contract;
    }

    public void Update(CateringContractDto dto, string userId)
    {
        if (Status == CateringContractStatus.Closed) throw new BadRequestException("Closed catering contracts cannot be edited.");
        Validate(dto);
        Apply(dto, userId);
    }

    public void Close(string userId)
    {
        Status = CateringContractStatus.Closed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private void Apply(CateringContractDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        CustomerId = dto.CustomerId;
        CustomerName = dto.CustomerName.Trim();
        CustomerNameEng = dto.CustomerNameEng?.Trim();
        GenericContractId = dto.GenericContractId;
        ServiceType = dto.ServiceType;
        SeasonLabel = string.IsNullOrWhiteSpace(dto.SeasonLabel) ? dto.ServiceType.ToString() : dto.SeasonLabel.Trim();
        RamadanYear = dto.RamadanYear;
        StartDate = dto.StartDate.Date;
        EndDate = dto.EndDate.Date;
        ContractedMealQuantity = dto.ContractedMealQuantity;
        MealDefinitionId = dto.MealDefinitionId;
        if (dto.Status != CateringContractStatus.Draft || Status != CateringContractStatus.Draft) Status = dto.Status;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(CateringContractDto dto)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (dto.CustomerId == Guid.Empty) throw new BadRequestException("Charity/customer is required.");
        if (string.IsNullOrWhiteSpace(dto.CustomerName)) throw new BadRequestException("Charity/customer name is required.");
        if (dto.MealDefinitionId == Guid.Empty) throw new BadRequestException("Meal definition is required.");
        if (dto.ContractedMealQuantity <= 0) throw new BadRequestException("Contracted meal quantity must be greater than zero.");
        if (dto.EndDate.Date < dto.StartDate.Date) throw new BadRequestException("End date cannot be before start date.");
    }
}

public class CateringContractAddendum : Entity<Guid>
{
    public Guid CateringContractId { get; private set; }
    public decimal AddedQuantity { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid? AttachmentDocumentId { get; private set; }

    private CateringContractAddendum() { }

    public static CateringContractAddendum Create(Guid contractId, CateringContractAddendumDto dto, string userId)
    {
        if (contractId == Guid.Empty) throw new BadRequestException("Catering contract is required.");
        if (dto.AddedQuantity <= 0) throw new BadRequestException("Added quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(dto.Reason)) throw new BadRequestException("Addendum reason is required.");
        if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value.Date < dto.EffectiveFrom.Date) throw new BadRequestException("Effective end date cannot be before start date.");

        return new CateringContractAddendum
        {
            Id = Guid.NewGuid(),
            CateringContractId = contractId,
            AddedQuantity = dto.AddedQuantity,
            EffectiveFrom = dto.EffectiveFrom.Date,
            EffectiveTo = dto.EffectiveTo?.Date,
            Reason = dto.Reason.Trim(),
            AttachmentDocumentId = dto.AttachmentDocumentId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class CateringArea : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameEng { get; private set; }
    public string? GenderGroup { get; private set; }
    public string? LocationText { get; private set; }
    public bool IsActive { get; private set; } = true;

    private CateringArea() { }

    public static CateringArea Create(CateringAreaDto dto, string userId)
    {
        var area = new CateringArea { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        area.Update(dto, userId);
        return area;
    }

    public void Update(CateringAreaDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new BadRequestException("Area name is required.");
        CompanyId = dto.CompanyId;
        Name = dto.Name.Trim();
        NameEng = dto.NameEng?.Trim();
        GenderGroup = dto.GenderGroup?.Trim();
        LocationText = dto.LocationText?.Trim();
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringSquare : Entity<Guid>
{
    public Guid AreaId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? NameEng { get; private set; }
    public string? LocationText { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    private CateringSquare() { }

    public static CateringSquare Create(CateringSquareDto dto, string userId)
    {
        var square = new CateringSquare { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        square.Update(dto, userId);
        return square;
    }

    public void Update(CateringSquareDto dto, string userId)
    {
        if (dto.AreaId == Guid.Empty) throw new BadRequestException("Area is required.");
        if (string.IsNullOrWhiteSpace(dto.Code)) throw new BadRequestException("Square code is required.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new BadRequestException("Square name is required.");
        AreaId = dto.AreaId;
        Code = dto.Code.Trim();
        Name = dto.Name.Trim();
        NameEng = dto.NameEng?.Trim();
        LocationText = dto.LocationText?.Trim();
        Latitude = dto.Latitude;
        Longitude = dto.Longitude;
        IsActive = dto.IsActive;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringDailySchedule : Entity<Guid>
{
    private readonly List<CateringSquareAllocation> _allocations = [];

    public Guid CateringContractId { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<CateringSquareAllocation> Allocations => _allocations.Where(x => !x.IsDeleted).ToList();

    private CateringDailySchedule() { }

    public static CateringDailySchedule Create(CateringDailyScheduleDto dto, string userId)
    {
        var schedule = new CateringDailySchedule { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        schedule.Update(dto, userId);
        return schedule;
    }

    public void Update(CateringDailyScheduleDto dto, string userId)
    {
        if (dto.CateringContractId == Guid.Empty) throw new BadRequestException("Catering contract is required.");
        if (dto.PlannedQuantity <= 0) throw new BadRequestException("Planned quantity must be greater than zero.");
        CateringContractId = dto.CateringContractId;
        ServiceDate = dto.ServiceDate.Date;
        PlannedQuantity = dto.PlannedQuantity;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringSquareAllocation : Entity<Guid>
{
    public Guid DailyScheduleId { get; private set; }
    public Guid SquareId { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal DistributedQuantity { get; private set; }
    public string? VarianceNotes { get; private set; }

    private CateringSquareAllocation() { }

    public static CateringSquareAllocation Create(CateringSquareAllocationDto dto, string userId)
    {
        var allocation = new CateringSquareAllocation { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        allocation.UpdatePlan(dto, userId);
        return allocation;
    }

    public void UpdatePlan(CateringSquareAllocationDto dto, string userId)
    {
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        if (dto.SquareId == Guid.Empty) throw new BadRequestException("Square is required.");
        if (dto.PlannedQuantity <= 0) throw new BadRequestException("Planned quantity must be greater than zero.");
        DailyScheduleId = dto.DailyScheduleId;
        SquareId = dto.SquareId;
        PlannedQuantity = dto.PlannedQuantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void RecordActuals(decimal receivedQuantity, decimal distributedQuantity, string? varianceNotes, string userId)
    {
        if (receivedQuantity < 0 || distributedQuantity < 0) throw new BadRequestException("Quantities cannot be negative.");
        ReceivedQuantity = receivedQuantity;
        DistributedQuantity = distributedQuantity;
        VarianceNotes = varianceNotes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringVehicleDelivery : Entity<Guid>
{
    public Guid DailyScheduleId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string VehicleName { get; private set; } = string.Empty;
    public string? PlateNumber { get; private set; }
    public Guid DriverEmployeeId { get; private set; }
    public string DriverName { get; private set; } = string.Empty;
    public Guid ReceivingSupervisorEmployeeId { get; private set; }
    public string ReceivingSupervisorName { get; private set; } = string.Empty;
    public DateTime ArrivalTime { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public string? Notes { get; private set; }

    private CateringVehicleDelivery() { }

    public static CateringVehicleDelivery Create(CateringVehicleDeliveryDto dto, string userId)
    {
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        if (dto.VehicleId == Guid.Empty) throw new BadRequestException("Vehicle is required.");
        if (dto.DriverEmployeeId == Guid.Empty) throw new BadRequestException("Driver is required.");
        if (dto.ReceivingSupervisorEmployeeId == Guid.Empty) throw new BadRequestException("Receiving supervisor is required.");
        if (dto.ReceivedQuantity <= 0) throw new BadRequestException("Received quantity must be greater than zero.");

        return new CateringVehicleDelivery
        {
            Id = Guid.NewGuid(),
            DailyScheduleId = dto.DailyScheduleId,
            VehicleId = dto.VehicleId,
            VehicleName = dto.VehicleName.Trim(),
            PlateNumber = dto.PlateNumber?.Trim(),
            DriverEmployeeId = dto.DriverEmployeeId,
            DriverName = dto.DriverName.Trim(),
            ReceivingSupervisorEmployeeId = dto.ReceivingSupervisorEmployeeId,
            ReceivingSupervisorName = dto.ReceivingSupervisorName.Trim(),
            ArrivalTime = dto.ArrivalTime,
            ReceivedQuantity = dto.ReceivedQuantity,
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class CateringAssignment : Entity<Guid>
{
    public Guid CateringContractId { get; private set; }
    public CateringAssignmentRole Role { get; private set; }
    public Guid? SquareId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string EmployeeName { get; private set; } = string.Empty;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public string CoveredSquareIdsCsv { get; private set; } = string.Empty;
    public string DistributorEmployeeIdsCsv { get; private set; } = string.Empty;

    private CateringAssignment() { }

    public static CateringAssignment Create(CateringAssignmentDto dto, string userId)
    {
        var assignment = new CateringAssignment { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        assignment.Update(dto, userId);
        return assignment;
    }

    public void Update(CateringAssignmentDto dto, string userId)
    {
        if (dto.CateringContractId == Guid.Empty) throw new BadRequestException("Catering contract is required.");
        if (dto.EmployeeId == Guid.Empty) throw new BadRequestException("Employee is required.");
        if (string.IsNullOrWhiteSpace(dto.EmployeeName)) throw new BadRequestException("Employee name is required.");
        if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value.Date < dto.StartDate.Value.Date) throw new BadRequestException("Assignment end date cannot be before start date.");
        CateringContractId = dto.CateringContractId;
        Role = dto.Role;
        SquareId = dto.SquareId;
        EmployeeId = dto.EmployeeId;
        EmployeeName = dto.EmployeeName.Trim();
        StartDate = dto.StartDate?.Date;
        EndDate = dto.EndDate?.Date;
        Notes = dto.Notes?.Trim();
        CoveredSquareIdsCsv = string.Join(",", dto.CoveredSquareIds.Distinct());
        DistributorEmployeeIdsCsv = string.Join(",", dto.DistributorEmployeeIds.Distinct());
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
