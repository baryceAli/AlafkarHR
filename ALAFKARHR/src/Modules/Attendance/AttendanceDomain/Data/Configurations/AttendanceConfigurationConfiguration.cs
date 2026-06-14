using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceConfigurationConfiguration : IEntityTypeConfiguration<AttendanceConfiguration>
{
    public void Configure(EntityTypeBuilder<AttendanceConfiguration> builder)
    {
        builder.ToTable("AttendanceConfigurations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstDayOfWeek).HasConversion<int>().IsRequired();
        builder.Property(x => x.SundayIsWorkingDay).HasDefaultValue(true);
        builder.Property(x => x.MondayIsWorkingDay).HasDefaultValue(true);
        builder.Property(x => x.TuesdayIsWorkingDay).HasDefaultValue(true);
        builder.Property(x => x.WednesdayIsWorkingDay).HasDefaultValue(true);
        builder.Property(x => x.ThursdayIsWorkingDay).HasDefaultValue(true);
        builder.Property(x => x.FridayIsWorkingDay).HasDefaultValue(false);
        builder.Property(x => x.SaturdayIsWorkingDay).HasDefaultValue(false);
        builder.Property(x => x.WeekendDays).HasMaxLength(100).HasDefaultValue("Friday,Saturday");
        builder.HasIndex(x => x.CompanyId).IsUnique();
    }
}
