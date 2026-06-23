using SharedWithUI.Catering.Enums;

namespace SharedWithUI.Catering.Dtos;

public class MealComponentDto
{
    public Guid Id { get; set; }
    public Guid MealDefinitionId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string? ComponentNameEng { get; set; }
    public decimal QuantityPerMeal { get; set; }
    public string? UnitName { get; set; }
    public string? Notes { get; set; }
}

public class MealDefinitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public CateringMealType MealType { get; set; } = CateringMealType.Dry;
    public string Name { get; set; } = string.Empty;
    public string? NameEng { get; set; }
    public int? Calories { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public List<MealComponentDto> Components { get; set; } = [];
}

public class CateringContractAddendumDto
{
    public Guid Id { get; set; }
    public Guid CateringContractId { get; set; }
    public decimal AddedQuantity { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid? AttachmentDocumentId { get; set; }
}

public class CateringContractDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNameEng { get; set; }
    public Guid? GenericContractId { get; set; }
    public CateringServiceType ServiceType { get; set; } = CateringServiceType.Ramadan;
    public string SeasonLabel { get; set; } = string.Empty;
    public int? RamadanYear { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public decimal ContractedMealQuantity { get; set; }
    public decimal AddendumMealQuantity { get; set; }
    public decimal TotalContractedMealQuantity { get; set; }
    public Guid MealDefinitionId { get; set; }
    public string? MealName { get; set; }
    public string? MealNameEng { get; set; }
    public CateringContractStatus Status { get; set; } = CateringContractStatus.Draft;
    public string? Notes { get; set; }
    public List<CateringContractAddendumDto> Addendums { get; set; } = [];
}

public class CateringAreaDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEng { get; set; }
    public string? GenderGroup { get; set; }
    public string? LocationText { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CateringSquareDto
{
    public Guid Id { get; set; }
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string? AreaNameEng { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEng { get; set; }
    public string? LocationText { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class CateringSquareAllocationDto
{
    public Guid Id { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid SquareId { get; set; }
    public string SquareName { get; set; } = string.Empty;
    public string? SquareNameEng { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
    public string? VarianceNotes { get; set; }
}

public class CateringDailyScheduleDto
{
    public Guid Id { get; set; }
    public Guid CateringContractId { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Today;
    public decimal PlannedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
    public string? Notes { get; set; }
    public List<CateringSquareAllocationDto> Allocations { get; set; } = [];
}

public class CateringVehicleDeliveryDto
{
    public Guid Id { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string? PlateNumber { get; set; }
    public Guid DriverEmployeeId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public Guid ReceivingSupervisorEmployeeId { get; set; }
    public string ReceivingSupervisorName { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; } = DateTime.Now;
    public decimal ReceivedQuantity { get; set; }
    public string? Notes { get; set; }
}

public class CateringAssignmentDto
{
    public Guid Id { get; set; }
    public Guid CateringContractId { get; set; }
    public CateringAssignmentRole Role { get; set; }
    public Guid? SquareId { get; set; }
    public string? SquareName { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public List<Guid> CoveredSquareIds { get; set; } = [];
    public List<Guid> DistributorEmployeeIds { get; set; } = [];
}

public class CateringDashboardDto
{
    public int ActiveContracts { get; set; }
    public int ActiveMeals { get; set; }
    public int ActiveSquares { get; set; }
    public decimal ContractedQuantity { get; set; }
    public decimal ScheduledQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
}

public class CateringReportRowDto
{
    public string PeriodKey { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public Guid? ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? SquareId { get; set; }
    public string? SquareName { get; set; }
    public Guid? VehicleId { get; set; }
    public string? VehicleName { get; set; }
    public decimal ContractedQuantity { get; set; }
    public decimal ScheduledQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
}
