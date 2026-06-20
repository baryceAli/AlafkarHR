using Shared.DDD;

namespace LeaveManagement.Leave.Models;

public class EmployeeLeaveBalance : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public int Year { get; private set; }
    public decimal AnnualLeaveDays { get; private set; }
    public bool AllowCarryForward { get; private set; }
    public decimal MaxCarryForwardDays { get; private set; }
    public decimal CarriedForwardDays { get; private set; }
    public decimal TakenDays { get; private set; }

    public decimal AvailableDays => AnnualLeaveDays + CarriedForwardDays;
    public decimal RemainingDays => AvailableDays - TakenDays;

    private EmployeeLeaveBalance() { }

    public static EmployeeLeaveBalance Create(Guid id, UpsertEmployeeLeaveBalanceDto dto, decimal carriedForwardDays, string? modifiedBy)
    {
        Validate(dto.AnnualLeaveDays, dto.MaxCarryForwardDays, carriedForwardDays);

        return new EmployeeLeaveBalance
        {
            Id = id,
            EmployeeId = dto.EmployeeId,
            CompanyId = dto.CompanyId,
            Year = dto.Year,
            AnnualLeaveDays = dto.AnnualLeaveDays,
            AllowCarryForward = dto.AllowCarryForward,
            MaxCarryForwardDays = dto.MaxCarryForwardDays,
            CarriedForwardDays = dto.AllowCarryForward ? carriedForwardDays : 0,
            TakenDays = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = modifiedBy
        };
    }

    public void Update(UpsertEmployeeLeaveBalanceDto dto, decimal carriedForwardDays, string? modifiedBy)
    {
        if (EmployeeId != dto.EmployeeId || CompanyId != dto.CompanyId || Year != dto.Year)
        {
            throw new BadRequestException("Leave balance identity fields cannot be changed.");
        }

        Validate(dto.AnnualLeaveDays, dto.MaxCarryForwardDays, carriedForwardDays);

        AnnualLeaveDays = dto.AnnualLeaveDays;
        AllowCarryForward = dto.AllowCarryForward;
        MaxCarryForwardDays = dto.MaxCarryForwardDays;
        CarriedForwardDays = dto.AllowCarryForward ? carriedForwardDays : 0;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

        if (RemainingDays < 0)
        {
            throw new BadRequestException("Leave balance cannot be lower than the already taken leave days.");
        }
    }

    public void AddTakenDays(decimal days, string? modifiedBy)
    {
        if (days <= 0)
        {
            throw new BadRequestException("Taken leave days must be greater than zero.");
        }

        if (RemainingDays < days)
        {
            throw new BadRequestException("Employee does not have enough remaining leave days.");
        }

        TakenDays += days;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void RecalculateTakenDays(decimal takenDays, string? modifiedBy)
    {
        if (takenDays < 0)
        {
            throw new BadRequestException("Taken leave days cannot be negative.");
        }

        if (AvailableDays < takenDays)
        {
            throw new BadRequestException("Taken leave days cannot exceed available leave days.");
        }

        TakenDays = takenDays;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private static void Validate(decimal annualLeaveDays, decimal maxCarryForwardDays, decimal carriedForwardDays)
    {
        if (annualLeaveDays < 0)
        {
            throw new BadRequestException("Annual leave days cannot be negative.");
        }

        if (maxCarryForwardDays < 0)
        {
            throw new BadRequestException("Maximum carry-forward days cannot be negative.");
        }

        if (carriedForwardDays < 0)
        {
            throw new BadRequestException("Carried-forward days cannot be negative.");
        }
    }
}
