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
    public decimal? CaloriesPerUnit { get; set; }
    public decimal? TotalCalories { get; set; }
    public string? Notes { get; set; }
}

public class MealDefinitionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public CateringMealType MealType { get; set; } = CateringMealType.Dry;
    public CateringMealStructureType StructureType { get; set; } = CateringMealStructureType.Product;
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string? ProductSkuName { get; set; }
    public string? ProductSkuNameEng { get; set; }
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
    public bool IsMealCaloriesRequired { get; set; }
    public decimal? MinMealCalories { get; set; }
    public decimal? MaxMealCalories { get; set; }
    public bool IsPackagingRequired { get; set; }
    public Guid? DefaultSourceWarehouseId { get; set; }
    public string? DefaultSourceWarehouseName { get; set; }
    public Guid? DefaultPackagingWarehouseId { get; set; }
    public string? DefaultPackagingWarehouseName { get; set; }
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
    public DateTime? PlannedArrivalTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public Guid? ReceivingSupervisorEmployeeId { get; set; }
    public string? ReceivingSupervisorName { get; set; }
    public Guid? TeamLeaderEmployeeId { get; set; }
    public string? TeamLeaderName { get; set; }
    public decimal VarianceQuantity { get; set; }
    public string? VarianceNotes { get; set; }
}

