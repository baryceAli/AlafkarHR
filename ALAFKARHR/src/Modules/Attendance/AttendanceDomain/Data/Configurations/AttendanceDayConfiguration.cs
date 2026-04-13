using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;


public class AttendanceDayConfiguration : IEntityTypeConfiguration<AttendanceDay>
{
    public void Configure(EntityTypeBuilder<AttendanceDay> builder)
    {
        builder.ToTable("AttendanceDays");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkedMinutes).HasDefaultValue(0);
        builder.Property(x => x.LateMinutes).HasDefaultValue(0);
        builder.Property(x => x.OvertimeMinutes).HasDefaultValue(0);

        builder.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
        builder.HasIndex(x => x.CompanyId);
    }
}