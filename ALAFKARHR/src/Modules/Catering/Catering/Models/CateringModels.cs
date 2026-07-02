namespace Catering.Models;

public class MealDefinition : Aggregate<Guid>
{
    private readonly List<MealComponent> _components = [];

    public Guid CompanyId { get; private set; }
    public CateringMealType MealType { get; private set; }
    public CateringMealStructureType StructureType { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string? ProductSkuName { get; private set; }
    public string? ProductSkuNameEng { get; private set; }
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

    public void RecalculateCalories(IEnumerable<MealComponent> components, string userId)
    {
        var totalCalories = components
            .Where(x => !x.IsDeleted)
            .Sum(x => x.TotalCalories ?? 0m);

        Calories = totalCalories > 0m ? (int)Math.Round(totalCalories, MidpointRounding.AwayFromZero) : null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void Apply(MealDefinitionDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        MealType = dto.MealType;
        StructureType = dto.StructureType;
        ProductId = dto.StructureType == CateringMealStructureType.Product ? dto.ProductId : null;
        ProductSkuId = dto.StructureType == CateringMealStructureType.Product ? dto.ProductSkuId : null;
        ProductPackageId = dto.StructureType == CateringMealStructureType.Product ? dto.ProductPackageId : null;
        ProductSkuName = dto.StructureType == CateringMealStructureType.Product ? dto.ProductSkuName?.Trim() : null;
        ProductSkuNameEng = dto.StructureType == CateringMealStructureType.Product ? dto.ProductSkuNameEng?.Trim() : null;
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
        if (dto.StructureType == CateringMealStructureType.Product && (!dto.ProductSkuId.HasValue || dto.ProductSkuId.Value == Guid.Empty))
            throw new BadRequestException("Product meal requires a catalog SKU.");
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
    public decimal? CaloriesPerUnit { get; private set; }
    public decimal? TotalCalories { get; private set; }
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
            CaloriesPerUnit = dto.CaloriesPerUnit,
            TotalCalories = dto.CaloriesPerUnit.HasValue ? dto.QuantityPerMeal * dto.CaloriesPerUnit.Value : dto.TotalCalories,
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
    public bool IsMealCaloriesRequired { get; private set; }
    public decimal? MinMealCalories { get; private set; }
    public decimal? MaxMealCalories { get; private set; }
    public bool IsPackagingRequired { get; private set; }
    public Guid? DefaultSourceWarehouseId { get; private set; }
    public Guid? DefaultPackagingWarehouseId { get; private set; }
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
        IsMealCaloriesRequired = dto.IsMealCaloriesRequired;
        MinMealCalories = dto.IsMealCaloriesRequired ? dto.MinMealCalories : null;
        MaxMealCalories = dto.IsMealCaloriesRequired ? dto.MaxMealCalories : null;
        IsPackagingRequired = dto.IsPackagingRequired;
        DefaultSourceWarehouseId = dto.DefaultSourceWarehouseId;
        DefaultPackagingWarehouseId = dto.DefaultPackagingWarehouseId;
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
        if (dto.IsMealCaloriesRequired)
        {
            if (!dto.MinMealCalories.HasValue || !dto.MaxMealCalories.HasValue) throw new BadRequestException("Meal calorie range is required.");
            if (dto.MinMealCalories.Value <= 0 || dto.MaxMealCalories.Value <= 0) throw new BadRequestException("Meal calories must be greater than zero.");
            if (dto.MinMealCalories.Value > dto.MaxMealCalories.Value) throw new BadRequestException("Minimum meal calories cannot exceed maximum meal calories.");
        }
        if (dto.IsPackagingRequired && !dto.DefaultSourceWarehouseId.HasValue)
            throw new BadRequestException("Default source warehouse is required when packaging is required.");
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

    public Guid? CateringOperationalPlanId { get; private set; }
    public Guid? CateringProjectId { get; private set; }
    public Guid? CateringProjectDailyPlanId { get; private set; }
    public Guid CateringContractId { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public CateringScheduleStatus Status { get; private set; }
    public DateTime? PlannedPackagingStartTime { get; private set; }
    public DateTime? PlannedPackagingEndTime { get; private set; }
    public DateTime? PlannedLoadTime { get; private set; }
    public DateTime? PlannedDepartureTime { get; private set; }
    public DateTime? PlannedArrivalTime { get; private set; }
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
        CateringOperationalPlanId = dto.CateringOperationalPlanId;
        CateringProjectId = dto.CateringProjectId;
        CateringProjectDailyPlanId = dto.CateringProjectDailyPlanId;
        CateringContractId = dto.CateringContractId;
        ServiceDate = dto.ServiceDate.Date;
        PlannedQuantity = dto.PlannedQuantity;
        Status = dto.Status;
        PlannedPackagingStartTime = dto.PlannedPackagingStartTime;
        PlannedPackagingEndTime = dto.PlannedPackagingEndTime;
        PlannedLoadTime = dto.PlannedLoadTime;
        PlannedDepartureTime = dto.PlannedDepartureTime;
        PlannedArrivalTime = dto.PlannedArrivalTime;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void SetStatus(CateringScheduleStatus status, string userId)
    {
        Status = status;
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
    public DateTime? PlannedArrivalTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }
    public Guid? ReceivingSupervisorEmployeeId { get; private set; }
    public string? ReceivingSupervisorName { get; private set; }
    public Guid? TeamLeaderEmployeeId { get; private set; }
    public string? TeamLeaderName { get; private set; }
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
        PlannedArrivalTime = dto.PlannedArrivalTime;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void RecordActuals(decimal receivedQuantity, decimal distributedQuantity, DateTime? actualArrivalTime, Guid? supervisorEmployeeId, string? supervisorName, Guid? teamLeaderEmployeeId, string? teamLeaderName, string? varianceNotes, string userId)
    {
        if (receivedQuantity < 0 || distributedQuantity < 0) throw new BadRequestException("Quantities cannot be negative.");
        ReceivedQuantity = receivedQuantity;
        DistributedQuantity = distributedQuantity;
        ActualArrivalTime = actualArrivalTime;
        ReceivingSupervisorEmployeeId = supervisorEmployeeId;
        ReceivingSupervisorName = supervisorName?.Trim();
        TeamLeaderEmployeeId = teamLeaderEmployeeId;
        TeamLeaderName = teamLeaderName?.Trim();
        VarianceNotes = varianceNotes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringOperationalPlan : Entity<Guid>
{
    private readonly List<CateringPlanResourceAssignment> _resources = [];

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid CateringContractId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public CateringPlanStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<CateringPlanResourceAssignment> Resources => _resources.Where(x => !x.IsDeleted).ToList();

    private CateringOperationalPlan() { }

    public static CateringOperationalPlan Create(CateringOperationalPlanDto dto, string userId)
    {
        var plan = new CateringOperationalPlan { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        plan.Update(dto, userId);
        return plan;
    }

    public void Update(CateringOperationalPlanDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (dto.CateringContractId == Guid.Empty) throw new BadRequestException("Catering contract is required.");
        if (dto.EndDate.Date < dto.StartDate.Date) throw new BadRequestException("Plan end date cannot be before start date.");
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        CateringContractId = dto.CateringContractId;
        StartDate = dto.StartDate.Date;
        EndDate = dto.EndDate.Date;
        Status = dto.Status;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }
}

public class CateringPlanResourceAssignment : Entity<Guid>
{
    public Guid CateringOperationalPlanId { get; private set; }
    public CateringPlanResourceType ResourceType { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public string? EmployeeName { get; private set; }
    public Guid? VehicleId { get; private set; }
    public string? VehicleName { get; private set; }
    public string? PlateNumber { get; private set; }
    public Guid? SquareId { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public string? Notes { get; private set; }

    private CateringPlanResourceAssignment() { }

    public static CateringPlanResourceAssignment Create(Guid planId, CateringPlanResourceAssignmentDto dto, string userId)
    {
        var assignment = new CateringPlanResourceAssignment { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        assignment.Update(planId, dto, userId);
        return assignment;
    }

    public void Update(Guid planId, CateringPlanResourceAssignmentDto dto, string userId)
    {
        if (planId == Guid.Empty) throw new BadRequestException("Operational plan is required.");
        if (dto.ResourceType == CateringPlanResourceType.Truck && (!dto.VehicleId.HasValue || dto.VehicleId.Value == Guid.Empty))
            throw new BadRequestException("Truck resource requires a vehicle.");
        if (dto.ResourceType != CateringPlanResourceType.Truck && (!dto.EmployeeId.HasValue || dto.EmployeeId.Value == Guid.Empty))
            throw new BadRequestException("People resources require an employee.");
        if (dto.EffectiveTo.HasValue && dto.EffectiveFrom.HasValue && dto.EffectiveTo.Value.Date < dto.EffectiveFrom.Value.Date)
            throw new BadRequestException("Resource effective end date cannot be before start date.");
        CateringOperationalPlanId = planId;
        ResourceType = dto.ResourceType;
        EmployeeId = dto.ResourceType == CateringPlanResourceType.Truck ? null : dto.EmployeeId;
        EmployeeName = dto.ResourceType == CateringPlanResourceType.Truck ? null : dto.EmployeeName?.Trim();
        VehicleId = dto.ResourceType == CateringPlanResourceType.Truck ? dto.VehicleId : null;
        VehicleName = dto.ResourceType == CateringPlanResourceType.Truck ? dto.VehicleName?.Trim() : null;
        PlateNumber = dto.ResourceType == CateringPlanResourceType.Truck ? dto.PlateNumber?.Trim() : null;
        SquareId = dto.SquareId;
        EffectiveFrom = dto.EffectiveFrom?.Date;
        EffectiveTo = dto.EffectiveTo?.Date;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringProject : Entity<Guid>
{
    private readonly List<CateringProjectContractLink> _contracts = [];
    private readonly List<CateringProjectSquareScope> _squares = [];
    private readonly List<CateringProjectDailyPlan> _dailyPlans = [];

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string ProjectName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public CateringProjectStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<CateringProjectContractLink> Contracts => _contracts.Where(x => !x.IsDeleted).ToList();
    public IReadOnlyCollection<CateringProjectSquareScope> Squares => _squares.Where(x => !x.IsDeleted).ToList();
    public IReadOnlyCollection<CateringProjectDailyPlan> DailyPlans => _dailyPlans.Where(x => !x.IsDeleted).ToList();

    private CateringProject() { }

    public static CateringProject Create(CateringProjectDto dto, string userId)
    {
        var project = new CateringProject { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        project.Update(dto, userId);
        return project;
    }

    public void Update(CateringProjectDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.ProjectName)) throw new BadRequestException("Project name is required.");
        if (dto.EndDate.Date < dto.StartDate.Date) throw new BadRequestException("Project end date cannot be before start date.");
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        ProjectName = dto.ProjectName.Trim();
        StartDate = dto.StartDate.Date;
        EndDate = dto.EndDate.Date;
        Status = dto.Status;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }
}

public class CateringProjectContractLink : Entity<Guid>
{
    public Guid CateringProjectId { get; private set; }
    public Guid CateringContractId { get; private set; }

    private CateringProjectContractLink() { }

    public static CateringProjectContractLink Create(Guid projectId, Guid contractId, string userId)
    {
        if (projectId == Guid.Empty) throw new BadRequestException("Catering project is required.");
        if (contractId == Guid.Empty) throw new BadRequestException("Catering contract is required.");
        return new CateringProjectContractLink
        {
            Id = Guid.NewGuid(),
            CateringProjectId = projectId,
            CateringContractId = contractId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class CateringProjectSquareScope : Entity<Guid>
{
    public Guid CateringProjectId { get; private set; }
    public Guid SquareId { get; private set; }

    private CateringProjectSquareScope() { }

    public static CateringProjectSquareScope Create(Guid projectId, Guid squareId, string userId)
    {
        if (projectId == Guid.Empty) throw new BadRequestException("Catering project is required.");
        if (squareId == Guid.Empty) throw new BadRequestException("Catering square is required.");
        return new CateringProjectSquareScope
        {
            Id = Guid.NewGuid(),
            CateringProjectId = projectId,
            SquareId = squareId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class CateringProjectDailyPlan : Entity<Guid>
{
    public Guid CateringProjectId { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public CateringProjectDailyPlanStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private CateringProjectDailyPlan() { }

    public static CateringProjectDailyPlan Create(CateringProjectDailyPlanDto dto, string userId)
    {
        var plan = new CateringProjectDailyPlan { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        plan.Update(dto, userId);
        return plan;
    }

    public void Update(CateringProjectDailyPlanDto dto, string userId)
    {
        if (dto.CateringProjectId == Guid.Empty) throw new BadRequestException("Catering project is required.");
        if (dto.PlannedQuantity <= 0) throw new BadRequestException("Project daily planned quantity must be greater than zero.");
        CateringProjectId = dto.CateringProjectId;
        ServiceDate = dto.ServiceDate.Date;
        PlannedQuantity = dto.PlannedQuantity;
        Status = dto.Status;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringPackagingPlan : Entity<Guid>
{
    public Guid DailyScheduleId { get; private set; }
    public bool IsPackagingRequired { get; private set; }
    public Guid SourceWarehouseId { get; private set; }
    public Guid? PackagingWarehouseId { get; private set; }
    public decimal RequiredMealCount { get; private set; }
    public decimal StockReleasedMealCount { get; private set; }
    public decimal PreparedMealCount { get; private set; }
    public decimal RejectedMealCount { get; private set; }
    public decimal DamagedMealCount { get; private set; }
    public CateringPackagingStatus Status { get; private set; }
    public DateTime? StockReleasedAt { get; private set; }
    public DateTime? PreparationStartedAt { get; private set; }
    public DateTime? PreparationCompletedAt { get; private set; }
    public string? InventoryReferenceIdsCsv { get; private set; }
    public string? VarianceReason { get; private set; }
    public string? Notes { get; private set; }

    private CateringPackagingPlan() { }

    public static CateringPackagingPlan Create(CateringPackagingPlanDto dto, string userId)
    {
        var plan = new CateringPackagingPlan { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        plan.UpdatePlan(dto, userId);
        return plan;
    }

    public void UpdatePlan(CateringPackagingPlanDto dto, string userId)
    {
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        if (dto.IsPackagingRequired && dto.SourceWarehouseId == Guid.Empty) throw new BadRequestException("Source warehouse is required.");
        if (dto.RequiredMealCount <= 0) throw new BadRequestException("Required meal count must be greater than zero.");
        DailyScheduleId = dto.DailyScheduleId;
        IsPackagingRequired = dto.IsPackagingRequired;
        SourceWarehouseId = dto.SourceWarehouseId;
        PackagingWarehouseId = dto.PackagingWarehouseId;
        RequiredMealCount = dto.RequiredMealCount;
        Status = dto.IsPackagingRequired ? dto.Status : CateringPackagingStatus.NotRequired;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void MarkStockReleased(decimal mealCount, IEnumerable<Guid> inventoryReferenceIds, string userId)
    {
        if (!IsPackagingRequired) throw new BadRequestException("Packaging is not required for this schedule.");
        if (mealCount <= 0 || mealCount > RequiredMealCount) throw new BadRequestException("Released meal count must be within the required meal count.");
        StockReleasedMealCount = mealCount;
        InventoryReferenceIdsCsv = string.Join(",", inventoryReferenceIds.Distinct());
        StockReleasedAt = DateTime.UtcNow;
        Status = CateringPackagingStatus.StockReleased;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void StartPreparation(string userId)
    {
        if (Status < CateringPackagingStatus.StockReleased) throw new BadRequestException("Release stock before starting packaging.");
        PreparationStartedAt = DateTime.UtcNow;
        Status = CateringPackagingStatus.InProgress;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Complete(decimal prepared, decimal rejected, decimal damaged, string? varianceReason, string userId)
    {
        if (prepared < 0 || rejected < 0 || damaged < 0) throw new BadRequestException("Packaging quantities cannot be negative.");
        if (prepared + rejected + damaged > StockReleasedMealCount) throw new BadRequestException("Prepared, rejected, and damaged quantities cannot exceed released stock.");
        if (prepared < RequiredMealCount && string.IsNullOrWhiteSpace(varianceReason)) throw new BadRequestException("Variance reason is required when prepared meals are below the required count.");
        PreparedMealCount = prepared;
        RejectedMealCount = rejected;
        DamagedMealCount = damaged;
        VarianceReason = varianceReason?.Trim();
        PreparationCompletedAt = DateTime.UtcNow;
        Status = prepared >= RequiredMealCount ? CateringPackagingStatus.Completed : CateringPackagingStatus.Exception;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringInventoryRequest : Entity<Guid>
{
    private readonly List<CateringInventoryRequestLine> _lines = [];

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? CateringOperationalPlanId { get; private set; }
    public Guid DailyScheduleId { get; private set; }
    public Guid? PackagingPlanId { get; private set; }
    public Guid SourceWarehouseId { get; private set; }
    public Guid RequestedByEmployeeId { get; private set; }
    public string RequestedByEmployeeName { get; private set; } = string.Empty;
    public DateTime RequestDate { get; private set; }
    public decimal PlannedMealCount { get; private set; }
    public CateringInventoryRequestStatus Status { get; private set; }
    public string? InventoryReferenceIdsCsv { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<CateringInventoryRequestLine> Lines => _lines.Where(x => !x.IsDeleted).ToList();

    private CateringInventoryRequest() { }

    public static CateringInventoryRequest Create(CateringInventoryRequestDto dto, IEnumerable<CateringInventoryRequestLineDto> lines, string userId)
    {
        Validate(dto);
        var request = new CateringInventoryRequest
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            CateringOperationalPlanId = dto.CateringOperationalPlanId,
            DailyScheduleId = dto.DailyScheduleId,
            PackagingPlanId = dto.PackagingPlanId,
            SourceWarehouseId = dto.SourceWarehouseId,
            RequestedByEmployeeId = dto.RequestedByEmployeeId,
            RequestedByEmployeeName = dto.RequestedByEmployeeName.Trim(),
            RequestDate = dto.RequestDate.Date,
            PlannedMealCount = dto.PlannedMealCount,
            Status = dto.Status == CateringInventoryRequestStatus.Draft ? CateringInventoryRequestStatus.Draft : dto.Status,
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        foreach (var line in lines)
        {
            request._lines.Add(CateringInventoryRequestLine.Create(request.Id, line, userId));
        }

        if (!request._lines.Any()) throw new BadRequestException("Inventory request must have at least one line.");
        return request;
    }

    public void Submit(string userId)
    {
        if (Status != CateringInventoryRequestStatus.Draft) throw new BadRequestException("Only draft requests can be submitted.");
        Status = CateringInventoryRequestStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Approve(string userId)
    {
        if (Status != CateringInventoryRequestStatus.Submitted) throw new BadRequestException("Only submitted requests can be approved.");
        Status = CateringInventoryRequestStatus.Approved;
        foreach (var line in _lines.Where(x => !x.IsDeleted))
        {
            line.Approve(line.RequiredQuantity, userId);
        }
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Fulfill(IEnumerable<Guid> inventoryReferenceIds, string userId)
    {
        if (Status != CateringInventoryRequestStatus.Approved) throw new BadRequestException("Only approved requests can be fulfilled.");
        InventoryReferenceIdsCsv = string.Join(",", inventoryReferenceIds.Distinct());
        Status = CateringInventoryRequestStatus.Fulfilled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(CateringInventoryRequestDto dto)
    {
        if (dto.CompanyId == Guid.Empty) throw new BadRequestException("Company is required.");
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        if (dto.SourceWarehouseId == Guid.Empty) throw new BadRequestException("Source warehouse is required.");
        if (dto.RequestedByEmployeeId == Guid.Empty) throw new BadRequestException("Requested by employee is required.");
        if (string.IsNullOrWhiteSpace(dto.RequestedByEmployeeName)) throw new BadRequestException("Requested by employee name is required.");
        if (dto.PlannedMealCount <= 0) throw new BadRequestException("Planned meal count must be greater than zero.");
    }
}

public class CateringInventoryRequestLine : Entity<Guid>
{
    public Guid CateringInventoryRequestId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string ProductSkuName { get; private set; } = string.Empty;
    public string? ProductSkuNameEng { get; private set; }
    public decimal QuantityPerMeal { get; private set; }
    public decimal RequiredQuantity { get; private set; }
    public decimal ApprovedQuantity { get; private set; }
    public string? UnitName { get; private set; }
    public string? Notes { get; private set; }

    private CateringInventoryRequestLine() { }

    public static CateringInventoryRequestLine Create(Guid requestId, CateringInventoryRequestLineDto dto, string userId)
    {
        if (requestId == Guid.Empty) throw new BadRequestException("Inventory request is required.");
        if (dto.ProductSkuId == Guid.Empty) throw new BadRequestException("Product SKU is required.");
        if (string.IsNullOrWhiteSpace(dto.ProductSkuName)) throw new BadRequestException("Product SKU name is required.");
        if (dto.RequiredQuantity <= 0) throw new BadRequestException("Required quantity must be greater than zero.");
        return new CateringInventoryRequestLine
        {
            Id = Guid.NewGuid(),
            CateringInventoryRequestId = requestId,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            ProductSkuName = dto.ProductSkuName.Trim(),
            ProductSkuNameEng = dto.ProductSkuNameEng?.Trim(),
            QuantityPerMeal = dto.QuantityPerMeal,
            RequiredQuantity = dto.RequiredQuantity,
            ApprovedQuantity = dto.ApprovedQuantity > 0 ? dto.ApprovedQuantity : dto.RequiredQuantity,
            UnitName = dto.UnitName?.Trim(),
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Approve(decimal quantity, string userId)
    {
        if (quantity <= 0 || quantity > RequiredQuantity) throw new BadRequestException("Approved quantity must be within required quantity.");
        ApprovedQuantity = quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringDispatchPlan : Entity<Guid>
{
    public Guid DailyScheduleId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string VehicleName { get; private set; } = string.Empty;
    public string? PlateNumber { get; private set; }
    public Guid DriverEmployeeId { get; private set; }
    public string DriverName { get; private set; } = string.Empty;
    public Guid? FleetAssignmentId { get; private set; }
    public bool IsFleetAssignmentManagedByCatering { get; private set; }
    public decimal LoadedMealCount { get; private set; }
    public CateringDispatchStatus Status { get; private set; }
    public DateTime? PlannedLoadTime { get; private set; }
    public DateTime? PlannedDepartureTime { get; private set; }
    public DateTime? PlannedArrivalTime { get; private set; }
    public DateTime? TruckArrivedForLoadingAt { get; private set; }
    public DateTime? LoadedAt { get; private set; }
    public DateTime? DepartedAt { get; private set; }
    public DateTime? ArrivedAtDistributionAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    private CateringDispatchPlan() { }

    public static CateringDispatchPlan Create(CateringDispatchPlanDto dto, string userId)
    {
        var plan = new CateringDispatchPlan { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        plan.UpdatePlan(dto, userId);
        return plan;
    }

    public void UpdatePlan(CateringDispatchPlanDto dto, string userId)
    {
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        if (dto.VehicleId == Guid.Empty) throw new BadRequestException("Vehicle is required.");
        if (dto.DriverEmployeeId == Guid.Empty) throw new BadRequestException("Driver is required.");
        if (string.IsNullOrWhiteSpace(dto.VehicleName)) throw new BadRequestException("Vehicle name is required.");
        if (string.IsNullOrWhiteSpace(dto.DriverName)) throw new BadRequestException("Driver name is required.");
        DailyScheduleId = dto.DailyScheduleId;
        VehicleId = dto.VehicleId;
        VehicleName = dto.VehicleName.Trim();
        PlateNumber = dto.PlateNumber?.Trim();
        DriverEmployeeId = dto.DriverEmployeeId;
        DriverName = dto.DriverName.Trim();
        FleetAssignmentId = dto.FleetAssignmentId;
        IsFleetAssignmentManagedByCatering = dto.IsFleetAssignmentManagedByCatering;
        PlannedLoadTime = dto.PlannedLoadTime;
        PlannedDepartureTime = dto.PlannedDepartureTime;
        PlannedArrivalTime = dto.PlannedArrivalTime;
        Status = dto.Status;
        Notes = dto.Notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void AttachFleetAssignment(Guid assignmentId, string userId)
    {
        FleetAssignmentId = assignmentId;
        IsFleetAssignmentManagedByCatering = true;
        Status = CateringDispatchStatus.VehicleAssigned;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Record(CateringExecutionEventType eventType, decimal? quantity, string userId)
    {
        var now = DateTime.UtcNow;
        switch (eventType)
        {
            case CateringExecutionEventType.TruckArrivedForLoading:
                TruckArrivedForLoadingAt = now;
                Status = CateringDispatchStatus.ArrivedForLoading;
                break;
            case CateringExecutionEventType.TruckLoaded:
                if (!quantity.HasValue || quantity.Value <= 0) throw new BadRequestException("Loaded meal count is required.");
                LoadedMealCount = quantity.Value;
                LoadedAt = now;
                Status = CateringDispatchStatus.Loaded;
                break;
            case CateringExecutionEventType.TruckDeparted:
                if (LoadedMealCount <= 0) throw new BadRequestException("Load the truck before departure.");
                DepartedAt = now;
                Status = CateringDispatchStatus.Departed;
                break;
            case CateringExecutionEventType.TruckArrivedAtDistribution:
                if (!DepartedAt.HasValue) throw new BadRequestException("Record departure before distribution arrival.");
                ArrivedAtDistributionAt = now;
                Status = CateringDispatchStatus.ArrivedAtDistribution;
                break;
            default:
                throw new BadRequestException("Unsupported dispatch event.");
        }
        ModifiedAt = now;
        ModifiedBy = userId;
    }

    public void Complete(string userId)
    {
        CompletedAt = DateTime.UtcNow;
        Status = CateringDispatchStatus.Completed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class CateringExecutionEvent : Entity<Guid>
{
    public Guid DailyScheduleId { get; private set; }
    public Guid? AllocationId { get; private set; }
    public Guid? DispatchPlanId { get; private set; }
    public CateringExecutionEventType EventType { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public decimal? Quantity { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public string? EmployeeName { get; private set; }
    public string? LocationText { get; private set; }
    public string? Notes { get; private set; }

    private CateringExecutionEvent() { }

    public static CateringExecutionEvent Create(CateringExecutionEventDto dto, string userId)
    {
        if (dto.DailyScheduleId == Guid.Empty) throw new BadRequestException("Daily schedule is required.");
        return new CateringExecutionEvent
        {
            Id = Guid.NewGuid(),
            DailyScheduleId = dto.DailyScheduleId,
            AllocationId = dto.AllocationId,
            DispatchPlanId = dto.DispatchPlanId,
            EventType = dto.EventType,
            OccurredAt = dto.OccurredAt == default ? DateTime.UtcNow : dto.OccurredAt,
            Quantity = dto.Quantity,
            EmployeeId = dto.EmployeeId,
            EmployeeName = dto.EmployeeName?.Trim(),
            LocationText = dto.LocationText?.Trim(),
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
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