public class CateringDailyScheduleDto
{
    public Guid Id { get; set; }
    public Guid? CateringOperationalPlanId { get; set; }
    public Guid? CateringProjectId { get; set; }
    public Guid? CateringProjectDailyPlanId { get; set; }
    public Guid CateringContractId { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Today;
    public decimal PlannedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
    public CateringScheduleStatus Status { get; set; } = CateringScheduleStatus.Draft;
    public DateTime? PlannedPackagingStartTime { get; set; }
    public DateTime? PlannedPackagingEndTime { get; set; }
    public DateTime? PlannedLoadTime { get; set; }
    public DateTime? PlannedDepartureTime { get; set; }
    public DateTime? PlannedArrivalTime { get; set; }
    public decimal AllocationPlannedQuantity { get; set; }
    public decimal RemainingAllocationQuantity => PlannedQuantity - AllocationPlannedQuantity;
    public string? Notes { get; set; }
    public List<CateringSquareAllocationDto> Allocations { get; set; } = [];
    public CateringPackagingPlanDto? PackagingPlan { get; set; }
    public CateringDispatchPlanDto? DispatchPlan { get; set; }
    public List<CateringExecutionEventDto> ExecutionEvents { get; set; } = [];
}

public class CateringProjectContractLinkDto
{
    public Guid Id { get; set; }
    public Guid CateringProjectId { get; set; }
    public Guid CateringContractId { get; set; }
    public string? ContractNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerNameEng { get; set; }
    public DateTime ContractStartDate { get; set; }
    public DateTime ContractEndDate { get; set; }
    public decimal TotalContractedMealQuantity { get; set; }
}

public class CateringProjectSquareScopeDto
{
    public Guid Id { get; set; }
    public Guid CateringProjectId { get; set; }
    public Guid SquareId { get; set; }
    public string? SquareName { get; set; }
    public string? SquareNameEng { get; set; }
}

public class CateringProjectDailyPlanDto
{
    public Guid Id { get; set; }
    public Guid CateringProjectId { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Today;
    public decimal PlannedQuantity { get; set; }
    public decimal ScheduledQuantity { get; set; }
    public bool IsExtensionDay { get; set; }
    public CateringProjectDailyPlanStatus Status { get; set; } = CateringProjectDailyPlanStatus.Draft;
    public string? Notes { get; set; }
    public List<CateringDailyScheduleDto> ContractSchedules { get; set; } = [];
}

public class CateringProjectDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public CateringProjectStatus Status { get; set; } = CateringProjectStatus.Draft;
    public string? Notes { get; set; }
    public int ContractCount { get; set; }
    public int PlannedDays { get; set; }
    public int ScheduledDays { get; set; }
    public decimal TotalPlannedMeals { get; set; }
    public bool IsExtended { get; set; }
    public List<CateringProjectContractLinkDto> Contracts { get; set; } = [];
    public List<CateringProjectSquareScopeDto> Squares { get; set; } = [];
    public List<CateringProjectDailyPlanDto> DailyPlans { get; set; } = [];
    public List<CateringPlanResourceAssignmentDto> Resources { get; set; } = [];
}

public class CateringGenerateProjectDailyPlansRequestDto
{
    public Guid CateringProjectId { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal PlannedQuantity { get; set; }
    public bool UseProjectDuration { get; set; } = true;
    public bool AllowExtensionDays { get; set; }
}

public class CateringOperationalPlanDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid CateringContractId { get; set; }
    public string? ContractNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerNameEng { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public CateringPlanStatus Status { get; set; } = CateringPlanStatus.Draft;
    public string? Notes { get; set; }
    public List<CateringPlanResourceAssignmentDto> Resources { get; set; } = [];
}

public class CateringPlanResourceAssignmentDto
{
    public Guid Id { get; set; }
    public Guid CateringOperationalPlanId { get; set; }
    public CateringPlanResourceType ResourceType { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? VehicleId { get; set; }
    public string? VehicleName { get; set; }
    public string? PlateNumber { get; set; }
    public Guid? SquareId { get; set; }
    public string? SquareName { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
}

public class CateringGenerateDailyPlanRequestDto
{
    public Guid CateringOperationalPlanId { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal PlannedQuantity { get; set; }
    public bool UseFullContractDuration { get; set; }
}

public class CateringGenerateDailyPlanResultDto
{
    public int CreatedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<DateTime> CreatedDates { get; set; } = [];
    public List<DateTime> SkippedDates { get; set; } = [];
}

public class CateringInventoryRequestDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? CateringOperationalPlanId { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid? PackagingPlanId { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public string? SourceWarehouseName { get; set; }
    public Guid RequestedByEmployeeId { get; set; }
    public string RequestedByEmployeeName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.Today;
    public decimal PlannedMealCount { get; set; }
    public CateringInventoryRequestStatus Status { get; set; } = CateringInventoryRequestStatus.Draft;
    public string? InventoryReferenceIdsCsv { get; set; }
    public string? Notes { get; set; }
    public List<CateringInventoryRequestLineDto> Lines { get; set; } = [];
}

public class CateringInventoryRequestLineDto
{
    public Guid Id { get; set; }
    public Guid CateringInventoryRequestId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string ProductSkuName { get; set; } = string.Empty;
    public string? ProductSkuNameEng { get; set; }
    public decimal QuantityPerMeal { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public string? UnitName { get; set; }
    public string? Notes { get; set; }
}

public class CateringPackagingPlanDto
{
    public Guid Id { get; set; }
    public Guid DailyScheduleId { get; set; }
    public bool IsPackagingRequired { get; set; } = true;
    public Guid SourceWarehouseId { get; set; }
    public string? SourceWarehouseName { get; set; }
    public Guid? PackagingWarehouseId { get; set; }
    public string? PackagingWarehouseName { get; set; }
    public decimal RequiredMealCount { get; set; }
    public decimal StockReleasedMealCount { get; set; }
    public decimal PreparedMealCount { get; set; }
    public decimal RejectedMealCount { get; set; }
    public decimal DamagedMealCount { get; set; }
    public CateringPackagingStatus Status { get; set; } = CateringPackagingStatus.Planned;
    public DateTime? StockReleasedAt { get; set; }
    public DateTime? PreparationStartedAt { get; set; }
    public DateTime? PreparationCompletedAt { get; set; }
    public string? InventoryReferenceIdsCsv { get; set; }
    public string? VarianceReason { get; set; }
    public string? Notes { get; set; }
}

public class CateringDispatchPlanDto
{
    public Guid Id { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string? PlateNumber { get; set; }
    public Guid DriverEmployeeId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public Guid? FleetAssignmentId { get; set; }
    public bool IsFleetAssignmentManagedByCatering { get; set; }
    public decimal LoadedMealCount { get; set; }
    public CateringDispatchStatus Status { get; set; } = CateringDispatchStatus.Planned;
    public DateTime? PlannedLoadTime { get; set; }
    public DateTime? PlannedDepartureTime { get; set; }
    public DateTime? PlannedArrivalTime { get; set; }
    public DateTime? TruckArrivedForLoadingAt { get; set; }
    public DateTime? LoadedAt { get; set; }
    public DateTime? DepartedAt { get; set; }
    public DateTime? ArrivedAtDistributionAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}

public class CateringExecutionEventDto
{
    public Guid Id { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid? AllocationId { get; set; }
    public Guid? DispatchPlanId { get; set; }
    public CateringExecutionEventType EventType { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.Now;
    public decimal? Quantity { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? LocationText { get; set; }
    public string? Notes { get; set; }
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
    public decimal PreparedQuantity { get; set; }
    public decimal LoadedQuantity { get; set; }
    public int TodaySchedules { get; set; }
    public int PackagingInProgress { get; set; }
    public int TrucksPendingLoad { get; set; }
    public int TrucksInTransit { get; set; }
    public int LateDispatches { get; set; }
    public int BlockVariances { get; set; }
    public int OperationalExceptions { get; set; }
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
    public CateringScheduleStatus? ScheduleStatus { get; set; }
    public CateringPackagingStatus? PackagingStatus { get; set; }
    public CateringDispatchStatus? DispatchStatus { get; set; }
    public decimal ContractedQuantity { get; set; }
    public decimal ScheduledQuantity { get; set; }
    public decimal PreparedQuantity { get; set; }
    public decimal LoadedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal DistributedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
}
