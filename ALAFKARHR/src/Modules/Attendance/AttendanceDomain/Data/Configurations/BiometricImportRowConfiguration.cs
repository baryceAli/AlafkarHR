using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class BiometricImportRowConfiguration : IEntityTypeConfiguration<BiometricImportRow>
{
    public void Configure(EntityTypeBuilder<BiometricImportRow> builder)
    {
        builder.ToTable("BiometricImportRows");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeviceEmployeeCode).HasMaxLength(100);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.HasIndex(x => new { x.BatchId, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.PunchTimeUtc });
        builder.HasIndex(x => x.EmployeeId);
    }
}
