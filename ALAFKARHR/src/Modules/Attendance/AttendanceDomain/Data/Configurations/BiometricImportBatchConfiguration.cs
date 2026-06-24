using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class BiometricImportBatchConfiguration : IEntityTypeConfiguration<BiometricImportBatch>
{
    public void Configure(EntityTypeBuilder<BiometricImportBatch> builder)
    {
        builder.ToTable("BiometricImportBatches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => x.ImportedAtUtc);
    }
}
