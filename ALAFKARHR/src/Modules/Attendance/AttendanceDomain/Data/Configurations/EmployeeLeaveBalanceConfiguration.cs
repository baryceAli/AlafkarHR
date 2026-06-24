using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class EmployeeLeaveBalanceConfiguration : IEntityTypeConfiguration<LegacyAttendanceEmployeeLeaveBalance>
{
    public void Configure(EntityTypeBuilder<LegacyAttendanceEmployeeLeaveBalance> builder)
    {
        builder.ToTable("EmployeeLeaveBalances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AnnualLeaveDays).HasPrecision(8, 2);
        builder.Property(x => x.TakenDays).HasPrecision(8, 2);
        builder.Property(x => x.CarriedForwardDays).HasPrecision(8, 2);
        builder.Property(x => x.MaxCarryForwardDays).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.CompanyId, x.Year, x.EmployeeId }).IsUnique();
    }
}
