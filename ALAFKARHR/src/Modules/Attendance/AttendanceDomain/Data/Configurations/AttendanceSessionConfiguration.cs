using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceSessionConfiguration : IEntityTypeConfiguration<AttendanceSession>
{
    public void Configure(EntityTypeBuilder<AttendanceSession> builder)
    {
        builder.ToTable("AttendanceSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttendanceType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.TotalHours).HasColumnType("decimal(10,2)").HasDefaultValue(0);
        builder.Property(x => x.TotalDistanceKm).HasColumnType("decimal(10,3)").HasDefaultValue(0);

        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => new { x.EmployeeId, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.ShiftStart });
    }
}
