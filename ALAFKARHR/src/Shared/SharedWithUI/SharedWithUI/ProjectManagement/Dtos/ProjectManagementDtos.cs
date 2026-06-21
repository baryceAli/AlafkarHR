using SharedWithUI.ProjectManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.ProjectManagement.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string ProjectNumber { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string NameEng { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public ProjectType Type { get; set; } = ProjectType.FoodPreparationDistribution;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public Guid? ManagerUserId { get; set; }
    public string? ManagerName { get; set; }
    public Guid? SourceOrderId { get; set; }
    public string? SourceOrderNumber { get; set; }
    public string? SourceOrderType { get; set; }
    public DateTime PlannedStartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime PlannedEndDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? Notes { get; set; }
    public decimal TotalPlannedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal CostVariance => TotalActualCost - TotalPlannedCost;
    public List<ProjectCustomerDto> Customers { get; set; } = [];
    public List<ProjectDeliverableDto> Deliverables { get; set; } = [];
    public List<ProjectDistributionScheduleDto> DistributionSchedules { get; set; } = [];
    public List<ProjectMaterialRequirementDto> MaterialRequirements { get; set; } = [];
    public List<ProjectResourceDto> Resources { get; set; } = [];
    public List<ProjectExpenseDto> Expenses { get; set; } = [];
    public List<ProjectHandoffDto> Handoffs { get; set; } = [];
    public List<ProjectTaskLinkDto> TaskLinks { get; set; } = [];
}

public class ProjectCustomerDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNameEng { get; set; }
    public Guid? SourceOrderId { get; set; }
    public string? SourceOrderNumber { get; set; }
    public decimal ContractedQuantity { get; set; }
    public decimal ContractedAmount { get; set; }
    public string? Notes { get; set; }
    public List<ProjectCustomerProductPlanDto> ProductPlans { get; set; } = [];
}

public class ProjectCustomerProductPlanDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ProjectCustomerId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductId { get; set; }
    public string ProductSkuName { get; set; } = string.Empty;
    public string? ProductSkuNameEng { get; set; }
    public string? SkuCode { get; set; }
    public string? SkuCodeEng { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string? PackageName { get; set; }
    public string? PackageNameEng { get; set; }
    public decimal Quantity { get; set; } = 1;
    public string? Notes { get; set; }
}

public class ProjectDeliverableDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductId { get; set; }
    public string ProductSkuName { get; set; } = string.Empty;
    public string? ProductSkuNameEng { get; set; }
    public MealHandlingType HandlingType { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public string? Notes { get; set; }
}

public class DistributionPlaceDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string NameEng { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProjectDistributionScheduleDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime DistributionDate { get; set; } = DateTime.UtcNow.Date;
    public TimeOnly? WindowStart { get; set; }
    public TimeOnly? WindowEnd { get; set; }
    public DistributionStatus Status { get; set; } = DistributionStatus.Scheduled;
    public string? Notes { get; set; }
    public List<ProjectDistributionAllocationDto> Allocations { get; set; } = [];
}

public class ProjectDistributionAllocationDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime DistributionDate { get; set; }
    public Guid ProjectCustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid DeliverableId { get; set; }
    public string DeliverableName { get; set; } = string.Empty;
    public Guid DistributionPlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal VarianceQuantity => ActualQuantity - PlannedQuantity;
    public string? Notes { get; set; }
}

public class ProjectMaterialRequirementDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid DeliverableId { get; set; }
    public Guid ComponentProductSkuId { get; set; }
    public string ComponentSkuName { get; set; } = string.Empty;
    public string? ComponentSkuNameEng { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
}

public class ProjectResourceDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectResourceType ResourceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; } = 1;
    public decimal PlannedRate { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal ActualRate { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? Notes { get; set; }
    public decimal PlannedCost => PlannedQuantity * PlannedRate;
    public decimal ActualCost => ActualQuantity * ActualRate;
}

public class ProjectExpenseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectExpenseCategory Category { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow.Date;
    public string Description { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? Notes { get; set; }
}

public class ProjectBudgetSummaryDto
{
    public Guid ProjectId { get; set; }
    public decimal PlannedResourceCost { get; set; }
    public decimal ActualResourceCost { get; set; }
    public decimal PlannedExpenseCost { get; set; }
    public decimal ActualExpenseCost { get; set; }
    public decimal InventoryMovementCost { get; set; }
    public decimal TotalPlannedCost => PlannedResourceCost + PlannedExpenseCost;
    public decimal TotalActualCost => ActualResourceCost + ActualExpenseCost + InventoryMovementCost;
    public decimal CostVariance => TotalActualCost - TotalPlannedCost;
    public decimal TotalDistributedQuantity { get; set; }
    public decimal CostPerMeal => TotalDistributedQuantity <= 0 ? 0 : TotalActualCost / TotalDistributedQuantity;
}

public class ProjectHandoffDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectHandoffType HandoffType { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime HandoffDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public bool ConsumeReservedQuantity { get; set; }
    public List<ProjectHandoffLineDto> Lines { get; set; } = [];
}

public class ProjectHandoffLineDto
{
    public Guid Id { get; set; }
    public Guid HandoffId { get; set; }
    public Guid? AllocationId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid? InventoryMovementId { get; set; }
}

public class ProjectTaskLinkDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public string? TaskNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ProjectDistributionReportRowDto
{
    public string PeriodKey { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public Guid? ProjectCustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? DistributionPlaceId { get; set; }
    public string? PlaceName { get; set; }
    public Guid? DeliverableId { get; set; }
    public string? DeliverableName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal VarianceQuantity => ActualQuantity - PlannedQuantity;
}

public class PlannedProductDemandRowDto
{
    public string PeriodKey { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public Guid? ProjectCustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductId { get; set; }
    public string ProductSkuName { get; set; } = string.Empty;
    public string? ProductSkuNameEng { get; set; }
    public string? SkuCode { get; set; }
    public string? SkuCodeEng { get; set; }
    public Guid? ProductPackageId { get; set; }
    public string? PackageName { get; set; }
    public string? PackageNameEng { get; set; }
    public decimal PlannedQuantity { get; set; }
}
