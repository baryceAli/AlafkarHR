using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class EmployeeShiftConfiguration : IEntityTypeConfiguration<EmployeeShift>
{
    public void Configure(EntityTypeBuilder<EmployeeShift> builder)
    {
        builder.ToTable("EmployeeShifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Scope)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.EffectiveFrom).IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasIndex(x => new { x.EmployeeId, x.IsActive, x.EffectiveFrom });
        builder.HasIndex(x => new { x.DepartmentId, x.IsActive, x.EffectiveFrom });
        builder.HasIndex(x => new { x.AdministrationId, x.IsActive, x.EffectiveFrom });
        builder.HasIndex(x => new { x.CompanyId, x.Scope, x.IsActive, x.EffectiveFrom });
    }
}
