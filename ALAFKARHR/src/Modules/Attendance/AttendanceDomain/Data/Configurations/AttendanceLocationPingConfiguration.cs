using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceLocationPingConfiguration : IEntityTypeConfiguration<AttendanceLocationPing>
{
    public void Configure(EntityTypeBuilder<AttendanceLocationPing> builder)
    {
        builder.ToTable("AttendanceLocationPings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Latitude).IsRequired();
        builder.Property(x => x.Longitude).IsRequired();

        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => new { x.EmployeeId, x.RecordedAtUtc });
        builder.HasIndex(x => x.ClientPingId).IsUnique().HasFilter("[ClientPingId] IS NOT NULL");
    }
}
