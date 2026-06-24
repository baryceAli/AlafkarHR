using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceWorkEntryConfiguration : IEntityTypeConfiguration<AttendanceWorkEntry>
{
    public void Configure(EntityTypeBuilder<AttendanceWorkEntry> builder)
    {
        builder.ToTable("AttendanceWorkEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Hours).HasColumnType("decimal(10,2)").HasDefaultValue(0);
        builder.Property(x => x.SourceModule).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ApprovedBy).HasMaxLength(450);
        builder.Property(x => x.LockedBy).HasMaxLength(450);
        builder.HasIndex(x => new { x.CompanyId, x.WorkDate });
        builder.HasIndex(x => new { x.EmployeeId, x.WorkDate });
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => new { x.SourceModule, x.SourceDocumentId });
    }
}
