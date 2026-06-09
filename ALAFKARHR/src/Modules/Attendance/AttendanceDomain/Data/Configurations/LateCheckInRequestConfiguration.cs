using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class LateCheckInRequestConfiguration : IEntityTypeConfiguration<LateCheckInRequest>
{
    public void Configure(EntityTypeBuilder<LateCheckInRequest> builder)
    {
        builder.ToTable("LateCheckInRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttendanceType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.ManagerNote).HasMaxLength(1000);
        builder.Property(x => x.ReviewedBy).HasMaxLength(100);

        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.ShiftId);
        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => new { x.Status, x.RequestedCheckInTimeUtc });
    }
}
