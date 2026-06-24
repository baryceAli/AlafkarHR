using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class ShiftSwapRequestConfiguration : IEntityTypeConfiguration<ShiftSwapRequest>
{
    public void Configure(EntityTypeBuilder<ShiftSwapRequest> builder)
    {
        builder.ToTable("ShiftSwapRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.ManagerNote).HasMaxLength(1000);
        builder.Property(x => x.ReviewedBy).HasMaxLength(450);
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => new { x.RequestingEmployeeId, x.WorkDate });
        builder.HasIndex(x => new { x.TargetEmployeeId, x.WorkDate });
    }
}
