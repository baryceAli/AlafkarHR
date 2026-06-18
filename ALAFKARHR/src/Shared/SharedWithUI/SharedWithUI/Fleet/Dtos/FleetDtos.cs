using System.ComponentModel.DataAnnotations;
using SharedWithUI.Fleet.Enums;
using SharedWithUI.Maintenance.Dtos;

namespace SharedWithUI.Fleet.Dtos;

public class FleetVehicleDto
{
    public Guid Id { get; set; }
    public string VehicleCode { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string? Vin { get; set; }
    public string? EngineNumber { get; set; }
    public FleetVehicleType VehicleType { get; set; }
    public FleetVehicleStatus Status { get; set; }
    public FleetVehicleOwnershipType OwnershipType { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? MaintenanceAssetId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? RentalContractId { get; set; }
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DailyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public int? AllowedKilometers { get; set; }
    public decimal? ExcessKilometerRate { get; set; }
    public int CurrentOdometer { get; set; }
    public FleetFuelType FuelType { get; set; }
    public decimal? FuelCapacity { get; set; }
    public Guid? DefaultDriverEmployeeId { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateFleetVehicleDto
{
    public string? VehicleCode { get; set; }

    [Required(ErrorMessage = "Plate number is required")]
    public string PlateNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "English name is required")]
    public string NameEng { get; set; } = string.Empty;

    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string? Vin { get; set; }
    public string? EngineNumber { get; set; }
    public FleetVehicleType VehicleType { get; set; } = FleetVehicleType.Sedan;
    public FleetVehicleStatus Status { get; set; } = FleetVehicleStatus.Active;
    public FleetVehicleOwnershipType OwnershipType { get; set; } = FleetVehicleOwnershipType.Owned;

    [Required(ErrorMessage = "Company is required")]
    public Guid CompanyId { get; set; }

    public Guid? BranchId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? RentalContractId { get; set; }
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DailyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public int? AllowedKilometers { get; set; }
    public decimal? ExcessKilometerRate { get; set; }
    public int CurrentOdometer { get; set; }
    public FleetFuelType FuelType { get; set; } = FleetFuelType.Petrol;
    public decimal? FuelCapacity { get; set; }
    public Guid? DefaultDriverEmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFleetVehicleDto : CreateFleetVehicleDto
{
    public Guid Id { get; set; }
}

public class FleetVehicleFilterDto
{
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public FleetVehicleOwnershipType? OwnershipType { get; set; }
    public FleetVehicleStatus? Status { get; set; }
    public FleetVehicleType? VehicleType { get; set; }
}

public class FleetVehicleAssignmentDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Purpose { get; set; }
    public int? OdometerOut { get; set; }
    public int? OdometerIn { get; set; }
    public decimal? FuelLevelOut { get; set; }
    public decimal? FuelLevelIn { get; set; }
    public FleetAssignmentStatus Status { get; set; }
}

public class CreateFleetVehicleAssignmentDto
{
    [Required(ErrorMessage = "Vehicle is required")]
    public Guid VehicleId { get; set; }

    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EndDate { get; set; }
    public string? Purpose { get; set; }
    public int? OdometerOut { get; set; }
    public decimal? FuelLevelOut { get; set; }
}

public class ReturnFleetVehicleAssignmentDto
{
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow.Date;
    public int? OdometerIn { get; set; }
    public decimal? FuelLevelIn { get; set; }
    public string? Notes { get; set; }
}

public class FleetVehicleDocumentDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public FleetDocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? RenewalCost { get; set; }
    public Guid? SupplierId { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public FleetDocumentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateFleetVehicleDocumentDto
{
    [Required(ErrorMessage = "Vehicle is required")]
    public Guid VehicleId { get; set; }

    public FleetDocumentType DocumentType { get; set; } = FleetDocumentType.Registration;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? RenewalCost { get; set; }
    public Guid? SupplierId { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFleetVehicleDocumentDto : CreateFleetVehicleDocumentDto
{
    public Guid Id { get; set; }
}

public class RenewFleetVehicleDocumentDto
{
    public DateTime? IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.Date.AddYears(1);
    public decimal? RenewalCost { get; set; }
    public Guid? SupplierId { get; set; }
    public string? Notes { get; set; }
}

public class FleetVehicleExpenseDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public FleetExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? SupplierId { get; set; }
    public string? VendorName { get; set; }
    public int? Odometer { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }
    public Guid? ContractId { get; set; }
    public Guid? ProcurementDocumentId { get; set; }
    public Guid? PaymentReferenceId { get; set; }
    public FleetExpenseApprovalStatus ApprovalStatus { get; set; }
}

public class CreateFleetVehicleExpenseDto
{
    [Required(ErrorMessage = "Vehicle is required")]
    public Guid VehicleId { get; set; }

    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow.Date;
    public FleetExpenseCategory Category { get; set; } = FleetExpenseCategory.Fuel;
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? SupplierId { get; set; }
    public string? VendorName { get; set; }
    public int? Odometer { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }
    public Guid? ContractId { get; set; }
    public Guid? ProcurementDocumentId { get; set; }
    public Guid? PaymentReferenceId { get; set; }
}

public class UpdateFleetVehicleExpenseDto : CreateFleetVehicleExpenseDto
{
    public Guid Id { get; set; }
}

public class ApproveFleetVehicleExpenseDto
{
    public bool IsApproved { get; set; }
    public string? Notes { get; set; }
}

public class FleetVehicleServiceRuleDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public FleetServiceType ServiceType { get; set; }
    public int? IntervalKilometers { get; set; }
    public int? IntervalDays { get; set; }
    public int? LastServiceOdometer { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public int? NextDueOdometer { get; set; }
    public DateTime? NextDueDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDue { get; set; }
    public string? Notes { get; set; }
}

public class CreateFleetVehicleServiceRuleDto
{
    [Required(ErrorMessage = "Vehicle is required")]
    public Guid VehicleId { get; set; }

    public FleetServiceType ServiceType { get; set; } = FleetServiceType.OilChange;
    public int? IntervalKilometers { get; set; }
    public int? IntervalDays { get; set; }
    public int? LastServiceOdometer { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateFleetVehicleServiceRuleDto : CreateFleetVehicleServiceRuleDto
{
    public Guid Id { get; set; }
}

public class FleetDashboardDto
{
    public int TotalVehicles { get; set; }
    public int OwnedVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int ActiveAssignments { get; set; }
    public int VehiclesUnderMaintenance { get; set; }
    public int ExpiringDocuments { get; set; }
    public int OverdueServices { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public List<FleetVehicleDocumentDto> ExpiringDocumentList { get; set; } = [];
    public List<FleetVehicleServiceRuleDto> DueServiceRules { get; set; } = [];
}

public class FleetVehicleDetailsDto
{
    public FleetVehicleDto Vehicle { get; set; } = new();
    public List<FleetVehicleAssignmentDto> Assignments { get; set; } = [];
    public List<FleetVehicleDocumentDto> Documents { get; set; } = [];
    public List<FleetVehicleExpenseDto> Expenses { get; set; } = [];
    public List<FleetVehicleServiceRuleDto> ServiceRules { get; set; } = [];
    public List<MaintenanceWorkOrderDto> MaintenanceWorkOrders { get; set; } = [];
}

public class FleetSummaryReportDto
{
    public decimal TotalExpenses { get; set; }
    public decimal MaintenanceExpenses { get; set; }
    public decimal RentedVehicleExpenses { get; set; }
    public List<FleetExpenseCategorySummaryDto> ExpensesByCategory { get; set; } = [];
    public List<FleetVehicleExpenseSummaryDto> ExpensesByVehicle { get; set; } = [];
}

public class FleetExpenseCategorySummaryDto
{
    public FleetExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
}

public class FleetVehicleExpenseSummaryDto
{
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
