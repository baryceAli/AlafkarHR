using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceRosterSubstituteConfigurationConfiguration : IEntityTypeConfiguration<AttendanceRosterSubstituteConfiguration>
{
    public void Configure(EntityTypeBuilder<AttendanceRosterSubstituteConfiguration> builder)
    {
        builder.ToTable("AttendanceRosterSubstituteConfigurations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.IsDeleted }).IsUnique();
    }
}
