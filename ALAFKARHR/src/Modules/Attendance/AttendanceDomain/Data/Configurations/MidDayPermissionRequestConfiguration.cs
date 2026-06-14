using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class MidDayPermissionRequestConfiguration : IEntityTypeConfiguration<MidDayPermissionRequest>
{
    public void Configure(EntityTypeBuilder<MidDayPermissionRequest> builder)
    {
        builder.ToTable("MidDayPermissionRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.ApproverUserId).HasMaxLength(100);
        builder.Property(x => x.ApproverComment).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.Date });
        builder.HasIndex(x => new { x.EmployeeId, x.Status, x.Date });
    }
}
