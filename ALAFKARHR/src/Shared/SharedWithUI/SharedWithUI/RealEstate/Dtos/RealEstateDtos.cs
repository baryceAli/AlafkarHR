using System.ComponentModel.DataAnnotations;
using SharedWithUI.RealEstate.Enums;

namespace SharedWithUI.RealEstate.Dtos;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string NameEng { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public PropertyStatus Status { get; set; } = PropertyStatus.Draft;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Notes { get; set; }
    public int UnitsCount { get; set; }
}

public class PropertyUnitDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;

    [Required]
    public string UnitNumber { get; set; } = string.Empty;

    public string? Name { get; set; }
    public PropertyUnitType UnitType { get; set; } = PropertyUnitType.Apartment;
    public UnitStatus Status { get; set; } = UnitStatus.Available;
    public string? Floor { get; set; }
    public decimal? Area { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public string? Notes { get; set; }
}

public class LeaseDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public LeaseDirection Direction { get; set; }
    public LeaseStatus Status { get; set; } = LeaseStatus.Draft;
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public Guid? UnitId { get; set; }
    public string? UnitNumber { get; set; }
    public Guid ContractId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string PartyType { get; set; } = string.Empty;
    public Guid PartyId { get; set; }
    public string PartyDisplayName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddYears(1);
    public decimal RentAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public BillingFrequency BillingFrequency { get; set; } = BillingFrequency.Monthly;
    public int DueDay { get; set; } = 1;
    public int CustomIntervalMonths { get; set; } = 1;
    public int GraceDays { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? Notes { get; set; }
    public List<LeaseInstallmentDto> Installments { get; set; } = [];
}

public class LeaseInstallmentDto
{
    public Guid Id { get; set; }
    public Guid LeaseId { get; set; }
    public int Sequence { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public InstallmentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class RentPaymentAllocationDto
{
    public Guid InstallmentId { get; set; }
    public decimal Amount { get; set; }
}

public class RecordRentPaymentDto
{
    public Guid LeaseId { get; set; }
    public Guid? PaymentReferenceId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public List<RentPaymentAllocationDto> Allocations { get; set; } = [];
}

public class PropertyExpenseDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public Guid? UnitId { get; set; }
    public Guid? LeaseId { get; set; }
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public bool IsRecoverableFromTenant { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public string? Notes { get; set; }
}

public class UtilityAccountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid? UnitId { get; set; }
    public UtilityServiceType ServiceType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? MeterNumber { get; set; }
    public string? ProviderName { get; set; }
    public Guid? SupplierId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UtilityBillDto
{
    public Guid Id { get; set; }
    public Guid UtilityAccountId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid? UnitId { get; set; }
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public Guid? ExpenseId { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public class RealEstateDashboardDto
{
    public int Properties { get; set; }
    public int Units { get; set; }
    public int OccupiedUnits { get; set; }
    public int ActiveTenantLeases { get; set; }
    public decimal ExpectedRent { get; set; }
    public decimal CollectedRent { get; set; }
    public decimal OverdueRent { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
}

public class RealEstateReportsDto
{
    public List<PropertyProfitabilityDto> Profitability { get; set; } = [];
    public List<PropertyOccupancyDto> Occupancy { get; set; } = [];
    public List<LeaseInstallmentDto> OverdueInstallments { get; set; } = [];
    public List<LeaseInstallmentDto> UpcomingOwnerPayables { get; set; } = [];
    public List<UtilityBillDto> UtilityAging { get; set; } = [];
    public List<ExpenseSummaryDto> ExpenseSummary { get; set; } = [];
}

public class PropertyProfitabilityDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public decimal ExpectedRent { get; set; }
    public decimal CollectedRent { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
}

public class PropertyOccupancyDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int Units { get; set; }
    public int OccupiedUnits { get; set; }
    public decimal OccupancyRate { get; set; }
}

public class ExpenseSummaryDto
{
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
