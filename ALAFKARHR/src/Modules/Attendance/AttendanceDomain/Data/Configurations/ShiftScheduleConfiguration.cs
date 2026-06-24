using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class ShiftScheduleConfiguration : IEntityTypeConfiguration<ShiftSchedule>
{
    public void Configure(EntityTypeBuilder<ShiftSchedule> builder)
    {
        builder.ToTable("ShiftSchedules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.PublishedBy).HasMaxLength(450);
        builder.Property(x => x.LockedBy).HasMaxLength(450);
        builder.HasIndex(x => new { x.CompanyId, x.StartDate, x.EndDate });
        builder.HasIndex(x => new { x.CompanyId, x.Status });
    }
}
