using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class EmployeeLeaveBalanceConfiguration : IEntityTypeConfiguration<EmployeeLeaveBalance>
{
    public void Configure(EntityTypeBuilder<EmployeeLeaveBalance> builder)
    {
        builder.ToTable("EmployeeLeaveBalances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AnnualLeaveDays).HasPrecision(8, 2).IsRequired();
        builder.Property(x => x.MaxCarryForwardDays).HasPrecision(8, 2).IsRequired();
        builder.Property(x => x.CarriedForwardDays).HasPrecision(8, 2).IsRequired();
        builder.Property(x => x.TakenDays).HasPrecision(8, 2).IsRequired();
        builder.Ignore(x => x.AvailableDays);
        builder.Ignore(x => x.RemainingDays);
        builder.HasIndex(x => new { x.CompanyId, x.Year, x.EmployeeId }).IsUnique();
    }
}
