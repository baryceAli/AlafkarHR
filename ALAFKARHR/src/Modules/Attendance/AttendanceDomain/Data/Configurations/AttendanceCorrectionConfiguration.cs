using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceCorrectionConfiguration : IEntityTypeConfiguration<AttendanceCorrection>
{
    public void Configure(EntityTypeBuilder<AttendanceCorrection> builder)
    {
        builder.ToTable("AttendanceCorrections");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.ManagerNote).HasMaxLength(1000);
        builder.Property(x => x.ReviewedBy).HasMaxLength(450);
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => new { x.EmployeeId, x.WorkDate });
        builder.HasIndex(x => x.SessionId);
    }
}
