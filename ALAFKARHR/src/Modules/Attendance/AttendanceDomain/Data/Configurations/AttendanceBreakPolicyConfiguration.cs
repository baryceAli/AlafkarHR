using AttendanceDomain.Attendance.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceDomain.Data.Configurations;

public class AttendanceBreakPolicyConfiguration : IEntityTypeConfiguration<AttendanceBreakPolicy>
{
    public void Configure(EntityTypeBuilder<AttendanceBreakPolicy> builder)
    {
        builder.ToTable("AttendanceBreakPolicies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Scope).HasConversion<int>().IsRequired();
        builder.Property(x => x.BreakMode).HasConversion<int>().HasDefaultValue(AttendanceBreakMode.Flexible).IsRequired();
        builder.HasIndex(x => new { x.CompanyId, x.Scope, x.EmployeeId, x.DepartmentId, x.AdministrationId });
    }
}
