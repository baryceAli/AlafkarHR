using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class EmergencyLeaveRequestConfiguration : IEntityTypeConfiguration<LegacyAttendanceEmergencyLeaveRequest>
{
    public void Configure(EntityTypeBuilder<LegacyAttendanceEmergencyLeaveRequest> builder)
    {
        builder.ToTable("EmergencyLeaveRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.AttachmentPath).HasMaxLength(500);
        builder.Property(x => x.ApproverUserId).HasMaxLength(100);
        builder.Property(x => x.ApproverComment).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.StartDate, x.EndDate });
        builder.HasIndex(x => new { x.EmployeeId, x.Status, x.StartDate });
    }
}
