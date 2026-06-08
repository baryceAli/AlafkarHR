using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceCheckInConfiguration : IEntityTypeConfiguration<AttendanceCheckIn>
{
    public void Configure(EntityTypeBuilder<AttendanceCheckIn> builder)
    {
        builder.ToTable("AttendanceCheckIns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SiteName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => new { x.EmployeeId, x.ArrivedAtUtc });
        builder.HasIndex(x => x.ClientCheckInId).IsUnique().HasFilter("[ClientCheckInId] IS NOT NULL");
    }
}
