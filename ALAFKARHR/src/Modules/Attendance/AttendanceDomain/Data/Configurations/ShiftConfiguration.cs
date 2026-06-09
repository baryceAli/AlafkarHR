using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;


public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.GracePeriodMinutes).HasDefaultValue(15);
        builder.Property(x => x.LateAfterMinutes).HasDefaultValue(15);
        builder.Property(x => x.ProhibitCheckInAfterMinutes).HasDefaultValue(120);
        builder.Property(x => x.BreakMinutes).HasDefaultValue(0);

        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}
